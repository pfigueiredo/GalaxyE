using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaxyE.Galaxy;

namespace GalaxyE.Players {

	public class SpaceBattle {

		public class BattleCommansPair {
			public int Turn { get; private set; }
			public BattleCommand A { get; set; }
			public BattleCommand B { get; set; }
			public BattleCommansPair(int turn) {
				this.Turn = turn;
			}
		}

		public enum SideTurnEnum { Any, SideA, SideB }

		public class BattleCommand {

			public string Command { get; set; }
			public DateTime Stamp { get; set; }
			public int HitPointsE { get; set; }
			public int HitPointsM { get; set; }
			public int RangePoints { get; set; }
			public int Multiplier { get; set; }
			public int DefencePointsE { get; set; }
			public int DefencePointsM { get; set; }

			public BattleCommand(
					string command,
					int hitPointsE, int hitPointsM, 
					int rangePoints, 
					int muliplier,
					int defencePointsE, int defencePointsM 
			) {
				this.Command = command;
				this.RangePoints = rangePoints;
				this.HitPointsE = hitPointsE;
				this.HitPointsM = hitPointsM;
				this.Multiplier = (muliplier == 0) ? 1 : muliplier;
				this.DefencePointsE = defencePointsE;
				this.DefencePointsM = defencePointsM;
				this.Stamp = DateTime.Now;
			}
		}

		SpaceShip sideA;
		SpaceShip sideB;
		SideTurnEnum sideTurn = SideTurnEnum.Any;

		public SpaceShip SideA { get { return sideA; } }
		public SpaceShip SideB { get { return sideB; } }

		bool battleStarted = false;
		bool battleEnded = false;

		public bool BattleStarted {
			get { return battleStarted; }
			private set { battleStarted = value; }
		}

		public bool BattleEnded {
			get { return battleEnded; }
			private set { battleEnded = value; }
		}

		public DateTime LastUpdate { get; private set; }

		public SpaceBattle(SpaceShip a, SpaceShip b) {
			this.sideA = a;
			this.sideB = b;
			this.commands = new Queue<BattleCommansPair>();
			this.LastUpdate = DateTime.Now;
		}

		private int turn = 0;
		Queue<BattleCommansPair> commands;
		private object commandLoker = new object();

		private int GetRangePoints(Equipment.ShipEquipment equip, double range) {

			if (range <= equip.ShortRangeDistance)
				return 0;
			else if (range <= equip.LongRangeDistance)
				return 2;
			else if (range <= equip.LongRangeDistance)
				return 4;
			else
				return 12;
			
		}

		private BattleCommand GetBattleCommand(SpaceShip ship, SpaceShip target, string command) {

			int rangePoints = 0, hitPointsE = 0, hitPointsM = 0, 
				multiplier = 0, defencePointsE = 0, defencePointsM = 0;
			var range = ship.SystemLocation.GetInSystemDistance(target.SystemLocation);
			bool commandOk = false;

			switch (command.ToUpper().Trim()) {
				case "M":
					if (ship.MissileLaunchers != null) {
						foreach (var missileLauncher in ship.MissileLaunchers) {
							if (missileLauncher.Activate()) {
								rangePoints += GetRangePoints(missileLauncher, range);
								hitPointsM += missileLauncher.HitPoints;
								multiplier++;
								commandOk = true;
							}
						}
					}

					if (commandOk) {
						if (ship.Player != null) {
							ship.Player.SendPlayerMessage(new PlayerMessage(
								"SHIP", PlayerMessageType.Battle, string.Format("{0} missiles fired at target {1}",
								multiplier, target.Code)));
						}
						if (target.Player != null) {
							target.Player.SendPlayerMessage(new PlayerMessage(
								ship.Code, PlayerMessageType.Battle,
								string.Format("missile incomming! range: {0}", SystemLocation.GetDistanceDesc(range))));
						}
					} else {
						ship.Player.SendPlayerMessage(new PlayerMessage(
							"SHIP", PlayerMessageType.BattleWarning, "Not enougf enery to fire missiles"));
					}
					break;

				case "F":
					if (ship.Lasers != null) {
						foreach (var laser in ship.Lasers) {
							if (laser.Activate()) {
								rangePoints += GetRangePoints(laser, range);
								hitPointsE += laser.HitPoints;
								multiplier++;
								commandOk = true;
							}
						}
					}

					if (commandOk) {
						if (ship.Player != null) {
							ship.Player.SendPlayerMessage(new PlayerMessage(
								"SHIP", PlayerMessageType.Battle, string.Format("{0} lasers fired at target {1}",
								multiplier, target.Code)));
						}
						if (target.Player != null) {
								target.Player.SendPlayerMessage(new PlayerMessage(
									ship.Code, PlayerMessageType.Battle,
									string.Format("laser fire! range: {0}", SystemLocation.GetDistanceDesc(range))));
						}
					} else {
						ship.Player.SendPlayerMessage(new PlayerMessage(
							"SHIP", PlayerMessageType.BattleWarning, "Not enougf enery to fire lasers"));
					}

					break;
				case "E":
					defencePointsE += 2; //Todo calculate this based on spaceship and experience;
					defencePointsM += 2; //Todo calculate this based on spaceship and experience;
					multiplier = 1;
					commandOk = true;
					ship.Player.SendPlayerMessage(new PlayerMessage(
							"SHIP", PlayerMessageType.Battle, "Performing evasive manouvers"));
					target.Player.SendPlayerMessage(new PlayerMessage(
							ship.Code, PlayerMessageType.Battle, "target is performing evasive manouvers"));
					break;
				case "X":
					break;
				case "S":
					break;
			}

			return new BattleCommand(command, hitPointsE, hitPointsM, rangePoints, 
				multiplier, defencePointsE, defencePointsM);
		}

