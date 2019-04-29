using GadgetBox.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace GadgetBox.Items.Tools
{
	public class AutoReelingRod : ModItem
	{
		public override void SetDefaults()
		{
			item.CloneDefaults(ItemID.MechanicsRod);
			item.shoot = mod.ProjectileType<AutoBobber>();
			item.rare = ItemRarityID.Pink;
			item.value = Item.buyPrice(gold: 20);
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			player.Gadget().autoReelAim = new Vector2(speedX, speedY).SafeNormalize(-Vector2.UnitY);
			return true;
		}
	}
}

