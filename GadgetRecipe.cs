using GadgetBox.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace GadgetBox
{
	public static class GadgetRecipes
	{
		internal static string AnyGoldBar = "";

		public static void AddRecipeGroups(Mod mod)
		{
			RecipeGroup group = new RecipeGroup(() => Language.GetTextValue("LegacyMisc.37") + " " + Lang.GetItemNameValue(ItemID.GoldBar), new int[]
			{
				ItemID.GoldBar,
				ItemID.PlatinumBar
			});
			AnyGoldBar = mod.Name + ":AnyGoldBar";
			RecipeGroup.RegisterGroup(AnyGoldBar, group);
		}

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
			recipe.AddRecipeGroup(AnyGoldBar, 5);
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