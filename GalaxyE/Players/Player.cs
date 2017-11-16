using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaxyE.Galaxy;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Data.SqlClient;
using GalaxyE.DB;

namespace GalaxyE.Players {

	public enum PlayerMessageType { Battle, BattleWarning, Warning, Channel, Say }

	public class PlayerMessage {
		public PlayerMessageType Type { get; set; }
		public string Message { get; set; }
		public string From { get; set; }

		public PlayerMessage(string from, PlayerMessageType type, string message) {
			this.Type = type;
			this.Message = message;
			this.From = from;
		}
	}

	public delegate void PlayerMessageEventHandler(Player sender, PlayerMessage message);
	public delegate void ShipEventHandler (Player player, SpaceShip ship);

	[Serializable]
	public class Player : ISerializable, IPersistable {

		public Guid Id { get; set; }
		public string Name { get; set; }
		public string EMail { get; set; }
		public string Password { get; set; }
		public int Confirmed { get; set; }
		public DateTime LastLogon { get; set; }
		public DateTime CreationDate { get; set; }
		public int LoginType { get; set; }
		public long Money { get; set; }
		public int Loan { get; set; }
		public bool InShip { get; set; }
		public string LoginToken { get; set; }
		public string ExternalLogin { get; set; }
		public GameLocation Location { get; set; }
		public virtual bool IsNPC { get { return false; } }

		private bool _isDirty = false;
		public bool IsDirty {
			get { return _isDirty; }
			set {
				_isDirty = value;
				this.SpaceShip.IsDirty |= value;
			}
		}

		public event PlayerMessageEventHandler SendMessage;
		public event ShipEventHandler ShipEnterSystem;
		public event ShipEventHandler ShipLeaveSystem;

		public void SendPlayerMessage(PlayerMessage message) {
			OnSendMessage(message);
		}

		protected void OnSendMessage(PlayerMessage message) {
			if (SendMessage != null)
				SendMessage(this, message);
		}

		public void Send_ShipEnterSystem(SpaceShip ship) {
			OnShipEnterSystem(ship);
		}

		protected void OnShipEnterSystem(SpaceShip ship) {
			if (ShipEnterSystem != null)
				ShipEnterSystem(this, ship);
		}

		public void Send_ShipLeaveSystem(SpaceShip ship) {
			OnShipLeaveSystem(ship);
		}

		protected void OnShipLeaveSystem(SpaceShip ship) {
			if (ShipLeaveSystem != null)
				ShipLeaveSystem(this, ship);
		}

		private SpaceShip _spaceShip;
		public SpaceShip SpaceShip {
			get { return _spaceShip; }
			set { 
				_spaceShip = value;
				if (_spaceShip != null)
					_spaceShip.Player = this;
			}
		}

		#region constructor login % create player
		protected Player(Guid playerId, GameLocation location) {
			this.Id = playerId;
			this.Location = location;
			this.SpaceShip = SpaceShip.CreateModel("Adder", this);
			this.Name = "";
		}

		public static Player Login(string name, string password) {
			return Player.GetObjectByLogin(name, password);
		}

		public static Player CreateNewPlayer(string name, string externalLogin, string email, int loginType) {
			Player player = new Player(Guid.NewGuid(), GetStartLocation());

			player.Name = name;
			player.Password = string.Empty;
			player.EMail = email;
			player.Confirmed = 1;
			player.LastLogon = DateTime.Now;
			player.CreationDate = DateTime.Now;
			player.LoginType = loginType;
			player.LoginToken = Guid.NewGuid().ToString();
			player.ExternalLogin = externalLogin;

			player.Persist();	

			return player;
		}

		public static Player CreateNewPlayer(string name, string password, string email) {
			Player player = new Player(Guid.NewGuid(), GetStartLocation());

			player.Name = name;
			player.Password = password;
			player.EMail = email;
			player.Confirmed = 0;
			player.LastLogon = DateTime.Now;
			player.CreationDate = DateTime.Now;
			player.LoginType = 0;
			player.LoginToken = Guid.NewGuid().ToString();
			player.ExternalLogin = null;

			player.Persist();

			return player;
		}
		#endregion

		#region Market Operations

		//Todo check limits
		public void MakeLoan(int amount) {
			if (Loan < 8000) {
				this.Loan += amount;
				this.Money += amount;
			}
		}

		public void PlayLoan(int amount) {
			this.Loan -= amount;
			this.Money -= amount;
		}

		public Market.Market.MarketOperationResult Buy(int goodId, int quantity, Market.Market market) {

			if (quantity > this.SpaceShip.CargoBay.FreeSpace)
				quantity = this.SpaceShip.CargoBay.FreeSpace;

			int price = market[goodId].Price * quantity;
			if (price < Money) {
				var ret = market[goodId].Buy(quantity);
				this.SpaceShip.CargoBay.StoreGoods(goodId, ret.Quantity);
				return ret;
			} else {
				int payableQuant = (int)Money / market[goodId].Price;
				var ret = market[goodId].Buy(payableQuant);
				this.SpaceShip.CargoBay.StoreGoods(goodId, ret.Quantity);
				return ret;
			}

		}

