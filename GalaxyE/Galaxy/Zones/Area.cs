using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaxyE.Players;

namespace GalaxyE.Galaxy.Zones {
	public class Area {

		public class AreaCommand {

			public string Command { get; set; }
			public void Action(Player player) {

			}

		}

		public class AreaObject {
			public string Name { get; set; }
			public string Description { get; set; }
		}

		public string AreaId { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }



	}
}
