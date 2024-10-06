using Steamworks;
using Steamworks.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rabid.Netcode.Steam
{
	public class ServerSocket : SocketManager
	{
		public override void OnConnected(Connection connection, ConnectionInfo info)
		{
			base.OnConnected(connection, info);

			RpcTesting.PossessPlayer(100);
			Trace.WriteLine(connection.Id + " connected to server");
		}

		public override void OnConnecting(Connection connection, ConnectionInfo info)
		{
			base.OnConnecting(connection, info);

			Trace.WriteLine(connection.Id + " connecting to server");
		}

		public override void OnDisconnected(Connection connection, ConnectionInfo info)
		{
			base.OnDisconnected(connection, info);
		}

		public override unsafe void OnMessage(Connection connection, NetIdentity identity, nint data, int size, long messageNum, long recvTime, int channel)
		{
			UnmanagedMemoryStream stream = new UnmanagedMemoryStream((byte*)data, size);
			NetBinaryReader reader = new NetBinaryReader(stream);

			NetMessageType type = (NetMessageType)reader.ReadByte();

			switch (type)
			{
				case NetMessageType.UnboundRpc:
					{
						// the key of the RPC in the unbound rpc registry
						NetId rpcId = reader.ReadByte();

						UnboundRpcService.RpcRegistry[rpcId](reader);
						break;
					}
				case NetMessageType.BoundRpc:
					{
						// the rpcObjectId is the NetId of the entity containing the RPC
						// we then use this to call the implementation on the same object across the network
						NetId rpcObjectId = reader.ReadByte();

						// the key of the RPC in the rpc entity's registry
						NetId rpcId = reader.ReadByte();

						Entity rpcObject = Entity.Resolve(rpcObjectId);
						rpcObject.RpcRegistry[rpcId](reader);
						break;
					}
			}


		}

		public NetDevice Networker;
	}
}
