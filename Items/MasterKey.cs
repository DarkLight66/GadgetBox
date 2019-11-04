using MonoMod.Cil;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using static Mono.Cecil.Cil.OpCodes;
using static Terraria.ModLoader.ModContent;

namespace GadgetBox.Items
{
	public class MasterKey : ModItem
	{
		public override void SetDefaults()
		{
			item.width = 16;
			item.height = 34;
			item.value = Item.sellPrice(gold: 10);
			item.rare = 6;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.GoldenKey, 10);
			recipe.AddIngredient(ItemID.ShadowKey);
			recipe.AddIngredient(ItemID.HallowedKey);
			recipe.AddRecipeGroup(GadgetRecipes.AnyCorruptionKey);
			recipe.AddIngredient(ItemID.FrozenKey);
			recipe.AddIngredient(ItemID.JungleKey);
			recipe.AddIngredient(ItemID.TempleKey);
			recipe.AddTile(TileID.TinkerersWorkbench);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}

		public override bool Autoload(ref string name)
		{
			IL.Terraria.UI.ItemSlot.RightClick_ItemArray_int_int += ILGoldenLockBox;
			return base.Autoload(ref name);
		}

		private void ILGoldenLockBox(ILContext il)
		{
			ILCursor cursor = new ILCursor(il);

			if (!cursor.TryGotoNext(MoveType.After, i => i.MatchLdcI4(ItemID.GoldenKey), i => i.MatchLdcI4(0),
				i => i.MatchCallvirt(typeof(Player).GetMethod(nameof(Player.ConsumeItem)))))
			{
				return;
			}

			ILLabel label = il.DefineLabel();

			cursor.Emit(Brtrue_S, label);
			cursor.Emit(Ldloc_0);
			cursor.EmitDelegate<Func<Player, bool>>(player => player.HasItem(ItemType<MasterKey>()));
			cursor.Index++;
			cursor.MarkLabel(label);
		}
	}
}