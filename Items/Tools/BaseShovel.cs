using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Achievements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;

namespace GadgetBox.Items.Tools
{
	public abstract class BaseShovel : ModItem
	{
		public int shovel = 0;
		private const int UseStyleShovel = 250760; // Shovel Knight Treasure Trove steam AppID
		public override bool CloneNewInstances => true;

		public sealed override void SetDefaults()
		{
			SetShovelDefaults();
			item.useStyle = UseStyleShovel; // Needed so the use style can actually be changed
			item.melee = true;
			item.pick = 0; // Shovels can't have pickaxe power
		}

		public virtual void SetShovelDefaults() { }

		public override bool CanUseItem(Player player)
		{
			if (player.noItems)
				return false;
			if (shovel > 0)
				player.toolTime = 1;
			return true;
		}

		public override void HoldItem(Player player)
		{
			if (player.noBuilding || player.whoAmI != Main.myPlayer || shovel <= 0)
				return;
			bool withinReach = player.position.X / 16f - Player.tileRangeX - item.tileBoost <= Player.tileTargetX && (player.position.X + player.width) / 16f + Player.tileRangeX + item.tileBoost - 1f >= Player.tileTargetX && player.position.Y / 16f - Player.tileRangeY - item.tileBoost <= Player.tileTargetY && (player.position.Y + player.height) / 16f + Player.tileRangeY + item.tileBoost - 2f >= Player.tileTargetY;
			if (!withinReach)
				return;
			if (!Main.GamepadDisableCursorItemIcon)
			{
				player.showItemIcon = true;
				Main.ItemIconCacheUpdate(item.type);
			}
			Tile tile = Main.tile[Player.tileTargetX, Player.tileTargetY];
			if (tile.active() && !Main.tileAxe[tile.type] && !Main.tileHammer[tile.type])
			{
				if (player.toolTime == 0 && player.itemAnimation > 0 && player.controlUseItem)
				{
					PickTile(player, Player.tileTargetX, Player.tileTargetY, shovel);
					player.itemTime = (int)(item.useTime * player.pickSpeed / PlayerHooks.TotalUseTimeMultiplier(player, item));
				}
			}
		}

		// Adapted from the vanilla Player class method
		public void PickTile(Player player, int x, int y, int shovelPower)
		{
			int tileId = player.hitTile.HitObject(x, y, 1);
			Tile tile = Main.tile[x, y];
			int actualPower = ShovelMethods.DigPower(tile.type, shovelPower);
			if (actualPower == 0)
			{
				player.PickTile(x, y, Math.Max(shovelPower / 8, 1));
				return;
			}
			if (!WorldGen.CanKillTile(x, y))
				actualPower = 0;
			if (player.hitTile.AddDamage(tileId, actualPower, true) >= 100)
			{
				AchievementsHelper.CurrentlyMining = true;
				player.hitTile.Clear(tileId);
				bool wasActive = tile.active();
				WorldGen.KillTile(x, y, false, false, false);
				if (wasActive && !tile.active())
					AchievementsHelper.HandleMining();
				AchievementsHelper.CurrentlyMining = false;
			}
			else
				WorldGen.KillTile(x, y, true, false, false);
			if (Main.netMode == NetmodeID.MultiplayerClient)
				NetMessage.SendData(MessageID.TileChange, -1, -1, null, 0, x, y);
			if (actualPower != 0)
				player.hitTile.Prune();
		}

		public override bool UseItemFrame(Player player)
		{
			if (player.itemAnimation < player.itemAnimationMax * 0.222f)
				player.bodyFrame.Y = player.bodyFrame.Height;
			else if (player.itemAnimation < player.itemAnimationMax * 0.555f)
				player.bodyFrame.Y = player.bodyFrame.Height * 2;
			else if (player.itemAnimation < player.itemAnimationMax * 0.888f)
				player.bodyFrame.Y = player.bodyFrame.Height * 3;
			else
				player.bodyFrame.Y = player.bodyFrame.Height * 4;
			return true;
		}

