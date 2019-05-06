using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace GadgetBox.Items.Tools
{
	public class ActivationRod : ModItem
	{
		private delegate void HitWireDelegate(int x, int y);
		private static HitWireDelegate HitWireSingle;

		public override void SetStaticDefaults()
		{
			// This is required since Wiring.HitWireSingle is private.
			HitWireSingle = (HitWireDelegate)typeof(Wiring).GetMethod("HitWireSingle", BindingFlags.Static | BindingFlags.NonPublic).CreateDelegate(typeof(HitWireDelegate));
		}

		public override void SetDefaults()
		{
			item.useStyle = 1;
			item.useTurn = true;
			item.useAnimation = 30;
			item.useTime = 30;
			item.width = 34;
			item.height = 34;
			item.rare = 4;
			item.value = Item.sellPrice(gold: 1);
			item.tileBoost = 20;
			item.mech = true;
		}

		public override void HoldItem(Player player)
		{
			if (Main.myPlayer != player.whoAmI || player.position.X / 16 - Player.tileRangeX - item.tileBoost - player.blockRange > Player.tileTargetX ||
				(player.position.X + player.width) / 16 + Player.tileRangeX + item.tileBoost - 1 + player.blockRange < Player.tileTargetX ||
				player.position.Y / 16 - Player.tileRangeY - item.tileBoost - player.blockRange > Player.tileTargetY ||
				(player.position.Y + player.height) / 16 + Player.tileRangeY + item.tileBoost - 2 + player.blockRange < Player.tileTargetY)
			{
				return;
			}

			if (!Main.GamepadDisableCursorItemIcon)
			{
				player.showItemIcon = true;
				Main.ItemIconCacheUpdate(item.type);
			}

			if (player.itemAnimation == player.itemAnimationMax - 1 && player.itemTime == 0 && player.controlUseItem)
			{
				if (Main.netMode == NetmodeID.MultiplayerClient)
				{
					ModPacket packet = GadgetBox.Instance.GetPacket(MessageType.TripWire, 8);
					packet.Write(Player.tileTargetX);
					packet.Write(Player.tileTargetY);
					packet.Send();
				}
				else
				{
					TriggerMech(Player.tileTargetX, Player.tileTargetY);
				}
			}
		}

		internal static void TriggerMech(int x, int y)
		{
			HitWireSingle(x, y);
			Wiring.TripWire(x, y, 1, 1);
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Switch, 50);
			recipe.AddRecipeGroup(GadgetRecipes.AnyCobaltBar, 10);
			recipe.AddIngredient(ItemID.Wire, 100);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}
