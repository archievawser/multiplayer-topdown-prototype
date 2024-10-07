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
		public void Connect()
		{
			Role = NetRole.Server;
			ServerManager = SteamNetworkingSockets.CreateNormalSocket<ServerSocket>(NetAddress.LocalHost(27939));
			ServerManager.Networker = this;
		}

		public void Connect(SteamId target)
		{
			if (Role == NetRole.Server)
				Role = NetRole.Host;
			else
				Role = NetRole.Client;

			ClientManager = SteamNetworkingSockets.ConnectNormal<ClientSocket>(NetAddress.LocalHost(27939));
			ClientManager.Networker = this;
		}

		public void Poll()
		{
			if(ClientManager != null)
			{
				ClientManager.Receive();
			} 
			
			if(ServerManager != null)
			{
				ServerManager.Receive();
			}
		}

		public void SendToServer(IntPtr data, int size, SendType type = SendType.Unreliable)
		{
			if(Role == NetRole.Server)
			{
				throw new Exception("Server attempted to send data to itself");
			}

			ClientManager.Connection.SendMessage(data, size, type);
		}

		public void SendToClient(SteamId clientId, IntPtr data, int size, SendType type = SendType.Unreliable)
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
			} else
			{
				throw new Exception("Client " + clientId + " doesn't exist");
			}
		}

		public void SendToAllClients(IntPtr data, int size, SendType type = SendType.Unreliable)
		{
			if(Role == NetRole.Client)
			{
				throw new Exception("Client attempted to send data directly to all clients");
			}

			foreach(Connection conn in ServerManager.Connected)
			{
				conn.SendMessage(data, size, type);
			}
		}

		public NetRole Role;
		public ClientSocket ClientManager;
		public ServerSocket ServerManager;
		public Dictionary<SteamId, Connection> Connections = new Dictionary<SteamId, Connection>();
	}
}