		public override void UseItemHitbox(Player player, ref Rectangle hitbox, ref bool noHitbox)
		{
			if (player.itemAnimation < player.itemAnimationMax * 0.222f)
			{
				hitbox.X -= (int)(hitbox.Width * (player.direction == 1 ? 0.8f : -0.7f));
				hitbox.Width = (int)(hitbox.Width * 1.2f);
				hitbox.Y -= (int)((hitbox.Height * 1.4f - hitbox.Height) * player.gravDir);
				hitbox.Height = (int)(hitbox.Height * 1.4f);
			}
			else if (player.itemAnimation < player.itemAnimationMax * 0.555f)
			{
				Point offset = (hitbox.Size() * 0.2f).ToPoint();
				hitbox.Inflate(offset.X, offset.Y);
				hitbox.Offset(offset.X / 2 * player.direction, -offset.Y / 2);
			}
			else if (player.itemAnimation < player.itemAnimationMax * 0.888f)
			{
				if (player.direction == -1)
				{
					hitbox.X -= (int)(hitbox.Width * 1.4f - hitbox.Width);
				}
				hitbox.Width = (int)(hitbox.Width * 1.4f);
				hitbox.Y += (int)(hitbox.Height * 0.5f * player.gravDir);
				hitbox.Height = (int)(hitbox.Height * 1.3f);
			}
			else
			{
				hitbox.Y += (int)(hitbox.Height * player.gravDir);
				hitbox.Height = (int)(hitbox.Height * 1.3f);
			}
		}

		//Copied from the vanilla brodasword usestyle (1) with a few modification to make it work in reverse.
		public override void UseStyle(Player player)
		{
			int mountOffset = player.mount.PlayerOffsetHitbox;
			int width = Main.itemTexture[item.type].Width;
			int height = Main.itemTexture[item.type].Height;
			byte widthOffset = 10, heightOffset = 10;
			if (player.itemAnimation < player.itemAnimationMax * 0.222f)
			{
				widthOffset = 6;
				if (width > 32)
					widthOffset = 10;
				if (width >= 48)
					widthOffset = 14;
				if (width >= 52)
					widthOffset = 20;
				if (width >= 64)
					widthOffset = 26;
				if (width >= 92)
					widthOffset = 34;
				player.itemLocation.X = player.position.X + player.width * 0.5f - (width * 0.5f - widthOffset) * player.direction;
				if (height > 52)
					heightOffset = 12;
				if (height > 64)
					heightOffset = 14;
				player.itemLocation.Y = player.position.Y + heightOffset + mountOffset;
			}
			else if (player.itemAnimation < player.itemAnimationMax * 0.555f)
			{
				if (width > 32)
					widthOffset = 14;
				if (width >= 52)
					widthOffset = 20;
				if (width >= 64)
					widthOffset = 26;
				if (width >= 92)
					widthOffset = 34;
				player.itemLocation.X = player.position.X + player.width * 0.5f + (width * 0.5f - widthOffset) * player.direction;
				if (height > 32)
					heightOffset = 8;
				if (height > 52)
					heightOffset = 12;
				if (height > 64)
					heightOffset = 14;
				player.itemLocation.Y = player.position.Y + heightOffset + mountOffset;
			}
			else
			{
				if (width > 32)
					widthOffset = 14;
				if (width >= 52)
					widthOffset = 24;
				if (width >= 64)
					widthOffset = 28;
				if (width >= 92)
					widthOffset = 38;
				player.itemLocation.X = player.position.X + player.width * 0.5f + (width * 0.5f - widthOffset) * player.direction;
				player.itemLocation.Y = player.position.Y + 24 + mountOffset;
			}
			player.itemRotation = ((float)(player.itemAnimationMax - player.itemAnimation) / player.itemAnimationMax - 0.7f) * -player.direction * 3.5f - player.direction * 0.3f;
			if (player.gravDir == -1f)
			{
				player.itemRotation = -player.itemRotation;
				player.itemLocation.Y = player.position.Y + player.height + (player.position.Y - player.itemLocation.Y);
			}
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			if (shovel <= 0)
				return;
			int ttindex = tooltips.FindLastIndex(t => t.mod == "Terraria" && (t.Name == "Knockback" || t.Name == "Speed"));
			if (ttindex != -1)
				tooltips.Insert(ttindex + 1, new TooltipLine(mod, "ShovelPower", shovel + mod.GetTextValue("Misc.ShovelPower")));
		}
	}

