using GadgetBox.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace GadgetBox
{
	public static class GadgetRecipes
	{
		internal static string AnyGoldBar;
		internal static string AnyCorruptionKey;
		internal static string AnyCobaltBar;

		public static void AddRecipeGroups(Mod mod)
		{
			RecipeGroup group = new RecipeGroup(() => Language.GetTextValue("LegacyMisc.37") + " " + Lang.GetItemNameValue(ItemID.GoldBar), new int[]
			{
				ItemID.GoldBar,
				ItemID.PlatinumBar
			});
			AnyGoldBar = mod.Name + ":AnyGoldBar";
			RecipeGroup.RegisterGroup(AnyGoldBar, group);
			group = new RecipeGroup(() => Language.GetTextValue("LegacyMisc.37") + " " + Lang.GetItemNameValue(ItemID.CorruptionKey), new int[]
			{
				ItemID.CorruptionKey,
				ItemID.CrimsonKey
			});
			AnyCorruptionKey = mod.Name + ":AnyCorruptionKey";
			RecipeGroup.RegisterGroup(AnyCorruptionKey, group);
			group = new RecipeGroup(() => Language.GetTextValue("LegacyMisc.37") + " " + Lang.GetItemNameValue(ItemID.CobaltBar), new int[]
			{
				ItemID.CobaltBar,
				ItemID.PalladiumBar
			});
			AnyCobaltBar = mod.Name + ":AnyCobaltBar";
			RecipeGroup.RegisterGroup(AnyCobaltBar, group);
		}

		public static void AddRecipes(Mod mod)
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.LihzahrdBrick, 10);
			recipe.AddIngredient(ItemID.VialofVenom);
			recipe.AddIngredient(ItemID.DartTrap);
			recipe.AddTile(TileType<LihzahrdWorkshopTile>());
			recipe.SetResult(ItemID.SuperDartTrap);
			recipe.AddRecipe();

			recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.LihzahrdBrick, 10);
			recipe.AddIngredient(ItemID.Wire, 5);
			recipe.AddIngredient(ItemID.SpikyBall, 50);
			recipe.AddTile(TileType<LihzahrdWorkshopTile>());
			recipe.SetResult(ItemID.SpikyBallTrap);
			recipe.AddRecipe();

			recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.LihzahrdBrick, 10);
			recipe.AddIngredient(ItemID.Spike, 10);
			recipe.AddRecipeGroup(AnyGoldBar, 5);
			recipe.AddTile(TileType<LihzahrdWorkshopTile>());
			recipe.SetResult(ItemID.SpearTrap);
			recipe.AddRecipe();

			recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.LihzahrdBrick, 10);
			recipe.AddIngredient(ItemID.LivingFireBlock, 5);
			recipe.AddIngredient(ItemID.Gel, 50);
			recipe.AddTile(TileType<LihzahrdWorkshopTile>());
			recipe.SetResult(ItemID.FlameTrap);
			recipe.AddRecipe();
		}
	}
}