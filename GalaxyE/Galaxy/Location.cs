using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaxyE.Galaxy.Zones;
using GalaxyE.Players;

namespace GalaxyE.Galaxy {

	public class GameLocation {

		public Sector Sector { get; private set;  }
		public StarSystem StarSystem { get; private set; }
		public SystemBody SystemBody { get; private set; }
		public string PlanetSector { get; private set; }
		public string Zone { get; private set; }

		public void SetPlanetSector(int x, int y) {
			string cod = string.Format("{0}{1}", (char)((int)'A' + y), x);
			PlanetSector = cod;
			Zone = "#";
		}

		public void SetPlanetSector(SpacePort port) {
			string cod = string.Format("p{0}", port.Id);
			PlanetSector = cod;
			Zone = "#";
		}

		public SpacePort GetSpacePort(Player player) {

			if (SystemBody != null) {
                string Id = player.SpaceShip.SpaceDockId;
                foreach (var port in SystemBody.SpacePort) {
                    if (port.Id == Id) {
                        return port;
                    }
                }
			}
				
			return null;
			
		}

		public GameLocation(Coords coords) {

			this.Sector = null;
			this.StarSystem = null;
			this.SystemBody = null;

			this.Sector = coords.GetSector();

			foreach (var system in this.Sector.Systems) {
				if (coords.IsInRange(system.Coords, 0.5)) {
					this.StarSystem = system;
					break;
				}
			}

		}

		public void UpdateSystemBody(SystemLocation location, bool inSpace) {
			if (this.StarSystem != null) {

				if (!inSpace) {
					foreach (var body in this.StarSystem.SystemBodies) {

						if (location.GetInSystemDistance(
							SystemLocation.GetUpdatedLocation(body)
						) <= (384403)) {

							this.SystemBody = body;
							break;

						}
					}
				} else {
					this.SystemBody = null;
				}
			}
		}

		public GameLocation(string locationCode, bool isBase36) {

			string[] tokens = locationCode.Split('.');

			this.Sector = null;
			this.StarSystem = null;
			this.SystemBody = null;

			if (tokens.Length >= 2) {
				uint sX = 0, sY = 0;

				if (isBase36) {
					sX = (uint)Base36.Decode(tokens[0]);
					sY = (uint)Base36.Decode(tokens[1]);
				} else {
					uint.TryParse(tokens[0], out sX);
					uint.TryParse(tokens[1], out sY);
				}

				this.Sector = new Sector(sX, sY);

			}

			if (tokens.Length >= 3) {

				int systemNum;

				if (isBase36) {
					systemNum = (int)Base36.Decode(tokens[2]);
				} else {
					int.TryParse(tokens[2], out systemNum);
				}

				if (this.Sector.Systems.Count > systemNum)
					this.StarSystem = this.Sector.Systems[systemNum];
				else
					this.StarSystem = StarSystem.GetStarSystem(this.Sector, systemNum);

			}

			if (tokens.Length >= 4) {
				int systemBodyId;

				if (isBase36) {
					systemBodyId = (int)Base36.Decode(tokens[3]);
				} else {
					int.TryParse(tokens[3], out systemBodyId);
				}

				if (this.StarSystem.SystemBodies.Length > systemBodyId)
					this.SystemBody = this.StarSystem.SystemBodies[systemBodyId];
				else
					this.SystemBody = new SystemBody(StarSystem, systemBodyId);

			}

			if (tokens.Length >= 5) {
				this.PlanetSector = tokens[4];
			}

			if (tokens.Length >= 6) {
				this.Zone = tokens[5];
			}

		}

		public override string ToString() {
			StringBuilder sBuilder = new StringBuilder();
			if (this.Sector != null)
				sBuilder.AppendFormat("{0}.{1}", Base36.Encode((long)this.Sector.IndexX), Base36.Encode((long)this.Sector.IndexY));
			if (this.StarSystem != null)
				sBuilder.AppendFormat(".{0}", Base36.Encode((long)this.StarSystem.SysNum));
			if (this.SystemBody != null)
				sBuilder.AppendFormat(".{0}", Base36.Encode((long)this.SystemBody.SystemBodyId));
			if (this.PlanetSector != null)
				sBuilder.AppendFormat(".{0}", this.PlanetSector);
			if (this.Zone != null)
				sBuilder.AppendFormat(".{0}", this.Zone);

			return sBuilder.ToString();
		}

	}
}
