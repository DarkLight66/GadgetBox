using Terraria;
using Terraria.ModLoader;
using Terraria.Utilities;
using GadgetBox.Items.Tools;
using System.Collections.Generic;
using Terraria.Localization;

namespace GadgetBox.Prefixes
{
	public class GadgetItemPrefix : GlobalItem
	{
		public override int ChoosePrefix(Item item, UnifiedRandom rand)
		{
			if (item.maxStack > 1 || item.damage < 1 || (item.pick < 1 && item.axe < 1 && item.hammer < 1 &&
				(!(item.modItem is BaseShovel) || ((BaseShovel)item.modItem).shovel < 1)) || rand.NextBool(2))
				return -1;
			var ToolPrefixes = new List<ToolPrefix>(ToolPrefix.ToolPrefixes);
			if (item.tileBoost < 0)
				ToolPrefixes.RemoveAll(p => p.tileBoost < 0);
			if (item.noUseGraphic)
				ToolPrefixes.RemoveAll(p => p.useTimeMult != 1);
			return rand.Next(ToolPrefixes).Type;
		}

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			if (item.prefix < 1 || !ToolPrefix.ToolPrefixes.Exists(p => p.Type == item.prefix))
				return;
			if (item.tileBoost != Main.cpItem.tileBoost)
			{
				int ttindex = tooltips.FindLastIndex(t => (t.mod == "Terraria" || t.mod == mod.Name) && (t.isModifier || t.Name.StartsWith("Tooltip") 
				|| t.Name.StartsWith("Material") || t.Name.StartsWith("TileBoost") || t.Name.EndsWith("Power")));
				if (ttindex != -1)
				{
					int tileBoost = item.tileBoost - Main.cpItem.tileBoost;
					TooltipLine tt = new TooltipLine(mod, "PrefixTileBoost", (tileBoost > 0 ? "+" : "") + tileBoost + Language.GetTextValue("LegacyTooltip.54"));
					tt.isModifier = true;
					if (tileBoost < 0)
						tt.isModifierBad = true;
					tooltips.Insert(ttindex + 1, tt);
				}
			}
		}
	}
}