		public Market.Market.MarketOperationResult Sell(int goodId, int quantity, Market.Market market) {

			int actQuantity = this.SpaceShip.CargoBay.GetGoods(goodId, quantity);
			var ret = market[goodId].Sell(actQuantity);

			this.Money += ret.Money;
				
			return ret;
		}

		#endregion

		public virtual bool Update(bool precist) {
			return SpaceShip.Update(precist);
		}

		public virtual bool Update() {
			return SpaceShip.Update();
		}

		private static GameLocation GetStartLocation() {
			GameLocation location = new GameLocation("4111.6303.0.2", false);
			return location;
		}

		public string GetSystemInformation() {
			return GetSystemInformation(this.Location);
		}

		public string GetSystemInformation(string locationCode, bool isBase36) {
			GameLocation location = new GameLocation(locationCode, isBase36);
			return GetSystemInformation(location);
		}

		public string GetSystemInformation(GameLocation location) {
			if (location.StarSystem != null) {
				return location.StarSystem.Name;
			}

			return null;
		}

		//public string GetLocalInformation() {
		//    if (this.Location.Zone != null)
		//        return this.Location.Zone.Description;
		//    if (this.Location.SystemBody != null)
		//        return this.Location.SystemBody.Description;
		//    if (this.Location.StarSystem != null)
		//        return this.Location.StarSystem.Name;
		//    else if (this.Location.Sector != null)
		//        return string.Format("{0} \nNo more information available.", this.Location.Sector);
		//    else
		//        return "No information available.";
		//}

		public double GetDistance(Coords coords) {

			if (this.Location.StarSystem != null)
				return this.Location.StarSystem.Coords.CalculateDistance(coords);
			else if (this.Location.Sector != null) {
				return this.Location.Sector.Coords.CalculateDistance(coords);
			} else
				return Double.PositiveInfinity;

		}

		public List<StarSystem> FindStarSystem(string name) { return FindStarSystem(name, false); }

		public List<StarSystem> FindStarSystem(string name, bool sort) {
			return FindStarSystem(name, 100, sort);
		}

		public List<StarSystem> FindStarSystem(string name, uint range, bool sort) {

			if (range > 6000) range = 6000;

			List<StarSystem> returnList = new List<StarSystem>();
			List<StarSystem> systems = GetSystemsInRange(range, false);

			foreach (var sys in systems) {
				if (sys.Name.ToLower().StartsWith(name.ToLower())) {
					returnList.Add(sys);
				}
			}

			if (sort) {
				returnList.Sort(
					delegate(StarSystem a, StarSystem b) {
						var distA = this.GetDistance(a.Coords);
						var distB = this.GetDistance(b.Coords);
						return (distA > distB) ? 1 : (distA < distB) ? -1 : 0;
					}
				);
			}

			return returnList;
			
		}

		public List<StarSystem> GetSystemsInRange(uint range) { return GetSystemsInRange(range, false); }

		public List<StarSystem> GetSystemsInRange(uint range, bool sort) {

			List<StarSystem> returnList = new List<StarSystem>();

			int sectors = (int)((range + 10) * 2 / GalaxyConstants.SectorWidth);

			Sector sector = null;
			Coords coords = null;

			if (this.SpaceShip != null) {
				sector = this.SpaceShip.Coords.GetSector();
				coords = this.SpaceShip.Coords;
			} else {
				sector = this.Location.Sector;
				if (sector != null)
					coords = sector.Coords;
			}

			if (sector != null && coords != null) {
				for (int x = -(sectors >> 1); x < (sectors >> 1) + 1; x++) {
					for (int y = -(sectors >> 1); y < (sectors >> 1) + 1; y++) {

						Sector sec = new Sector(
							(uint)(sector.IndexX + x),
							(uint)(sector.IndexY + y)
						);

						for (int s = 0; s < sec.Systems.Count; s++) {

							if (s > 0 && (s & 0xFFF) == 0) 
								System.Threading.Thread.Sleep(0);

							var distance = coords.CalculateDistance(sec.Systems[s].Coords);
							if (distance < range) {
								returnList.Add(sec.Systems[s]);
							}
						}
					}
				}
			}

			if (sort) {
				returnList.Sort(
					delegate(StarSystem a, StarSystem b) {
						var distA = this.GetDistance(a.Coords);
						var distB = this.GetDistance(b.Coords);
						return (distA > distB) ? 1 : (distA < distB) ? -1 : 0;
					}
				);
			}

			return returnList;

		}

		#region IPersistable Members

		public static Player Deserialize(byte[] data) {
			MemoryStream ms = new MemoryStream(data);
			BinaryFormatter formatter = new BinaryFormatter();
			Player obj = formatter.Deserialize(ms) as Player;
			return obj;
		}

