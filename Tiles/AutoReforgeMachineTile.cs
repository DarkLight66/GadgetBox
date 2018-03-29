using GadgetBox.GadgetUI;
using GadgetBox.Items.Placeable;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace GadgetBox.Tiles
{
	public class AutoReforgeMachineTile : ModTile
	{
		public override void SetDefaults()
		{
			TileID.Sets.HasOutlines[Type] = true;
			Main.tileFrameImportant[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
			TileObjectData.newTile.Width = 4;
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.newTile.Direction = TileObjectDirection.PlaceRight;
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceLeft;
			TileObjectData.newAlternate.Origin = new Point16(2, 2);
			TileObjectData.addAlternate(0);
			TileObjectData.addTile(Type);
			ModTranslation name = CreateMapEntryName();
			AddMapEntry(new Color(132, 155, 255), name);
			disableSmartCursor = true;
			animationFrameHeight = 54;
			dustType = 1;
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			Item.NewItem(i * 16, j * 16, 54, 54, mod.ItemType<AutoReforgeMachine>());
		}

		public override void AnimateTile(ref int frame, ref int frameCounter)
		{
			if (++frameCounter > 4)
			{
				frameCounter = 0;
				if (++frame > 3)
					frame = 0;
			}
		}

		public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
		{
			int baseX = (i - Main.tile[i, j].frameX / 18) / 4;
			int uniqueAnimationFrame = Main.tileFrame[Type] + baseX;
			if (baseX % 2 == 0)
				uniqueAnimationFrame += 3;
			if (baseX % 3 == 0)
				uniqueAnimationFrame += 3;
			if (baseX % 4 == 0)
				uniqueAnimationFrame += 3;
			uniqueAnimationFrame = uniqueAnimationFrame % 4;
			frameYOffset = uniqueAnimationFrame * animationFrameHeight;
		}

		public override void RightClick(int i, int j)
		{
			Main.mouseRightRelease = false;
			Point16 CenterPos = Center(i, j);
			Player player = Main.LocalPlayer;
			player.CloseVanillaUIs();
			if (ChlorophyteExtractorUI.ExtractorTE.CurrentPlayer == player.whoAmI)
				ChlorophyteExtractorUI.ExtractorTE.CloseUI(true);
			GadgetBox.Instance.reforgeMachineUI.ToggleUI(!ReforgeMachineUI.visible, CenterPos);
		}

		public override void MouseOver(int i, int j)
		{
			Player player = Main.LocalPlayer;
			player.noThrow = 2;
			player.showItemIcon = true;
			player.showItemIcon2 = mod.ItemType<AutoReforgeMachine>();
		}

		public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;

		public override bool HasSmartInteract() => true;

		Point16 Center(int i, int j) => new Point16(i - Main.tile[i, j].frameX / 18 + 1, j - Main.tile[i, j].frameY % animationFrameHeight / 18 + 1);
	}
}