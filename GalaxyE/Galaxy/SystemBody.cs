using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using GalaxyE.DB;
using System.IO;
using GalaxyE.Galaxy.Zones;

namespace GalaxyE.Galaxy {

	public enum SystemBodyType {
		Asteroid = 0,
		RockyPlanetoid = 1,
		RockyWorld = 2,
		Venuzian = 3,
		Inferno = 4,
		IceWorld = 5,
		WaterWorld = 6,
		Terrestrial = 7,
		SubGasGiant = 8,
		RingedGasGiant = 9,
		GasGiant = 10
	}

	public class NaturalResource {

		private static Dictionary<int, NaturalResource> resources;

		private string _name;
		private int _nrNumber;
		private Dictionary<SystemBodyType, nrConcentration> _concentration;

		public string Name { get { return _name; } }
		public int Number { get { return _nrNumber; } }
		public Dictionary<SystemBodyType, nrConcentration> Concentration { get { return _concentration; } }

		private NaturalResource(string name, int nrNumber, Dictionary<SystemBodyType, nrConcentration> concentration) {
			this._name = name;
			this._nrNumber = nrNumber;
			this._concentration = concentration;
		}

		public class nrConcentration {
			private int min;
			private int max;

			public int Minimum { get { return min; } }
			public int Maximum { get { return max; } }

			public nrConcentration(int min, int max) {
				this.min = min;
				this.max = max;
			}
		}

