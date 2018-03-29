using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;

namespace GadgetBox.GadgetUI
{
	internal class UIReforgeLabel : UILeftAlignedLabel
	{
		internal byte prefix;
		internal bool selected;

		static Dictionary<int, Color> rarityColors = new Dictionary<int, Color>()
		{
			[-11] = Colors.RarityAmber,
			[-1] = Colors.RarityTrash,
			[0] = Colors.RarityNormal,
			[1] = Colors.RarityBlue,
			[2] = Colors.RarityGreen,
			[3] = Colors.RarityOrange,
			[4] = Colors.RarityRed,
			[5] = Colors.RarityPink,
			[6] = Colors.RarityPurple,
			[7] = Colors.RarityLime,
			[8] = Colors.RarityYellow,
			[9] = Colors.RarityCyan,
			[10] = new Color(255, 40, 100),
			[11] = new Color(180, 40, 255)
		};

		int _rarity;
		int _value;

		public UIReforgeLabel(byte prefix = 0, int rarity = 0, int value = 0) :
			base(Lang.prefix[prefix].Value, Color.White)
		{
			this.prefix = prefix;
			_rarity = rarity;
			_value = value;
			if (rarityColors.ContainsKey(rarity))
				TextColor = rarityColors[rarity];
			if (Text.StartsWith("("))
				Text = Text.Split('(', ')')[1];
		}

		public override int CompareTo(object obj)
		{
			UIReforgeLabel other = obj as UIReforgeLabel;
			int diffSelected = -selected.CompareTo(other.selected);
			int diffValue = -_value.CompareTo(other._value);
			return diffSelected != 0 ? diffSelected : diffValue != 0 ? diffValue : prefix.CompareTo(other.prefix);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			BackgroundColor = selected ? Color.LightSkyBlue : Color.CornflowerBlue;
			if (_rarity == -12)
				TextColor = Main.DiscoColor;
			base.DrawSelf(spriteBatch);
		}
	}
}