using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace GadgetBox.Items.Tools
{
	public class OldShovel : BaseShovel
	{
		public override void SetShovelDefaults()
		{
			item.Size = new Vector2(34);
			item.damage = 6;
			item.knockBack = 4f;
			item.melee = true;
			item.useAnimation = 23;
			item.useTime = 15;
			item.value = Item.buyPrice(silver: 10);
			item.autoReuse = true;
			item.useTurn = true;
			item.UseSound = SoundID.Item1;
			shovel = 50;
		}
	}
}