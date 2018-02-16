using Terraria;
using Microsoft.Xna.Framework;

namespace GadgetBox.Items.Tools
{
	public class PlatedShovel : BaseShovel
	{
		public override void SetShovelDefaults()
		{
			item.Size = new Vector2(40);
			item.damage = 14;
			item.knockBack = 6f;
			item.melee = true;
			item.useAnimation = 18;
			item.useTime = 13;
			item.value = Item.sellPrice(silver: 20);
			item.rare = 1;
			item.autoReuse = true;
			item.useTurn = true;
			shovel = 75;
		}
	}
}
