using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace GadgetBox.Projectiles
{
    public class TempleBoulder : ModProjectile
    {
        public override void SetDefaults()
        {
			projectile.Size = new Vector2(32);
            projectile.friendly = true;
            projectile.hostile = true;
            projectile.ranged = true;
            projectile.penetrate = -1;
            projectile.trap = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.hide = true;
        }

        public override void AI()
        {
            if (projectile.ai[1] > 7f)
            {
                if (projectile.aiStyle != 25)
                {
                    projectile.tileCollide = true;
                    projectile.aiStyle = 25;
                    projectile.velocity.X += 0.005f - projectile.ai[0] * 0.01f;
                    projectile.ai[0] = 0;
                    projectile.hide = false;
                }
            }
            else
            {
                projectile.ai[1]++;
                projectile.rotation += projectile.velocity.X * 0.06f;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (projectile.velocity.Y != oldVelocity.Y && oldVelocity.Y > 5f)
            {
                Collision.HitTiles(projectile.position, oldVelocity, projectile.width, projectile.height);
                Main.PlaySound(SoundID.Dig, projectile.Center);
                projectile.velocity.Y = -oldVelocity.Y * 0.2f;
            }

            if (projectile.velocity.X != oldVelocity.X)
            {
                if (Math.Abs(oldVelocity.X) > 3f)
                    return true;
                projectile.velocity.X = -oldVelocity.X * 1.5f;
            }
            return false;
        }

        public override void DrawBehind(int index, List<int> drawCacheProjsBehindNPCsAndTiles, List<int> drawCacheProjsBehindNPCs, List<int> drawCacheProjsBehindProjectiles, List<int> drawCacheProjsOverWiresUI)
        {
            if (projectile.hide)
                drawCacheProjsBehindNPCsAndTiles.Add(index);
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Dig, projectile.Center);
            for (int i = 0; i < 15; i++)
            {
                Dust dust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, 148);
                if (Main.rand.Next(2) == 0)
                    dust.scale *= 1.4f;
            }
        }
    }
}
