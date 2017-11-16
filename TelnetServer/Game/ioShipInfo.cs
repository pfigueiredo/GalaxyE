using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaxyE.Players;
using System.IO;
using GalaxyE.Galaxy;

namespace TelnetServer.Game {
	public class ioShipInfo {

		public SpaceShip SpaceShip { get; private set; }
		public TextCanvas Canvas { get; private set; }
		private int screenWidth;

		public ioShipInfo(SpaceShip ship) : this (ship, 79) { }

		public ioShipInfo(SpaceShip ship, int screenWidth) {
			this.SpaceShip = ship;
			this.screenWidth = (screenWidth > 90) ? 90 : screenWidth;
			Init();
		}

		public static string GetStatusString(SpaceShip ship) {
			string baseStatus = "► {2}: H {0} E {1}";

			ANSI.ANSIColor_16 energyColor;
			ANSI.ANSIColor_16 hullColor;

			if (ship.Hull > 50) hullColor = ANSI.ANSIColor_16.Green;
			else if (ship.Hull > 25) hullColor = ANSI.ANSIColor_16.DarkYellow;
			else hullColor = ANSI.ANSIColor_16.Red;

			if (ship.Energy > 50) energyColor = ANSI.ANSIColor_16.Green;
			else if (ship.Energy > 25) energyColor = ANSI.ANSIColor_16.DarkYellow;
			else energyColor = ANSI.ANSIColor_16.Red;

			string hullValue = 
				ANSI.ColorANSI16(hullColor) +
				string.Empty.PadLeft(ship.Hull * 10 / 100, '■').PadLeft(10) +
				ANSI.ClearFormat();

			string energyValue = 
				ANSI.ColorANSI16(energyColor) +
				string.Empty.PadLeft(ship.Energy * 10 / 100, '■').PadLeft(10) +
				ANSI.ClearFormat();

			return string.Format(baseStatus, hullValue, energyValue, ship.Code);
		}

		private void Init() {

			string baseDir = "./Messages/Ships";

			//get the star ansi image;
			string filename = Path.Combine(baseDir, string.Format("{0}.png.MII", this.SpaceShip.MKIIFile));
			var shipFile = Messages.Message.ReadMII2File(filename);

			TextCanvas canvas = new TextCanvas(screenWidth, 18, TextCanvas.NumOfColorsEnum.XTERM256);
			shipFile.Write(3, 1, canvas);

			Sector sector = SpaceShip.Coords.GetSector();
			StarSystem starSystem = null;
			foreach (var sys in sector.Systems) {
				if (sys.SystemId == SpaceShip.SystemId) {
					starSystem = sys;
					break;
				}
			}

			canvas.DrawString(20, 1, string.Format("Model: {0} - Reg Code: {1}",
				(SpaceShip.Name == null || SpaceShip.Name.Trim() == string.Empty) ? "Unknown" : SpaceShip.Name, 
				SpaceShip.Code), 250, 0);

			canvas.DrawString(20, 2, string.Format("Sector: {0} - System {1} - Target: {2}", 
				SpaceShip.Coords.GetSector(), 
				(starSystem != null) ? starSystem.Name : "None",
				(SpaceShip.Target != null) ? SpaceShip.Target.ToString() : "No target"
				), 250, 0);
			canvas.DrawString(20, 5, string.Format("Speed Capability: {0}", SpaceShip.Speed), 250, 0);
			canvas.DrawString(20, 6, string.Format("Mass: {0} - Hull {1}", SpaceShip.Mass, SpaceShip.Hull), 250, 0);
			canvas.DrawString(20, 7, string.Format("Main Thruster: {0} - Retro Thruster: {1}", SpaceShip.MainThrusterAccel, SpaceShip.RetroThrusterAccel), 250, 0);
			canvas.DrawString(20, 8, string.Format("Cargo Bay Cap: {0} ", SpaceShip.CargoBay.Capacity), 250, 0);
			canvas.DrawString(20, 9, string.Format("Energy: {0} - Generator {1}", SpaceShip.Energy, SpaceShip.Generator.Energy), 250, 0);
			canvas.DrawString(20, 10, string.Format("Gun Moutings: {0} - Guns {1}", SpaceShip.GunMoutings, SpaceShip.Lasers.Length), 250, 0);
			canvas.DrawString(20, 11, string.Format("Missile Pylons: {0} - Launchers {1}", SpaceShip.MissilePylons, SpaceShip.MissileLaunchers.Length), 250, 0);

			this.Canvas = canvas;
			
		}

	}
}
