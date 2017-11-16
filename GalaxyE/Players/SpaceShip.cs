using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaxyE.Galaxy;
using System.Runtime.Serialization;
using GalaxyE.Players.Equipment;
using GalaxyE.Managers;
using GalaxyE.Galaxy.Zones;

namespace GalaxyE.Players {

	public delegate void BattleStartedEventHandler(object sender, SpaceBattle battle, SpaceShip enemy);
	public delegate void BattleEndedEventHandler(object sender, SpaceBattle battle, SpaceShip enemy);
	public delegate void TargetInRange (object sender, SectorObject target);
 	public delegate void TargetReached(object sender, SectorObject target);
	public delegate void Docked (object sender, SpacePort port);

	[Serializable]
	public class SpaceShip : PersistentSectorObject, ISerializable {

		#region Battle Events and handlers
		public void ReportBattleStarted(SpaceBattle battle, SpaceShip enemy) {
			OnBattleStarted(battle, enemy);
		}

		protected virtual void OnBattleStarted(SpaceBattle battle, SpaceShip enemy) {
			if (BattleStarted != null)
				BattleStarted(this, battle, enemy);
		}

		public event BattleStartedEventHandler BattleStarted;

		public void ReportBattleEnded(SpaceBattle battle, SpaceShip enemy) {
			OnBattleEnded(battle, enemy);
		}

		protected virtual void OnBattleEnded(SpaceBattle battle, SpaceShip enemy) {
			if (BattleEnded != null)
				BattleEnded(this, battle, enemy);
		}

		public event BattleEndedEventHandler BattleEnded;


		public void MakeBattleCommand(SpaceBattle battle) {

			if (this.Lasers != null && this.Lasers.Length > 0) {

				int requiredEnergy = 0;
				foreach (var l in this.Lasers)
					requiredEnergy += l.Energy;

				if (requiredEnergy < this.Energy) {
					battle.DoBattleCommand(this, "F");
					return;
				}
 
			}

			if (this.MissileLaunchers != null && this.MissileLaunchers.Length > 0) {
				int requiredEnergy = 0;
				foreach (var m in this.MissileLaunchers)
					requiredEnergy += m.Energy;

				if (requiredEnergy < this.Energy) {
					battle.DoBattleCommand(this, "M");
					return;
				}
			}

			battle.DoBattleCommand(this, "E");
			
		}

		#endregion

		#region override On -> Enter & Leave System
		protected override void OnEnterSystem(Coords system) {
			if (this.Player != null)
				GameManager.Instance.StarSystemManager.Join(this.Player, system);
			base.OnEnterSystem(system);
		}

		protected override void OnEnterSystem(StarSystem system) {
			if (this.Player != null)
				GameManager.Instance.StarSystemManager.Join(this.Player, system.Coords);
			base.OnEnterSystem(system);
		}

		protected override void OnLeaveSystem(Coords system) {
			if (this.Player != null)
				GameManager.Instance.StarSystemManager.Leave(this.Player, system);
			base.OnLeaveSystem(system);
		}

		protected override void OnLeaveSystem(StarSystem system) {
			if (this.Player != null)
				GameManager.Instance.StarSystemManager.Leave(this.Player, system.Coords);
			base.OnLeaveSystem(system);
		}
		#endregion

		#region override Persist
		public override void Persist() {
			if (this.Player != null && !this.Player.IsNPC)
				base.Persist();
		}
		#endregion

		private Player player = null;

		public Player Player {
			get {
				return player;
			}
			set {
				this.player = value;
				if (player != null)
					base.pId = player.Id;
			} 
		}

		public string Code { get; set; }
		public int Mass { get; private set; }
		public int InternalCap { get; private set; }
		public int GunMoutings { get; private set; }
		public int MissilePylons { get; private set; }
		public Laser[] Lasers { get; private set; }
		public MissileLauncher[] MissileLaunchers { get; internal set; }
		public EnergyGenerator Generator { get; internal set; }
		public int Energy { get; internal set; }
		public int Hull { get; internal set; }
		public int Crew { get; private set; }
		public int MainThrusterAccel { get; private set; }
		public int RetroThrusterAccel { get; private set; }
		public int Drive { get; private set; }
		public int Cost { get; private set; }
		//This is going to be fased out not beeing presisted anymore
		private string ANSI16File { get; set; }
		public string MKIIFile { get; private set; }
		public SectorObject Target { get; set; }

