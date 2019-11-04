using GadgetBox.Items.Accessories;
using GadgetBox.Items.Consumables;
using GadgetBox.Items.Placeable;
using GadgetBox.Items.Tools;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

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
					{
						Item.NewItem(npc.getRect(), ItemType<EtherealVortex>(), 1, false, -1);
					}
					break;
				case NPCID.ArmoredSkeleton:
				case NPCID.BlueArmoredBones:
				case NPCID.BlueArmoredBonesMace:
				case NPCID.BlueArmoredBonesNoPants:
				case NPCID.BlueArmoredBonesSword:
					if (Main.rand.NextBool(Main.expertMode ? 25 : 50))
					{
						Item.NewItem(npc.getRect(), ItemType<EnchantedPolish>(), Main.rand.Next(1, 3));
					}
					break;
			}
		}

		public override void SetupShop(int type, Chest shop, ref int nextSlot)
		{
			switch (type)
			{
				case NPCID.Merchant:
					int slot = 0;
					while (slot <= nextSlot)
					{
						if (shop.item[slot].type != ItemID.CopperAxe && slot != nextSlot)
						{
							slot++;
							continue;
						}
						for (int i = nextSlot; i > slot + 1; i--)
						{
							shop.item[i] = shop.item[i - 1];
						}
						shop.item[slot + 1] = new Item();
						shop.item[slot + 1].SetDefaults(ItemType<OldShovel>());
						break;
					}
					break;
				case NPCID.Wizard:
					shop.item[nextSlot++].SetDefaults(ItemType<ReflectorBlock>());
					break;
				case NPCID.Steampunker:
					shop.item[nextSlot++].SetDefaults(ItemType<AutoReforgeMachine>());
					if (NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3)
					{
						shop.item[nextSlot++].SetDefaults(ItemType<ClockworkDigger>());
					}
					shop.item[nextSlot++].SetDefaults(ItemType<AutoReelingRod>());
					break;
				case NPCID.GoblinTinkerer:
					if (Main.hardMode)
					{
						shop.item[nextSlot++].SetDefaults(ItemType<RestoringKit>());
					}
					break;
			}
		}

		public override void SetupTravelShop(int[] shop, ref int nextSlot)
		{
			if (Main.hardMode && nextSlot < shop.Length && Main.rand.NextBool(25))
			{
				shop[nextSlot++] = ItemType<EnchantedPolish>();
			}
		}
	}
}