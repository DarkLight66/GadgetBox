using GadgetBox.Tiles;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace GadgetBox.Items.Placeable
{
	public class CrystalClearBlock : ModItem
	{
		public override void SetDefaults()
		{
			item.CloneDefaults(ItemID.TeamBlockWhite);
			item.createTile = TileType<CrystalClearBlockTile>();
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Glass, 10);
			recipe.AddIngredient(ItemID.CrystalShard, 1);
			recipe.AddTile(TileID.CrystalBall);
			recipe.SetResult(this);
			recipe.AddRecipe();

			recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemType<CrystalClearBlockWall>(), 4);
			recipe.AddTile(TileID.WorkBenches);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}