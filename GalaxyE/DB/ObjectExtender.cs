using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaxyE.Galaxy;
using System.IO;
using GalaxyE.Galaxy.Zones;

namespace GalaxyE.DB {
	public class ObjectExtender {

		#region Instance
		private static ObjectExtender instance = null;
		private static object instanceLocker = new object();

		public static ObjectExtender Instance {
			get {
				if (instance == null) {
					lock (instanceLocker) {
						if (instance == null)
							instance = new ObjectExtender();
					}
				}

				return instance;
			}

		}
		#endregion

		private List<Coords> sectors = null;

		private ObjectExtender() {
			this.sectors = GetSectorsWithExtentions();
		}

		private Dictionary<string, string> ReadProperties(string filename) {

			Dictionary<string, string> properties = new Dictionary<string, string>();
			string currentProperty = null;
			StringBuilder currentValue = null;

			string data = FlatFileDB.Instance.ReadFile(filename);
			if (data != null) {
				StringReader sr = new StringReader(data);
				string line;

				while ((line = sr.ReadLine()) != null) {
					line = line.Trim();
					if (!line.StartsWith("#")) {
						if (!line.StartsWith("@")) {
							if (line.IndexOf('=') > 0) {
								string[] pTokens = line.Split(new char[] { '=' }, 2, StringSplitOptions.None);
								if (pTokens.Length > 1 && pTokens[0].Length > 1) {
									string p = pTokens[0].Substring(1).TrimEnd();
									string v = pTokens[1].Trim();
									if (!properties.ContainsKey(p))
										properties.Add(p, v);
									else
										properties[p] = v;
								}
								currentProperty = null;
							}

							if (currentProperty != null) {
								if (!properties.ContainsKey(currentProperty))
									properties.Add(currentProperty, currentValue.ToString());
								else
									properties[currentProperty] = currentValue.ToString();
							}

							currentProperty = line.Substring(1);
							currentValue = new StringBuilder();
						} else if (currentProperty != null) {
							currentValue.AppendLine(line);
						}
					} else
						currentProperty = null;
				}
			}

			return properties;

		}

		public void Extend(SystemBody body) {
			Coords sectorCoords = new Coords(body.StarSystem.Coords.SectorIndexX, body.StarSystem.Coords.SectorIndexY);
			if (sectors.Contains(sectorCoords)) {
				string baseDir = GetExtentionDir(body);

				Dictionary<string, string> properties = new Dictionary<string, string>();

				if (Directory.Exists(baseDir)) {
					string filename = Path.Combine(baseDir, "properties.txt");
					properties = ReadProperties(filename);

					IExtendableObject obj = body as IExtendableObject;
					if (obj != null) {
						foreach (var key in properties.Keys) {
							obj.ExtendedProperties.SetValue(key.ToLower(), properties[key]);
						}
					}

				}
			}
		}

		public void Extend(StarSystem system) {
			Coords sectorCoords = new Coords(system.Coords.SectorIndexX, system.Coords.SectorIndexY);
			if (sectors.Contains(sectorCoords)) {
				string baseDir = string.Format(
					"./Data/Sector/s.{0}.{1}/Systems/s.{2}",
					sectorCoords.SectorIndexX, sectorCoords.SectorIndexY,
					system.SysNum
				);

				Dictionary<string, string> properties = new Dictionary<string, string>();	

				if (Directory.Exists(baseDir)) {
					string filename = Path.Combine(baseDir, "properties.txt");
					properties = ReadProperties(filename);

					IExtendableObject obj = system as IExtendableObject;
					if (obj != null) {
						foreach (var key in properties.Keys) {
							obj.ExtendedProperties.SetValue(key.ToLower(), properties[key]);
						}
					}

				}
			}
		}

		public string MakeExtentionDirs(SystemBody body) {
			string baseDir = MakeExtentionDirs(body.StarSystem);
			string bodyDirPart = string.Format("b.{0}", body.SystemBodyId);
			string bodyDir = Path.Combine(baseDir, bodyDirPart);
			if (!Directory.Exists(bodyDir))
				Directory.CreateDirectory(bodyDir);

			return bodyDir;
		}

