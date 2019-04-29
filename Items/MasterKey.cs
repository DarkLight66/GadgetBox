using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace GadgetBox.Items
{
	public class MasterKey : ModItem
	{
		public override void SetDefaults()
		{
			item.width = 16;
			item.height = 34;
			item.value = Item.sellPrice(gold: 10);
			item.rare = 6;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.GoldenKey, 10);
			recipe.AddIngredient(ItemID.ShadowKey);
			recipe.AddIngredient(ItemID.HallowedKey);
			recipe.AddRecipeGroup(GadgetRecipes.AnyCorruptionKey);
			recipe.AddIngredient(ItemID.FrozenKey);
			recipe.AddIngredient(ItemID.JungleKey);
			recipe.AddIngredient(ItemID.TempleKey);
			recipe.AddTile(TileID.TinkerersWorkbench);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}