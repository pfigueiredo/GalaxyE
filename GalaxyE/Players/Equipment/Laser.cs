using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GalaxyE.Players.Equipment {

	[Serializable]
	public class ShipEquipment {

		public virtual int HitPoints { get { return this.Power; } }
		public int Power { get; set; }
		public int Energy { get; set; }

		public int ShortRangeDistance { get; set; }
		public int MediumRangeDistance { get; set; }
		public int LongRangeDistance { get; set; }

		public SpaceShip SpaceShip { get; set; }

		public ShipEquipment(SpaceShip ship) {
			this.SpaceShip = ship;
			// default values;
			ShortRangeDistance = 500;
			MediumRangeDistance = 1000;
			LongRangeDistance = 1000 * 10;
		}

		public bool Activate() {

			if (this.SpaceShip != null) {
				if (this.SpaceShip.Energy > this.Energy) {
					this.SpaceShip.Energy -= Energy;
				}

				if (this.SpaceShip.Energy < 0) this.SpaceShip.Energy = 0;
				if (this.SpaceShip.Energy > 100) this.SpaceShip.Energy = 100;
				return true;
			}
			return false;
		}

	}

	[Serializable]
	public class Laser : ShipEquipment {
		public Laser(SpaceShip ship) : base(ship) { }
	}

	[Serializable]
	public class ShieldGenerator : ShipEquipment {
		public ShieldGenerator(SpaceShip ship) : base(ship) { }
	}

	[Serializable]
	public class ECM : ShipEquipment {
		public ECM(SpaceShip ship) : base(ship) { }
	}

	[Serializable]
	public class MissileLauncher : ShipEquipment {
		public MissileLauncher(SpaceShip ship) : base(ship) { }
	}

	[Serializable]
	public class EnergyGenerator : ShipEquipment {
		public EnergyGenerator(SpaceShip ship) : base(ship) { }
	}
}