		public bool Docked { get { return SpaceDockId != null && SpaceDockId != string.Empty; } }
		public string SpaceDockId { get; set; } 

		private CargoBay _cargoBay = null;
		public CargoBay CargoBay {
			get {
				if (_cargoBay == null)
					_cargoBay = new CargoBay(this.InternalCap);

				return _cargoBay;
			}
			private set { this._cargoBay = value; } 
		}

		public void EngageShip(SpaceShip ship) {
			if (ship != null && this.player != null) {
				this.player.SpaceShip.Target = ship;
				this.player.SpaceShip.SetDestSystemLocation(ship.SystemLocation);
				GameManager.Instance.BattleManager.CreateBattle(this.player, ship);
			}
		}

		public void DoDamage(int value) {

			if (value == 0) return;

			double damage = (double)value / (double)((Mass / 100) / 3);

			if (double.IsPositiveInfinity(damage))
				damage = 100;
			else if (double.IsNegativeInfinity(damage))
				damage = 0;
			else if (double.IsNaN(damage))
				damage = 0;
			else if (double.IsInfinity(damage))
				damage = 100;

			Hull -= (int)damage;
			if (Hull < 0) Hull = 0;
			if (Hull > 100) Hull = 100;

			if (Hull == 0) {
				//BANG!
			}

			//TODO: write thee code to make damage to the ship;
			// AND Decide how to die... :PPP
		}

		#region ISerializable Members

		public override void GetObjectData(SerializationInfo info, StreamingContext context) {
			base.GetObjectData(info, context);
			info.AddValue("code", Code);
			info.AddValue("pId", pId);
			info.AddValue("mass", Mass);
			info.AddValue("intCap", InternalCap);
			info.AddValue("gMount", GunMoutings);
			info.AddValue("mPyln", MissilePylons);
			info.AddValue("crew", Crew);
			info.AddValue("mThr", MainThrusterAccel);
			info.AddValue("rThr", RetroThrusterAccel);
			info.AddValue("driv", Drive);
			info.AddValue("cost", Cost);
			info.AddValue("h", Hull);
			info.AddValue("g", Lasers);
			info.AddValue("m", MissileLaunchers);
			info.AddValue("gen", Generator);
			//info.AddValue("ANSI16File", this.ANSI16File);
			info.AddValue("MKIIFile", this.MKIIFile);
			info.AddValue("DockId", this.SpaceDockId);
			info.AddValue("cbay", this.CargoBay, typeof(CargoBay));
		}

		public SpaceShip(SerializationInfo info, StreamingContext context) : base(info, context) {

			List<string> sNames = new List<string>();
			foreach (var entry in info) sNames.Add(entry.Name);

			this.Code = info.GetString("code");
			this.Mass = info.GetInt32("mass");
			this.InternalCap = info.GetInt32("intCap");
			this.GunMoutings = info.GetInt32("gMount");
			this.MissilePylons = info.GetInt32("mPyln");
			this.Crew = info.GetInt32("crew");
			this.MainThrusterAccel = info.GetInt32("mThr");
			this.RetroThrusterAccel = info.GetInt32("rThr");
			this.Drive = info.GetInt32("driv");
			this.Cost = info.GetInt32("cost");

			//if (sNames.Contains("ANSI16File"))
			//    this.ANSI16File = info.GetString("ANSI16File");
			//else
			//    this.ANSI16File = "Ship25_16.ans";

			if (sNames.Contains("DockId")) {
				this.SpaceDockId = info.GetString("DockId");
			} else
				this.SpaceDockId = null;

			if (sNames.Contains("MKIIFile")) {
				this.MKIIFile = info.GetString("MKIIFile");
				if (this.MKIIFile.EndsWith(".MKII"))
					this.MKIIFile = this.MKIIFile.Substring(0, this.MKIIFile.Length - 5);
			} else
				this.MKIIFile = "Ship25_Sprite";

			if (sNames.Contains("cbay"))
				this.CargoBay = info.GetValue("cbay", typeof(CargoBay)) as CargoBay;

			base.pId = (Guid?)info.GetValue("pId", typeof(Guid?));

			this.Energy = 100;

			if (sNames.Contains("h")) {
				this.Hull = info.GetInt32("h");
			} else
				this.Hull = 100;

			//if (sNames.Contains("g"))
			//    this.Lasers = (Laser[])info.GetValue("g", typeof(Laser[]));
			//else {
				this.Lasers = new Laser[1];
				this.Lasers[0] = new Laser(this);
				this.Lasers[0].Energy = 5;
				this.Lasers[0].Power = 5;
			//}

			//if (sNames.Contains("m"))
			//    this.MissileLaunchers = (MissileLauncher[])info.GetValue("m", typeof(MissileLauncher[]));
			//else {
				this.MissileLaunchers = new MissileLauncher[1];
				this.MissileLaunchers[0] = new MissileLauncher(this);
				this.MissileLaunchers[0].Energy = 1;
				this.MissileLaunchers[0].Power = 1;
			//}

			//if (sNames.Contains("gen"))
			//    this.Generator = (EnergyGenerator)info.GetValue("gen", typeof(EnergyGenerator));
			//else {
				this.Generator = new EnergyGenerator(this);
				this.Generator.Energy = -5;
				this.Generator.Power = 1;
			//}


				this.Update(false);

		}

