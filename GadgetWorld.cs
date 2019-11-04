using System.Collections.Generic;
using GadgetBox.Items.Consumables;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.World.Generation;
using static Terraria.ModLoader.ModContent;

namespace GadgetBox
{
	public class GadgetWorld : ModWorld
	{
		public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
		{
			int genIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Final Cleanup"));
			if (genIndex != -1)
			{
				tasks.Insert(genIndex, new PassLegacy("Adding More Loot", GenerateMoreLoot));
			}

			genIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Lihzahrd Altars"));
			if (genIndex != -1)
			{
				tasks.Insert(genIndex + 1, new PassLegacy("Boulder Traps", GenerateTrap));
			}
		}

		public void GenerateMoreLoot(GenerationProgress progress)
		{
			progress.Message = "Adding More Loot";
			progress.CurrentPassWeight = 1.0f;

			foreach (var chest in Main.chest)
			{
				if (chest == null)
				{
					continue;
				}

				Tile tile = Main.tile[chest.x, chest.y];
				if (!TileID.Sets.BasicChest[tile.type] || chest.item == null)
				{
					continue;
				}

				int chestType = tile.type >= TileID.Count ? 1 : tile.frameX / 36;
				int chance = 5;

				switch (chestType)
				{
					case 0:
					case 17:
						chance = 9;
						break;

					case 2:
					case 8:
					case 10:
						chance = 4;
						break;

					case 4:
					case 13:
					case 15:
						chance = 3;
						break;
				}

				if (!WorldGen.genRand.NextBool(chance))
				{
					continue;
				}

				int itemid = ItemType<LesserReforgingKit>();
				if (chestType == 5 || chestType == 16 || chestType >= 23 && chestType <= 27 || chestType == 2 && WorldGen.genRand.NextBool(2) || WorldGen.genRand.NextBool(4))
				{
					itemid = ItemType<ReforgingKit>();
				}

				for (int i = 0; i < chest.item.Length; i++)
				{
					if (chest.item[i] == null)
					{
						chest.item[i] = new Item();
					}

					if (chest.item[i].type != 0 && chest.item[i].stack > 0)
					{
						continue;
					}

					if (i < 2)
					{
						break;
					}

					chest.item[i].SetDefaults(itemid);
					chest.item[i].stack = WorldGen.genRand.Next(1, 5);
					break;
				}
			}
		}

		public void GenerateTrap(GenerationProgress progress)
		{
			progress.Message = "Adding More Temple Traps";

			int minX = WorldGen.tLeft + 5, minY = WorldGen.tTop + 5;
			int maxX = WorldGen.tRight - 5, maxY = WorldGen.tBottom - 5;

			int trapsAmount = 1 + (int)(WorldGen.tRooms * (0.8f + WorldGen.genRand.NextFloat(-0.2f, 0.2f)));

			int tileX, tileY, trapsGen = 0, genAttemps = 0;
			progress.CurrentPassWeight = progress.TotalWeight * 0.01f;
			progress.Set(0);

			bool floorTrap = WorldGen.genRand.NextBool(), cFloorTrap;
			while (trapsGen < trapsAmount)
			{
				tileX = WorldGen.genRand.Next(minX, maxX);
				tileY = WorldGen.genRand.Next(minY, maxY);
				Tile tile = Framing.GetTileSafely(tileX, tileY);

				cFloorTrap = genAttemps > 50 ? WorldGen.genRand.NextBool() : floorTrap;
				if (tile.wall == WallID.LihzahrdBrickUnsafe && !tile.active() && GenerateBoulderTrap(tileX, tileY, cFloorTrap) || ++genAttemps > 100)
				{
					trapsGen++;
					floorTrap = WorldGen.genRand.NextBool();
					progress.Set((float)trapsGen / trapsAmount);
					genAttemps = 0;
				}
			}
		}

