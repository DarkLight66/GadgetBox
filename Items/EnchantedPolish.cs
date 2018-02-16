using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using GadgetBox.Buffs;

namespace GadgetBox.Items
{
	public class EnchantedPolish : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 26;
            item.useStyle = 4;
            item.useTurn = true;
            item.useAnimation = 30;
            item.useTime = 30;
            item.maxStack = 30;
            item.consumable = true;
            item.buffType = mod.BuffType<ShinyEquipment>();
            item.buffTime = 36000;
            item.value = Item.buyPrice(gold:1);
            item.rare = 4;
            item.UseSound = SoundID.Item29;
        }

        public override bool CanUseItem(Player player)
        {
            return !player.GetModPlayer<GadgetPlayer>().shinyEquips;
        }
    }
}
