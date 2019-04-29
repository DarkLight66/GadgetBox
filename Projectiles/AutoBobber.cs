using System;
using GadgetBox.Items.Tools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace GadgetBox.Projectiles
{
	public class AutoBobber : ModProjectile
	{
		public override bool CloneNewInstances => true; 

		private bool autoCatched = false;

		public override void SetDefaults()
		{
			projectile.CloneDefaults(ProjectileID.BobberMechanics);
			drawOriginOffsetY = -8;
		}

		public override void AI()
		{
			if (projectile.owner != Main.myPlayer || projectile.ai[0] != 0 || projectile.ai[1] >= 0 || projectile.localAI[1] == 0)
			{
				return;
			}

			projectile.ai[0] = 1;
			if (projectile.wet && projectile.velocity.Y > -10)
			{
				projectile.velocity.Y = -10;
			}

			projectile.netUpdate2 = true;

			bool caughtSomething = false;
			int baitType = 0;

			Player player = Main.LocalPlayer;

			for (int i = 0; i < Main.maxInventory; i++)
			{
				Item bait = player.inventory[i];
				if (bait.stack <= 0 || bait.bait <= 0)
				{
					continue;
				}

				bool consumeBait = false;
				int consumeRoll = 1 + bait.bait / 5;
				if (consumeRoll < 1)
				{
					consumeRoll = 1;
				}

				if (projectile.localAI[1] < 0 || (player.accTackleBox ? Main.rand.NextBool(consumeRoll) : !Main.rand.NextBool(consumeRoll - 1, consumeRoll + 2)))
				{
					consumeBait = true;
				}

				if (projectile.localAI[1] > 0)
				{
					Item item = new Item();
					item.SetDefaults((int)projectile.localAI[1], true);
					if (item.rare < 0)
					{
						consumeBait = false;
					}
				}

				if (consumeBait)
				{
					baitType = bait.type;
					if (ItemLoader.ConsumeItem(bait, player) && --bait.stack <= 0)
					{
						bait.SetDefaults(0, false);
					}
				}
				caughtSomething = true;
				break;
			}

			if (caughtSomething)
			{
				autoCatched = true;
				if (baitType == ItemID.TruffleWorm)
				{
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						NPC.SpawnOnPlayer(player.whoAmI, NPCID.DukeFishron);
					}
					else
					{
						NetMessage.SendData(MessageID.SpawnBoss, -1, -1, null, player.whoAmI, NPCID.DukeFishron);
					}

					projectile.ai[0] = 2;
					autoCatched = false;
				}
				else if (Main.rand.NextBool(player.accFishingLine ? 7 : 4))
				{
					projectile.ai[0] = 2;
				}
				else
				{
					projectile.ai[1] = projectile.localAI[1];
				}

				projectile.netUpdate = true;
			}
		}

		public override void Kill(int timeLeft)
		{
			Player player = Main.LocalPlayer;
			if (autoCatched && player.HeldItem.type == mod.ItemType<AutoReelingRod>())
			{
				Vector2 position = player.RotatedRelativePoint(player.MountedCenter);
				Vector2 velocity = Main.MouseWorld - position;
				if (player.gravDir == -1f)
				{
					velocity.Y = Main.screenPosition.Y + Main.screenHeight - Main.mouseY - position.Y;
				}

				velocity = player.Gadget().autoReelAim * player.HeldItem.shootSpeed;
				Projectile.NewProjectile(position, velocity, projectile.type, projectile.damage, projectile.knockBack, projectile.owner);
			}
		}

		public override bool PreDrawExtras(SpriteBatch spriteBatch)
		{
			Player player = Main.player[projectile.owner];
			Item item = player.HeldItem;

			if (item.holdStyle != 1 || item.type != mod.ItemType<AutoReelingRod>())
			{
				return true;
			}

			Vector2 lineStart = player.MountedCenter;

			lineStart.X += 46 * player.direction;
			if (player.direction < 0)
			{
				lineStart.X -= 13;
			}

			lineStart.Y += player.gfxOffY - 30 * player.gravDir;
			if (player.gravDir == -1)
			{
				lineStart.Y -= 12;
			}

			lineStart = player.RotatedRelativePoint(lineStart + new Vector2(8)) - new Vector2(8);

			Vector2 lineSegment = projectile.Center - lineStart;
			bool drawLine = true;
			float lineStep = 12;
			int colorStep = 0;

			if (lineSegment == Vector2.Zero)
			{
				drawLine = false;
			}
			else
			{
				float lineLength = lineStep / lineSegment.Length();
				lineSegment *= lineLength;
				lineStart -= lineSegment;
				lineSegment = projectile.Center - lineStart;
			}
			while (drawLine)
			{
				float currentStep = lineStep;
				float lineLength = lineSegment.Length();
				float lineLength2 = lineLength;
				if (float.IsNaN(lineLength))
				{
					drawLine = false;
				}
				else
				{
					if (lineLength < 20f)
					{
						currentStep = lineLength - 8f;
						drawLine = false;
					}
					lineLength = lineStep / lineLength;
					lineSegment *= lineLength;
					lineStart += lineSegment;
					lineSegment = projectile.position + new Vector2(projectile.width * 0.5f, projectile.height * 0.1f) - lineStart;

					if (lineLength2 > lineStep)
					{
						float mult1 = 0.3f;
						float mult2 = Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y);
						if (mult2 > 16f)
						{
							mult2 = 16f;
						}

						mult2 = 1f - mult2 / 16f;
						mult1 *= mult2;
						mult2 = lineLength2 / 80f;
						if (mult2 > 1f)
						{
							mult2 = 1f;
						}

						mult1 *= mult2;
						if (mult1 < 0f)
						{
							mult1 = 0f;
						}

						mult2 = 1f - projectile.localAI[0] / 100f;
						mult1 *= mult2;
						if (lineSegment.Y > 0f)
						{
							lineSegment.Y *= 1f + mult1;
							lineSegment.X *= 1f - mult1;
						}
						else
						{
							mult2 = Math.Abs(projectile.velocity.X) / 3f;
							if (mult2 > 1f)
							{
								mult2 = 1f;
							}

							mult2 -= 0.5f;
							mult1 *= mult2;
							if (mult1 > 0f)
							{
								mult1 *= 2f;
							}

							lineSegment.Y *= 1f + mult1;
							lineSegment.X *= 1f - mult1;
						}
					}
					float rotation = lineSegment.ToRotation() - MathHelper.PiOver2;
					Color color = Lighting.GetColor((int)lineStart.X / 16, (int)(lineStart.Y / 16f), (player.miscCounter / 3 + colorStep++) / 4 % 2 == 0 ? Color.DarkGoldenrod : Color.DarkGray);

					spriteBatch.Draw(Main.fishingLineTexture, lineStart - Main.screenPosition + Main.fishingLineTexture.Size() * 0.5f, new Rectangle(0, 0, Main.fishingLineTexture.Width, (int)currentStep), color, rotation, new Vector2(Main.fishingLineTexture.Width * 0.5f, 0f), 1f, SpriteEffects.None, 0f);
				}
			}
			return false;
		}
	}
}
