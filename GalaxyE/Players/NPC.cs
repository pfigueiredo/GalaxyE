using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaxyE.Galaxy;

namespace GalaxyE.Players {
	public class NPC : Player {

		private DateTime nextActionStamp;
		private int seed;
		private Random rand;

		public int Seed { 
			get { return seed; } 
			set { 
				seed = value;
				rand = new Random(seed);
			} 
		}

		public override bool IsNPC { get { return true; } }

		public int NPCId { get; set; }

		public NPC(Coords coords)
			: base(Guid.Empty, new GameLocation(coords) ) {
				this.SpaceShip.Coords = coords;
				this.nextActionStamp = DateTime.Now;
				this.Name = Utils.RandomNameGenerator.GetRandomName();
		}

		public override void Persist() { }

		public override bool Update(bool precist) {
			return base.Update(false);
		}

		public override bool Update() {
			return base.Update(false);
		}

		public virtual void NPCTick() { }

		
	}
}
