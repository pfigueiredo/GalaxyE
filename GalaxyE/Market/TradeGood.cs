using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaxyE.Galaxy;

namespace GalaxyE.Market {

	public class TradeGood {

		public const int Food = 1;
		public const int Water = 2;
		public const int Textiles = 3;
		public const int Radioactives = 4;
		public const int Luxuries = 5;
		public const int RareSpecies = 6;
		public const int Narcotics = 7;
		public const int Microships = 8;
		public const int Computers = 9;
		public const int Drones = 10;
		public const int Machinery = 11;
		public const int Alloys = 12;
		public const int ComonOre = 13;
		public const int RareMetals = 14;
		public const int Arms = 15;
		public const int Minerals = 16;
		public const int RareMinerals = 17;
		public const int GemStrones = 18;
		public const int Crystals = 19;
		public const int Fuel = 20;
		public const int AlienItems = 21;
		public const int QuantumMaterials = 22;

		public int TradeGoodId { get; private set; }
		public int BasePrice { get; private set; }
		public int BaseQuantity { get; private set; }
		public int MinTechLevel { get; private set; }
		public List<SystemBodyType> MainOrigin { get; private set; }

		public string Name {
			get {
				return _names[TradeGoodId - 1];
			}
		}

		private TradeGood (
			int tradeGoodId, int basePrice, int baseQuantity,
			int minTechLevel, params SystemBodyType[] mainOrigin) {

			this.TradeGoodId = tradeGoodId;
			this.BasePrice = basePrice;
			this.BaseQuantity = baseQuantity;
			this.MinTechLevel = minTechLevel;
			this.MainOrigin = new List<SystemBodyType>(mainOrigin);

		}

		public override string ToString() {
			return this.Name;
		}

		private static object _baseGoodsLocker = new object();
		private static List<TradeGood> _baseGoods = null;
		public static List<TradeGood> Goods {
			get {
				if (_baseGoods == null) {
					lock (_baseGoodsLocker) {
						if (_baseGoods == null) {
							CreateBaseGoods();
						}
					}
				}
				return _baseGoods;
			}
		}

		public static TradeGood GetTradeGoodByName(string name) {
			for (int i = 0; i < _names.Length; i++) {
				if (_names[i].Equals(name, StringComparison.InvariantCultureIgnoreCase)) {
					return TradeGood.Goods[i];
				}
			}
			return null;
		}

