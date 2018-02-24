using Terraria;
using Terraria.ModLoader;

namespace GadgetBox.Items.Accessories
{
	public class CritterNetAttachment : ModItem
	{
		public override void SetDefaults()
		{
			item.width = 30;
			item.height = 26;
			item.accessory = true;
			item.value = Item.sellPrice(gold: 2);
			item.rare = 3;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.GetModPlayer<GadgetPlayer>().critterCatch = true;
		}
	}
}