		public static Player GetObjectByLogin(string name, string password) {

			Player p = Database.GetObject<Player>("Player", name);
			if (p != null) {
				if (p.Password == password) {
					return p;
				}
			}

			return null;


			#region old code
			//SqlConnection conn = new SqlConnection(Parameters.Instance.SqlConnectionString);
			//SqlCommand comm = new SqlCommand("SELECT * FROM dbo.GetPlayerByLogin (@EMail, @Password)", conn);

			//comm.Parameters.Add(new SqlParameter("@EMail", email));
			//comm.Parameters.Add(new SqlParameter("@Password", password));

			//try {
			//    conn.Open();
			//    var reader = comm.ExecuteReader();

			//    if (reader.Read()) {
			//        byte[] data = (byte[])reader["Object"]; //all the other information is for SQL Usage
			//        Player obj = Player.Deserialize(data);
			//        return obj;
			//    }

			//} finally {
			//    if (conn.State != System.Data.ConnectionState.Closed)
			//        conn.Close();
			//}

			//return null;
			#endregion
		}

		//public static Player GetObject(Guid id) {
		//    SqlConnection conn = new SqlConnection(Parameters.Instance.SqlConnectionString);
		//    SqlCommand comm = new SqlCommand("SELECT * FROM dbo.GetPlayerById (@Id)", conn);

		//    comm.Parameters.Add(new SqlParameter("@Id", id));

		//    try {
		//        conn.Open();
		//        var reader = comm.ExecuteReader();

		//        if (reader.Read()) {
		//            byte[] data = (byte[])reader["Object"]; //all the other information is for SQL Usage
		//            Player obj = Player.Deserialize(data);
		//            return obj;
		//        }

		//    } finally {
		//        if (conn.State != System.Data.ConnectionState.Closed)
		//            conn.Close();
		//    }

		//    return null;
		//}

		public virtual void Persist() {

			if (IsNPC) return;

			if (IsDirty) {
				Database.Persist("Player", this.Name, this);
				IsDirty = false;
			}

			this.SpaceShip.Persist();

		}

		#endregion

		#region ISerializable Members

		public void GetObjectData(SerializationInfo info, StreamingContext context) {

			info.AddValue("id", this.Id);
			info.AddValue("name", this.Name);
			info.AddValue("mail", this.EMail);
			info.AddValue("password", this.Password);
			info.AddValue("confirm", this.Confirmed);
			info.AddValue("llogon", this.LastLogon);
			info.AddValue("date", this.CreationDate);
			info.AddValue("lType", this.LoginType);
			info.AddValue("token", this.LoginToken);
			info.AddValue("extLogin", this.ExternalLogin);
			info.AddValue("sId", this.SpaceShip.Id);
			info.AddValue("loc", this.Location.ToString());
			info.AddValue("money", this.Money);
			info.AddValue("loan", this.Loan);
			info.AddValue("inshp", this.InShip);

		}

		public Player(SerializationInfo info, StreamingContext context) {

			List<string> sNames = new List<string>();
			foreach (var entry in info) sNames.Add(entry.Name);

			if (sNames.Contains("loan")) this.Loan = (int)info.GetUInt32("loan");
			if (sNames.Contains("money")) this.Money = (long)info.GetUInt64("money");

			this.Id = (Guid)info.GetValue("id", typeof(Guid));
			this.Name = info.GetString("name");
			this.EMail = info.GetString("mail");
			this.Password = info.GetString("password");
			this.Confirmed = info.GetInt32("confirm");
			this.LastLogon = info.GetDateTime("llogon");
			this.CreationDate = info.GetDateTime("date");

			if (sNames.Contains("lType")) {
				try { this.LoginType = info.GetInt32("lType"); } catch { } //runft
			}

			this.LoginToken = info.GetString("token");
			this.ExternalLogin = info.GetString("extLogin");
			this.Location = new GameLocation(info.GetString("loc"), true);

			Guid sId = (Guid)info.GetValue("sId", typeof(Guid));

			if (sId != Guid.Empty) {
				try {
					this.SpaceShip = PersistentSectorObject.GetObject(sId) as SpaceShip;
				} catch {
					Console.WriteLine("Couldn't load player space ship Id {0}", sId);
				}
			} 
			
			if (this.SpaceShip == null) {
				this.SpaceShip = SpaceShip.CreateModel("Adder", this);
				this.SpaceShip.Persist();
			}
			

			this.SpaceShip.Update(false);

			if (sNames.Contains("inshp")) {
				this.InShip = info.GetBoolean("inshp");
				if (InShip) {
					this.Location = new GameLocation(this.SpaceShip.Coords);
					this.Location.UpdateSystemBody(this.SpaceShip.SystemLocation, false);
				}
			}
		}

		#endregion
	}
}
