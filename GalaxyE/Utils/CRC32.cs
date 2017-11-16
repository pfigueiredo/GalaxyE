using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GalaxyE.Utils {
	public class CRC32 {
		
		private static readonly uint[] Table;

        static CRC32()
        {
            Table = new uint[256];
            const uint kPoly = 0xEDB88320;
            for (uint i = 0; i < 256; i++)
            {
                uint r = i;
                for (int j = 0; j < 8; j++)
                    if ((r & 1) != 0)
                        r = (r >> 1) ^ kPoly;
                    else
                        r >>= 1;
                Table[i] = r;
            }
        }

        private uint value;

		public CRC32()
        {
            Init();
        }

        /// <summary>
        /// Reset CRC
        /// </summary>
        public void Init()
        {
            value = 0xFFFFFFFF;
        }

        public void UpdateByte(byte b)
        {
            value = Table[(((byte)value) ^ b)] ^ (value >> 8);
        }

        public void Update(byte[] data, int offset, int size)
        {
            if (size < 0) throw new ArgumentOutOfRangeException("size");
            while (size-- != 0)
                value = Table[(((byte)value) ^ data[offset++])] ^ (value >> 8);
        }

        public int Value
        {
            get { return (int)(value ^ 0xFFFFFFFF); }
        }

        static public int Compute(byte[] data, int offset, int size)
        {
			var crc = new CRC32();
            crc.Update(data, offset, size);
            return crc.Value;
        }

        static public int Compute(byte[] data)
        {
            return Compute(data, 0, data.Length);
        }

		static public int Compute(string data) {
			return Compute(Encoding.Default.GetBytes(data));
		}

        static public int Compute(ArraySegment<byte> block)
        {
            return Compute(block.Array, block.Offset, block.Count);
        }
    }
	
}
