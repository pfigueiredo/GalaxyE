using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Data.SqlClient;
using System.Globalization;
using GalaxyE.DB;
using GalaxyE.Managers;

namespace GalaxyE.Galaxy {

	[Serializable]
	public class SystemObject : SectorObject, ISerializable {

		protected StarSystem starSystem;

		public virtual StarSystem StarSystem {
			get { return starSystem; }
			set { starSystem = value; }
		}

		public override SystemId SystemId {
			get { return base.SystemId; }
			set {
				base.SystemId = value;
				starSystem = StarSystem.GetStarSystem(base.Sector, SystemId.SystemNum);
			}
		}

		protected SystemObject() { }

		public SystemObject(StarSystem starSystem)
			: base(starSystem.Sector, starSystem.Coords) {
			this.starSystem = starSystem;
			base.SystemId = starSystem.SystemId;
		}

		#region ISerializable Members

		public override void GetObjectData(SerializationInfo info, StreamingContext context) {
			base.GetObjectData(info, context);
			info.AddValue("starSystem", starSystem);
		}

		public SystemObject(SerializationInfo info, StreamingContext context)
			: base(info, context) {
			this.starSystem = (StarSystem)info.GetValue("starSystem", typeof(StarSystem));
		}

		#endregion
	}

	public class SectorEventArgs : EventArgs {
		public Sector Sector { get; private set; }

		public SectorEventArgs(Sector sector) {
			this.Sector = sector;
		}
	}

	public class SystemEventArgs : EventArgs {
		public StarSystem StarSystem { get; private set; }
		public Coords Coords { get; private set; }

		public SystemEventArgs(StarSystem system) {
			this.StarSystem = system;
			this.Coords = system.Coords;
		}

		public SystemEventArgs(Coords coords) {
			this.StarSystem = null;
			this.Coords = coords;
		}

	}

	public delegate void EnterSectorEventHandler(SectorObject sender, SectorEventArgs e);
	public delegate void LeaveSectorEventhandler(SectorObject sender, SectorEventArgs e);
	public delegate void EnterSystemEventHandler(SectorObject sender, SystemEventArgs e);
	public delegate void LeaveSystemEventhandler(SectorObject sender, SystemEventArgs e);
	public delegate void TargetReachedEventhandler(SectorObject sender, SystemEventArgs e);
	public delegate void TargetLostEventhandler(SectorObject sender, EventArgs e);
	public delegate void IdleEventhandler(SectorObject sender, EventArgs e);

	[Serializable]
	public class SectorObject : ISerializable {

		private SystemLocation _systemLocation;
		private SystemLocation _dsystemLocation;
		private Coords _coords;
		private Coords _destination;
		private float _speed;
		private Sector _sector;
		private string _name;
		private Guid _id = Guid.Empty;

		private SystemId _systemId = new SystemId();
		private DateTime _destinationStamp = DateTime.Now;
		private DateTime _lastUpdate = DateTime.Now;

		public virtual Guid Id { get { return _id; } set { _id = value; } }
		public virtual string Name { get { return _name; } protected set { _name = value; } }
		public virtual Coords Coords { get { return _coords; } set { SetCoords(value); } }
		public virtual Coords dCoords { get { return _destination; } private set { _destination = value; } }
		public virtual Sector Sector { get { return _sector; } protected set { this._sector = value; } }
		public virtual SystemId SystemId { get { return _systemId; } set { _systemId = value; } }
		public virtual float Speed { get { return _speed; } set { _speed = value; } }
		public virtual float sysSpeed { get { return _speed * 14959800 * 2; } }
		public virtual DateTime LastUpdate { get { return _lastUpdate; } private set { _lastUpdate = value; } }
		public virtual SystemLocation SystemLocation { get { return _systemLocation; } set { _systemLocation = value; } }
		public virtual SystemLocation dSystemLocation { get { return _dsystemLocation; } private set { _dsystemLocation = value; } }
		public virtual bool InHiperspace { get; protected set; }
		public virtual bool InSpace { get; protected set; }
		public virtual bool InPlanet { get; protected set; }


