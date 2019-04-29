using GadgetBox.Items.Placeable;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace GadgetBox.Tiles
{
	public class CrystalClearBlockTile : ModTile
	{
		public override void SetDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileBrick[Type] = true;
			TileID.Sets.DrawsWalls[Type] = true;
			TileID.Sets.BlocksStairs[Type] = true;
			TileID.Sets.GemsparkFramingTypes[Type] = Type;
			dustType = DustID.SilverCoin;
			drop = mod.ItemType<CrystalClearBlock>();
			AddMapEntry(Color.Transparent);
		}

		public override void PostSetDefaults()
		{
			Main.tileNoSunLight[Type] = false;
		}

		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			if (Main.LocalPlayer.Gadget().crystalLens)
			{
				num = fail ? 1 : 3;
			}
		}

		public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) => Main.LocalPlayer.Gadget().crystalLens;

		public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
		{
			Framing.SelfFrame8Way(i, j, Main.tile[i, j], resetFrame);
			return false;
		}
	}
}