using GadgetBox.Walls;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace GadgetBox.Items.Placeable
{
	public class CrystalClearBlockWall : ModItem
	{
		public override void SetDefaults()
		{
			item.CloneDefaults(ItemID.CrystalBlockWall);
			item.createWall = WallType<CrystalClearBlockWallWall>();
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemType<CrystalClearBlock>());
			recipe.AddTile(TileID.WorkBenches);
			recipe.SetResult(this, 4);
			recipe.AddRecipe();
		}
	}
}