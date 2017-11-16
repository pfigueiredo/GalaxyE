using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.IO;

namespace GalaxyE.Players {

	[Serializable]
	public class CargoBay : ISerializable {

		public int Capacity { get; private set; }
		public int OccupiedSpace { get; private set; }
		public int FreeSpace { get { return Capacity - OccupiedSpace; } }

		public Dictionary<int, int> Cargo;

		public CargoBay(int capacity) {
			this.Capacity = capacity;
			this.OccupiedSpace = 0;
			Cargo = new Dictionary<int, int>();
		}

		public int GetQuantity(int goodId) {
			if (Cargo.ContainsKey(goodId))
				return Cargo[goodId];
			else
				return 0;
		}

		public int StoreGoods(int goodId, int quantity) {

			int storeQuantity;

			if (quantity > this.FreeSpace)
				storeQuantity = this.FreeSpace;
			else
				storeQuantity = quantity;

			if (!Cargo.ContainsKey(goodId))
				Cargo.Add(goodId, storeQuantity);
			else
				Cargo[goodId] += storeQuantity;

			OccupiedSpace += storeQuantity;

			return storeQuantity;

		}

		public int GetGoods(int goodId, int quantity) {

			int getQuantity;

			if (!Cargo.ContainsKey(goodId))
				getQuantity = 0;
			else {
				if (Cargo[goodId] >= quantity)
					getQuantity = quantity;
				else
					getQuantity = Cargo[goodId];

				Cargo[goodId] -= getQuantity;

			}

			OccupiedSpace -= getQuantity;

			return getQuantity;

		}


		#region ISerializable Members

		public void GetObjectData(SerializationInfo info, StreamingContext context) {

			MemoryStream ms = new MemoryStream();
			BinaryWriter bw = new BinaryWriter(ms);

			bw.Write(this.Capacity);
			bw.Write(this.Cargo.Count);

			foreach (var key in Cargo.Keys) {
				bw.Write(key);
				bw.Write(Cargo[key]);
			}

			Byte[] data = new Byte[ms.Length];

			ms.Position = 0;
			ms.Read(data, 0, data.Length);

			info.AddValue("D", data, typeof(byte[]));

		}

		public CargoBay(SerializationInfo info, StreamingContext context) {

			byte[] data = (byte[]) info.GetValue("D", typeof(byte[]));
			MemoryStream ms = new MemoryStream(data);
			BinaryReader br = new BinaryReader(ms);

			this.Capacity = br.ReadInt32();
			int lenght = br.ReadInt32();

			this.Cargo = new Dictionary<int, int>();

			for (int i = 0; i < lenght; i++) {
				int key = br.ReadInt32();
				int val = br.ReadInt32();
				this.OccupiedSpace += val;
				this.Cargo.Add(key, val);
			}
		}

		#endregion
	}
}
