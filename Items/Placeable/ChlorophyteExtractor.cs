﻿using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using GadgetBox.Tiles;

namespace GadgetBox.Items.Placeable
{
	public class ChlorophyteExtractor : ModItem
	{
		public override void SetDefaults()
		{
			item.Size = new Vector2(46);
			item.maxStack = 99;
			item.useTurn = true;
			item.autoReuse = true;
			item.useAnimation = 15;
			item.useTime = 10;
			item.useStyle = 1;
			item.consumable = true;
			item.value = Item.sellPrice(gold: 2);
			item.createTile = mod.TileType<ChlorophyteExtractorTile>();
		}

		public override void AddRecipes()
		{

		}
	}
}
