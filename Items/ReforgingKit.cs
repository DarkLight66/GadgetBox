﻿using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;

namespace GadgetBox.Items
{
    public class ReforgingKit : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.AddTranslation(GameCulture.Spanish, "Equipo de refuerzo");
            Tooltip.SetDefault("Grants a random prefix to your held item"
                + "\n<right> to use");
            Tooltip.AddTranslation(GameCulture.Spanish, "Le da un sufijo al azar a tu objeto en mano"
                + "\n<right> para usar");
        }

        public override void SetDefaults()
        {
            item.width = 32;
            item.height = 38;
            item.consumable = true;
            item.maxStack = 99;
            item.rare = 4;
            item.value = 0;
        }

        public override bool CanRightClick()
        {
            Item toPrefix = Main.LocalPlayer.HeldItem;
            return toPrefix != null && toPrefix.Prefix(-3) && ItemLoader.PreReforge(toPrefix);
        }

        public override void RightClick(Player player)
        {
            player.PrefixHeldItem();
        }
    }
}
