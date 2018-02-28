using System.Collections.Generic;
using GadgetBox.Items.Tools;
using Terraria;
using Terraria.ModLoader;

namespace GadgetBox.Prefixes
{
	public class ToolPrefix : ModPrefix
	{
		internal static List<byte> ToolPrefixes = new List<byte>();
		internal int critBonus = 0;
		internal float damageMult = 1f;
		internal float knockbackMult = 1f;
		internal int tileBoost = 0;
		internal float useTimeMult = 1f;
		
		public override PrefixCategory Category => PrefixCategory.AnyWeapon;

		public ToolPrefix() { }

		public ToolPrefix(float damageMult = 1f, float knockbackMult = 1f, float useTimeMult = 1f, int critBonus = 0, int tileBoost = 0)
		{
			this.damageMult = damageMult;
			this.knockbackMult = knockbackMult;
			this.useTimeMult = useTimeMult;
			this.critBonus = critBonus;
			this.tileBoost = tileBoost;
		}

		public override bool Autoload(ref string name)
		{
			if (base.Autoload(ref name))
			{
				AddToolPrefix(mod, ToolPrefixType.Trashed, 0.85f, 1, 1.15f, 0, -1);
				AddToolPrefix(mod, ToolPrefixType.Shortened, 0.9f, 1, 1, 0, -2);
				AddToolPrefix(mod, ToolPrefixType.Restless, 0.8f, 1, 0.85f, 2);
				AddToolPrefix(mod, ToolPrefixType.Accelerated, 1, 0.85f, 0.9f, 1);
				AddToolPrefix(mod, ToolPrefixType.Reaching, 1, 1, 1, 2, 1);
				AddToolPrefix(mod, ToolPrefixType.Enlarged, 1.07f, 1.1f, 1.05f, 0, 2);
				AddToolPrefix(mod, ToolPrefixType.Extended, 1, 1, 1, 0, 3);
				AddToolPrefix(mod, ToolPrefixType.Engineered, 1.07f, 1.05f, 0.82f, 4, 2);
			}
			return false;
		}

		public override void Apply(Item item) => item.tileBoost += item.tileBoost == 0 && tileBoost < 0 ? -1 : tileBoost;

		public override void ModifyValue(ref float valueMult) => valueMult *= 1 + tileBoost * 0.04f;

		public override bool CanRoll(Item item) => item.pick + item.axe + item.hammer + ((item.modItem as BaseShovel)?.shovel ?? 0) > 0 && item.tileBoost + tileBoost > -3 && (!item.noUseGraphic || useTimeMult == 1f);

		public override float RollChance(Item item) => item.noUseGraphic ? 1.15f : 3.3f; // 33.33% chance of getting a tool prefix, assuming all of them can be applied and no other modded prefixes were added. May check for other mods prefixes in the future.

		public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus)
		{
			damageMult = this.damageMult;
			knockbackMult = this.knockbackMult;
			useTimeMult = this.useTimeMult;
			critBonus = this.critBonus;
		}

		static void AddToolPrefix(Mod mod, ToolPrefixType prefixType, float damageMult = 1f, float knockbackMult = 1f, float useTimeMult = 1f, int critBonus = 0, int tileBoost = 0)
		{
			mod.AddPrefix(prefixType.ToString(), new ToolPrefix(damageMult, knockbackMult, useTimeMult, critBonus, tileBoost));
			ToolPrefixes.Add(mod.GetPrefix(prefixType.ToString()).Type);
		}
	}

	public enum ToolPrefixType : byte
	{
		None,
		Trashed,
		Shortened,
		Restless,
		Accelerated,
		Reaching,
		Enlarged,
		Extended,
		Engineered
	}
}