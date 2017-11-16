using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace GalaxyE.Managers {
	public class GameManager {

		private static GameManager instance = null;
		private static object instanceLocker = new object();
		private StarSystemAreaManager starSystemManager;
		private BattleManager battleManager;
		private MarketManager marketManager;

		public MarketManager MarketManager {
			get { return marketManager; }
		}

		public BattleManager BattleManager {
			get { return battleManager; }
		}

		public StarSystemAreaManager StarSystemManager {
			get { return starSystemManager; }
		}

		private Thread tickThread;

		private GameManager() {
			this.starSystemManager = new StarSystemAreaManager();
			this.battleManager = new BattleManager();
			this.marketManager = new MarketManager();

			this.tickThread = new Thread(
				new ParameterizedThreadStart(Tick));

			this.tickThread.IsBackground = true;
			this.tickThread.Priority = ThreadPriority.BelowNormal;
			this.tickThread.Start();
		}

		private int tickNum = 0;
		private void Tick(object state) {

			while (true) {
				tickNum++;
				System.Threading.Thread.Sleep(1000);



				if (tickNum % 10 == 0) {
					starSystemManager.Tick();
					battleManager.Tick();
					starSystemManager.Clean();
					battleManager.Clean();
					marketManager.Clean();
				}
			}
			//Tick(null);
		}

		public static GameManager Instance {
			get {
				if (instance == null) {
					lock (instanceLocker) {
						if (instance == null) {
							instance = new GameManager();
						}
					}
				}

				return instance;
			}

		}
	}
}
