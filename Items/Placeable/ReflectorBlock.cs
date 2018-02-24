using GadgetBox.Tiles;
using Terraria.ID;
using Terraria.ModLoader;

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