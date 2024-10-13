using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Rabid
{
	public unsafe class NetEntity : Entity
	{
		public NetEntity()
		{ }

		public static NetEntity Resolve(NetId id)
		{
			return mEntityNetRegistry[id];
		}		
		
		public static NetEntity[] GetAll()
		{
			// TODO: check performance
			return mEntityNetRegistry.Values.ToArray();
		}

		public void SetNetIdentity(NetId id)
		{
			Id = id;
			mEntityNetRegistry.Add(id, this);
		}



		public override void Prepare()
		{
			foreach (Component component in mComponents.Values)
			{
#if DEBUG
				foreach (RequiresComponent attrib in component.GetType().GetCustomAttributes<RequiresComponent>())
				{
					if (!mComponents.ContainsKey(attrib.ComponentType))
					{
						throw new Exception("Component requirement not met");
					}
				}
#endif

				SetupRPCs();

				component.Prepare();
			}
		}

		public int MethodInfoComparison(MethodInfo a, MethodInfo b)
		{
			return a.Name.CompareTo(b.Name);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void RosRpcPrefix(NetEntity __instance, dynamic[] __args, MethodBase __originalMethod)
		{
			MemoryStream stream = new MemoryStream();
			NetBinaryWriter writer = new NetBinaryWriter(stream);

			writer.Write(NetMessageType.BoundRpc);
			writer.Write(__instance.Id.Value);
			writer.Write(__instance.RpcIdentifierRegistry[__originalMethod.Name]);

			foreach (var v in __args)
			{
				writer.Write(v);
			}

			writer.Flush();

			unsafe
			{
				fixed (byte* bufferRaw = stream.GetBuffer())
				{
					World.Instance.Networker.SendToServer((IntPtr)bufferRaw, (int)stream.Length);
				}
			}

			return;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void MulticastRpcPrefix(NetEntity __instance, dynamic[] __args, MethodBase __originalMethod)
		{
			MemoryStream stream = new MemoryStream();
			NetBinaryWriter writer = new NetBinaryWriter(stream);
			writer.Write(NetMessageType.BoundRpc);
			writer.Write(__instance.Id.Value);
			writer.Write(__instance.RpcIdentifierRegistry[__originalMethod.Name]);

			foreach (var v in __args)
			{
				writer.Write(v);
			}

			writer.Flush();

			unsafe
			{
				fixed (byte* bufferRaw = stream.GetBuffer())
				{
					World.Instance.Networker.SendToAllClients((IntPtr)bufferRaw, (int)stream.Length);
				}
			}

			return;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void RunOnClientRpcPrefix(NetEntity __instance, dynamic[] __args, MethodBase __originalMethod)
		{
			MemoryStream stream = new MemoryStream();
			NetBinaryWriter writer = new NetBinaryWriter(stream);
			writer.Write(NetMessageType.BoundRpc);
			writer.Write(__instance.Id.Value);
			writer.Write(__instance.RpcIdentifierRegistry[__originalMethod.Name]);

			// start at 1 to account for the first arg (target steam id)
			int i = 1;
			for (; i < __args.Length; i++)
			{
				writer.Write(__args[i]);
			}

			writer.Flush();

			unsafe
			{
				fixed (byte* bufferRaw = stream.GetBuffer())
				{
					World.Instance.Networker.SendToClient(__args[0], (IntPtr)bufferRaw, (int)stream.Length);
				}
			}

			return;
		}

		private void SetupRPCs()
		{
			// order MethodInfo[] to ensure RPC function is at the same index of Rpcs across all machines
			MethodInfo[] info = GetType().GetMethods();
			Array.Sort(info, MethodInfoComparison);

			int rpcIndex = 0;
			foreach (MethodInfo method in GetType().GetMethods())
			{
				// RPCs can't be generic
				if (method.IsGenericMethod)
					continue;
				
				// check if RPC
				switch (UnboundRpcService.GetRpcType(method))
				{
					case UnboundRpcService.RpcType.RunOnServer:
						Application.Instance.Harmony.Patch(method, new HarmonyMethod(RosRpcPrefix));
						ServerRpcRegistry[rpcIndex] = (UnboundRpcService.ServerRpcImplementation)Delegate.CreateDelegate(typeof(UnboundRpcService.ServerRpcImplementation), this, GetType().GetMethod(method.Name + "_Impl"));
						break;

					case UnboundRpcService.RpcType.Multicast:
						Application.Instance.Harmony.Patch(method, new HarmonyMethod(MulticastRpcPrefix));
						RpcRegistry[rpcIndex] = (UnboundRpcService.RpcImplementation)Delegate.CreateDelegate(typeof(UnboundRpcService.RpcImplementation), this, GetType().GetMethod(method.Name + "_Impl"));
						break;

					case UnboundRpcService.RpcType.RunOnClient:
						Application.Instance.Harmony.Patch(method, new HarmonyMethod(RunOnClientRpcPrefix));
						RpcRegistry[rpcIndex] = (UnboundRpcService.RpcImplementation)Delegate.CreateDelegate(typeof(UnboundRpcService.RpcImplementation), this, GetType().GetMethod(method.Name + "_Impl"));
						break;

					case UnboundRpcService.RpcType.None:
						continue;
				}

				// add rpc implementation (orig name + "_Impl") 
				RpcIdentifierRegistry[method.Name] = rpcIndex;

				rpcIndex++;
			}
		}

		public Dictionary<string, NetId> RpcIdentifierRegistry = new Dictionary<string, NetId>();
		public Dictionary<NetId, UnboundRpcService.RpcImplementation> RpcRegistry = new Dictionary<NetId, UnboundRpcService.RpcImplementation>();
		public Dictionary<NetId, UnboundRpcService.ServerRpcImplementation> ServerRpcRegistry = new Dictionary<NetId, UnboundRpcService.ServerRpcImplementation>();
		public NetId Id = -1;
		public bool IsLocallyOwned;
		private static Dictionary<NetId, NetEntity> mEntityNetRegistry = new Dictionary<NetId, NetEntity>();
	}
}