		static NaturalResource() {
			resources = new Dictionary<int, NaturalResource>();

			Dictionary<SystemBodyType, nrConcentration> waterConcentr 
                = new Dictionary<SystemBodyType, nrConcentration>();

			waterConcentr.Add(SystemBodyType.Asteroid, new nrConcentration(0, 15));
			waterConcentr.Add(SystemBodyType.GasGiant, new nrConcentration(0, 10));
			waterConcentr.Add(SystemBodyType.SubGasGiant, new nrConcentration(0, 10));
			waterConcentr.Add(SystemBodyType.RingedGasGiant, new nrConcentration(0, 10));
			waterConcentr.Add(SystemBodyType.IceWorld, new nrConcentration(70, 100));
			waterConcentr.Add(SystemBodyType.Inferno, new nrConcentration(0, 5));
			waterConcentr.Add(SystemBodyType.RockyPlanetoid, new nrConcentration(0, 15));
			waterConcentr.Add(SystemBodyType.RockyWorld, new nrConcentration(0, 25));
			waterConcentr.Add(SystemBodyType.Terrestrial, new nrConcentration(50, 75));
			waterConcentr.Add(SystemBodyType.Venuzian, new nrConcentration(0, 50));
			waterConcentr.Add(SystemBodyType.WaterWorld, new nrConcentration(80, 100));

			Dictionary<SystemBodyType, nrConcentration> iridiumConcentr 
                = new Dictionary<SystemBodyType, nrConcentration>();

			iridiumConcentr.Add(SystemBodyType.Asteroid, new nrConcentration(5, 60));
			iridiumConcentr.Add(SystemBodyType.GasGiant, new nrConcentration(0, 5));
			iridiumConcentr.Add(SystemBodyType.SubGasGiant, new nrConcentration(0, 2));
			iridiumConcentr.Add(SystemBodyType.RingedGasGiant, new nrConcentration(0, 2));
			iridiumConcentr.Add(SystemBodyType.IceWorld, new nrConcentration(5, 10));
			iridiumConcentr.Add(SystemBodyType.Inferno, new nrConcentration(60, 80));
			iridiumConcentr.Add(SystemBodyType.RockyPlanetoid, new nrConcentration(5, 60));
			iridiumConcentr.Add(SystemBodyType.RockyWorld, new nrConcentration(10, 60));
			iridiumConcentr.Add(SystemBodyType.Terrestrial, new nrConcentration(1, 15));
			iridiumConcentr.Add(SystemBodyType.Venuzian, new nrConcentration(0, 50));
			iridiumConcentr.Add(SystemBodyType.WaterWorld, new nrConcentration(0, 15));

			Dictionary<SystemBodyType, nrConcentration> octeriumConcentr 
                = new Dictionary<SystemBodyType, nrConcentration>();

			octeriumConcentr.Add(SystemBodyType.Asteroid, new nrConcentration(5, 60));
			octeriumConcentr.Add(SystemBodyType.GasGiant, new nrConcentration(0, 5));
			octeriumConcentr.Add(SystemBodyType.SubGasGiant, new nrConcentration(0, 2));
			octeriumConcentr.Add(SystemBodyType.RingedGasGiant, new nrConcentration(0, 2));
			octeriumConcentr.Add(SystemBodyType.IceWorld, new nrConcentration(5, 10));
			octeriumConcentr.Add(SystemBodyType.Inferno, new nrConcentration(60, 80));
			octeriumConcentr.Add(SystemBodyType.RockyPlanetoid, new nrConcentration(5, 60));
			octeriumConcentr.Add(SystemBodyType.RockyWorld, new nrConcentration(10, 60));
			octeriumConcentr.Add(SystemBodyType.Terrestrial, new nrConcentration(1, 15));
			octeriumConcentr.Add(SystemBodyType.Venuzian, new nrConcentration(0, 50));
			octeriumConcentr.Add(SystemBodyType.WaterWorld, new nrConcentration(0, 15));

			Dictionary<SystemBodyType, nrConcentration> criptiumConcentr 
                = new Dictionary<SystemBodyType, nrConcentration>();

			criptiumConcentr.Add(SystemBodyType.Asteroid, new nrConcentration(5, 60));
			criptiumConcentr.Add(SystemBodyType.GasGiant, new nrConcentration(0, 5));
			criptiumConcentr.Add(SystemBodyType.SubGasGiant, new nrConcentration(0, 2));
			criptiumConcentr.Add(SystemBodyType.RingedGasGiant, new nrConcentration(0, 2));
			criptiumConcentr.Add(SystemBodyType.IceWorld, new nrConcentration(5, 10));
			criptiumConcentr.Add(SystemBodyType.Inferno, new nrConcentration(60, 80));
			criptiumConcentr.Add(SystemBodyType.RockyPlanetoid, new nrConcentration(5, 60));
			criptiumConcentr.Add(SystemBodyType.RockyWorld, new nrConcentration(10, 60));
			criptiumConcentr.Add(SystemBodyType.Terrestrial, new nrConcentration(1, 15));
			criptiumConcentr.Add(SystemBodyType.Venuzian, new nrConcentration(0, 50));
			criptiumConcentr.Add(SystemBodyType.WaterWorld, new nrConcentration(0, 15));

			Dictionary<SystemBodyType, nrConcentration> comonOreConcentr 
                = new Dictionary<SystemBodyType, nrConcentration>();

			comonOreConcentr.Add(SystemBodyType.Asteroid, new nrConcentration(50, 100));
			comonOreConcentr.Add(SystemBodyType.GasGiant, new nrConcentration(0, 5));
			comonOreConcentr.Add(SystemBodyType.SubGasGiant, new nrConcentration(0, 2));
			comonOreConcentr.Add(SystemBodyType.RingedGasGiant, new nrConcentration(0, 2));
			comonOreConcentr.Add(SystemBodyType.IceWorld, new nrConcentration(30, 60));
			comonOreConcentr.Add(SystemBodyType.Inferno, new nrConcentration(50, 80));
			comonOreConcentr.Add(SystemBodyType.RockyPlanetoid, new nrConcentration(50, 80));
			comonOreConcentr.Add(SystemBodyType.RockyWorld, new nrConcentration(50, 80));
			comonOreConcentr.Add(SystemBodyType.Terrestrial, new nrConcentration(25, 50));
			comonOreConcentr.Add(SystemBodyType.Venuzian, new nrConcentration(25, 60));
			comonOreConcentr.Add(SystemBodyType.WaterWorld, new nrConcentration(15, 25));

			Dictionary<SystemBodyType, nrConcentration> diritiumConcentr 
                = new Dictionary<SystemBodyType, nrConcentration>();

			diritiumConcentr.Add(SystemBodyType.Asteroid, new nrConcentration(5, 60));
			diritiumConcentr.Add(SystemBodyType.GasGiant, new nrConcentration(0, 5));
			diritiumConcentr.Add(SystemBodyType.SubGasGiant, new nrConcentration(0, 2));
			diritiumConcentr.Add(SystemBodyType.RingedGasGiant, new nrConcentration(0, 2));
			diritiumConcentr.Add(SystemBodyType.IceWorld, new nrConcentration(5, 10));
			diritiumConcentr.Add(SystemBodyType.Inferno, new nrConcentration(60, 80));
			diritiumConcentr.Add(SystemBodyType.RockyPlanetoid, new nrConcentration(5, 60));
			diritiumConcentr.Add(SystemBodyType.RockyWorld, new nrConcentration(10, 60));
			diritiumConcentr.Add(SystemBodyType.Terrestrial, new nrConcentration(1, 15));
			diritiumConcentr.Add(SystemBodyType.Venuzian, new nrConcentration(0, 50));
			diritiumConcentr.Add(SystemBodyType.WaterWorld, new nrConcentration(0, 15));

			Dictionary<SystemBodyType, nrConcentration> comonCarbonsConcentr 
                = new Dictionary<SystemBodyType, nrConcentration>();

			comonCarbonsConcentr.Add(SystemBodyType.Asteroid, new nrConcentration(0, 5));
			comonCarbonsConcentr.Add(SystemBodyType.GasGiant, new nrConcentration(0, 5));
			comonCarbonsConcentr.Add(SystemBodyType.SubGasGiant, new nrConcentration(0, 2));
			comonCarbonsConcentr.Add(SystemBodyType.RingedGasGiant, new nrConcentration(0, 2));
			comonCarbonsConcentr.Add(SystemBodyType.IceWorld, new nrConcentration(30, 60));
			comonCarbonsConcentr.Add(SystemBodyType.Inferno, new nrConcentration(0, 25));
			comonCarbonsConcentr.Add(SystemBodyType.RockyPlanetoid, new nrConcentration(0, 10));
			comonCarbonsConcentr.Add(SystemBodyType.RockyWorld, new nrConcentration(0, 10));
			comonCarbonsConcentr.Add(SystemBodyType.Terrestrial, new nrConcentration(25, 80));
			comonCarbonsConcentr.Add(SystemBodyType.Venuzian, new nrConcentration(15, 30));
			comonCarbonsConcentr.Add(SystemBodyType.WaterWorld, new nrConcentration(30, 65));

			Dictionary<SystemBodyType, nrConcentration> hallumConcentr 
                = new Dictionary<SystemBodyType, nrConcentration>();

			hallumConcentr.Add(SystemBodyType.Asteroid, new nrConcentration(50, 100));
			hallumConcentr.Add(SystemBodyType.GasGiant, new nrConcentration(0, 5));
			hallumConcentr.Add(SystemBodyType.SubGasGiant, new nrConcentration(0, 2));
			hallumConcentr.Add(SystemBodyType.RingedGasGiant, new nrConcentration(0, 2));
			hallumConcentr.Add(SystemBodyType.IceWorld, new nrConcentration(30, 60));
			hallumConcentr.Add(SystemBodyType.Inferno, new nrConcentration(50, 80));
			hallumConcentr.Add(SystemBodyType.RockyPlanetoid, new nrConcentration(50, 80));
			hallumConcentr.Add(SystemBodyType.RockyWorld, new nrConcentration(50, 80));
			hallumConcentr.Add(SystemBodyType.Terrestrial, new nrConcentration(25, 50));
			hallumConcentr.Add(SystemBodyType.Venuzian, new nrConcentration(25, 60));
			hallumConcentr.Add(SystemBodyType.WaterWorld, new nrConcentration(15, 25));

			Dictionary<SystemBodyType, nrConcentration> tritiumConcentr 
                = new Dictionary<SystemBodyType, nrConcentration>();

			tritiumConcentr.Add(SystemBodyType.Asteroid, new nrConcentration(5, 60));
			tritiumConcentr.Add(SystemBodyType.GasGiant, new nrConcentration(0, 5));
			tritiumConcentr.Add(SystemBodyType.SubGasGiant, new nrConcentration(0, 2));
			tritiumConcentr.Add(SystemBodyType.RingedGasGiant, new nrConcentration(0, 2));
			tritiumConcentr.Add(SystemBodyType.IceWorld, new nrConcentration(5, 10));
			tritiumConcentr.Add(SystemBodyType.Inferno, new nrConcentration(60, 80));
			tritiumConcentr.Add(SystemBodyType.RockyPlanetoid, new nrConcentration(5, 60));
			tritiumConcentr.Add(SystemBodyType.RockyWorld, new nrConcentration(10, 60));
			tritiumConcentr.Add(SystemBodyType.Terrestrial, new nrConcentration(1, 15));
			tritiumConcentr.Add(SystemBodyType.Venuzian, new nrConcentration(0, 50));
			tritiumConcentr.Add(SystemBodyType.WaterWorld, new nrConcentration(0, 15));

			Dictionary<SystemBodyType, nrConcentration> siliciumConcentr 
                = new Dictionary<SystemBodyType, nrConcentration>();

			siliciumConcentr.Add(SystemBodyType.Asteroid, new nrConcentration(50, 100));
			siliciumConcentr.Add(SystemBodyType.GasGiant, new nrConcentration(0, 5));
			siliciumConcentr.Add(SystemBodyType.SubGasGiant, new nrConcentration(0, 2));
			siliciumConcentr.Add(SystemBodyType.RingedGasGiant, new nrConcentration(0, 2));
			siliciumConcentr.Add(SystemBodyType.IceWorld, new nrConcentration(30, 60));
			siliciumConcentr.Add(SystemBodyType.Inferno, new nrConcentration(50, 80));
			siliciumConcentr.Add(SystemBodyType.RockyPlanetoid, new nrConcentration(50, 80));
			siliciumConcentr.Add(SystemBodyType.RockyWorld, new nrConcentration(50, 80));
			siliciumConcentr.Add(SystemBodyType.Terrestrial, new nrConcentration(25, 30));
			siliciumConcentr.Add(SystemBodyType.Venuzian, new nrConcentration(25, 60));
			siliciumConcentr.Add(SystemBodyType.WaterWorld, new nrConcentration(15, 25));

			Dictionary<SystemBodyType, nrConcentration> metalsConcentr 
                = new Dictionary<SystemBodyType, nrConcentration>();

			metalsConcentr.Add(SystemBodyType.Asteroid, new nrConcentration(50, 100));
			metalsConcentr.Add(SystemBodyType.GasGiant, new nrConcentration(0, 5));
			metalsConcentr.Add(SystemBodyType.SubGasGiant, new nrConcentration(0, 2));
			metalsConcentr.Add(SystemBodyType.RingedGasGiant, new nrConcentration(0, 2));
			metalsConcentr.Add(SystemBodyType.IceWorld, new nrConcentration(30, 60));
			metalsConcentr.Add(SystemBodyType.Inferno, new nrConcentration(50, 80));
			metalsConcentr.Add(SystemBodyType.RockyPlanetoid, new nrConcentration(50, 80));
			metalsConcentr.Add(SystemBodyType.RockyWorld, new nrConcentration(50, 80));
			metalsConcentr.Add(SystemBodyType.Terrestrial, new nrConcentration(10, 25));
			metalsConcentr.Add(SystemBodyType.Venuzian, new nrConcentration(25, 60));
			metalsConcentr.Add(SystemBodyType.WaterWorld, new nrConcentration(15, 25));

			Dictionary<SystemBodyType, nrConcentration> cristalsConcentr 
                = new Dictionary<SystemBodyType, nrConcentration>();

			cristalsConcentr.Add(SystemBodyType.Asteroid, new nrConcentration(5, 80));
			cristalsConcentr.Add(SystemBodyType.GasGiant, new nrConcentration(0, 2));
			cristalsConcentr.Add(SystemBodyType.SubGasGiant, new nrConcentration(0, 2));
			cristalsConcentr.Add(SystemBodyType.RingedGasGiant, new nrConcentration(0, 2));
			cristalsConcentr.Add(SystemBodyType.IceWorld, new nrConcentration(5, 10));
			cristalsConcentr.Add(SystemBodyType.Inferno, new nrConcentration(60, 80));
			cristalsConcentr.Add(SystemBodyType.RockyPlanetoid, new nrConcentration(5, 60));
			cristalsConcentr.Add(SystemBodyType.RockyWorld, new nrConcentration(10, 60));
			cristalsConcentr.Add(SystemBodyType.Terrestrial, new nrConcentration(1, 15));
			cristalsConcentr.Add(SystemBodyType.Venuzian, new nrConcentration(0, 50));
			cristalsConcentr.Add(SystemBodyType.WaterWorld, new nrConcentration(0, 10));

			Dictionary<SystemBodyType, nrConcentration> gazesConcentr 
                = new Dictionary<SystemBodyType, nrConcentration>();

			gazesConcentr.Add(SystemBodyType.Asteroid, new nrConcentration(0, 2));
			gazesConcentr.Add(SystemBodyType.GasGiant, new nrConcentration(90, 100));
			gazesConcentr.Add(SystemBodyType.SubGasGiant, new nrConcentration(90, 100));
			gazesConcentr.Add(SystemBodyType.RingedGasGiant, new nrConcentration(90, 100));
			gazesConcentr.Add(SystemBodyType.IceWorld, new nrConcentration(5, 50));
			gazesConcentr.Add(SystemBodyType.Inferno, new nrConcentration(25, 60));
			gazesConcentr.Add(SystemBodyType.RockyPlanetoid, new nrConcentration(5, 10));
			gazesConcentr.Add(SystemBodyType.RockyWorld, new nrConcentration(5, 10));
			gazesConcentr.Add(SystemBodyType.Terrestrial, new nrConcentration(25, 35));
			gazesConcentr.Add(SystemBodyType.Venuzian, new nrConcentration(25, 40));
			gazesConcentr.Add(SystemBodyType.WaterWorld, new nrConcentration(25, 45));


			resources.Add(0, new NaturalResource("Water", 0, waterConcentr));
			resources.Add(1, new NaturalResource("Iridium", 1, iridiumConcentr));
			resources.Add(2, new NaturalResource("Octerium", 2, octeriumConcentr));
			resources.Add(3, new NaturalResource("Criptium", 3, criptiumConcentr));
			resources.Add(4, new NaturalResource("ComonOre", 4, comonOreConcentr));
			resources.Add(5, new NaturalResource("Diritium", 5, diritiumConcentr));
			resources.Add(6, new NaturalResource("Comon Carbons", 6, comonCarbonsConcentr));
			resources.Add(7, new NaturalResource("Hallum", 7, hallumConcentr));
			resources.Add(8, new NaturalResource("Tritium", 8, tritiumConcentr));
			resources.Add(9, new NaturalResource("Silicium", 9, siliciumConcentr));
			resources.Add(10, new NaturalResource("Metals", 10, metalsConcentr));
			resources.Add(11, new NaturalResource("Cristals", 11, cristalsConcentr));
			resources.Add(12, new NaturalResource("Gazes", 12, gazesConcentr));

		}