	public class AShovelSmartCursor : ModPlayer
	{
		// Reset effects happens almost right after the vanilla SmarCursorLookup
		public override void ResetEffects() => ShovelMethods.SmartCursorLookup(player);
	}

	public class AShovelSmartSelect : GlobalTile
	{
		public override bool AutoSelect(int i, int j, int type, Item item)
		{
			BaseShovel myShovel = item.modItem as BaseShovel;
			return myShovel != null && ShovelMethods.DigPower((ushort)type, myShovel.shovel) >= 100;
		}
	}

	public static class ShovelMethods
	{
		public static bool CanDigTile(ushort type, int shovelPower = 50) => DigPower(type, shovelPower) > 0;

		public static int DigPower(ushort type, int shovelPower = 50)
		{
			List<int> weakBlocks = new List<int>()
			{
				TileID.Dirt,
				TileID.ClayBlock,
				TileID.Sand,
				TileID.HardenedSand,
				TileID.Silt,
				TileID.Slush,
				TileID.Ash
			};
			int actualPower = 0;
			if (Main.tileNoFail[type])
				actualPower = 100;
			else if (weakBlocks.Contains(type) || TileID.Sets.Grass[type] || TileID.Sets.GrassSpecial[type] || TileID.Sets.Snow[type] ||
				 TileID.Sets.Mud[type] || TileID.Sets.Leaves[type] || TileID.Sets.NeedsGrassFraming[type])
				actualPower = shovelPower * 2;
			else if (TileID.Sets.Falling[type] || TileID.Sets.Conversion.HardenedSand[type] || TileID.Sets.Conversion.Sandstone[type])
				actualPower = shovelPower;
			else if (type == TileID.Stone)
				actualPower = shovelPower / 2;
			if (TileID.Sets.Conversion.Stone[type] || TileID.Sets.Conversion.Ice[type] || TileID.Sets.Conversion.Moss[type])
				actualPower = shovelPower / 3;
			return actualPower;
		}