		public bool GenerateBoulderTrap(int x, int y, bool floorTrap)
		{
			int plateY = y;

			while (!WorldGen.SolidTile3(x, plateY))
			{
				plateY++;
				if (plateY > WorldGen.tBottom)
				{
					return false;
				}
			}

			if (Main.tile[x, plateY].type == TileID.WoodenSpikes)
			{
				return false;
			}

			plateY--;

			if (Main.tile[x, plateY].type == TileID.LihzahrdAltar)
			{
				return false;
			}

			int trapY = y, trapX = WorldGen.genRand.Next(2), plateX = WorldGen.genRand.Next(2);
			bool placePlate = true;
			byte wireColor = 4;

			if (floorTrap)
			{
				trapY = plateY + 1;

				if ((Main.tile[x - 1 + ((trapX ^ 1) * 2), plateY].type != TileID.LihzahrdBrick &&
					Main.tile[x - 1 + ((trapX ^ 1) * 2), plateY].type != TileID.WoodenSpikes &&
					Main.tile[x - 1 + ((trapX ^ 1) * 2), plateY].type != TileID.LihzahrdAltar) &&
					GadgetMethods.TileAreaCheck(x - trapX, trapY, 2, 2, true, TileID.LihzahrdBrick, true, WallID.LihzahrdBrickUnsafe))
				{
					trapX = x - trapX;
				}
				else if ((Main.tile[x - 1 + (trapX * 2), plateY].type != TileID.LihzahrdBrick &&
					Main.tile[x - 1 + (trapX * 2), plateY].type != TileID.WoodenSpikes &&
					Main.tile[x - 1 + (trapX * 2), plateY].type != TileID.LihzahrdAltar) &&
					GadgetMethods.TileAreaCheck(x - (trapX ^ 1), trapY, 2, 2, true, TileID.LihzahrdBrick, true, WallID.LihzahrdBrickUnsafe))
				{
					trapX = x - (trapX ^ 1);
				}
				else
				{
					return false;
				}

				placePlate = !(GadgetMethods.HasWire(trapX, plateY) || GadgetMethods.HasWire(trapX + 1, plateY));

				if (placePlate)
				{
					if (WorldGen.TileEmpty(trapX + plateX, plateY))
					{
						plateX = trapX + plateX;
					}
					else if (WorldGen.TileEmpty(trapX + (plateX ^ 1), plateY))
					{
						plateX = trapX + (plateX ^ 1);
					}
					else
					{
						return false;
					}

					if (Main.tile[trapX + (trapX == plateX ? 1 : 0), trapY].slope() != 0)
					{
						Main.tile[trapX + (trapX == plateX ? 1 : 0), trapY].slope(0);
					}

					if (Main.tile[trapX + (trapX == plateX ? 1 : 0), trapY].halfBrick())
					{
						Main.tile[trapX + (trapX == plateX ? 1 : 0), trapY].halfBrick(false);
					}
				}
				else
				{
					wireColor = GadgetMethods.GetRandomWireColor(trapX + plateX, plateY, 0);
					if (wireColor == 0)
					{
						wireColor = GadgetMethods.GetRandomWireColor(trapX + (plateX ^ 1), plateY, 4);
					}

					if (Main.tile[trapX, trapY].slope() != 0)
					{
						Main.tile[trapX, trapY].slope(0);
					}

					if (Main.tile[trapX, trapY].halfBrick())
					{
						Main.tile[trapX, trapY].halfBrick(false);
					}

					if (Main.tile[trapX + 1, trapY].slope() != 0)
					{
						Main.tile[trapX + 1, trapY].slope(0);
					}

					if (Main.tile[trapX + 1, trapY].halfBrick())
					{
						Main.tile[trapX + 1, trapY].halfBrick(false);
					}
				}
			}
			else
			{
				if (!GadgetMethods.HasWire(x, plateY) && Main.tile[x, plateY].active())
				{
					return false;
				}

				while (plateY - trapY < 7)
				{
					trapY--;
					if (trapY < WorldGen.tTop || WorldGen.SolidTile3(x, trapY))
					{
						return false;
					}
				}

				bool unoccupiedt = GadgetMethods.TileAreaCheck(x, trapY, 1, 2, false, 0, true, WallID.LihzahrdBrickUnsafe) &&
					(GadgetMethods.TileAreaCheck(x + 1, trapY, 1, 2, false, 0, true, WallID.LihzahrdBrickUnsafe) ||
					GadgetMethods.TileAreaCheck(x - 1, trapY, 1, 2, false, 0, true, WallID.LihzahrdBrickUnsafe));

				if (!unoccupiedt)
				{
					return false;
				}

				bool unoccupiedb = unoccupiedt;

				int maxY = trapY;

				while (unoccupiedb && (plateY - trapY) > 7)
				{
					trapY++;
					unoccupiedb = GadgetMethods.TileAreaCheck(x, trapY, 1, 2, false, 0, true, WallID.LihzahrdBrickUnsafe) &&
						(GadgetMethods.TileAreaCheck(x + 1, trapY, 1, 2, false, 0, true, WallID.LihzahrdBrickUnsafe) ||
						GadgetMethods.TileAreaCheck(x - 1, trapY, 1, 2, false, 0, true, WallID.LihzahrdBrickUnsafe));

					if (!unoccupiedb)
					{
						trapY -= 3;
						break;
					}
				}

				while (unoccupiedt)
				{
					maxY--;
					unoccupiedt = GadgetMethods.TileAreaCheck(x, maxY, 1, 2, false, 0, true, WallID.LihzahrdBrickUnsafe) &&
						(GadgetMethods.TileAreaCheck(x + 1, maxY, 1, 2, false, 0, true, WallID.LihzahrdBrickUnsafe) ||
						GadgetMethods.TileAreaCheck(x - 1, maxY, 1, 2, false, 0, true, WallID.LihzahrdBrickUnsafe));

					if (!unoccupiedt)
					{
						maxY += 3;
						if (trapY < maxY)
						{
							return false;
						}

						break;
					}
				}

				trapY = trapY == maxY ? trapY : WorldGen.genRand.Next(maxY, trapY + 1);

				if (GadgetMethods.TileAreaCheck(x - trapX, trapY, 2, 2, false, 0, true, WallID.LihzahrdBrickUnsafe))
				{
					trapX = x - trapX;
				}
				else if (GadgetMethods.TileAreaCheck(x - (trapX ^ 1), trapY, 2, 2, false, 0, true, WallID.LihzahrdBrickUnsafe))
				{
					trapX = x - (trapX ^ 1);
				}
				else
				{
					return false;
				}

				for (int i = trapX - 1; i < trapX + 3; i++)
				{
					for (int j = trapY - 1; j < trapY + 3; j++)
					{
						if ((i == trapX - 1 || i == trapX + 2) && (j == trapY - 1 || j == trapY + 2) || !GadgetMethods.HasWire(i, j))
						{
							continue;
						}

						placePlate = false;
						plateY = trapY;
						wireColor = GadgetMethods.GetRandomWireColor(i, j, 4);
						break;
					}
				}

				if (placePlate)
				{
					for (int j = trapY + 2; j <= plateY; j++)
					{
						if (!GadgetMethods.HasWire(x, j))
						{
							continue;
						}

						placePlate = false;
						plateY = j;
						wireColor = GadgetMethods.GetRandomWireColor(x, j, 4);
						break;
					}
				}

				for (int j = trapY + 2; j < plateY; j++)
				{
					GadgetMethods.PlaceWire(x, j, wireColor);
				}

				plateX = x;
			}

			ushort boulderTrap = (ushort)TileType<Tiles.BoulderTrapTile>();
			int style = floorTrap ? 3 : 0;

			Main.tile[trapX, trapY].active(true);
			Main.tile[trapX, trapY].frameY = 0;
			Main.tile[trapX, trapY].frameX = (short)(36 * style);
			Main.tile[trapX, trapY].type = boulderTrap;
			GadgetMethods.PlaceWire(trapX, trapY, wireColor);
			Main.tile[trapX + 1, trapY].active(true);
			Main.tile[trapX + 1, trapY].frameY = 0;
			Main.tile[trapX + 1, trapY].frameX = (short)(36 * style + 18);
			Main.tile[trapX + 1, trapY].type = boulderTrap;
			GadgetMethods.PlaceWire(trapX + 1, trapY, wireColor);
			Main.tile[trapX, trapY + 1].active(true);
			Main.tile[trapX, trapY + 1].frameY = 18;
			Main.tile[trapX, trapY + 1].frameX = (short)(36 * style);
			Main.tile[trapX, trapY + 1].type = boulderTrap;
			GadgetMethods.PlaceWire(trapX, trapY + 1, wireColor);
			Main.tile[trapX + 1, trapY + 1].active(true);
			Main.tile[trapX + 1, trapY + 1].frameY = 18;
			Main.tile[trapX + 1, trapY + 1].frameX = (short)(36 * style + 18);
			Main.tile[trapX + 1, trapY + 1].type = boulderTrap;
			GadgetMethods.PlaceWire(trapX + 1, trapY + 1, wireColor);

			if (!floorTrap)
			{
				Main.tile[trapX, trapY].inActive(true);
				Main.tile[trapX + 1, trapY].inActive(true);
				Main.tile[trapX, trapY + 1].inActive(true);
				Main.tile[trapX + 1, trapY + 1].inActive(true);
			}

			if (placePlate)
			{
				if (Main.tile[plateX, plateY + 1].slope() != 0)
				{
					Main.tile[plateX, plateY + 1].slope(0);
				}

				if (Main.tile[plateX, plateY + 1].halfBrick())
				{
					Main.tile[plateX, plateY + 1].halfBrick(false);
				}

				WorldGen.PlaceTile(plateX, plateY, TileID.PressurePlates, true, true, -1, 6);
				GadgetMethods.PlaceWire(plateX, plateY, wireColor);
			}
			return true;
		}
	}
}