using System;
using System.Collections.Generic;
using System.IO;
using GadgetBox.Items.Accessories;
using GadgetBox.Items.Consumables;
using GadgetBox.Tiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace GadgetBox
{
	public class GadgetItem : GlobalItem
	{
		internal TweakType tweak = TweakType.None;
		public override bool InstancePerEntity => true;
		public override bool CloneNewInstances => true;

		public override void GrabRange(Item item, Player player, ref int grabRange)
		{
			if (ItemID.Sets.ItemNoGravity[item.type] && player.Gadget().etherMagnet)
			{
				grabRange += grabRange > Player.defaultItemGrabRange ? 100 : 500;
			}
			if (item.makeNPC > 0 && player.Gadget().critterCatch)
			{
				grabRange += grabRange > Player.defaultItemGrabRange ? 70 : 300;
			}
		}

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			int index;
			if (item.accessory && tweak != TweakType.None)
			{
				index = tooltips.FindIndex(tt => tt.mod == "Terraria" && tt.Name == "Equipable");
				if (index != -1)
				{
					TooltipLine tooltip = new TooltipLine(mod, "TweakName", mod.GetTextValue($"TweakName.{tweak}"))
					{
						overrideColor = tweak == TweakType.Malleable ? new Color(152, 76, 26) : Color.Goldenrod
					};
					tooltips.Insert(index + 1, tooltip);
					index = tooltips.FindIndex(tt => tt.mod == "Terraria" && (tt.Name == "SetBonus" ||
					tt.Name == "Expert" || tt.Name == "SpecialPrice" || tt.Name == "Price"));
					if (index == -1)
					{
						index = tooltips.Count;
					}
					tooltip = new TooltipLine(mod, "TweakTooltipGood", mod.GetTextValue($"TweakTooltipGood.{tweak}"))
					{
						isModifier = true
					};
					tooltips.Insert(index++, tooltip);
					tooltip = new TooltipLine(mod, "TweakTooltipBad", mod.GetTextValue($"TweakTooltipBad.{tweak}"))
					{
						isModifier = true,
						isModifierBad = true
					};
					tooltips.Insert(index, tooltip);
				}
			}

			if (item.prefix < PrefixID.Hard || item.prefix > PrefixID.Violent ||!Main.LocalPlayer.Gadget().shinyEquips)
			{
				return;
			}

			index = tooltips.FindLastIndex(tt => tt.mod == "Terraria" && tt.Name.StartsWith("PrefixAcc"));
			if (index > -1)
			{
				string text = "";
				if (item.prefix <= PrefixID.Warding)
				{
					text = $"+{(item.prefix - PrefixID.Hard + 1) * 2} {Language.GetTextValue("LegacyTooltip.30")}";
				}
				else if (item.prefix == PrefixID.Arcane)
				{
					text = $"-2{Language.GetTextValue("LegacyTooltip.42")}";
				}
				else if (item.prefix <= PrefixID.Lucky)
				{
					text = mod.GetTextValue("Misc.CriticalDamage", item.prefix - PrefixID.Precise + 1);
				}
				else if (item.prefix <= PrefixID.Menacing)
				{
					text = mod.GetTextValue("Misc.ArmorPenetration", (item.prefix - PrefixID.Jagged + 2) / 2);
				}
				else if (item.prefix <= PrefixID.Quick2)
				{
					text = mod.GetTextValue("Misc.JumpHeight", item.prefix - PrefixID.Brisk + 1);
				}
				else
				{
					text = mod.GetTextValue("Misc.MiningSpeed", item.prefix - PrefixID.Wild + 1);
				}

				TooltipLine tt = new TooltipLine(mod, "ShinyAcc", text)
				{
					isModifier = true
				};
				tooltips.Insert(index + 1, tt);
			}
		}

		public override void OpenVanillaBag(string context, Player player, int arg)
		{
			if (context == "bossBag" && arg == ItemID.WallOfFleshBossBag && Main.rand.NextBool(5))
			{
				player.QuickSpawnItem(mod.ItemType<EtherealVortex>());
			}

			if (context == "crate")
			{
				int chance = !Main.hardMode ? 25 : !NPC.downedMechBossAny ? 15 : 10;
				if (arg == ItemID.WoodenCrate)
				{
					chance += (chance / 3);
				}
				else if (arg == ItemID.GoldenCrate)
				{
					chance /= 2;
				}
				if (Main.rand.NextBool(chance))
				{
					player.QuickSpawnItem(mod.ItemType<LesserReforgingKit>(), Main.rand.NextBool() ? 2 : 1);
				}
				else if (Main.hardMode && Main.rand.NextBool((int)(chance * 1.5f)))
				{
					player.QuickSpawnItem(mod.ItemType<ReforgingKit>(), Main.rand.NextBool(4) ? 2 : 1);
				}
			}
		}

		public override void UpdateEquip(Item item, Player player)
		{
			if (item.prefix < PrefixID.Hard || item.prefix > PrefixID.Violent)
			{
				return;
			}
			GadgetPlayer modPlayer = player.Gadget();
			if (!modPlayer.shinyEquips)
			{
				return;
			}
			if (item.prefix <= PrefixID.Warding)
			{
				player.statLifeMax2 += (item.prefix - PrefixID.Hard + 1) * 2;
			}
			else if (item.prefix == PrefixID.Arcane)
			{
				player.manaCost -= 0.02f;
			}
			else if (item.prefix <= PrefixID.Lucky)
			{
				modPlayer.critShine += (byte)(item.prefix - PrefixID.Precise + 1);
			}
			else if (item.prefix <= PrefixID.Menacing)
			{
				player.armorPenetration += (item.prefix - PrefixID.Jagged + 2) / 2;
			}
			else if (item.prefix <= PrefixID.Quick2)
			{
				modPlayer.speedShine += (byte)(item.prefix - PrefixID.Brisk + 1);
				player.jumpSpeedBoost += (item.prefix - PrefixID.Brisk + 1) * 0.1f;
			}
			else
			{
				player.pickSpeed -= (item.prefix - PrefixID.Wild + 1) * 0.01f;
			}
		}

		public override bool ReforgePrice(Item item, ref int reforgePrice, ref bool canApplyDiscount)
		{
			if (tweak == TweakType.Malleable)
			{
				reforgePrice = (int)Math.Ceiling(reforgePrice * 0.75f);
			}
			else if (tweak == TweakType.Precious)
			{
				reforgePrice = (int)(reforgePrice * 1.5f);
			}
			return true;
		}

		public override void OnCraft(Item item, Recipe recipe)
		{
			int workshop = mod.TileType<LihzahrdWorkshopTile>();
			if (item.accessory && Main.LocalPlayer.adjTile[workshop] &&
				Array.Exists(recipe.requiredTile, x => x == TileID.TinkerersWorkbench || x == workshop))
			{
				tweak = Main.rand.NextBool(3) ? TweakType.Malleable : TweakType.Precious;
			}
		}

		public override bool NeedsSaving(Item item) => tweak != TweakType.None;
		public override TagCompound Save(Item item) => new TagCompound { ["Tweak"] = (byte)tweak };
		public override void Load(Item item, TagCompound tag) => tweak = (TweakType)tag.GetByte("Tweak");
		public override void NetSend(Item item, BinaryWriter writer) => writer.Write((byte)tweak);
		public override void NetReceive(Item item, BinaryReader reader) => tweak = (TweakType)reader.ReadByte();

		internal enum TweakType : byte
		{
			None,
			Malleable,
			Precious
		}

		public override bool Autoload(ref string name)
		{
			On.Terraria.Item.GetStoreValue += OnGetStoreValue;
			return base.Autoload(ref name);
		}

		private static int OnGetStoreValue(On.Terraria.Item.orig_GetStoreValue orig, Item self)
		{
			int value = orig(self);
			if (!self.buy && self.shopSpecialCurrency == -1)
			{
				TweakType tweakType = self.Gadget().tweak;
				if (tweakType == TweakType.Malleable)
				{
					value /= 2;
				}
				else if (tweakType == TweakType.Precious)
				{
					value *= 2;
				}
			}
			return value;
		}
	}
}