		// Adapted from the vanilla Player class method
		public static void SmartCursorLookup(Player player)
		{
			if (player.whoAmI != Main.myPlayer || !Main.SmartCursorEnabled || (Main.SmartCursorShowing && !Player.SmartCursorSettings.SmartAxeAfterPickaxe) || Main.SmartInteractShowingGenuine)
				return;
			try
			{
				Item item = player.inventory[player.selectedItem];
				if (!(item.modItem is BaseShovel) || ((BaseShovel)item.modItem).shovel <= 0)
					return;
				Vector2 MousePoss = Main.ReverseGravitySupport(Main.MouseScreen) + Main.screenPosition;
				int x = Utils.Clamp((int)(MousePoss.X / 16), 10, Main.maxTilesX - 10);
				int y = Utils.Clamp((int)(MousePoss.Y / 16), 10, Main.maxTilesY - 10);
				if (Main.tile[x, y] == null)
					return;
				bool DisableSmart = false;
				if (Main.tile[x, y].active())
					DisableSmart = CanDigTile(Main.tile[x, y].type);
				TileLoader.DisableSmartCursor(Main.tile[x, y], ref DisableSmart);
				int tileBoost = item.tileBoost;
				int minX = (int)(player.position.X / 16) - Player.tileRangeX - tileBoost + 1;
				int maxX = (int)((player.position.X + player.width) / 16) + Player.tileRangeX + tileBoost - 1;
				int minY = (int)(player.position.Y / 16) - Player.tileRangeY - tileBoost + 1;
				int maxY = (int)((player.position.Y + player.height) / 16) + Player.tileRangeY + tileBoost - 2;
				minX = Utils.Clamp(minX, 10, Main.maxTilesX - 10);
				maxX = Utils.Clamp(maxX, 10, Main.maxTilesX - 10);
				minY = Utils.Clamp(minY, 10, Main.maxTilesY - 10);
				maxY = Utils.Clamp(maxY, 10, Main.maxTilesY - 10);
				if (DisableSmart && x >= minX && x <= maxX && y >= minY && y <= maxY)
					return;
				List<Tuple<int, int>> grapledTiles = new List<Tuple<int, int>>();
				for (int i = 0; i < player.grapCount; i++)
				{
					Projectile projectile = Main.projectile[player.grappling[i]];
					grapledTiles.Add(new Tuple<int, int>((int)(projectile.Center.X / 16), (int)(projectile.Center.Y / 16)));
				}
				int smartX = -1;
				int smartY = -1;
				if (PlayerInput.UsingGamepad)
				{
					Vector2 navigatorDirections = PlayerInput.Triggers.Current.GetNavigatorDirections();
					Vector2 gamepadThumbstickLeft = PlayerInput.GamepadThumbstickLeft;
					Vector2 gamepadThumbstickRight = PlayerInput.GamepadThumbstickRight;
					if (navigatorDirections == Vector2.Zero && gamepadThumbstickLeft.Length() < 0.05f && gamepadThumbstickRight.Length() < 0.05f)
						MousePoss = player.Center + new Vector2(player.direction * 1000, 0f);
				}
				Vector2 MouseDir = MousePoss - player.Center;
				int leftOrRight = Math.Sign(MouseDir.X);
				int avobeOrBelow = Math.Sign(MouseDir.Y);
				if (Math.Abs(MouseDir.X) > Math.Abs(MouseDir.Y) * 3f)
				{
					avobeOrBelow = 0;
					MousePoss.Y = player.Center.Y;
				}
				if (Math.Abs(MouseDir.Y) > Math.Abs(MouseDir.X) * 3f)
				{
					leftOrRight = 0;
					MousePoss.X = player.Center.X;
				}
				int playerTileX = (int)(player.Center.X / 16);
				int playerTileY = (int)(player.Center.Y / 16);
				List<Tuple<int, int>> startTiles = new List<Tuple<int, int>>();
				List<Tuple<int, int>> endTiles = new List<Tuple<int, int>>();
				int notUpperCorners = avobeOrBelow == -1 && leftOrRight != 0 ? -1 : 1;
				int sideSearchX = (int)((player.position.X + player.width / 2 + ((player.width / 2 - 1) * leftOrRight)) / 16);
				int topSearchY = (int)(notUpperCorners == -1 ? (player.position.Y + player.height - 1f) / 16 : (player.position.Y + 0.1f) / 16);
				int widthInTiles = player.width / 16 + ((player.width % 16 == 0) ? 0 : 1);
				int heightInTiles = player.height / 16 + ((player.height % 16 == 0) ? 0 : 1);
				if (leftOrRight != 0)
				{
					for (int i = 0; i < heightInTiles; i++)
					{
						if (Main.tile[sideSearchX, topSearchY + i * notUpperCorners] == null)
							return;
						startTiles.Add(new Tuple<int, int>(sideSearchX, topSearchY + i * notUpperCorners));
					}
				}
				if (avobeOrBelow != 0)
				{
					for (int i = 0; i < widthInTiles; i++)
					{
						if (Main.tile[(int)(player.position.X / 16f) + i, topSearchY] == null)
							return;
						startTiles.Add(new Tuple<int, int>((int)(player.position.X / 16f) + i, topSearchY));
					}
				}
				int endX = (int)((MousePoss.X + ((player.width / 2 - 1) * leftOrRight)) / 16);
				int endY = (int)((MousePoss.Y + 0.1f - (player.height / 2 + 1)) / 16);
				if (notUpperCorners == -1)
					endY = (int)((MousePoss.Y + (player.height / 2) - 1f) / 16);
				if (player.gravDir == -1f && avobeOrBelow == 0)
					endY++;
				if (endY < 10)
					endY = 10;
				if (endY > Main.maxTilesY - 10)
					endY = Main.maxTilesY - 10;
				if (leftOrRight != 0)
				{
					for (int i = 0; i < heightInTiles; i++)
					{
						if (Main.tile[endX, endY + i * notUpperCorners] == null)
							return;
						endTiles.Add(new Tuple<int, int>(endX, endY + i * notUpperCorners));
					}
				}
				if (avobeOrBelow != 0)
				{
					for (int i = 0; i < widthInTiles; i++)
					{
						if (Main.tile[(int)((MousePoss.X - (player.width / 2)) / 16f) + i, endY] == null)
							return;
						endTiles.Add(new Tuple<int, int>((int)((MousePoss.X - (player.width / 2)) / 16f) + i, endY));
					}
				}
				List<Tuple<int, int>> validTiles = new List<Tuple<int, int>>();
				while (startTiles.Count > 0)
				{
					Tuple<int, int> tuple = startTiles[0];
					Tuple<int, int> tuple2 = endTiles[0];
					Tuple<int, int> tuple3;
					if (!Collision.TupleHitLine(tuple.Item1, tuple.Item2, tuple2.Item1, tuple2.Item2, leftOrRight * (int)player.gravDir, -avobeOrBelow * (int)player.gravDir, grapledTiles, out tuple3))
					{
						startTiles.Remove(tuple);
						endTiles.Remove(tuple2);
					}
					else
					{
						if (tuple3.Item1 != tuple2.Item1 || tuple3.Item2 != tuple2.Item2)
							validTiles.Add(tuple3);
						Tile tile2 = Main.tile[tuple3.Item1, tuple3.Item2];
						if (!tile2.inActive() && tile2.active() && Main.tileSolid[tile2.type] && !Main.tileSolidTop[tile2.type] && !grapledTiles.Contains(tuple3))
							validTiles.Add(tuple3);
						startTiles.Remove(tuple);
						endTiles.Remove(tuple2);
					}
				}
				List<Tuple<int, int>> tilesToRemove = new List<Tuple<int, int>>();
				for (int i = 0; i < validTiles.Count; i++)
					if (DigPower(Main.tile[validTiles[i].Item1, validTiles[i].Item2].type) < 100 || !WorldGen.CanKillTile(validTiles[i].Item1, validTiles[i].Item2))
						tilesToRemove.Add(validTiles[i]);
				for (int i = 0; i < tilesToRemove.Count; i++)
					validTiles.Remove(tilesToRemove[i]);
				tilesToRemove.Clear();
				if (validTiles.Count > 0)
				{
					float distance = -1f;
					Tuple<int, int> tuple4 = validTiles[0];
					for (int i = 0; i < validTiles.Count; i++)
					{
						float cdistance = Vector2.DistanceSquared(new Vector2(validTiles[i].Item1, validTiles[i].Item2) * 16 + Vector2.One * 8, player.Center);
						if (distance == -1f || cdistance < distance)
						{
							distance = cdistance;
							tuple4 = validTiles[i];
						}
					}
					if (Collision.InTileBounds(tuple4.Item1, tuple4.Item2, minX, minY, maxX, maxY))
					{
						smartX = tuple4.Item1;
						smartY = tuple4.Item2;
					}
				}
				startTiles.Clear();
				endTiles.Clear();
				validTiles.Clear();
				if (smartX != -1 && smartY != -1)
				{
					Main.SmartCursorX = (Player.tileTargetX = smartX);
					Main.SmartCursorY = (Player.tileTargetY = smartY);
					Main.SmartCursorShowing = true;
				}
				grapledTiles.Clear();
			}
			catch
			{
				Main.SmartCursorEnabled = false;
			}
		}
	}
}