using GadgetBox.Items.Placeable;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace GadgetBox.Tiles
{
	public class LihzahrdWorkshopTile : ModTile
	{
		public override void SetDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.addTile(Type);
			ModTranslation name = CreateMapEntryName();
			AddMapEntry(new Color(152, 76, 26), name);
			dustType = 148;
			disableSmartCursor = true;
			animationFrameHeight = 54;
			adjTiles = new int[] { TileID.TinkerersWorkbench, TileID.LihzahrdFurnace };
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			Item.NewItem(i * 16, j * 16, 54, 54, mod.ItemType<LihzahrdWorkshop>());
		}

		public override void AnimateTile(ref int frame, ref int frameCounter)
		{
			frame = Main.tileFrame[TileID.LihzahrdFurnace];
		}

		public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
		{
			int baseX = (i - Main.tile[i, j].frameX / 18) / 3;
			int uniqueAnimationFrame = Main.tileFrame[Type] + baseX;
			if (baseX % 2 == 0)
			{
				uniqueAnimationFrame += 3;
			}

			if (baseX % 3 == 0)
			{
				uniqueAnimationFrame += 3;
			}

			if (baseX % 4 == 0)
			{
				uniqueAnimationFrame += 3;
			}

			uniqueAnimationFrame = uniqueAnimationFrame % 5;
			frameYOffset = uniqueAnimationFrame * animationFrameHeight;
		}

		public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
	}
}