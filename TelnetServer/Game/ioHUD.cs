using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaxyE.Managers;
using GalaxyE.Galaxy;
using GalaxyE.Players;

namespace TelnetServer.Game {
	class ioHUD {

		public int Width { get; set; }
		public int Height { get; set; }
		public ioPlayer Player { get; set; }

		public ioHUD(int width, int height, ioPlayer player) {
			this.Width = width;
			this.Height = height;
			this.Player = player;
		}

		public class ObjInfo {
			public bool IsPlayer { get; set; }
			public bool IsEnemy { get; set; }
			public bool IsTarget { get; set; }
			public Double Distance { get; set; }
			public string Display { get; set; }
			public Double X { get; set; }
			public Double Y { get; set; }
		}

		public string GetHud() {

			List<SectorObject> secObjects = new List<SectorObject>();
			List<ObjInfo> objects = new List<ObjInfo>();

			GameManager manager = GameManager.Instance;

			//int smallHudWidth = 30, smallHudHeight = 15;
			int smallHudWidth = 20, smallHudHeight = 12;

			TextCanvas closeDistHUDCanvas = new TextCanvas(smallHudWidth, smallHudHeight, TextCanvas.NumOfColorsEnum.XTERM256);
			closeDistHUDCanvas.DrawGrid(0, 0, smallHudWidth, smallHudHeight, (byte)234);
			closeDistHUDCanvas.DrawRectangle(0, 0, smallHudWidth, smallHudHeight, (byte)ANSI.ANSIColor_8.Cyan);

			TextCanvas nearbyShipsHUDCanvas = new TextCanvas(smallHudWidth, smallHudHeight, TextCanvas.NumOfColorsEnum.XTERM256);
			nearbyShipsHUDCanvas.DrawGrid(0, 0, smallHudWidth, smallHudHeight, (byte)234);
			nearbyShipsHUDCanvas.DrawRectangle(0, 0, smallHudWidth, smallHudHeight, (byte)ANSI.ANSIColor_8.Cyan);

			TextCanvas closeDistHUDCanvas_Target = new TextCanvas(smallHudWidth, smallHudHeight, TextCanvas.NumOfColorsEnum.XTERM256);
			closeDistHUDCanvas_Target.DrawGrid(0, 0, smallHudWidth, smallHudHeight, (byte)234);
			closeDistHUDCanvas_Target.DrawRectangle(0, 0, smallHudWidth, smallHudHeight, (byte)ANSI.ANSIColor_8.Cyan);

			TextCanvas nearbyEShipsHUDCanvas = new TextCanvas(smallHudWidth, smallHudHeight, TextCanvas.NumOfColorsEnum.XTERM256);
			nearbyEShipsHUDCanvas.DrawGrid(0, 0, smallHudWidth, smallHudHeight, (byte)234);
			nearbyEShipsHUDCanvas.DrawRectangle(0, 0, smallHudWidth, smallHudHeight, (byte)ANSI.ANSIColor_8.Cyan);


			TextCanvas canvas = new TextCanvas(Width - 5, smallHudHeight, TextCanvas.NumOfColorsEnum.XTERM256);
			//canvas.DrawRectangle(0, 0, Width -5, Height -5, (byte)ANSI.ANSIColor_8.Cyan);
			//canvas.DrawString(Width / 2, Height / 2, "▲", (byte)ANSI.ANSIColor_16.Green, (byte)ANSI.ANSIColor_16.Black);

			StarSystem system = this.Player.Player.Location.StarSystem;
			SpaceShip ship = this.Player.Player.SpaceShip;

			if (system == null) return string.Empty;

			var sysM = manager.StarSystemManager.GetStarSystemArea(system.Coords);
			
			secObjects.AddRange(system.SystemBodies);
			secObjects.AddRange(sysM.GetShipsInSystem().Cast<SectorObject>());
			secObjects.Add(system);
			if (ship.Target != null) secObjects.Add(ship.Target);
			secObjects.Add(ship);

			

			double minDist = Double.MaxValue;
			double minDistShips = Double.MaxValue;

			double minDist_Target = Double.MaxValue;
			double minDistEShips = Double.MaxValue;

			foreach (SectorObject secObject in secObjects) {
				double dist = secObject.SystemLocation.GetInSystemDistance(ship.SystemLocation);
				double dist_Target = 
					(ship.Target != null) ?
						secObject.SystemLocation.GetInSystemDistance(ship.Target.SystemLocation)
						: 0;

				if (dist < minDist && dist > 0) minDist = dist;
				if (dist_Target < minDist_Target && dist_Target > 0) minDist_Target = dist_Target;

				SpaceShip shipObj = secObject as SpaceShip;
				SystemBody bodyObj = secObject as SystemBody;
				StarSystem sysObj = secObject as StarSystem;

				ObjInfo info = new ObjInfo();
				info.X = secObject.SystemLocation.X;
				info.Y = secObject.SystemLocation.Y;
				info.Distance = dist;

				if (secObject == ship)
					info.IsPlayer = true;

				if (secObject == ship.Target)
					info.IsTarget = true;

				if (shipObj != null) {
					info.Display = "▪";
					if (dist < minDistShips && dist > 0) minDistShips = dist;

					if (info.IsEnemy) {
						if (dist < minDistEShips && dist > 0) minDistEShips = dist;
					}

				} else if (bodyObj != null) {
					info.Display = "●";
				} else if (sysObj != null) {
					info.Display = "☼";
				}

				objects.Add(info);

			}

			if (minDist_Target == Double.MaxValue) minDist_Target = 0;
			if (minDistShips == Double.MaxValue) minDistShips = 0;
			if (minDistEShips == Double.MaxValue) minDistEShips = 0;

			//write close distance HUD
			double hudRange = minDist * (smallHudHeight / 2); // 149598000 / 2;
			double zX = ship.SystemLocation.X;
			double zY = ship.SystemLocation.Y;

			#region Close Distance HUD

			foreach (var obj in objects) {
				if (obj.Distance < (hudRange)) {
					double x = obj.X - zX, y = obj.Y - zY;

					int cX = (int)(x * smallHudWidth / hudRange + (smallHudWidth / 2));
					int cY = (int)(y * smallHudHeight / hudRange + (smallHudHeight / 2));

					byte foreColor = (byte)ANSI.ANSIColor_16.Cyan;

					if (obj.IsPlayer)
						foreColor = (byte)ANSI.ANSIColor_16.Green;

					if (obj.IsEnemy)
						foreColor = (byte)ANSI.ANSIColor_16.Red;

					if (obj.IsTarget)
						foreColor = (byte)ANSI.ANSIColor_16.DarkYellow;

					closeDistHUDCanvas.DrawString(cX, cY, obj.Display,
						foreColor, (byte)ANSI.ANSIColor_16.Black);

				}
			}

			closeDistHUDCanvas.DrawString(1, 1, string.Format("{0}", SystemLocation.GetDistanceDesc(hudRange)), 6, 0);

			#endregion

			#region Nearby Ships HUD

			hudRange = minDistShips * (smallHudHeight / 2);

			foreach (var obj in objects) {
				if (obj.Distance < (hudRange)) {
					double x = obj.X - zX, y = obj.Y - zY;

					int cX = (int)(x * smallHudWidth / hudRange + (smallHudWidth / 2));
					int cY = (int)(y * smallHudHeight / hudRange + (smallHudHeight / 2));

					byte foreColor = (byte)ANSI.ANSIColor_16.Cyan;

					if (obj.IsPlayer)
						foreColor = (byte)ANSI.ANSIColor_16.Green;

					if (obj.IsEnemy)
						foreColor = (byte)ANSI.ANSIColor_16.Red;

					if (obj.IsTarget)
						foreColor = (byte)ANSI.ANSIColor_16.DarkYellow;

					nearbyShipsHUDCanvas.DrawString(cX, cY, obj.Display,
						foreColor, (byte)ANSI.ANSIColor_16.Black);

				}
			}

			nearbyShipsHUDCanvas.DrawString(1, 1, string.Format("{0}", SystemLocation.GetDistanceDesc(hudRange)), 6, 0);

			#endregion

			#region Nearby Enemy Ships HUD

			hudRange = minDistEShips * (smallHudHeight / 2);

			foreach (var obj in objects) {
				if (obj.Distance < (hudRange) && obj.IsEnemy) {
					double x = obj.X - zX, y = obj.Y - zY;

					int cX = (int)(x * smallHudWidth / hudRange + (smallHudWidth / 2));
					int cY = (int)(y * smallHudHeight / hudRange + (smallHudHeight / 2));

					byte foreColor = (byte)ANSI.ANSIColor_16.Cyan;

					if (obj.IsPlayer)
						foreColor = (byte)ANSI.ANSIColor_16.Green;

					if (obj.IsEnemy)
						foreColor = (byte)ANSI.ANSIColor_16.Red;

					if (obj.IsTarget)
						foreColor = (byte)ANSI.ANSIColor_16.DarkYellow;

					nearbyEShipsHUDCanvas.DrawString(cX, cY, obj.Display,
						foreColor, (byte)ANSI.ANSIColor_16.Black);

				}
			}

			nearbyEShipsHUDCanvas.DrawString(1, 1, string.Format("{0}", SystemLocation.GetDistanceDesc(hudRange)), 6, 0);

			#endregion

			#region Target Close Distance HUD

			hudRange = minDist_Target * (smallHudHeight / 2); // 149598000 / 2;

			if (ship.Target != null) {
				zX = ship.Target.SystemLocation.X;
				zY = ship.Target.SystemLocation.Y;
			}

			foreach (var obj in objects) {
				if (obj.Distance < (hudRange)) {
					double x = obj.X - zX, y = obj.Y - zY;

					int cX = (int)(x * smallHudWidth / hudRange + (smallHudWidth / 2));
					int cY = (int)(y * smallHudHeight / hudRange + (smallHudHeight / 2));

					byte foreColor = (byte)ANSI.ANSIColor_16.Cyan;

					if (obj.IsPlayer)
						foreColor = (byte)ANSI.ANSIColor_16.Green;

					if (obj.IsEnemy)
						foreColor = (byte)ANSI.ANSIColor_16.Red;

					if (obj.IsTarget)
						foreColor = (byte)ANSI.ANSIColor_16.DarkYellow;

					closeDistHUDCanvas_Target.DrawString(cX, cY, obj.Display,
						foreColor, (byte)ANSI.ANSIColor_16.Black);

				}
			}

			closeDistHUDCanvas_Target.DrawString(1, 1, string.Format("{0}", SystemLocation.GetDistanceDesc(hudRange)), 6, 0);

			#endregion

			

			canvas.DrawRawData(0, 0, closeDistHUDCanvas.GetRawData());
			canvas.DrawRawData(smallHudWidth, 0, nearbyShipsHUDCanvas.GetRawData());

			canvas.DrawRawData(smallHudWidth * 2, 0, closeDistHUDCanvas_Target.GetRawData());
			canvas.DrawRawData(smallHudWidth * 3, 0, nearbyEShipsHUDCanvas.GetRawData());

			long baseX, baseY;

			baseX = (long)(ship.SystemLocation.X / minDist);
			baseY = (long)(ship.SystemLocation.Y / minDist);


			return canvas.ToString();
		}


	}
}
