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
	// TODO: create NetEntity sub class to avoid networking overhead
	public unsafe class Entity : IGameEvents
	{
		public Entity()
		{
			Id = IdHelper.GetNextId();
			mEntityNetRegistry.Add(Id, this);
		}

		public static Entity Resolve(NetId id)
		{
			return mEntityNetRegistry[id];
		}

		public virtual void Prepare()
		{
			foreach(Component component in mComponents.Values)
			{
#if DEBUG
				foreach(RequiresComponent attrib in component.GetType().GetCustomAttributes<RequiresComponent>())
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
		public static void RosRpcPrefix(Entity __instance, dynamic[] __args)
		{
			MemoryStream stream = new MemoryStream();
			BinaryWriter writer = new BinaryWriter(stream);
			writer.Write(__instance.Id.Value);
			writer.Write(1);
			foreach(var v in __args)
			{
				writer.Write((dynamic)v);
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
		public static void RocRpcPrefix(Entity __instance, dynamic[] __args)
		{
			MemoryStream stream = new MemoryStream();
			NetBinaryWriter writer = new NetBinaryWriter(stream);
			writer.Write(__instance.Id.Value);
			writer.Write(1);
			foreach(var v in __args)
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
					throw new Exception("RPCs cannot be generic");

				// check if RPC
				if (method.GetCustomAttribute<RunOnServer>() != null)
				{
					SetupRosRpc(method, rpcIndex);
				}
				else if (method.GetCustomAttribute<RunOnClient>() != null)
				{
					SetupRosRpc(method, rpcIndex);
				} else
				{
					continue;
				}

				RpcRegistry[rpcIndex] = GetType().GetMethod(method.Name + "_Impl");

				rpcIndex++;
			}
		}

		private void SetupRosRpc(MethodInfo method, int rpcId)
		{
			Application.Instance.Harmony.Patch(method, new HarmonyMethod(RosRpcPrefix));
		}

		private void SetupRocRpc(MethodInfo method)
		{
			Application.Instance.Harmony.Patch(method, new HarmonyMethod(RocRpcPrefix));
		}

		public virtual void Start()
		{
			foreach (Component component in mComponents.Values)
			{
				component.Start();
			}
		}		

		public virtual void PreUpdate()
		{
			foreach (Component component in mComponents.Values)
			{
				component.PreUpdate();
			}
		}

		public virtual void Update(float dt)
		{
			foreach(Component component in mComponents.Values)
			{
				component.Update(dt);
			}
		}
		
		public virtual void PostUpdate()
		{
			foreach (Component component in mComponents.Values)
			{
				component.PostUpdate();
			}
		}

		public T AddComponent<T>(T component) where T : Component
		{
			component.ParentEntity = this;
			mComponents.Add(typeof(T), component);
			return component;
		}

		public T GetComponent<T>() where T : Component
		{
			return (T)mComponents[typeof(T)];
		}

		public BaseScene ParentScene;
		public Dictionary<NetId, RpcImplementation> RpcRegistry = new Dictionary<NetId, RpcImplementation>();
		public NetId Id = IdHelper.GetNextId();
		public delegate void RpcImplementation(NetBinaryReader data);
		public bool IsLocallyOwned;
		private static Dictionary<NetId, Entity> mEntityNetRegistry = new Dictionary<NetId, Entity>();
		private Dictionary<Type, Component> mComponents = new Dictionary<Type, Component>();
	}
}
