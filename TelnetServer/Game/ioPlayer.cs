using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TelnetServer.SocketServer;
using System.Reflection;
using TelnetServer.Messages;
using GalaxyE;
using GalaxyE.Galaxy;
using GalaxyE.Players;
using GalaxyE.Market;
using GalaxyE.DB;
using GalaxyE.Galaxy.Zones;
using GalaxyE.Managers;


namespace TelnetServer.Game {

	[Flags]
	public enum ioPlayerMode {
		None = 0,
		Space = 1,
		Planet = 2,
		Battle = 4,
		LoggedIn = 8,
		SpaceBattle = Space | Battle,
		PlanetBattle = Planet | Battle
	}

	class ioPlayer {

		internal enum PlayerStateEnum {
			WaitingForInit,
            Init,
            WaitingForCommand
		}

		private static Dictionary<string, CommandInfo> commands = new Dictionary<string, CommandInfo>();

		class CommandInfo {

			public MethodInfo Method { get; private set; }
			public int Delay { get; private set; }
			public ioPlayerMode RequiredMode { get; private set; }
			public string Usage { get; set; }
			public string Help { get; set; } 
            public string Name { get; set; }
            public int RequiredParameters { get; set; }

			public CommandInfo(MethodInfo method, string name, int delay) {
				this.Method = method;
				this.Delay = delay;
				this.RequiredMode = ioPlayerMode.None;
                this.Name = name;
				TryGetMetaInfo();
			}

			public CommandInfo(MethodInfo method, string name, int delay, ioPlayerMode required) {
				this.Method = method;
				this.Delay = delay;
				this.RequiredMode = required;
                this.Name = name;
				TryGetMetaInfo();
			}

			private void TryGetMetaInfo() {
				if (this.Method != null) {
					Type type = this.Method.DeclaringType;
					var mInfo = type.GetMethod(string.Format("__{0}_Info", this.Method.Name), 
						BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
					if (mInfo != null) {
						mInfo.Invoke(null, new object[] { this });
                        this.Help = this.Help.Replace("#Command#", this.Name);
                        this.Usage = this.Usage.Replace("#Command#", this.Name);
                        Console.WriteLine("Adding meta info to method");
                        return;
					}
				}

                this.Help = string.Format("Command '{0}' help not available", this.Name);
                this.Usage = this.Name; ;
			}

		}

		class InvokeInfo {
			public CommandInfo CommandInfo { get; private set; }
			public object[] Arguments { get; private set; }

			public InvokeInfo(CommandInfo commandInfo, object[] arguments) {
				this.CommandInfo = commandInfo;
				this.Arguments = arguments;
			}
		}

		static ioPlayer() {
			Type type = typeof(ioPlayer);
			commands.Add("help", new CommandInfo(type.GetMethod("CmdHelp"), "help", 0));
			commands.Add("encoding", new CommandInfo(type.GetMethod("CmdEncoding"), "enconding", 0));
			commands.Add("register", new CommandInfo(type.GetMethod("CmdRegister"), "register", 0));
			commands.Add("connect", new CommandInfo(type.GetMethod("CmdConnect"), "connect", 0));
			commands.Add("login", new CommandInfo(type.GetMethod("CmdConnect"), "login", 0));
			commands.Add("sector", new CommandInfo(type.GetMethod("CmdSectorMap"), "sector", 0, ioPlayerMode.LoggedIn));
			commands.Add("sec", new CommandInfo(type.GetMethod("CmdSectorMap"), "sec", 0, ioPlayerMode.LoggedIn));
			commands.Add("region", new CommandInfo(type.GetMethod("CmdRegion"), "region", 0, ioPlayerMode.LoggedIn));
			commands.Add("galaxy", new CommandInfo(type.GetMethod("CmdGalaxyMap"), "galaxy", 0, ioPlayerMode.LoggedIn));
			commands.Add("coords", new CommandInfo(type.GetMethod("CmdGetCoords"), "coords", 0, ioPlayerMode.LoggedIn));
			commands.Add("coordinates", new CommandInfo(type.GetMethod("CmdGetCoords"), "coordinates", 0, ioPlayerMode.LoggedIn));
			commands.Add("system", new CommandInfo(type.GetMethod("CmdSystemInfo"), "system", 0, ioPlayerMode.LoggedIn));
			commands.Add("sys", new CommandInfo(type.GetMethod("CmdSystemInfo"), "sys", 0, ioPlayerMode.LoggedIn));
			commands.Add("planet", new CommandInfo(type.GetMethod("CmdSystemBodyInfo"), "planet", 0, ioPlayerMode.LoggedIn));
			commands.Add("jump", new CommandInfo(type.GetMethod("CmdJump"), "jump", 0, ioPlayerMode.LoggedIn));
			commands.Add("jmp", new CommandInfo(type.GetMethod("CmdJump"), "jmp", 0, ioPlayerMode.LoggedIn));
			commands.Add("abortjump", new CommandInfo(type.GetMethod("CmdAbortJump"), "abortJump", 0, ioPlayerMode.LoggedIn));
			commands.Add("ajmp", new CommandInfo(type.GetMethod("CmdAbortJump"), "ajmp", 0, ioPlayerMode.LoggedIn));
			commands.Add("ship", new CommandInfo(type.GetMethod("CmdShipInfo"), "ship", 0, ioPlayerMode.LoggedIn));
			commands.Add("scan", new CommandInfo(type.GetMethod("CmdListSystemShips"), "scan", 0, ioPlayerMode.LoggedIn));
			commands.Add("move", new CommandInfo(type.GetMethod("CmdMoveToTarget"), "move", 0, ioPlayerMode.LoggedIn));
			commands.Add("dock", new CommandInfo(type.GetMethod("CmdDock"), "dock", 0, ioPlayerMode.LoggedIn));
			commands.Add("launch", new CommandInfo(type.GetMethod("CmdLaunch"), "launch", 0, ioPlayerMode.LoggedIn));
			commands.Add("hud", new CommandInfo(type.GetMethod("Cmd_HUD"), "hud", 0, ioPlayerMode.LoggedIn));
			commands.Add("see", new CommandInfo(type.GetMethod("Cmd_Look"), "see", 0, ioPlayerMode.LoggedIn));
			commands.Add("look", new CommandInfo(type.GetMethod("Cmd_Look"), "lock", 0, ioPlayerMode.LoggedIn));

			//commands.Add("search", new CommandInfo(type.GetMethod("CmdSearch"), 0)); //very dangerouse command

			commands.Add("engage", new CommandInfo(type.GetMethod("CmdEngageShip"), "engage", 0, ioPlayerMode.LoggedIn));
			commands.Add("battle", new CommandInfo(type.GetMethod("CmdBattle"), "battle", 0, ioPlayerMode.LoggedIn));
			commands.Add("laser", new CommandInfo(type.GetMethod("CmdFireLasers"), "laser", 0, ioPlayerMode.LoggedIn));
			//commands.Add("missile", new CommandInfo(null, 0, ioPlayerMode.LoggedIn));
			//commands.Add("shield", new CommandInfo(null, 0, ioPlayerMode.LoggedIn));
			//commands.Add("jammer", new CommandInfo(null, 0, ioPlayerMode.LoggedIn));
			//commands.Add("evade", new CommandInfo(null, 0, ioPlayerMode.LoggedIn));

			//commands.Add("fg", new CommandInfo(null, 0, ioPlayerMode.LoggedIn));
			//commands.Add("fm", new CommandInfo(null, 0, ioPlayerMode.LoggedIn));
			//commands.Add("sd", new CommandInfo(null, 0, ioPlayerMode.LoggedIn));
			//commands.Add("jm", new CommandInfo(null, 0, ioPlayerMode.LoggedIn));
			//commands.Add("ev", new CommandInfo(null, 0, ioPlayerMode.LoggedIn));

			//commands.Add("gn", new CommandInfo(null, 0, ioPlayerMode.LoggedIn));
			//commands.Add("gs", new CommandInfo(null, 0, ioPlayerMode.LoggedIn));
			//commands.Add("ge", new CommandInfo(null, 0, ioPlayerMode.LoggedIn));
			//commands.Add("gw", new CommandInfo(null, 0, ioPlayerMode.LoggedIn));
			//commands.Add("gd", new CommandInfo(null, 0, ioPlayerMode.LoggedIn));
			//commands.Add("gu", new CommandInfo(null, 0, ioPlayerMode.LoggedIn));

			commands.Add("city", new CommandInfo(type.GetMethod("CmdTestCity"), "city", 0));
			commands.Add("createdirs", new CommandInfo(type.GetMethod("Cmd_CreateDirectories"), "createdirs", 0, ioPlayerMode.LoggedIn));

			commands.Add("market", new CommandInfo(type.GetMethod("CmdMarket"), "market", 0, ioPlayerMode.LoggedIn));
			commands.Add("quote", new CommandInfo(type.GetMethod("CmdMarketQuote"), "quote", 0, ioPlayerMode.LoggedIn));
			commands.Add("buy", new CommandInfo(type.GetMethod("CmdMarketBuy"), "buy", 0, ioPlayerMode.LoggedIn));
			commands.Add("sell", new CommandInfo(type.GetMethod("CmdMarketSell"), "sell", 0, ioPlayerMode.LoggedIn));

			//commands.Add("jump", type.GetMethod("JumpTo"));
			//commands.Add("j", type.GetMethod("JumpTo"));
			//commands.Add("info", type.GetMethod("LocalInfo"));
			//commands.Add("i", type.GetMethod("LocalInfo"));
			//commands.Add("bbs", type.GetMethod("OpenBBS"));
			//commands.Add("opt", type.GetMethod("SelectBBSOption"));
			//commands.Add("range", type.GetMethod("SystemsInRange"));
			//commands.Add("chart", type.GetMethod("SectorChart"));
			//commands.Add("abort", type.GetMethod("Abort"));
			//commands.Add("say", type.GetMethod("SayChannel"));
			//commands.Add("chkmsg", type.GetMethod("CheckMessages"));
			//commands.Add("join", type.GetMethod("JoinChannel"));
			//commands.Add("leave", type.GetMethod("LeaveChannel"));
			//commands.Add("sector", type.GetMethod("SystemsInSector"));
			//commands.Add("jumpstatus", type.GetMethod("JumpStatus"));
			//commands.Add("loginstatus", type.GetMethod("LoginStatus"));
			//commands.Add("orbit", type.GetMethod("flyTo"));
			//commands.Add("buy", type.GetMethod("Buy"));
			//commands.Add("sell", type.GetMethod("Sell"));
			//commands.Add("market", type.GetMethod("QueryMarket"));
		}

