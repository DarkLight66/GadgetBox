using System;
using GadgetBox.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace GadgetBox.GadgetUI
{
	internal class ReforgeMachineUI : UIState
	{
		internal static bool visible = false;
		internal UIPanel reforgePanel;
		internal UIItemSlot reforgeSlot;
		internal UIFancyButton reforgeButton;
		internal Mod mod;
		int reforgePrice = 0;

		public override void OnInitialize()
		{
			mod = GadgetBox.Instance;

			reforgePanel = new UIReforgePanel(() => reforgeSlot.item, () => reforgePrice);
			reforgePanel.SetPadding(4);
			reforgePanel.Top.Set(Main.instance.invBottom + 60f, 0);
			reforgePanel.Left.Set(140, 0);
			reforgePanel.Width.Set(320, 0);
			reforgePanel.Height.Set(140, 0);

			reforgeSlot = new UIItemSlot(0.85f);
			reforgeSlot.Top.Set(12, 0);
			reforgeSlot.Left.Set(12, 0);
			reforgeSlot.CanClick += () => Main.mouseItem.type == 0 || Main.mouseItem.Prefix(-3);
			reforgeSlot.OnMouseDown += (a, b) => reforgePrice = reforgeSlot.item.ReforgePrice();
			reforgePanel.Append(reforgeSlot);

			reforgeButton = new UIFancyButton(Main.reforgeTexture[0], Main.reforgeTexture[1]);
			reforgeButton.Top.Set(20, 0);
			reforgeButton.Left.Set(64, 0);
			reforgeButton.CanClick += () => !reforgeSlot.item.IsAir && Main.LocalPlayer.CanBuyItem(reforgePrice, -1) && ItemLoader.PreReforge(reforgeSlot.item);
			reforgeButton.OnMouseDown += ReforgeItem;
			reforgeButton.HoverText = Language.GetTextValue("LegacyInterface.19");
			reforgePanel.Append(reforgeButton);

			Append(reforgePanel);
		}

		void ReforgeItem(UIMouseEvent evt, UIElement listeningElement)
		{
			Main.LocalPlayer.BuyItem(reforgePrice, -1);
			GadgetMethods.PrefixItem(ref reforgeSlot.item);
			reforgePrice = reforgeSlot.item.ReforgePrice();
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			if (reforgePanel.ContainsPoint(Main.MouseScreen))
				Main.LocalPlayer.mouseInterface = true;
			reforgeButton.visible = !reforgeSlot.item.IsAir;
		}

		internal void ToggleUI(bool showUI, Point16 centerPos, bool silent = false)
		{
			if (visible)
			{
				if (reforgeSlot.item?.type > 0)
				{
					Player player = Main.LocalPlayer;
					reforgeSlot.item.position = player.Center;
					Item item = player.GetItem(player.whoAmI, reforgeSlot.item, false, true);
					if (item.stack > 0)
					{
						int index = Item.NewItem(player.getRect(), item.type, item.stack, false, reforgeSlot.item.prefix, true, false);
						Main.item[index] = item.Clone();
						Main.item[index].newAndShiny = false;
						if (Main.netMode == NetmodeID.MultiplayerClient)
							NetMessage.SendData(MessageID.SyncItem, -1, -1, null, index, 1f);
					}
					reforgeSlot.item = new Item();
					reforgePrice = 0;
				}
				if (!silent)
					Main.PlaySound(showUI ? SoundID.MenuTick : SoundID.MenuClose);
			}
			else if (showUI)
			{
				Main.playerInventory = true;
				Main.recBigList = false;
				if (!silent)
					Main.PlaySound(SoundID.MenuOpen);
			}
			visible = showUI;
			if (visible)
				Main.LocalPlayer.GetModPlayer<GadgetPlayer>().machinePos = centerPos;
		}
	}
}