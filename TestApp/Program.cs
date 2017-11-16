using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaxyE.Galaxy;

namespace TestApp {
	class Program {
		static void Main(string[] args) {


			Sector sector = new Sector(
				(uint)Base36.Decode("368"), (uint)Base36.Decode("4V5"));


			Console.ReadLine();


		}
	}
}
