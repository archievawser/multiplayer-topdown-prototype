using Steamworks;
using Steamworks.Data;
using System;
using System.Collections.Generic;
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
			Role = NetRole.Host;
			ServerManager = SteamNetworkingSockets.CreateNormalSocket<ServerSocket>(NetAddress.AnyIp(21893));
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
			if(ClientManager != null)
			{
				ClientManager.Receive();
			} 
			else if(ServerManager != null)
			{
				ServerManager.Receive();
			}
		}

		public void SendToServer(IntPtr data, int size, SendType type = SendType.Unreliable)
		{
			if(Role != NetRole.Client)
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

			NetConnection conn;
			bool targetClientExists = Connections.TryGetValue(clientId, out conn);

			if(targetClientExists)
			{
				conn.Base.SendMessage(data, size, type);
			}
		}

		public void SendToAllClients(IntPtr data, int size, SendType type = SendType.Unreliable)
		{
			if(Role == NetRole.Client)
			{
				throw new Exception("Client attempted to send data directly to all clients");
			}

			foreach(NetConnection conn in Connections.Values)
			{
				conn.Base.SendMessage(data, size, type);
			}
		}

		public NetRole Role;
		public ClientSocket ClientManager;
		public ServerSocket ServerManager;
		public Dictionary<SteamId, NetConnection> Connections;
	}
}