		public static NaturalResource GetNaturalResource(int nrNumber) {
			if (resources.ContainsKey(nrNumber))
				return resources[nrNumber];

			return null;
		}

		public static NaturalResource GetNaturalResource(string name) {

			foreach (NaturalResource n in resources.Values) {
				if (n._name.ToLower() == name.ToLower())
					return n;
			}

			return null;
		}

		public static int Count { get { return resources.Count; } }


	}


	[Serializable]
	public class SystemBody : SystemObject, ISerializable, IExtendableObject {

		private static float[][] PlanetBaseDistance = new float[][] {
            new float[] { 0.3F, 8F },    // Asteroid
            new float[] { 15F, 60F },    // RockyPlanetoid
            new float[] { 1.5F, 6F },    // RockyWorld
            new float[] { 0.5F, 0.9F },  // Venuzian
            new float[] { 0.01F, 0.7F }, // Inferno,
            new float[] { 1.8F, 5F },    // IceWorld,
            new float[] { 0.5F, 1.4F },  // WaterWorld,
            new float[] { 0.6F, 1.5F },  // Terrestrial,
            new float[] { 9F, 200F },    // SubGasGiant,
            new float[] { 8F, 20F },     // RingedGasGiant,
            new float[] { 4F, 80F },     // GasGiant
        };

