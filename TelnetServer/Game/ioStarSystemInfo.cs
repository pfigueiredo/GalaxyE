using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaxyE.Galaxy;
using System.IO;

namespace TelnetServer.Game {
	class ioStarSystemInfo {


		public StarSystem StarSystem { get; private set; }
		public TextCanvas Canvas { get; private set; }
		private int screenWidth;

		public ioStarSystemInfo(StarSystem system) : this (system, 79) { }

		public ioStarSystemInfo(StarSystem system, int screenWidth) {
			this.StarSystem = system;
			this.screenWidth = (screenWidth > 90) ? 90 : screenWidth;
			Init();
		}

		private void Init() {

			string baseDir = "./Messages/Stars";

			//get the star ansi image;
			string filename = Path.Combine(baseDir, string.Format("{0}.MII", this.StarSystem.SpectralClass));
			var starFile = Messages.Message.ReadMII2File(filename);

			TextCanvas canvas = new TextCanvas(screenWidth, 25, TextCanvas.NumOfColorsEnum.XTERM256);
			starFile.Write(0, 0, canvas);

			canvas.FillRectangle(41, 4, 79 - 41, 10, 0);
			canvas.DrawString(41, 4, string.Format("{0} System", StarSystem.Name), (byte)ANSI.ANSIColor_16.Cyan, 0);
			canvas.DrawString(41, 5, string.Format("Stable system with {0} major bodies", StarSystem.NumberOfBodies + 1), (byte)ANSI.ANSIColor_16.Cyan, 0);
			canvas.DrawString(41, 7, string.Format("Main Star: {0}", StarSystem.Name), (byte)ANSI.ANSIColor_16.Cyan, 0);
			canvas.DrawString(41, 8, string.Format("{1} : {2}", StarSystem.Name, StarSystem.StarType, StarSystem.Desc), (byte)ANSI.ANSIColor_16.Cyan, 0);
			canvas.DrawString(41, 9, string.Format("Spectral Class: {0}", StarSystem.SpectralClass), (byte)ANSI.ANSIColor_16.Cyan, 0);
			canvas.DrawString(41, 10, string.Format("Luminosity: {0}", StarSystem.LuminosityClass), (byte)ANSI.ANSIColor_16.Cyan, 0);
			canvas.DrawString(41, 11, string.Format("Magnitude: {0}", StarSystem.Magnitude), (byte)ANSI.ANSIColor_16.Cyan, 0);
			canvas.DrawString(41, 12, string.Format("Size: ~{0:0.00}", StarSystem.Size), (byte)ANSI.ANSIColor_16.Cyan, 0);
			canvas.DrawString(41, 13, string.Format("Mass: ~{0:0.00}", StarSystem.Mass), (byte)ANSI.ANSIColor_16.Cyan, 0);
			canvas.DrawString(41, 14, string.Format("Habitable Zone ~{0:0.00}Au", StarSystem.HZone), (byte)ANSI.ANSIColor_16.Cyan, 0);

			//int boxX = 40, boxY = 0;

			//canvas.FillRectangle(boxX, boxY, 79 - boxX, 30 - boxY, 0);
			//canvas.DrawRectangle(boxX, boxY, 79 - boxX, 30 - boxY, 250);

			this.Canvas = canvas;

		}







	}
}
