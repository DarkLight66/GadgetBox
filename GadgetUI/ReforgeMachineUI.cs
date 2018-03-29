using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace GadgetBox.GadgetUI
{
	internal class ReforgeMachineUI : UIState
	{
		internal static bool visible = false;
		internal UIPanel reforgePanel;
		internal UIItemSlot reforgeSlot;
		internal UIFancyButton reforgeButton;
		internal UIPanel reforgeListPanel;
		internal UIList reforgeList;
		internal Mod mod;

		List<byte> selectedPrefixes = new List<byte>();
		int reforgePrice;
		bool autoReforge;
		int reforgeTries;
		byte tickCounter;

		public override void OnInitialize()
		{
			mod = GadgetBox.Instance;

			reforgePanel = new UIReforgePanel(() => reforgeSlot.item, () => reforgePrice);
			reforgePanel.SetPadding(4);
			reforgePanel.Top.Pixels = Main.instance.invBottom + 60;
			reforgePanel.Left.Pixels = 154;
			reforgePanel.MinHeight.Pixels = 260;

			reforgeSlot = new UIItemSlot(0.85f);
			reforgeSlot.Top.Pixels = reforgeSlot.Left.Pixels = 12;
			reforgeSlot.CanClick += () => Main.mouseItem.type == 0 || Main.mouseItem.Prefix(-3);
			reforgeSlot.OnMouseDown += (a, b) => { selectedPrefixes.Clear(); OnItemChanged(); };
			reforgePanel.Append(reforgeSlot);

			reforgeButton = new UIFancyButton(Main.reforgeTexture[0], Main.reforgeTexture[1]);
			reforgeButton.Top.Pixels = 20;
			reforgeButton.Left.Pixels = 64;
			reforgeButton.CanClick += CanReforgeItem;
			reforgeButton.OnMouseDown += OnReforgeButtonClick;
			reforgeButton.HoverText = Language.GetTextValue("LegacyInterface.19");
			reforgePanel.Append(reforgeButton);

			reforgeListPanel = new UIPanel();
			reforgeListPanel.Top.Pixels = 70;
			reforgeListPanel.Left.Pixels = 12;
			reforgeListPanel.Width.Set(-24, 1);
			reforgeListPanel.Height.Set(-82, 1);
			reforgeListPanel.SetPadding(6);
			reforgeListPanel.BackgroundColor = Color.CadetBlue;
			reforgePanel.Append(reforgeListPanel);

			reforgeList = new UIList();
			reforgeList.Width.Precent = reforgeList.Height.Precent = 1f;
			reforgeList.Width.Pixels = -24;
			reforgeList.ListPadding = 2;
			reforgeListPanel.Append(reforgeList);

			var reforgeListScrollbar = new FixedUIScrollbar(GadgetBox.Instance.reforgeMachineInterface);
			reforgeListScrollbar.SetView(100f, 1000f);
			reforgeListScrollbar.Top.Pixels = 4;
			reforgeListScrollbar.Height.Set(-8, 1f);
			reforgeListScrollbar.Left.Set(-20, 1f);
			reforgeListPanel.Append(reforgeListScrollbar);
			reforgeList.SetScrollbar(reforgeListScrollbar);

			Append(reforgePanel);
		}

		public override void Update(GameTime gameTime)
		{
			reforgePrice = reforgeSlot.item.ReforgePrice();
			if (autoReforge)
			{
				if (selectedPrefixes.Count == 0 || selectedPrefixes.Contains(reforgeSlot.item.prefix) || !CanReforgeItem())
				{
					autoReforge = false;
					reforgeTries = tickCounter = 0;
				}
				else if (++tickCounter > 9)
				{
					tickCounter = 0;
					ReforgeItem();
					if (selectedPrefixes.Contains(reforgeSlot.item.prefix) || ++reforgeTries > 200)
					{
						autoReforge = false;
						reforgeTries = tickCounter = 0;
					}
				}
			}
			base.Update(gameTime);
		}

		internal void ToggleUI(bool showUI, Point16 centerPos, bool silent = false)
		{
			Player player = Main.LocalPlayer;
			GadgetPlayer gadgetPlayer = player.Gadget();
			bool switching = visible && centerPos != Point16.NegativeOne && centerPos != gadgetPlayer.machinePos;
			if (visible)
			{
				if (reforgeSlot.item?.type > 0)
				{
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
					selectedPrefixes.Clear();
					OnItemChanged();
				}
				if (!silent)
					Main.PlaySound(switching ? SoundID.MenuTick : SoundID.MenuClose);
			}
			else if (showUI)
			{
				Main.playerInventory = true;
				Main.recBigList = false;
				if (!silent)
					Main.PlaySound(SoundID.MenuOpen);
			}

			if (!switching)
				visible = showUI;
			if (visible)
				gadgetPlayer.machinePos = centerPos;
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			if (reforgePanel.ContainsPoint(Main.MouseScreen))
			{
				Main.LocalPlayer.mouseInterface = true;
				Main.HoverItem.TurnToAir();
				Main.hoverItemName = "";
			}
			reforgeButton.visible = !reforgeSlot.item.IsAir;
		}

		void OnReforgeButtonClick(UIMouseEvent evt, UIElement listeningElement)
		{
			if (autoReforge)
			{
				autoReforge = false;
				reforgeTries = tickCounter = 0;
			}
			else if (selectedPrefixes.Count > 0)
				autoReforge = true;
			else
				ReforgeItem();
		}

		bool CanReforgeItem() => !reforgeSlot.item.IsAir && Main.LocalPlayer.CanBuyItem(reforgePrice, -1) && ItemLoader.PreReforge(reforgeSlot.item);

		void OnItemChanged()
		{
			reforgeList.Clear();
			if (reforgeSlot.item.IsAir || !ItemLoader.PreReforge(reforgeSlot.item))
				return;
			Item controlItem = new Item();
			UIReforgeLabel reforgeLabel;
			List<byte> tempSelected = new List<byte>();
			bool isArmor = false;
			for (byte i = 1; i < ModPrefix.PrefixCount; i++)
			{
				controlItem.SetDefaults(reforgeSlot.item.type, true);
				isArmor = ModCompat.ArmorPrefix(controlItem);
				if (isArmor && !controlItem.accessory)
					controlItem.accessory = true;
				if (!controlItem.CanApplyPrefix(i))
					continue;
				controlItem.Prefix(i);
				if (isArmor)
					ModCompat.ApplyArmorPrefix(controlItem, i);
				if (controlItem.prefix != i)
					continue;
				reforgeLabel = new UIReforgeLabel(i, controlItem.expert ? -12 : controlItem.rare, controlItem.value);
				reforgeLabel.OnMouseDown += ChoseReforge;
				reforgeLabel.SetPadding(10);
				if (selectedPrefixes.Contains(i))
				{
					reforgeLabel.selected = true;
					tempSelected.Add(i);
				}
				reforgeList.Add(reforgeLabel);
			}
			selectedPrefixes = tempSelected;
		}

		void ChoseReforge(UIMouseEvent evt, UIElement listeningElement)
		{
			UIReforgeLabel element = ((UIReforgeLabel)listeningElement);
			element.selected = !element.selected;
			if (!selectedPrefixes.Remove(element.prefix))
				selectedPrefixes.Add(element.prefix);
			reforgeList.UpdateOrder();
			Main.PlaySound(SoundID.MenuTick);
		}

		void ReforgeItem()
		{
			Main.LocalPlayer.BuyItem(reforgePrice, -1);
			GadgetMethods.PrefixItem(ref reforgeSlot.item);
			OnItemChanged();
		}
	}
}