﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace GadgetBox.Items.Accessories
{
	public class EtherealVortex : ModItem
	{
		public override void SetStaticDefaults()
		{
			Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(8, 3));
		}

		public override void SetDefaults()
		{
			item.Size = new Vector2(28);
			item.accessory = true;
			item.value = Item.sellPrice(gold: 2);
			item.rare = 4;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.Gadget().etherMagnet = true;
		}
	}
}