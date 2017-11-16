using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TelnetServer.Game;

namespace TelnetServer.SocketServer {
	class xConnection {

		private Dictionary<byte, bool> serverOptions = new Dictionary<byte, bool>();
		private Dictionary<byte, bool> clientOptions = new Dictionary<byte, bool>();
		private List<string> terminalTypes = new List<string>();
		public Encoding encoding;
		public bool terminalIsANSI = false;
		public bool terminal256Colors = false;
        public bool negotiationComplete = false;
		public string terminalType = "DUMB";

		#region negotiation functions
		private void VerifyTerminalType() {

			if (terminalType.ToUpper().StartsWith("TIMTIM++")) {
				encoding = Encoding.UTF8;
				terminalIsANSI = true;
				terminal256Colors = true;
			} else if (terminalType.ToUpper().StartsWith("MUSHClient")) {
				encoding = Encoding.UTF8;
				terminalIsANSI = true;
				terminal256Colors = true;
			} else if (terminalType.ToUpper().StartsWith("XTERM")) {
				encoding = Encoding.UTF8;
				terminalIsANSI = true;
				terminal256Colors = true;
			} else if (terminalType.ToUpper().StartsWith("ANSI")) {
				encoding = Encoding.UTF8;
				terminalIsANSI = true;
				terminal256Colors = (terminalType.IndexOf("256") > 0);
			} else if (terminalType.ToUpper().StartsWith("VT")) {
				encoding = Encoding.UTF8;
				terminalIsANSI = true;
				terminal256Colors = (terminalType.IndexOf("256") > 0);
			} else {
				//Defaults to 
				encoding = Encoding.UTF8;
				terminalIsANSI = true;
				terminal256Colors = false;
			}

            if (this.player == null || this.player.PlayerState != ioPlayer.PlayerStateEnum.WaitingForCommand) {
                this.sender.SendSucessMessage(string.Format("Setting encoding to {0}", encoding), "OK");

                if (terminalIsANSI)
                    this.sender.SendSucessMessage("ANSI Terminal ", "YES");
                else
                    this.sender.SendFailMessage("ANSI Terminal", "NO");

                if (terminal256Colors)
                    this.sender.SendSucessMessage("Terminal supports 256 colors ", "YES");
                else
                    this.sender.SendFailMessage("Terminal supports 256 colors", "NO");
            }
                
        }

		private void AddServerOption(byte option, bool ok) {
			if (!serverOptions.ContainsKey(option))
				serverOptions.Add(option, ok);
			else
				serverOptions[option] = ok;
		}

		private void AddClientOption(byte option, bool ok) {
			if (!clientOptions.ContainsKey(option))
				clientOptions.Add(option, ok);
			else
				clientOptions[option] = ok;
		}

		private bool ServerOptionNegociated(byte option) {
			return serverOptions.ContainsKey(option);
		}

		private bool ClientOptionNegociated(byte option) {
			return clientOptions.ContainsKey(option);
		}

		public bool GetServerOption(byte option) {
			if (serverOptions.ContainsKey(option)) {
				return serverOptions[option];
			} else
				return false;
		}

		public bool GetClientOption(byte option) {
			if (serverOptions.ContainsKey(option)) {
				return serverOptions[option];
			} else
				return false;
		}
		#endregion

		public xConnection() {
			this.player = new ioPlayer(this);
            this.sender = new xSend(this);
			this.encoding = Encoding.GetEncoding(437);
			//a new connections was started.
			//lets wait for negociation in another thread and then send our negociation data

			System.Threading.Thread nThread = new System.Threading.Thread(
				new System.Threading.ParameterizedThreadStart(DoServerNegotiation));

			nThread.IsBackground = true;
			nThread.Priority = System.Threading.ThreadPriority.BelowNormal;
			nThread.Start();
		}

