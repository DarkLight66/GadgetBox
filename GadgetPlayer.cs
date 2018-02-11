using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace GadgetBox
{
    class GadgetPlayer : ModPlayer
    {
        public bool etherMagnet = false;
        public bool shinyEquips = false;
        
        public byte critShine = 0;
        public byte speedShine = 0;

        public override void ResetEffects()
        {
            etherMagnet = false;
            shinyEquips = false;
            
            critShine = 0;
            speedShine = 0;
        }

        public override void ModifyHitNPC(Item item, NPC target, ref int damage, ref float knockback, ref bool crit)
        {
            if (shinyEquips && crit && critShine > 0)
                damage += (int)(damage * (critShine * 0.01f));
        }

        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (shinyEquips && crit && critShine > 0)
                damage += (int)(damage * (critShine * 0.01f));
        }

        public override void PostUpdateRunSpeeds()
        {
            if (player.mount.Active || !shinyEquips || speedShine == 0)
                return;
            int jumpHeight = speedShine;
            if (player.sticky)
                jumpHeight /= 10;
            if (player.dazed)
                jumpHeight /= 5;
            Player.jumpHeight += speedShine;
        }
    }
}