﻿using System.Collections.Generic;
using GadgetBox.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using static Terraria.ModLoader.ModContent;

namespace GadgetBox.GadgetUI
{
	internal class ReforgeMachineUI : UIState
	{
		internal UIPanel reforgePanel;
		internal UIItemSlot reforgeSlot;
		internal UIFancyButton reforgeButton;
		internal UIPanel reforgeListPanel;
		internal UIList reforgeList;
		private List<byte> selectedPrefixes = new List<byte>();
		private int reforgePrice;
		private int reforgeTries;
		private bool autoReforge;
		private byte tickCounter;

		public override void OnInitialize()
		{
			Main.recBigList = false;
			Main.playerInventory = true;

			reforgePanel = new UIReforgePanel(() => reforgeSlot.item, () => reforgePrice);
			reforgePanel.SetPadding(4);
			reforgePanel.Top.Pixels = Main.instance.invBottom + 60;
			reforgePanel.Left.Pixels = 110;
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

		public override void OnDeactivate()
		{
			if (!reforgeSlot.item.IsAir)
			{
				Main.LocalPlayer.QuickSpawnClonedItem(reforgeSlot.item, reforgeSlot.item.stack);
				reforgeSlot.item.TurnToAir();
			}
			Main.LocalPlayer.Gadget().machinePos = Point16.NegativeOne;
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			Player player = Main.LocalPlayer;
			Point16 machinePos = player.Gadget().machinePos;
			bool closeUI = false, silent = false;
			if (!Main.playerInventory || player.chest != -1 || Main.npcShop != 0 || player.talkNPC != -1 || player.sign >= 0)
			{
				closeUI = true;
				silent = true;
			}
			else if (Framing.GetTileSafely(machinePos).type != TileType<AutoReforgeMachineTile>() ||
				player.OutOfTileBounds(machinePos, Player.tileRangeX + 1, Player.tileRangeY + 1, Player.tileRangeX + 2, Player.tileRangeY + 1))
			{
				closeUI = true;
			}

			if (closeUI)
			{
				if (!silent)
				{
					Main.PlaySound(SoundID.MenuClose);
				}
				GadgetBox.Instance.reforgeMachineInterface.SetState(null);
				return;
			}

			reforgePrice = reforgeSlot.item.ReforgePrice();
			if (autoReforge)
			{
				tickCounter++;
				reforgeButton.Rotation += 0.2f;
				if (selectedPrefixes.Count == 0 || selectedPrefixes.Contains(reforgeSlot.item.prefix) || !CanReforgeItem())
				{
					autoReforge = false;
					reforgeTries = tickCounter = 0;
				}
				else if (tickCounter > 9)
				{
					tickCounter = 0;
					reforgeTries++;
					ReforgeItem();
					if (selectedPrefixes.Contains(reforgeSlot.item.prefix) || reforgeTries > 200)
					{
						autoReforge = false;
						reforgeTries = tickCounter = 0;
					}
				}
			}
			else if (reforgeButton.Rotation != 0)
			{
				if (reforgeButton.Rotation > MathHelper.TwoPi)
				{
					reforgeButton.Rotation %= MathHelper.TwoPi;
				}
				reforgeButton.Rotation = MathHelper.TwoPi - reforgeButton.Rotation <= 0.2f ? 0 : reforgeButton.Rotation + 0.2f;
			}
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			if (reforgePanel.ContainsPoint(Main.MouseScreen))
			{
				Main.LocalPlayer.mouseInterface = true;
				Main.HoverItem.TurnToAir();
				Main.hoverItemName = "";
			}
			Main.HidePlayerCraftingMenu = true;
			reforgeButton.Visible = !reforgeSlot.item.IsAir;
		}

		private void OnReforgeButtonClick(UIMouseEvent evt, UIElement listeningElement)
		{
			if (autoReforge)
			{
				autoReforge = false;
				reforgeTries = tickCounter = 0;
			}
			else if (selectedPrefixes.Count > 0)
			{
				autoReforge = true;
			}
			else
			{
				ReforgeItem();
			}
		}

		private bool CanReforgeItem() => !reforgeSlot.item.IsAir && !selectedPrefixes.Contains(reforgeSlot.item.prefix) &&
			Main.LocalPlayer.CanBuyItem(reforgePrice, -1) && ItemLoader.PreReforge(reforgeSlot.item);

		private void OnItemChanged()
		{
			reforgeList.Clear();
			if (reforgeSlot.item.IsAir)
			{
				return;
			}

			Item controlItem = reforgeSlot.item.Clone();
			if (!ItemLoader.PreReforge(controlItem))
			{
				return;
			}
			controlItem.netDefaults(reforgeSlot.item.netID);
			controlItem = controlItem.CloneWithModdedDataFrom(reforgeSlot.item);

			UIReforgeLabel reforgeLabel;
			List<byte> tempSelected = new List<byte>();
			bool isArmor = false;
			for (byte i = 1; i < ModPrefix.PrefixCount; i++)
			{
				Item tempItem = controlItem.Clone();

				isArmor = ModCompat.ArmorPrefix(tempItem);
				if (isArmor && !tempItem.accessory)
				{
					tempItem.accessory = true;
				}

				if (!tempItem.CanApplyPrefix(i))
				{
					continue;
				}

				tempItem.Prefix(i);
				if (isArmor)
				{
					ModCompat.ApplyArmorPrefix(tempItem, i);
				}

				if (tempItem.prefix != i)
				{
					continue;
				}

				reforgeLabel = new UIReforgeLabel(tempItem);
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

		private void ChoseReforge(UIMouseEvent evt, UIElement listeningElement)
		{
			UIReforgeLabel element = ((UIReforgeLabel)listeningElement);
			element.selected = !element.selected;
			if (!selectedPrefixes.Remove(element.shownItem.prefix))
			{
				selectedPrefixes.Add(element.shownItem.prefix);
			}
			reforgeList.UpdateOrder();
			Main.PlaySound(SoundID.MenuTick);
		}

		private void ReforgeItem()
		{
			Main.LocalPlayer.BuyItem(reforgePrice, -1);
			GadgetMethods.PrefixItem(ref reforgeSlot.item);
			OnItemChanged();
		}
	}
}