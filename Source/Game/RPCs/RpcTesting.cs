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
		[Multicast]
		public static void PossessPlayer(NetId playerId) { }
		public static void PossessPlayer_Impl(NetBinaryReader args)
		{
			NetId id = args.ReadByte();
			Player player = new Player();
			player.SetNetIdentity(id);
		}
	}
}
