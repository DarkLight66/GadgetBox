using GadgetBox.Tiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

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