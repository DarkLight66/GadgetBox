using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;

namespace GadgetBox.GadgetUI
{
	internal class UIDrawAction : UIElement
	{
		Action<SpriteBatch> _drawAction;

		public UIDrawAction(Action<SpriteBatch> drawAction)
		{
			_drawAction = drawAction;
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			_drawAction(spriteBatch);
		}
	}
}