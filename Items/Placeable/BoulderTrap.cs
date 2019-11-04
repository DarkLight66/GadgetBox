using GadgetBox.Tiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace GadgetBox.Items.Placeable
{
	public class BoulderTrap : ModItem
	{
		public override void SetDefaults()
		{
			item.Size = new Vector2(22);
			item.useStyle = 1;
			item.useTurn = true;
			item.useAnimation = 15;
			item.useTime = 10;
			item.autoReuse = true;
			item.maxStack = 99;
			item.consumable = true;
			item.value = Item.sellPrice(silver: 80);
			item.createTile = TileType<BoulderTrapTile>();
			item.mech = true;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.LihzahrdBrick, 40);
			recipe.AddIngredient(ItemID.Cog, 20);
			recipe.AddIngredient(ItemID.Boulder, 5);
			recipe.AddTile(TileType<LihzahrdWorkshopTile>());
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}