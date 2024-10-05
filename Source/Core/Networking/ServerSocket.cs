using Steamworks;
using Steamworks.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rabid.Netcode.Steam
{
	public class ServerSocket : SocketManager
	{
		public override void OnConnected(Connection connection, ConnectionInfo info)
		{
			connection.Accept();
		}

		public override void OnConnecting(Connection connection, ConnectionInfo info)
		{

		}

		public override void OnDisconnected(Connection connection, ConnectionInfo info)
		{

		}

		public override void OnMessage(Connection connection, NetIdentity identity, nint data, int size, long messageNum, long recvTime, int channel)
		{

		}

		public NetDevice Networker;
	}
}
