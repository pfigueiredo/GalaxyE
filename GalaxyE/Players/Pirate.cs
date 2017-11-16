using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaxyE.Managers;
using GalaxyE.Galaxy;

namespace GalaxyE.Players {
	public class Pirate : NPC {

		private string _state = "SEARCH";
		private string State {
			get { return _state; }
			set {
				_state = value; 
				Console.WriteLine("{0} STATE: {1}", this, value);
			}
		}

		public Pirate(Coords systemCoords) : base(systemCoords) {
			//Init(systemCoords);
		}

		public void Init() {
			this.SpaceShip.BattleStarted += new BattleStartedEventHandler(SpaceShip_BattleStarted);
			this.SpaceShip.BattleEnded += new BattleEndedEventHandler(SpaceShip_BattleEnded);
			this.SpaceShip.TargetReached += new TargetReachedEventhandler(SpaceShip_TargetReached);
		}

		void SpaceShip_BattleEnded(object sender, SpaceBattle battle, SpaceShip enemy) {
			this.State = "SEARCH";
		}

		void SpaceShip_TargetReached(SectorObject sender, SystemEventArgs e) {
			if (State == "TARGET" || State == "BATTLE") {
				SpaceShip ship = this.SpaceShip.Target as SpaceShip;
				if (ship != null) {
					//GameManager.Instance.BattleManager.DoBattleCommand(this, ship, "F");
				}
			}
		}

		void SpaceShip_BattleStarted(object sender, SpaceBattle battle, SpaceShip enemy) {
			this.SpaceShip.EngageShip(enemy);
		}

		public override void NPCTick() {
			if (State == "SEARCH" && ScanForTargetsAndEngage()) this.State = "TARGET";
			if (State == "TARGET" && this.SpaceShip.SystemLocation.GetInSystemDistance(this.SpaceShip.Target.SystemLocation) < 1000) this.State = "BATTLE";
			if (State == "BATTLE") SpaceShip_TargetReached(this.SpaceShip, null);
			this.Update(false);
		}

		public bool ScanForTargetsAndEngage() {

			double dist = Double.MaxValue;
			SpaceShip target = null;

			var area = GameManager.Instance.StarSystemManager.GetStarSystemArea(this.SpaceShip.Coords);
			foreach (var ship in area.GetShipsInSystem()) {
				if ((ship.Player as Pirate) == null) { // will not attach other pirates
					double sDist = this.SpaceShip.SystemLocation.GetInSystemDistance(ship.SystemLocation);
					if (sDist < dist) {
						dist = sDist;
						target = ship;
					}
				}
			}

			//if (target != null)
			//    this.SpaceShip.EngageShip(target);

			return false; // target != null;
		}

		public void Init(Coords systemCoords) {

			//GameManager.Instance.StarSystemManager.Join(this, systemCoords);
			//var area = GameManager.Instance.StarSystemManager.GetStarSystemArea(systemCoords);

			//foreach (var ship in area.GetShipsInSystem()) {
			//    if ((ship.Player as Pirate) == null) { // will not attach other pirates
			//        break;
			//    }
			//}

		}



	}
}
