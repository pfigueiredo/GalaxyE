using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaxyE.DB;
using System.IO;

namespace GalaxyE.Galaxy.Zones {
	public class SystemObjectSector {

		public string Definition { get; set; }
		public SystemBody SystemBody { get; set; }

		public SystemObjectSector(SystemBody body, string bSectorId) {
			string extentionFilename = string.Format("{0}.txt", bSectorId);
			if (ObjectExtender.Instance.HasExtention(this, extentionFilename)) {
				string data = ObjectExtender.Instance.GetExtentionData(this, extentionFilename);
				this.Definition = data;
				ParseData(data, body);
			}
		}

		public class Building {
			public int X { get; set; }
			public int Y { get; set; }
			public string Code { get; set; }
			public string Def { get; set; }
			int Seed { get; set; }

			public Building(int x, int y, string def, string code, int seed) {
				
				this.X = x;
				this.Y = y;
				this.Def = def;
				this.Code = code;
				this.Seed = seed;
				
			}
			
		}

		private void ParseData(string spec, SystemBody body) {

			Random rand = new Random((int)body.SystemId.Id);
			

			List<char[]> def = new List<char[]>();

			StringReader sr = new StringReader(spec);
			string line;

			while ((line = sr.ReadLine()) != null) { def.Add(line.ToCharArray()); }

			//equalise
			int maxLength = 0;
			for (int y = 0; y < def.Count; y++) {
				if (def[y].Length > maxLength) maxLength = def[y].Length;
			}

			for (int y = 0; y < def.Count; y++) {
				for (int x = 0; x < def[y].Length; x++) {
					char c = def[y][x];
					if (c == 'B') {

						#region void
						//string filename = Path.Combine(baseDir, buildings[rand.Next(0, buildings.Length - 1)]);
						//var buildind = Messages.Message.ReadMII2File(filename);
						//buildind.Write(x * 21, y * 8, mainCanvas);
						#endregion

					} else if (c == 'P') {
						//Give space for the port [2x2]
						if (x + 1 < def[y].Length) { def[y][x + 1] = '+'; }
						if (y + 1 < def.Count) {
							if (x < def[y + 1].Length) { def[y + 1][x] = '_'; }
							if (x + 1 < def[y + 1].Length) { def[y + 1][x + 1] = '_'; }
						}

						#region void
						//string filename = Path.Combine(baseDir, sPorts[0]);
						//var buildind = Messages.Message.ReadMII2File(filename);
						//buildind.Write(x * 21, y * 8, mainCanvas);
						#endregion

					} else if (c == 'F') {
						//Give space for the factory [2x2]
						if (x + 1 < def[y].Length) { def[y][x + 1] = '+'; }
						if (y + 1 < def.Count) {
							if (x < def[y + 1].Length) { def[y + 1][x] = '_'; }
							if (x + 1 < def[y + 1].Length) { def[y + 1][x + 1] = '_'; }
						}

						#region void 
						//string filename = Path.Combine(baseDir, factoriesB[rand.Next(0, factoriesB.Length - 1)]);
						//var buildind = Messages.Message.ReadMII2File(filename);
						//buildind.Write(x * 21, y * 8, mainCanvas);
						#endregion

					} else if (c == 'f') {
						//Give space for the factory [1x2]
						if (x + 1 < def[y].Length) { def[y][x + 1] = '+'; }

						#region void
						//string filename = Path.Combine(baseDir, factoriesA[rand.Next(0, factoriesB.Length - 1)]);
						//var buildind = Messages.Message.ReadMII2File(filename);
						//buildind.Write(x * 21, y * 8, mainCanvas);
						#endregion

					}
				}
			}
		}
	}
}