		private static int[][] PlanetBaseTemperature = new int[][] {
            new int[] {  400,  -800 }, // Asteroid
            new int[] {  100,  -500 }, // RockyPlanetoid
            new int[] {  -50,  -400 }, // RockyWorld
            new int[] {  450,  350 },  // Venuzian
            new int[] {  350,  100 },  // Inferno,
            new int[] {  -50,  -100 }, // IceWorld,
            new int[] {  50,   15 },   // WaterWorld,
            new int[] {  50,   -5 },   // Terrestrial,
            new int[] { -350,  -250 }, // SubGasGiant,
            new int[] { -150,  -190 }, // RingedGasGiant,
            new int[] { -100,  -150 }, // GasGiant
        };

		private static float[] PlanetTypeCLevel = new float[] {
          2.000F,  // Asteroid
          1.000F,  // RockyPlanetoid
          0.250F,  // RockyWorld
          0.100F,  // Venuzian
          0.100F,  // Inferno,
          0.120F,  // IceWorld,
          0.090F,  // WaterWorld,
          0.050F,  // Terrestrial,
          0.004F,  // SubGasGiant,
          0.003F,  // RingedGasGiant,
          0.001F,  // GasGiant
        };

		private static float[] PlanetTypeMass = new float[] {
          0.00001F, // Asteroid
          0.005F,   // RockyPlanetoid
          0.1F,     // RockyWorld
          0.8F,     // Venuzian
          0.4F,     // Inferno,
          0.6F,     // IceWorld,
          1.1F,     // WaterWorld,
          1,        // Terrestrial,
          25F,      // SubGasGiant,
          100F,     // RingedGasGiant,
          300F,     // GasGiant
        };


