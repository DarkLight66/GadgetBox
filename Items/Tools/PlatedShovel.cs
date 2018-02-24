﻿using Terraria;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Terraria.ID;

namespace GadgetBox.Items.Tools
{
	public class PlatedShovel : BaseShovel
	{
		public override void SetShovelDefaults()
		{
			item.Size = new Vector2(40);
			item.damage = 13;
			item.knockBack = 6f;
			item.melee = true;
			item.useAnimation = 19;
			item.useTime = 13;
			item.tileBoost = 1;
			item.value = Item.sellPrice(silver: 20);
			item.rare = 1;
			item.autoReuse = true;
			item.useTurn = true;
			item.UseSound = SoundID.Item1;
			shovel = 75;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(mod.ItemType<OldShovel>());
			recipe.AddRecipeGroup(GadgetBox.AnyGoldBar, 10);
			recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}
