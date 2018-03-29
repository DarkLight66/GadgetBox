using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace GadgetBox.GadgetUI
{
	internal class InvisibleFixedUIScrollbar : FixedUIScrollbar
	{
		public InvisibleFixedUIScrollbar(UserInterface userInterface) : base(userInterface) { }

		protected override void DrawSelf(SpriteBatch spriteBatch) { }
	}
}