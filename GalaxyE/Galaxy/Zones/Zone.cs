using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GalaxyE.Galaxy {

	public enum ZoneType {
		None,
		OrbitalStation,
		OrbitalCity,
		OrbitalFort,
		City,
		Street,
		StarPort,
		MonorailStation,
		MonorailTrain,
		Hospital,
		Hotel,
		HousesBlock,
		OfficeBlock,
		Bar,
		Shop,
		Room
	}
	

	public class Zone {

		public const ulong CityMask = 0x0000000000FF;
		public const ulong BuildingMask = 0x00000FFFFF00;
		public const ulong RoomMask = 0xFFFFF0000000;


		public bool IsCity { get { return (ZoneId & ~CityMask) == 0; } }
		public bool IsBuilding { get { return ((ZoneId & ~(BuildingMask | CityMask)) == 0) && !IsCity; } }
		public bool IsRoom { get { return (ZoneId & RoomMask) != 0; } }

		public uint CityId { get { return (uint)(ZoneId & CityMask); } }
		public uint BuildingId { get { return (uint)(ZoneId & (BuildingMask | CityMask)); } }
		public uint RoomId { get { return (uint)(ZoneId); } }

		public int CityIndex { get { return (int)(ZoneId & CityMask); } }
		public int BuildingIndex { get { return (int)((ZoneId & BuildingMask) >> 8); } }
		public int RoomIndex { get { return (int)((ZoneId & RoomMask) >> 20); } }

		protected Generator.Block _block { get; set; } 

		//pSector:zType:ID
		public ulong ZoneId { get; private set; }
		public string Description { get; private set; }
		public SystemBody SystemBody { get; private set; }


		public Zone Parent { get; private set; }
		public Dictionary<int, Zone> Childs { get; private set; }
		public ZoneType ZoneType { get; private set; }


		public Zone(SystemBody systemBody, ulong zoneId, Generator.Block block)
			: this(systemBody, zoneId, false) {

				this._block = block;

		}

		public Zone(SystemBody systemBody, ulong zoneId) 
			: this (systemBody, zoneId, true) {
		}

		private Zone(SystemBody systemBody, ulong zoneId, bool generate) {
			this.SystemBody = systemBody;
			this.ZoneId = zoneId;
			if (generate)
				GenerateZone();
		}

		private Zone GenerateZone() {

			Random rand = new Random((int)(SystemBody.SystemId.Id ^ ZoneId));
			//this.ZoneType = (ZoneType)rand.Next(1, 4);
			
			if (this.IsCity)
				GenerateCity();

			if (this.IsBuilding) {
				Zone city = new Zone(SystemBody, this.CityId).GenerateZone();
				this.Parent = city;
				Zone generatedZone = city.Childs[this.BuildingIndex].GenerateZone();
			} else {
				Zone city = new Zone(SystemBody, this.CityId).GenerateZone();
				Zone building = city.Childs[this.BuildingIndex].GenerateZone();
				this.Parent = building;
				Zone generatedZone = building.Childs[this.RoomIndex].GenerateZone();
			}

			return this;
		}

		public void GenerateCity() {

		}

		public void GenerateBuilding() {

		}

		public static int CalculateNumZones(SystemBody body) {

			float hz = body.StarSystem.HZone;
			float baseDistance = body.OrbitalRadius / hz;

			if (baseDistance > 0.5 && baseDistance < 10) {

				int min = 0;
				int max = 5;

				switch (body.BodyType) {
					case SystemBodyType.Asteroid: min = 0; max = 1; break;
					case SystemBodyType.GasGiant: return 0;
					case SystemBodyType.IceWorld: min = 0; max = 4; break;
					case SystemBodyType.Inferno: min = 0; max = 1; break;
					case SystemBodyType.RingedGasGiant: return 0;
					case SystemBodyType.RockyPlanetoid: min = 0; max = 1; break;
					case SystemBodyType.RockyWorld: min = 0; max = 3; break;
					case SystemBodyType.SubGasGiant: return 0;
					case SystemBodyType.Terrestrial: min = 2; max = 8; break;
					case SystemBodyType.Venuzian: return 0;
					case SystemBodyType.WaterWorld: min = 1; max = 4; break;
				}

				Random rand = new Random((int)body.SystemId.Id >> 4);
				int num = rand.Next(min, max);
				return num;

			} else
				return 0;

		}
	}
}