		public event EnterSectorEventHandler EnterSector;
		public event LeaveSectorEventhandler LeaveSector;
		public event EnterSystemEventHandler EnterSystem;
		public event LeaveSystemEventhandler LeaveSystem;
		public event IdleEventhandler Idle;
		public event TargetReachedEventhandler TargetReached;
		public event TargetLostEventhandler TargetLost;

		protected virtual void OnIdle() {
			if (Idle != null)
				Idle(this, new EventArgs());
		}

		protected virtual void OnTargetLost() {
			if (TargetLost != null)
				TargetLost(this, new EventArgs());
		}

		protected virtual void OnTargetReached(Coords coords) {
			if (TargetReached != null)
				TargetReached(this, new SystemEventArgs(coords));
		}

		protected SectorObject() {
			this.SystemLocation = new SystemLocation(0, 0, 0);
			this.InHiperspace = false;
			this.InSpace = false;
		}

		public SectorObject(Sector sector, Coords coords) {
			this._sector = sector;
			this._speed = 0;
			SetCoords(coords);
			this.SystemLocation = new SystemLocation(0, 0, 0);
			this._name = "";
			this.InHiperspace = false;
			this.InSpace = false;
		}

		protected virtual void OnEnterSector(Sector sector) {
			if (EnterSector != null)
				EnterSector(this, new SectorEventArgs(sector));
		}

		protected virtual void OnLeaveSector(Sector sector) {
			if (LeaveSector != null)
				LeaveSector(this, new SectorEventArgs(sector));
		}

		protected virtual void OnEnterSystem(StarSystem system) {
			if (EnterSystem != null)
				EnterSystem(this, new SystemEventArgs(system));
		}

		protected virtual void OnLeaveSystem(StarSystem system) {
			if (LeaveSystem != null)
				LeaveSystem(this, new SystemEventArgs(system));
		}

		protected virtual void OnEnterSystem(Coords system) {
			if (EnterSystem != null)
				EnterSystem(this, new SystemEventArgs(system));
		}

		protected virtual void OnLeaveSystem(Coords system) {
			if (LeaveSystem != null)
				LeaveSystem(this, new SystemEventArgs(system));
		}

		public virtual void SetCoords(Coords coords) {

			Sector currentSector = this._sector;
			bool isStarSystem = (this as StarSystem) != null;
			bool isSystemBody = (this as SystemBody) != null;

			if (coords != _coords && !InHiperspace && !isStarSystem && !isSystemBody) {
				if (_coords != null)
					OnLeaveSystem(_coords);

				if (coords != null)
					OnEnterSystem(coords);
			}


			_coords = coords;
			_sector = coords.GetSector();

			if (_sector != currentSector && currentSector != null && !isStarSystem && !isSystemBody) {
				OnLeaveSector(currentSector);
			}

			bool sysIdOk = false;

			if (!isStarSystem && !isSystemBody) {
				foreach (var sys in _sector.Systems) {
				    if (sys.Coords == coords) {
				        _systemId = sys.SystemId;
				        sysIdOk = true;
				    }
				}
			}

			if (!sysIdOk) {
				_systemId = new SystemId(-1, _sector.IndexX, _sector.IndexY);
			}

		}

		public virtual void SetDestSystemLocation(SystemLocation dest) {
			if (dest != null && (dest.IsInHyperspace || dest.IsInPlanet))
				dest = null;

			_dsystemLocation = dest;
			_destinationStamp = DateTime.Now;
			_lastUpdate = DateTime.Now;
			this.InSpace = (dest != null) ? true : false;
		}

		public virtual void SetDestCoords(Coords dest) {
			_destination = dest;
			_destinationStamp = DateTime.Now;
			_lastUpdate = DateTime.Now;
			this.InHiperspace = (dest != null) ? true : false;
			if (this.InHiperspace) {
				OnLeaveSystem(this.Coords);
				this.SystemLocation.SetInHyperspace();
			}
		}

