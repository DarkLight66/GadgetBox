using GadgetBox.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace GadgetBox.Items.Placeable
{
	public class ReflectorBlock : ModItem
	{
		public override void SetDefaults()
		{
			item.CloneDefaults(ItemID.TeamBlockWhite);
			item.createTile = TileType<ReflectorBlockTile>();
			item.value = Item.buyPrice(silver: 10);
		}
	}
}