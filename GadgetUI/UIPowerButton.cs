using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;

namespace GadgetBox.GadgetUI
{
	internal class UIDelayedPowerButton : UIElement
	{
		public delegate bool StateCheck();
		Texture2D _onTexture;
		Texture2D _offTexture;
		StateCheck _isOn;
		StateCheck _canTurnOn;
		ushort _toggleDelay;
		ushort _toggleDelayCounter = 0;

		public UIDelayedPowerButton(Texture2D onTexture, Texture2D offTexture, ushort toogleDelay, StateCheck isOn, StateCheck canTurnOn)
		{
			_onTexture = onTexture;
			_offTexture = offTexture;
			_toggleDelay = toogleDelay;
			_isOn = isOn;
			_canTurnOn = canTurnOn;
			Width.Set(_onTexture.Width, 0f);
			Height.Set(_onTexture.Height, 0f);
		}

		public override void Click(UIMouseEvent evt)
		{
			if (_toggleDelayCounter <= 0 && (!_isOn() && _canTurnOn() || _isOn()))
			{
				base.Click(evt);
				_toggleDelayCounter = _toggleDelay;
			}
		}

		public override void Update(GameTime gameTime)
		{
			if (_toggleDelayCounter > 0)
				_toggleDelayCounter--;
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			Texture2D texture = _isOn() ? _onTexture : _offTexture;
			Color color = _toggleDelayCounter > 0 ? Color.Gray : IsMouseHovering ? Color.White : Color.Silver;
			spriteBatch.Draw(texture, GetDimensions().ToRectangle(), null, color);
		}
	}
}