using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;
using System.Text;

namespace GalaxyE.DB {
	class Database {

		private static string DBRoot = "./DB";

		public static bool Exists(string entity, string objName) {
			string path = Path.Combine(DBRoot, entity.ToUpper());
			if (Directory.Exists(path)) {
				string filename = Path.Combine(path, string.Format("{0}.bin", objName.ToUpper()));
				return File.Exists(filename);
			}
			return false;
		}

		public static T GetObject<T>(string entity, string objName) where T : ISerializable {

			string path = Path.Combine(DBRoot, entity.ToUpper());
			if (Directory.Exists(path)) {
				string filename = Path.Combine(path, string.Format("{0}.bin", objName.ToUpper()));
				if (File.Exists(filename)) {
					return FlatFileDB.Instance.GetObject<T>(filename);
				}
			}

			return default(T);
		}

		public static void Persist(string entity, string objName, object data) {

			string path = Path.Combine(DBRoot, entity.ToUpper());

			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);

			string filename = Path.Combine(path, string.Format("{0}.bin", objName.ToUpper()));
			FlatFileDB.Instance.PercistObject(filename, data);

		}



	}
}
