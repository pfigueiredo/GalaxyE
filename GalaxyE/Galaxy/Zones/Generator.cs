using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GalaxyE.Galaxy {
	public class Generator {

		public enum Orientation { None, Vertical, Horizontal }
		public enum Direction { North, South, East, West, Up, Down }

		public abstract class Block {

			public class IdGenerator {
				private uint seed = 0;
				private uint cityId = 0;

				public IdGenerator(uint cityId) {
					this.cityId = cityId;
				}

				public uint GetNext() {
					return (++seed << 4) & 0xFFF0 | (cityId & 0xF);
				}


			}

			public Block North { get; set; }
			public Block South { get; set; }
			public Block East { get; set; }
			public Block West { get; set; }
			public Block Up { get; set; }
			public Block Down { get; set; }

			public IdGenerator IdSeed { get; private set; }
			public uint Id { get; private set; }
			public int X { get; private set; }
			public int Y { get; private set; }
			public Block[,] City { get; private set; }
			public Zone Zone { get; protected set; }

			public abstract string StrShape { get; }

			public Orientation Orientation { get; set; }

			public Block(Random rand, IdGenerator seed, Orientation orientation, Block[,] city, int x, int y) {
				this.Orientation = orientation;
				this.City = city;
				this.X = x;
				this.Y = y;
				this.Id = seed.GetNext();
				this.IdSeed = seed;

				if (IsInsideCity(x, y))
					this.City[x, y] = this;
			}

			public bool IsInsideCity(int x, int y) {
				return (x > 0 && x < City.GetLength(0)
					&& y > 0 && y < City.GetLength(1)
				);
			}

			public Block GetBlockAt(int x, int y) {

				if (x > 0 && x < City.GetLength(0)
					&& y > 0 && y < City.GetLength(1)
				)
					return City[x, y];

				return null;
			}

			public abstract void Generate(Random rand, int prob);

			public void GenerateCityBuildings(Random rand) {
				for (int y = 0; y < City.GetLength(1); y++) {
					for (int x = 0; x < City.GetLength(0); x++) {
						Block block = City[x, y];

						if (block == null) {

							StreetBlock a = GetBlockAt(x - 1, y) as StreetBlock;
							StreetBlock b = GetBlockAt(x + 1, y) as StreetBlock;
							StreetBlock c = GetBlockAt(x, y + 1) as StreetBlock;
							StreetBlock d = GetBlockAt(x, y - 1) as StreetBlock;

							if (a != null || b != null || c != null || d != null) {
								CityBuildingBlock newBlock = new CityBuildingBlock(rand, this.IdSeed, City, x, y);

								newBlock.North = GetBlockAt(x, y - 1);
								newBlock.South = GetBlockAt(x, y + 1);
								newBlock.East = GetBlockAt(x + 1, y);
								newBlock.West = GetBlockAt(x - 1, y);

							}
						}

					}
				}
			}
		}

		internal class CityBuildingBlock : Block {

			public override string StrShape {
				get { return "█"; }
			}

			public CityBuildingBlock(Random rand, Block.IdGenerator seed, Block[,] city, int x, int y)
				: base(rand, seed, Orientation.None, city, x, y) {
				
			}

			public override void Generate(Random rand, int prob) {

			}
		}

		internal class StreetBlock : Block {

			public override string StrShape {
				get {
					switch (Orientation) {
						case Generator.Orientation.Horizontal: return "═";
						case Generator.Orientation.Vertical: return "║";
						default: return " ";
					}
				}
			}

			public StreetBlock(Random rand, Block.IdGenerator seed, Orientation orientation, Block[,] city, int x, int y)
				: base(rand, seed, orientation, city, x, y) {
				
			}

			public override void Generate(Random rand, int prob) {
				if (this.Orientation == Generator.Orientation.Vertical) {

					this.North = GetBlockAt(this.X, this.Y -1);

					if (North == null && rand.Next(0, 100) < prob && IsInsideCity(this.X, this.Y - 1)) {
						North = new CitySquareBlock(rand, this.IdSeed, City, this.X, this.Y - 1);
						North.Generate(rand, prob - 1);
					}

					this.South = GetBlockAt(this.X, this.Y + 1);

					if (South == null && rand.Next(0, 100) < prob && IsInsideCity(this.X, this.Y + 1)) {
						South = new CitySquareBlock(rand, this.IdSeed, City, this.X, this.Y - 1);
						South.Generate(rand, prob - 1);
					}

					//this.East = GetBlockAt(this.X + 1, this.Y);

					//if (East == null && rand.Next(0, 100) < prob && IsInsideCity(this.X + 1, this.Y)) {
					//    East = new CitySquareBlock(rand, City, this.X + 1, this.Y);
					//    East.Generate(rand, prob - 1);
					//}

					//this.West = GetBlockAt(this.X - 1, this.Y);

					//if (West == null && rand.Next(0, 100) < prob && IsInsideCity(this.X - 1, this.Y)) {
					//    West = new CitySquareBlock(rand, City, this.X - 1, this.Y);
					//    West.Generate(rand, prob - 1);
					//}
				}

				if (this.Orientation == Generator.Orientation.Horizontal) {

					//this.North = GetBlockAt(this.X, this.Y - 1);

					//if (North == null && rand.Next(0, 100) < prob && IsInsideCity(this.X, this.Y - 1)) {
					//    North = new CitySquareBlock(rand, City, this.X, this.Y - 1);
					//    North.Generate(rand, prob - 1);
					//}

					//this.South = GetBlockAt(this.X, this.Y + 1);

					//if (South == null && rand.Next(0, 100) < prob && IsInsideCity(this.X, this.Y + 1)) {
					//    South = new CitySquareBlock(rand, City, this.X, this.Y - 1);
					//    South.Generate(rand, prob - 1);
					//}


					this.East = GetBlockAt(this.X + 1, this.Y);

					if (East == null && rand.Next(0, 100) < prob && IsInsideCity(this.X + 1, this.Y)) {
						East = new CitySquareBlock(rand, this.IdSeed, City, this.X + 1, this.Y);
						East.Generate(rand, prob - 1);
					}

					this.West = GetBlockAt(this.X - 1, this.Y);

					if (West == null && rand.Next(0, 100) < prob && IsInsideCity(this.X - 1, this.Y)) {
						West = new CitySquareBlock(rand, this.IdSeed, City, this.X - 1, this.Y);
						East.Generate(rand, prob - 1);
					}
				}
			}
		}

		internal class CitySquareBlock : StreetBlock {

			public override string StrShape {
				get {
					if (North != null && South != null && West != null && East != null)
						return "╬";
					if (North != null && South != null && West != null && East == null)
						return "╣";
					if (North != null && South != null && West == null && East != null)
						return "╠";
					if (North == null && South != null && West != null && East != null)
						return "╦";
					if (North != null && South == null && West != null && East != null)
						return "╩";
					if (North == null && South == null && West != null && East != null)
						return "═";
					if (North != null && South != null && West == null && East == null)
						return "║";

					if (North == null && South != null && West != null && East == null)
						return "╗";
					if (North != null && South == null && West != null && East == null)
						return "╝";
					if (North != null && South == null && West == null && East != null)
						return "╚";
					if (North == null && South != null && West == null && East != null)
						return "╔";

					return "╬";
				}
			}

			public CitySquareBlock(Random rand, Block.IdGenerator seed, Block[,] city, int x, int y) 
				: base(rand, seed, Orientation.None, city, x, y) { }

			public override void Generate(Random rand, int prob) {

				this.North = GetBlockAt(this.X, this.Y - 1);
				if (North == null && rand.Next(0, 100) < prob && IsInsideCity(this.X, this.Y - 1)) {
					North = new StreetBlock(rand, this.IdSeed, Generator.Orientation.Vertical, City, this.X, this.Y - 1);
					North.Generate(rand, prob - 1);
				}

				this.South = GetBlockAt(this.X, this.Y + 1);
				if (South == null && rand.Next(0, 100) < prob && IsInsideCity(this.X, this.Y + 1)) {
					South = new StreetBlock(rand, this.IdSeed, Generator.Orientation.Vertical, City, this.X, this.Y + 1);
					South.Generate(rand, prob - 1);
				}

				this.East = GetBlockAt(this.X + 1, this.Y);
				if (East == null && rand.Next(0, 100) < prob && IsInsideCity(this.X + 1, this.Y)) {
					East = new StreetBlock(rand, this.IdSeed, Generator.Orientation.Horizontal, City, this.X + 1, this.Y);
					East.Generate(rand, prob - 1);
				}

				this.West = GetBlockAt(this.X - 1, this.Y);
				if (West == null && rand.Next(0, 100) < prob && IsInsideCity(this.X - 1, this.Y)) {
					West = new StreetBlock(rand, this.IdSeed, Generator.Orientation.Horizontal, City, this.X - 1, this.Y);
					West.Generate(rand, prob - 1);
				}

			}
		}

		public Dictionary<uint, Zone> GetZones() {
			Dictionary<uint, Zone> zones = new Dictionary<uint, Zone>();
			for (int y = 0; y < city.GetLength(1); y++) {
				for (int x = 0; x < city.GetLength(0); x++) {
					Block block = city[x, y];
					if (block != null) {
						zones.Add(block.Id, block.Zone);
					}
				}
			}
			return zones;
		}

		Random rand;
		Block[,] city;
		Block.IdGenerator seed;

		public Generator(int size, uint cityId) {

			rand = new Random();
			city = new Block[20, 20];
			seed = new Block.IdGenerator(cityId);			

			CitySquareBlock centerBlock = new CitySquareBlock(rand, seed, city, 10, 10);
			centerBlock.Generate(rand, 75);

			centerBlock.GenerateCityBuildings(rand);

			//for (int y = 0; y < city.GetLength(1); y++) {
			//    for (int x = 0; x < city.GetLength(0); x++) {
			//        Block block = city[x, y];

			//        if (block != null)
			//            Console.Write(block.StrShape);
			//        else
			//            Console.Write(" ");

			//        //CitySquareBlock sqBlock = block as CitySquareBlock;

			//        //if (sqBlock != null) {
			//        //    Console.Write("+");
			//        //} else {
			//        //    StreetBlock sBlock = block as StreetBlock;
			//        //    if (sBlock != null) {
			//        //        Console.Write((sBlock.Orientation == Orientation.Vertical) ? "|" : "-");
			//        //    } else {
			//        //        CityBuildingBlock bBlock = block as CityBuildingBlock;
			//        //        if (bBlock != null) {
			//        //            Console.Write("#");
			//        //        } else
			//        //            Console.Write(" ");
			//        //    }
			//        //}
			//    }
			//    Console.WriteLine();
			//}

		}


	}
}