		public void DoServerNegotiation(object state) {

			System.Threading.Thread.Sleep(1000); //W8 4 the client :P

			List<byte> data = new List<byte>();

			if (!ServerOptionNegociated((byte)TelnetOption.Echo)) {
				data.AddRange(new byte[] { 255, (byte)TelnetCommand.WILL, (byte)TelnetOption.Echo });
				Console.WriteLine(">> SERVER {0} {1}", TelnetCommand.WILL, TelnetOption.Echo);
			}
			if (!ServerOptionNegociated((byte)TelnetOption.SuppressGoAhead)) {
				data.AddRange(new byte[] { 255, (byte)TelnetCommand.WILL, (byte)TelnetOption.SuppressGoAhead });
				Console.WriteLine(">> SERVER {0} {1}", TelnetCommand.WILL, TelnetOption.SuppressGoAhead);
            }

			if (!ClientOptionNegociated((byte)TelnetOption.WindowSize)) {
				data.AddRange(new byte[] { 255, (byte)TelnetCommand.DO, (byte)TelnetOption.WindowSize });
				Console.WriteLine(">> SERVER {0} {1}", TelnetCommand.DO, TelnetOption.WindowSize);
            }

			if (!ClientOptionNegociated((byte)TelnetOption.TerminalType)) {
				data.AddRange(new byte[] { 255, (byte)TelnetCommand.DO, (byte)TelnetOption.TerminalType });
				Console.WriteLine(">> SERVER {0} {1}", TelnetCommand.DO, TelnetOption.TerminalType);
            }

			if (!ClientOptionNegociated((byte)TelnetOption.Charset)) {
				data.AddRange(new byte[] { 255, (byte)TelnetCommand.WILL, (byte)TelnetOption.Charset });
				Console.WriteLine(">> SERVER {0} {1}", TelnetCommand.WILL, TelnetOption.Charset);
            }

			server.Send(data.ToArray(), this);

            System.Threading.Thread.Sleep(3000); //waiting 3 sec before setting the main negotiation fase complete.
            negotiationComplete = true;

        }

		public byte[] buffer;
		public System.Net.Sockets.Socket socket;
		public Server server;
		public ioPlayer player;
        public xSend sender;

		private int terminalWidth = 80; //Default
		private int terminalHeight = 24; //Default

		public int TerminalWidth {
			get { return terminalWidth; }
		}

		public int TerminalHeight {
			get { return terminalHeight; }
		}
		private Queue<byte> tnNegCommands = new Queue<byte>();
		private Queue<byte> rawData = new Queue<byte>();
		private Queue<char> echoData = new Queue<char>();
        private int echoPosition = 0;
		private Queue<char> commandData = new Queue<char>();
        private List<string> commandHistory = new List<string>();
        private int commandHistoryPointer = -1;
		private bool dirty = true;
		private bool echoSent = false;
        private bool skipEcho = false;

        private void ResetHistoryPointer() {
            commandHistoryPointer = -1;
        }

        private string HistoryUp() {

            skipEcho = true;

            if (commandHistoryPointer < 0)
                commandHistoryPointer = commandHistory.Count - 1;
            else
                commandHistoryPointer--;

            if (commandHistoryPointer >= commandHistory.Count)
                commandHistoryPointer = commandHistory.Count - 1;

            if (commandHistoryPointer >= 0) {
                return commandHistory[commandHistoryPointer];
            }

            return null;

        }

        private string HistoryDown() {

            skipEcho = true;

            if (commandHistoryPointer < 0)
                commandHistoryPointer = 0;
            else
                commandHistoryPointer++;

            if (commandHistoryPointer >= commandHistory.Count)
                commandHistoryPointer = commandHistory.Count - 1;

            if (commandHistoryPointer > 0) {
                return commandHistory[commandHistoryPointer];
            }

            return null;
        }

        private void AddHistory(string command) {
            if (string.IsNullOrEmpty(command)) return;
            if (commandHistory.Count == 0 || command != commandHistory[commandHistory.Count - 1])
                commandHistory.Add(command);
        }

		private void Send(byte[] data) {
			if (server != null) {
				this.dirty = true;
				if (echoSent) this.server.Send(Encoding.UTF8.GetBytes(("\r\n").ToArray()), this);
				this.server.Send(data, this);
				echoSent = false;
			}
		}

		public void Send(string data) {

			if (server != null) {
				this.dirty = true;
				if (echoSent) this.server.Send(Encoding.UTF8.GetBytes(("\r\n").ToArray()), this);
				this.server.Send(Encoding.UTF8.GetBytes(data.ToArray()), this);
				echoSent = false;
			}
		}

