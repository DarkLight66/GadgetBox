﻿using System;
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
		public byte CurrentPlayer { get; internal set; }
		public short Power { get; private set; }
		public short Mud { get; private set; }
		public short Chlorophyte { get; private set; }

		public byte FrameYOffset { get; private set; }
		public bool CanTurnOn => Power > 0 && Mud > 0 && Chlorophyte < MaxResources;
		public bool IsWorking => IsON && CanTurnOn;

		bool OldIsWorking { get; set; }
		short DigDelay { get; set; }
		TEMessage ServerMsg { get; set; } = TEMessage.None;

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
			int y = Utils.Clamp(Main.rand.Next(3, 10) + Position.Y, 10, Main.maxTilesY - 10);
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
				NetMessage.SendTileSquare(-1, x, y, 1);
			Power--;
			Mud--;
			Chlorophyte++;
			if (!CanTurnOn)
				IsON = false;
			SendServer();
		}

		public override void NetSend(BinaryWriter writer, bool lightSend)
		{
			writer.Write(lightSend ? (byte)ServerMsg : CurrentPlayer);
			bool sendAll = ServerMsg == TEMessage.SyncAll || !lightSend;
			if (sendAll || ServerMsg == TEMessage.SyncOn)
				writer.Write(IsON);
			if (sendAll || ServerMsg == TEMessage.SyncPower)
				writer.Write(Power);
			if (sendAll || ServerMsg == TEMessage.SyncMud)
				writer.Write(Mud);
			if (sendAll || ServerMsg == TEMessage.SyncChlorophyte)
				writer.Write(Chlorophyte);
			ServerMsg = TEMessage.None;
		}

		public override void NetReceive(BinaryReader reader, bool lightReceive)
		{
			TEMessage serverMsg = TEMessage.None;
			if (lightReceive)
				serverMsg = (TEMessage)reader.ReadByte();
			else
				CurrentPlayer = reader.ReadByte();
			bool receiveAll = serverMsg == TEMessage.SyncAll || !lightReceive;
			if (receiveAll || serverMsg == TEMessage.SyncOn)
				IsON = reader.ReadBoolean();
			if (receiveAll || serverMsg == TEMessage.SyncPower)
				Power = reader.ReadInt16();
			if (receiveAll || serverMsg == TEMessage.SyncMud)
				Mud = reader.ReadInt16();
			if (receiveAll || serverMsg == TEMessage.SyncChlorophyte)
				Chlorophyte = reader.ReadInt16();
			if (IsWorking != OldIsWorking)
			{
				FrameYOffset = (byte)(IsWorking ? Main.tileFrame[TileID.Extractinator] : 0);
				OldIsWorking = IsWorking;
			}
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

		internal static void SyncExtractorPlayerIndex(BinaryReader reader, int whoAmI)
		{
			if (Main.netMode != NetmodeID.MultiplayerClient)
				return;
			ChlorophyteExtractorTE extractorTE = ExtractorByID(reader.ReadInt32());
			if (extractorTE == null)
				return;
			extractorTE.CurrentPlayer = reader.ReadByte();
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

		internal void TogglePower()
		{
			Main.PlaySound(SoundID.Mech, Style: 0);
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				IsON = !IsON;
				return;
			}
			SendClient(TEMessage.SyncOn);
		}

		internal void ProvidePower()
		{
			if (Main.mouseItem == null || Main.mouseItem.stack <= 0 || Main.mouseItem.type != ItemID.LihzahrdPowerCell)
				return;
			if (--Main.mouseItem.stack <= 0)
				Main.mouseItem.TurnToAir();
			Power = MaxResources;
			Main.PlaySound(SoundID.Grab);
			if (Main.netMode == NetmodeID.MultiplayerClient)
				SendClient(TEMessage.SyncPower);
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
			Main.PlaySound(SoundID.Grab);
			if (Main.netMode == NetmodeID.MultiplayerClient)
				SendClient(TEMessage.SyncMud, addAmount);
			return;
		}

		internal void ExtractChloro()
		{
			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				SendClient(TEMessage.SyncChlorophyte);
				return;
			}
			Item.NewItem(new Rectangle((Position.X - 1) * 16, (Position.Y - 1) * 16, 54, 54), ItemID.ChlorophyteOre, Chlorophyte);
			Chlorophyte = 0;
		}

		internal void RecieveMessage(BinaryReader reader, int whoAmI)
		{
			TEMessage clientMsg = (TEMessage)reader.ReadByte();
			if (clientMsg == TEMessage.SyncOn)
				IsON = !IsON;
			else if (clientMsg == TEMessage.SyncPower)
				Power = MaxResources;
			else if (clientMsg == TEMessage.SyncMud)
				Mud += reader.ReadInt16();
			else if (clientMsg == TEMessage.SyncChlorophyte)
				ExtractChloro();
			SendServer(clientMsg);
		}

		void SendServer(TEMessage serverMsg = TEMessage.SyncAll)
		{
			if (Main.netMode != NetmodeID.Server)
				return;
			ServerMsg = serverMsg;
			NetMessage.SendData(MessageID.TileEntitySharing, -1, -1, null, ID);
		}

		void SendClient(TEMessage msgType, short extraData = 0)
		{
			bool AddMud = msgType == TEMessage.SyncMud;
			ModPacket packet = GadgetBox.Instance.GetPacket(MessageType.SendExtractorMessage, AddMud ? 7 : 5);
			packet.Write(ID);
			packet.Write((byte)msgType);
			if (AddMud)
				packet.Write(extraData);
			packet.Send();
		}

		enum TEMessage : byte
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