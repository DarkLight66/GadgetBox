using Terraria;
using Terraria.ModLoader;

namespace GadgetBox.Items
{
	public abstract class ItemOnRightClick : ModItem
	{
		public abstract bool CanRightClick(Item item, bool byMouseItem);
		public abstract void RightClick(ref Item item, Player player, bool byMouseItem);
	}
}
