using System;
using System.Collections.Generic;
using System.IO;
using GadgetBox.GadgetUI;
using GadgetBox.Items;
using GadgetBox.Items.Consumables;
using GadgetBox.Tiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace GadgetBox
{
	public class GadgetBox : Mod
	{
		public static GadgetBox Instance { get; private set; }

		internal UserInterface chloroExtractInterface;
		internal ChlorophyteExtractorUI chlorophyteExtractorUI;
		internal UserInterface reforgeMachineInterface;

		int lastSeenScreenWidth;
		int lastSeenScreenHeight;
		bool lastFocus;

		public override void Load()
		{
			Version targetVersion = new Version(0, 11);
			if (ModLoader.version < targetVersion)
			{
				throw new Exception($"\nThis mod uses functionality only present in versions {targetVersion} or newer of tModLoader. Please update tModLoader to use this mod\n\n");
			}

			Instance = this;

			if (!Main.dedServ)
			{
				chloroExtractInterface = new UserInterface();
				chlorophyteExtractorUI = new ChlorophyteExtractorUI();
				chlorophyteExtractorUI.Activate();
				chloroExtractInterface.SetState(chlorophyteExtractorUI);
				reforgeMachineInterface = new UserInterface();
			}

			GadgetHooks.Initialize();
		}

		public override void PostSetupContent()
		{
			ModCompat.Load();
		}

		public override void Unload()
		{
			ModCompat.Unload();
			ChlorophyteExtractorUI.ExtractorTE = null;
			Instance = null;
		}

		public override void UpdateUI(GameTime gameTime)
		{
			if (ChlorophyteExtractorUI.visible)
			{
				chloroExtractInterface.Update(gameTime);
			}
			else
			{
				chlorophyteExtractorUI.powerButton.Update(gameTime);
			}

			if (reforgeMachineInterface != null)
			{
				reforgeMachineInterface.Update(gameTime);
			}
		}

		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			int invIndex = layers.FindIndex(l => l.Name.Equals("Vanilla: Inventory"));
			if (invIndex != -1)
			{
				layers.Insert(invIndex, new LegacyGameInterfaceLayer(Name + ": Machine UI", () =>
					{
						if (Main.playerInventory && !Main.recBigList)
						{
							if (lastSeenScreenWidth != Main.screenWidth || lastSeenScreenHeight != Main.screenHeight || !lastFocus && Main.hasFocus)
							{
								chlorophyteExtractorUI.Recalculate();
								reforgeMachineInterface.Recalculate();
								lastSeenScreenWidth = Main.screenWidth;
								lastSeenScreenHeight = Main.screenHeight;
							}
							if (lastFocus != Main.hasFocus)
							{
								lastFocus = Main.hasFocus;
							}
							if (ChlorophyteExtractorUI.visible)
							{
								chlorophyteExtractorUI.Draw(Main.spriteBatch);
							}
							reforgeMachineInterface.Draw(Main.spriteBatch, Main._drawInterfaceGameTime);
						}
						return true;
					},
					InterfaceScaleType.UI)
				);
			}
		}

		public override void AddRecipeGroups() => GadgetRecipes.AddRecipeGroups(this);
		public override void AddRecipes() => GadgetRecipes.AddRecipes(this);

		public override void HandlePacket(BinaryReader reader, int whoAmI)
		{
			MessageType message = (MessageType)reader.ReadByte();
			switch (message)
			{
				case MessageType.CatchNPC:
					GadgetMethods.CatchNPC(reader.ReadByte(), whoAmI, reader.ReadBoolean()); // fixes catching of npcs with projectiles
					break;
				case MessageType.ExtractorMessage:
					this.GetTileEntity<ChlorophyteExtractorTE>(reader.ReadInt32())?.ReceiveExtractorMessage(reader, whoAmI);
					break;
				case MessageType.TripWire:
					int tileX = reader.ReadInt32();
					int tileY = reader.ReadInt32();
					if (WorldGen.InWorld(tileX, tileY) && Main.tile[tileX, tileY] != null)
					{
						Items.Tools.ActivationRod.TriggerMech(tileX, tileY);
					}
					break;
			}
		}

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
		ExtractorMessage,
		TripWire
	}
}