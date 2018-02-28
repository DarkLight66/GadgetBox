using Terraria;
using Terraria.ModLoader;

namespace GadgetBox.Items.Consumables
{
	public class ReforgingKit : ModItem
	{
		public override void SetDefaults()
		{
			item.width = 32;
			item.height = 38;
			item.consumable = true;
			item.maxStack = 99;
			item.rare = 4;
			item.value = 0;
		}

		public override bool CanRightClick()
		{
			Item toPrefix = Main.LocalPlayer.HeldItem;
			return toPrefix != null && toPrefix.Prefix(-3) && ItemLoader.PreReforge(toPrefix);
		}

		public override void RightClick(Player player)
		{
			player.PrefixHeldItem();
		}
	}
}