		private xConnection conn;
		private PlayerStateEnum playerState = PlayerStateEnum.WaitingForInit;
		private CommandInfo defaultCommand = null;
		private System.Threading.ReaderWriterLock rwl = new System.Threading.ReaderWriterLock();
		private Player player;
		private GameManager game;
		private ioPlayerMode playerMode;

		#region PlayerMode Commands

		public bool InMode(ioPlayerMode mode) {
			return (this.playerMode & mode) == mode;
		}

		public void SetMode(ioPlayerMode mode, bool active) {
			if (active)
				playerMode |= mode;
			else
				playerMode &= ~mode;
		}

		public void ClearModes() {
			playerMode = ioPlayerMode.None;
		}


		#endregion

		public Player Player {
			get { return player; }
		}

		private bool inHyperspace = false;
		private bool inSpace = false;
		private System.Threading.Timer tickTimer = null;

		private int jobsRunning = 0;
		public bool ProcessingCommand { 
			get {

				if ((jobsRunning > 1 && commandQueue.Count == 0) || jobsRunning < 0) {
					lock (commandQueueLocker) {
						if ((jobsRunning > 1 && commandQueue.Count == 0) || jobsRunning < 0) {
							//some stupid shit append... log it an release everything let the new commands flow;
							//TODO: log it :PP
							jobsRunning = 0;
						}
					}
				}

				return jobsRunning > 0; 
			} 
		}

		internal PlayerStateEnum PlayerState {
			get { return playerState; }
			set { playerState = value; }
		}

		public ioPlayer(xConnection conn) {
			this.conn = conn;
		}

		public string GetPrompt() {
			switch (playerState) {
				case PlayerStateEnum.WaitingForCommand:

					StringBuilder sBuilder = new StringBuilder();

					if (this.player != null) {

						sBuilder.AppendLine(string.Empty.PadLeft(Math.Min(this.conn.TerminalWidth, 79), '-'));
						if (this.player.SpaceShip != null && !this.player.SpaceShip.InPlanet) {
							sBuilder.AppendLine(ioShipInfo.GetStatusString(this.player.SpaceShip));
						}

						sBuilder.Append(player.Name.ToLower());

						if (player.Location != null) {

							if (this.player.Location.Sector != null)
								sBuilder.AppendFormat("@{0}", this.player.Location.Sector.ToString());

							if (this.player.SpaceShip != null && this.player.SpaceShip.InHiperspace) {
								sBuilder.AppendFormat("/HIPERSPACE");
							} else {
								if (this.player.Location.StarSystem != null)
									sBuilder.AppendFormat("/{0}", this.player.Location.StarSystem.Name);

								if (this.player.Location.SystemBody != null)
									sBuilder.AppendFormat("/{0}", this.player.Location.SystemBody.Name);
								else
									sBuilder.AppendFormat("/SPACE");
							}

							//this.player.SpaceShip.GetETA();
						}
					}

					sBuilder.Append(":");
					return sBuilder.ToString();
			}
			return "";
		}

		public void Init() {
			this.game = GameManager.Instance; //Let the fun begin;
            this.playerState = PlayerStateEnum.Init;

            System.Threading.Thread nThread = new System.Threading.Thread(
                new System.Threading.ParameterizedThreadStart(DoSendBanner));

            nThread.IsBackground = true;
            nThread.Priority = System.Threading.ThreadPriority.Normal;
            nThread.Start();


        }

        public void DoSendBanner(object state) {
            StartSendData();

            System.Threading.Thread.Sleep(4000);
            this.playerState = PlayerStateEnum.WaitingForCommand;

            SendLine(Message.ReadDefinedMessage("gamebanner"));
            if (conn.GetServerOption((byte)TelnetServer.SocketServer.xConnection.TelnetOption.Echo)) {
                SendLine(ANSI.SetColorANSI16("To provide a better experience the command prompt is hidden by default.", ANSI.ANSIColor_16.Darkgray));
                SendLine(ANSI.SetColorANSI16("To show the prompt hit [Enter] or start typing.", ANSI.ANSIColor_16.Darkgray));
                SendLine(ANSI.SetColorANSI16("If your system doen't require server echo the prompt will not be shown at all", ANSI.ANSIColor_16.Darkgray));
            }

            EndSendData();
        }


        public void StartSendData() {
			System.Threading.Interlocked.Increment(ref jobsRunning);
		}

		internal void AcquireSendDataLock() {
			if (!rwl.IsWriterLockHeld) {
				if (rwl.IsReaderLockHeld) rwl.ReleaseReaderLock();
				rwl.AcquireWriterLock(-1);
			}
		}

		internal void ReleaseSendDataLock() {
			if (rwl.IsReaderLockHeld) rwl.ReleaseReaderLock();
			if (rwl.IsWriterLockHeld) rwl.ReleaseWriterLock();
		}

        public void Send(string data) {
            _SendData(ANSI.EscapeFormatingCodes(data));
        }

        public void SendFormat(string format, params object[] objs) {
			_SendData(string.Format(ANSI.EscapeFormatingCodes(format), objs));
		}

        public void SendLine(string format, params object[] objs) {
            _SendData(string.Format(
                "{0}\r\n", 
                string.Format(ANSI.EscapeFormatingCodes(format), objs)
            ));
		}

		private void _SendData(string data) {
			bool hasLock = rwl.IsWriterLockHeld || rwl.IsReaderLockHeld;
			if (!hasLock) {
				rwl.AcquireReaderLock(-1);
				try {
					conn.Send(data);
				} finally {
					rwl.ReleaseReaderLock();
				}
			} else
				conn.Send(data);
		}

		public void EndSendData() {
			EndSendData("", true);
		}

		public void EndSendData(string data, bool stopProcessingCommand) {

			System.Threading.Interlocked.Decrement(ref jobsRunning);
			bool doEnd = true;

			if (this.player != null) {
				this.player.IsDirty = true;
				this.player.Update(true); //save changes
			}

			//safety measures
			if (rwl.IsWriterLockHeld) rwl.ReleaseWriterLock();
			if (rwl.IsReaderLockHeld) rwl.ReleaseReaderLock();

			if (commandQueue.Count > 0) {
				CommandData command = null;
				lock (commandQueueLocker) {
					if (commandQueue.Count > 0) {
						command = commandQueue.Dequeue();
					}
				}
				if (command != null) {
					DoCommand(command.Command, command.Args);
					doEnd = false;
				}
			}

			if (doEnd) {
				conn.Send(string.Format("{0}", data));
				//conn.SafeWithPrompt(this.GetPrompt(), string.Format("{0}", data));
			}
		}

		public void ProcessRawCommand(string command) {
			if (command != null) {
				string[] tokens = command.Trim().Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
				if (tokens.Length > 0)
					DoCommand(tokens[0], tokens);
			}
		}

		private class CommandData {
			public string Command { get; private set; }
			public string[] Args { get; private set; }
			public CommandData(string command, string[] args) {
				this.Command = command;
				this.Args = args;
			}
		}

		private void InvokeBgCommand(object state) {
			try {
				InvokeInfo info = state as InvokeInfo;
				if (info != null) {
					object[] args = info.Arguments;
					CommandInfo cInfo = info.CommandInfo;

					if (cInfo.Delay > 0)
						System.Threading.Thread.Sleep(cInfo.Delay);

					MethodInfo mCommand = cInfo.Method;
                    if (mCommand != null) {
                        object ret = mCommand.Invoke(this, args);
                        if (ret is bool) {
                            if (!(bool)ret) {
                                SendLine(cInfo.Usage);
                            }
                        }
                    }
				}



			} catch (Exception ex) {
				while (ex.InnerException != null) {
					ex = ex.InnerException; //TODO Store errors
					SendLine(ex.ToString());
				}
			}

			EndSendData();
		}

		private object commandQueueLocker = new object();
		private Queue<CommandData> commandQueue = new Queue<CommandData>();

