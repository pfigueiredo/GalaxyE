using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaxyE.Galaxy;
using System.IO;

namespace TelnetServer.Game {
	public class ioPlanetInfo {

		public SystemBody Planet { get; private set; }
		public TextCanvas Canvas { get; private set; }

		private int screenWidth = 0;

		public ioPlanetInfo(SystemBody planet) : this (planet, 79) { }

		public ioPlanetInfo(SystemBody planet, int screenWidth) {
			this.Planet = planet;
			this.screenWidth = screenWidth;
		}

		private void GetPlanetSectorString(SystemBody.PlanetSectorType sector, StringBuilder zoneBuilder) {

			bool isRetricted = (sector & SystemBody.PlanetSectorType.Restricted) == SystemBody.PlanetSectorType.Restricted; 
			bool hasStarPort = (sector & SystemBody.PlanetSectorType.StarPort) == SystemBody.PlanetSectorType.StarPort;

			bool isGaz = (sector & SystemBody.PlanetSectorType.Gaz) == SystemBody.PlanetSectorType.Gaz; 
			bool isRock = (sector & SystemBody.PlanetSectorType.Rock) == SystemBody.PlanetSectorType.Rock; 
			bool isLand = (sector & SystemBody.PlanetSectorType.Land) == SystemBody.PlanetSectorType.Land; 
			bool isLava = (sector & SystemBody.PlanetSectorType.Lava) == SystemBody.PlanetSectorType.Lava; 
 			bool isWater = (sector & SystemBody.PlanetSectorType.Water) == SystemBody.PlanetSectorType.Water;
			bool isIce = (sector & SystemBody.PlanetSectorType.Ice) == SystemBody.PlanetSectorType.Ice;
			bool isUnclassified = (sector & SystemBody.PlanetSectorType.Unclassified) == SystemBody.PlanetSectorType.Unclassified;

			zoneBuilder.AppendFormat("{0}[{1}", ANSI.ColorANSI16((isRetricted) ? ANSI.ANSIColor_16.Red : ANSI.ANSIColor_16.Cyan), ANSI.ClearFormat());

			if (hasStarPort) {
				zoneBuilder.AppendFormat("{0}{{P}}{1}", ANSI.ColorANSI16(ANSI.ANSIColor_16.Green), ANSI.ClearFormat());
			} else {

				if (isGaz)
					zoneBuilder.AppendFormat("{0}%%%{1}",
						ANSI.ColorANSI16(ANSI.ANSIColor_16.Magenta), 
						ANSI.ClearFormat());
				if (isRock)
					zoneBuilder.AppendFormat("{0}###{1}",
						ANSI.ColorANSI16(ANSI.ANSIColor_16.Darkgray), 
						ANSI.ClearFormat());
				if (isLand)
					zoneBuilder.AppendFormat("{0}###{1}",
						ANSI.ColorANSI16(ANSI.ANSIColor_16.Green), 
						ANSI.ClearFormat());
				if (isLava)
					zoneBuilder.AppendFormat("{0}%%%{1}",
						ANSI.ColorANSI16(ANSI.ANSIColor_16.Red), 
						ANSI.ClearFormat());
				if (isWater)
					zoneBuilder.AppendFormat("{0}%%%{1}",
						ANSI.ColorANSI16(ANSI.ANSIColor_16.Blue), 
						ANSI.ClearFormat());
				if (isIce)
					zoneBuilder.AppendFormat("{0}###{1}",
						ANSI.ColorANSI16(ANSI.ANSIColor_16.White), 
						ANSI.ClearFormat());
				if (isUnclassified)
					zoneBuilder.AppendFormat("{0}%%%{1}",
						ANSI.ColorANSI16(ANSI.ANSIColor_16.BrightYellow),
						ANSI.ClearFormat());
			}

			zoneBuilder.AppendFormat("{0}]{1}", ANSI.ColorANSI16((isRetricted) ? ANSI.ANSIColor_16.Red : ANSI.ANSIColor_16.Cyan), ANSI.ClearFormat());
			//return zoneBuilder.ToString();
		}

		private string WrapString(string s) {

			int columnWidth = 79;
			string[] words = s.Split(' ');
			StringBuilder newSentence = new StringBuilder();

			string line = "";
			foreach (string word in words) {
				if ((line + word).Length > columnWidth) {
					newSentence.AppendLine(line);
					line = "";
				}

				line += string.Format("{0} ", word);
			}

			if (line.Length > 0)
				newSentence.AppendLine(line);

			return newSentence.ToString();
		}

