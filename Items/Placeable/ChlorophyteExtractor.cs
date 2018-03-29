﻿using GadgetBox.Tiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace GadgetBox.Items.Placeable
{
	public class ChlorophyteExtractor : ModItem
	{
		public override void SetDefaults()
		{
			item.Size = new Vector2(34, 38);
			item.maxStack = 99;
			item.useTurn = true;
			item.autoReuse = true;
			item.useAnimation = 15;
			item.useTime = 10;
			item.useStyle = 1;
			item.consumable = true;
			item.rare = 3;
			item.value = Item.sellPrice(gold: 2);
			item.createTile = mod.TileType<ChlorophyteExtractorTile>();
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.LihzahrdBrick, 25);
			recipe.AddIngredient(ItemID.LunarTabletFragment, 20);
			recipe.AddIngredient(ItemID.Wire, 20);
			recipe.AddIngredient(ItemID.Cog, 20);
			recipe.AddTile(mod.TileType<LihzahrdWorkshopTile>());
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}