using Terraria;

namespace GadgetBox.Items.Consumables
{
	public class RestoringKit : ItemOnRightClick
	{
		public override void SetDefaults()
		{
			item.width = 30;
			item.height = 34;
			item.consumable = true;
			item.maxStack = 99;
			item.rare = 2;
		}

		public override bool CanRightClick() => CanRightClick(Main.mouseItem, true);
		public override bool CanRightClick(Item item, bool byMouseItem) => item != null && !item.IsAir && item.prefix > 0;

		public override void RightClick(Player player) => RightClick(ref Main.mouseItem, player, true);
		public override void RightClick(ref Item item, Player player, bool byMouseItem)
		{
			GadgetMethods.PrefixItem(ref item, false, true);
			if (byMouseItem)
			{
				player.inventory[58] = item.Clone();
			}
		}
	}
}
