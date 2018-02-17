using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using GadgetBox.Items;
using GadgetBox.Items.Accessories;
using GadgetBox.Items.Tools;

namespace GadgetBox
{
    public class GadgetNPC : GlobalNPC
    {
        public override void NPCLoot(NPC npc)
        {
            switch (npc.type)
            {
                case NPCID.WallofFlesh:
                    if (Main.rand.NextBool(10))
                        Item.NewItem(npc.getRect(), mod.ItemType<EtherealVortex>(), 1, false, -1);
                    break;
                case NPCID.ArmoredSkeleton:
                case NPCID.BlueArmoredBones:
                case NPCID.BlueArmoredBonesMace:
                case NPCID.BlueArmoredBonesNoPants:
                case NPCID.BlueArmoredBonesSword:
					if (Main.rand.NextBool(Main.expertMode ? 50 : 100))
						Item.NewItem(npc.getRect(), mod.ItemType<EnchantedPolish>(), Main.rand.Next(1, 3));
                    break;
            }
        }

		public override void SetupShop(int type, Chest shop, ref int nextSlot)
		{
			if (nextSlot > 38)
				return;
			if (type == NPCID.Merchant)
			{
				int slot = 0;
				while (slot < nextSlot)
				{
					if (shop.item[++slot - 1].type != ItemID.CopperAxe && slot != nextSlot)
						continue;
					for (int i = nextSlot; i > slot; i--)
						shop.item[i] = shop.item[i - 1];
					shop.item[slot] = new Item();
					shop.item[slot].SetDefaults(mod.ItemType<OldShovel>());
					slot = ++nextSlot;
				}
			}
		}

		public override void SetupTravelShop(int[] shop, ref int nextSlot)
        {
            if (!Main.hardMode || nextSlot >= shop.Length || !Main.rand.NextBool(50))
                return;
            shop[nextSlot++] = mod.ItemType<EnchantedPolish>();
        }
    }
}
