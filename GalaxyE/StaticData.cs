using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GalaxyE {

	public class Parameters {

		public string SqlConnectionString { get; set; }

		private Parameters() {
			//Default desenv connection string must overriden in configuration
			this.SqlConnectionString = @"server=FAVVUS-TOSH\MSSQLSERVERR2;Persist Security Info=False;Integrated Security=SSPI;database=MUSA";
		}

		private static Parameters instance = null;
		private static object instanceLocker = new object();

		public static Parameters Instance {
			get {
				if (instance == null) {
					lock (instanceLocker) {
						if (instance == null) {
							instance = new Parameters();
						}
					}
				}

				return instance;
			}
		}

	}

}
