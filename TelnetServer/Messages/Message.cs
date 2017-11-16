using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TelnetServer.Game;

namespace TelnetServer.Messages {
	class Message {

		private static Dictionary<string, string> _messagesDef;

		static Message() {
			_messagesDef = new Dictionary<string, string>();
			_messagesDef.Add("gamebanner", "StartBanner1.MII");
			_messagesDef.Add("galaxymap", "GalaxyMap.ans");
			_messagesDef.Add("tt", "Start2_80.ans");
		}

		public static List<string> _RealAllCities() {
			List<string> returnList = new List<string>();
			DirectoryInfo dInfo = new DirectoryInfo("./Messages/Citys");
			if (dInfo.Exists) {
				foreach (FileInfo fInfo in dInfo.GetFiles("*.ans", SearchOption.TopDirectoryOnly)) {
					returnList.Add(
						 ReadMessageFromFile(fInfo.FullName)
					);
				}
			}

			return returnList;
		}

		public static List<string> _RealAllStars() {
			List<string> returnList = new List<string>();
			DirectoryInfo dInfo = new DirectoryInfo("./Messages/Stars");
			if (dInfo.Exists) {
				foreach (FileInfo fInfo in dInfo.GetFiles("*.ans", SearchOption.TopDirectoryOnly)) {
					returnList.Add(
						 ReadMessageFromFile(fInfo.FullName)
					);
				}
			}

			return returnList;
		}


		public static string ReadDefinedMessage(string name) {
			if (name != null && _messagesDef.ContainsKey(name.ToLower())) {
				return ReadMessageFromFile(Path.Combine("./Messages/", _messagesDef[name.ToLower()]));
			} else
				return null;
		}

		private static string ReadMessageFromFile(string filename) {

			FileInfo fInfo = new FileInfo(filename);

			if (fInfo.Exists) {

				if (fInfo.Extension == ".ans") {
					StreamReader sr = new StreamReader(File.OpenRead(filename), Encoding.GetEncoding(437));
					string message = sr.ReadToEnd();
					sr.Close();
					return message + TelnetServer.Game.ANSI.ClearFormat();
				} else if (fInfo.Extension == ".MII") {
					var file = ReadMII2File(filename);
					return file.ToString();
				} else if (fInfo.Extension == ".txt") {
					StreamReader sr = new StreamReader(File.OpenRead(filename), Encoding.Default);
					string message = sr.ReadToEnd();
					sr.Close();
					return message;
				}
			}

			return null;

		}


		public static List<string> ReadSpaceShipFile(string filename) {

			FileInfo fInfo = new FileInfo(Path.Combine("./Messages/Ships/", filename));
			List<string> returnList = new List<string>();

			if (fInfo.Exists) {

				Encoding encoding = Encoding.UTF8;
				bool translateToUtf8 = false;

				if (fInfo.Extension == ".ans") {
					encoding = Encoding.GetEncoding(437);
					translateToUtf8 = true;
				} 

				StreamReader sr = new StreamReader(File.OpenRead(fInfo.FullName), encoding);
				string line;

				while ((line = sr.ReadLine()) != null) {

					if (translateToUtf8)
						line = FANSI.ConvertFromCP437ToUTF8(line);

					returnList.Add(line);
				}



				sr.Close();
			}

			return returnList;

		}

		public static MII2 ReadMII2File(string filename) {

			FileInfo fInfo = new FileInfo(filename);

			if (fInfo.Exists) {

				Encoding encoding = Encoding.ASCII;

				StreamReader sr = new StreamReader(File.OpenRead(filename), encoding);

				MII2 fileData = new MII2();
				fileData.ReadData(sr);
				sr.Close();

				return (fileData.Error) ? null : fileData;
			}

			return null;

		}

	}
}
