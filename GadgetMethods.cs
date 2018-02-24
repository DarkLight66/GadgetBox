using System;
using System.Collections.Generic;
using GadgetBox.Prefixes;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace GadgetBox
{
	public static class GadgetMethods
	{
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

		public static void PrefixHeldItem(this Player player)
		{
			Item toPrefix = player.inventory[player.selectedItem];
			bool favorited = toPrefix.favorited;
			int stack = toPrefix.stack;
			Item item = new Item();
			item.netDefaults(toPrefix.netID);
			item = item.CloneWithModdedData(Main.reforgeItem);
			item.Prefix(-2);
			toPrefix = player.inventory[player.selectedItem] = item.Clone();
			toPrefix.Center = player.Center;
			toPrefix.favorited = favorited;
			toPrefix.stack = stack;
			ItemLoader.PostReforge(toPrefix);
			ItemText.NewText(toPrefix, toPrefix.stack, true, false);
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

		internal static void AddToolPrefix(this Mod mod, ToolPrefixType prefixType, float damageMult = 1f, float knockbackMult = 1f, float useTimeMult = 1f, int critBonus = 0, int tileBoost = 0)
		{
			mod.AddPrefix(prefixType.ToString(), new ToolPrefix(damageMult, knockbackMult, useTimeMult, critBonus, tileBoost));
			ToolPrefix.ToolPrefixes.Add((ToolPrefix)mod.GetPrefix(prefixType.ToString()));
		}
	}
}