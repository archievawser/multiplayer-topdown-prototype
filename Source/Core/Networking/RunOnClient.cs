using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rabid
{
	[AttributeUsage(System.AttributeTargets.Method)]
	public class RunOnClient : System.Attribute
	{
		public RunOnClient()
		{

		}
	}
}