		public string MakeExtentionDirs(StarSystem system) {
			string baseDir = "./Data/Sector";
			string sectorDirName = string.Format("s.{0}.{1}", system.Coords.SectorIndexX, system.Coords.SectorIndexY);
			string sectorDir = Path.Combine(baseDir, sectorDirName);
			if (!Directory.Exists(sectorDir))
				Directory.CreateDirectory(sectorDir);

			string systemDirPart = string.Format("Systems/s.{0}", system.SysNum);
			string systemDir = Path.Combine(sectorDir, systemDirPart);

			if (!Directory.Exists(systemDir))
				Directory.CreateDirectory(systemDir);

			Coords sectorCoords = new Coords(system.Coords.SectorIndexX, system.Coords.SectorIndexY);
			if (!sectors.Contains(sectorCoords))
				sectors.Add(sectorCoords);

			return systemDir;

		}

		public string GetExtentionData(SystemBody body, string extention) {
			string filename = Path.Combine(GetExtentionDir(body), extention);
			return FlatFileDB.Instance.ReadFile(filename);
		}

		public string GetExtentionData(StarSystem system, string extention) {
			string filename = Path.Combine(GetExtentionDir(system), extention);
			return FlatFileDB.Instance.ReadFile(filename);
		}

		public string GetExtentionData(SystemObjectSector sector, string extention) {
			string filename = Path.Combine(GetExtentionDir(sector), extention);
			return FlatFileDB.Instance.ReadFile(filename);
		}

		public bool HasExtention(SystemBody body, string extention) {
			string filename = Path.Combine(GetExtentionDir(body), extention);
			return File.Exists(filename);
		}

		public bool HasExtention(StarSystem system, string extention) {
			string filename = Path.Combine(GetExtentionDir(system), extention);
			return File.Exists(filename);
		}

		public bool HasExtention(SystemObjectSector sector, string extention) {
			string filename = Path.Combine(GetExtentionDir(sector), extention);
			return File.Exists(filename);
		}

		public string GetExtentionDir(SystemBody body) {
			string baseDir = GetExtentionDir(body.StarSystem);
			string bodyDirPart = string.Format("b.{0}", body.SystemBodyId);
			string bodyDir = Path.Combine(baseDir, bodyDirPart);
			return bodyDir;
		}

		public string GetExtentionDir(SystemObjectSector sector) {
			string baseDir = GetExtentionDir(sector.SystemBody);
			string bodyDirPart = "Sectors";
			string bodyDir = Path.Combine(baseDir, bodyDirPart);
			return bodyDir;
		}

		public string GetExtentionDir(StarSystem system) {
			string baseDir = "./Data/Sector";
			string sectorDirName = string.Format("s.{0}.{1}", system.Coords.SectorIndexX, system.Coords.SectorIndexY);
			string sectorDir = Path.Combine(baseDir, sectorDirName);
			string systemDirPart = string.Format("Systems/s.{0}", system.SysNum);
			string systemDir = Path.Combine(sectorDir, systemDirPart);
			return systemDir;
		}

		private List<Coords> GetSectorsWithExtentions() {
			string baseDir = "./Data/Sector";
			List<Coords> returnList = new List<Coords>();
			DirectoryInfo dInfo = new DirectoryInfo(baseDir);
			if (dInfo.Exists) {
				foreach (var subDir in dInfo.GetDirectories()) {
					if (subDir.Name.StartsWith("s.")) {
						string[] tokens = subDir.Name.Split('.');
						if (tokens.Length == 3) {
							string strX = tokens[1];
							string strY = tokens[2];
							uint indexX, indexY;
							
							bool ok = uint.TryParse(strX, out indexX);
							ok &= uint.TryParse(strY, out indexY);

							if (ok) {
								Coords c = new Coords(indexX, indexY);
								returnList.Add(c);
							}
						}
					}
				}
			}

			return returnList;
		}

	}
}
