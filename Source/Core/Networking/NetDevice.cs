using Steamworks;
using Steamworks.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Rabid.Netcode.Steam
{
	public class NetDevice
	{
		public void Host()
		{
			Role = NetRole.Host;
			ServerManager = SteamNetworkingSockets.CreateRelaySocket<ServerSocket>();
			ServerManager.Networker = this;
		}

		public void Connect(SteamId target)
		{
			Role = NetRole.Client;
			ClientManager = SteamNetworkingSockets.ConnectRelay<ClientSocket>(target);
			ClientManager.Networker = this;
		}

		public void Poll()
		{
			ClientManager?.Receive();
			ServerManager?.Receive();
		}

		public void SendToServer(IntPtr data, int size, SendType type = SendType.Reliable)
		{
			if(Role != NetRole.Client)
			{
				return;
			}

			ClientManager.Connection.SendMessage(data, size, type);
		}

		public void SendToClient(SteamId clientId, IntPtr data, int size, SendType type = SendType.Reliable)
		{
			if(Role == NetRole.Client)
			{
				throw new Exception("Client attempted to send data directly to another client or itself");
			}

			Connection conn;
			bool targetClientExists = Connections.TryGetValue(clientId, out conn);

			if(targetClientExists)
			{
				conn.SendMessage(data, size, type);
			}
		}

		public void SendToAllClients(IntPtr data, int size, SteamId[] exceptions = null, SendType type = SendType.Reliable)
		{
			if(Role == NetRole.Client)
			{
				throw new Exception("Client attempted to send data directly to all clients");
			}

			foreach(Connection conn in ServerManager.Connected)
			{
				if (exceptions?.Contains(ConnectionInfos[conn.Id].Identity.SteamId) ?? false)
					continue;

				conn.SendMessage(data, size, type);
			}
		}

		public NetRole Role;
		public ClientSocket ClientManager;
		public ServerSocket ServerManager;
		public Dictionary<SteamId, Connection> Connections = new Dictionary<SteamId, Connection>();
		public Dictionary<uint, ConnectionInfo> ConnectionInfos = new Dictionary<uint, ConnectionInfo>();
	}
}
