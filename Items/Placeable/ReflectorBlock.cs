using Terraria.ID;
using Terraria.ModLoader;
using GadgetBox.Tiles;

namespace GadgetBox.Items.Placeable
{
	public class ReflectorBlock : ModItem
    {
        public override void SetDefaults()
        {
            item.CloneDefaults(ItemID.TeamBlockWhite);
            item.createTile = mod.TileType<ReflectorBlockTile>();
        }
    }
}