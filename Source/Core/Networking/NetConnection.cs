using Rabid;
using Steamworks;
using Steamworks.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rabid.Netcode.Steam
{
	public class NetConnection
	{
		public NetConnection(Connection conn)
		{
			Base = conn;
		}

		public Connection Base;
		public NetDevice Device;
		public Pawn Possessed;
	}
}
