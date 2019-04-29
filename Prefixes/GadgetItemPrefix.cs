using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace GadgetBox.Prefixes
{
	public class GadgetItemPrefix : GlobalItem
	{
		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			if (item.prefix < 1 || !ToolPrefix.ToolPrefixes.Contains(item.prefix))
			{
				return;
			}

			if (item.tileBoost != Main.cpItem.tileBoost)
			{
				int ttindex = tooltips.FindLastIndex(t => (t.mod == "Terraria" || t.mod == mod.Name) && (t.isModifier || t.Name.StartsWith("Tooltip")
				|| t.Name.StartsWith("Material") || t.Name.StartsWith("TileBoost") || t.Name.EndsWith("Power")));
				if (ttindex != -1)
				{
					int tileBoost = item.tileBoost - Main.cpItem.tileBoost;
					TooltipLine tt = new TooltipLine(mod, "PrefixTileBoost", (tileBoost > 0 ? "+" : "") + tileBoost + Language.GetTextValue("LegacyTooltip.54"))
					{
						isModifier = true
					};
					if (tileBoost < 0)
					{
						tt.isModifierBad = true;
					}

					tooltips.Insert(ttindex + 1, tt);
				}
			}
		}
	}
}