		#endregion

		public SpaceShip(Player player) {
			this.Speed = 5;
			this.Code = base.MakeSoCode("CCC-DDDD");
			this.Player = player;
			if (Player != null && Player.SpaceShip == null)
				Player.SpaceShip = this;
		}

		public void JumpTo(Coords dest) {
			SetDestCoords(dest);
		}

		public void FlyTo(SystemLocation dest) {
			SetDestSystemLocation(dest);
		}

		public override void SetCoords(Coords coords) {
			base.SetCoords(coords);
		}

		public override void SetDestCoords(Coords dest) {
			this.Target = null;
			base.SetDestSystemLocation(null);
			base.SetDestCoords(dest);
			if (Player != null)
				Player.Persist();
			else
				this.Persist();
			
		}

		public override void SetDestSystemLocation(SystemLocation dest) {
			base.SetDestSystemLocation(dest);
			if (Player != null)
				Player.Persist();
			else
				this.Persist();
		}


		public bool Update(bool presist) {

			if (Target != null && !this.InSpace) { //Follow Target

				SystemLocation dLocation = null;
				SystemBody body = this.Target as SystemBody;
				if (body != null)
					dLocation = SystemLocation.GetUpdatedLocation(body);
				else
					dLocation = this.Target.SystemLocation;

				if (dLocation.GetInSystemDistance(this.SystemLocation) > (1000))
					SetDestSystemLocation(dLocation);
			}

			this.IsDirty |= base.Update();

			if (this.IsDirty) {
				if (Player != null) {
					Player.Location = new GameLocation(this.Coords);
					Player.Location.UpdateSystemBody(this.SystemLocation, this.InSpace);
					if (presist) {
						Player.Persist();
						return true;
					}
				}
			}
			return false;
		}

		public override bool Update() {

			if (Target != null && !this.InSpace) { //Follow Target

				SystemLocation dLocation = null;
				SystemBody body = this.Target as SystemBody;
				if (body != null)
					dLocation = SystemLocation.GetUpdatedLocation(body);
				else
					dLocation = this.Target.SystemLocation;

				if (dLocation.GetInSystemDistance(this.SystemLocation) > (1000))
					SetDestSystemLocation(dLocation);
			}

			this.IsDirty |= base.Update();

			if (this.IsDirty) {
				if (Player != null) {
					Player.Location = new GameLocation(this.Coords);
					Player.Location.UpdateSystemBody(this.SystemLocation, this.InSpace);
					Player.Persist();
				}
				return true;
			} else
				return false;
		}

		public void BattleTick() {
			if (this.Generator != null)
				this.Generator.Activate();
		}

		public SpaceShip Clone(Player player) {

			SpaceShip newSpaceShip = new SpaceShip(player);

			newSpaceShip.Name = this.Name;
			newSpaceShip.Mass = this.Mass;
			newSpaceShip.Hull = 100;
			newSpaceShip.Energy = 100;
			newSpaceShip.InternalCap = this.InternalCap;
			newSpaceShip.GunMoutings = this.GunMoutings;
			newSpaceShip.MissilePylons = this.MissilePylons;
			newSpaceShip.Crew = this.Crew;
			newSpaceShip.MainThrusterAccel = this.MainThrusterAccel;
			newSpaceShip.RetroThrusterAccel = this.RetroThrusterAccel;
			newSpaceShip.Drive = this.Drive;
			newSpaceShip.Id = Guid.NewGuid();
			newSpaceShip.Code = newSpaceShip.MakeSoCode("CCC-DDDD");
			newSpaceShip.CargoBay = new CargoBay(this.InternalCap);
			newSpaceShip.MKIIFile = this.MKIIFile;
			newSpaceShip.Generator = new EnergyGenerator(newSpaceShip);
			newSpaceShip.Lasers = new Laser[1];
			newSpaceShip.Lasers[0] = new Laser(this);
			

			return newSpaceShip;
		}

