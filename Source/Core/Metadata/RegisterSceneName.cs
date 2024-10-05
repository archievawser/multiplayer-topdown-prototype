using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rabid
{
	[AttributeUsage(System.AttributeTargets.Class)]
	public class SceneInfo : System.Attribute
	{
		public SceneInfo(string name)
		{
			Name = name;
		}

		public string Name;
	}
}
