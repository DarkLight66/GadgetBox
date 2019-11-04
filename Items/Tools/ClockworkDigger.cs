using GadgetBox.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace GadgetBox.Items.Tools
{
	public class ClockworkDigger : BaseShovel
	{
		public override void SetShovelDefaults()
		{
			item.Size = new Vector2(54, 24);
			item.useStyle = 5;
			item.damage = 37;
			item.knockBack = 4.75f;
			item.useAnimation = 22;
			item.useTime = 6;
			item.tileBoost = 1;
			item.value = Item.sellPrice(0, 5, 50);
			item.rare = 5;
			item.UseSound = SoundID.Item23;
			item.shoot = ProjectileType<ClockworkDiggerProj>();
			item.shootSpeed = 40;
			item.noMelee = true;
			item.noUseGraphic = true;
			item.channel = true;
			item.pick = 200;
			Shovel = 150;
		}
	}
}