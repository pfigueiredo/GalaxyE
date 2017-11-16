using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaxyE.Galaxy;
using System.Threading;

namespace GalaxyE.Managers {
	public class MarketManager {

		private Dictionary<string, MarketManager_Entry> markets = new Dictionary<string, MarketManager_Entry>();
		private ReaderWriterLock wrl = new ReaderWriterLock();

		public void Clean() {
			wrl.AcquireReaderLock(-1);
			try {
				List<string> marketToRemove = new List<string>();

				foreach (var key in markets.Keys) {
					MarketManager_Entry ioM = markets[key];
					if (DateTime.Now.Subtract(ioM.Stamp).TotalMinutes > 10)
						marketToRemove.Add(key);
				}

				if (marketToRemove.Count > 0) {
					var lc = wrl.UpgradeToWriterLock(-1);
					try {
						foreach (var key in marketToRemove) {
							if (markets.ContainsKey(key)) {
								MarketManager_Entry ioM = markets[key];
								if (DateTime.Now.Subtract(ioM.Stamp).TotalMinutes > 10)
									markets.Remove(key);
							}
						}
					} finally {
						wrl.DowngradeFromWriterLock(ref lc);
					}
				}


			} finally {
				wrl.ReleaseReaderLock();
			}

		}


		public Market.Market GetMarket(SystemBody body, string code) {

			string marketCode = string.Format("{0}/{1}/{2}", body.StarSystem.Coords, body.SystemBodyId, code);

			wrl.AcquireReaderLock(-1);
			try {

				if (markets.ContainsKey(marketCode)) {
					MarketManager_Entry ioM = markets[marketCode];
					ioM.Stamp = DateTime.Now;
					return ioM.Market;
				} else {

					LockCookie lc = wrl.UpgradeToWriterLock(-1);
					try {

						Market.Market m = new Market.Market(body, code);
						MarketManager_Entry ioM = new MarketManager_Entry(m);
						markets.Add(marketCode, ioM);
						return m;

					} finally {
						wrl.DowngradeFromWriterLock(ref lc);
					}
				}


			} finally {
				wrl.ReleaseReaderLock();
			}
		}

		public class MarketManager_Entry {

			public Market.Market Market { get; private set; }
			public DateTime Stamp { get; set; }

			public MarketManager_Entry(Market.Market market) {
				this.Market = market;
				this.Stamp = DateTime.Now;
			}

		}

	}
}
