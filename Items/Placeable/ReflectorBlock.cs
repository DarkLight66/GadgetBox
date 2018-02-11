using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using GadgetBox.Tiles;

namespace GadgetBox.Items.Placeable
{
    public class ReflectorBlock : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Reflects most projectiles");
            DisplayName.AddTranslation(GameCulture.Spanish, "Bloque Reflector");
            Tooltip.AddTranslation(GameCulture.Spanish, "Refleja la mayoría de los projectiles");
        }

        public override void SetDefaults()
        {
            item.CloneDefaults(ItemID.TeamBlockWhite);
            item.createTile = mod.TileType<ReflectorBlockTile>();
        }
    }
}