		public void SafeWithPrompt(string prompt, string data) {

			Send(data);
			SendPrompt(prompt);

		}

		public void SendPrompt(string prompt) {

			
			string output;
			string echo;

			if (GetServerOption((byte)TelnetOption.Echo))
				echo = new string(echoData.ToArray());
			else 
				echo = string.Empty;

			//Have to deal with the problem that some clients don't cariagereturns
			if (GetServerOption((byte)TelnetOption.Echo)) {
				output = string.Format("{0} {1}", prompt, echo);
			} else {
				output = string.Format("{0} {1}", prompt, echo);
			}

			//this.Send((byte)TelnetCommand.IAC, (byte)TelnetCommand.EL);
			this.Send(output);
			this.dirty = false;

		}

		public void ProcessBuffer(int lenght) {

			//string data = Encoding.UTF8.GetString(buffer, 0, lenght);
			ReadTelnetData(lenght);

			if (telnetCommandStatus == TelnetCommandStatus.DATA && tnNegCommands.Count > 0) {
				ParseTelnetNegData();
			}

			byte[] aRawData = rawData.ToArray();
			char[] aChars = encoding.GetChars(aRawData, 0, aRawData.Length);

			for (int i = 0; i < aChars.Length; i++) {
				commandData.Enqueue(aChars[i]);
			}

			List<string> commands = ParseCommandData();

			if (player.PlayerState == ioPlayer.PlayerStateEnum.WaitingForInit) {
				player.Init();
			} else {
				echoSent = false;
				if (GetServerOption((byte)TelnetOption.Echo)) {
					//this is safe because after the command the server will send 
					//the prompt with echo data in the buffer
					//Console.WriteLine("'{0}'", Encoding.UTF8.GetString(aRawData));

					if (!player.ProcessingCommand) {
						var wasDirty = dirty;

						if (dirty || (commands.Count > 0 && this.echoData.Count == 0)) {
							this.Send("\r\n");
						}

						if (wasDirty) {
							this.SendPrompt(player.GetPrompt());
                            echoPosition = this.echoData.Count;
                            echoSent = true;
						} else if (this.echoData.Count > 0) {
                            if (!skipEcho) {

                                while (echoPosition >= this.echoData.Count) {
                                    //this.server.Send(new byte[] { 0x08 }, this);
                                    echoPosition--;
                                }

                                this.server.Send(
                                    Encoding.UTF8.GetBytes(
                                        this.echoData.Skip(echoPosition).ToArray()
                                    ), 
                                    this
                               );
                            }
                            //this.server.Send(rawData.ToArray(), this);
                            echoPosition = this.echoData.Count;
							echoSent = true;
                            skipEcho = false;
						}
					}
				}
			}

			foreach (string command in commands) {
                AddHistory(command);
				player.ProcessRawCommand(command);
			}

			rawData.Clear();
		}

		private TelnetCommandStatus telnetCommandStatus = TelnetCommandStatus.DATA;
		internal enum TelnetCommandStatus { DATA, IAC, SB, ESC, ESC2, ESCNum, ESCEnd }
		/// <summary>
		/// Handles a Telnet command.
		/// </summary>
		/// <param name="e">Information about the data received.</param>
		private void ReadTelnetData(int lenght) {

			for (int i = 0; i < lenght; i++) {

				byte currByte = buffer[i];

				//Console.Write(" {0} ", (int)currByte);

				switch (telnetCommandStatus) {
					case TelnetCommandStatus.DATA:
						if (currByte != (int)TelnetCommand.IAC) {
							rawData.Enqueue(currByte);
						} else {
							telnetCommandStatus = TelnetCommandStatus.IAC;
							tnNegCommands.Enqueue(currByte);
						}
						break;
					case TelnetCommandStatus.IAC:
						if (currByte == (int)TelnetCommand.IAC) {
							rawData.Enqueue(currByte);
							telnetCommandStatus = TelnetCommandStatus.DATA;
						} else if (currByte == (int)TelnetCommand.SB) {
							tnNegCommands.Enqueue(currByte);
							telnetCommandStatus = TelnetCommandStatus.SB;
						} else {
							tnNegCommands.Enqueue(buffer[i]);
							tnNegCommands.Enqueue(buffer[++i]);
							telnetCommandStatus = TelnetCommandStatus.DATA;
						}
						break;
					case TelnetCommandStatus.SB:
						tnNegCommands.Enqueue(currByte);
						if (currByte == (int)TelnetCommand.SE) {
							telnetCommandStatus = TelnetCommandStatus.DATA;
						}
						break;
				}

			}

		}