		public string GetInfo() {


			string baseDir = "./Messages/Planets";
			string filename = string.Empty;

			switch (Planet.BodyType) {
				case SystemBodyType.Asteroid: filename = "AS.MII"; break;
				case SystemBodyType.RockyWorld: filename = "R.MII"; break;
				case SystemBodyType.RockyPlanetoid: filename = "R2.MII"; break;
				case SystemBodyType.Inferno: filename = "I.MII"; break;
				case SystemBodyType.Venuzian: filename = "V.MII"; break;
				case SystemBodyType.Terrestrial: filename = "T.MII"; break;
				case SystemBodyType.WaterWorld: filename = "W.MII"; break;
				case SystemBodyType.IceWorld: filename = "W2.MII"; break;
				case SystemBodyType.GasGiant: filename = "GS.MII"; break;
				case SystemBodyType.RingedGasGiant: filename = "GS2.MII"; break;
				case SystemBodyType.SubGasGiant: filename = "GS3.MII"; break;
				default: filename = "R.MII"; break;
			}


			//get the star ansi image;
			filename = Path.Combine(baseDir, filename);
			var starFile = Messages.Message.ReadMII2File(filename);

			TextCanvas canvas = new TextCanvas(screenWidth, starFile.Height, TextCanvas.NumOfColorsEnum.XTERM256);
			starFile.Write(0, 0, canvas);

			canvas.DrawString(40, canvas.Height - 5, string.Format("Planet: {0}", Planet.Name), (byte)ANSI.ANSIColor_16.Cyan, 0);
			canvas.DrawString(40, canvas.Height - 4, string.Format("Planet class: {0}", Planet.GetClass_Ext()), (byte)ANSI.ANSIColor_16.Cyan, 0);
			canvas.DrawString(40, canvas.Height - 3, string.Format("Orbital period: {0} Earth years", Planet.OrbitalPeriod), (byte)ANSI.ANSIColor_16.Cyan, 0);
			canvas.DrawString(40, canvas.Height - 2, string.Format("Orbital radius: {0:0.000} AU", Planet.OrbitalRadius), (byte)ANSI.ANSIColor_16.Cyan, 0);
			canvas.DrawString(40, canvas.Height - 1, string.Format("Surface Temperature: {0:0.0}°", Planet.Temperature), (byte)ANSI.ANSIColor_16.Cyan, 0);


			StringBuilder sBuilder = new StringBuilder();

			sBuilder.AppendLine(canvas.ToString());
			//sBuilder.AppendLine(WrapString(Planet.Description));

			sBuilder.AppendFormat("\r\n");
			sBuilder.AppendFormat(ANSI.SetColorANSI16("Surface sectors:\r\n", ANSI.ANSIColor_16.BrightCyan));

			char lineChar = 'A';
			for (int y = 0; y < Planet.PlanetSector.GetLength(1); y++) {

				if (y == 0) {
					sBuilder.Append("  ");
					sBuilder.Append(ANSI.ColorANSI16(ANSI.ANSIColor_16.BrightCyan));
					for (int x = 0; x < Planet.PlanetSector.GetLength(0); x++) {
						sBuilder.AppendFormat("  {0,-2} ", x + 1);
					}
					sBuilder.Append(ANSI.ClearFormat());
					sBuilder.AppendFormat("\r\n");
				}

				sBuilder.Append(ANSI.SetColorANSI16(string.Format("{0,-2}", lineChar), ANSI.ANSIColor_16.BrightCyan)); lineChar++;
				for (int x = 0; x < Planet.PlanetSector.GetLength(0); x++) {
					GetPlanetSectorString(Planet.PlanetSector[x, y], sBuilder);
				}

				sBuilder.AppendFormat("\r\n");
			}

			sBuilder.AppendFormat(ANSI.SetColorANSI16("\r\nSpace Ports / Stations: ", ANSI.ANSIColor_16.BrightCyan));
			sBuilder.AppendFormat("{0}\r\n", (Planet.SpacePort.Count > 0) ? "" : ANSI.SetColorANSI16("None", ANSI.ANSIColor_16.Cyan));
			foreach (var s in Planet.SpacePort) {
				sBuilder.AppendFormat("{3}[{4}{0}{3}] {4}{1,-30}{3} Sector: {4}{2}{5}\r\n", 
					s.Id, s.Name, s.PlanetSector,
					ANSI.ColorANSI16(ANSI.ANSIColor_16.Cyan),
					ANSI.ColorANSI16(ANSI.ANSIColor_16.BrightCyan),
					ANSI.ClearFormat()
				);
			}

			return sBuilder.ToString();

		}

	}
}
