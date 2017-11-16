using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GalaxyE.Galaxy {
	public class StarDate {

		public DateTime Date { get; private set; }
		public Double SDate {
			get {
				double starDate = Date.Year;
				starDate += Date.DayOfYear / 365.25;
				return starDate;
			}
		}

		public StarDate(DateTime starDateTime) {
			this.Date = starDateTime;
		}

		public static StarDate Now {
			get { return GetStarDate(DateTime.Now); }
		}

		public static StarDate GetStarDate(DateTime date) {

			DateTime baseDate = new DateTime(2012, 1, 1);
			TimeSpan diff = date.Subtract(baseDate);
			//TimeSpan baseSpan = new TimeSpan(diff.Ticks);

			//baseDate = baseDate.Add(baseSpan).AddYears(1204);
			baseDate = baseDate.Add(diff).AddYears(1204);

			StarDate starDate = new StarDate(baseDate);

			return starDate;
		}

		public override string ToString() {
			return String.Format("{0:0.0000} EGMT {1} {2}", 
				this.SDate, 
				this.Date.ToShortDateString(), 
				this.Date.ToShortTimeString()
			);
		}
	}
}