		public virtual TimeSpan GetInSystemETA() {

			if (dSystemLocation == null) return TimeSpan.FromSeconds(0);

			SystemLocation destination = (dSystemLocation == null) ? SystemLocation : dSystemLocation;

			double distanceToDest = SystemLocation.GetInSystemDistance(destination);

			if (distanceToDest != 0 && !double.IsNaN(distanceToDest) && sysSpeed > 0) {
				return TimeSpan.FromMinutes(distanceToDest / (sysSpeed * 15));
			} else
				return TimeSpan.FromMinutes(0);
		}

		public virtual TimeSpan GetETA() {

			if (dCoords == null) return TimeSpan.FromSeconds(0);

			Coords destination = (dCoords == null) ? Coords : dCoords;

			double distanceToDest = Coords.Galactic.CalculateDistance(destination.Galactic);
			
			if (distanceToDest != 0 && !double.IsNaN(distanceToDest) && Speed > 0) {
				return TimeSpan.FromMinutes(distanceToDest / (Speed * 40));
			} else 
				return TimeSpan.FromMinutes(0);
		}

		public virtual bool Update() {
			DateTime Stamp = DateTime.Now;
			TimeSpan diff = Stamp.Subtract(LastUpdate);

			bool retVal = false;

			if (diff.TotalSeconds > 1 && dCoords != null) {
				float speed = (float)(diff.TotalMinutes * (this.Speed * 40));

				Coords newCoords;

				var result = CalculateNext(Coords, dCoords, speed, out newCoords);

				switch (result) {
					case CoordsCalculationResult.Continues:
						this.Coords = newCoords;
						this.InHiperspace = true;
						this.SystemLocation.SetInHyperspace();
						retVal = true;
						break;
					case CoordsCalculationResult.End:
						this.SystemLocation.RandomizeInSystemCoords();
						this.Coords = newCoords;
						this.dCoords = null;
						this.InHiperspace = false;						
						retVal = true;
						OnEnterSector(newCoords.GetSector());
						OnEnterSystem(newCoords);
						break;
					case CoordsCalculationResult.None:
						this.dCoords = null;
						this.InHiperspace = false;
						break;
				}

				this.LastUpdate = Stamp;

			} else {
				if (dCoords == null) {
					this.InHiperspace = false;
					OnEnterSystem(this.Coords);
				}
			}

			if (diff.TotalSeconds > 1 && dSystemLocation != null) {
				float speed = (float)(diff.TotalMinutes * this.sysSpeed * 15);

				retVal = false;

				SystemLocation newLocation;

				var result = CalculateNext(SystemLocation, dSystemLocation, speed, out newLocation);

				switch (result) {
					case CoordsCalculationResult.Continues:
						this.SystemLocation = newLocation;
						this.InSpace = true;
						break;
					case CoordsCalculationResult.End:
						this.SystemLocation = newLocation.GetLocationAtDistance(150);
						this.dSystemLocation = null;
						this.InSpace = false;
						this.OnTargetReached(this.Coords);
						this.OnIdle();
						retVal = true;
						break;
					case CoordsCalculationResult.None:
						this.dSystemLocation = null;
						this.InSpace = false;
						this.OnIdle();
						break;
				}

				this.LastUpdate = Stamp;

			} else {
				if (dSystemLocation == null)
					this.InSpace = false;
			}

			
			return retVal;
		}

		public enum CoordsCalculationResult { None, Continues, End }

