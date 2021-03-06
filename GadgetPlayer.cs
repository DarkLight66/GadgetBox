﻿using System;
using System.Collections.Generic;
using GadgetBox.GadgetUI;
using GadgetBox.Items.Accessories;
using GadgetBox.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace GadgetBox
{
	public class GadgetPlayer : ModPlayer
	{
		public static float crystalLensFadeMult;

		public bool etherMagnet;
		public bool shinyEquips;
		public bool critterCatch;
		public bool nearBanner;
		public bool crystalLens;

		public byte critShine;
		public byte speedShine;

		public Point16 machinePos = Point16.NegativeOne;
		public Vector2 autoReelAim = Vector2.Zero;

		public override void ResetEffects()
		{
			etherMagnet = false;
			shinyEquips = false;
			critterCatch = false;
			nearBanner = false;
			crystalLens = false;
			critShine = 0;
			speedShine = 0;
		}

		public override void UpdateDead()
		{
			ResetEffects();
			if (Main.netMode == NetmodeID.Server)
			{
				return;
			}

			if (ChlorophyteExtractorUI.ExtractorTE.CurrentPlayer == player.whoAmI)
			{
				ChlorophyteExtractorUI.ExtractorTE.CloseUI(true);
			}
		}

		public override void OnEnterWorld(Player player)
		{
			crystalLensFadeMult = 0;
		}

		public override void PreUpdate()
		{
			if (Main.myPlayer != player.whoAmI || !ChlorophyteExtractorUI.visible)
			{
				return;
			}

			bool closeUIs = false, silent = false;
			if (!Main.playerInventory || player.chest != -1 || Main.npcShop != 0 || player.talkNPC != -1)
			{
				closeUIs = true;
				silent = true;
			}
			else
			{
				int playerX = (int)(player.Center.X / 16);
				int playerY = (int)(player.Center.Y / 16);
				if (playerX < machinePos.X - Player.tileRangeX || playerX > machinePos.X + Player.tileRangeX + 1 ||
					playerY < machinePos.Y - Player.tileRangeY || playerY > machinePos.Y + Player.tileRangeY + 1)
				{
					closeUIs = true;
				}
			}
			if (!closeUIs)
			{
				return;
			}

			if (ChlorophyteExtractorUI.visible)
			{
				ChlorophyteExtractorUI.ExtractorTE.CloseUI(silent);
			}
		}

		public override void PostUpdateBuffs()
		{
			if (autoReelAim != Vector2.Zero && player.ownedProjectileCounts[ProjectileType<AutoBobber>()] <= 0)
			{
				autoReelAim = Vector2.Zero;
			}
		}

		public override void PostUpdateEquips()
		{
			if (Main.myPlayer == player.whoAmI)
			{
				if (crystalLens && crystalLensFadeMult < 1)
				{
					crystalLensFadeMult += 0.00625f;
				}
				else if (!crystalLens && crystalLensFadeMult > 0)
				{
					crystalLensFadeMult -= 0.00625f;
				}
			}
		}

		public override bool? CanHitNPCWithProj(Projectile proj, NPC target)
		{
			if (critterCatch && target.catchItem > 0 && proj.Colliding(proj.getRect(), target.getRect()))
			{
				GadgetMethods.CatchNPC(target.whoAmI, player.whoAmI, false);
				return false;
			}
			return null;
		}

		public override void MeleeEffects(Item item, Rectangle hitbox)
		{
			if (!critterCatch || Main.myPlayer != player.whoAmI || item.type == ItemID.BugNet || item.type == ItemID.GoldenBugNet)
			{
				return;
			}

			for (byte i = 0; i < Main.maxNPCs; i++)
			{
				NPC npc = Main.npc[i];
				if (!npc.active || npc.catchItem <= 0)
				{
					continue;
				}

				if (hitbox.Intersects(npc.getRect()) && (npc.noTileCollide || player.CanHit(npc)))
				{
					GadgetMethods.CatchNPC(i, player.whoAmI);
				}
			}
		}

		public override void AnglerQuestReward(float rareMultiplier, List<Item> rewardItems)
		{
			if (player.anglerQuestsFinished > 25 && Main.rand.NextBool((int)((Main.hardMode ? 75 : 100) * rareMultiplier)))
			{
				Item item = new Item();
				item.SetDefaults(ItemType<CritterNetAttachment>());
				rewardItems.Add(item);
			}
		}

		public override void PostSellItem(NPC vendor, Item[] shopInventory, Item item)
		{
			GadgetItem gadget = item.Gadget();
			if (gadget.tweak != GadgetItem.TweakType.None)
			{
				if (gadget.tweak == GadgetItem.TweakType.Malleable)
				{
					player.BuyItem(item.value / 2);
				}
				else if (gadget.tweak == GadgetItem.TweakType.Precious)
				{
					player.SellItem(item.value * 5, 1);
				}
			}
		}

		public override void ModifyHitNPC(Item item, NPC target, ref int damage, ref float knockback, ref bool crit)
		{
			if (shinyEquips && crit && critShine > 0)
			{
				damage += (int)(damage * (critShine * 0.01f));
			}
		}

		public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			if (shinyEquips && crit && critShine > 0)
			{
				damage += (int)(damage * (critShine * 0.01f));
			}
		}

		public override void PostUpdateRunSpeeds()
		{
			if (player.mount.Active || !shinyEquips || speedShine == 0)
			{
				return;
			}

			int jumpHeight = speedShine;
			if (player.sticky)
			{
				jumpHeight /= 10;
			}

			if (player.dazed)
			{
				jumpHeight /= 5;
			}

			Player.jumpHeight += jumpHeight;
		}

		public override void ModifyDrawLayers(List<PlayerLayer> layers)
		{
			layers.Add(AutoReelAim);
		}

		public readonly PlayerLayer AutoReelAim = new PlayerLayer("GadgetBox", "AutoReelAim", AutoReelAimDelegate);

		private static void AutoReelAimDelegate(PlayerDrawInfo info)
		{
			Player drawPlayer = info.drawPlayer;
			if (drawPlayer.whoAmI != Main.myPlayer)
			{
				return;
			}
			Vector2 aimPos = drawPlayer.Gadget().autoReelAim;
			if (aimPos == Vector2.Zero)
			{
				return;
			}
			aimPos = ((aimPos * 100).Floor() + drawPlayer.RotatedRelativePoint(drawPlayer.MountedCenter) + new Vector2(0, drawPlayer.gfxOffY)).Floor() - Main.screenPosition;
			Texture2D texture = GadgetBox.Instance.GetTexture("Images/AutoReelingAim");
			DrawData data = new DrawData(texture, aimPos, null, Main.mouseTextColorReal, 0f, texture.Size() * 0.5f, 1f, drawPlayer.gravDir == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None, 0);
			Main.playerDrawData.Add(data);
		}
	}
}