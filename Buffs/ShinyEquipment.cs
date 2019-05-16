using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace GadgetBox.Buffs
{
	public class ShinyEquipment : ModBuff
	{
		public override void Update(Player player, ref int buffIndex)
		{
			player.Gadget().shinyEquips = true;
		}

		public override void ModifyBuffTip(ref string tip, ref int rare)
		{
			Player player = Main.LocalPlayer;
			int[] effect = new int[6];
			for (int i = 0; i < 8 + player.extraAccessorySlots; i++)
			{
				Item item = player.armor[i];
				if (item.prefix < PrefixID.Hard || item.prefix > PrefixID.Violent)
				{
					continue;
				}
				if (item.prefix <= PrefixID.Warding)
				{
					effect[0] += (item.prefix - PrefixID.Hard + 1) * 2;
				}
				else if (item.prefix == PrefixID.Arcane)
				{
					effect[1] += 2;
				}
				else if (item.prefix <= PrefixID.Lucky)
				{
					effect[2] += item.prefix - PrefixID.Precise + 1;
				}
				else if (item.prefix <= PrefixID.Menacing)
				{
					effect[3] += (item.prefix - PrefixID.Jagged + 2) / 2;
				}
				else if (item.prefix <= PrefixID.Quick2)
				{
					effect[4] += item.prefix - PrefixID.Brisk + 1;
				}
				else
				{
					effect[5] += item.prefix - PrefixID.Wild + 1;
				}
			}
			tip += Environment.NewLine + mod.GetTextValue("Misc.CurrentlyGrants");
			string text = "";
			if (effect[0] > 0)
			{
				text += $"\n+{effect[0]} {Language.GetTextValue("LegacyTooltip.30")}";
			}
			if (effect[1] > 0)
			{
				text += $"\n-{effect[1]} {Language.GetTextValue("LegacyTooltip.42")}";
			}
			if (effect[2] > 0)
			{
				text += $"\n{mod.GetTextValue("Misc.CriticalDamage", effect[2])}";
			}
			if (effect[3] > 0)
			{
				text += $"\n{mod.GetTextValue("Misc.ArmorPenetration", effect[3])}";
			}
			if (effect[4] > 0)
			{
				text += $"\n{mod.GetTextValue("Misc.JumpHeight", effect[4])}";
			}
			if (effect[5] > 0)
			{
				text += $"\n{mod.GetTextValue("Misc.MiningSpeed", effect[5])}";
			}
			tip += text == "" ? "\n" + mod.GetTextValue("Misc.NoEffects") : text;
		}
	}
}