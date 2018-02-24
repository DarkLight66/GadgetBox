using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace GadgetBox.GadgetUI
{
	internal class UIExtractorSlot : UIHoverText
	{
		public delegate bool ItemCheck();
		Texture2D _slotTexture;
		Texture2D _itemTexture;
		ItemCheck _hasItem;
		ItemCheck _canClick;

		public UIExtractorSlot(Texture2D slotTexture, Texture2D itemTexture, ItemCheck hasItem, ItemCheck canClick = null)
		{
			_hasItem = hasItem;
			_canClick = canClick ?? hasItem;
			_slotTexture = slotTexture;
			_itemTexture = itemTexture;
			Width.Set(_slotTexture.Width, 0f);
			Height.Set(_slotTexture.Height, 0f);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			CalculatedStyle dimensions = GetDimensions();
			Vector2 drawOrigin = _itemTexture.Size() * .5f;
			spriteBatch.Draw(_slotTexture, dimensions.ToRectangle(), null, Color.White);
			if (_hasItem())
				spriteBatch.Draw(_itemTexture, dimensions.Center() - _itemTexture.Size() * .5f, null, Color.White);
		}

		public override void Click(UIMouseEvent evt)
		{
			if (_canClick())
				base.Click(evt);
		}
	}
}