		private static void CreateBaseGoods() {

			List<TradeGood> goods = new List<TradeGood>();

			//Food
			goods.Add(new TradeGood(Food, 100, 5000, 0, SystemBodyType.Terrestrial, SystemBodyType.WaterWorld, SystemBodyType.IceWorld));
			//Water
			goods.Add(new TradeGood(Water, 50, 7000, 0, SystemBodyType.Terrestrial, SystemBodyType.WaterWorld, SystemBodyType.IceWorld));
			//Textiles
			goods.Add(new TradeGood(Textiles, 150, 500, 0, SystemBodyType.Terrestrial, SystemBodyType.WaterWorld, SystemBodyType.IceWorld));
			//Radioactives
			goods.Add(new TradeGood(Radioactives, 200, 10, 5));
			//Luxuries
			goods.Add(new TradeGood(Luxuries, 300000, 10, 4));
			//Rare Species
			goods.Add(new TradeGood(RareSpecies, 150000, 2, 4, SystemBodyType.Terrestrial, SystemBodyType.WaterWorld, SystemBodyType.IceWorld, SystemBodyType.IceWorld, SystemBodyType.Inferno, SystemBodyType.Venuzian));
			//Narcotics
			goods.Add(new TradeGood(Narcotics, 150, 6, 1));
			//Microships
			goods.Add(new TradeGood(Microships, 5000, 250, 6));
			//Computers
			goods.Add(new TradeGood(Computers, 10000, 100, 6));
			//Drones
			goods.Add(new TradeGood(Drones, 11000, 80, 6));
			//Machinery
			goods.Add(new TradeGood(Machinery, 10000, 100, 5));
			//Alloys
			goods.Add(new TradeGood(Alloys, 1500, 800, 3));
			//Comon Ore
			goods.Add(new TradeGood(ComonOre, 500, 3000, 1, SystemBodyType.Asteroid, SystemBodyType.RockyPlanetoid, SystemBodyType.RockyWorld));
			//Rare Metals
			goods.Add(new TradeGood(RareMetals, 300000, 100, 4));
			//Arms
			goods.Add(new TradeGood(Arms, 20000, 80, 4));
			//Minerals
			goods.Add(new TradeGood(Minerals, 3500, 2000, 1, SystemBodyType.Asteroid, SystemBodyType.RockyPlanetoid, SystemBodyType.RockyWorld));
			//Rare Minerals
			goods.Add(new TradeGood(RareMinerals, 250000, 50, 2, SystemBodyType.RockyPlanetoid, SystemBodyType.Venuzian));
			//Gem-Strones
			goods.Add(new TradeGood(GemStrones, 900000, 10, 2, SystemBodyType.Asteroid, SystemBodyType.RockyPlanetoid, SystemBodyType.RockyWorld));
			//Crystals
			goods.Add(new TradeGood(Crystals, 60000, 120, 2, SystemBodyType.Asteroid, SystemBodyType.RockyPlanetoid, SystemBodyType.RockyWorld));
			//Fuel
			goods.Add(new TradeGood(Fuel, 1000, 2000, 3));
			//Alien Items
			goods.Add(new TradeGood(AlienItems, 1000000, 1, 10));
			//Quantum Materials
			goods.Add(new TradeGood(QuantumMaterials, 10000, 250, 7));

			_baseGoods = goods;

		}


		private static string[] _names = new string[] {
			"Food","Water","Textiles","Radioactives","Luxuries","Rare Species",
			"Narcotics","Microships","Computers","Drones","Machinery","Alloys",
			"Comon Ore","Rare Metals","Arms","Minerals","Rare Minerals",
			"Gem-Strones","Crystals","Fuel","Alien Items","Quantum Materials"
		};

	}
}

/*                   {
                    {0x13,-0x02,0x06,0x01,0,"Food        "},
                    {0x14,-0x01,0x0A,0x03,0,"Textiles    "},
                    {0x41,-0x03,0x02,0x07,0,"Radioactives"},
#if POLITICALLY_CORRECT
                    {0x28,-0x05,0xE2,0x1F,0,"Robot Slaves"},
                    {0x53,-0x05,0xFB,0x0F,0,"Beverages   "},
#else
                    {0x28,-0x05,0xE2,0x1F,0,"Slaves      "},
                    {0x53,-0x05,0xFB,0x0F,0,"Liquor/Wines"},
#endif 
                    {0xC4,+0x08,0x36,0x03,0,"Luxuries    "},
#if POLITICALLY_CORRECT
                    {0xEB,+0x1D,0x08,0x78,0,"Rare Species"},
#else
                    {0xEB,+0x1D,0x08,0x78,0,"Narcotics   "},
#endif 
                    {0x9A,+0x0E,0x38,0x03,0,"Computers   "},
                    {0x75,+0x06,0x28,0x07,0,"Machinery   "},
                    {0x4E,+0x01,0x11,0x1F,0,"Alloys      "},
                    {0x7C,+0x0d,0x1D,0x07,0,"Firearms    "},
                    {0xB0,-0x09,0xDC,0x3F,0,"Furs        "},
                    {0x20,-0x01,0x35,0x03,0,"Minerals    "},
                    {0x61,-0x01,0x42,0x07,1,"Gold        "},
                    {0xAB,-0x02,0x37,0x1F,1,"Platinum    "},
                    {0x2D,-0x01,0xFA,0x0F,2,"Gem-Strones "},
                    {0x35,+0x0F,0xC0,0x07,0,"Alien Items "},
                   };*/