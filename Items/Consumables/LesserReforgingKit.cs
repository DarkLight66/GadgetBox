using Terraria;
using Terraria.ModLoader;

namespace GadgetBox.Items.Consumables
{
	public class LesserReforgingKit : ModItem
	{
		public override void SetDefaults()
		{
			item.width = 30;
			item.height = 34;
			item.consumable = true;
			item.maxStack = 99;
			item.rare = 1;
			item.value = 0;
		}

		public override bool CanRightClick()
			=> Main.mouseItem != null && !Main.mouseItem.IsAir && Main.mouseItem.prefix == 0 && Main.mouseItem.Prefix(-3) && ItemLoader.PreReforge(Main.mouseItem);

		public override void RightClick(Player player)
		{
			GadgetMethods.PrefixItem(ref Main.mouseItem);
			player.inventory[58] = Main.mouseItem.Clone();
		}
	}
}