using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace GalaxyE.Galaxy {


	#region GalaxyConstacts
	public class GalaxyConstants {
		public const double GalaxyWidth = 100000; // Lyears
		public const double NumberSectors = 8191; // nº sectors in each side
		public const double SectorWidth = GalaxyWidth / NumberSectors; // Lyears
		public const double SectorHeight = GalaxyWidth / NumberSectors;
		public const double SectorDepth = SectorWidth * 2; // Lyears
		public const double ScreenSectorSize = 1024; // screen points
		public const double LocalSectorWidth = 128; // sector coords
		public const double LocalSectorHeight = 128; // sector coords
		public const double LocalSectorDepth = LocalSectorWidth * 2; // sector coords
	}
	#endregion

	#region GalacticLocation (in Ly)
	[Serializable]
	public struct GalacticLocation {
		public double X, Y, Z;

		public double gX { get { return X; } set { X = value; } }
		public double gY { get { return Y; } set { Y = value; } }
		public double gZ { get { return Z; } set { Z = value; } }

		public GalacticLocation(double gx, double gy, double gz) {
			this.X = gx;
			this.Y = gy;
			this.Z = gz;
		}

		public GalacticLocation(uint sectorIndexX, uint sectorIndexY) :
			this(sectorIndexX, sectorIndexY, new SectorLocation(0, 0, 0)) { }

		public GalacticLocation(uint sectorIndexX, uint sectorIndexY, SectorLocation sectorLocation) {
			this.X = (
				(sectorIndexX * GalaxyConstants.SectorWidth) +
				(
					(sectorLocation.X + GalaxyConstants.LocalSectorWidth / 2) *
						GalaxyConstants.SectorWidth / GalaxyConstants.LocalSectorWidth
				)
			);
			this.Y = (
				(sectorIndexY * GalaxyConstants.SectorWidth) +
				(
					(sectorLocation.Y + GalaxyConstants.LocalSectorWidth / 2) *
						GalaxyConstants.SectorWidth / GalaxyConstants.LocalSectorWidth
				)
			);
			this.Z = (sectorLocation.Z + GalaxyConstants.LocalSectorDepth / 2) * GalaxyConstants.SectorDepth / GalaxyConstants.LocalSectorDepth;
		}

		public SectorLocation ToSectorLocation() {
			double sx, sy, sz;

			sx = (X % GalaxyConstants.SectorWidth)
				* GalaxyConstants.LocalSectorWidth / GalaxyConstants.SectorWidth;
			sy = (Y % GalaxyConstants.SectorWidth)
				* GalaxyConstants.LocalSectorWidth / GalaxyConstants.SectorWidth;
			sz = ((this.Z) * GalaxyConstants.LocalSectorDepth / GalaxyConstants.SectorDepth);

			return new SectorLocation(
				(int)Math.Round((sx - GalaxyConstants.LocalSectorWidth / 2), 0),
				(int)Math.Round((sy - GalaxyConstants.LocalSectorWidth / 2), 0),
				(int)Math.Round((sz - GalaxyConstants.LocalSectorDepth / 2), 0)
			);

		}

		public double CalculateDistance(GalacticLocation location) {
			double result = 0;

			double XDiference = Math.Abs((float)(location.X - this.X));
			double YDiference = Math.Abs((float)(location.Y - this.Y));
			double ZDiference = Math.Abs((float)(location.Z - this.Z));

			result = Math.Sqrt(Math.Pow(XDiference, 2) + Math.Pow(YDiference, 2) + Math.Pow(ZDiference, 2));

			return result;
		}
	}
	#endregion

	[Serializable]
	public class SystemLocation : ISerializable {

		public double X;
		public double Y;
		public double Z;

		public SystemLocation GetLocationAtDistance(double dist) {

			Random rand = new Random();

			var ns = new SystemLocation(
				this.X - (dist * 2 / rand.Next(1, 5)),
				this.Y - (dist * 2 / rand.Next(1, 5)),
				this.Z
			);

			return ns;
		}

		public static string GetDistanceDesc(double dist) {

			double auDist = dist / 149598000;

			return (auDist > 0.001) ?
				string.Format("{0:0.0000} AU", auDist) :
				string.Format("{0:0.00} Km", dist);
		}

		public SystemLocation(double x, double y, double z) {
			this.X = x;
			this.Y = y;
			this.Z = z;
		}

		public long GetInSystemDistance(SystemLocation location) {
			return (long)Math.Sqrt(
				Math.Pow(Math.Abs(X - location.X), 2) +
				Math.Pow(Math.Abs(Y - location.Y), 2)
			);
		}

		public static SystemLocation GetUpdatedLocation(SystemBody body) {
			
			double angle = body.BaseAngle;
			double radius = body.OrbitalRadius;
			double period = body.OrbitalPeriod;

			StarDate baseDate = StarDate.GetStarDate(new DateTime(2012, 1, 1));
			//StarDate currentDate = StarDate.Now;

			//Passou a estar fixo pois estava com alguma dificuldade em manter as coisas em orbita ;P
			StarDate currentDate = StarDate.GetStarDate(new DateTime(2024, 1, 1));

			TimeSpan span = currentDate.Date.Subtract(baseDate.Date);
			angle += ((span.TotalDays / 365.25) * period * 360);

			double rad = 0.0174532925;

			double ssX = (long)(radius * Math.Cos(angle * rad) * 149598000);
			double ssY = (long)(radius * Math.Sin(angle * rad) * 149598000);
			double ssZ = 0;

			return new SystemLocation(ssX, ssY, ssZ);

		}

		public static void UpdateLocation(SystemBody body) {

			double angle = body.BaseAngle;
			double radius = body.OrbitalRadius;
			double period = body.OrbitalPeriod;

			StarDate baseDate = StarDate.GetStarDate(new DateTime(2012, 1, 1));
			//StarDate currentDate = StarDate.Now;

			//Passou a estar fixo pois estava com alguma dificuldade em manter as coisas em orbita ;P
			StarDate currentDate = StarDate.GetStarDate(new DateTime(2024, 1, 1));

			TimeSpan span = currentDate.Date.Subtract(baseDate.Date);

			angle += ((span.TotalDays / 365.25) * period * 360);

			double rad = 0.0174532925;

			double ssX = (long)(radius * Math.Cos(angle * rad) * 149598000);
			double ssY = (long)(radius * Math.Sin(angle * rad) * 149598000);
			double ssZ = 0;
			body.SystemLocation.SetInSystemCoords(ssX, ssY, ssZ);

		}

		public void RandomizeInSystemCoords() {
			Random rand = new Random((int)(DateTime.Now.Ticks & 0xFFFFFFFF));
			this.X = rand.Next(-8, 8) * 149598000;
			this.Y = rand.Next(-8, 8) * 149598000;
			this.Z = 0;
		}

		public bool IsInHyperspace {
			get {
				return double.IsPositiveInfinity(this.X)
					|| double.IsPositiveInfinity(this.Y)
					|| double.IsPositiveInfinity(this.Z);
			}
		}

		public bool IsInPlanet {
			get {
				return double.IsNaN(this.X)
					|| double.IsNaN(this.Y)
					|| double.IsNaN(this.Z);
			}
		}

		public void SetInHyperspace() {
			this.X = double.PositiveInfinity;
			this.Y = double.PositiveInfinity;
			this.Z = double.PositiveInfinity;
		}

		public void SetInPlanet() {
			this.X = double.NaN;
			this.Y = double.NaN;
			this.Z = double.NaN;
		}

		public void SetInSystemCoords(double X, double Y, double Z) {
			this.X = X;
			this.Y = Y;
			this.Z = Z;
		}

		#region ISerializable Members

		public void GetObjectData(SerializationInfo info, StreamingContext context) {
			info.AddValue("x", this.X);
			info.AddValue("y", this.Y);
			info.AddValue("z", this.Z);
		}

		public SystemLocation(SerializationInfo info, StreamingContext context) {
			this.X = info.GetDouble("x");
			this.Y = info.GetDouble("y");
			this.Z = info.GetDouble("z");
		}

		#endregion
	}

	#region SectorLocation in local sector coords
	[Serializable]
	public struct SectorLocation {
		public int X, Y, Z;
		

		public SectorLocation(int x, int y, int z) {
			this.X = x;
			this.Y = y;
			this.Z = z;
		}

	}
	#endregion

	#region SreenLocation in screen points
	[Serializable]
	public struct ScreenLocation {
		public int X, Y, Z;

		public ScreenLocation(int sx, int sy, int sz) {
			this.X = sx;
			this.Y = sy;
			this.Z = sz;
		}

		public ScreenLocation(SectorLocation sLocation) {
			this.X = sLocation.X * 8;
			this.Y = sLocation.Y * 8;
			this.Z = sLocation.Z + 128;
		}

	}
	#endregion

	#region SystemBodyId
	[Serializable]
	public struct SystemBodyId {

		SystemId systemId;
		int bodyNumber;

		public SystemId SystemId { get { return systemId; } }
		public int BodyNumber { get { return bodyNumber; } }

		public SystemBodyId(SystemId systemId, int bodyNumber) {
			this.systemId = systemId;
			this.bodyNumber = bodyNumber;
		}

		public SystemBodyId(ulong systemId, int bodyNumber) {
			this.systemId = new SystemId(systemId);
			this.bodyNumber = bodyNumber;
		}
	}
	#endregion

	#region SectorId
	[Serializable]
	public struct SectorId {
		ulong sectorId;

		public ulong Id { get { return sectorId; } }
		public uint SectorIndexX { get { return (uint)((sectorId >> 13) & 0x1FFF); } }
		public uint SectorIndexY { get { return (uint)(sectorId & 0x1FFF); } }

		public SectorId(ulong id) {
			this.sectorId = id & 0x3FFFFFF;
		}

		public SectorId(uint SectorIndexX, uint SectorIndexY) {
			this.sectorId = (((ulong)SectorIndexX) << 13) + (ulong)SectorIndexY;
		}
	}
	#endregion

	#region SystemId
	public struct SystemId {

		//uniqueid = (((ulong)Sysnum) << 26) + (((ulong)xl) << 13) + (ulong)yl;
		ulong systemId;

		public ulong Id { get { return systemId; } set { systemId = value; } }
		public uint SectorIndexX { get { return (uint)((systemId >> 13) & 0x1FFF); } }
		public uint SectorIndexY { get { return (uint)(systemId & 0x1FFF); } }
		public int SystemNum { get { return (int)(systemId >> 26); } }

		public SystemId(ulong id) {
			this.systemId = id;
		}

		public SystemId(int sysNum, uint SectorIndexX, uint SectorIndexY) {
			this.systemId = (((ulong)sysNum) << 26) + (((ulong)SectorIndexX) << 13) + (ulong)SectorIndexY;
		}

		public SectorId SectorId {
			get { return new SectorId(systemId); }
		}

		public Sector GetSector() {
			return new Sector(this.SectorIndexX, this.SectorIndexY);
		}

		public StarSystem GetSystem() {
			Sector sector = this.GetSector();
			if (this.SystemNum >= 0 && this.SystemNum < sector.Systems.Count)
				return sector.Systems[this.SystemNum];
			else
				return null;
		}

		public static implicit operator SystemId(ulong systemIdValue) {
			return new SystemId(systemIdValue);
		}

		public static implicit operator ulong(SystemId systemId) {
			return systemId.Id;
		}
	}
	#endregion

	#region Coords (location of an object)
	[Serializable]
	public class Coords : ISerializable {

		private GalacticLocation _galacticLocation;
		private SectorLocation _sectorLocation;
		private ScreenLocation _screenLocation;
		private uint _sectorIndexX;
		private uint _sectorIndexY;

		public GalacticLocation Galactic {
			get { return _galacticLocation; }
			set {
				this._galacticLocation = value;
				this._sectorLocation = _galacticLocation.ToSectorLocation();
				this._screenLocation = new ScreenLocation(_sectorLocation);
				this._sectorIndexX = (uint)(_galacticLocation.X / GalaxyConstants.SectorWidth);
				this._sectorIndexY = (uint)(_galacticLocation.Y / GalaxyConstants.SectorWidth);
			}
		}

		public SectorLocation Sector { get { return _sectorLocation; } }
		public ScreenLocation Screen { get { return _screenLocation; } }
		public uint SectorIndexX { get { return _sectorIndexX; } }
		public uint SectorIndexY { get { return _sectorIndexY; } }

		private Coords() { }

		public Coords(uint sectorIndexX, uint sectorIndexY) {
			this._sectorIndexX = sectorIndexX;
			this._sectorIndexY = sectorIndexY;
			this._sectorLocation = new SectorLocation(0, 0, 0);
			this._galacticLocation = new GalacticLocation(sectorIndexX, sectorIndexY, _sectorLocation);
			this._screenLocation = new ScreenLocation(_sectorLocation);
		}

		public Coords(uint sectorIndexX, uint sectorIndexY, int sX, int sY, int sZ) {
			this._sectorIndexX = sectorIndexX;
			this._sectorIndexY = sectorIndexY;
			this._sectorLocation = new SectorLocation(sX, sY, sZ);
			this._galacticLocation = new GalacticLocation(_sectorIndexX, _sectorIndexY, _sectorLocation);
			this._screenLocation = new ScreenLocation(_sectorLocation);
		}

		public Coords(uint sectorIndexX, uint sectorIndexY, SectorLocation sectorLocation) {
			this._sectorIndexX = sectorIndexX;
			this._sectorIndexY = sectorIndexY;
			this._sectorLocation = new SectorLocation(sectorLocation.X, sectorLocation.Y, sectorLocation.Z);
			this._galacticLocation = new GalacticLocation(sectorIndexX, sectorIndexY, sectorLocation);
			this._screenLocation = new ScreenLocation(sectorLocation);
		}

		public Coords(GalacticLocation location) {
			this._galacticLocation = location;
			this._sectorLocation = _galacticLocation.ToSectorLocation();
			this._screenLocation = new ScreenLocation(_sectorLocation);
			this._sectorIndexX = (uint)(_galacticLocation.X / GalaxyConstants.SectorWidth);
			this._sectorIndexY = (uint)(_galacticLocation.Y / GalaxyConstants.SectorWidth);
		}

		public Sector GetSector() {
			return new Sector(this._sectorIndexX, this._sectorIndexY);
		}

		public bool IsInSector(Sector sector) {
			return (sector.IndexX == this._sectorIndexX
				&& sector.IndexY == this._sectorIndexY);
		}

		public double CalculateDistance(Coords coords) {
			return this._galacticLocation.CalculateDistance(coords._galacticLocation);
		}

		public bool IsInRange(Coords coords, double range) {
			return (this._galacticLocation.CalculateDistance(coords._galacticLocation) <= range);
		}

		#region ISerializable Members

		public virtual void GetObjectData(SerializationInfo info, StreamingContext context) {

			info.AddValue("sectorIndexX", _sectorIndexX);
			info.AddValue("sectorIndexY", _sectorIndexY);
			info.AddValue("sX", _sectorLocation.X);
			info.AddValue("sY", _sectorLocation.Y);
			info.AddValue("sZ", _sectorLocation.Z);

		}

		public Coords(SerializationInfo info, StreamingContext context) {
			this._sectorIndexX = info.GetUInt32("sectorIndexX");
			this._sectorIndexY = info.GetUInt32("sectorIndexY");
			int sX, sY, sZ;

			sX = info.GetInt32("sX");
			sY = info.GetInt32("sY");
			sZ = info.GetInt32("sZ");

			this._sectorLocation = new SectorLocation(sX, sY, sZ);
			this._galacticLocation = new GalacticLocation(_sectorIndexX, _sectorIndexY, _sectorLocation);
			this._screenLocation = new ScreenLocation(_sectorLocation);
		}

		#endregion

		#region Equals
		public static bool operator ==(Coords sObj1, Coords sObj2) {

			if (Object.ReferenceEquals(sObj1, null) || Object.ReferenceEquals(sObj2, null))
				return Object.ReferenceEquals(sObj1, sObj2);
			else
				return sObj1.Equals(sObj2);

		}

		public static bool operator !=(Coords sObj1, Coords sObj2) {

			if (Object.ReferenceEquals(sObj1, null) || Object.ReferenceEquals(sObj2, null))
				return !Object.ReferenceEquals(sObj1, sObj2);
			else
				return !sObj1.Equals(sObj2);
		}

		public override bool Equals(object obj) {
			Coords cObj = obj as Coords;
			if (!Object.ReferenceEquals(cObj, null)) {
				return (
					   this._sectorIndexX == cObj._sectorIndexX
					&& this._sectorIndexY == cObj._sectorIndexY
					&& this._sectorLocation.X == cObj._sectorLocation.X
					&& this._sectorLocation.Y == cObj._sectorLocation.Y
					&& this._sectorLocation.Z == cObj._sectorLocation.Z
			   );
			}
			return false;
		}

		public override int GetHashCode() {
			return (int)(((_sectorIndexX) << 13) + _sectorIndexY);
		}

		#endregion

		public override string ToString() {
			return string.Format(
				"{0}.{1}:{2}.{3}.{4}",
				Base36.Encode((long)this._sectorIndexX), 
				Base36.Encode((long)this._sectorIndexY),
				Base36.Encode((long)this._sectorLocation.X),
				Base36.Encode((long)this._sectorLocation.Y),
				Base36.Encode((long)this._sectorLocation.Z)
			);
		}

		public static bool TryParse(string value, out Coords coords, bool isBase36) {
			string[] mainTokens = value.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

			uint sectorIndexX = 0; 
			uint sectorIndexY = 0; 
			int sX = 0, sY = 0, sZ = 0;
			coords = new Coords(0, 0, 0, 0, 0);

			if (mainTokens.Length == 0)
				return false;

			string[] sectorTokens = mainTokens[0].Split('.');

			if (sectorTokens.Length != 2)
				return false;
			else {
				if (isBase36)
					sectorIndexX = (uint)Base36.Decode(sectorTokens[0]);
				else if (!uint.TryParse(sectorTokens[0], out sectorIndexX))
					return false;
				
				if (isBase36)
					sectorIndexY = (uint)Base36.Decode(sectorTokens[1]);
				else if (!uint.TryParse(sectorTokens[1], out sectorIndexY))
					return false;
			}

			if (mainTokens.Length > 1) {
				string[] systemTokens = mainTokens[1].Split('.');
				if (systemTokens.Length >= 3) {
					if (isBase36)
						sX = (int)Base36.Decode(systemTokens[0]);
					else if (!int.TryParse(systemTokens[0], out sX))
						return false;
					if (isBase36)
						sY = (int)Base36.Decode(systemTokens[1]);
					else if (!int.TryParse(systemTokens[1], out sY))
						return false;
					if (isBase36)
						sZ = (int)Base36.Decode(systemTokens[2]);
					else if (!int.TryParse(systemTokens[2], out sZ))
						return false;
				}
			}

			coords = new Coords(sectorIndexX, sectorIndexY, sX, sY, sZ);
			return true;
		}
	}
	#endregion


}
