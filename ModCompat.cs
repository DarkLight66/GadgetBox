using Terraria;
using Terraria.ModLoader;

namespace GadgetBox
{
	internal static class ModCompat
	{
		internal static Mod RAmod;

		internal static void Load()
		{
			RAmod = ModLoader.GetMod("ReforgeArmor");
		}

		internal static void Unload()
		{
			RAmod = null;
		}

		internal static bool ArmorPrefix(Item item) => RAmod != null && !item.vanity && (item.headSlot != -1 || item.bodySlot != -1 || item.legSlot != -1);

		internal static void ApplyArmorPrefix(Item item, byte prefix)
		{
			if (item.value <= 1 && item.rare > 0)
			{
				item.value = (item.rare * item.defense * 2500);
				item.Prefix(item.prefix);
			}
		}
	}
}