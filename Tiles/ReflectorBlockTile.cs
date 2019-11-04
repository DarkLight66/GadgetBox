using System;
using GadgetBox.Items.Placeable;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace GadgetBox.Tiles
{
	public class ReflectorBlockTile : ModTile
	{
		public override void SetDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileBrick[Type] = true;
			Main.tileBlockLight[Type] = true;
			dustType = DustID.SilverCoin;
			drop = ItemType<ReflectorBlock>();
			AddMapEntry(new Color(180, 180, 220));
		}

		public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
	}

	public class DeflectProjectiles : GlobalProjectile
	{
		private static readonly int[] NonDeflect = new int[] { 9, 10, 11, 17, 26, 35, 39, 45, 58, 61, 63, 73, 75, 88, 90, 99, 102, 103, 121, 124, 144 };
		private static uint lastGameUpdateCount;

		public override bool OnTileCollide(Projectile projectile, Vector2 oldVelocity)
		{
			if (projectile.minion || projectile.sentry || projectile.bobber || projectile.manualDirectionChange ||
				ProjectileID.Sets.LightPet[projectile.type] || ProjectileID.Sets.StardustDragon[projectile.type] ||
				projectile.damage < 1 || oldVelocity.LengthSquared() <= 4f || Array.Exists(NonDeflect, x => x == projectile.aiStyle))
			{
				return true;
			}

			var tiles = GadgetMethods.TilesHit(projectile.position, oldVelocity, projectile.width, projectile.height);
			var deflectors = tiles.FindAll(x => x.Item2 == TileType<ReflectorBlockTile>());

			if (deflectors.Count != 0 && deflectors.Count >= (tiles.Count / 2))
			{
				for (int i = 0; i < deflectors.Count; i++)
				{
					WorldGen.KillTile(deflectors[i].Item1.X, deflectors[i].Item1.Y, true, true);
				}

				float bouncyness = projectile.aiStyle == 2 || projectile.aiStyle == 113 ? 0.5f : 0.9f;
				projectile.Bounce(oldVelocity, bouncyness);
				if (projectile.timeLeft > 5)
				{
					projectile.timeLeft -= projectile.timeLeft > 100 ? (int)(projectile.timeLeft * 0.1f) : 2;
				}

				projectile.damage = (int)(projectile.damage * 0.95f);
				if (!Main.dedServ && Main.GameUpdateCount - lastGameUpdateCount > 30)
				{
					lastGameUpdateCount = Main.GameUpdateCount;
					Main.PlaySound(SoundID.Item30.WithVolume(.1f).WithPitchVariance(0.2f), projectile.Center);
				}
				return projectile.damage < 1;
			}
			return true;
		}
	}
}