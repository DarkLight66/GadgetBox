using GadgetBox.Items.Placeable;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

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
			drop = ItemType<CrystalClearBlock>();
			AddMapEntry(Color.Orchid);
		}

		public override void PostSetDefaults()
		{
			Main.tileNoSunLight[Type] = false;
		}

		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = Main.LocalPlayer.Gadget().crystalLens ? fail ? 1 : 3 : 0;
		}

		public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
		{
			Framing.SelfFrame8Way(i, j, Main.tile[i, j], resetFrame);
			return false;
		}

		public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref Color drawColor, ref int nextSpecialDrawIndex)
		{
			drawColor *= GadgetPlayer.crystalLensFadeMult;
		}
	}
}