		public void DoCommand(string command, params string[] args) {

			if (ProcessingCommand) {
				lock (commandQueueLocker) {
					commandQueue.Enqueue(new CommandData(command, args));
				}
				return;
			}

			StartSendData();

			try {
				CommandInfo mCommand = null;

				if (defaultCommand != null)
					mCommand = defaultCommand;
				else if (commands.ContainsKey(command)) {
					mCommand = commands[command];
				}

				if (mCommand != null) {

					if (!this.InMode(mCommand.RequiredMode)) {
						SendLine("This Command cannot be executed at this time (needs: {0})", mCommand.RequiredMode);
						EndSendData();
						return;
					}

					if (player != null) {
						player.Update(false);
						if (tickTimer == null) {
							tickTimer = new System.Threading.Timer(
								new System.Threading.TimerCallback(PlayerTick),
								new PlayerTickState(true),
								System.Threading.Timeout.Infinite,
								System.Threading.Timeout.Infinite);
						}
						PlayerTick(new PlayerTickState(false));
					}


					System.Threading.Thread wThread = new System.Threading.Thread(
						new System.Threading.ParameterizedThreadStart(InvokeBgCommand));

					wThread.IsBackground = true;
					wThread.Start(new InvokeInfo(mCommand, new object[] { args }));

				} else {
					SendLine("Command not found");
					EndSendData();
				}

			} catch (Exception ex) {
				while (ex.InnerException != null) {
					ex = ex.InnerException; //TODO Store errors
					SendLine(ex.ToString());
					EndSendData();
				}
			}

		}

		private void ProcessPlayerInit() {

			player.SpaceShip.EnterSystem += new EnterSystemEventHandler(SpaceShip_EnterSystem);
			player.SpaceShip.LeaveSystem += new LeaveSystemEventhandler(SpaceShip_LeaveSystem);
			player.SendMessage += new PlayerMessageEventHandler(player_SendMessage);
			player.ShipEnterSystem += new ShipEventHandler(player_ShipEnterSystem);
			player.ShipLeaveSystem += new ShipEventHandler(player_ShipLeaveSystem); 

			this.game.StarSystemManager.Join(this.Player, player.SpaceShip.Coords);

		}

		void player_ShipLeaveSystem(Player player, SpaceShip ship) {
			Gm_ShipLeaveSystem(ship);
		}

		void player_ShipEnterSystem(Player player, SpaceShip ship) {
			Gm_ShipEnterSystem(ship);
		}

		void player_SendMessage(Player sender, PlayerMessage message) {
			this.Gm_SendMessage(player, message);
		}

		void SpaceShip_EnterSystem(SectorObject sender, SystemEventArgs e) {
			//this.game.StarSystemManager.Join(this.Player, e.Coords);
		}

		void SpaceShip_LeaveSystem(SectorObject sender, SystemEventArgs e) {
			//this.game.StarSystemManager.Leave(this.Player, e.Coords);
		}

		#region GMessages

		public void Gm_SendMessage(Player sender, PlayerMessage message) {
			this.StartSendData();
			try {

				string nickSeq = string.Format("{0}{1}{2}",
					ANSI.SetColorANSI16("<", ANSI.ANSIColor_16.Blue),
					ANSI.SetColorANSI16(message.From, ANSI.ANSIColor_16.White),
					ANSI.SetColorANSI16(">", ANSI.ANSIColor_16.Blue));

				string messageSeq = ANSI.SetColorANSI16(message.Message, ANSI.ANSIColor_16.Magenta);

				SendLine(string.Format("{0} {1}", nickSeq, messageSeq));
			} catch {

			} finally {
				EndSendData();
			}
		}

		public void Gm_ShipEnterSystem(SpaceShip ship) {
			this.StartSendData();
			try {
				var dist = ship.SystemLocation.GetInSystemDistance(
					this.player.SpaceShip.SystemLocation);

				var desc = SystemLocation.GetDistanceDesc(dist);
				string descSeq = ANSI.SetColorANSI16(desc, ANSI.ANSIColor_16.White);
				string codeSeq = ANSI.SetColorANSI16(ship.Code, ANSI.ANSIColor_16.DarkYellow);
				string nameSeq = string.Empty;

				if (ship.Player != null && ship.Player.Name != null && ship.Player.Name != string.Empty)
					nameSeq = string.Format("{0}{1}{2}",
						ANSI.SetColorANSI16("[", ANSI.ANSIColor_16.Blue),
						ANSI.SetColorANSI16(ship.Player.Name, ANSI.ANSIColor_16.White),
						ANSI.SetColorANSI16("]", ANSI.ANSIColor_16.Blue));

				ioShipComputer cmt = new ioShipComputer(this);

				cmt.SendMessage(string.Format("Hyperspace entry detected. Distance: {0}", descSeq));
				cmt.SendMessage(string.Format("Spaceship code: {0} {1}", codeSeq, nameSeq));

			} catch {
			} finally {
				this.EndSendData();
			}
		}

		public void Gm_ShipLeaveSystem(SpaceShip ship) {
			this.StartSendData();

			try {
				var dist = ship.SystemLocation.GetInSystemDistance(
					this.player.SpaceShip.SystemLocation);

				var desc = SystemLocation.GetDistanceDesc(dist);
				string descSeq = ANSI.SetColorANSI16(desc, ANSI.ANSIColor_16.White);
				string codeSeq = ANSI.SetColorANSI16(ship.Code, ANSI.ANSIColor_16.DarkYellow);
				string nameSeq = string.Empty;

				if (ship.Player != null && ship.Player.Name != null && ship.Player.Name != string.Empty)
					nameSeq = string.Format("{0}{1}{2}",
						ANSI.SetColorANSI16("[", ANSI.ANSIColor_16.Blue),
						ANSI.SetColorANSI16(ship.Player.Name, ANSI.ANSIColor_16.White),
						ANSI.SetColorANSI16("]", ANSI.ANSIColor_16.Blue));

				ioShipComputer cmt = new ioShipComputer(this);
				cmt.SendMessage(string.Format("Hyperspace exit detected. Distance: {0}", descSeq));

				Coords coords = ship.dCoords;
				StarSystem possibleSystem = null;
				if (coords != null) {
					Sector sector = coords.GetSector();
					//try get a possible system

					foreach (StarSystem sys in sector.Systems) {
						if (sys.Coords == coords)
							possibleSystem = sys;
					}
				}

				string strDest = (possibleSystem != null) ?
					string.Format("{0} {1}", possibleSystem.Name, possibleSystem.Coords) :
					(coords != null) ? coords.ToString() : "unknown";

				cmt.SendMessage(string.Format("Spaceship code {0} {2} - possible destinantion: {1}", 
					codeSeq, strDest, nameSeq
				));


			} catch {
			} finally {
				this.EndSendData();
			}
			
		}

		public void Gm_ChanneMessage(string channel, Player player, string message) {
			this.StartSendData();
			try {

				string nickSeq = string.Format("{0}{1}{2}",
					ANSI.SetColorANSI16("<", ANSI.ANSIColor_16.Blue),
					ANSI.SetColorANSI16(player.Name, ANSI.ANSIColor_16.White),
					ANSI.SetColorANSI16(">", ANSI.ANSIColor_16.Blue));

				string channelSeq = string.Format("{0}{1}{2}",
					ANSI.SetColorANSI16("[", ANSI.ANSIColor_16.Blue),
					ANSI.SetColorANSI16(channel, ANSI.ANSIColor_16.Gray),
					ANSI.SetColorANSI16("]", ANSI.ANSIColor_16.Blue));

				string messageSeq = ANSI.SetColorANSI16(message, ANSI.ANSIColor_16.Cyan);

				this.SendLine(string.Format("{0} {1} {2}", channelSeq, nickSeq, messageSeq));
			} catch {
			} finally {
				this.EndSendData();
			}
		}

        #endregion

        #region Commands

        static void __CmdHelp_Info(CommandInfo info) {
            info.Usage = "#Command# [command]";
            info.RequiredParameters = 2;
            StringBuilder help_sBuilder = new StringBuilder();
            help_sBuilder.AppendFormat("Displays help usage on a comamnd");
            info.Help = help_sBuilder.ToString();
        }

        public bool CmdHelp(string[] args) {
            if (args.Length > 1) {
                string commName = args[1];
                foreach (var cmd in commands) {
                    if (cmd.Key == commName) {
                        SendLine(cmd.Value.Help);
                        SendLine("usage: {0}", cmd.Value.Usage);
                        return true;
                    }
                }
                SendLine("Command {0} not found", commName);
            } else {
                SendLine("Type &B(help [command]) for command especific help:");
                SendLine("Available commands:");
                foreach (var cmd in commands) {
                    SendLine(string.Format("&W({0}) &w(- {1})", cmd.Key, cmd.Value.Help));
                }
            }
            return true;
		}
		
		public void CmdTestCity(string[] args) {
			ioCity city = new ioCity();
			SendLine(city.Test().ToString());
		}

        static void __CmdEncoding_Info(CommandInfo info) {
            info.Usage = "#Command# [encoding]";
            info.RequiredParameters = 2;
            StringBuilder help_sBuilder = new StringBuilder();
            help_sBuilder.AppendFormat("Changes or displays the encoding");
            info.Help = help_sBuilder.ToString();
        }

