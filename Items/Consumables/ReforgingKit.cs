using Terraria;
using Terraria.ID;
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
		}

		public override bool CanRightClick()
			=> Main.mouseItem != null && !Main.mouseItem.IsAir && Main.mouseItem.Prefix(-3) && ItemLoader.PreReforge(Main.mouseItem);

		public override void RightClick(Player player)
		{
			GadgetMethods.PrefixItem(ref Main.mouseItem);
			player.inventory[58] = Main.mouseItem.Clone();
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(mod.ItemType<LesserReforgingKit>(), 5);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}