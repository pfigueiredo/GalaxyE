using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace TelnetServer.SocketServer {
	class Server {

		private List<xConnection> _sockets;
		private System.Net.Sockets.Socket _serverSocket;
		private int _port = 6364;
		private int _backlog = 100;
		private int _bufferSize = 1024;

		public Server(int port, int backlog, int bufferSize) {
			this._sockets = new List<xConnection>();
			this._port = port;
			this._backlog = backlog;
			this._bufferSize = bufferSize;
		}

		public bool Start() {

			System.Net.IPHostEntry localhost = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
			System.Net.IPEndPoint serverEndPoint;

			try {
				serverEndPoint = new System.Net.IPEndPoint(System.Net.IPAddress.Any, _port);

			} catch (System.ArgumentOutOfRangeException e) {
				throw new ArgumentOutOfRangeException("Port number entered would seem to be invalid, should be between 1024 and 65000", e);
			}

			try {
				_serverSocket = new System.Net.Sockets.Socket(
						serverEndPoint.Address.AddressFamily, 
						SocketType.Stream, 
						ProtocolType.Tcp
				);
			} catch (System.Net.Sockets.SocketException e) {
				throw new ApplicationException("Could not create socket, check to make sure not duplicating port", e);
			}

			try {
				_serverSocket.Bind(serverEndPoint);
				_serverSocket.Listen(_backlog);
			} catch (Exception e) {
				throw new ApplicationException("Error occured while binding socket, check inner exception", e);
			}
			try {
				//warning, only call this once, this is a bug in .net 2.0 that breaks if 
				// you're running multiple asynch accepts, this bug may be fixed, but
				// it was a major pain in the ass previously, so make sure there is only one
				//BeginAccept running
				_serverSocket.BeginAccept(new AsyncCallback(acceptCallback), _serverSocket);
			} catch (Exception e) {
				throw new ApplicationException("Error occured starting listeners, check inner exception", e);
			}

			return true;
		}

		private void acceptCallback(IAsyncResult result) {
			xConnection conn = null; // new xConnection();
			try {
				//Finish accepting the connection
				System.Net.Sockets.Socket s = (System.Net.Sockets.Socket)result.AsyncState;
				conn = new xConnection();
				conn.socket = s.EndAccept(result);
				conn.buffer = new byte[_bufferSize];
				conn.server = this;
				lock (_sockets) {
					_sockets.Add(conn);
				}
				//Queue recieving of data from the connection
				conn.socket.BeginReceive(conn.buffer, 0, conn.buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), conn);
				//Queue the accept of the next incomming connection
				_serverSocket.BeginAccept(new AsyncCallback(acceptCallback), _serverSocket);
			} catch (SocketException) {
				if (conn.socket != null) {
					conn.socket.Close();
					lock (_sockets) {
						_sockets.Remove(conn);
					}
				}
				//Queue the next accept, think this should be here, stop attacks based on killing the waiting listeners
				_serverSocket.BeginAccept(new AsyncCallback(acceptCallback), _serverSocket);
			} catch (Exception) {
				if (conn.socket != null) {
					conn.socket.Close();
					lock (_sockets) {
						_sockets.Remove(conn);
					}
				}
				//Queue the next accept, think this should be here, stop attacks based on killing the waiting listeners
				_serverSocket.BeginAccept(new AsyncCallback(acceptCallback), _serverSocket);
			}
		}

		private void ReceiveCallback(IAsyncResult result) {
			//get our connection from the callback
			xConnection conn = (xConnection)result.AsyncState;
			//catch any errors, we'd better not have any
			try {
				//Grab our buffer and count the number of bytes receives
				int bytesRead = conn.socket.EndReceive(result);
				//make sure we've read something, if we haven't it supposadly means that the client disconnected
				if (bytesRead > 0) {
					//put whatever you want to do when you receive data here
					conn.ProcessBuffer(bytesRead);
					conn.socket.BeginReceive(conn.buffer, 0, 4, SocketFlags.None, new AsyncCallback(ReceiveCallback), conn);
				} else {
					//Callback run but no data, close the connection
					//supposadly means a disconnect
					//and we still have to close the socket, even though we throw the event later
					conn.socket.Close();
					lock (_sockets) {
						_sockets.Remove(conn);
					}
				}
			} catch (SocketException) {
				//Something went terribly wrong
				//which shouldn't have happened
				if (conn.socket != null) {
					conn.socket.Close();
					lock (_sockets) {
						_sockets.Remove(conn);
					}
				}
			}
		}

		public bool Send(byte[] message, int lenght, xConnection conn) {
			if (conn != null && conn.socket.Connected) {
				lock (conn.socket) {

					//we use a blocking mode send, no async on the outgoing
					//since this is primarily a multithreaded application, shouldn't cause problems to send in blocking mode
					conn.socket.Send(message, lenght, SocketFlags.None);
				}
			} else
				return false;
			return true;
		}

		public bool Send(byte[] message, xConnection conn) {
			if (conn != null && conn.socket.Connected) {
				lock (conn.socket) {

					//we use a blocking mode send, no async on the outgoing
					//since this is primarily a multithreaded application, shouldn't cause problems to send in blocking mode
					conn.socket.Send(message, message.Length, SocketFlags.None);
				}
			} else
				return false;
			return true;
		}

	}
}
