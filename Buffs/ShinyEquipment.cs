using Terraria;
using Terraria.ModLoader;

namespace GadgetBox.Buffs
{
    public class ShinyEquipment : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Shiny Equipment");
            Description.SetDefault("Equipment grants extra effects depending on their prefix");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<GadgetPlayer>().shinyEquips = true;
        }
    }
}