		private int _systemBodyId;
		private int _temperature;
		private float _distance;
		private float _orbitalPeriod;
		private int _radius;
		private int _angle;
		//private string _name;
		private SystemBodyType _systemBodyType;
		private Dictionary<NaturalResource, int> _nrConcentration;

		public SystemBodyType BodyType { get { return _systemBodyType; } }
		public int SystemBodyId { get { return _systemBodyId; } }
		public Dictionary<NaturalResource, int> Concentration { get { return _nrConcentration; } }
		public float OrbitalRadius { get { return _distance; } }
		public int BaseAngle { get { return _angle; } }
		public float OrbitalPeriod { get { return _orbitalPeriod; } }
		public int Temperature { get { return _temperature; } }

		[Flags]
		public enum PlanetSectorType {
			None = 0,
			Gaz = 1,
			Rock = 2, 
			Land = 4, 
			Lava = 8,
 			Water = 16,
			Ice = 32,
			Unclassified = 64, 
			StarPort = 1024,
			Restricted = 2048,
		}

		private PlanetSectorType[,] planetSector;

		public PlanetSectorType[,] PlanetSector {
			get {
				if (planetSector == null) {

					int w = 0, h = 0, p = -1;
					switch (BodyType) {
						case SystemBodyType.Asteroid: w = 3; h = 3; break;
						case SystemBodyType.GasGiant: w = 10; h = 10; break;
						case SystemBodyType.IceWorld: w = 10; h = 12; p = 5; break;
						case SystemBodyType.Inferno: w = 8; h = 8; p = 20; break;
						case SystemBodyType.RingedGasGiant: w = 10; h = 10; break;
						case SystemBodyType.RockyPlanetoid: w = 5; h = 6; break;
						case SystemBodyType.RockyWorld: w = 8; h = 10; break;
						case SystemBodyType.SubGasGiant: w = 10; h = 10; break;
						case SystemBodyType.Terrestrial: w = 10; h = 12; p = 50; break;
						case SystemBodyType.Venuzian: w = 10; h = 10; break;
						case SystemBodyType.WaterWorld: w = 10; h = 12; p = 8; break;
					}

					planetSector = new PlanetSectorType[w, h];

					switch (BodyType) {
						case SystemBodyType.Asteroid: FillPlanetAllSectors(PlanetSectorType.Rock); break;
						case SystemBodyType.GasGiant: FillPlanetAllSectors(PlanetSectorType.Gaz); break;
						case SystemBodyType.IceWorld: 
							FillPlanetAllSectors(PlanetSectorType.Ice);
							FillPlanetSectors(PlanetSectorType.Water, p);
							break;
						case SystemBodyType.Inferno: 
							FillPlanetAllSectors(PlanetSectorType.Lava);
							FillPlanetSectors(PlanetSectorType.Rock, p);
							break;
						case SystemBodyType.SubGasGiant:
						case SystemBodyType.RingedGasGiant: FillPlanetAllSectors(PlanetSectorType.Gaz); break;
						case SystemBodyType.RockyPlanetoid:
						case SystemBodyType.RockyWorld: 
							FillPlanetAllSectors(PlanetSectorType.Rock);
							if (this.Temperature > 0) {
								FillPlanetSectors(PlanetSectorType.Lava, 4);
							} else
								FillPlanetSectors(PlanetSectorType.Ice, 8);
							break;
						
						case SystemBodyType.Terrestrial: 
							FillPlanetAllSectors(PlanetSectorType.Land);
							FillPlanetSectors(PlanetSectorType.Water, p);
							FillPlanetSectors(PlanetSectorType.Ice, 10);
						break;
						case SystemBodyType.Venuzian: FillPlanetAllSectors(PlanetSectorType.Unclassified); break;
						case SystemBodyType.WaterWorld: 
							FillPlanetAllSectors(PlanetSectorType.Water);
							FillPlanetSectors(PlanetSectorType.Land, p);
							break;
					}

					if (ObjectExtender.Instance.HasExtention(this, "sectors.txt")) {
						string data = ObjectExtender.Instance.GetExtentionData(this, "sectors.txt");
						StringReader sr = new StringReader(data);
						string line; int y = 0;
						while ((line = sr.ReadLine()) != null) {
							for (int x = 0; x < line.Length; x++) {
								if (x < planetSector.GetLength(0) && y < planetSector.GetLength(1)) {
									switch (line[x]) {
										case 'W': planetSector[x, y] = PlanetSectorType.Water; break;
										case 'L': planetSector[x, y] = PlanetSectorType.Land; break;
										case 'I': planetSector[x, y] = PlanetSectorType.Ice; break;
										case 'G': planetSector[x, y] = PlanetSectorType.Gaz; break;
										case 'V': planetSector[x, y] = PlanetSectorType.Unclassified; break;
										case 'M': planetSector[x, y] = PlanetSectorType.Lava; break;
										case 'R': planetSector[x, y] = PlanetSectorType.Rock; break;
									}
								}
							}
							y++;
						}
					}

				}

				return planetSector;
			}
		}

