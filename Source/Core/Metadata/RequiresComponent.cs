using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rabid
{
	/// <summary>
	/// Validates the assumption that a component's entity contains another component type
	/// </summary>
	[System.AttributeUsage(System.AttributeTargets.Class)]
	public class RequiresComponent : System.Attribute
	{
		public RequiresComponent(Type type)
		{
			ComponentType = type;
		}

		public Type ComponentType;
	}
}
