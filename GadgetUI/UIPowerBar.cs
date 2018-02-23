using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;

namespace GadgetBox.GadgetUI
{
	public class UIPowerBar : UIElement
	{
		Texture2D _barTexture;
		Texture2D _fillTexture;
		float _visualProgress;
		float _targetProgress;

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
			_targetProgress = value;
			if (value >= _visualProgress)
				_visualProgress = value;
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
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
			_visualProgress = _visualProgress * 0.95f + 0.05f * _targetProgress;
			dimensions.X = GetInnerDimensions().X;
			dimensions.Width *= _visualProgress;
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
			spriteBatch.Draw(_barTexture, GetDimensions().Position(), Color.White);
		}
	}
}
