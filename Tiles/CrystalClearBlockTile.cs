using System;
using GadgetBox.Items.Placeable;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using static Mono.Cecil.Cil.OpCodes;

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
			AddMapEntry(Color.Orchid);
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

		public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
		{
			Framing.SelfFrame8Way(i, j, Main.tile[i, j], resetFrame);
			return false;
		}

		public override bool Autoload(ref string name, ref string texture)
		{
			IL.Terraria.Main.DrawTiles += ILDrawTiles;
			return base.Autoload(ref name, ref texture);
		}

		private void ILDrawTiles(ILContext il)
		{
			ILCursor cursor = new ILCursor(il);

			if (!cursor.TryGotoNext(i => i.MatchStloc(22)))
			{
				return;
			}

			cursor.Index++;
			cursor.Emit(Ldloca_S, (byte)22);
			cursor.Emit(Ldloc_S, (byte)17);
			cursor.EmitDelegate<ColorMod>(ColorModMult);
		}

		private delegate void ColorMod(ref Color color, ushort tile);

		private static void ColorModMult(ref Color color, ushort tile)
		{
			if (tile == GadgetBox.Instance.TileType<CrystalClearBlockTile>())
			{
				color *= GadgetPlayer.crystalLensFadeMult;
			}
		}
	}
}