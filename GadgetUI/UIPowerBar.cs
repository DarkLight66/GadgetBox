using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;

namespace GadgetBox.GadgetUI
{
	internal class UIPowerBar : UIHoverText
	{
		private Texture2D _barTexture;
		private Texture2D _fillTexture;
		private float _powerPercentage;
		private float _targetPercentage;

		public UIPowerBar(Texture2D barTexture, Texture2D fillTexture, int HPadding, int VPadding)
		{
			_barTexture = barTexture;
			_fillTexture = fillTexture;
			Height.Set(barTexture.Height, 0);
			Width.Set(barTexture.Width, 0);
			PaddingLeft = PaddingRight = HPadding;
			PaddingBottom = PaddingTop = VPadding;
		}

		public void SetPercentage(float value, bool transition)
		{
			_targetPercentage = value;
			if (!transition)
			{
				_powerPercentage = value;
			}
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			base.DrawSelf(spriteBatch);
			CalculatedStyle dimensions = GetInnerDimensions();
			int drawAmount = (int)(dimensions.Width / _fillTexture.Width);
			float endWidth = dimensions.Width - _fillTexture.Width * drawAmount;
			Rectangle sourceRect = _fillTexture.Bounds;
			for (int i = drawAmount; i >= 0; i--)
			{
				if (i == 0)
				{
					sourceRect.Width = (int)endWidth + 1;
				}

				spriteBatch.Draw(_fillTexture, dimensions.Position(), sourceRect, Color.Gray);
				dimensions.X += _fillTexture.Width;
			}
			_powerPercentage = _powerPercentage * 0.95f + 0.05f * _targetPercentage;
			if (_powerPercentage > 0)
			{
				dimensions.X = GetInnerDimensions().X;
				dimensions.Width *= _powerPercentage;
				drawAmount = (int)(dimensions.Width / _fillTexture.Width);
				endWidth = dimensions.Width - _fillTexture.Width * drawAmount;
				sourceRect = _fillTexture.Bounds;
				for (int i = drawAmount; i >= 0; i--)
				{
					if (i == 0)
					{
						sourceRect.Width = (int)endWidth + 1;
					}

					spriteBatch.Draw(_fillTexture, dimensions.Position(), sourceRect, Color.White);
					dimensions.X += _fillTexture.Width;
				}
			}
			spriteBatch.Draw(_barTexture, GetDimensions().Position(), Color.White);
		}
	}
}