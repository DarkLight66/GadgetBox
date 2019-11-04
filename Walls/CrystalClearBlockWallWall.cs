using GadgetBox.Items.Placeable;
using Microsoft.Xna.Framework;
using MonoMod.Cil;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using static Mono.Cecil.Cil.OpCodes;
using static Terraria.ModLoader.ModContent;

namespace GadgetBox.Walls
{
	public class CrystalClearBlockWallWall : ModWall
	{
		public override void SetDefaults()
		{
			Main.wallHouse[Type] = true;
			Main.wallLight[Type] = true;
			dustType = DustID.SilverCoin;
			drop = ItemType<CrystalClearBlockWall>();
			AddMapEntry(Color.Orchid);
		}

		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			if (Main.LocalPlayer.Gadget().crystalLens)
			{
				num = fail ? 1 : 3;
			}
		}

		public override bool Autoload(ref string name, ref string texture)
		{
			IL.Terraria.Main.DrawWalls += ILDrawWalls;
			return base.Autoload(ref name, ref texture);
		}

		private void ILDrawWalls(ILContext il)
		{
			ILCursor cursor = new ILCursor(il);

			if (!cursor.TryGotoNext(i => i.MatchLdloca(14)))
			{
				return;
			}

			cursor.Emit(Ldloca_S, (byte)14);
			cursor.Emit(Ldloc_S, (byte)13);
			cursor.EmitDelegate<ColorMod>(ColorModMult);
		}

		private delegate void ColorMod(ref Color color, ushort wall);

		private static void ColorModMult(ref Color color, ushort wall)
		{
			if (wall == WallType<CrystalClearBlockWallWall>())
			{
				color *= GadgetPlayer.crystalLensFadeMult;
			}
		}
	}
}