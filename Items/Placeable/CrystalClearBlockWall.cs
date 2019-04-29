using GadgetBox.Walls;
using Terraria.ID;
using Terraria.ModLoader;

namespace GadgetBox.Items.Placeable
{
	public class CrystalClearBlockWall : ModItem
	{
		public override void SetDefaults()
		{
			item.CloneDefaults(ItemID.CrystalBlockWall);
			item.createWall = mod.WallType<CrystalClearBlockWallWall>();
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(mod.GetItem<CrystalClearBlock>());
			recipe.AddTile(TileID.WorkBenches);
			recipe.SetResult(this, 4);
			recipe.AddRecipe();
		}
	}
}