        public bool CmdEncoding(string[] args) {

			StringBuilder sBuilder = new StringBuilder();

			string encodingName = null;
			if (args.Length > 1) {
				encodingName = args[1].Trim().ToUpper();
				bool changed = true;
				switch (encodingName) {
					case "UNICODE":
					case "UTF":
					case "UTF8":
					case "UTF-8": conn.encoding = Encoding.UTF8; break;
					case "IMBCP437":
					case "CP437":
					case "OEM437": conn.encoding = Encoding.GetEncoding(437); break;
					case "ISO-8859-1":
					case "ISO8859-1":
					case "ISO-8859":
					case "ISO-88591":
					case "ISO88591":
					case "ISO8859": conn.encoding = Encoding.GetEncoding("ISO-8859-1"); break;
					case "ASCII": conn.encoding = Encoding.ASCII; break;
					default: changed = false; break;
				}

				if (changed)
					sBuilder.AppendFormat("Encoding changed.\r\n");
				else
					sBuilder.AppendFormat("Encoding not changed... you provided '{0}'\r\n", encodingName);

			}

			sBuilder.AppendFormat("Current encoding is '{0}' codepage {1}\r\n\r\n",
				conn.encoding.EncodingName,
				conn.encoding.CodePage);

			sBuilder.AppendFormat("To change the current encoding please use the command: encoding <name>\r\n");
			sBuilder.AppendFormat(" where name can be one of:\r\n");
			sBuilder.AppendFormat(" - UTF8 (**Recomended** if your terminal suports it)\n\r");
			sBuilder.AppendFormat(" - CP437 (Nice but you have to use a compatible font ex. Terminal\n\r");
			sBuilder.AppendFormat(" - ISO-8859-1 (*not recomended use it only if you must)\r\n");
			sBuilder.AppendFormat(" - ASCII (*not recomended use it only if you must)");

			SendLine(sBuilder.ToString());
            return true;

			//var ibm437 = Encoding.GetEncoding(437); //IBM/OEM code page 437
			//var ascii = Encoding.ASCII;
			//var utf8 = Encoding.UTF8;
		}

		static void __CmdRegister_Info(CommandInfo info) {
			info.Usage = "#Command# username password";
            info.RequiredParameters = 2;
			StringBuilder help_sBuilder = new StringBuilder();
			help_sBuilder.AppendFormat("Registers a new user in the system");
			info.Help = help_sBuilder.ToString();
		}

		public bool CmdRegister(string[] args) {
			if (args.Length >= 3) {
				string username = args[1];
				string password = args[2];
				this.player = Player.CreateNewPlayer(username, password, "");
				ProcessPlayerInit();
				SendFormat("Registered! Welcome {0}\r\n", player.Name);
				SetMode(ioPlayerMode.LoggedIn, true);
                return true;
			}
            return false;
		}

		static void __CmdConnect_Info(CommandInfo info) {
			info.Usage = "#Command# [username] [password]";
			StringBuilder help_sBuilder = new StringBuilder();
			help_sBuilder.AppendFormat("Logs a user into the game");
			info.Help = help_sBuilder.ToString();
		}

		public bool CmdConnect(string[] args) {

			bool loginOk = false;

			if (args.Length >= 3) {

				string username = args[1];
				string password = args[2];

				Player loggedPlayer = Player.Login(username, password);
				if (loggedPlayer != null) {
					this.player = loggedPlayer;
					ProcessPlayerInit();
					loginOk = true;
				}
			}

			if (loginOk) {
				SetMode(ioPlayerMode.LoggedIn, true);
				SendLine(string.Format("Welcome back {0}", player.Name));
                return true;
			} else {
				ClearModes();
				SendLine(string.Format("Connect failed wrong name / password"));
                return false;
			}
		}

        static void __Cmd_Look_Info(CommandInfo info) {
            info.Usage = "#Command#";
            info.RequiredParameters = 2;
            StringBuilder help_sBuilder = new StringBuilder();
            help_sBuilder.AppendFormat("Displays info about the current player location");
            info.Help = help_sBuilder.ToString();
        }

        public bool Cmd_Look(string[] args) {

			if (player != null && player.Location != null) {
				StarSystem system = this.player.Location.StarSystem;
				SystemBody planet = this.player.Location.SystemBody;
                SpacePort spacePort = this.player.Location.GetSpacePort(player);

                if (spacePort != null)
                    CmdSpacePortInfo(new string[0]);
                else if (planet != null)
                    CmdSystemBodyInfo(new string[0]);
                else if (system != null)
                    CmdSystemInfo(new string[0]);
                else
                    CmdSectorMap(new string[0]);
			}
            return true;
		}

		static void __CmdSectorMap_Info(CommandInfo info) {
			info.Usage = "#Command#";
			StringBuilder help_sBuilder = new StringBuilder();
			help_sBuilder.AppendFormat("Returns a sector map centered in the current sector. ");
			info.Help = help_sBuilder.ToString();
		}

		public bool CmdSectorMap(string[] args) {


			SendLine(">> SECTOR MAP {0}", this.player.Location);

			List<StarSystem> systems = player.GetSystemsInRange(80);
			List<object[]> mapInfo = new List<object[]>();

			foreach (StarSystem sSystem in systems) {
				var playerCoords = this.player.SpaceShip.Coords.Galactic;
				var systemCoords = sSystem.Coords.Galactic;
				var screenCoords = 
					new GalacticLocation(
						playerCoords.gX - systemCoords.gX,
						playerCoords.gY - systemCoords.gY,
						playerCoords.gZ - systemCoords.gZ
					);
				var distance = this.player.SpaceShip.Coords.CalculateDistance(
					sSystem.Coords);

				byte fgColor = 253;

				switch (sSystem.SpectralClass) {
					case StarSpectralClass.O: fgColor = 27; break;  //Blue
					case StarSpectralClass.B: fgColor = 39; break;	//Blue
					case StarSpectralClass.A: fgColor = 159; break;	//Bright Blue
					case StarSpectralClass.F: fgColor = 230; break;	//Bright Yellow
					case StarSpectralClass.G: fgColor = 226; break;	//Yellow
					case StarSpectralClass.K: fgColor = 202; break;	//Orange
					case StarSpectralClass.M: fgColor = 160; break;	//Red
				}

				object[] miobj = new object[] {
						(int)((screenCoords.gX * 2) + (this.conn.TerminalWidth / 2.0)), 
						(int)((screenCoords.gY * 2) + (this.conn.TerminalHeight / 2.0)),
						distance, sSystem.NumberOfBodies, sSystem.Name, fgColor
					};
				
				mapInfo.Add(miobj);

				//Console.WriteLine("{0} {1} {2}", miobj[0], miobj[1], miobj[4]);
			}

			char star1 = (char)0x2022;
			//char star2 = (char)0x25CB; //Normal faint
			//char star3 = (char)0x26CC; //Sun

			int w = this.conn.TerminalWidth;
			int h = this.conn.TerminalHeight;

			StringBuilder sBuilder = new StringBuilder();

			TextCanvas.NumOfColorsEnum numOfColors;

			if (conn.terminal256Colors)
				numOfColors = TextCanvas.NumOfColorsEnum.XTERM256;
			else if (conn.terminalIsANSI)
				numOfColors = TextCanvas.NumOfColorsEnum.ANSI;
			else
				numOfColors = TextCanvas.NumOfColorsEnum.None;

			numOfColors = TextCanvas.NumOfColorsEnum.XTERM256;

			TextCanvas canvas = new TextCanvas(
				this.conn.TerminalWidth - 2,
				this.conn.TerminalHeight - 4,
				numOfColors
			);

			canvas.DrawGrid(0, 0, canvas.Width, canvas.Height, (byte)234);

			//SendData(sBuilder.ToString()); //Make room for the map.

			foreach (object[] def in mapInfo) {

				if ((int)def[0] < 0 || (int)def[0] > this.conn.TerminalWidth -1) continue;
				if ((int)def[1] < 0 || (int)def[1] > this.conn.TerminalHeight -1) continue;

				byte fgColor = (byte)def[5];
				canvas.DrawString(((int)def[0]), ((int)def[1]), star1.ToString(), fgColor, 0);
				//canvas.DrawString(((int)def[0]) + 1, ((int)def[1]) + 1, def[3].ToString());

				//SendData(ANSI.AtPosition(star1.ToString(), ((int)def[0]), ((int)def[1]), true));
				//SendData(ANSI.AtPosition(def[3].ToString(), ((int)def[0]) + 1, ((int)def[1]) + 1, true)); 

			}

			foreach (object[] def in mapInfo) {
				if ((int)def[0] < 0 || (int)def[0] > this.conn.TerminalWidth -1) continue;
				if ((int)def[1] < 0 || (int)def[1] > this.conn.TerminalHeight -1) continue;

				string starName;

				//if ((int)def[1] + 1 + def[4].ToString().Length > this.conn.TerminalWidth)
				//    starName = def[4].ToString().Substring(0, 
				//        (int)def[1] + 1 + def[4].ToString().Length - this.conn.TerminalWidth);
				//else
					starName = def[4].ToString();

				byte fgColor = (byte)def[5];
				canvas.DrawString(((int)def[0]) + 1, ((int)def[1]), starName, fgColor, 0);
				//SendData(ANSI.AtPosition(starName, , true));
			}

			canvas.DrawString(0, 0,
				string.Format("Map centered on Sector {0}", player.SpaceShip.Sector.ToString())
				.PadRight(conn.TerminalWidth - 4)
			);

			//SendData(ANSI.AtPosition(
			//    string.Format("Map centered on Sector {0}", player.SpaceShip.Sector.ToString())
			//        .PadRight(conn.TerminalWidth - 4), 0, 0, true)
			//);

			canvas.DrawRectangle(0, 0, canvas.Width, canvas.Height, (byte)ANSI.ANSIColor_8.Cyan);

			SendLine(canvas.ToString());
            return true;
		}

