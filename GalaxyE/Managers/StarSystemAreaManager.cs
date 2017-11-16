using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using GalaxyE.Players;
using GalaxyE.Galaxy;

namespace GalaxyE.Managers {

	public class StarSystemAreaManager {

		private ReaderWriterLock rwl = new ReaderWriterLock();
		Dictionary<string, StarSystemAreaManager_Entry> Systems { get; set; }

		public StarSystemAreaManager() {
			Systems = new Dictionary<string, StarSystemAreaManager_Entry>();
		}

		public StarSystemAreaManager_Entry GetStarSystemArea(Coords system) {
			rwl.AcquireReaderLock(-1);
			try {
				string systemId = system.ToString();
				if (Systems.ContainsKey(systemId))
					return Systems[systemId];
			} finally {
				rwl.ReleaseReaderLock();
			}

			return null;
		}

		private void CreateNPCs(StarSystemAreaManager_Entry area) {
			Pirate p = new Pirate(area.Coords);
		}

		public void Join(Player player, Coords system) {
			rwl.AcquireReaderLock(-1);
			string systemId = system.ToString();
			try {
				if (!Systems.ContainsKey(systemId)) {
					var lc = rwl.UpgradeToWriterLock(-1);
					try {
						if (!Systems.ContainsKey(systemId)) {
							Console.WriteLine("Creating System Area {0}", systemId);
							StarSystemAreaManager_Entry obj = new StarSystemAreaManager_Entry(system);
							Systems.Add(systemId, obj);
							obj.Join(player);
							CreateNPCs(obj);
						}
					} finally {
						rwl.DowngradeFromWriterLock(ref lc);
					}
				} else {
					Systems[systemId].Join(player);
				}
			} finally {
				rwl.ReleaseReaderLock();
			}
		}

		public void Leave(Player player, Coords system) {
			rwl.AcquireReaderLock(-1);
			string systemId = system.ToString();
			try {
				if (Systems.ContainsKey(systemId)) {
					var lc = rwl.UpgradeToWriterLock(-1);
					try {
						if (Systems.ContainsKey(systemId)) {
							StarSystemAreaManager_Entry obj = Systems[systemId];
							obj.Leave(player);
						}
					} finally {
						rwl.DowngradeFromWriterLock(ref lc);
					}
				}
			} finally {
				rwl.ReleaseReaderLock();
			}
		}

		public void Tick() {
			rwl.AcquireReaderLock(-1);
			try {
				foreach (var key in Systems.Keys.ToArray()) {
					Systems[key].Tick();
				}
			} finally {
				rwl.ReleaseReaderLock();
			}
		}

		public void Clean() {

			rwl.AcquireReaderLock(-1);
			try {

				List<string> systemsToRemove = new List<string>();
				DateTime current = DateTime.Now;
				foreach (var key in Systems.Keys) {
					Systems[key].Clean();
					if (current.Subtract(Systems[key].Stamp).TotalMinutes > 3) {
						systemsToRemove.Add(key);
					}
				}

				if (systemsToRemove.Count > 1) {
					var lc = rwl.UpgradeToWriterLock(-1);
					try {
						foreach (var key in systemsToRemove) {
							Console.WriteLine("Destroing System Area {0}", key);
							Systems.Remove(key);
						}
					} finally {
						rwl.DowngradeFromWriterLock(ref lc);
					}
				}

			} finally {
				rwl.ReleaseReaderLock();
			}

		}

	}

	public class StarSystemAreaManager_Entry {

		private ReaderWriterLock rwl = new ReaderWriterLock();
		public string SystemId { get; set; }
		public DateTime Stamp { get; set; }
		public Coords Coords { get; set; }
		public Dictionary<Player, DateTime> Players { get; set; }
		public List<Player> NPCs { get; set; }
		public List<SpaceShip> SpaceShips { get; set; }

		public StarSystemAreaManager_Entry(Coords coords) {
			this.SystemId = coords.ToString();
			this.Coords = coords;
			this.Players = new Dictionary<Player, DateTime>();
			this.SpaceShips = new List<SpaceShip>();
		}

		public StarSystemAreaManager_Entry(StarSystem system) {
			this.SystemId = system.Coords.ToString();
			this.Coords = system.Coords;
			this.Players = new Dictionary<Player, DateTime>();
			this.SpaceShips = new List<SpaceShip>();
		}

