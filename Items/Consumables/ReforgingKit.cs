using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace GadgetBox.Items.Consumables
{
	public class ReforgingKit : ItemOnRightClick
	{
		public override void SetDefaults()
		{
			item.width = 32;
			item.height = 38;
			item.consumable = true;
			item.maxStack = 99;
			item.rare = 4;
		}

		public override bool CanRightClick() => CanRightClick(Main.mouseItem, true);
		public override bool CanRightClick(Item item, bool byMouseItem) => item != null && !item.IsAir && item.Prefix(-3) && ItemLoader.PreReforge(item);

		public override void RightClick(Player player) => RightClick(ref Main.mouseItem, player, true);
		public override void RightClick(ref Item item, Player player, bool byMouseItem)
		{
			GadgetMethods.PrefixItem(ref item);
			if (byMouseItem)
			{
				player.inventory[58] = item.Clone();
			}
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemType<LesserReforgingKit>(), 2);
			recipe.AddIngredient(ItemType<RestoringKit>(), 2);
			recipe.AddTile(TileID.TinkerersWorkbench);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}