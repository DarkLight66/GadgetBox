using GadgetBox.Items.Placeable;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace GadgetBox.Walls
{
	public class CrystalClearBlockWallWall : ModWall
	{
		public override void SetDefaults()
		{
			Main.wallHouse[Type] = true;
			Main.wallLight[Type] = true;
			dustType = DustID.SilverCoin;
			drop = mod.ItemType<CrystalClearBlockWall>();
			AddMapEntry(Color.Transparent);
		}

		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			if (Main.LocalPlayer.Gadget().crystalLens)
			{
				num = fail ? 1 : 3;
			}
		}

		public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) => Main.LocalPlayer.Gadget().crystalLens;
	}
}