using GadgetBox.Tiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace GadgetBox.Items.Placeable
{
	// Credits to AdipemDragon for the item an tile sprites
	public class LihzahrdWorkshop : ModItem
	{
		public override void SetDefaults()
		{
			item.Size = new Vector2(24);
			item.maxStack = 99;
			item.useTurn = true;
			item.autoReuse = true;
			item.useAnimation = 15;
			item.useTime = 10;
			item.useStyle = 1;
			item.consumable = true;
			item.value = Item.sellPrice(gold: 2);
			item.createTile = TileType<LihzahrdWorkshopTile>();
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.LihzahrdFurnace);
			recipe.AddIngredient(ItemID.TinkerersWorkshop);
			recipe.AddIngredient(ItemID.LunarTabletFragment, 10);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}