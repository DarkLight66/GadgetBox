using System.Collections.Generic;
using GadgetBox.Items.Accessories;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace GadgetBox
{
	class GadgetPlayer : ModPlayer
    {
        public bool etherMagnet = false;
        public bool shinyEquips = false;
		public bool critterCatch = false;
        
        public byte critShine = 0;
        public byte speedShine = 0;

        public override void ResetEffects()
        {
            etherMagnet = false;
            shinyEquips = false;
			critterCatch = false;
            
            critShine = 0;
            speedShine = 0;
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
				return;
			NPC npc;
			for (byte i = 0; i < Main.maxNPCs; i++)
			{
				npc = Main.npc[i];
				if (!npc.active || npc.catchItem <= 0)
					continue;
				if (hitbox.Intersects(npc.getRect()) && (npc.noTileCollide || player.CanHit(npc)))
					GadgetMethods.CatchNPC(i, player.whoAmI);
			}
		}

		public override void AnglerQuestReward(float rareMultiplier, List<Item> rewardItems)
		{
			if (player.anglerQuestsFinished > 25 && Main.rand.NextBool((int)((Main.hardMode ? 75 : 100) * rareMultiplier)))
			{
				Item item = new Item();
				item.SetDefaults(mod.ItemType<CritterNetAttachment>());
				rewardItems.Add(item);
			}
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