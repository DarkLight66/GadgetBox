using System;
using System.IO;
using GadgetBox.GadgetUI;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace GadgetBox.Tiles
{
	public class ChlorophyteExtractorTE : ModTileEntity
	{
		public const short MaxResources = 999;
		private bool _isON = false;

		public bool IsON
		{
			get
			{
				return _isON;
			}
			private set
			{
				if (Main.netMode != NetmodeID.MultiplayerClient && value != _isON)
					DigDelay = (short)(value ? Main.rand.Next(540, 1080) : 0);
				_isON = value;
			}
		}

		public bool Animating { get; internal set; }
		public byte CurrentPlayer { get; internal set; }
		public short Power { get; private set; }
		public short Mud { get; private set; }
		public short Chlorophyte { get; private set; }

		public byte FrameYOffset { get; internal set; }
		public bool CanTurnOn => Power > 0 && Mud > 0 && Chlorophyte < MaxResources;
		public bool IsWorking => IsON && CanTurnOn;

		short DigDelay { get; set; }

		public ChlorophyteExtractorTE()
		{
			IsON = false;
			Power = 0;
			Mud = 0;
			Chlorophyte = 0;
			FrameYOffset = 0;
			CurrentPlayer = 255;
			Animating = false;
		}

		public override void OnKill()
		{
			if (CurrentPlayer != byte.MaxValue)
			{
				if (Main.netMode == NetmodeID.Server)
				{
					ModPacket packet = GadgetBox.Instance.GetPacket(MessageType.ExtractorMessage, 6);
					packet.Write(ID);
					packet.Write((byte)ExtractorMessage.SyncPlayer);
					packet.Write(byte.MaxValue);
					packet.Send(CurrentPlayer);
				}
				else
					ChlorophyteExtractorUI.ExtractorTE.CloseUI();
			}
			Rectangle hitbox = new Rectangle((Position.X - 1) * 16, (Position.Y - 1) * 16, 54, 54);
			if (Chlorophyte > 0)
				Item.NewItem(hitbox, ItemID.ChlorophyteOre, Chlorophyte);
			if (Mud > 0)
				Item.NewItem(hitbox, ItemID.MudBlock, Mud);
		}

		public override void Update()
		{
			DigDelay--;
			if (DigDelay > 0 || !IsWorking)
				return;
			DigDelay = (short)Main.rand.Next(360, 720);
			short x = (short)Utils.Clamp(Main.rand.Next(-3, 4) + Position.X, 10, Main.maxTilesX - 10);
			short y = (short)Utils.Clamp(Main.rand.Next(3, 10) + Position.Y, 10, Main.maxTilesY - 10);
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
			ExtractorEffect(x, y);
			if (Main.netMode == NetmodeID.Server)
				NetMessage.SendTileSquare(-1, x, y, 1);
			Power--;
			Mud--;
			Chlorophyte++;
			if (!CanTurnOn)
				IsON = false;
			SendServerMessage();
		}

		public override void NetSend(BinaryWriter writer, bool lightSend)
		{
			writer.Write(IsON);
			writer.Write(CurrentPlayer);
			writer.Write(Power);
			writer.Write(Mud);
			writer.Write(Chlorophyte);
		}

		public override void NetReceive(BinaryReader reader, bool lightReceive)
		{
			IsON = reader.ReadBoolean();
			CurrentPlayer = reader.ReadByte();
			Power = reader.ReadInt16();
			Mud = reader.ReadInt16();
			Chlorophyte = reader.ReadInt16();
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

		internal void ReceiveExtractorMessage(BinaryReader reader, int whoAmI)
		{
			ExtractorMessage message = (ExtractorMessage)reader.ReadByte();
			ModPacket packet;
			switch (message)
			{
				case ExtractorMessage.SyncPlayer:
					byte owner = reader.ReadByte();
					if (Main.netMode == NetmodeID.MultiplayerClient)
					{
						if (ID == ChlorophyteExtractorUI.ExtractorTE.ID && owner != Main.myPlayer)
						{
							CloseUI(false, true);
							if (owner == byte.MaxValue)
								break;
						}
						else if (ID != ChlorophyteExtractorUI.ExtractorTE.ID && owner == Main.myPlayer)
						{
							Main.LocalPlayer.GetModPlayer<GadgetPlayer>().machinePos = Position;
							OpenUI(true);
						}
						CurrentPlayer = owner;
						break;
					}
					if (owner != byte.MaxValue && owner != whoAmI)
						break;
					CurrentPlayer = owner;
					packet = GadgetBox.Instance.GetPacket(MessageType.ExtractorMessage, 6);
					packet.Write(ID);
					packet.Write((byte)ExtractorMessage.SyncPlayer);
					packet.Write(owner);
					packet.Send(-1, whoAmI);
					break;

				case ExtractorMessage.RequestExtractorOpen:
					if (Main.netMode != NetmodeID.Server || CurrentPlayer != byte.MaxValue)
						break;
					CurrentPlayer = (byte)whoAmI;
					packet = GadgetBox.Instance.GetPacket(MessageType.ExtractorMessage, 6);
					packet.Write(ID);
					packet.Write((byte)ExtractorMessage.SyncPlayer);
					packet.Write((byte)whoAmI);
					packet.Send();
					break;

				case ExtractorMessage.SyncOn:
				case ExtractorMessage.SyncPower:
				case ExtractorMessage.SyncMud:
				case ExtractorMessage.SyncChlorophyte:
				case ExtractorMessage.SyncAll:
					bool syncAll = message == ExtractorMessage.SyncAll;
					bool isServer = Main.netMode == NetmodeID.Server;

					if (message == ExtractorMessage.SyncOn || syncAll)
						IsON = isServer ? !IsON : reader.ReadBoolean();
					if (message == ExtractorMessage.SyncPower || syncAll)
						Power = isServer ? MaxResources : reader.ReadInt16();
					if (message == ExtractorMessage.SyncMud || syncAll)
						Mud = (short)(reader.ReadInt16() + (isServer ? Mud : 0));
					if (message == ExtractorMessage.SyncChlorophyte || syncAll)
					{
						if (isServer)
							ExtractChloro();
						else
							Chlorophyte = reader.ReadInt16();
					}
					SendServerMessage(message);
					break;

				case ExtractorMessage.ExtractorEffect:
					ExtractorEffect(reader.ReadInt16(), reader.ReadInt16());
					break;
			}
		}

		internal void TogglePower()
		{
			Main.PlaySound(SoundID.Mech, Style: 0);
			IsON = !IsON;
			if (Main.netMode == NetmodeID.MultiplayerClient)
				SendClientMessage(ExtractorMessage.SyncOn);
		}

		internal void ProvidePower()
		{
			if (Main.mouseItem == null || Main.mouseItem.stack <= 0 || Main.mouseItem.type != ItemID.LihzahrdPowerCell)
				return;
			Main.mouseItem.Consume();
			Power = MaxResources;
			Main.PlaySound(SoundID.Grab);
			Recipe.FindRecipes();
			SendClientMessage(ExtractorMessage.SyncPower);
			return;
		}

		internal void ProvideMud()
		{
			if (Main.mouseItem == null || Main.mouseItem.stack <= 0 || Main.mouseItem.type != ItemID.MudBlock)
				return;
			short addAmount = (short)Math.Min(Main.mouseItem.stack, MaxResources - Mud);
			Main.mouseItem.Consume(addAmount);
			Mud += addAmount;
			Main.PlaySound(SoundID.Grab);
			Recipe.FindRecipes();
			SendClientMessage(ExtractorMessage.SyncMud, addAmount);
			return;
		}

		internal void ExtractChloro()
		{
			SendClientMessage(ExtractorMessage.SyncChlorophyte);
			if (Main.netMode == NetmodeID.MultiplayerClient)
				return;
			Item.NewItem(new Rectangle((Position.X - 1) * 16, (Position.Y - 1) * 16, 54, 54), ItemID.ChlorophyteOre, Chlorophyte);
			Chlorophyte = 0;
		}

		internal void OpenUI(bool fromNet = false)
		{
			bool switching = ChlorophyteExtractorUI.ExtractorTE.CurrentPlayer != byte.MaxValue;
			if (switching)
				ChlorophyteExtractorUI.ExtractorTE.CurrentPlayer = byte.MaxValue;
			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				ModPacket packet;
				if (switching)
				{
					packet = GadgetBox.Instance.GetPacket(MessageType.ExtractorMessage, 6);
					packet.Write(ChlorophyteExtractorUI.ExtractorTE.ID);
					packet.Write((byte)ExtractorMessage.SyncPlayer);
					packet.Write(byte.MaxValue);
					packet.Send();
				}
				if (!fromNet)
				{
					Main.stackSplit = 600;
					packet = GadgetBox.Instance.GetPacket(MessageType.ExtractorMessage, 5);
					packet.Write(ID);
					packet.Write((byte)ExtractorMessage.RequestExtractorOpen);
					packet.Send();
					return;
				}
			}
			Main.stackSplit = 600;
			if (PlayerInput.GrappleAndInteractAreShared)
				PlayerInput.Triggers.JustPressed.Grapple = false;
			ChlorophyteExtractorUI.visible = true;
			ChlorophyteExtractorUI.ExtractorTE = this;
			Main.playerInventory = true;
			Main.recBigList = false;
			Main.PlaySound(switching ? SoundID.MenuTick : SoundID.MenuOpen);
		}

		internal void CloseUI(bool silent = false, bool fromNet = false)
		{
			if (!fromNet)
			{
				if (Main.netMode == NetmodeID.MultiplayerClient)
				{
					ModPacket packet = GadgetBox.Instance.GetPacket(MessageType.ExtractorMessage, 6);
					packet.Write(ID);
					packet.Write((byte)ExtractorMessage.SyncPlayer);
					packet.Write(byte.MaxValue);
					packet.Send();
				}
				else
					Main.stackSplit = 600;
			}
			ChlorophyteExtractorUI.visible = false;
			CurrentPlayer = byte.MaxValue;
			ChlorophyteExtractorUI.ExtractorTE = new ChlorophyteExtractorTE();
			if (!silent)
				Main.PlaySound(SoundID.MenuClose);
		}

		void ExtractorEffect(short i, short j)
		{
			if (Main.netMode == NetmodeID.Server)
			{
				ModPacket packet = GadgetBox.Instance.GetPacket(MessageType.ExtractorMessage, 9);
				packet.Write(ID);
				packet.Write((byte)ExtractorMessage.ExtractorEffect);
				packet.Write(i);
				packet.Write(j);
				packet.Send();
				return;
			}

			Vector2 position = new Vector2(i * 16, j * 16);
			Main.PlaySound(SoundID.Tink, (int)(position.X + 8), (int)(position.Y + 8), 1, 0.5f);
			for (int d = 0; d < 10; d++)
			{
				Dust dust = Dust.NewDustDirect(position, 16, 16, 128);
				dust.velocity *= 0.2f;
				dust.noGravity = true;
				dust.noLight = false;
				dust.fadeIn = 1.5f;
			}
		}

		void SendServerMessage(ExtractorMessage serverMsg = ExtractorMessage.SyncAll)
		{
			if (Main.netMode != NetmodeID.Server)
				return;
			bool syncAll = serverMsg == ExtractorMessage.SyncAll;
			ModPacket packet = GadgetBox.Instance.GetPacket(MessageType.ExtractorMessage, syncAll ? 12 : serverMsg == ExtractorMessage.SyncOn ? 6 : 7);
			packet.Write(ID);
			packet.Write((byte)serverMsg);
			if (serverMsg == ExtractorMessage.SyncOn || syncAll)
				packet.Write(IsON);
			if (serverMsg == ExtractorMessage.SyncPower || syncAll)
				packet.Write(Power);
			if (serverMsg == ExtractorMessage.SyncMud || syncAll)
				packet.Write(Mud);
			if (serverMsg == ExtractorMessage.SyncChlorophyte || syncAll)
				packet.Write(Chlorophyte);
			packet.Send();
		}

		void SendClientMessage(ExtractorMessage clientMsg, short mud = 0)
		{
			if (Main.netMode != NetmodeID.MultiplayerClient)
				return;
			bool AddMud = clientMsg == ExtractorMessage.SyncMud;
			ModPacket packet = GadgetBox.Instance.GetPacket(MessageType.ExtractorMessage, AddMud ? 7 : 5);
			packet.Write(ID);
			packet.Write((byte)clientMsg);
			if (AddMud)
				packet.Write(mud);
			packet.Send();
		}

		enum ExtractorMessage : byte
		{
			None,
			SyncOn,
			SyncPower,
			SyncMud,
			SyncChlorophyte,
			SyncAll,
			RequestExtractorOpen,
			SyncPlayer,
			ExtractorEffect
		}
	}
}