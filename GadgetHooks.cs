using System;
using GadgetBox.Items;
using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;
using static Mono.Cecil.Cil.OpCodes;

namespace GadgetBox
{
	internal static class GadgetHooks
	{
		internal static void Initialize()
		{
			IL.Terraria.UI.ItemSlot.RightClick_ItemArray_int_int += ILItemOnRightClick;
		}

		private static void ILItemOnRightClick(ILContext il)
		{
			ILCursor cursor = new ILCursor(il);
			ILLabel label = il.DefineLabel();

			if (!cursor.TryGotoNext(i => i.MatchBr(out label), i => i.MatchLdarg(0), i => i.MatchLdarg(2),
				i => i.MatchLdelemRef(), i => i.MatchCall(out _)))
			{
				return;
			}

			cursor.Index++;
			cursor.MoveAfterLabels();
			cursor.Emit(Ldarg_0);
			cursor.Emit(Ldarg_2);
			cursor.Emit(Ldloc_0);
			cursor.EmitDelegate<Func<Item[], int, Player, bool>>((item, slot, player) =>
			{
				if (Main.mouseRight && Main.mouseRightRelease && Main.mouseItem.modItem is ItemOnRightClick modItem)
				{
					if (modItem.CanRightClick(item[slot], false))
					{
						Main.mouseItem.Consume();
						modItem.RightClick(ref item[slot], player, false);
						Main.mouseRightRelease = false;
						Recipe.FindRecipes();
					}
					return true;
				}
				return false;
			});
			cursor.Emit(Brtrue, label);
		}
	}
}