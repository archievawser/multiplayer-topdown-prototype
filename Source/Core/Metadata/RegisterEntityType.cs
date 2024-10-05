using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rabid
{
	[AttributeUsage(System.AttributeTargets.Class)]
	public class RegisterEntityType : System.Attribute
	{
		public RegisterEntityType(string name)
		{
			Name = name;
		}

		public string Name;
	}
}
