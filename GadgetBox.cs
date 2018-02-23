using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using GadgetBox.GadgetUI;
using GadgetBox.Tiles;
using Terraria.DataStructures;

namespace GadgetBox
{
    class GadgetBox : Mod
    {
        internal static GadgetBox Instance;
		internal static string AnyGoldBar;

		internal static UserInterface chloroExtractInterface;
		internal ChlorophyteExtractorUI chlorophyteExtractorUI;

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

		int lastSeenScreenWidth;
		int lastSeenScreenHeight;

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
							chloroExtractInterface.Update(Main._drawInterfaceGameTime);
							chlorophyteExtractorUI.Draw(Main.spriteBatch);
						}
						else
							chlorophyteExtractorUI.powerButton.Update(Main._drawInterfaceGameTime);
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
				case MessageType.SyncPlayerExtractor:
					ChlorophyteExtractorTE.SyncPlayerExtractor(reader, whoAmI);
					break;
				case MessageType.SyncPlayerExtractorIndex:
					ChlorophyteExtractorTE.SyncPlayerExtractorIndex(reader, whoAmI);
					break;
				case MessageType.ExtractorMessage:
					TileEntity ExtractorTE;
					if(TileEntity.ByID.TryGetValue(reader.ReadInt16(), out ExtractorTE) && ExtractorTE is ChlorophyteExtractorTE)
					((ChlorophyteExtractorTE)ExtractorTE).RecieveMessage(reader, whoAmI);
					break;
			}
		}

		internal ModPacket GetPacket(MessageType type, int capacity)
        {
            ModPacket packet = GetPacket(capacity + 1);
            packet.Write((byte)type);
            return packet;
        }

        internal static void Log(object message)
            => ErrorLogger.Log(string.Format("[{0}][{1}] {2}", Instance, DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), message));

        internal static void Log(string message, params object[] formatData)
            => ErrorLogger.Log(string.Format("[{0}][{1}] {2}", Instance, DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), string.Format(message, formatData)));
    }

    internal enum MessageType
    {
        CatchNPC,
		RequestExtractorOpen,
		SyncPlayerExtractor,
		CloseExtractor,
		ExtractorMessage,
		SyncPlayerExtractorIndex
	}
}