		public static CoordsCalculationResult CalculateNext
			(SystemLocation actual, SystemLocation dest, float speed, out SystemLocation calculated) {

			//Cant travel to an unreachable destination
			if (dest == null || dest.IsInHyperspace || dest.IsInPlanet) {
				calculated = actual;
				return CoordsCalculationResult.None;
			}

			double distanceToDest = actual.GetInSystemDistance(dest);

			if (distanceToDest == 0) {
				calculated = dest;
				return CoordsCalculationResult.End;
			}

			double diffX = actual.X - dest.X;
			double diffY = actual.Y - dest.Y;
			double diffZ = actual.Z - dest.Z;

			double speedDiffX = (speed * diffX / distanceToDest);
			double speedDiffY = (speed * diffY / distanceToDest);
			double speedDiffZ = (speed * diffZ / distanceToDest);

			int overflow = 0;

			if (Math.Abs(speedDiffX) >= Math.Abs(diffX)) { speedDiffX = diffX; overflow++; }
			if (Math.Abs(speedDiffY) >= Math.Abs(diffY)) { speedDiffY = diffY; overflow++; }
			if (Math.Abs(speedDiffZ) >= Math.Abs(diffZ)) { speedDiffZ = diffZ; overflow++; }

			if (overflow == 3 || (speedDiffX == 0 && speedDiffY == 0 && speedDiffZ == 0)) {
				calculated = dest;
				return CoordsCalculationResult.End;
			}

			SystemLocation newLocation = new SystemLocation(
				actual.X - speedDiffX,
				actual.Y - speedDiffY,
				actual.Z - speedDiffZ
			);

			calculated = newLocation;
			return CoordsCalculationResult.Continues;

		}

		public static CoordsCalculationResult CalculateNext(Coords actual, Coords dest, float speed, out Coords calculated) {

			if (dest == null) {
				calculated = actual;
				return CoordsCalculationResult.None;
			}

			double distanceToDest = actual.Galactic.CalculateDistance(dest.Galactic);

			if (distanceToDest == 0) {
				calculated = dest;
				return CoordsCalculationResult.End;
			}

			double galDiffX = actual.Galactic.X - dest.Galactic.X;
			double galDiffY = actual.Galactic.Y - dest.Galactic.Y;
			double galDiffZ = actual.Galactic.Z - dest.Galactic.Z;
			
			double speedDiffX = (speed * galDiffX / distanceToDest);
			double speedDiffY = (speed * galDiffY / distanceToDest);
			double speedDiffZ = (speed * galDiffZ / distanceToDest);

			int overflow = 0;

			if (Math.Abs(speedDiffX) >= Math.Abs(galDiffX)) { speedDiffX = galDiffX; overflow++; }
			if (Math.Abs(speedDiffY) >= Math.Abs(galDiffY)) { speedDiffY = galDiffY; overflow++; }
			if (Math.Abs(speedDiffZ) >= Math.Abs(galDiffZ)) { speedDiffZ = galDiffZ; overflow++; }

			if (overflow == 3 || (speedDiffX == 0 && speedDiffY == 0 && speedDiffZ == 0)) {
				calculated = dest;
				return CoordsCalculationResult.End;
			}

			GalacticLocation newGalacticLocation = new GalacticLocation(
				actual.Galactic.X - speedDiffX,
				actual.Galactic.Y - speedDiffY,
				actual.Galactic.Z - speedDiffZ
			);

			Coords coords = new Coords(newGalacticLocation);
			calculated = coords;
			return CoordsCalculationResult.Continues;

		}

		#region ISerializable Members

		public virtual void GetObjectData(SerializationInfo info, StreamingContext context) {
			info.AddValue("coord", _coords);
			info.AddValue("destination", _destination);
			info.AddValue("speed", _speed);
			info.AddValue("name", _name);
			info.AddValue("sLoc", _systemLocation);
			info.AddValue("dsLoc", _dsystemLocation);
			info.AddValue("dStp", this._destinationStamp);
			info.AddValue("id", this._id);
			info.AddValue("stp", this._lastUpdate);
			info.AddValue("sId", (ulong)this._systemId);
			info.AddValue("iHy", InHiperspace);
			info.AddValue("iSp", InSpace);
			info.AddValue("iPl", InPlanet);

			/*
					public virtual Sector Sector { get { return _sector; } protected set { this._sector = value; } }
					public virtual bool InHiperspace { get; protected set; }
					public virtual bool InSpace { get; protected set; }
					public virtual bool InPlanet { get; protected set; }
			 */


		}

