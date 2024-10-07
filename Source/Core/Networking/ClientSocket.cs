using Steamworks;
using Steamworks.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
			base.OnConnected(info);

			Trace.WriteLine("Connected to server");
		}

		public override void OnConnecting(ConnectionInfo info)
		{
			base.OnConnecting(info);

			Trace.WriteLine("Connecting to server");
		}

		public override void OnDisconnected(ConnectionInfo info)
		{
			base.OnDisconnected(info);
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

						NetEntity rpcObject = NetEntity.Resolve(rpcObjectId);
						rpcObject.RpcRegistry[rpcId](reader);

						break;
					}
			}

			base.OnMessage(data, size, messageNum, recvTime, channel);
		}

		public NetDevice Networker;
	}
}
