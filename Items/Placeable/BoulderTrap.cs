using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;
using Microsoft.Xna.Framework;

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
            item.createTile = mod.TileType<Tiles.BoulderTrapTile>();
            item.mech = true;
        }
    }
}