		public bool CmdRegion(string[] args) {
			
			Sector baseSector = this.player.SpaceShip.Sector;
			TextCanvas canvas = new TextCanvas(
				this.conn.TerminalWidth, 
				this.conn.TerminalHeight, 
				TextCanvas.NumOfColorsEnum.XTERM256
			);

			SendLine(">> REGION MAP {0}", baseSector);

			Sector firstSector = null, lastSector = null;
			double maxDistance = 0.0;

			for (int y = 0; y < (this.conn.TerminalHeight); y++) {
				for (int x = 0; x < this.conn.TerminalWidth; x++) {
					Sector cSector = new Sector(
						(uint)(baseSector.IndexX - (this.conn.TerminalWidth / 2) + x),
						(uint)(baseSector.IndexY - (this.conn.TerminalHeight / 2) + y)
					);

					if (firstSector == null) firstSector = cSector;
					lastSector = cSector;

					double dist = cSector.CalculateDistance(baseSector);
					if (dist > maxDistance) maxDistance = dist;

					int numOfSystems = cSector.NumSystems;

					char sectorChar = ' '; // ·•●

					if (numOfSystems == 0) sectorChar = ' ';
					else if (numOfSystems < 3) sectorChar = '·';
					else if (numOfSystems < 5) sectorChar = '•';
					else sectorChar = '●';

					byte color = 7;
					if (cSector == baseSector)
						color = (byte)ANSI.ANSIColor_8.Green;

					canvas.DrawString(x, y, sectorChar.ToString(), color, 0);
				}
			}

			SendLine(canvas.ToString());
			StringBuilder sBuilder = new StringBuilder();
			sBuilder.AppendFormat(
				"Local region map ({0:0.00} Ly). Maximum distance from current location {1:0.00} Ly\r\nFrom sector {2} to sector {3}",
				maxDistance * 2, maxDistance, firstSector, lastSector);

			SendLine(sBuilder.ToString());
            return true;

        }

		static void __CmdGalaxyMap_Info(CommandInfo info) {
			info.Usage = "#Command#";
			StringBuilder help_sBuilder = new StringBuilder();
			help_sBuilder.AppendFormat("the #Command# returns a representation of the galaxy\r\n");
			info.Help = help_sBuilder.ToString();
		}

		public bool CmdGalaxyMap(string[] args) {
			SendLine(">> GALAXY MAP {0}", this.player.Location);
			SendLine(Message.ReadDefinedMessage("galaxymap"));
			SendLine(string.Format("galaxy map (+/- {0:0.00} Ly).", GalaxyConstants.GalaxyWidth));
            return true;
        }

		public bool CmdSearch(string[] args) {

			string systemName = null;

			if (args != null && args.Length > 1) {
				systemName = "";
				for (int i = 1; i < args.Length; i++)
					systemName += string.Format("{0} ", args[i]);
				systemName = systemName.Trim();
			}

			if (systemName == null || systemName.Trim() == string.Empty) {
				SendLine("Please provide some information to search for");
				return true;
			}

			SendLine(string.Format("Doing an extended (1500Ly) search for '{0}*' - please wait", systemName));

			StringBuilder sBuilder = new StringBuilder();
			var systems = player.FindStarSystem(systemName, 1500, true);

				
			sBuilder.AppendFormat("{0} Systems found by searching '{1}*' in a 1500 ly range\r\n", systems.Count, systemName);
			foreach (var sys in systems) {
				System.Threading.Thread.Sleep(0);
				sBuilder.AppendFormat("{0,-30} {1,6:0.00} Ly\r\n", sys.Name, player.GetDistance(sys.Coords));
			}

			SendLine(sBuilder.ToString());
            return true;
        }

		public bool CmdGetCoords(string[] args) {

			SendLine(">> COORDINATES");

			Coords coords = null;

			if (player != null && player.Location != null) {

				if (player.Location.SystemBody != null)
					coords = player.Location.SystemBody.Coords;
				else if (player.Location.StarSystem != null)
					coords = player.Location.StarSystem.Coords;
				else if (player.Location.Sector != null)
					coords = player.Location.Sector.Coords;

			}

			ioShipComputer cmt = new ioShipComputer(this);
			

			if (coords != null) {
				cmt.SendMessage(string.Format("Current coordinates: {0}", coords.ToString()));
			} else
				cmt.SendMessage("No information available");

            return true;
		}

        public bool CmdSpacePortInfo(string[] args) {
            SendLine(">> SPACE PORT INFO");

            SpacePort port = this.player.Location.GetSpacePort(player);
            if (port != null) {
                ioSpacePort spacePortInfo = new ioSpacePort(port, conn.TerminalWidth);
                SendLine(spacePortInfo.GetInfo());
            }

            return true;
        }

        public bool CmdSystemBodyInfo(string[] args) {

			SendLine(">> PLANET INFO");

			string sysCode = "";
			bool useCurrentLocation = false;

			if (args.Length > 1) {
				for (int i = 1; i < args.Length; i++)
					sysCode += string.Format("{0} ", args[i]);
			} else
				useCurrentLocation = true;

			sysCode = sysCode.Trim();

			StarSystem system = this.player.Location.StarSystem;
			SystemBody planet = this.player.Location.SystemBody;
			if (system != null) {

				int sysNum = 0; bool useSysNum = false;
				if (sysCode.ToUpper().StartsWith("P") && sysCode.Length > 1) {
					useSysNum = int.TryParse(sysCode.Substring(1), out sysNum);
				}

				if (!useCurrentLocation) {
					foreach (var body in system.SystemBodies) {
						if (
							(useSysNum && body.SystemBodyId == sysNum) ||
							body.Name.Equals(sysCode, StringComparison.InvariantCultureIgnoreCase)

						) {
							planet = body;
							break;
						}
					}
				}

				if (planet != null) {
					ioPlanetInfo planetInfo = new ioPlanetInfo(planet, conn.TerminalWidth);
					SendLine(planetInfo.GetInfo());
				} else {
					SendLine("Target not provided no information available!");
				}

			}
            return true;
		}

		public bool CmdSystemInfo(string[] args) {

			SendLine(">> STAR SYSTEM INFO");

			StringBuilder sBuilder = new StringBuilder();
			string systemName = null;
			StarSystem system = null;

			if (args != null && args.Length > 1) {
				systemName = "";
				for (int i = 1; i < args.Length; i++)
					systemName += string.Format("{0} ", args[i]);
				systemName = systemName.Trim();
			} else if (player != null && player.Location != null && player.Location.StarSystem != null)
				system = player.Location.StarSystem;


			if (systemName != null) {
				var systems = player.FindStarSystem(systemName, true);

				if (systems.Count != 1) {
					sBuilder.AppendFormat("{0} Systems found by searching '{1}*' in a 100 ly range\r\n", systems.Count, systemName);
					foreach (var sys in systems) {
						//Console.WriteLine(string.Empty.PadLeft(40, '-'));
						sBuilder.AppendFormat("{0,-30} {1,6:0.00} Ly\r\n", sys.Name, player.GetDistance(sys.Coords));
					}
				} else
					system = systems[0];
			}

			if (system != null) {

				ioStarSystemInfo sInfo = new ioStarSystemInfo(system, this.conn.TerminalWidth);
				sBuilder.Append(sInfo.Canvas.ToString());

				sBuilder.AppendFormat(ANSI.SetColorANSI16("Planets:\r\n", ANSI.ANSIColor_16.BrightCyan));

				
				
				foreach (var systemBody in system.SystemBodies) {

					string planetSymbol = ANSI.SetColorANSI16("•", ANSI.ANSIColor_16.Gray);
					switch (systemBody.BodyType) {
						case SystemBodyType.Asteroid: planetSymbol = ANSI.SetColorANSI16("•", ANSI.ANSIColor_16.Darkgray); break;
						case SystemBodyType.GasGiant: planetSymbol = ANSI.SetColorANSI16("Ɵ", ANSI.ANSIColor_16.Magenta); break;
						case SystemBodyType.IceWorld: planetSymbol = ANSI.SetColorANSI16("Ō", ANSI.ANSIColor_16.White); break;
						case SystemBodyType.Inferno: planetSymbol = ANSI.SetColorANSI16("ǒ", ANSI.ANSIColor_16.Red); break;
						case SystemBodyType.RingedGasGiant: planetSymbol = ANSI.SetColorANSI16("Ø", ANSI.ANSIColor_16.BrightYellow); break;
						case SystemBodyType.RockyPlanetoid: planetSymbol = ANSI.SetColorANSI16("•", ANSI.ANSIColor_16.Gray); break;
						case SystemBodyType.RockyWorld: planetSymbol = ANSI.SetColorANSI16("●", ANSI.ANSIColor_16.BrightRed); break;
						case SystemBodyType.SubGasGiant: planetSymbol = ANSI.SetColorANSI16("○", ANSI.ANSIColor_16.BrightMagenta); break;
						case SystemBodyType.Terrestrial: planetSymbol = ANSI.SetColorANSI16("Ȭ", ANSI.ANSIColor_16.BrightBlue); break;
						case SystemBodyType.Venuzian: planetSymbol = ANSI.SetColorANSI16("Ŏ", ANSI.ANSIColor_16.DarkYellow); break;
						case SystemBodyType.WaterWorld: planetSymbol = ANSI.SetColorANSI16("Ő", ANSI.ANSIColor_16.Blue); break;
					}

					sBuilder.AppendFormat("{6}[{7}P{5,-2}{6}] {9} {7}{0,-19} {6}C:{7} {1,-4} {6}P:{7} {2,10:0.0Yr} {6}R:{7} ~{3,8:0.0Au} {6}T:{7} {4,5:0.0°}{8}\r\n", 
						systemBody.Name, 
						systemBody.GetClass(), 
						systemBody.OrbitalPeriod,
						systemBody.OrbitalRadius,
						systemBody.Temperature,
						systemBody.SystemBodyId,
						ANSI.ColorANSI16(ANSI.ANSIColor_16.Cyan),
						ANSI.ColorANSI16(ANSI.ANSIColor_16.BrightCyan),
						ANSI.ClearFormat(), planetSymbol
					);
				}

				sBuilder.Append(ANSI.ColorANSI16(ANSI.ANSIColor_16.Gray));
				sBuilder.AppendFormat("\r\nLegend:\r\n");
				sBuilder.Append(ANSI.ColorANSI16(ANSI.ANSIColor_16.Darkgray));
				sBuilder.AppendFormat("C - Class; P - Orbital Period; R: Orbital Radius; T: Surface Temperature\r\n");
				sBuilder.Append(ANSI.ColorANSI16(ANSI.ANSIColor_16.Gray));
				sBuilder.AppendFormat("Classes:\r\n");
				sBuilder.Append(ANSI.ColorANSI16(ANSI.ANSIColor_16.Darkgray));
				sBuilder.AppendFormat("As - Asteroid;  R   - Rocky World; R2  - Rocky Planetoid; I - Inferno;\r\n");
				sBuilder.AppendFormat("V  - Venuzian;  T   - Terrestrial; W   - Water World;     W2 - Ice World;\r\n");
				sBuilder.AppendFormat("Gs - Gas Giant; Gs2 - Gas Giant;   Gs3 - Sub Gas Giant;\r\n");
				sBuilder.Append(ANSI.ClearFormat());
			}

			SendLine(sBuilder.ToString());
            return true;
		}