		public int DoBattleCommand(SpaceShip ship, string command) {

			bool alreadyStarted = battleStarted;
			BattleCommansPair pair = null;

			lock (commandLoker) {

				if (commands.Count == 0)
					sideTurn = SideTurnEnum.Any;

				if (sideTurn == SideTurnEnum.Any) {
					pair = new BattleCommansPair(++turn);

					var message = new PlayerMessage("Battle", PlayerMessageType.Battle,
							string.Format("Battle turn {0} created {1} vs {2}",
								turn, sideA.Player.Name, sideB.Player.Name));
					sideA.Player.SendPlayerMessage(message);
					sideB.Player.SendPlayerMessage(message);

				} else if (commands.Count > 0) {
					pair = commands.Peek();
				}

				if (pair != null) {
					if (ship == sideA && pair.A == null) {
						pair.A = GetBattleCommand(ship, sideB, command);
						sideTurn = SideTurnEnum.SideB;
						battleStarted = true;
					} else if (ship == sideB && pair.B == null) {
						pair.B = GetBattleCommand(ship, sideA, command);
						sideTurn = SideTurnEnum.SideA;
						battleStarted = true;
					}

					if (pair.A != null && pair.B != null)
						sideTurn = SideTurnEnum.Any;
				}
			}

			if (!alreadyStarted && battleStarted) {
				sideA.ReportBattleStarted(this, sideB);
				sideB.ReportBattleStarted(this, sideA);
			}

			return turn;
		}

		//Default type of commands
		//
		// F - Fire main weapons
		// M - Fire missile
		// E - Evade maneuver
		// X - Jammer 
		// S - Energy Shield

		private void CalculateCommands(BattleCommand a, BattleCommand b) {

			if (a == null) a = GetBattleCommand(sideA, sideB, "E");
			if (b == null) b = GetBattleCommand(sideB, sideA, "E"); ;

			int baseToHitA = 4; //Check ship parameters
			int baseToHitB = 4; //Check ship parameters

			baseToHitA += (a.RangePoints / a.Multiplier);
			baseToHitB += (b.RangePoints / b.Multiplier);

			Random rand = new Random();

			bool AWasHit = (rand.Next(1, 12) > baseToHitB);
			bool BWasHit = (rand.Next(1, 12) > baseToHitA);

			//Calculate Defence

			int hitA = (b.HitPointsE - a.DefencePointsE) + (b.DefencePointsM - a.DefencePointsM);
			int hitB = (a.HitPointsE - b.DefencePointsE) + (a.DefencePointsM - b.DefencePointsM);

			if (AWasHit) sideA.DoDamage(hitA);
			if (BWasHit) sideB.DoDamage(hitB);

		}

		public void BattleTick() {

			this.LastUpdate = DateTime.Now;

			var dist = sideA.SystemLocation.GetInSystemDistance(sideB.SystemLocation);

			if (dist < 2000) {
				sideA.MakeBattleCommand(this);
				sideB.MakeBattleCommand(this);
			}

			if (sideA.SystemId != sideB.SystemId 
				|| sideA.InPlanet || sideB.InPlanet
				|| sideA.InHiperspace || sideB.InHiperspace
			) {
				//someone has runned away...
				this.battleEnded = true;
				sideA.ReportBattleEnded(this, sideB);
				sideB.ReportBattleEnded(this, sideA);
			}

			if (!battleStarted || battleEnded) return;

			sideA.Update(false);
			sideB.Update(false);

			sideA.BattleTick();
			sideB.BattleTick();

			lock (commandLoker) {

				while (commands.Count > 0) {
					var pair = commands.Peek();
					if (pair.A != null && pair.B != null) {
						pair = commands.Dequeue();
						CalculateCommands(pair.A, pair.B);
					} else {
						if (
							(pair.A != null && DateTime.Now.Subtract(pair.A.Stamp).TotalSeconds > 15)
							|| (pair.B != null && DateTime.Now.Subtract(pair.B.Stamp).TotalSeconds > 15)
						) {
							pair = commands.Dequeue();
							CalculateCommands(pair.A, pair.B);
						}
					}
					
				}
			}
		}

	}
}
