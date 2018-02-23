using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace GadgetBox.GadgetUI
{
	internal class UIPowerSlot : UIElement
	{
		public delegate bool PowerCheck();
		private Texture2D _hasPowerTexture;
		private Texture2D _noPowerTexture;
		private PowerCheck _hasPower;
		
		public UIPowerSlot(Texture2D hasPowerTexture, Texture2D noPowerTexture, PowerCheck hasPower)
		{
			_hasPower = hasPower;
			_hasPowerTexture = hasPowerTexture;
			_noPowerTexture = noPowerTexture;
			Width.Set(_hasPowerTexture.Width, 0f);
			Height.Set(_hasPowerTexture.Height, 0f);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			CalculatedStyle dimensions = GetDimensions();
			Texture2D texture = _hasPower() ? _hasPowerTexture: _noPowerTexture;
			spriteBatch.Draw(texture, new Rectangle((int)dimensions.X, (int)dimensions.Y, (int)dimensions.Width, (int)dimensions.Height), null, Color.White);
		}

		public override void Click(UIMouseEvent evt)
		{
			if (!_hasPower())
				base.Click(evt);
		}
	}
}