		#region Dock & Launch
		public bool CmdLaunch(string[] args) {

			ioShipComputer comp = new ioShipComputer(this);

			if (!this.Player.SpaceShip.Docked) {
				comp.SendMessage("Launch procedure aborted... Spaceship is not docked!");
				return true;
			}

			string dockCode = this.Player.SpaceShip.SpaceDockId;
			StarSystem system = this.player.Location.StarSystem;
			SystemBody body = this.player.Location.SystemBody;
			SpacePort port = null;

			foreach (var dock in body.SpacePort) {
				if (dock.Id == dockCode) {
					port = dock;
					break;
				}
			}

			Random rand = new Random();
			string orbitPos = string.Format("Obr{0}", rand.Next(100, 999));

			comp.SendSucessMessage("Requestion launch permission", "OK");
			comp.SendMessage(
				string.Format("{0}: >> launch permission granted. You are clear to procede planet orbit at {1}",
				(port != null) ? port.Name : "Dock control",
				orbitPos));
			comp.SendMessage("please use a slow X12-Z departure procedure until VRZ-1");
			comp.SendSucessMessage(string.Format("Calculating parameters for {0}", orbitPos), "OK");
			comp.SendSucessMessage("Sealling airlocks", "OK");
			comp.SendCountDownMessage("Launching in ", "OK", 3, 1000, true);
			comp.SendCountDownMessage("Rotation ", "OK", 3, 500, false);
			comp.SendCountDownMessage("Accelerating ", "VRZ-1", 10, 200, false);
			comp.SendSucessMessage("Reactivating manual system control", "OK");
			comp.SendMessage(string.Format("in {0} orbit", (body != null) ? body.Name : orbitPos));

			this.player.SpaceShip.SpaceDockId = null;
            return true;
        }

		public bool CmdDock(string[] args) {

			ioShipComputer comp = new ioShipComputer(this);

			if (this.Player.SpaceShip.Docked) {
				comp.SendMessage("Dock procedure aborted... Spaceship is already docked!");
				return true;
			}

			string dockCode = "";
			if (args.Length > 1) {
				for (int i = 1; i < args.Length; i++)
					dockCode += string.Format("{0} ", args[i]);
			} else {
				comp.SendMessage("'Dock' procedure aborted... Target not provided!");
				return true;
			}

			dockCode = dockCode.Trim();

			StarSystem system = this.player.Location.StarSystem;
			bool dockLocationFound = (system != null);
			
			SystemBody body = this.player.Location.SystemBody;
			dockLocationFound &= (body != null);

			SpacePort targetPort = null;

			if (dockLocationFound) {
				dockLocationFound = false;
				foreach (var port in body.SpacePort) {
					if (port.Id.ToLower() == dockCode.ToLower()) {
						dockLocationFound = true;
						targetPort = port;
						break;
					} else if (port.Name.ToLower().StartsWith(dockCode.ToLower())) {
						dockLocationFound = true;
						targetPort = port;
						break;
					}
				}
			}

			if (!dockLocationFound) {
				comp.SendMessage("'Dock' procedure aborted... Target not found!");
				return true;
			} else {
				//Start Dock Procedure

				Random rand = new Random();
				
				comp.SendSucessMessage("Calculating aproch vector", "OK");
				comp.SendSucessMessage("Requesting dock permission", "OK");
				comp.SendMessage(
					string.Format(
						"{0}: >> Dock permission granted please procede to D{1} and dock as soon as you get green position light",
						targetPort.Name, rand.Next(1, 2)));

				comp.SendSucessMessage("Starting dock procedure", "OK");
				comp.SendCountDownMessage("Ajusting speed ", "VRZ-1", 5, 200, false);
				comp.SendSucessMessage("Got green position light! dock identified", "OK");
				comp.SendCountDownMessage("Docking ", "Docked", 10, 200, false);
				comp.SendSucessMessage("Establish computer link with station compt.", "OK");
				comp.SendMessage(
					string.Format("{0} >> Welcome to {0} station please enjoy you stay.",
					targetPort.Name, body.Name));
				comp.SendMessage(
					string.Format("{0}: To connect to the station bbs please use the command BBS",
					targetPort.Name));


				player.SpaceShip.SpaceDockId = targetPort.Id;

			}
            return true;
        }
		#endregion

		#region jump & move
		public bool CmdMoveToTarget(string[] args) {

			ioShipComputer computer = new ioShipComputer(this);

			if (this.Player.SpaceShip.Docked) {
				computer.SendMessage("'Nav' procedure aborted... Spaceship is docked!");
				return true;
			}

			string jumpCode = "";

			if (args.Length > 1) {
				for (int i = 1; i < args.Length; i++)
					jumpCode += string.Format("{0} ", args[i]);
			} else {

				if (player.SpaceShip.GetInSystemETA().Seconds > 0) {
					TimeSpan eta = player.SpaceShip.GetInSystemETA().Add(TimeSpan.FromSeconds(2));
					computer.SendMessage(
						string.Format("Currently in space! ETA {0}", eta));
                    return true;
				} else {
					computer.SendMessage("'Nav' procedure aborted... Target not provided!");
				}
                return true;
            }

			jumpCode = jumpCode.Trim();

			StarSystem system = this.player.Location.StarSystem;
			if (system != null) {

				int sysNum = 0; bool useSysNum = false;
				if (jumpCode.ToUpper().StartsWith("P") && jumpCode.Length > 1) {
					useSysNum = int.TryParse(jumpCode.Substring(1), out sysNum);
				}

				foreach (var body in system.SystemBodies) {
					if (
						(useSysNum && body.SystemBodyId == sysNum) ||
						body.Name.Equals(jumpCode, StringComparison.InvariantCultureIgnoreCase)

					) {
						SystemLocation dest = SystemLocation.GetUpdatedLocation(body);

						this.inSpace = true;
						this.player.SpaceShip.FlyTo(dest);
						this.player.SpaceShip.Target = body;
						var dist = this.player.SpaceShip.SystemLocation.GetInSystemDistance(dest);

						TimeSpan eta = player.SpaceShip.GetInSystemETA().Add(TimeSpan.FromSeconds(2));

						computer.SendMessage("Move command received");
						computer.SendMessage(
							string.Format("Target [{0}]", body.Name));

						computer.SendSucessMessage(string.Format("Calculating parameters... Distance {0}", SystemLocation.GetDistanceDesc(dist)), "OK");
						computer.SendSucessMessage("Calculating target vector", "OK");
						computer.SendCountDownMessage("Ajusting Position ", "OK", 4, 200, false);
						computer.SendCountDownMessage("Accelerating ", "OK", 5, 200, false);
						computer.SendSucessMessage("Reactivating manual control", "OK");
						computer.SendMessage(string.Format("[IN SPACE] ETA - {0}", eta.ToString()));

						tickTimer.Change(2000, 2000);

						break;
					}
				}


			}
            return true;

        }

		public bool CmdTargetReached() {

			StartSendData();

			this.inSpace = false;
			var systemBody = this.player.Location.SystemBody;

			ioShipComputer computer = new ioShipComputer(this);

			if (systemBody != null) {
				computer.SendSucessMessage("Starting orbital procedures", "OK");
				computer.SendSucessMessage("Calculating orbital vectors", "OK");
				computer.SendCountDownMessage("Ajusting speed and aproch angle ", "in Orbit", 10, 200, false);
				computer.SendSucessMessage("Reactivating manual system control", "OK");
				computer.SendMessage(string.Format("In {0} Orbit", systemBody.Name));
			} else {
				computer.SendCountDownMessage("Aproching Target ", "OK", 10, 50, false);
				computer.SendSucessMessage("Reactivating manual system control", "OK");
				computer.SendMessage("Target reached");
			}

			EndSendData();
            return true;
        }

