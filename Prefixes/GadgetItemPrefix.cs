using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace GadgetBox.Prefixes
{
	public class GadgetItemPrefix : GlobalItem
	{
		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			if (item.prefix < PrefixID.Count || !ToolPrefix.ToolPrefixes.Contains(item.prefix))
			{
				return;
			}

			if (item.tileBoost != Main.cpItem.tileBoost)
			{
				int ttindex = tooltips.FindLastIndex(t => (t.mod == "Terraria" || t.mod == mod.Name) && (t.isModifier || 
				t.Name.StartsWith("Tooltip") || t.Name.Equals("Material") || t.Name.Equals("TileBoost") || t.Name.EndsWith("Power")));
				if (ttindex != -1)
				{
					int tileBoost = item.tileBoost - Main.cpItem.tileBoost;
					TooltipLine tt = new TooltipLine(mod, "PrefixTileBoost", (tileBoost > 0 ? "+" : "") + tileBoost + Language.GetTextValue("LegacyTooltip.54"))
					{
						isModifier = true,
						isModifierBad = tileBoost < 0
					};
					tooltips.Insert(ttindex + 1, tt);
				}
			}
		}
	}
}