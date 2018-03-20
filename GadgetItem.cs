using System.Collections.Generic;
using GadgetBox.Items.Accessories;
using GadgetBox.Items.Consumables;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace GadgetBox
{
	public class GadgetItem : GlobalItem
	{
		public override void GrabRange(Item item, Player player, ref int grabRange)
		{
			if (ItemID.Sets.ItemNoGravity[item.type] && player.GetModPlayer<GadgetPlayer>().etherMagnet)
				grabRange += grabRange > Player.defaultItemGrabRange ? 100 : 500;
			if (item.makeNPC > 0 && player.GetModPlayer<GadgetPlayer>().critterCatch)
				grabRange += grabRange > Player.defaultItemGrabRange ? 70 : 300;
		}

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			if (item.prefix < PrefixID.Hard || item.prefix > PrefixID.Violent)
				return;

			GadgetPlayer modPlayer = Main.LocalPlayer.GetModPlayer<GadgetPlayer>();
			if (!modPlayer.shinyEquips)
				return;

			int index = tooltips.FindLastIndex(tt => tt.mod == "Terraria" && tt.Name.StartsWith("PrefixAcc"));
			if (index > -1)
			{
				string text = "";
				if (item.prefix <= PrefixID.Warding)
					text = "+" + ((item.prefix - PrefixID.Hard + 1) * 2) + " " + Language.GetTextValue("LegacyTooltip.30");
				else if (item.prefix == PrefixID.Arcane)
					text = "-2" + Language.GetTextValue("LegacyTooltip.42");
				else if (item.prefix <= PrefixID.Lucky)
					text = "+" + (item.prefix - PrefixID.Precise + 1) + mod.GetTextValue("CriticalDamage");
				else if (item.prefix <= PrefixID.Menacing)
					text = "+" + ((item.prefix - PrefixID.Jagged + 2) / 2) + mod.GetTextValue("ArmorPenetration");
				else if (item.prefix <= PrefixID.Quick2)
					text = "+" + (item.prefix - PrefixID.Brisk + 1) + " " + mod.GetTextValue("JumpHeight");
				else
					text = "+" + (item.prefix - PrefixID.Wild + 1) + " " + mod.GetTextValue("MiningSpeed");
				TooltipLine tt = new TooltipLine(mod, "ShinyAcc", text);
				tt.isModifier = true;
				tooltips.Insert(index + 1, tt);
			}
		}

		public override void OpenVanillaBag(string context, Player player, int arg)
		{
			if (context == "bossBag" && arg == ItemID.WallOfFleshBossBag && Main.rand.NextBool(5))
				player.QuickSpawnItem(mod.ItemType<EtherealVortex>());
			if (context == "crate")
			{
				int chance = !Main.hardMode ? 50 : NPC.downedMechBossAny ? 20 : 35;
				if (arg == ItemID.WoodenCrate)
					chance += (chance / 2);
				else if (arg == ItemID.GoldenCrate)
					chance /= 2;
				if (Main.rand.NextBool((int)chance))
					player.QuickSpawnItem(mod.ItemType<LesserReforgingKit>(), Main.rand.NextBool(4) ? 2 : 1);
				else if (Main.hardMode && Main.rand.NextBool(chance * 2))
					player.QuickSpawnItem(mod.ItemType<ReforgingKit>(), Main.rand.NextBool(8) ? 2 : 1);
			}
		}

		public override void UpdateEquip(Item item, Player player)
		{
			if (item.prefix < PrefixID.Hard || item.prefix > PrefixID.Violent)
				return;

			GadgetPlayer modPlayer = player.GetModPlayer<GadgetPlayer>();
			if (!modPlayer.shinyEquips)
				return;

			if (item.prefix <= PrefixID.Warding)
				player.statLifeMax2 += (item.prefix - PrefixID.Hard + 1) * 2;
			else if (item.prefix == PrefixID.Arcane)
				player.manaCost -= 0.02f;
			else if (item.prefix <= PrefixID.Lucky)
				modPlayer.critShine += (byte)(item.prefix - PrefixID.Precise + 1);
			else if (item.prefix <= PrefixID.Menacing)
				player.armorPenetration += (item.prefix - PrefixID.Jagged + 2) / 2;
			else if (item.prefix <= PrefixID.Quick2)
				modPlayer.speedShine += (byte)(item.prefix - PrefixID.Brisk + 1);
			else
				player.pickSpeed -= (item.prefix - PrefixID.Wild + 1) * 0.01f;
		}
	}
}