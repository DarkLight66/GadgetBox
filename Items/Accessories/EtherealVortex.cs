using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Terraria.Localization;

namespace GadgetBox.Items.Accessories
{
    public class EtherealVortex : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.AddTranslation(GameCulture.Spanish, "Vórtice etéreo");
            Tooltip.SetDefault("Increases pickup range for floating items");
            Tooltip.AddTranslation(GameCulture.Spanish, "Aumenta el alcance de recolección de objetos flotantes");
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
            player.GetModPlayer<GadgetPlayer>().etherMagnet = true;
        }
    }
}
