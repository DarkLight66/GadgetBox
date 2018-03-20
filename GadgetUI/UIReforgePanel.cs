using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.UI;
using Terraria.UI.Chat;

namespace GadgetBox.GadgetUI
{
	internal class UIReforgePanel : UIPanel
	{
		Func<Item> _reforgeItem;
		Func<int> _reforgePrice;

		public UIReforgePanel(Func<Item> reforgeItem, Func<int> reforgePrice)
		{
			_reforgeItem = reforgeItem;
			_reforgePrice = reforgePrice;
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			base.DrawSelf(spriteBatch);
			CalculatedStyle style = GetDimensions();
			string priceText;
			Vector2 priceOffset = new Vector2(style.X + 68, style.Y + 26);
			if (!_reforgeItem().IsAir)
			{
				priceOffset += new Vector2(36, -12);
				priceText = Language.GetTextValue("LegacyInterface.46");
				ItemSlot.DrawMoney(spriteBatch, "", priceOffset.X + 10, priceOffset.Y - 42, Utils.CoinsSplit(Math.Max(_reforgePrice(), 1)), true);
				ItemSlot.DrawSavings(spriteBatch, priceOffset.X, priceOffset.Y - 12, true);
			}
			else
				priceText = Language.GetTextValue("LegacyInterface.20");
			ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Main.fontMouseText, priceText, priceOffset, new Color(Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor), 0f, Vector2.Zero, Vector2.One, -1f, 2f);
		}
	}
}
