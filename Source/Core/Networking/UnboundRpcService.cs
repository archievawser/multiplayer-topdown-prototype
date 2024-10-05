using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Rabid
{
	public static class UnboundRpcService
	{
		private enum RpcType
		{
			None,
			RunOnServer,
			RunOnClient,
			Multicast
		}

		public static void PatchRpcs()
		{
			foreach (Type type in Application.Instance.GetType().Assembly.GetTypes())
			{
				if (type.GetCustomAttribute<ContainsRpc>() != null)
				{
					foreach (MethodInfo method in type.GetMethods())
					{
						switch(GetRpcType(method))
						{
							case RpcType.RunOnServer:
								Application.Instance.Harmony.Patch(method, new HarmonyMethod(RunOnServerRpcPrefix));
								break;

							case RpcType.Multicast:
								Application.Instance.Harmony.Patch(method, new HarmonyMethod(MulticastRpcPrefix));
								break;
						}
					}
				}
			}
		}


		[MethodImpl(MethodImplOptions.NoInlining)]
		public static unsafe void MulticastRpcPrefix(dynamic[] __args, ref MethodBase __originalMethod)
		{
			MemoryStream stream = new MemoryStream();
			NetBinaryWriter writer = new NetBinaryWriter(stream);
			writer.Write(NetMessageType.BoundRpc);
			writer.Write(RpcIdentifierRegistry[__originalMethod.Name]);

			foreach (var v in __args)
			{
				writer.Write(v);
			}

			writer.Flush();

			fixed (byte* bufferRaw = stream.GetBuffer())
			{
				World.Instance.Networker.SendToAllClients((IntPtr)bufferRaw, (int)stream.Length);
			}

			return;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static unsafe void RunOnServerRpcPrefix(dynamic[] __args, ref MethodBase __originalMethod)
		{
			MemoryStream stream = new MemoryStream();
			NetBinaryWriter writer = new NetBinaryWriter(stream);
			writer.Write(NetMessageType.BoundRpc);
			writer.Write(RpcIdentifierRegistry[__originalMethod.Name]);

			foreach (var v in __args)
			{
				writer.Write((dynamic)v);
			}

			writer.Flush();
			
			fixed (byte* bufferRaw = stream.GetBuffer())
			{
				World.Instance.Networker.SendToServer((IntPtr)bufferRaw, (int)stream.Length);
			}

			return;
		}
		private static RpcType GetRpcType(MethodInfo method)
		{
			if (method.GetCustomAttribute<RunOnServer>() != null)
				return RpcType.RunOnServer;

			if (method.GetCustomAttribute<Multicast>() != null)
				return RpcType.Multicast;

			// TODO: add run on client
			return RpcType.None;
		}

		public static Dictionary<NetId, UnboundRpcService.RpcImplementation> RpcRegistry = new Dictionary<NetId, UnboundRpcService.RpcImplementation>();
		public static Dictionary<string, NetId> RpcIdentifierRegistry = new Dictionary<string, NetId>();
		public delegate void RpcImplementation(NetBinaryReader data);
	}
}
