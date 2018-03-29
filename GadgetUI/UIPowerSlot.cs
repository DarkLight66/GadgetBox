using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;

namespace GadgetBox.GadgetUI
{
	internal class UIPowerSlot : UIElement
	{
		private Texture2D _hasPowerTexture;
		private Texture2D _noPowerTexture;
		private Func<bool> _hasPower;

		public UIPowerSlot(Texture2D hasPowerTexture, Texture2D noPowerTexture, Func<bool> hasPower)
		{
			_hasPower = hasPower;
			_hasPowerTexture = hasPowerTexture;
			_noPowerTexture = noPowerTexture;
			Width.Set(_hasPowerTexture.Width, 0f);
			Height.Set(_hasPowerTexture.Height, 0f);
		}

		public override void MouseDown(UIMouseEvent evt)
		{
			if (!_hasPower())
				base.MouseDown(evt);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			CalculatedStyle dimensions = GetDimensions();
			Texture2D texture = _hasPower() ? _hasPowerTexture : _noPowerTexture;
			spriteBatch.Draw(texture, new Rectangle((int)dimensions.X, (int)dimensions.Y, (int)dimensions.Width, (int)dimensions.Height), null, Color.White);
		}
	}
}