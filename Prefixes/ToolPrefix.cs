﻿using Terraria;
using Terraria.ModLoader;
using System.Collections.Generic;

namespace GadgetBox.Prefixes
{
	public class ToolPrefix : ModPrefix
	{
		float damageMult = 1f;
		float knockbackMult = 1f;
		float useTimeMult = 1f;
		float scaleMult = 1f;
		int critBonus = 0;
		int tileBoost = 0;

		internal static List<byte> ToolPrefixes = new List<byte>();

		public ToolPrefix() { }

		public ToolPrefix(float damageMult = 1f, float knockbackMult = 1f, float useTimeMult = 1f, float scaleMult = 1f, int critBonus = 0, int tileBoost = 0)
		{
			this.damageMult = damageMult;
			this.knockbackMult = knockbackMult;
			this.useTimeMult = useTimeMult;
			this.scaleMult = scaleMult;
			this.critBonus = critBonus;
			this.tileBoost = tileBoost;
		}

		public override bool Autoload(ref string name)
		{
			if (base.Autoload(ref name))
			{
				mod.AddToolPrefix(ToolPrefixType.Trashed, 0.85f, 1, 1.15f, 0.9f, 0, -1);
				mod.AddToolPrefix(ToolPrefixType.Shortened, 0.9f, 1, 1, 0.87f, 0, -2);
				mod.AddToolPrefix(ToolPrefixType.Restless, 0.8f, 1, 0.85f, 1, 2);
				mod.AddToolPrefix(ToolPrefixType.Accelerated, 1, 0.85f, 0.9f, 1, 1);
				mod.AddToolPrefix(ToolPrefixType.Reaching, 1, 1, 1, 1.05f, 2, 1);
				mod.AddToolPrefix(ToolPrefixType.Enlarged, 1.07f, 1.1f, 1.05f, 1.1f, 0, 2);
				mod.AddToolPrefix(ToolPrefixType.Extended, 1, 1, 1, 1.15f, 0, 3);
				mod.AddToolPrefix(ToolPrefixType.Engineered, 1.07f, 1.05f, 0.82f, 1.1f, 4, 2);
			}
			return false;
		}

		public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus)
		{
			damageMult = this.damageMult;
			knockbackMult = this.knockbackMult;
			useTimeMult = this.useTimeMult;
			scaleMult = this.scaleMult;
			critBonus = this.critBonus;
		}

		public override void ValidateItem(Item item, ref bool invalid)
		{
			if (!invalid && (tileBoost < 0 && item.tileBoost < 0 || item.noUseGraphic && useTimeMult != 1))
				invalid = true;
		}

		public override void Apply(Item item) => item.tileBoost += item.tileBoost == 0 && tileBoost < 0 ? -1 : tileBoost;

		public override void ModifyValue(ref float valueMult) => valueMult *= 1 + tileBoost * 0.04f;
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