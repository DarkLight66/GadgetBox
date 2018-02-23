using System;
using System.IO;
using GadgetBox.GadgetUI;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace GadgetBox.Tiles
{
	public class ChlorophyteExtractorTE : ModTileEntity
	{
		public const int MaxResources = 999;

		public bool IsON { get; private set; }
		public short Power { get; private set; }
		public short Mud { get; private set; }
		public short Chlorophyte { get; private set; }
		public byte FrameYOffset { get; private set; }
		public bool IsWorking => IsON && Power > 0 && Mud > 0 && Chlorophyte < MaxResources;

		TEServerMsg ServerMsg { get; set; } = TEServerMsg.None;

		public ChlorophyteExtractorTE()
		{
			IsON = false;
			Power = 0;
			Mud = 0;
			Chlorophyte = 0;
			FrameYOffset = 0;
			ID = -1;
		}

		public override void OnKill()
		{
			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				if (ChlorophyteExtractorUI.visible)
					ChlorophyteExtractorUI.CloseUI(Main.LocalPlayer.GetModPlayer<GadgetPlayer>(), false);
				return;
			}
			Rectangle hitbox = new Rectangle((Position.X - 1) * 16, (Position.Y - 1) * 16, 54, 54);
			if (Chlorophyte > 0)
				Item.NewItem(hitbox, ItemID.ChlorophyteOre, Chlorophyte);
			if (Mud > 0)
				Item.NewItem(hitbox, ItemID.MudBlock, Mud);
		}

		public override void Update()
		{
			if (!IsWorking)
				return;
			int x = Utils.Clamp(Main.rand.Next(-3, 4) + Position.X, 10, Main.maxTilesX - 10);
			int y = Utils.Clamp(Main.rand.Next(2, 9) + Position.Y, 10, Main.maxTilesY - 10);
			Tile tile = Framing.GetTileSafely(x, y);
			if (!tile.active() || tile.type != TileID.Chlorophyte)
				return;
			int nextChloro = 0;
			for (int i = x - 1; i < x + 2; i++)
			{
				for (int j = y - 1; j < y + 2; j++)
				{
					tile = Framing.GetTileSafely(i, j);
					if (i == x && j == y || !tile.active() || tile.type != TileID.Chlorophyte)
						continue;
					if (++nextChloro > 2)
						break;
				}
			}
			if (nextChloro < 3)
				return;
			Main.tile[x, y].type = TileID.Mud;
			WorldGen.SquareTileFrame(x, y, true);
			if (Main.netMode == NetmodeID.Server)
				NetMessage.SendTileSquare(-1, x, y, 1, TileChangeType.None);
			if (--Power <= 0)
				IsON = false;
			Mud--;
			Chlorophyte++;
			SendServer(TEServerMsg.SyncAll);
		}

		internal void TogglePower()
		{
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				IsON = !IsON;
				return;
			}
			SendClient(TEClientMsg.ToggleOn);
		}

		private void SendServer(TEServerMsg serverMsg)
		{
			if (Main.netMode != NetmodeID.Server || serverMsg == TEServerMsg.None)
				return;
			ServerMsg = serverMsg;
			NetMessage.SendData(MessageID.TileEntitySharing, -1, -1, null, ID);
		}

		public override void NetSend(BinaryWriter writer, bool lightSend)
		{
			if (lightSend)
				writer.Write((byte)ServerMsg);
			bool sendAll = ServerMsg == TEServerMsg.SyncAll || !lightSend;
			if (sendAll || ServerMsg == TEServerMsg.SyncOn)
				writer.Write(IsON);
			if (sendAll || ServerMsg == TEServerMsg.SyncPower)
				writer.Write(Power);
			if (sendAll || ServerMsg == TEServerMsg.SyncMud)
				writer.Write(Mud);
			if (sendAll || ServerMsg == TEServerMsg.SyncChlorophyte)
				writer.Write(Chlorophyte);
			ServerMsg = TEServerMsg.None;
		}

		internal static void SyncPlayerExtractorIndex(BinaryReader reader, int whoAmI)
		{
			if (Main.netMode != NetmodeID.MultiplayerClient)
				return;
			Player player = Main.player[reader.ReadByte()];
			player.GetModPlayer<GadgetPlayer>().extractor = reader.ReadInt16();
		}

		public override void NetReceive(BinaryReader reader, bool lightReceive)
		{
			TEServerMsg serverMsg = lightReceive ? (TEServerMsg)reader.ReadByte() : TEServerMsg.SyncAll;
			bool receiveAll = serverMsg == TEServerMsg.SyncAll;
			if (receiveAll || serverMsg == TEServerMsg.SyncOn)
				IsON = reader.ReadBoolean();
			if (receiveAll || serverMsg == TEServerMsg.SyncPower)
				Power = reader.ReadInt16();
			if (receiveAll || serverMsg == TEServerMsg.SyncMud)
				Mud = reader.ReadInt16();
			if (receiveAll || serverMsg == TEServerMsg.SyncChlorophyte)
				Chlorophyte = reader.ReadInt16();
		}

		internal void ProvidePower()
		{
			if (Main.netMode != NetmodeID.Server && Main.mouseItem != null && Main.mouseItem.stack > 0 && Main.mouseItem.type == ItemID.LihzahrdPowerCell)
			{
				if (--Main.mouseItem.stack <= 0)
					Main.mouseItem.TurnToAir();
				Power = MaxResources;
				if (Main.netMode == NetmodeID.MultiplayerClient)
					SendClient(TEClientMsg.FillPower);
				return;
			}
			Power = MaxResources;
		}

		internal void ProvideMud(short addAmount = 0)
		{
			if (Main.netMode != NetmodeID.Server && Main.mouseItem != null && Main.mouseItem.stack > 0 && Main.mouseItem.type == ItemID.MudBlock)
			{
				addAmount = (short)Math.Min(Main.mouseItem.stack, MaxResources - Mud);
				if ((Main.mouseItem.stack -= addAmount) <= 0)
					Main.mouseItem.TurnToAir();
				Mud += addAmount;
				if (Main.netMode == NetmodeID.MultiplayerClient)
					SendClient(TEClientMsg.AddMud, addAmount);
				return;
			}
			Mud += addAmount;
		}

		internal void ExtractChloro()
		{
			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				SendClient(TEClientMsg.ExtractChlorophyte);
				return;
			}
			Item.NewItem(new Rectangle((Position.X - 1) * 16, (Position.Y - 1) * 16, 54, 54), ItemID.ChlorophyteOre, Chlorophyte);
		}

		internal void RecieveMessage(BinaryReader reader, int whoAmI)
		{
			TEClientMsg clientMsg = (TEClientMsg)reader.ReadByte();
			if (clientMsg == TEClientMsg.ToggleOn)
				IsON = !IsON;
			else if (clientMsg == TEClientMsg.FillPower)
				Power = MaxResources;
			else if (clientMsg == TEClientMsg.AddMud)
				Mud += reader.ReadInt16();
			else if (clientMsg == TEClientMsg.ExtractChlorophyte)
				ExtractChloro();
			SendServer((TEServerMsg)clientMsg);
		}

		internal static void RequestOpen(Point16 extractorPos)
		{
			ModPacket packet = GadgetBox.Instance.GetPacket(MessageType.RequestExtractorOpen, 8);
			packet.Write(extractorPos.X);
			packet.Write(extractorPos.Y);
			packet.Send();
		}

		internal static void ReceiveRequestOpen(BinaryReader reader, int whoAmI)
		{
			if (Main.netMode != NetmodeID.Server)
				return;
			Point16 position = new Point16(reader.ReadInt16(), reader.ReadInt16());
			TileEntity extractorTE;
			if (!ByPosition.TryGetValue(position, out extractorTE))
				return;

			short curID = (short)extractorTE.ID;
			if (UsingExtractor(curID) == -1)
			{
				ModPacket packet = GadgetBox.Instance.GetPacket(MessageType.SyncPlayerExtractor, 12);
				packet.Write(curID);
				packet.Write(position.X);
				packet.Write(position.Y);
				packet.Send(whoAmI);
				Main.player[whoAmI].GetModPlayer<GadgetPlayer>().extractor = curID;
				packet = GadgetBox.Instance.GetPacket(MessageType.SyncPlayerExtractorIndex, 5);
				packet.Write((byte)whoAmI);
				packet.Write(curID);
				packet.Send(-1, whoAmI);
			}
		}

		internal static void SyncPlayerExtractor(BinaryReader reader, int whoAmI)
		{
			short curID = reader.ReadInt16();
			if (Main.netMode == NetmodeID.Server)
			{
				Main.player[whoAmI].GetModPlayer<GadgetPlayer>().extractor = curID;
				ModPacket packet = GadgetBox.Instance.GetPacket(MessageType.SyncPlayerExtractorIndex, 5);
				packet.Write((byte)whoAmI);
				packet.Write(curID);
				packet.Send(-1, whoAmI);
				return;
			}
			Point16 position = new Point16(reader.ReadInt16(), reader.ReadInt16());
			GadgetPlayer gadgetPlayer = Main.LocalPlayer.GetModPlayer<GadgetPlayer>();
			if (gadgetPlayer.extractor == -1)
			{
				Main.playerInventory = true;
				Main.PlaySound(SoundID.MenuOpen);
			}
			else if (gadgetPlayer.extractor != curID && curID != -1)
			{
				Main.playerInventory = true;
				Main.PlaySound(SoundID.MenuTick);
				Main.recBigList = false;
			}
			else if (gadgetPlayer.extractor != -1 && curID == -1)
			{
				Main.PlaySound(SoundID.MenuClose);
				Main.recBigList = false;
			}
			gadgetPlayer.extractor = curID;
			gadgetPlayer.extractorPos = position;
			return;
		}

		private static int UsingExtractor(int ID)
		{
			for (int i = 0; i < 255; i++)
			{
				if (!Main.player[i].active || Main.player[i].dead)
					continue;
				if (Main.player[i].GetModPlayer<GadgetPlayer>().extractor != ID)
					continue;
				return i;
			}
			return -1;
		}

		void SendClient(TEClientMsg msgType, short extraData = 0)
		{
			bool AddMud = msgType == TEClientMsg.AddMud;
			ModPacket packet = GadgetBox.Instance.GetPacket(MessageType.ExtractorMessage, AddMud ? 5 : 1);
			packet.Write((short)ID);
			packet.Write((byte)msgType);
			if (AddMud)
				packet.Write(extraData);
			packet.Send();
		}

		public override bool ValidTile(int i, int j)
		{
			var tile = Main.tile[i, j];
			return tile.active() && tile.type == mod.TileType<ChlorophyteExtractorTile>() && (tile.frameX / 18 == 1) && (tile.frameY % 54 / 18 == 1);
		}

		public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction)
		{
			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				NetMessage.SendTileSquare(Main.myPlayer, i, j, 3);
				NetMessage.SendData(MessageID.TileEntityPlacement, -1, -1, null, i, j, Type);
				return -1;
			}
			return Place(i, j);
		}

		public override TagCompound Save()
		{
			return new TagCompound
			{
				["IsON"] = IsON,
				["Power"] = Power,
				["Mud"] = Mud,
				["Chlorophyte"] = Chlorophyte
			};
		}

		public override void Load(TagCompound tag)
		{
			IsON = tag.GetBool("IsON");
			Power = tag.GetShort("Power");
			Mud = tag.GetShort("Mud");
			Chlorophyte = tag.GetShort("Chlorophyte");
		}

		enum TEClientMsg : byte
		{
			None,
			ToggleOn,
			FillPower,
			AddMud,
			ExtractChlorophyte
		}

		enum TEServerMsg : byte
		{
			None,
			SyncOn,
			SyncPower,
			SyncMud,
			SyncChlorophyte,
			SyncAll
		}
	}
}
