using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;

namespace GadgetBox.GadgetUI
{
	internal class UIPowerBar : UIHoverText
	{
		Texture2D _barTexture;
		Texture2D _fillTexture;
		float _powerPercentage;
		//float _targetPercentage;

		public UIPowerBar(Texture2D barTexture, Texture2D fillTexture, int HPadding, int VPadding)
		{
			_barTexture = barTexture;
			_fillTexture = fillTexture;
			Height.Set(barTexture.Height, 0);
			Width.Set(barTexture.Width, 0);
			PaddingLeft = PaddingRight = HPadding;
			PaddingBottom = PaddingTop = VPadding;
		}

		public void SetProgress(float value)
		{
			_powerPercentage = value;
		}

		//public void SetTargetProgress(float value)
		//{
		//	_targetPercentage = value;
		//	if (value < _powerPercentage)
		//		_powerPercentage = value;
		//}

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
					sourceRect.Width = (int)endWidth + 1;
				spriteBatch.Draw(_fillTexture, dimensions.Position(), sourceRect, Color.Gray);
				dimensions.X += _fillTexture.Width;
			}
			//_powerPercentage = _powerPercentage * 0.95f + 0.05f * _targetPercentage;
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
						sourceRect.Width = (int)endWidth + 1;
					spriteBatch.Draw(_fillTexture, dimensions.Position(), sourceRect, Color.White);
					dimensions.X += _fillTexture.Width;
				}
			}
			spriteBatch.Draw(_barTexture, GetDimensions().Position(), Color.White);
		}
	}
}
