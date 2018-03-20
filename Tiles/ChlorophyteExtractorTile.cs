using GadgetBox.GadgetUI;
using GadgetBox.Items.Placeable;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace GadgetBox.Tiles
{
	public class ChlorophyteExtractorTile : ModTile
	{
		public override void SetDefaults()
		{
			TileID.Sets.HasOutlines[Type] = true;
			Main.tileFrameImportant[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
			TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(mod.GetTileEntity<ChlorophyteExtractorTE>().Hook_AfterPlacement, -1, 0, false);
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.addTile(Type);
			ModTranslation name = CreateMapEntryName();
			AddMapEntry(new Color(152, 76, 26), name);
			dustType = 148;
			disableSmartCursor = true;
			animationFrameHeight = 54;
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			Item.NewItem(i * 16, j * 16, 54, 54, mod.ItemType<ChlorophyteExtractor>());
			mod.GetTileEntity<ChlorophyteExtractorTE>().Kill(i + 1, j + 1);
		}

		public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
		{
			ChlorophyteExtractorTE extractorTE = ChlorophyteExtractorTE.ExtractorByPosition(TEPosition(i, j));
			if (extractorTE == null)
				return;
			if (extractorTE.Animating && !extractorTE.IsWorking && (Main.tileFrame[TileID.Extractinator] - extractorTE.FrameYOffset + 10) % 10 == 0)
			{
				extractorTE.Animating = false;
				extractorTE.FrameYOffset = 0;
			}
			else if (!extractorTE.Animating && extractorTE.IsWorking)
			{
				extractorTE.Animating = true;
				extractorTE.FrameYOffset = (byte)Main.tileFrame[TileID.Extractinator];
			}
			frameYOffset = !extractorTE.Animating ? 0 : (Main.tileFrame[TileID.Extractinator] - extractorTE.FrameYOffset + 10) % 10 * animationFrameHeight;
		}

		public override void RightClick(int i, int j)
		{
			Main.mouseRightRelease = false;
			Point16 extractorPos = TEPosition(i, j);
			ChlorophyteExtractorTE extractorTE = ChlorophyteExtractorTE.ExtractorByPosition(extractorPos);
			if (extractorTE == null)
				return;
			Player player = Main.LocalPlayer;
			GadgetPlayer gadgetPlayer = player.GetModPlayer<GadgetPlayer>();

			player.CloseVanillaUIs();
			if (ReforgeMachineUI.visible)
				GadgetBox.Instance.reforgeMachineUI.ToggleUI(false, Point16.Zero, true);
			if (extractorTE.CurrentPlayer == player.whoAmI)
				ChlorophyteExtractorUI.CloseUI(extractorTE);
			else
			{
				extractorTE.CurrentPlayer = (byte)player.whoAmI;
				gadgetPlayer.machinePos = extractorPos;
				ChlorophyteExtractorUI.OpenUI(extractorTE);
			}
		}

		public override void MouseOver(int i, int j)
		{
			Player player = Main.LocalPlayer;
			player.noThrow = 2;
			player.showItemIcon = true;
			player.showItemIcon2 = mod.ItemType<ChlorophyteExtractor>();
		}

		public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;

		public override bool HasSmartInteract() => true;

		Point16 TEPosition(int i, int j) => new Point16(i - Main.tile[i, j].frameX / 18 + 1, j - Main.tile[i, j].frameY % animationFrameHeight / 18 + 1);
	}
}