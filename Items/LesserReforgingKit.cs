using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;

namespace GadgetBox.Items
{
    public class LesserReforgingKit : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.AddTranslation(GameCulture.Spanish, "Equipo de refuerzo menor");
            Tooltip.SetDefault("Grants a random prefix to your held item"
                + "\nOnly works on unprefixed items"
                + "\n<right> to use");
            Tooltip.AddTranslation(GameCulture.Spanish, "Le da un sufijo al azar a tu objeto en mano"
                + "\nSolo funciona con objetos sin sufijo"
                + "\n<right> para usar");
        }

        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 34;
            item.consumable = true;
            item.maxStack = 99;
            item.rare = 1;
            item.value = 0;
        }

        public override bool CanRightClick()
        {
            Item toPrefix = Main.LocalPlayer.HeldItem;
            return toPrefix != null && toPrefix.prefix == 0 && toPrefix.Prefix(-3) && ItemLoader.PreReforge(toPrefix);
        }

        public override void RightClick(Player player)
        {
            player.PrefixHeldItem();
        }
    }
}