		public SectorObject(SerializationInfo info, StreamingContext context) {

			List<string> sNames = new List<string>();
			foreach (var entry in info) sNames.Add(entry.Name);

			this._coords = (Coords)info.GetValue("coord", typeof(Coords));
			this._destination = (Coords)info.GetValue("destination", typeof(Coords));
			this._speed = info.GetSingle("speed");
			this._name = info.GetString("name");
			this._sector = _coords.GetSector();
			this._systemLocation = (SystemLocation)info.GetValue("sLoc", typeof(SystemLocation));

			if (sNames.Contains("dsLoc"))
				this._dsystemLocation = (SystemLocation)info.GetValue("dsLoc", typeof(SystemLocation));

			this._destinationStamp = info.GetDateTime("dStp");
			this._id = (Guid)info.GetValue("id", typeof(Guid));
			this._lastUpdate = info.GetDateTime("stp");
			this._systemId = (SystemId)(ulong)info.GetUInt64("sId");

			if (sNames.Contains("iHy")) this.InHiperspace = info.GetBoolean("iHy");
			if (sNames.Contains("iSp")) this.InHiperspace = info.GetBoolean("iSp");
			if (sNames.Contains("iPl")) this.InHiperspace = info.GetBoolean("iPl");

		}

		#endregion

		public override string ToString() {
			return this.Name;
		}
	}

	public interface IPersistable {
		bool IsDirty { get; set; }
		void Persist();
	}

	[Serializable]
	public class PersistentSectorObject : SectorObject, ISerializable, IPersistable {

		public Nullable<Guid> pId { get; set; }
		public Nullable<int> NPCId { get; set; }
		public bool IsDirty { get; set; }

		public PersistentSectorObject() {
			pId = null;
			NPCId = null;
		}

		protected override void OnEnterSector(Sector sector) {
			base.OnEnterSector(sector);
			Persist();
		}

		protected override void OnLeaveSector(Sector sector) {
			base.OnLeaveSector(sector);
			Persist();
		}

		public virtual void Persist() {

			Database.Persist("SectorObject", this.Id.ToString(), this);
			this.IsDirty = false;

		}

		public static PersistentSectorObject Deserialize(byte[] data) {
			MemoryStream ms = new MemoryStream(data);
			BinaryFormatter formatter = new BinaryFormatter();
			PersistentSectorObject obj = formatter.Deserialize(ms) as PersistentSectorObject;
			return obj;
		}

		public static PersistentSectorObject GetObject(Guid id) {

			return Database.GetObject<PersistentSectorObject>("SectorObject", id.ToString());


		}

		public string MakeSoCode(string format) {

			StringBuilder sBuilder = new StringBuilder();

			byte[] t = this.Id.ToByteArray();

			long id = 
				t[0]            | t[1] << 8       | t[2] << (8 * 2) | t[3] << (8 * 3) |
				t[4] << (8 * 4) | t[5] << (8 * 5) | t[6] << (8 * 6) | t[7] << (8 * 7); 

			if (format != null) {
				for (int i = 0; i < format.Length; i++) {
					char c = format[i];
					switch (c) {
						case 'X': 
							sBuilder.AppendFormat("{0:X}", Math.Abs(id & 0xF)); 
							id >>= 4; 
							break;
						case 'D': 
							sBuilder.AppendFormat("{0}", Math.Abs(id % 10)); 
							id /= 10; 
							break;
						case 'C': 
							sBuilder.AppendFormat("{0}", (char)Math.Abs((int)'A' + (int)(id & 0xFF) % 23)); 
							id -= ((int)'A' + (int)(id & 0xFF) % 23); 
							break;
						default: 
							sBuilder.Append(c); 
							break;
					}
				}
			}

			//while (id > 0) {
			//    sBuilder.AppendFormat("{0}", Math.Abs(id % 10));
			//    id /= 10;
			//}

			return sBuilder.ToString();

		}


		#region ISerializable Members

		public override void GetObjectData(SerializationInfo info, StreamingContext context) {
			base.GetObjectData(info, context);
		}

		public PersistentSectorObject(SerializationInfo info, StreamingContext context)
			: base(info, context) {

		}

		#endregion
	}

}