		public bool CmdJump(string[] args) {

			ioShipComputer computer = new ioShipComputer(this);

			if (this.Player.SpaceShip.Docked) {
				computer.SendMessage("Jump procedure aborted... Spaceship is docked!");
                return true;
            }

			if (player.SpaceShip.GetETA().Seconds > 0) {
				TimeSpan eta = player.SpaceShip.GetETA().Add(TimeSpan.FromSeconds(2));
				computer.SendMessage(
					string.Format("Currently in Hiperspace! ETA {0}", eta));
                return true;
            }

			string jumpCode = "";

			if (args.Length > 1) {
				for (int i = 1; i < args.Length; i++)
					jumpCode += string.Format("{0} ", args[i]);
			} else {
				computer.SendMessage("Jump procedure aborted... Target not provided!");
                return true;
            }

			jumpCode = jumpCode.Trim();

			StringBuilder sBuilder = new StringBuilder();
			List<StarSystem> systems = player.FindStarSystem(jumpCode, true);
			//wsReturn returnObject = new wsReturn();
			Coords destCoords = new Coords(0, 0, 0, 0, 0);
			StarSystem destSystem = null;

			bool jumpOk = true;

			if (!Coords.TryParse(jumpCode, out destCoords, true)) {

				if (systems.Count != 1) {
					sBuilder.AppendFormat("{0} Systems found by searching '{1}*' in a 100 ly range\r\n", systems.Count, jumpCode);
					foreach (var sys in systems) {
						sBuilder.AppendFormat("{0,-30} {1,6:0.00} Ly\r\n", sys.Name, player.GetDistance(sys.Coords));
					}
					jumpOk = false;
				} else {
					destCoords = systems[0].Coords;
					destSystem = systems[0];
				}
			}

			if (jumpOk) {

				this.inHyperspace = true;
				this.inSpace = false;

				player.SpaceShip.JumpTo(destCoords);

				var dist = player.SpaceShip.Coords.CalculateDistance(destCoords);

				TimeSpan eta = player.SpaceShip.GetETA().Add(TimeSpan.FromSeconds(2));
				//returnObject.ETA = eta;

				computer.SendMessage("Hyperspace jump command received");
				computer.SendMessage(
					string.Format("Target [{0}] Coordinates {1}",
						(destSystem != null) ? destSystem.Name : "Space", destCoords));

				computer.SendSucessMessage(string.Format("Calculating jump parameters... Distance {0:0.000} Ly", dist), "OK");
				computer.SendSucessMessage("Deactivating conventional drive", "OK");
				computer.SendCountDownMessage("Charging hyperdrive ", "OK", 9, 200, false);
				computer.SendCountDownMessage("Entering Hyperspace in ", "OK", 5, 1000, true);
				computer.SendSucessMessage("Reactivating manual control", "OK");
				computer.SendMessage(string.Format("[IN HYPERSPACE] ETA - {0}", eta.ToString()));

				tickTimer.Change(2000, 2000);

			} else 
				SendLine(sBuilder.ToString());

            return true;

        }

		public bool CmdOutOfJump() {

			StartSendData();

			this.inHyperspace = false;

			ioShipComputer computer = new ioShipComputer(this);
			computer.SendCountDownMessage("Leaving Hypersapce in ", "OK", 5, 1000, true);
			computer.SendSucessMessage("Ship auto test ....", "OK");
			computer.SendSucessMessage("Reactivating conventional drive", "OK");
			computer.SendCountDownMessage("Scanning region ", "OK", 3, 500, false);

			StarSystem system = player.Location.StarSystem;
			string strSystem = (system != null) ? system.Name : "None (In Space)";

			computer.SendMessage(
				string.Format("Current System {0} Coordinates {1}", strSystem, player.SpaceShip.Coords));

			if (system != null) {
				double dist = double.MaxValue;
				string planet = null;
				foreach (var body in system.SystemBodies) {
					var d = player.SpaceShip.SystemLocation.GetInSystemDistance(
						body.SystemLocation);
					if (d < dist) { dist = d; planet = body.Name; }
				}

				var starDistance = player.SpaceShip.SystemLocation.GetInSystemDistance(
					new SystemLocation(0, 0, 0));

				computer.SendMessage(
					string.Format("Distance to the the main star {0} ",
						SystemLocation.GetDistanceDesc(starDistance)
					));

				//TODO: scan for a real starport
				if (planet != null) {
					computer.SendMessage(
						string.Format("Distance to the nearest starport {0} at '{1}'",
							SystemLocation.GetDistanceDesc(dist), planet
						));
				}
			}

			EndSendData();
            return true;
        }
		#endregion

		#region market
		public bool CmdMarket(string[] args) {

			if (!this.Player.SpaceShip.Docked) {
				SendLine("Unable to connect to local stockmarket");
				return true;
			}

			string dockCode = this.Player.SpaceShip.SpaceDockId;
			StarSystem system = this.player.Location.StarSystem;
			SystemBody body = this.player.Location.SystemBody;

			Market market = game.MarketManager.GetMarket(body, dockCode);

			StringBuilder sBuilder = new StringBuilder();

			sBuilder.AppendFormat("Stock Market\r\n");
			sBuilder.AppendFormat("┌{0}┬{1}┬{2}┬{2}┐\r\n", 
				string.Empty.PadLeft(25, '─'),
				string.Empty.PadLeft(13, '─'),
				string.Empty.PadLeft(12, '─')
			);
			sBuilder.AppendFormat("│{0,-25}│{1,13}│{2,12}│{3,12}│\r\n", "Trade Good", "Price*", "Stock", "Cargo Bay");
			sBuilder.AppendFormat("├{0}┼{1}┼{2}┼{2}┤\r\n",
				string.Empty.PadLeft(25, '─'),
				string.Empty.PadLeft(13, '─'),
				string.Empty.PadLeft(12, '─')
			);
			foreach (var e in market.GetEntries()) {
				sBuilder.AppendFormat("│{0,-25}│{1,10} Cr│{2,10} T│{3,10} T│\r\n",
					e.TradeGood.Name, e.Price / 100.00, e.Quantity,
					this.Player.SpaceShip.CargoBay.GetQuantity(e.TradeGood.TradeGoodId)
				);
			}

			sBuilder.AppendFormat("└{0}┴{1}┴{2}┴{2}┘\r\n",
				string.Empty.PadLeft(25, '─'),
				string.Empty.PadLeft(13, '─'),
				string.Empty.PadLeft(12, '─')
			);

			sBuilder.AppendFormat("* prices shown are floting prices. Real trade prices depend on quantity, to get a quote please issue a 'QUOTE' command");

			SendLine(sBuilder.ToString());
            return true;

        }

		public bool CmdMarketQuote(string[] args) {

			if (!this.Player.SpaceShip.Docked) {
				SendLine("Unable to connect to local stockmarket");
                return true;
			}

			string dockCode = this.Player.SpaceShip.SpaceDockId;
			StarSystem system = this.player.Location.StarSystem;
			SystemBody body = this.player.Location.SystemBody;

			Market market = game.MarketManager.GetMarket(body, dockCode);

			StringBuilder sBuilder = new StringBuilder();

			if (market != null && args.Length >= 3) {

				string[] gTokens = new string[args.Length - 2];

				Array.Copy(args, 1, gTokens, 0, gTokens.Length);

				string sG = string.Join(" ", gTokens);
				string sQ = args[args.Length - 1];
				int q = 0;

				if (int.TryParse(sQ, out q)) {

					foreach (var e in market.GetEntries()) {
						if (e.TradeGood.Name.ToLower().StartsWith(sG.ToLower())) {

							var sellQ = e.GetSellQuote(q);
							var buyQ = e.GetBuyQuote(q);

							sBuilder.AppendFormat(
								"Quote result for {0}T of {1} - buy: ({2}T) {3} Cr sell: ({4}T) {5} Cr\r\n",
								q, e.TradeGood.Name, buyQ.Quantity, buyQ.Money / 100.00, sellQ.Quantity, sellQ.Money / 100.00
							);
						}
					}
				}
				
			}

			SendLine(sBuilder.ToString());
            return true;

        }

		public bool CmdMarketSell(string[] args) {

			if (!this.Player.SpaceShip.Docked) {
				SendLine("Unable to connect to local stockmarket");
                return true;
            }

			string dockCode = this.Player.SpaceShip.SpaceDockId;
			StarSystem system = this.player.Location.StarSystem;
			SystemBody body = this.player.Location.SystemBody;

			Market market = game.MarketManager.GetMarket(body, dockCode);

			StringBuilder sBuilder = new StringBuilder();

			if (market != null && args.Length >= 3) {

				string[] gTokens = new string[args.Length - 2];

				Array.Copy(args, 1, gTokens, 0, gTokens.Length);

				string sG = string.Join(" ", gTokens);
				string sQ = args[args.Length - 1];
				int q = 0;

				if (int.TryParse(sQ, out q)) {

					List<TradeGood> goodsFound = new List<TradeGood>();

					foreach (var e in market.GetEntries()) {
						if (e.TradeGood.Name.ToLower().StartsWith(sG.ToLower())) {
							goodsFound.Add(e.TradeGood);
						}
					}

					if (goodsFound.Count == 0) {
						sBuilder.AppendFormat("No goods found by searching '{0}*'", sG);
					} if (goodsFound.Count > 1) {
						sBuilder.AppendFormat("{0} types of goods found by searching '{0}*'\r\n", goodsFound.Count, sG);
						foreach (var g in goodsFound)
							sBuilder.AppendFormat("[{0}] ", g.Name);

					} else {
						var res = Player.Sell(goodsFound[0].TradeGoodId, q, market);
						sBuilder.AppendFormat("{0}T of {1} sold for {2}Cr",
							res.Quantity, goodsFound[0].Name, res.Money / 100.00);
					}

				}

			}

			SendLine(sBuilder.ToString());
            return true;
        }

