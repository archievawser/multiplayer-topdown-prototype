using Steamworks;
using Steamworks.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Rabid.Netcode.Steam
{
	public class ClientSocket : ConnectionManager
	{
		public override void OnConnected(ConnectionInfo info)
		{

		}

		public override void OnConnecting(ConnectionInfo info)
		{
		
		}

		public override void OnDisconnected(ConnectionInfo info)
		{

		}

		public override unsafe void OnMessage(nint data, int size, long messageNum, long recvTime, int channel)
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
