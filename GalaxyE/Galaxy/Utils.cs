using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GalaxyE.Galaxy {
	public class Utils {
		public static void Rotate_SystemParams(ref ulong SystemParam_0, ref ulong SystemParam_1) {
			ulong Tmp1, Tmp2;

			Tmp1 = (SystemParam_0 << 3) | (SystemParam_0 >> 29);
			Tmp2 = SystemParam_0 + SystemParam_1;
			Tmp1 += Tmp2;
			SystemParam_0 = Tmp1;
			SystemParam_1 = (Tmp2 << 5) | (Tmp2 >> 27);
		}


		public static uint _rotl(uint value, int shift) {
			int max_bits = sizeof(uint) << 3;
			if (shift < 0)
				return _rotr(value, -shift);

			if (shift > max_bits)
				shift = shift % max_bits;
			return (value << shift) | (value >> (max_bits - shift));
		}

		public static uint _rotr(uint value, int shift) {
			int max_bits = sizeof(uint) << 3;
			if (shift < 0)
				return _rotl(value, -shift);

			if (shift > max_bits << 3)
				shift = shift % max_bits;
			return (value >> shift) | (value << (max_bits - shift));
		}
	}
}
