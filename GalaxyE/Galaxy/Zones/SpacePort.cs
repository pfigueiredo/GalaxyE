using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GalaxyE.Galaxy.Zones {

	public enum SpacePortType {
		SpacePort,
		OrbitalStation,
		MoonSpacePort
	}

	public class SpacePort {

		public string Name { get; set; }
		public SystemBody Body { get; set; }
		public string Id { get; set; }
		public string PlanetSector { get; set; }
		public string Modifiers { get; set; }
		public SpacePortType Type { get; set; }

		public SpacePort(SystemBody body, string Id, string name, string sector, string Modifiers, SpacePortType type) {
			this.Body = body;
			this.Id = Id;
			this.Name = name;
			this.PlanetSector = sector;
			this.Modifiers = Modifiers;
			this.Type = type;
		}


		public static List<SpacePort> GeneratePorts(SystemBody body) {

			List<SpacePort> ports = new List<SpacePort>();

			var distance = body.StarSystem.Coords.CalculateDistance((new Sector()).Coords);
			var prob = 2 - (distance * 2 / 150);
			if (prob < 0) prob = 0;

			int minP = 0, maxP = 0;
			switch (body.BodyType) {
				case SystemBodyType.Asteroid:		minP = 0; maxP = 1; break;
				case SystemBodyType.GasGiant:		minP = 0; maxP = 0; break;
				case SystemBodyType.IceWorld:		minP = 1; maxP = 3; break;
				case SystemBodyType.Inferno:		minP = 0; maxP = 2; break;
				case SystemBodyType.RingedGasGiant: minP = 0; maxP = 0; break;
				case SystemBodyType.RockyPlanetoid: minP = 0; maxP = 1; break;
				case SystemBodyType.RockyWorld:		minP = 1; maxP = 2; break;
				case SystemBodyType.SubGasGiant:	minP = 0; maxP = 0; break;
				case SystemBodyType.Terrestrial:	minP = 1; maxP = 3; break;
				case SystemBodyType.Venuzian:		minP = 0; maxP = 0; break;
				case SystemBodyType.WaterWorld:		minP = 1; maxP = 3; break;
			}

			minP = (int)(minP * prob);
			maxP = (int)(maxP * prob);

			Random rand = new Random(
				(int)(body.SystemBodyId ^ body.SystemId.SectorIndexX ^ body.SystemId.SectorIndexY));

			int numOfPorts = rand.Next(minP, maxP);

			for (int i = 0; i < numOfPorts; i++) {

				int a = body.PlanetSector.GetLength(0);
				int b = body.PlanetSector.GetLength(1);
				int s1 = rand.Next(0, a);
				int s2 = rand.Next(0, b);

				string Id = string.Format("P{0}", i);
				string sector = string.Format("{0}{1}", (char)((int)'A' + s1), s2);
				string name = string.Format("{0} P{1}", body.Name, i);

				SpacePort p = new SpacePort(body, Id, name, sector, "", SpacePortType.SpacePort);

				ports.Add(p);
			}

			return ports;

		}


	}
}
