using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaxyE.Players;
using System.Threading;

namespace GalaxyE.Managers {
	public class BattleManager {

		private Dictionary<SpaceShip, List<SpaceBattle>> PlayerBattles { get; set; }
		private List<SpaceBattle> Battles { get; set; }
		private ReaderWriterLock rwl = new ReaderWriterLock();

		public BattleManager() {
			this.PlayerBattles = new Dictionary<SpaceShip, List<SpaceBattle>>();
			this.Battles = new List<SpaceBattle>();
		}

		public List<SpaceShip> GetCurrentBattleTargets(Player player) {
			rwl.AcquireReaderLock(-1);
			List<SpaceShip> returnList = new List<SpaceShip>();
			try {
				if (PlayerBattles.ContainsKey(player.SpaceShip)) {
					foreach (var battle in PlayerBattles[player.SpaceShip]) {
						returnList.Add((battle.SideA == player.SpaceShip) ? battle.SideB : battle.SideA);
					}
				}
			} finally {
				rwl.ReleaseReaderLock();
			}
			return returnList;
		}

		private	void DoBattleCommand(Player player, SpaceShip enemyShip, string command) {
			rwl.AcquireReaderLock(-1);
			try {
				if (PlayerBattles.ContainsKey(player.SpaceShip)) {
					foreach (var battle in PlayerBattles[player.SpaceShip]) {
						if (battle.SideA == enemyShip || battle.SideB == enemyShip) {
							battle.DoBattleCommand(player.SpaceShip, command);
							break;
						}
					}
				}
			} finally {
				rwl.ReleaseReaderLock();
			}
		}

		public void CreateBattle(Player player, SpaceShip enemyShip) {

			rwl.AcquireReaderLock(-1);
			try {
				if (player.SpaceShip.SystemId == enemyShip.SystemId) {

					bool alreadyInBattle = false;
					SpaceBattle currentBattle = null;

					if (!PlayerBattles.ContainsKey(player.SpaceShip)) {
						rwl.UpgradeToWriterLock(-1);
						PlayerBattles.Add(player.SpaceShip, new List<SpaceBattle>());
					} else {
						//check if the battle already exists
						foreach (var battle in PlayerBattles[player.SpaceShip]) {
							if (battle.SideA == enemyShip || battle.SideB == enemyShip) {
								alreadyInBattle = true;
								currentBattle = battle;
								break;
							}
						}
					}

					if (!PlayerBattles.ContainsKey(enemyShip)) {
						if (!rwl.IsWriterLockHeld) rwl.UpgradeToWriterLock(-1);
						PlayerBattles.Add(enemyShip, new List<SpaceBattle>());
					}

					if (!alreadyInBattle) {
						if (!rwl.IsWriterLockHeld) rwl.UpgradeToWriterLock(-1);
						Console.WriteLine("Creating battle");
						currentBattle = new SpaceBattle(player.SpaceShip, enemyShip);
						PlayerBattles[player.SpaceShip].Add(currentBattle);
						PlayerBattles[enemyShip].Add(currentBattle);
						Battles.Add(currentBattle);
					}
				}
			} finally {
				if (rwl.IsWriterLockHeld) rwl.ReleaseWriterLock();
				if (rwl.IsReaderLockHeld) rwl.ReleaseReaderLock();
			}
		}


		public void Tick() {
			rwl.AcquireReaderLock(-1);
			try {
			    foreach (var battle in Battles) {
					battle.BattleTick();
			    }
			} finally {
				rwl.ReleaseReaderLock();
			}

		}

		public void Clean() {

			rwl.AcquireReaderLock(-1);
			try {

				List<SpaceBattle> battlesToRemove = new List<SpaceBattle>();

				foreach (var battle in Battles) {
					if (
						battle.BattleEnded 
						|| DateTime.Now.Subtract(battle.LastUpdate).TotalMinutes > 2) {


						Console.WriteLine("Destroing battle");

						var k = rwl.UpgradeToWriterLock(-1);
						
						if (PlayerBattles.ContainsKey(battle.SideA)) {
							if (PlayerBattles[battle.SideA].Contains(battle))
								PlayerBattles[battle.SideA].Remove(battle);
						}

						if (PlayerBattles.ContainsKey(battle.SideB)) {
							if (PlayerBattles[battle.SideB].Contains(battle))
								PlayerBattles[battle.SideB].Remove(battle);
						}

						battlesToRemove.Add(battle);
					}
				}

				if (battlesToRemove.Count > 0) {
					if (!rwl.IsWriterLockHeld)
						rwl.UpgradeToWriterLock(-1);
					
					foreach (var battle in battlesToRemove) {
						if (Battles.Contains(battle)) {
							Battles.Remove(battle);
						}
					}	
					
				}

			} finally {
				if (rwl.IsWriterLockHeld) rwl.ReleaseWriterLock();
				if (rwl.IsReaderLockHeld) rwl.ReleaseReaderLock();
			}

		}

	}
}
