using GadgetBox.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace GadgetBox
{
	public class GadgetTile : GlobalTile
	{
		public override void MouseOver(int i, int j, int type)
		{
			Tile tile = Main.tile[i, j];
			if (!TileID.Sets.BasicChest[tile.type] && tile.type != TileID.ClosedDoor)
			{
				return;
			}

			Player player = Main.LocalPlayer;
			int masterKey = ItemType<MasterKey>();
			bool mayUnlock = false;
			if (TileID.Sets.BasicChest[tile.type])
			{
				int left = i, top = j;
				if (tile.frameX % 36 != 0)
				{
					left--;
				}
				if (tile.frameY != 0)
				{
					top--;
				}
				if (Chest.isLocked(left, top))
				{
					mayUnlock = true;
				}
			}
			else if (WorldGen.IsLockedDoor(i, j))
			{
				mayUnlock = true;
			}
			if (mayUnlock && (player.showItemIcon2 < 0 || !player.HasItem(player.showItemIcon2)) && player.HasItem(masterKey))
			{
				player.showItemIcon2 = masterKey;
			}
		}

		public override void RightClick(int i, int j, int type)
		{
			Player player = Main.LocalPlayer;
			Tile tile = Main.tile[i, j];
			int masterKey = ItemType<MasterKey>();
			if (TileID.Sets.BasicChest[tile.type] && player.talkNPC == -1)
			{
				int left = i, top = j;
				if (tile.frameX % 36 != 0)
				{
					left--;
				}
				if (tile.frameY != 0)
				{
					top--;
				}
				if (Chest.isLocked(left, top) && player.HasItem(masterKey) && Chest.Unlock(left, top) && Main.netMode == NetmodeID.MultiplayerClient)
				{
					player.tileInteractionHappened = true;
					NetMessage.SendData(MessageID.Unlock, -1, -1, null, player.whoAmI, 1f, left, top);
				}
			}
			else if (tile.type == TileID.ClosedDoor && WorldGen.IsLockedDoor(i, j) && player.HasItem(masterKey))
			{
				WorldGen.UnlockDoor(i, j);
				if (Main.netMode == NetmodeID.MultiplayerClient)
				{
					player.tileInteractionHappened = true;
					NetMessage.SendData(MessageID.Unlock, -1, -1, null, player.whoAmI, 2f, i, j);
				}
			}
		}
	}
}