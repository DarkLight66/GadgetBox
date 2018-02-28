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
		{
			Item toPrefix = Main.LocalPlayer.HeldItem;
			return toPrefix != null && toPrefix.prefix == 0 && toPrefix.Prefix(-3) && ItemLoader.PreReforge(toPrefix);
		}

		public override void RightClick(Player player)
		{
			player.PrefixHeldItem();
		}
	}
}