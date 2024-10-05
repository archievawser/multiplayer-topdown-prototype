using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rabid
{
	[AttributeUsage(System.AttributeTargets.Method)]
	public class RunOnServer : System.Attribute
	{
		public RunOnServer(bool requiresAuthority = true)
		{
			RequiresAuthority = requiresAuthority;
		}

		public bool RequiresAuthority;
	}
}
