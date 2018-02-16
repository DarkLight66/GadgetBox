using Terraria;
using Microsoft.Xna.Framework;

namespace GadgetBox.Items.Tools
{
	public class OldShovel : BaseShovel
	{
		public override void SetShovelDefaults()
		{
			item.Size = new Vector2(34);
			item.damage = 8;
			item.knockBack = 4f;
			item.melee = true;
			item.useAnimation = 23;
			item.useTime = 15;
			item.value = Item.buyPrice(silver: 1);
			item.rare = 0;
			item.autoReuse = true;
			item.useTurn = true;
			shovel = 50;
		}
	}
}