		public static SpaceShip CreateRandomModel(string modelName, Player player) {
			var defaultModels = SpaceShip.GetDefaultModels();
			Random rand = new Random();
			int modelNum = rand.Next(0, defaultModels.Count - 1);
			return defaultModels.Values.ElementAt(modelNum).Clone(player);
		}

		public static SpaceShip CreateModel(string modelName, Player player) {
			var defaultModels = SpaceShip.GetDefaultModels();
			if (defaultModels.ContainsKey(modelName)) {
				SpaceShip ship = defaultModels[modelName].Clone(player);
				GameLocation location = player.Location;
				if (location.SystemBody != null) {
					ship.Coords = location.SystemBody.Coords;
					ship.SystemLocation = location.SystemBody.SystemLocation;
				} else if (location.StarSystem != null) {
					ship.Coords = location.StarSystem.Coords;
					ship.SystemLocation = new SystemLocation(0, 0, 0);
					ship.SystemLocation.RandomizeInSystemCoords();
				} else if (location.Sector != null) {
					ship.Coords = location.Sector.Coords;
					ship.SystemLocation = new SystemLocation(
						Double.PositiveInfinity, Double.PositiveInfinity, Double.PositiveInfinity
					);
				} else {
					// this can't be possible
					ship.Coords = new Coords(new GalacticLocation(0, 0, 0));
					ship.SystemLocation = new SystemLocation(
						Double.PositiveInfinity, Double.PositiveInfinity, Double.PositiveInfinity
					);
				}
				
				return ship;
			} else
				return null;
		}

		private static Dictionary<string, SpaceShip> GetDefaultModels() {

			Dictionary<string, SpaceShip> defaults = new Dictionary<string, SpaceShip>();

			SpaceShip adder = new SpaceShip(null);

			adder.Name = "Adder";
			adder.Mass = 55;
			adder.InternalCap = 15;
			adder.Crew = 1;
			adder.GunMoutings = 1;
			adder.MissilePylons = 0;
			adder.Drive = 2;
			adder.Cost = 73000;
			adder.MainThrusterAccel = 18;
			adder.RetroThrusterAccel = 8;
			adder.CargoBay = new CargoBay(15);
			adder.Speed = 5;
			adder.MKIIFile = "Ship41_Sprite";

			defaults.Add("Adder", adder);


			SpaceShip anaconda = new SpaceShip(null);
			anaconda.Name = "Anaconda";
			anaconda.Mass = 800;
			anaconda.InternalCap = 150;
			anaconda.Crew = 10;
			anaconda.GunMoutings = 2;
			anaconda.MissilePylons = 8;
			anaconda.Drive = 6;
			anaconda.Cost = 1060000;
			anaconda.MainThrusterAccel = 6;
			anaconda.RetroThrusterAccel = 3;
			anaconda.CargoBay = new CargoBay(150);
			anaconda.Speed = 15;
			//anaconda.ANSI16File = "Ship23_16.ans";
			anaconda.MKIIFile = "Ship23_Sprite";

			defaults.Add("Anaconda", anaconda);

			SpaceShip asp = new SpaceShip(null);
			asp.Name = "Asp Explorer";
			asp.Mass = 150;
			asp.InternalCap = 30;
			asp.Crew = 2;
			asp.GunMoutings = 2;
			asp.MissilePylons = 1;
			asp.Drive = 3;
			asp.Cost = 187000;
			asp.MainThrusterAccel = 22;
			asp.RetroThrusterAccel = 7;
			asp.CargoBay = new CargoBay(30);
			asp.Speed = 20;
			//asp.ANSI16File = "Ship12_16.ans";
			asp.MKIIFile = "Ship12_Sprite";

			defaults.Add("Asp", asp);

			return defaults;

		}



	}
}
