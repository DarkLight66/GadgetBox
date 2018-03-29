using GadgetBox.Tiles;
using Terraria.ID;
using Terraria.ModLoader;

namespace GadgetBox
{
	public static class GadgetRecipes
	{
		public static void AddRecipes(Mod mod)
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.LihzahrdBrick, 10);
			recipe.AddIngredient(ItemID.VialofVenom);
			recipe.AddIngredient(ItemID.DartTrap);
			recipe.AddTile(mod.TileType<LihzahrdWorkshopTile>());
			recipe.SetResult(ItemID.SuperDartTrap);
			recipe.AddRecipe();

			recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.LihzahrdBrick, 10);
			recipe.AddIngredient(ItemID.Wire, 5);
			recipe.AddIngredient(ItemID.SpikyBall, 50);
			recipe.AddTile(mod.TileType<LihzahrdWorkshopTile>());
			recipe.SetResult(ItemID.SpikyBallTrap);
			recipe.AddRecipe();

			recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.LihzahrdBrick, 10);
			recipe.AddIngredient(ItemID.Spike, 10);
			recipe.AddRecipeGroup(GadgetBox.AnyGoldBar, 5);
			recipe.AddTile(mod.TileType<LihzahrdWorkshopTile>());
			recipe.SetResult(ItemID.SpearTrap);
			recipe.AddRecipe();

			recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.LihzahrdBrick, 10);
			recipe.AddIngredient(ItemID.LivingFireBlock, 5);
			recipe.AddIngredient(ItemID.Gel, 50);
			recipe.AddTile(mod.TileType<LihzahrdWorkshopTile>());
			recipe.SetResult(ItemID.FlameTrap);
			recipe.AddRecipe();
		}
	}
}