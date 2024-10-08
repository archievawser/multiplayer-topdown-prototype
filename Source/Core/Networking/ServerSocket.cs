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

			Networker.ConnectionInfos[connection.Id] = info;
			Networker.Connections[info.Identity.SteamId] = connection;

			NetId playerId = IdHelper.GetNextId();
			
			foreach(var v in NetEntity.GetAll())
			{
				RpcTesting.PlayerJoined(info.Identity.SteamId, v.Id);
			}

			RpcTesting.CreatePlayer(playerId, false);

			foreach (var v in Networker.Connections)
			{
				if (v.Key == info.Identity.SteamId)
					continue;

				// tell connection that a player joined (and the player's NetId)
				RpcTesting.PlayerJoined(v.Key, playerId);
			}

			
			
			RpcTesting.PossessPlayer(info.Identity.SteamId, playerId);

			Trace.WriteLine(info.Identity.SteamId + " has connected to our server");
		}

		public override void OnConnecting(Connection connection, ConnectionInfo info)
		{
			base.OnConnecting(connection, info);

			Trace.WriteLine(info.Identity.SteamId + " is attempting to connect to our server");
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

						UnboundRpcService.ServerRpcRegistry[rpcId](identity.SteamId.Value, reader);
						break;
					}
				case NetMessageType.BoundRpc:
					{
						// the rpcObjectId is the NetId of the entity containing the RPC
						// we then use this to call the implementation on the same object across the network
						NetId rpcObjectId = reader.ReadByte();

						// the key of the RPC in the rpc entity's registry
						NetId rpcId = reader.ReadByte();

						NetEntity rpcObject = NetEntity.Resolve(rpcObjectId);
						rpcObject.ServerRpcRegistry[rpcId](identity.SteamId.Value, reader);
						break;
					}
			}
		}

		public NetDevice Networker;
	}
}
