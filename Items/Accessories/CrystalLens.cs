using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace GadgetBox.Items.Accessories
{
	public class CrystalLens : ModItem
	{
		public override void SetDefaults()
		{
			item.width = 20;
			item.height = 26;
			item.value = Item.sellPrice(silver: 50);
			item.rare = 4;
			item.accessory = true;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			if (!hideVisual)
			{
				player.Gadget().crystalLens = true;
			}
		}

		public override void UpdateInventory(Player player)
		{
			if (item.favorited)
			{
				UpdateAccessory(player, false);
			}
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.CrystalShard, 5);
			recipe.AddIngredient(ItemID.Lens, 5);
			recipe.AddIngredient(ItemID.SoulofLight, 5);
			recipe.AddTile(TileID.CrystalBall);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}
