using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rabid
{
	[ContainsRpc]
	public static class RpcTesting
	{
		[RunOnClient]
		public static void PossessPlayer(SteamId target, NetId playerId) { }
		public static void PossessPlayer_Impl(NetBinaryReader args)
		{
			NetId id = args.ReadByte();
			CreatePlayer(id, true);
		}

		[RunOnClient]
		public static void PlayerJoined(SteamId target, NetId playerId) { }
		public static void PlayerJoined_Impl(NetBinaryReader args) 
		{
			NetId id = args.ReadByte();
			CreatePlayer(id, false);
		}

		public static void CreatePlayer(NetId id, bool local)
		{
			Player player = new Player();
			player.IsLocallyOwned = local;
			player.SetNetIdentity(id);

			player.ParentScene = World.Instance.CurrentScene;
			player.Prepare();
			player.Start();
			World.Instance.CurrentScene.AddEntity(player);
			
		}
	}
}