		public void CreateNPCs() {
			if (Coords != null) {
				Pirate pirate = new Pirate(this.Coords);
			}
		}

		public List<SpaceShip> GetShipsInSystem() {
			rwl.AcquireReaderLock(-1);
			try {
				return new List<SpaceShip>(this.SpaceShips.ToArray());
			} finally {
				rwl.ReleaseReaderLock();
			}
		}

		public void Join(Player player) {

			if (player.SpaceShip == null) return;

			rwl.AcquireReaderLock(-1);
			Stamp = DateTime.Now;
			try {
				if (!Players.ContainsKey(player)) {
					var lc = rwl.UpgradeToWriterLock(-1);
					try {
						if (!Players.ContainsKey(player)) {
							Console.WriteLine("Enter system Area {0} - {1}", player.Name, SystemId);
							Players.Add(player, DateTime.Now);
							if (!SpaceShips.Contains(player.SpaceShip))
								SpaceShips.Add(player.SpaceShip);
						}
					} finally {
						rwl.DowngradeFromWriterLock(ref lc);
					}

					foreach (var p in Players.Keys) {
						if (p != player)
							p.Send_ShipEnterSystem(player.SpaceShip);
					}
				}
			} finally {
				rwl.ReleaseReaderLock();
			}
		}

		public void Tick(Player player) {
			rwl.AcquireReaderLock(-1);
			Stamp = DateTime.Now;
			try {
				if (Players.ContainsKey(player)) {
					var lc = rwl.UpgradeToWriterLock(-1);
					try {
						if (Players.ContainsKey(player)) {
							Players[player] = DateTime.Now;
						}
					} finally {
						rwl.DowngradeFromWriterLock(ref lc);
					}
				}
			} finally {
				rwl.ReleaseReaderLock();
			}
		}

		public void Leave(Player player) {
			rwl.AcquireReaderLock(-1);
			try {
				if (Players.ContainsKey(player)) {
					var lc = rwl.UpgradeToWriterLock(-1);
					try {
						if (Players.ContainsKey(player)) {
							Console.WriteLine("Leaving System Area {0} - {1}", player.Name, SystemId);
							Players.Remove(player);
							if (SpaceShips.Contains(player.SpaceShip))
								SpaceShips.Remove(player.SpaceShip);
						}
					} finally {
						rwl.DowngradeFromWriterLock(ref lc);
					}

					foreach (var p in Players.Keys) {
						if (p != player)
							p.Send_ShipLeaveSystem(player.SpaceShip);
					}
				}
			} finally {
				rwl.ReleaseReaderLock();
			}
		}

		public IEnumerable<SpaceShip> ListSpaceShips() {

			rwl.AcquireReaderLock(-1);
			try {

				foreach (SpaceShip ship in this.SpaceShips)
					yield return ship;
				
			} finally {
				rwl.ReleaseReaderLock();
			}

		}

		public void Tick() {
			rwl.AcquireReaderLock(-1);
			try {

				foreach (var p in this.Players.Keys) {
					NPC npc = p as NPC;
					if (npc != null) {
						npc.NPCTick();
					}
				}

			} finally {
				rwl.ReleaseReaderLock();
			}
		}

		public void Clean() {

			rwl.AcquireReaderLock(-1);
			try {

				List<Player> usersToRemove = new List<Player>();
				DateTime current = DateTime.Now;
				foreach (var key in Players.Keys) {
					if (current.Subtract(Players[key]).TotalMinutes > 2) {
						if (key.SpaceShip.Coords.ToString() != this.SystemId)
							usersToRemove.Add(key);
					}
				}

				if (usersToRemove.Count > 1) {
					var lc = rwl.UpgradeToWriterLock(-1);
					try {
						foreach (var key in usersToRemove) {
							
							Players.Remove(key);
							if (SpaceShips.Contains(key.SpaceShip))
								SpaceShips.Remove(key.SpaceShip);
						}

						if (Players.Count > 0)
							this.Stamp = DateTime.Now;

					} finally {
						rwl.DowngradeFromWriterLock(ref lc);
					}
				}

			} finally {
				rwl.ReleaseReaderLock();
			}

		}


	}
}