		private void FillPlanetSectors(PlanetSectorType type, int percentage) {
			Random rand = new Random((int)(this.SystemId.Id >> 4 & 0xFFFFFF | (ulong)(this.SystemBodyId & 0xFF) << 8));
			if (planetSector != null) {
				for (int x = 0; x < planetSector.GetLength(0); x++) {
					for (int y = 0; y < planetSector.GetLength(1); y++) {
						bool fill = (rand.Next(0, 100) <= percentage);
						if (fill) {
							planetSector[x, y] = (PlanetSectorType)((~1023) & (int)planetSector[x, y]);
							planetSector[x, y] |= type;
						}
					}
				}
			}
		}

		private void FillPlanetAllSectors(PlanetSectorType type) {
			if (planetSector != null) {
				for (int x = 0; x < planetSector.GetLength(0); x++) {
					for (int y = 0; y < planetSector.GetLength(1); y++) {
						planetSector[x, y] |= type;
					}
				}
			}
		}

		private int _numOfZones = -1;
		private Dictionary<ulong, Zone> _planetZones;
		public Zone GetZone(ulong zoneId) {

			if (_planetZones == null) {
				_planetZones = new Dictionary<ulong, Zone>();
				if (_numOfZones == -1)
					_numOfZones = Zone.CalculateNumZones(this);

				for (int i = 0; i < _numOfZones; i++) {
					Zone mainZone = new Zone(this, (uint)i & 0xF);
					_planetZones.Add(mainZone.ZoneId, mainZone);
				}
			}

			if (_planetZones.ContainsKey(zoneId))
				return _planetZones[zoneId];
			else {

				//Lets search inside the main zone

				//foreach (var zone in _planetZones.Values) {
				//    if (false)
				//        return zone;
				//}

				return null;

			}
		}

		//private Zone[] _zones = null;
		//public Zone[] Zones {
		//    get {
		//        if (_zones == null) {

		//            if (_numOfZones == 0)
		//                _numOfZones = Zone.CalculateNumZones(this);

		//            _zones = new Zone[_numOfZones];
		//            for (int i = 0; i < _zones.Length; i++)
		//                _zones[i] = new Zone(this, (uint)i);
		//        }
		//        return _zones;
		//    }
		//}

		private string _description = null;
		public string Description {
			get {

				if (_description == null) {
					StringBuilder sbuilder = new StringBuilder();
					if (ObjectExtender.Instance.HasExtention(this, "desc.txt")) {
						sbuilder.AppendLine(ObjectExtender.Instance.GetExtentionData(this, "desc.txt"));
					} else {
						//Need a desciption of each object type
					}

					_description = sbuilder.ToString();
				}

				return _description;
			}
		}

		private List<SpacePort> spacePorts = null;
		public List<SpacePort> SpacePort {
			get {
				if (spacePorts == null) {
					spacePorts = new List<SpacePort>();
					if (ObjectExtender.Instance.HasExtention(this, "ports.txt")) {
						StringReader sr = new StringReader(ObjectExtender.Instance.GetExtentionData(this, "ports.txt"));
						string line;
						while ((line = sr.ReadLine()) != null) {
							string[] tokensA = line.Split(':');
							if (tokensA.Length == 2) {
								string[] tokensB = tokensA[1].Split(';');
								if (tokensB.Length >= 3) {
									string Id = tokensA[0].Trim();
									string name = tokensB[0].Trim();
									string sector = tokensB[1].Trim();
									string strType = tokensB[2].Trim();

									if (Id.Length > 0 && name.Length > 0) {
										SpacePortType type = SpacePortType.SpacePort;
										if (strType.Length > 0) {
											switch (strType[0]) {
												case 'P': type = SpacePortType.SpacePort; break;
												case 'S': type = SpacePortType.OrbitalStation; break;
												case 'M': type = SpacePortType.MoonSpacePort; break;
												default: type = SpacePortType.SpacePort; break;
											}
										}

										SpacePort port = new SpacePort(this, Id, name, sector, "", type);
										spacePorts.Add(port);
									}
								}
							}
						}
					} else {
						spacePorts.AddRange(GalaxyE.Galaxy.Zones.SpacePort.GeneratePorts(this));
					}
				}
				return spacePorts;
			}
		}

		public string PlanetName {
			get {
				return this.Name;
			}
		}

