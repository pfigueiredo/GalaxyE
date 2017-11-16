using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GalaxyE.Galaxy {
	/// <summary>
	/// A Base36 De- and Encoder
	/// </summary>
	public static class Base36 {
		private const string CharList = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

		/// <summary>
		/// Encode the given number into a Base36 string
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static String Encode(long input) {

			bool isNegative = false;

			if (input < 0) {
				isNegative = true;
				input = Math.Abs(input);
			}

			char[] clistarr = CharList.ToCharArray();
			var result = new Stack<char>();
			while (input != 0) {
				result.Push(clistarr[input % 36]);
				input /= 36;
			}

			if (result.Count == 0) result.Push('0');

			return string.Format("{0}{1}", (isNegative) ? "-" : "", new string(result.ToArray()));
		}

		/// <summary>
		/// Decode the Base36 Encoded string into a number
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static Int64 Decode(string data) {

			data = data.Trim();

			bool isNegative = data.StartsWith("-");

			if (isNegative) {
				data = data.Substring(1);
			}

			string input = 
				new string(data.ToUpper().TrimStart(new char[] { '0' }).ToCharArray());

			var reversed = input.Reverse();
			long result = 0;
			int pos = 0;
			foreach (char c in reversed) {
				result += CharList.IndexOf(c) * (long)Math.Pow(36, pos);
				pos++;
			}
			return result * ((isNegative) ? -1 : 1);
		}
	}
}
