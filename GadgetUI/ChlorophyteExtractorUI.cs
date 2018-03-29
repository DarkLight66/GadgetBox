using GadgetBox.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace GadgetBox.GadgetUI
{
	internal class ChlorophyteExtractorUI : UIState
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
		int oldExtractorID;

		public override void OnInitialize()
		{
			mod = GadgetBox.Instance;
			ExtractorTE = new ChlorophyteExtractorTE();
			oldExtractorID = -1;

			extractorPanel = new UIPanel();
			extractorPanel.SetPadding(4);
			extractorPanel.BorderColor = new Color(18, 0, 26);
			extractorPanel.BackgroundColor = new Color(150, 64, 16);
			extractorPanel.Top.Set(Main.instance.invBottom + 60f, 0);
			extractorPanel.Left.Set(180, 0);
			extractorPanel.Width.Set(220, 0);
			extractorPanel.Height.Set(120, 0);

			powerCellSlot = new UIPowerSlot(mod.GetTexture("GadgetUI/PowerSlot_Closed"), mod.GetTexture("GadgetUI/PowerSlot_Open"), () => ExtractorTE.Power > 0);
			powerCellSlot.Top.Set(8, 0);
			powerCellSlot.Left.Set(8, 0);
			powerCellSlot.OnMouseDown += (a, b) => ExtractorTE.ProvidePower();
			extractorPanel.Append(powerCellSlot);

			powerButton = new UIDelayedPowerButton(mod.GetTexture("GadgetUI/PowerButton_ON"), mod.GetTexture("GadgetUI/PowerButton_OFF"), 240, () => ExtractorTE.IsON, () => ExtractorTE.CanTurnOn);
			powerButton.Top.Set(-16, 0);
			powerButton.Left.Set(10, 0);
			powerButton.VAlign = 1f;
			powerButton.OnMouseDown += (a, b) => ExtractorTE.TogglePower();
			extractorPanel.Append(powerButton);

			powerBar = new UIPowerBar(mod.GetTexture("GadgetUI/PowerBar"), mod.GetTexture("GadgetUI/PowerBarFill"), 6, 4);
			powerBar.Top.Set(8, 0);
			powerBar.Left.Set(-10, 0);
			powerBar.HAlign = 1f;
			extractorPanel.Append(powerBar);

			mudSlot = new UIExtractorSlot(mod.GetTexture("GadgetUI/ExtractorSlot"), Main.itemTexture[ItemID.MudBlock], () => ExtractorTE.Mud > 0, () => ExtractorTE.Mud < ChlorophyteExtractorTE.MaxResources);
			mudSlot.Top.Set(-8, 0);
			mudSlot.Left.Set(52, 0);
			mudSlot.OnMouseDown += (a, b) => ExtractorTE.ProvideMud();
			mudSlot.VAlign = 1;
			extractorPanel.Append(mudSlot);

			chloroSlot = new UIExtractorSlot(mod.GetTexture("GadgetUI/ExtractorSlot"), Main.itemTexture[ItemID.ChlorophyteOre], () => ExtractorTE.Chlorophyte > 0);
			chloroSlot.Top.Set(-8, 0);
			chloroSlot.Left.Set(-10, 0);
			chloroSlot.OnMouseDown += (a, b) => ExtractorTE.ExtractChloro();
			chloroSlot.VAlign = chloroSlot.HAlign = 1;
			extractorPanel.Append(chloroSlot);

			Append(extractorPanel);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			if (extractorPanel.ContainsPoint(Main.MouseScreen))
				Main.LocalPlayer.mouseInterface = true;
			float progress = (float)ExtractorTE.Power / ChlorophyteExtractorTE.MaxResources;
			powerBar.SetPercentage(progress, oldExtractorID == ExtractorTE.ID);
			oldExtractorID = ExtractorTE.ID;
			powerBar.HoverText = (int)(progress * 100) + "% Power";
			mudSlot.HoverText = (ExtractorTE.Mud > 0 ? ExtractorTE.Mud + "" : "Needs") + " Mud";
			chloroSlot.HoverText = (ExtractorTE.Chlorophyte < ChlorophyteExtractorTE.MaxResources ? ExtractorTE.Chlorophyte > 0 ? ExtractorTE.Chlorophyte + "" : "No" : "Full of") + " Chlorophyte";
		}
	}
}