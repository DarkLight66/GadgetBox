using System;
using System.Collections.Generic;
using GadgetBox.Prefixes;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace GadgetBox
{
	public static class GadgetMethods
	{
		public static GadgetPlayer Gadget(this Player player) => player.GetModPlayer<GadgetPlayer>();

		public static void Bounce(this Projectile projectile, Vector2 oldVelocity, float bouncyness = 1f)
		{
			if (projectile.velocity.X != oldVelocity.X)
				projectile.velocity.X = -oldVelocity.X * bouncyness;
			if (projectile.velocity.Y != oldVelocity.Y)
				projectile.velocity.Y = -oldVelocity.Y * bouncyness;
		}

		public static void CatchNPC(int npcIndex, int who = -1, bool fromMelee = true)
		{
			NPC npc = Main.npc[npcIndex];
			if (!npc.active)
				return;
			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				ModPacket packet = GadgetBox.Instance.GetPacket(MessageType.CatchNPC, 2);
				packet.Write((byte)npcIndex);
				packet.Write(fromMelee);
				packet.Send();
				npc.active = false;
				return;
			}
			if (npc.catchItem > 0)
			{
				npc.active = false;
				NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npcIndex);
				if (npc.SpawnedFromStatue)
				{
					Vector2 position = npc.Center - new Vector2(20);
					Utils.PoofOfSmoke(position);
					if (Main.netMode == NetmodeID.Server)
					{
						NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npcIndex);
						NetMessage.SendData(MessageID.PoofOfSmoke, -1, -1, null, (int)position.X, position.Y);
					}
					return;
				}
				if (fromMelee)
					Item.NewItem(Main.player[who].Center, 0, 0, npc.catchItem, 1, false, 0, true);
				else
					Item.NewItem(npc.Center, 0, 0, npc.catchItem);
			}
		}

		public static string GetTextValue(this Mod mod, string key) => Language.GetTextValue($"Mods.{mod.Name}.{key}");

		public static void CloseVanillaUIs(this Player player)
		{
			if (player.sign >= 0 || player.talkNPC >= 0)
			{
				player.talkNPC = -1;
				player.sign = -1;
				Main.npcChatCornerItem = 0;
				Main.editSign = false;
				Main.npcChatText = "";
				player.releaseMount = false;
			}
			if (Main.editChest)
			{
				Main.PlaySound(SoundID.MenuTick);
				Main.editChest = false;
				Main.npcChatText = "";
			}
			if (player.editedChestName)
			{
				NetMessage.SendData(MessageID.SyncPlayerChest, -1, -1, NetworkText.FromLiteral(Main.chest[player.chest].name), player.chest, 1f);
				player.editedChestName = false;
			}
			if (player.chest != -1)
			{
				player.chest = -1;
				player.flyingPigChest = -1;
				Recipe.FindRecipes();
			}
		}

		public static int ReforgePrice(this Item item, Player player = null)
		{
			if (item == null || item.IsAir)
				return 0;
			if (player == null)
				player = Main.LocalPlayer;
			int reforgePrice = item.value;
			bool canApplyDiscount = true;
			if (ItemLoader.ReforgePrice(item, ref reforgePrice, ref canApplyDiscount))
			{
				if (canApplyDiscount && player.discount)
					reforgePrice = (int)(reforgePrice * 0.8f);
				reforgePrice /= 3;
			}
			return reforgePrice;
		}

		public static void Consume(this Item item, int amount = 1, bool checkConsumable = true)
		{
			if (checkConsumable && !item.consumable)
				return;
			item.stack -= amount;
			if (item.stack <= 0)
				item.TurnToAir();
		}

		public static void PrefixItem(ref Item item, bool silent = false)
		{
			bool favorited = item.favorited;
			int stack = item.stack;
			Item tempItem = new Item();
			tempItem.netDefaults(item.netID);
			tempItem = tempItem.CloneWithModdedDataFrom(item);
			tempItem.Prefix(-2);
			item = tempItem.Clone();
			item.Center = Main.LocalPlayer.Center;
			item.favorited = favorited;
			item.stack = stack;
			ItemLoader.PostReforge(item);
			if (silent) return;
			ItemText.NewText(item, item.stack, true, false);
			Main.PlaySound(SoundID.Item37);
		}

		public static List<Tuple<Point16, ushort>> TilesHit(Vector2 Position, Vector2 Velocity, int Width, int Height)
		{
			Vector2 vector = Position + Velocity;
			int num = (int)(Position.X / 16f) - 1;
			int num2 = (int)((Position.X + Width) / 16f) + 2;
			int num3 = (int)(Position.Y / 16f) - 1;
			int num4 = (int)((Position.Y + Height) / 16f) + 2;
			if (num < 0)
				num = 0;
			if (num2 > Main.maxTilesX)
				num2 = Main.maxTilesX;
			if (num3 < 0)
				num3 = 0;
			if (num4 > Main.maxTilesY)
				num4 = Main.maxTilesY;

			List<Tuple<Point16, ushort>> tiles = new List<Tuple<Point16, ushort>>();

			for (int i = num; i < num2; i++)
			{
				for (int j = num3; j < num4; j++)
				{
					if (Main.tile[i, j] != null && !Main.tile[i, j].inActive() && Main.tile[i, j].active() && (Main.tileSolid[Main.tile[i, j].type] || (Main.tileSolidTop[Main.tile[i, j].type] && Main.tile[i, j].frameY == 0)))
					{
						Vector2 vector2 = new Vector2(i * 16, j * 16);
						int num5 = 16;
						if (Main.tile[i, j].halfBrick())
						{
							vector2.Y += 8f;
							num5 -= 8;
						}
						if (vector.X + Width >= vector2.X && vector.X <= vector2.X + 16f && vector.Y + Height >= vector2.Y && vector.Y <= vector2.Y + num5)
						{
							tiles.Add(new Tuple<Point16, ushort>(new Point16(i, j), Main.tile[i, j].type));
						}
					}
				}
			}
			return tiles;
		}

		public static bool TileCheck(int x, int y, bool tile = false, ushort tileType = 0, bool checkWall = false, ushort wallType = 0)
		{
			Tile sTile = Framing.GetTileSafely(x, y);
			return (!checkWall || sTile.wall == wallType) && (!sTile.active() || tile && sTile.type == tileType);
		}

		public static bool TileAreaCheck(int x, int y, int width, int height, bool tile = false, ushort tileType = 0, bool wall = false, ushort wallType = 0)
		{
			if (width == 0 || height == 0)
				return false;

			for (int i = x; i != x + width; i += Math.Sign(width))
			{
				for (int j = y; j != y + height; j += Math.Sign(height))
				{
					if (TileCheck(i, j, tile, tileType, wall, wallType))
						continue;
					return false;
				}
			}
			return true;
		}

		public static byte GetRandomWireColor(int x, int y, byte defColor)
		{
			List<byte> colors = new List<byte>(4);

			if (Main.tile[x, y].wire())
				colors.Add(1);
			if (Main.tile[x, y].wire2())
				colors.Add(2);
			if (Main.tile[x, y].wire3())
				colors.Add(3);
			if (Main.tile[x, y].wire4())
				colors.Add(4);
			if (colors.Count == 0) return defColor;
			return colors[WorldGen.genRand.Next(colors.Count)];
		}

		public static bool PlaceWire(int x, int y, byte color, bool mute = true)
		{
			ushort comp = (ushort)(128 * (color == 3 ? 4 : color));
			if (color > 0 && color < 4 && ((Main.tile[x, y].sTileHeader & comp) != comp))
				Main.tile[x, y].sTileHeader |= comp;
			else if (color == 4 && ((Main.tile[x, y].bTileHeader & 128) != 128))
				Main.tile[x, y].bTileHeader |= 128;
			else
				return false;
			if (!mute) Main.PlaySound(0, x * 16 + 8, y * 16 + 8);
			return true;
		}

		public static bool HasWire(int x, int y) => Main.tile[x, y] != null && (Main.tile[x, y].wire() || Main.tile[x, y].wire2() || Main.tile[x, y].wire3() || Main.tile[x, y].wire4());

	}
}