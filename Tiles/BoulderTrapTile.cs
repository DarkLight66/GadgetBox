﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace GadgetBox.Tiles
{
	public class BoulderTrapTile : ModTile
	{
		public override void SetDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileBlockLight[Type] = true;
			Main.tileSolid[Type] = true;
			TileObjectData.newTile.Width = 2;
			TileObjectData.newTile.Height = 2;
			TileObjectData.newTile.Origin = Point16.Zero;
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 };
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidBottom, 2, 0);
			TileObjectData.newAlternate.Origin = new Point16(1, 0);
			TileObjectData.addAlternate(0);
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.AnchorWall = true;
			TileObjectData.addAlternate(0);
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Origin = new Point16(1, 0);
			TileObjectData.newAlternate.AnchorWall = true;
			TileObjectData.addAlternate(0);
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Origin = new Point16(1, 0);
			TileObjectData.newAlternate.AnchorRight = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, 2, 0);
			TileObjectData.addAlternate(1);
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.AnchorLeft = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, 2, 0);
			TileObjectData.addAlternate(2);
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Origin = new Point16(0, 1);
			TileObjectData.newAlternate.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide | AnchorType.SolidWithTop, 2, 0);
			TileObjectData.addAlternate(3);
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Origin = new Point16(1, 1);
			TileObjectData.newAlternate.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide | AnchorType.SolidWithTop, 2, 0);
			TileObjectData.addAlternate(3);
			TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidBottom, 2, 0);
			TileObjectData.addTile(Type);
			AddMapEntry(new Color(141, 56, 0));
			AddMapEntry(new Color(50, 15, 8));
			dustType = 148;
			disableSmartCursor = true;
		}

		public override ushort GetMapOption(int i, int j)
		{
			Tile tile = Main.tile[i, j];
			if (tile.wall == WallID.LihzahrdBrickUnsafe && tile.inActive() && !NPC.downedGolemBoss)
				return 1;
			return 0;
		}

		public override bool Slope(int i, int j)
		{
			int y = j - (Main.tile[i, j].frameY / 18);
			int x = i - ((Main.tile[i, j].frameX % 36) / 18);
			int frame = ((Main.tile[x, y].frameX / 36) + 1) % 4;

			Main.tile[x, y].frameX = (short)(frame * 36);
			Main.tile[x + 1, y].frameX = (short)(frame * 36 + 18);
			Main.tile[x, y + 1].frameX = (short)(frame * 36);
			Main.tile[x + 1, y + 1].frameX = (short)(frame * 36 + 18);

			if (Main.netMode == 1)
				NetMessage.SendTileSquare(-1, x, y, 2);
			return false;
		}

		public override void HitWire(int i, int j)
		{
			Point16 Pos = new Point16(i - ((Main.tile[i, j].frameX % 36) / 18), j - (Main.tile[i, j].frameY / 18));

			Wiring.SkipWire(Pos);
			Wiring.SkipWire(Pos.X, Pos.Y + 1);
			Wiring.SkipWire(Pos.X + 1, Pos.Y);
			Wiring.SkipWire(Pos.X + 1, Pos.Y + 1);

			if (Wiring.CheckMech(Pos.X, Pos.Y, 500))
			{
				int cframe = Main.tile[Pos.X, Pos.Y].frameX / 36;
				Vector2 spawn = Pos.ToWorldCoordinates(16f, 16f);
				Vector2 velocity = Vector2.UnitY;
				switch (cframe)
				{
					case 1:
						velocity = -Vector2.UnitX;
						break;
					case 2:
						velocity = Vector2.UnitX;
						break;
					case 3:
						velocity = -velocity;
						break;
				}
				Projectile.NewProjectile(spawn, velocity * 3.5f, mod.ProjectileType<Projectiles.TempleBoulder>(), 70, 3f, Main.myPlayer, Main.rand.Next(2));
			}
		}

		public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;

		public override bool CanKillTile(int i, int j, ref bool blockDamaged) => CanMineTrap(i, j, Type);

		public override bool CanExplode(int i, int j) => CanMineTrap(i, j, Type);

		public override void KillMultiTile(int i, int j, int frameX, int frameY) => Item.NewItem(i * 16, j * 16, 32, 32, mod.ItemType<Items.Placeable.BoulderTrap>());

		public override bool Dangersense(int i, int j, Player player) => true;

		public static bool CanMineTrap(int i, int j, ushort trap)
		{
			int y = j - (Main.tile[i, j].frameY / 18);
			int x = i - ((Main.tile[i, j].frameX % 36) / 18);

			ushort type1 = Main.tile[x, y - 1].type, type2 = Main.tile[x + 1, y - 1].type;
			bool cankill = !TileID.Sets.BasicChest[type2] && !TileID.Sets.BasicChest[type1] && !TileID.Sets.BasicChestFake[type1] && !TileID.Sets.BasicChestFake[type2] && !TileLoader.IsDresser(type2) && !TileLoader.IsDresser(type1);

			if (cankill)
			{
				if (y != j || x == i)
				{
					Main.tile[x + 1, y].type = TileID.Stone;
					cankill &= WorldGen.CanKillTile(x + 1, y);
					Main.tile[x + 1, y].type = trap;
				}
				if (y != j || x != i)
				{
					Main.tile[x, y].type = TileID.Stone;
					cankill &= WorldGen.CanKillTile(x, y);
					Main.tile[x, y].type = trap;
				}
			}
			return cankill;
		}
	}
}