		public bool CmdMarketBuy(string[] args) {
			if (!this.Player.SpaceShip.Docked) {
				SendLine("Unable to connect to local stockmarket");
                return true;
            }

			string dockCode = this.Player.SpaceShip.SpaceDockId;
			StarSystem system = this.player.Location.StarSystem;
			SystemBody body = this.player.Location.SystemBody;

			Market market = game.MarketManager.GetMarket(body, dockCode);

			StringBuilder sBuilder = new StringBuilder();

			if (market != null && args.Length >= 3) {

				string[] gTokens = new string[args.Length - 2];

				Array.Copy(args, 1, gTokens, 0, gTokens.Length);

				string sG = string.Join(" ", gTokens);
				string sQ = args[args.Length - 1];
				int q = 0;

				if (int.TryParse(sQ, out q)) {

					List<TradeGood> goodsFound = new List<TradeGood>();

					foreach (var e in market.GetEntries()) {
						if (e.TradeGood.Name.ToLower().StartsWith(sG.ToLower())) {
							goodsFound.Add(e.TradeGood);
						}
					}

					if (goodsFound.Count == 0) {
						sBuilder.AppendFormat("No goods found by searching '{0}*'", sG);
					} if (goodsFound.Count > 1) {
						sBuilder.AppendFormat("{0} types of goods found by searching '{0}*'\r\n", goodsFound.Count, sG);
						foreach (var g in goodsFound)
							sBuilder.AppendFormat("[{0}] ", g.Name);

					} else {
						var res = Player.Buy(goodsFound[0].TradeGoodId, q, market);
						sBuilder.AppendFormat("{0}T of {1} bought for {2}Cr",
							res.Quantity, goodsFound[0].Name, res.Money / 100.00);
					}

				}

			}

			SendLine(sBuilder.ToString());
            return true;
        }
		#endregion

		public bool CmdAbortJump(string[] args) {

			StringBuilder sBuilder = new StringBuilder();
			player.SpaceShip.JumpTo(player.SpaceShip.Coords);

			TimeSpan eta = player.SpaceShip.GetETA().Add(TimeSpan.FromSeconds(2));

			System.Threading.Thread.Sleep(3);
			SendLine("Aborting jump!!");
            return true;

        }

		public bool CmdBattleCommand(string[] args) {
            return true;
        }

		#region space battle commands

		//public void CmdFireLasers(string[] args) {

		//    string filter = string.Empty;
		//    bool argumentIsTargetNum = false;
		//    int filterTargetNum = 0;
		//    int tNum = 0;

		//    if (args.Length > 1) {
		//        filter = args[1];
		//        if (filter.Length > 0 && filter.ToLower()[0] == 't') {
		//            argumentIsTargetNum = int.TryParse(filter.Substring(1), out filterTargetNum);
		//        }
		//    }

		//    var battleTargetList = this.game.BattleManager.GetCurrentBattleTargets(this.Player);
		//    foreach (var t in battleTargetList) {
		//        tNum++;
		//        if (argumentIsTargetNum && tNum == filterTargetNum)
		//            this.game.BattleManager.DoBattleCommand(this.Player, t, "F");
		//        else if (t.Code.ToLower().StartsWith(filter.ToLower()))
		//            this.game.BattleManager.DoBattleCommand(this.Player, t, "F");
		//    }
		//}

		public void CmdBattle(string[] args) {
			var battleTargetList = this.game.BattleManager.GetCurrentBattleTargets(this.Player);
			int tNum = 0;
			StringBuilder sBuilder = new StringBuilder();
			sBuilder.AppendFormat("There are currenty {0} battle targets:\r\n", battleTargetList.Count);
			foreach (var t in battleTargetList) {
				tNum++;
				var dist = t.SystemLocation.GetInSystemDistance(this.player.SpaceShip.SystemLocation);
				sBuilder.AppendFormat("[T{0}] {1} Dist: {2}\r\n", tNum, t.Code, SystemLocation.GetDistanceDesc(dist));
			}
			SendLine(sBuilder.ToString());
		}

		public void CmdEngageShip(string[] args) {

			if (args.Length > 1) {
				string shipCode = args[1];
				SendLine(string.Format("Searching system for code '{0}*'\r\n", shipCode));
				SpaceShip ship = null;
				var area = this.game.StarSystemManager.GetStarSystemArea(this.player.SpaceShip.Coords);
				if (area != null) {
					var list = area.GetShipsInSystem();
					foreach (var s in list) {
						if (s.Code.ToLower().StartsWith(shipCode.ToLower())) {
							ship = s;
							SendLine(string.Format("Target found {0}", ship.Code));
							break;
						}
					}
				}

				if (ship != null) {
					SendLine("Engaging ship!");
					SendLine("Distance {0} ETA {1}",
						SystemLocation.GetDistanceDesc(player.SpaceShip.SystemLocation.GetInSystemDistance(ship.SystemLocation)),
						player.SpaceShip.GetInSystemETA());
					this.player.SpaceShip.EngageShip(ship);
				}

				tickTimer.Change(2000, 2000);

			}
		}

		#endregion

		public bool CmdListSystemShips(string[] args) {
			
			var area = this.game.StarSystemManager.GetStarSystemArea(this.player.SpaceShip.Coords);
			if (area != null) {
				var list = area.GetShipsInSystem();

				SendLine(string.Format("found '{0}' spaceships in system space", list.Count));

				foreach (var s in list) {
					var dist = s.SystemLocation.GetInSystemDistance(
						this.player.SpaceShip.SystemLocation);
					SendLine(string.Format("Code {0} - Distance: {1} - Model {2}", 
						s.Code, 
						SystemLocation.GetDistanceDesc(dist),
						s.Name
					)); 
				}
			}
            return true;
        }

		public void CmdShipInfo(string[] args) {

			
			SpaceShip ship = null;
			bool searching = false;

			if (args.Length > 1) {
				searching = true;
				string shipCode = args[1];

				SendLine(string.Format("Searching system for code '{0}*'", shipCode));

				var area = this.game.StarSystemManager.GetStarSystemArea(this.player.SpaceShip.Coords);
				if (area != null) {
					var list = area.GetShipsInSystem();
					foreach (var s in list) {
						if (s.Code.ToLower().StartsWith(shipCode.ToLower())) {
							ship = s;
							break;
						}
					}
				}
			}

			if (ship == null && !searching) 
				ship = this.player.SpaceShip;

			if (ship != null) {
				ioShipInfo sInfo = new ioShipInfo(ship, conn.TerminalWidth);
				SendLine(sInfo.Canvas.ToString());
			}

		}

		public void Cmd_HUD(string[] args) {
			ioHUD hud = new ioHUD(this.conn.TerminalWidth, this.conn.TerminalHeight, this);
			SendLine(hud.GetHud());
		}


		#endregion

		#region Create data commands

		public void Cmd_CreateDirectories(string[] args) {
			StarSystem system = system = player.Location.StarSystem;
			ObjectExtender.Instance.MakeExtentionDirs(system);

			foreach (var body in system.SystemBodies)
				ObjectExtender.Instance.MakeExtentionDirs(body);
		}

		#endregion


		public class PlayerTickState {
			public bool Update { get; set; }
			public PlayerTickState(bool update) { this.Update = update; }
		}

		private bool doingPlayerTick = false;

		public void PlayerTick(object state) {

			if (!doingPlayerTick) {

				doingPlayerTick = true;

				try {
					PlayerTickState jState = state as PlayerTickState;
					bool update = true;

					if (jState != null)
						update = jState.Update;

					if (player != null) {
						if (update)
							player.Update();

						if (player.SpaceShip.Target != null) {
							var dist = player.SpaceShip.SystemLocation.GetInSystemDistance(
								player.SpaceShip.Target.SystemLocation);

							ioShipComputer cmt = new ioShipComputer(this);

							if (dist < 406300) {
								this.StartSendData();
								cmt.SendMessage(string.Format("Target in range: {0} Range: {1}",
									player.SpaceShip.Target,
									SystemLocation.GetDistanceDesc(
										player.SpaceShip.SystemLocation.GetInSystemDistance(
											player.SpaceShip.Target.SystemLocation))));
								this.EndSendData();
							}
						}

						if (!player.SpaceShip.InHiperspace && !player.SpaceShip.InSpace) {
							tickTimer.Change(System.Threading.Timeout.Infinite,
								System.Threading.Timeout.Infinite);

							if (this.inHyperspace) { // i was in hyperspace before
								CmdOutOfJump();
							}

							if (this.inSpace && !this.inHyperspace) { // i was in space before
								CmdTargetReached();
							}

						} else {
							tickTimer.Change(2000, 2000);
						}

					}
				} finally {
					doingPlayerTick = false;
				}
			}
		}

	}

}