		public override string Name {
			get {
				if (this.ExtendedProperties.ContainsKey("name")) {
					return this.ExtendedProperties.GetValue<string>("name");
				} else
					return base.Name;
			}
			protected set {
				base.Name = value;
			}
		}

		public string GetClass() {
			switch (this.BodyType) {
				case SystemBodyType.Asteroid: return "As";
				case SystemBodyType.GasGiant: return "Gs";
				case SystemBodyType.IceWorld: return "W2";
				case SystemBodyType.Inferno: return "I";
				case SystemBodyType.RingedGasGiant: return "Gs2";
				case SystemBodyType.RockyPlanetoid: return "R2";
				case SystemBodyType.RockyWorld: return "R";
				case SystemBodyType.SubGasGiant: return "Gs3";
				case SystemBodyType.Terrestrial: return "T";
				case SystemBodyType.Venuzian: return "V";
				case SystemBodyType.WaterWorld: return "W";
				default: return "NA";
			}
		}

		public string GetClass_Ext() {
			switch (this.BodyType) {
				case SystemBodyType.Asteroid: return "Asteroid";
				case SystemBodyType.GasGiant: return "Gas Giant";
				case SystemBodyType.IceWorld: return "Ice world";
				case SystemBodyType.Inferno: return "Inferno (Mercury like)";
				case SystemBodyType.RingedGasGiant: return "Gas Giant (Saturn like)";
				case SystemBodyType.RockyPlanetoid: return "Rocky Planetoid";
				case SystemBodyType.RockyWorld: return "Rocky World";
				case SystemBodyType.SubGasGiant: return "Sub Gas Giant";
				case SystemBodyType.Terrestrial: return "Terrestrial";
				case SystemBodyType.Venuzian: return "Venuzian";
				case SystemBodyType.WaterWorld: return "Water World";
				default: return "Unknown";
			}
		}

		public SystemBody(StarSystem starSystem, int systemBodyId)
			: base(starSystem) {


			this._systemBodyId = systemBodyId;
			this.Name = string.Format("{0} {1}", starSystem.Name, (char)((int)'b' + systemBodyId));
			GenerateBody();

			if (!starSystem.Generated && starSystem.Name == "Sol") {
				GenerateSolBodies(systemBodyId);
			}
		}

		public void GenerateSolBodies(int systemBodyId) {

			switch (systemBodyId) {
				case 0:
					this.Name = "Mercury";
					this._systemBodyType = SystemBodyType.Inferno;
					this._numOfZones = 0;
					this._distance = 0.41F;
					this._orbitalPeriod = 0.24F;
					this._temperature = 396;
					break;
				case 1:
					this.Name = "Venus";
					this._systemBodyType = SystemBodyType.Venuzian;
					this._numOfZones = 2;
					this._distance = 0.72F;
					this._orbitalPeriod = 0.65F;
					this._temperature = 480;
					break;
				case 2:
					this.Name = "Earth";
					this._systemBodyType = SystemBodyType.Terrestrial;
					this._numOfZones = 4;
					this._distance = 1F;
					this._orbitalPeriod = 1F;
					this._temperature = 22;
					break;
				case 3:
					this.Name = "Mars";
					this._systemBodyType = SystemBodyType.RockyWorld;
					this._numOfZones = 3;
					this._distance = 1.45F;
					this._orbitalPeriod = 1.88F;
					this._temperature = -25;
					break;
				case 4:
					this.Name = "Jupiter";
					this._systemBodyType = SystemBodyType.GasGiant;
					this._distance = 5.42F;
					this._orbitalPeriod = 11.8F;
					this._temperature = -161;
					break;
				case 5:
					this.Name = "Saturn";
					this._systemBodyType = SystemBodyType.RingedGasGiant;
					this._distance = 10.11F;
					this._orbitalPeriod = 29.45F;
					this._temperature = -190;
					break;
				case 6:
					this.Name = "Uranus";
					this._systemBodyType = SystemBodyType.SubGasGiant;
					this._distance = 20.08F;
					this._orbitalPeriod = 84.32F;
					this._temperature = -224;
					break;
				case 7:
					this.Name = "Neptune";
					this._systemBodyType = SystemBodyType.SubGasGiant;
					this._distance = 30.44F;
					this._orbitalPeriod = 164.79F;
					this._temperature = -218;
					break;
				case 8:
					this.Name = "Pluto";
					this._systemBodyType = SystemBodyType.RockyPlanetoid;
					this._distance = 33.45F;
					this._orbitalPeriod = 248.09F;
					this._temperature = -260;
					break;
			}

			SystemLocation.UpdateLocation(this);

		}

