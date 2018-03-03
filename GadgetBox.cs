using System;
using System.Collections.Generic;
using System.IO;
using GadgetBox.GadgetUI;
using GadgetBox.Tiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace GadgetBox
{
	internal class GadgetBox : Mod
	{
		internal static GadgetBox Instance;
		internal static string AnyGoldBar;

		internal static UserInterface chloroExtractInterface;
		internal ChlorophyteExtractorUI chlorophyteExtractorUI;

		int lastSeenScreenWidth;

		int lastSeenScreenHeight;

		public GadgetBox()
		{
			Properties = new ModProperties()
			{
				Autoload = true,
				AutoloadGores = true,
				AutoloadSounds = true
			};
		}

		public override void Load()
		{
			Version targetVersion = new Version(0, 10, 1, 3);
			if (ModLoader.version < targetVersion)
			{
				throw new Exception(string.Format("\nThis mod uses functionality only present in versions {0} or newer of tModLoader. Please update tModLoader to use this mod\n\n", targetVersion));
			}

			Instance = this;
			if (!Main.dedServ)
			{
				chlorophyteExtractorUI = new ChlorophyteExtractorUI();
				chlorophyteExtractorUI.Activate();
				chloroExtractInterface = new UserInterface();
				chloroExtractInterface.SetState(chlorophyteExtractorUI);
			}
		}

		public override void Unload()
		{
			ChlorophyteExtractorUI.ExtractorTE = null;
			chloroExtractInterface = null;
			Instance = null;
		}

		public override void UpdateUI(GameTime gameTime)
		{
			chloroExtractInterface.Update(gameTime);
		}

		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			int invIndex = layers.FindIndex(l => l.Name.Equals("Vanilla: Inventory"));
			if (invIndex != -1)
			{
				layers.Insert(invIndex, new LegacyGameInterfaceLayer(
					Name + ": Chlorophyte Extractor UI",
					delegate
					{
						if (ChlorophyteExtractorUI.visible && !Main.recBigList)
						{
							if (lastSeenScreenWidth != Main.screenWidth || lastSeenScreenHeight != Main.screenHeight)
							{
								chlorophyteExtractorUI.Recalculate();
								lastSeenScreenWidth = Main.screenWidth;
								lastSeenScreenHeight = Main.screenHeight;
							}
							chlorophyteExtractorUI.Draw(Main.spriteBatch);
						}
						return true;
					},
					InterfaceScaleType.UI)
				);
			}
		}

		public override void AddRecipeGroups()
		{
			RecipeGroup group = new RecipeGroup(() => Language.GetTextValue("LegacyMisc.37") + " " + Language.GetTextValue("ItemName.GoldBar"), new int[]
			{
				ItemID.GoldBar,
				ItemID.PlatinumBar
			});
			AnyGoldBar = Name + ":AnyGoldBar";
			RecipeGroup.RegisterGroup(AnyGoldBar, group);
		}

		public override void HandlePacket(BinaryReader reader, int whoAmI)
		{
			MessageType message = (MessageType)reader.ReadByte();
			switch (message)
			{
				case MessageType.CatchNPC:
					GadgetMethods.CatchNPC(reader.ReadByte(), whoAmI, reader.ReadBoolean()); // fixes catching of npcs with projectiles
					break;
				case MessageType.RequestExtractorOpen:
					ChlorophyteExtractorTE.ReceiveRequestOpen(reader, whoAmI);
					break;
				case MessageType.SyncExtractorPlayer:
					ChlorophyteExtractorTE.SyncExtractorPlayer(reader, whoAmI);
					break;
				case MessageType.SyncExtractorPlayerIndex:
					ChlorophyteExtractorTE.SyncExtractorPlayerIndex(reader, whoAmI);
					break;
				case MessageType.ExtractorClientMessage:
					ChlorophyteExtractorTE.ExtractorByID(reader.ReadInt32())?.RecieveClientMessage(reader, whoAmI);
					break;
				case MessageType.ExtractorServerMessage:
					ChlorophyteExtractorTE.ExtractorByID(reader.ReadInt32())?.RecieveServerMessage(reader, whoAmI);
					break;
			}
		}

		internal static void Log(object message)
			=> ErrorLogger.Log(string.Format("[{0}][{1}] {2}", Instance, DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), message));

		internal static void Log(string message, params object[] formatData)
			=> ErrorLogger.Log(string.Format("[{0}][{1}] {2}", Instance, DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), string.Format(message, formatData)));

		internal ModPacket GetPacket(MessageType type, int capacity)
		{
			ModPacket packet = GetPacket(capacity + 1);
			packet.Write((byte)type);
			return packet;
		}
	}

	internal enum MessageType
	{
		CatchNPC,
		RequestExtractorOpen,
		SyncExtractorPlayer,
		SyncExtractorPlayerIndex,
		CloseExtractor,
		ExtractorClientMessage,
		ExtractorServerMessage
	}
}