		private void ParseTelnetNegData() {

			byte[] negBuffer = tnNegCommands.ToArray();
			tnNegCommands.Clear();

			for (int i = 0; i < negBuffer.Length; i += 3) {
				if (negBuffer.Length - i < 3) {
					break;
				}

				if (negBuffer[i] == (int)TelnetCommand.IAC) {

					TelnetCommand command = (TelnetCommand)negBuffer[i + 1];
					TelnetOption option = (TelnetOption)negBuffer[i + 2];

					Console.WriteLine("CLIENT {0} {1}", command, option);

					switch (command) {
						case TelnetCommand.WONT:
							if (!ClientOptionNegociated((byte)option) || GetClientOption((byte)option))
								server.Send(new byte[] { 255, (byte)TelnetCommand.DONT, (byte)option }, this);
							Console.WriteLine("SERVER {0} {1}", TelnetCommand.DONT, option);
							AddClientOption((byte)option, false);
                            this.sender.SendFailMessage(string.Format("Negotiating Client Option '{0}'", option), "OFF");
							break;
						case TelnetCommand.DONT:
							if (!ServerOptionNegociated((byte)option) || GetServerOption((byte)option))
								server.Send(new byte[] { 255, (byte)TelnetCommand.WONT, (byte)option }, this);
							Console.WriteLine("SERVER {0} {1}", TelnetCommand.WONT, option);
							AddServerOption((byte)option, false);
                            this.sender.SendFailMessage(string.Format("Negotiating Server Option '{0}'", option), "OFF");

                            if (option == TelnetOption.SuppressGoAhead) {
								//I WILL DISCONNECT ECHO ALSO!!!
								server.Send(new byte[] { 
											(byte)TelnetCommand.IAC, 
											(byte)TelnetCommand.WONT, 
											(byte)TelnetOption.Echo }, this);
								AddServerOption((byte)TelnetOption.Echo, false);
                                this.sender.SendFailMessage(string.Format("Negotiating Server Option '{0}'", option), "OFF");
                                Console.WriteLine("SERVER {0} {1}", TelnetCommand.WONT, TelnetOption.Echo);
							}

							break;
						case TelnetCommand.DO:
							switch (option) {
								case TelnetOption.Echo:
									if (!ServerOptionNegociated((byte)option) || !GetServerOption((byte)option))
										server.Send(new byte[] { 
											(byte)TelnetCommand.IAC, 
											(byte)TelnetCommand.WILL, 
											(byte)TelnetOption.Echo }, this);

									Console.WriteLine("SERVER {0} {1}", TelnetCommand.WILL, TelnetOption.Echo);
									AddServerOption((byte)option, true);
                                    this.sender.SendSucessMessage(string.Format("Negotiating Server Option '{0}'", option), "ON");
                                    break;
								case TelnetOption.SuppressGoAhead:
									if (!ServerOptionNegociated((byte)option) || !GetServerOption((byte)option))
										server.Send(new byte[] { 
											(byte)TelnetCommand.IAC, 
											(byte)TelnetCommand.WILL, 
											(byte)TelnetOption.SuppressGoAhead }, this);
									Console.WriteLine("SERVER {0} {1}", TelnetCommand.WILL, TelnetOption.SuppressGoAhead);
									AddServerOption((byte)option, true);
                                    this.sender.SendSucessMessage(string.Format("Negotiating Server Option '{0}'", option), "ON");
                                    break;
							}
							break;
						case TelnetCommand.WILL:

							switch (option) {
								case TelnetOption.SuppressGoAhead:
									server.Send(new byte[] { 
										(byte)TelnetCommand.IAC, 
										(byte)TelnetCommand.DO, 
										(byte)TelnetOption.SuppressGoAhead }, this);
									Console.WriteLine("SERVER {0} {1}", TelnetCommand.DO, TelnetOption.SuppressGoAhead);
									AddClientOption((byte)option, true);
                                    this.sender.SendSucessMessage(string.Format("Negotiating Client Option '{0}'", option), "ON");
                                    break;
								case TelnetOption.Echo:
									server.Send(new byte[] { 
										(byte)TelnetCommand.IAC, 
										(byte)TelnetCommand.DO, 
										(byte)TelnetOption.Echo }, this);
									Console.WriteLine("SERVER {0} {1}", TelnetCommand.DO, TelnetOption.Echo);
									AddClientOption((byte)option, true);
                                    this.sender.SendSucessMessage(string.Format("Negotiating Client Option '{0}'", option), "ON");
                                    break;
								case TelnetOption.WindowSize:
									server.Send(new byte[] { 
										(byte)TelnetCommand.IAC, 
										(byte)TelnetCommand.DO, 
										(byte)TelnetOption.WindowSize }, this);
									Console.WriteLine("SERVER {0} {1}", TelnetCommand.DO, TelnetOption.WindowSize);
									AddClientOption((byte)option, true);
                                    this.sender.SendSucessMessage(string.Format("Negotiating Client Option '{0}'", option), "ON");
                                    //server.Send(new byte[] { 
                                    //    (byte)TelnetCommand.IAC, 
                                    //    (byte)TelnetCommand.DO, 
                                    //    (byte)TelnetOption.DataEntryTerminal}, this);
                                    //Console.WriteLine("SERVER {0} {1}", TelnetCommand.DO, TelnetOption.DataEntryTerminal);
                                    break;
								case TelnetOption.TerminalType:
									server.Send(new byte[] { 
								        (byte)TelnetCommand.IAC, 
								        (byte)TelnetCommand.SB, 
								        (byte)TelnetOption.TerminalType, 0x1, 
										(byte)TelnetCommand.IAC, 
								        (byte)TelnetCommand.SE 
									}, this);
									Console.WriteLine("SERVER {0} {1}", TelnetCommand.SB, TelnetOption.TerminalType);
									AddClientOption((byte)option, true);
                                    this.sender.SendSucessMessage(string.Format("Negotiating Client Option '{0}'", option), "ON");
                                    break;
								case TelnetOption.Charset:
									server.Send(new byte[] { 
								        (byte)TelnetCommand.IAC, 
								        (byte)TelnetCommand.SB, 
								        (byte)TelnetOption.Charset, 0x1, 
										(byte)'U', (byte)'T', (byte)'F', (byte)'-', (byte)'8',
										(byte)TelnetCommand.IAC, 
								        (byte)TelnetCommand.SE
									}, this);
									Console.WriteLine("SERVER {0} {1}", TelnetCommand.SB, TelnetOption.TerminalType);
									AddClientOption((byte)option, true);
                                    this.sender.SendSucessMessage(string.Format("Negotiating Client Option '{0}'", option), "ON");
                                    break;
							}
							break;

						case TelnetCommand.SB:
							switch (option) {
								case TelnetOption.WindowSize:
									int w = negBuffer[i + 3] << 4 | negBuffer[i + 4];
									int h = negBuffer[i + 5] << 4 | negBuffer[i + 6];

									this.terminalHeight = h;
									this.terminalWidth = w;

									Console.WriteLine("Window Size: {0} {1}", w, h);
									i = i + 6;
                                    if (this.player == null || this.player.PlayerState != ioPlayer.PlayerStateEnum.WaitingForCommand)
                                        this.sender.SendMessage(string.Format("Got Terminal Size: '{0}x{1}'", w, h));

                                    break;
								case TelnetOption.TerminalType:
									byte Opt = negBuffer[i + 3];
									List<byte> tType = new List<byte>();
									bool isValid = false;
									for (int ct = i + 4; ct < negBuffer.Length; ct++) {
										i = ct;
										if (negBuffer[ct] == (byte)TelnetCommand.IAC
											&& ct + 1 < negBuffer.Length
											&& negBuffer[ct + 1] == (byte)TelnetCommand.SE) {

											isValid = true;
											break;

										} else {
											tType.Add(negBuffer[ct]);
										}
									}

									string tt = Encoding.UTF8.GetString(
										Encoding.Convert(encoding, Encoding.UTF8, tType.ToArray())).ToUpper();

									if (isValid) {
										if (!terminalTypes.Contains(tt)) {
											terminalTypes.Add(tt);
											terminalType = tt;
											VerifyTerminalType();
											Console.WriteLine("Got known terminal type: {0}", tt);
                                            if (this.player == null || this.player.PlayerState != ioPlayer.PlayerStateEnum.WaitingForCommand)
                                                this.sender.SendSucessMessage("Got known terminal type", tt);
                                        } else {
											VerifyTerminalType();
											Console.WriteLine("Got unknown Terminal type: {0}", tt);
                                            if (this.player == null || this.player.PlayerState != ioPlayer.PlayerStateEnum.WaitingForCommand)
                                                this.sender.SendFailMessage("Got unknown terminal type", tt);
                                        }
									} else {
										Console.WriteLine("Terminal type: {0}", tt);
										Console.WriteLine("WTF!!!!");
                                        if (this.player == null || this.player.PlayerState != ioPlayer.PlayerStateEnum.WaitingForCommand)
                                            this.sender.SendFailMessage("Unable to decode terminal type", tt);
                                    }

                                    

                                    break;
								case TelnetOption.Charset:
									Console.WriteLine("Charset");
									break;
							}
							break;
					}
				}
			}

			//Console.WriteLine("----------------");

		}