		public void GenerateBody() {

			ulong param1 = starSystem.SystemId << 16 | starSystem.SystemId >> 12 + (_systemBodyId & 0xFF);
			ulong param2 = ((ulong)_systemBodyId << 16 | starSystem.SystemId >> 12) + (starSystem.SystemId & 0xFF);

			Utils.Rotate_SystemParams(ref param1, ref param2);

			Random rand = new Random(
				(int)(param1 + param2)
			);

			float hz = this.StarSystem.HZone;
			float minDist = hz * 0.2F;
			float maxDist = hz * 60.0F;

			//No planets more then 5000Au away
			if (maxDist > 5000) maxDist = 5000;
			if (minDist > 600) minDist = 600;

			int totalBodies = this.StarSystem.NumberOfBodies;

			double pNum = (_systemBodyId + 1) * 0.9;
			double pMin = ((Math.Pow(2, pNum - 1))*(maxDist - minDist)/((Math.Pow(2,totalBodies-1))))+minDist;
			double pMax = ((Math.Pow(2, pNum))*(maxDist - minDist)/((Math.Pow(2,totalBodies-1))))+minDist;

			double spreadHint = pMax - pMin;
			double baseDistance = rand.NextDouble() * (spreadHint / 2) + pMin;

			List<SystemBodyType> eTypes = new List<SystemBodyType>();

			for (int i = 0; i < PlanetBaseDistance.Length; i++) {
				double mind = PlanetBaseDistance[i][0] * hz;
				double maxd = PlanetBaseDistance[i][1] * hz;
				if (baseDistance >= mind && baseDistance <= maxd) {
					eTypes.Add((SystemBodyType)i);
				}
			}

			if (eTypes.Count == 0) {
				eTypes.Add(SystemBodyType.RockyPlanetoid);
				eTypes.Add(SystemBodyType.Asteroid);
			}

			_systemBodyType = eTypes[rand.Next(0, eTypes.Count -1)];

			if ((int)_systemBodyType > 11)
				_systemBodyType = SystemBodyType.Asteroid;

			int baseTempA = PlanetBaseTemperature[(int)_systemBodyType][0];
			int baseTempB = PlanetBaseTemperature[(int)_systemBodyType][1];
			int baseTempR = baseTempA - baseTempB;
			double baseDistA = (double)(PlanetBaseDistance[(int)_systemBodyType][0] * hz);
			double baseDistB = (double)(PlanetBaseDistance[(int)_systemBodyType][1] * hz);
			double baseDistR = baseDistB - baseDistA;

			_temperature = (int)(
				baseTempA - (
					(Math.Pow(baseDistance, 0.2) - Math.Pow(baseDistA, 0.2)) 
					/ Math.Pow(baseDistR, 0.2) * baseTempR
				)
			);

			_distance = (float)baseDistance;
			_orbitalPeriod = (float)Math.Round(Math.Pow(_distance, 1.5) * 1000, 5) / 1000;
			_radius = rand.Next(20, 1000) / 100;
			_angle = rand.Next(0, 360);

			//double rad = 0.0174532925;

			//double ssX = (long)(_distance * Math.Cos(_angle * rad) * 149598000);
			//double ssY = (long)(_distance * Math.Sin(_angle * rad) * 149598000);
			//double ssZ = 0;
			//this.SystemLocation.SetInSystemCoords(ssX, ssY, ssZ);

			SystemLocation.UpdateLocation(this);

			this._nrConcentration = new Dictionary<NaturalResource, int>();

			for (int i = 0; i < NaturalResource.Count; i++) {
				NaturalResource r = NaturalResource.GetNaturalResource(i);
				int c = rand.Next(
					r.Concentration[_systemBodyType].Minimum,
					r.Concentration[_systemBodyType].Maximum);
				this._nrConcentration.Add(r, c);
			}



		}

		#region ISerializable Members

		public override void GetObjectData(SerializationInfo info, StreamingContext context) {
			base.GetObjectData(info, context);
			info.AddValue("systemBodyId", this._systemBodyId);
		}

		public SystemBody(SerializationInfo info, StreamingContext context)
			: base(info, context) {
			this._systemBodyId = info.GetInt32("systemBodyId");
			this.Name = string.Format("{0} {1}", starSystem.Name, _systemBodyId);
			GenerateBody();
		}

		#endregion

		#region Equals
		public static bool operator ==(SystemBody sObj1, SystemBody sObj2) {

			if (Object.ReferenceEquals(sObj1, null) || Object.ReferenceEquals(sObj2, null))
				return Object.ReferenceEquals(sObj1, sObj2);
			else
				return sObj1.Equals(sObj2);

		}

		public static bool operator !=(SystemBody sObj1, SystemBody sObj2) {

			if (Object.ReferenceEquals(sObj1, null) || Object.ReferenceEquals(sObj2, null))
				return !Object.ReferenceEquals(sObj1, sObj2);
			else
				return !sObj1.Equals(sObj2);
		}

		public override bool Equals(object obj) {
			SystemBody sObj = obj as SystemBody;
			if (!Object.ReferenceEquals(sObj, null)) {
				return this._systemBodyId == sObj._systemBodyId
					&& this.starSystem.SystemId == sObj.starSystem.SystemId;
			}
			return false;
		}

		public override int GetHashCode() {
			return (int)this.starSystem.SystemId;
		}
		#endregion

		#region SectorObject Members

		//public override Report MakeReport(int turn) {
		//    Report report = new Report(this._name, -1, ReportType.SystemBody, Guid.Empty, turn, this.Coords, true);
		//    report["SystemBody"] = this;
		//    report["Desc"] = "System body report";
		//    return report;
		//}

		#endregion


		#region IExtendableObject Members

		private Properties _extendedProperties;
		public Properties ExtendedProperties {
			get { 
				if (_extendedProperties == null) {
					_extendedProperties = new Properties();
					ObjectExtender.Instance.Extend(this);
				}
				return _extendedProperties;
			}
		}

		#endregion
	}
}
