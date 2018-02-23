using System;
using GadgetBox.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace GadgetBox.GadgetUI
{
	class ChlorophyteExtractorUI : UIState
	{
		internal static bool visible = false;
		internal static ChlorophyteExtractorTE ExtractorTE = new ChlorophyteExtractorTE();
		internal UIPanel extractorPanel;
		internal UIDelayedPowerButton powerButton;
		internal UIPowerSlot powerCellSlot;
		internal UIPowerBar powerBar;
		internal UIExtractorSlot mudSlot;
		internal UIExtractorSlot chloroSlot;
		internal Mod mod;

		public override void OnInitialize()
		{
			mod = GadgetBox.Instance;
			extractorPanel = new UIPanel();
			extractorPanel.SetPadding(4);
			extractorPanel.BorderColor = new Color(18, 0, 26);
			extractorPanel.BackgroundColor = new Color(150, 64, 16);
			extractorPanel.Top.Set(Main.instance.invBottom + 60f, 0);
			extractorPanel.Left.Set(180, 0);
			extractorPanel.Width.Set(220, 0);
			extractorPanel.Height.Set(120, 0);

			powerCellSlot = new UIPowerSlot(mod.GetTexture("GadgetUI/PowerSlot_Open"), mod.GetTexture("GadgetUI/PowerSlot_Closed"), () => ExtractorTE.Power > 0);
			powerCellSlot.Top.Set(8, 0);
			powerCellSlot.Left.Set(8, 0);
			powerCellSlot.OnClick += (a,b) => ExtractorTE.ProvidePower();
			extractorPanel.Append(powerCellSlot);

			powerButton = new UIDelayedPowerButton(mod.GetTexture("GadgetUI/PowerButton_ON"), mod.GetTexture("GadgetUI/PowerButton_OFF"), 240, () => ExtractorTE.IsON == true, () => ExtractorTE.Power > 0);
			powerButton.Top.Set(16, 0);
			powerButton.Left.Set(10, 0);
			powerButton.VAlign = 1f;
			powerButton.OnClick += (a, b) => ExtractorTE.TogglePower();
			extractorPanel.Append(powerButton);

			powerBar = new UIPowerBar(mod.GetTexture("GadgetUI/PowerBar"), mod.GetTexture("GadgetUI/PowerBarFill"), 6, 4);
			powerBar.Top.Set(22, 0);
			powerBar.Left.Set(18, 0);
			powerBar.HAlign = 1f;
			extractorPanel.Append(powerBar);

			mudSlot = new UIExtractorSlot(mod.GetTexture("GadgetUI/ExtractorSlot"), Main.itemTexture[ItemID.MudBlock], () => ExtractorTE.Mud > 0);
			mudSlot.Top.Set(8, 0);
			mudSlot.Left.Set(52, 0);
			mudSlot.OnClick += (a,b) => ExtractorTE.ProvideMud();
			mudSlot.VAlign = 1;
			extractorPanel.Append(mudSlot);

			chloroSlot = new UIExtractorSlot(mod.GetTexture("GadgetUI/ExtractorSlot"), Main.itemTexture[ItemID.ChlorophyteOre], () => ExtractorTE.Chlorophyte < ChlorophyteExtractorTE.MaxResources);
			chloroSlot.Top.Set(8, 0);
			chloroSlot.Left.Set(12, 0);
			chloroSlot.OnClick += (a, b) => ExtractorTE.ExtractChloro();
			chloroSlot.VAlign = chloroSlot.HAlign = 1;
			extractorPanel.Append(chloroSlot);

			Append(extractorPanel);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			if (ContainsPoint(Main.MouseScreen))
				Main.LocalPlayer.mouseInterface = true;
			powerBar.SetProgress(ExtractorTE.Power / ChlorophyteExtractorTE.MaxResources);
		}

		internal static void OpenUI(GadgetPlayer gadgetPlayer)
		{
			if (PlayerInput.GrappleAndInteractAreShared)
				PlayerInput.Triggers.JustPressed.Grapple = false;
			visible = true;
			ExtractorTE = (ChlorophyteExtractorTE)TileEntity.ByID[gadgetPlayer.extractor];
			Main.playerInventory = true;
			Main.recBigList = false;
			Main.PlaySound(SoundID.MenuOpen);
		}

		internal static void CloseUI(GadgetPlayer gadgetPlayer, bool silent = false)
		{
			visible = false;
			gadgetPlayer.extractor = -1;
			ExtractorTE = new ChlorophyteExtractorTE();
			if (silent) return;
			if (PlayerInput.GrappleAndInteractAreShared)
				PlayerInput.Triggers.JustPressed.Grapple = false;
			Main.PlaySound(SoundID.MenuTick);
		}
	}
}
