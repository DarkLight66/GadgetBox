using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using GadgetBox.Items;
using GadgetBox.Items.Accessories;

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
                        Item.NewItem(npc.getRect(), mod.ItemType<EnchantedPolish>(), Main.rand.Next(1,3));
                    break;
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
