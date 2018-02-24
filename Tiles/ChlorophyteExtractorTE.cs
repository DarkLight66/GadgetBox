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
		public byte CurrentPlayer { get; internal set; }
		public bool CanTurnOn => Power > 0 && Mud > 0 && Chlorophyte < MaxResources;
		public bool IsWorking => IsON && CanTurnOn;
		bool OldIsWorking { get; set; }
		TEServerMsg ServerMsg { get; set; } = TEServerMsg.None;
		short DigDelay { get; set; }

		public ChlorophyteExtractorTE()
		{
			IsON = false;
			Power = 0;
			Mud = 0;
			Chlorophyte = 0;
			FrameYOffset = 0;
			CurrentPlayer = 255;
			OldIsWorking = IsWorking;
			DigDelay = 0;
		}

		public override void OnKill()
		{
			if (CurrentPlayer != byte.MaxValue)
			{
				if (Main.netMode == NetmodeID.Server)
				{
					ModPacket packet = GadgetBox.Instance.GetPacket(MessageType.SyncExtractorPlayer, 4);
					packet.Write(-1);
					packet.Send(CurrentPlayer);
				}
				else
					ChlorophyteExtractorUI.CloseUI(ChlorophyteExtractorUI.ExtractorTE);
			}
			Rectangle hitbox = new Rectangle((Position.X - 1) * 16, (Position.Y - 1) * 16, 54, 54);
			if (Chlorophyte > 0)
				Item.NewItem(hitbox, ItemID.ChlorophyteOre, Chlorophyte);
			if (Mud > 0)
				Item.NewItem(hitbox, ItemID.MudBlock, Mud);
		}

		public override void Update()
		{
			if (IsWorking != OldIsWorking)
			{
				FrameYOffset = (byte)(IsWorking ? Main.tileFrame[TileID.Extractinator] : 0);
				DigDelay = (short)(IsWorking ? Main.rand.Next(540, 1080) : 0);
				OldIsWorking = IsWorking;
			}
			if (DigDelay > 0)
				DigDelay--;
			if (DigDelay > 0 || !IsWorking)
				return;
			DigDelay = (short)Main.rand.Next(360, 720);
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
			Power--;
			Mud--;
			Chlorophyte++;
			if (!CanTurnOn)
				IsON = false;
			SendServer();
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

		private void SendServer(TEServerMsg serverMsg = TEServerMsg.SyncAll)
		{
			if (Main.netMode != NetmodeID.Server)
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

		internal static void SyncExtractorPlayerIndex(BinaryReader reader, int whoAmI)
		{
			if (Main.netMode != NetmodeID.MultiplayerClient)
				return;
			ChlorophyteExtractorTE extractorTE = ExtractorByID(reader.ReadInt32());
			if (extractorTE == null)
				return;
			extractorTE.CurrentPlayer = reader.ReadByte();
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
			if (!OldIsWorking && IsWorking)
				FrameYOffset = (byte)Main.tileFrame[TileID.Extractinator];
			OldIsWorking = IsWorking;
		}

		internal void ProvidePower()
		{
			if (Main.mouseItem == null || Main.mouseItem.stack <= 0 || Main.mouseItem.type != ItemID.LihzahrdPowerCell)
				return;
			if (--Main.mouseItem.stack <= 0)
				Main.mouseItem.TurnToAir();
			Power = MaxResources;
			if (Main.netMode == NetmodeID.MultiplayerClient)
				SendClient(TEClientMsg.FillPower);
			return;
		}

		internal void ProvideMud(short addAmount = 0)
		{
			if (Main.mouseItem == null || Main.mouseItem.stack <= 0 || Main.mouseItem.type != ItemID.MudBlock)
				return;
			addAmount = (short)Math.Min(Main.mouseItem.stack, MaxResources - Mud);
			if ((Main.mouseItem.stack -= addAmount) <= 0)
				Main.mouseItem.TurnToAir();
			Mud += addAmount;
			if (Main.netMode == NetmodeID.MultiplayerClient)
				SendClient(TEClientMsg.AddMud, addAmount);
			return;
		}

		internal void ExtractChloro()
		{
			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				SendClient(TEClientMsg.ExtractChlorophyte);
				return;
			}
			Item.NewItem(new Rectangle((Position.X - 1) * 16, (Position.Y - 1) * 16, 54, 54), ItemID.ChlorophyteOre, Chlorophyte);
			Chlorophyte = 0;
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

		internal static void ReceiveRequestOpen(BinaryReader reader, int whoAmI)
		{
			if (Main.netMode != NetmodeID.Server)
				return;
			int curID = reader.ReadInt32();
			ChlorophyteExtractorTE extractorTE = ExtractorByID(curID);
			if (extractorTE == null || extractorTE.CurrentPlayer != byte.MaxValue)
				return;

			extractorTE.CurrentPlayer = (byte)whoAmI;
			ModPacket packet = GadgetBox.Instance.GetPacket(MessageType.SyncExtractorPlayer, 8);
			packet.Write(curID);
			packet.Write(extractorTE.Position.X);
			packet.Write(extractorTE.Position.Y);
			packet.Send(whoAmI);
			packet = GadgetBox.Instance.GetPacket(MessageType.SyncExtractorPlayerIndex, 5);
			packet.Write(curID);
			packet.Write((byte)whoAmI);
			packet.Send(-1, whoAmI);
		}

		internal static void SyncExtractorPlayer(BinaryReader reader, int whoAmI)
		{
			int curID = reader.ReadInt32();
			if (curID == -1 && Main.netMode == NetmodeID.MultiplayerClient && ChlorophyteExtractorUI.ExtractorTE.CurrentPlayer == Main.myPlayer)
			{
				ChlorophyteExtractorUI.CloseUI(ChlorophyteExtractorUI.ExtractorTE, false, true);
				return;
			}
			ChlorophyteExtractorTE extractorTE = ExtractorByID(curID);
			if (extractorTE == null)
				return;
			if (Main.netMode == NetmodeID.Server)
			{
				extractorTE.CurrentPlayer = (byte)whoAmI;
				ModPacket packet = GadgetBox.Instance.GetPacket(MessageType.SyncExtractorPlayerIndex, 5);
				packet.Write(curID);
				packet.Write((byte)whoAmI);
				packet.Send(-1, whoAmI);
				return;
			}
			Main.LocalPlayer.GetModPlayer<GadgetPlayer>().extractorPos = new Point16(reader.ReadInt16(), reader.ReadInt16());
			extractorTE.CurrentPlayer = (byte)Main.myPlayer;
			ChlorophyteExtractorUI.OpenUI(extractorTE, true);
			return;
		}

		void SendClient(TEClientMsg msgType, short extraData = 0)
		{
			bool AddMud = msgType == TEClientMsg.AddMud;
			ModPacket packet = GadgetBox.Instance.GetPacket(MessageType.ExtractorMessage, AddMud ? 7 : 5);
			packet.Write(ID);
			packet.Write((byte)msgType);
			if (AddMud)
				packet.Write(extraData);
			packet.Send();
		}

		public static ChlorophyteExtractorTE ExtractorByID(int ID)
		{
			TileEntity tempTE;
			if (!ByID.TryGetValue(ID, out tempTE))
				return null;
			return tempTE as ChlorophyteExtractorTE;
		}

		public static ChlorophyteExtractorTE ExtractorByPosition(Point16 position)
		{
			TileEntity tempTE;
			if (!ByPosition.TryGetValue(position, out tempTE))
				return null;
			return tempTE as ChlorophyteExtractorTE;
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
				NetMessage.SendTileSquare(Main.myPlayer, i, j - 1, 3);
				NetMessage.SendData(MessageID.TileEntityPlacement, -1, -1, null, i, j - 1, Type);
				return -1;
			}
			return Place(i, j - 1);
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