		private List<string> ParseCommandData() {

			StringBuilder sBuilder = new StringBuilder();
			List<string> commands = new List<string>();

			echoData.Clear();

			while (commandData.Count > 0) {

				char c = commandData.Dequeue();
                if (c == '\n') {
                    commands.Add(sBuilder.ToString());
                    sBuilder = new StringBuilder();
                    echoData.Clear();
                    ResetHistoryPointer();
                } else if (c == '\u001b') { //escape sequence
                    if (commandData.Count > 0) {
                        char p = commandData.Dequeue();
                        if (p == '[') {
                            char cmd = commandData.Dequeue();
                            switch (cmd) {
                                case 'A':
                                    echoData.Clear();
                                    sBuilder = new StringBuilder();
                                    sBuilder.Append(HistoryUp());
                                    break;
                                case 'B':
                                    echoData.Clear();
                                    sBuilder = new StringBuilder();
                                    sBuilder.Append(HistoryDown());
                                    break;
                            }
                        }
                    }
                } else if (!char.IsControl(c)) {
					sBuilder.Append(c);
					echoData.Enqueue(c);
                    ResetHistoryPointer();
				} else if (c == (char)0x08 || c == (char)0x7F) {
					if (sBuilder.Length > 0) {
						sBuilder.Remove(sBuilder.Length - 1, 1);
						echoData.Enqueue(c);
                        ResetHistoryPointer();
                    }
				}
			}

			commandData.Clear();

			string remaining = sBuilder.ToString();
			if (remaining != null && remaining.Length > 0) {
				for (int i = 0; i < remaining.Length; i++) {
					commandData.Enqueue(remaining[i]);
				}
			}

			return commands;

		}

		internal enum TelnetCommand : byte {
			SE = 240,
			NOP = 241,
			DM = 242,
			BRK = 243,
			IP = 244,
			AO = 245,
			AYT = 246,
			EC = 247,
			EL = 248,
			GA = 249,
			SB = 250,
			WILL = 251,
			WONT = 252,
			DO = 253,
			DONT = 254,
			IAC = 255
		}

		/// <summary>
		/// Telnet command options.
		/// </summary>
		internal enum TelnetOption : byte {
			Echo = 1,
			SuppressGoAhead = 3,
			Status = 5,
			TimingMark = 6,
			DataEntryTerminal = 20,
			TerminalType = 24,
			WindowSize = 31,
			TerminalSpeed = 32,
			RemoteFlowControl = 33,
			LineMode = 34,
			EnvironmentVariables = 36,
			Charset = 42
		}
	}
}
