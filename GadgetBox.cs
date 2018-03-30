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
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace GadgetBox
{
	internal class GadgetBox : Mod
	{
		internal static GadgetBox Instance;
		internal static string AnyGoldBar;

		internal UserInterface chloroExtractInterface;
		internal ChlorophyteExtractorUI chlorophyteExtractorUI;
		internal UserInterface reforgeMachineInterface;
		internal ReforgeMachineUI reforgeMachineUI;

		int lastSeenScreenWidth;
		int lastSeenScreenHeight;
		bool lastFocus;

		public GadgetBox()
		{
			Properties = new ModProperties()
			{
				Autoload = true
			};
		}

		public override void Load()
		{
			Version targetVersion = new Version(0, 10, 1, 3);
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
				reforgeMachineUI = new ReforgeMachineUI();
				reforgeMachineUI.Activate();
				reforgeMachineInterface.SetState(reforgeMachineUI);
			}
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
				chloroExtractInterface.Update(gameTime);
			else
				chlorophyteExtractorUI.powerButton.Update(gameTime);

			if (ReforgeMachineUI.visible)
				reforgeMachineInterface.Update(gameTime);
		}

		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			int invIndex = layers.FindIndex(l => l.Name.Equals("Vanilla: Inventory"));
			if (invIndex != -1)
			{
				layers.Insert(invIndex++, new LegacyGameInterfaceLayer(Name + ": RightClick Hacks", RightClickHacks));

				layers.Insert(invIndex + 1, new LegacyGameInterfaceLayer(Name + ": Machine UI", () =>
					{
						if (Main.playerInventory && !Main.recBigList)
						{
							if (lastSeenScreenWidth != Main.screenWidth || lastSeenScreenHeight != Main.screenHeight || !lastFocus && Main.hasFocus)
							{
								chlorophyteExtractorUI.Recalculate();
								reforgeMachineUI.Recalculate();
								lastSeenScreenWidth = Main.screenWidth;
								lastSeenScreenHeight = Main.screenHeight;
							}

							if (lastFocus != Main.hasFocus)
								lastFocus = Main.hasFocus;

							if (ChlorophyteExtractorUI.visible)
								chlorophyteExtractorUI.Draw(Main.spriteBatch);
							if (ReforgeMachineUI.visible)
								reforgeMachineUI.Draw(Main.spriteBatch);
						}
						return true;
					},
					InterfaceScaleType.UI)
				);
			}
		}

		public override void AddRecipeGroups()
		{
			RecipeGroup group = new RecipeGroup(() => Language.GetTextValue("LegacyMisc.37") + " " + Lang.GetItemNameValue(ItemID.GoldBar), new int[]
			{
				ItemID.GoldBar,
				ItemID.PlatinumBar
			});
			AnyGoldBar = Name + ":AnyGoldBar";
			RecipeGroup.RegisterGroup(AnyGoldBar, group);
		}

		public override void AddRecipes()
		{
			GadgetRecipes.AddRecipes(this);
		}

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

		bool RightClickHacks()
		{
			if (!Main.playerInventory || PlayerInput.IgnoreMouseInterface || !Main.mouseRight || !Main.mouseRightRelease)
				return true;
			Player player = Main.player[Main.myPlayer];

			float scale = 0.85f;
			int x, y, slot;

			for (int i = 0; i < 10; i++)
			{
				for (int j = 0; j < 5; j++)
				{
					x = (int)(20f + i * 56 * scale);
					y = (int)(20f + j * 56 * scale);
					if (Main.mouseX < x || Main.mouseX > x + Main.inventoryBackTexture.Width * scale || Main.mouseY < y || Main.mouseY > y + Main.inventoryBackTexture.Height * scale)
						continue;

					slot = i + j * 10;
					Item item = player.inventory[slot];
					if (item.IsAir)
						return true;

					if (item.type == ItemID.LockBox)
					{
						if (player.HasItem(ItemID.GoldenKey) || !player.HasItem(ItemType<MasterKey>()))
							return true;

						item.Consume(1, false);
						Main.PlaySound(SoundID.Grab);
						Main.stackSplit = 30;
						player.openLockBox();
						Main.mouseRightRelease = false;
						Recipe.FindRecipes();
					}
					else if (Main.mouseItem.type == ItemType<ReforgingKit>() || Main.mouseItem.type == ItemType<LesserReforgingKit>())
					{
						if (Main.mouseItem.type == ItemType<LesserReforgingKit>() && item.prefix != 0 || !item.Prefix(-3) || !ItemLoader.PreReforge(item))
							return true;

						GadgetMethods.PrefixItem(ref player.inventory[slot]);

						Main.mouseItem.Consume();
						player.inventory[58] = Main.mouseItem.Clone();
						Main.mouseRightRelease = false;
						Recipe.FindRecipes();
					}
					return true;
				}
			}
			return true;
		}
	}

	internal enum MessageType
	{
		CatchNPC,
		ExtractorMessage
	}
}