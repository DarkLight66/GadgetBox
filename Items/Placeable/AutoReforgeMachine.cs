using GadgetBox.Tiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace GadgetBox.Items.Placeable
{
	public class AutoReforgeMachine : ModItem
	{
		public override void SetDefaults()
		{
			item.Size = new Vector2(44, 32);
			item.maxStack = 99;
			item.useTurn = true;
			item.autoReuse = true;
			item.useAnimation = 15;
			item.useTime = 10;
			item.useStyle = 1;
			item.rare = 3;
			item.consumable = true;
			item.value = Item.buyPrice(gold: 20);
			item.createTile = TileType<AutoReforgeMachineTile>();
		}
	}
}