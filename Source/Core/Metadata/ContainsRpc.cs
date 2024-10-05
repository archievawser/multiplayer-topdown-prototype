using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rabid
{
	[AttributeUsage(System.AttributeTargets.Class)]
	public class ContainsRpc : System.Attribute
	{
		public ContainsRpc()
		{}
	}
}
