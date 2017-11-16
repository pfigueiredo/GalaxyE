using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaxyE.Galaxy;
using GalaxyE.Galaxy.Zones;

namespace GalaxyE.Players {

	public interface IBSSOptionContainer {
		List<BBS.BBSOption> Options { get; set; }
		BBS.BBSOption Parent { get; set; }
	}

	public class BBS : IBSSOptionContainer {

		public class BBSOption : IBSSOptionContainer {

			public string OptionCode { get; set; }
			public string Text { get; set; }
			public string Cmd { get; set; }
			public List<BBSOption> Options { get; set; }
			public bool JumpPrevious { get; set; }
			public BBSOption Parent { get; set; }

			public BBSOption(string optionCode, string text, string cmd, BBSOption parent) {

				this.OptionCode = optionCode;
				this.Text = text;
				this.Options = new List<BBSOption>();
				this.Parent = parent;
				this.JumpPrevious = false;
				this.Cmd = cmd;

			}

			public BBSOption(string optionCode, string text, string cmd, BBSOption parent, bool jumpPrevious) {

				this.OptionCode = optionCode;
				this.Text = text;
				this.Options = new List<BBSOption>();
				this.Parent = parent;
				this.JumpPrevious = jumpPrevious;
				this.Cmd = cmd;

			}

		}

		public string BBSCode { get; set; }
		public string BBSName { get; set; }
		public List<BBSOption> Options { get; set; }
		public BBSOption Parent { get; set; }

		public BBSOption SelectOption(IBSSOptionContainer container, string value) {

			foreach (var opt in container.Options) {
				if (opt.OptionCode.Equals(value, StringComparison.InvariantCultureIgnoreCase)) {
					if (opt.JumpPrevious)
						return container.Parent;
					else
						return opt;
				}
			}

			return null;
		}

		public BBS(SpacePort port) : this() {
			this.BBSName = port.Name;
			this.BBSCode = string.Format("{0}/{1}:{2}", port.Body.Coords.ToString(), port.Body.Name, port.Name);
		}

		public BBS(SystemBody body) : this() {
			this.BBSName = body.Name;
			this.BBSCode = string.Format("{0}/{1}", body.Coords.ToString(), body.Name);
			this.Parent = null;
		}

		public BBS(StarSystem system) : this() {

			this.BBSName = system.Name;
			this.BBSCode = system.Coords.ToString();
			this.Parent = null;

		} 

		private BBS() {

			Options = new List<BBSOption>();

			Options.Add(new BBSOption("1", "Local Authority", "", null));
			Options.Add(new BBSOption("2", "Shipyard", "", null));
			Options.Add(new BBSOption("3", "Stock Market", "StockMarket", null));
			Options.Add(new BBSOption("5", "Bulletin Board", "BBSAds", null));
			Options.Add(new BBSOption("6", "Exit", "", null));

			Options[0].Options.Add(new BBSOption("1", "Pay Fines", "PayFines", Options[0]));
			Options[0].Options.Add(new BBSOption("2", "Back", "", Options[0], true));

			Options[1].Options.Add(new BBSOption("1", "Upgrades", "ShpUpgrade", Options[1]));
			Options[1].Options.Add(new BBSOption("2", "Repairs and Servicing", "ShpRepair", Options[1]));
			Options[1].Options.Add(new BBSOption("3", "New and Reconditioned Ships", "ShpShop", Options[1]));
			Options[1].Options.Add(new BBSOption("4", "Back", "", Options[1], true));

		}



	}
}
