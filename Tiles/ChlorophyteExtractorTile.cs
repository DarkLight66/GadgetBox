using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using GadgetBox.Items.Placeable;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.Enums;
using System.Collections.Generic;
using Terraria.Localization;
using GadgetBox.GadgetUI;

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
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.AlternateTile, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.AnchorAlternateTiles = new int[] { TileID.Mud, TileID.JungleGrass, TileID.Chlorophyte };
			TileObjectData.newTile.UsesCustomCanPlace = true;
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
			Point16 position = TEPosition(i,j);
			if (TileEntity.ByPosition.ContainsKey(position))
			{
				var Extractor = TileEntity.ByPosition[position] as ChlorophyteExtractorTE;
				if (Extractor == null)
					return;
				if (!Extractor.IsWorking)
					frameYOffset = 0;
				else
					frameXOffset = (Main.tileFrame[TileID.Extractinator] - Extractor.FrameYOffset + 10) % 10 * animationFrameHeight;
			}
		}

		public override void RightClick(int i, int j)
		{
			Player player = Main.LocalPlayer;
			GadgetPlayer gadgetPlayer = player.GetModPlayer<GadgetPlayer>();
			Main.mouseRightRelease = false;
			Point16 extractorPos = TEPosition(i, j);
			if (player.sign >= 0 || player.talkNPC >= 0)
				Main.CloseNPCChatOrSign();
			if (Main.editChest)
			{
				Main.PlaySound(SoundID.MenuTick);
				Main.editChest = false;
				Main.npcChatText = "";
			}
			if (player.editedChestName)
			{
				NetMessage.SendData(MessageID.SyncPlayerChest, -1, -1, NetworkText.FromLiteral(Main.chest[player.chest].name), player.chest, 1f);
				player.editedChestName = false;
			}
			if (player.chest != -1)
			{
				player.chest = -1;
				player.flyingPigChest = -1;
				Recipe.FindRecipes();
				Main.PlaySound(SoundID.MenuClose);
			}
			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				if (gadgetPlayer.extractorPos == extractorPos && gadgetPlayer.extractor >= 0)
				{
					gadgetPlayer.extractor = -1;
					ChlorophyteExtractorUI.visible = false;
					Main.PlaySound(SoundID.MenuClose);
				}
				else
				{
					ChlorophyteExtractorTE.RequestOpen(extractorPos);
					Main.stackSplit = 600;
				}
				return;
			}

			if (!TileEntity.ByPosition.ContainsKey(extractorPos))
				return;
			var Extractor = TileEntity.ByPosition[extractorPos] as ChlorophyteExtractorTE;
			if (Extractor == null)
				return;

			Main.stackSplit = 600;
			if (Extractor.ID == gadgetPlayer.extractor)
				ChlorophyteExtractorUI.CloseUI(gadgetPlayer);
			else
			{
				gadgetPlayer.extractor = Extractor.ID;
				gadgetPlayer.extractorPos = extractorPos;
				ChlorophyteExtractorUI.OpenUI(gadgetPlayer);
			}
			Recipe.FindRecipes();
		}

		public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
		public override bool HasSmartInteract() => true;
		Point16 TEPosition(int i, int j) => new Point16(i - Main.tile[i, j].frameX / 18 + 1, j - Main.tile[i, j].frameY % animationFrameHeight / 18 + 1);
	}
}
