using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TelnetServer.Game {
	public class ioCity {


		string[] buildings = new string[] {
		    "b1.png.MII","b2.png.MII","b3.png.MII","b4.png.MII","b5.png.MII",
		    "b6.png.MII","b7.png.MII","b8.png.MII","b9.png.MII","b10.png.MII"
		};

		string[] factoriesA = new string[] { "f1.png.MII" };
		string[] factoriesB = new string[] { "f2.png.MII" };
		string[] sPorts = new string[] { "p1.png.MII" };

		//TextCanvas mainCanvas = new TextCanvas(21 * 3, 8 * 3, TextCanvas.NumOfColorsEnum.XTERM256);
		TextCanvas mainCanvas = new TextCanvas(120, 8 * 3, TextCanvas.NumOfColorsEnum.XTERM256);

		string spec = "Bf--B\nBP B|B\nB   B\n";

		public TextCanvas Test() {

			byte wallColor = 239;
			byte topColor = 241;

			List<char[]> def = new List<char[]>(); 

			StringReader sr = new StringReader(spec);
			string line;

			while ((line = sr.ReadLine()) != null) { def.Add(line.ToCharArray()); }

			Random rand = new Random();
			string baseDir = "./Messages/Citys";


			string tileFilename = Path.Combine(baseDir, "tile3.png.MII");
			var tile = Messages.Message.ReadMII2File(tileFilename);
			//tile.WriteTile(mainCanvas);

			mainCanvas.FillRectangle(0, 0, mainCanvas.Width, mainCanvas.Height, 234);

			//equalise
			int maxLength = 0;
			for (int y = 0; y < def.Count; y++) {
				if (def[y].Length > maxLength) maxLength = def[y].Length;
			}

			for (int y = 0; y < def.Count; y++) {
				if (def[y].Length < maxLength) {
					char[] n = new char[maxLength];
					for (int x = 0; x < maxLength; x++)
						n[x] = ' ';

					Array.Copy(def[y], n, def[y].Length);
					def[y] = n;
				}
			}

			for (int y = 0; y < def.Count; y++) {
				for (int x = 0; x < def[y].Length; x++) {
					char c = def[y][x];
					if (c == 'B') {
						string filename = Path.Combine(baseDir, buildings[rand.Next(0, buildings.Length - 1)]);
						var buildind = Messages.Message.ReadMII2File(filename);
						buildind.Write(x * 21, y * 8, mainCanvas);
					} else if (c == 'P') {
						//Give space for the port [2x2]
						if (x + 1 < def[y].Length) { def[y][x + 1] = '+'; }
						if (y + 1 < def.Count) {
							if (x < def[y + 1].Length) { def[y + 1][x] = '_'; }
							if (x + 1 < def[y + 1].Length) { def[y + 1][x + 1] = '_'; }
						}
						string filename = Path.Combine(baseDir, sPorts[0]);
						var buildind = Messages.Message.ReadMII2File(filename);
						buildind.Write(x * 21, y * 8, mainCanvas);
					} else if (c == 'F') {
						//Give space for the factory [2x2]
						if (x + 1 < def[y].Length) { def[y][x + 1] = '+'; }
						if (y + 1 < def.Count) {
							if (x < def[y + 1].Length) { def[y + 1][x] = '_'; }
							if (x + 1 < def[y + 1].Length) { def[y + 1][x + 1] = '_'; }
						}

						string filename = Path.Combine(baseDir, factoriesB[rand.Next(0, factoriesB.Length -1)]);
						var buildind = Messages.Message.ReadMII2File(filename);
						buildind.Write(x * 21, y * 8, mainCanvas);
					} else if (c == 'f') {
						//Give space for the factory [1x2]
						if (x + 1 < def[y].Length) { def[y][x + 1] = '+'; }
						string filename = Path.Combine(baseDir, factoriesA[rand.Next(0, factoriesB.Length - 1)]);
						var buildind = Messages.Message.ReadMII2File(filename);
						buildind.Write(x * 21, y * 8, mainCanvas);
					}
				}
			}

			for (int y = 0; y < def.Count; y++) {
				for (int x = 0; x < def[y].Length; x++) {
					char c = def[y][x];
					if (c != ' ' && c != 'F' && c != 'P' && c != 'f' && c != '_') {

						bool makeTunnelRight = x + 1 < def[y].Length;
						bool makeTunnelDown = y + 1 < def.Count;
						bool makeTunnelUp = y > 0;

						if (makeTunnelRight) makeTunnelRight = (
							def[y][x + 1] != ' ' && def[y][x + 1] != '_' &&
							def[y][x + 1] != '|'
						);

						if (makeTunnelDown) makeTunnelDown = (
							def[y + 1][x] != ' ' && def[y + 1][x] != '_'
						);
						
						if (makeTunnelUp) makeTunnelUp = (
							def[y - 1][x] != ' ' && def[y - 1][x] != '_' && c != '|'
						);

						if (makeTunnelRight) {
							if (c != '|') {
								if (makeTunnelDown)
									mainCanvas.DrawString(x * 21 + 21 - 3, y * 8 + 4, "▀▀█▀", topColor, wallColor);
								else
									mainCanvas.DrawString(x * 21 + 21 - 3, y * 8 + 4, "▀▀▀▀", topColor, wallColor);
							} else {
								if (makeTunnelDown)
									mainCanvas.DrawString(x * 21 + 21 - 1, y * 8 + 4, "█▀", topColor, wallColor);
								else
									mainCanvas.DrawString(x * 21 + 21 - 1, y * 8 + 4, "▀▀", topColor, wallColor);
							}

						} else if (makeTunnelUp) {
							mainCanvas.DrawString(x * 21 + 21 - 3, y * 8 + 4, "▀▀▀", topColor, wallColor);
							//mainCanvas.DrawString(x * 21 + 21 - 3, y * 8 + 5, "███", wallColor, 0);
						} else if (makeTunnelDown) {
							if (c == '|')
								mainCanvas.DrawString(x * 21 + 21 - 1, y * 8 + 4, "█", topColor, wallColor);
							else
								mainCanvas.DrawString(x * 21 + 21 - 3, y * 8 + 4, "▀▀█", topColor, wallColor);
						}

						if (makeTunnelDown) {
							for (int i = 0; i < 8; i++)
								mainCanvas.DrawString(x * 21 + 22 - 2, y * 8 + 5 + i, "█", topColor, 0);
						}

						if (makeTunnelRight && def[y][x + 1] == '-') {
							mainCanvas.DrawString(x * 21 + 22, y * 8 + 4, "▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀", topColor, wallColor);
						}
					}
				}
			}

			return mainCanvas;
		}
	}
}
