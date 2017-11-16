using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TelnetServer.SocketServer;

namespace TelnetServer {
	class Program {
		static void Main(string[] args) {

			Server server = new Server(6000, 100, 64);

			server.Start();
			Console.ReadLine();

			//server.Init();
			//server.Start(new System.Net.IPEndPoint(System.Net.IPAddress.Any, 6000));


		}
	}
}
