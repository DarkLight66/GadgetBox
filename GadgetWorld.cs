using System.Collections.Generic;
using GadgetBox.Items;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.World.Generation;

namespace GadgetBox
{
	public class GadgetWorld : ModWorld
	{
		public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
		{
			var LastIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Final Cleanup"));
			if (LastIndex != -1)
			{
				tasks.Insert(LastIndex, new PassLegacy("Adding More Loot", delegate (GenerationProgress progress)
				{
					progress.Message = "Adding More Loot";
					progress.CurrentPassWeight = 1.0f;

					foreach (var chest in Main.chest)
					{
						if (chest == null)
							continue;
						Tile tile = Main.tile[chest.x, chest.y];
						if (!TileID.Sets.BasicChest[tile.type] || tile.type > TileID.Count || chest.item == null)
							continue;
						int chestType = tile.frameX / 36;
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

						if (!Main.rand.NextBool(chance))
							continue;

						int itemid = mod.ItemType<LesserReforgingKit>();
						if (chestType == 16 || chestType >= 23 && chestType <= 27)
							itemid = mod.ItemType<ReforgingKit>();

						for (int i = 0; i < chest.item.Length; i++)
						{
							if (chest.item[i] == null)
								chest.item[i] = new Item();
							if (chest.item[i].type != 0 && chest.item[i].stack > 0)
								continue;
							if (i < 2)
								break;
							chest.item[i].SetDefaults(itemid);
							if (Main.rand.NextBool(3))
								chest.item[i].stack = 2;
							break;
						}
					}
				}));
			}
		}
	}
}