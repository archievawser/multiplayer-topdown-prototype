using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rabid
{
	[AttributeUsage(System.AttributeTargets.Method)]
	public class Multicast : System.Attribute
	{
		public Multicast()
		{

		}
	}
}
