using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;

namespace GadgetBox.GadgetUI
{
	internal class UIExtractorSlot : UIHoverText
	{
		Texture2D _slotTexture;
		Texture2D _itemTexture;
		Func<bool> _hasItem;
		Func<bool> _canClick;

		public UIExtractorSlot(Texture2D slotTexture, Texture2D itemTexture, Func<bool> hasItem, Func<bool> canClick = null)
		{
			_hasItem = hasItem;
			_canClick = canClick ?? hasItem;
			_slotTexture = slotTexture;
			_itemTexture = itemTexture;
			Width.Set(_slotTexture.Width, 0f);
			Height.Set(_slotTexture.Height, 0f);
		}

		public override void MouseDown(UIMouseEvent evt)
		{
			if (_canClick())
			{
				base.MouseDown(evt);
			}
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			base.DrawSelf(spriteBatch);
			CalculatedStyle dimensions = GetDimensions();
			Vector2 drawOrigin = _itemTexture.Size() * .5f;
			spriteBatch.Draw(_slotTexture, dimensions.ToRectangle(), null, Color.White);
			if (_hasItem())
			{
				spriteBatch.Draw(_itemTexture, dimensions.Center() - _itemTexture.Size() * .5f, null, Color.White);
			}
		}
	}
}