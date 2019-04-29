using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;

namespace GadgetBox.GadgetUI
{
	internal class UIDelayedPowerButton : UIElement
	{
		Texture2D _turnOnTexture;
		Texture2D _turnOffTexture;
		Func<bool> _isOn;
		Func<bool> _canTurnOn;
		ushort _toggleDelay;
		ushort _toggleDelayCounter = 0;

		public UIDelayedPowerButton(Texture2D turnOffTexture, Texture2D turnOnTexture, ushort toogleDelay, Func<bool> isOn, Func<bool> canTurnOn)
		{
			_turnOffTexture = turnOffTexture;
			_turnOnTexture = turnOnTexture;
			_toggleDelay = toogleDelay;
			_isOn = isOn;
			_canTurnOn = canTurnOn;
			Width.Set(_turnOffTexture.Width, 0f);
			Height.Set(_turnOffTexture.Height, 0f);
		}

		public override void MouseDown(UIMouseEvent evt)
		{
			if (_toggleDelayCounter <= 0 && (!_isOn() && _canTurnOn() || _isOn()))
			{
				base.MouseDown(evt);
				_toggleDelayCounter = _toggleDelay;
			}
		}

		public override void Update(GameTime gameTime)
		{
			if (_toggleDelayCounter > 0)
			{
				_toggleDelayCounter--;
			}
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			Texture2D texture = _isOn() ? _turnOffTexture : _turnOnTexture;
			Color color = _toggleDelayCounter > 0 ? Color.Gray : IsMouseHovering ? Color.White : Color.Silver;
			spriteBatch.Draw(texture, GetDimensions().ToRectangle(), null, color);
		}
	}
}