using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaxyE.Players;
using GalaxyE.Galaxy.Zones;

namespace TelnetServer.Game {

	class ioSpacePortBBS {

		public BBS SelectedBBS { get; set; }
		public BBS.BBSOption SelectedOption { get; set; }

		public BBS.BBSOption SelectOption(string value) {
			BBS.BBSOption option = SelectedBBS.SelectOption(
				(SelectedOption != null) ? (IBSSOptionContainer)SelectedOption : SelectedBBS, value
			);

			SelectedOption = option;

			return option;
		}

		public void OpenBBS(SpacePort port) {
			if (port != null) {
				SelectedBBS = new BBS(port);
				SelectedOption = null;
			} else {
				SelectedBBS = null;
				SelectedOption = null;
			}

		}

	}
}
