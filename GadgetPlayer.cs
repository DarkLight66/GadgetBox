using System.Collections.Generic;
using GadgetBox.Items.Accessories;
using GadgetBox.GadgetUI;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using GadgetBox.Tiles;

namespace GadgetBox
{
	class GadgetPlayer : ModPlayer
    {
        public bool etherMagnet = false;
        public bool shinyEquips = false;
		public bool critterCatch = false;
        
        public byte critShine = 0;
        public byte speedShine = 0;

		public Point16 extractorPos = Point16.NegativeOne;
		public int extractor = -1;

		public override void ResetEffects()
        {
            etherMagnet = false;
            shinyEquips = false;
			critterCatch = false;
            
            critShine = 0;
            speedShine = 0;
        }

		public override void UpdateDead()
		{
			if (extractor > -1)
				ChlorophyteExtractorUI.CloseUI(this, true);
		}

		public override void clientClone(ModPlayer clientClone)
		{
			GadgetPlayer gadgetClone = clientClone as GadgetPlayer;
			gadgetClone.extractor = extractor;
		}

		public override void SendClientChanges(ModPlayer clientPlayer)
		{
			GadgetPlayer gadgetCient = clientPlayer as GadgetPlayer;
			if (gadgetCient.extractor != extractor && extractor == -1)
			{
				ModPacket packet = GadgetBox.Instance.GetPacket(MessageType.SyncPlayerExtractor, 4);
				packet.Write((short)-1);
				packet.Send();
			}
		}

		public override void PreUpdate()
		{
			if (Main.myPlayer != player.whoAmI || !ChlorophyteExtractorUI.visible)
				return;
			if (!Main.playerInventory || player.chest != -1 || Main.npcShop != 0 || player.talkNPC != -1 || ChlorophyteExtractorUI.ExtractorTE.ID == -1)
				ChlorophyteExtractorUI.CloseUI(this, true);
			else
			{
				int playerX = (int)(player.Center.X / 16);
				int playerY = (int)(player.Center.Y / 16);
				if (playerX < extractorPos.X - Player.tileRangeX || playerX > extractorPos.X + Player.tileRangeX + 1 ||
					playerY < extractorPos.Y - Player.tileRangeY || playerY > extractorPos.Y + Player.tileRangeY + 1)
					ChlorophyteExtractorUI.CloseUI(this);
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