using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rabid
{
	public static class IdHelper
	{
		public static int GetNextId() => sNextId++;

		private static int sNextId = 0;
	}
}
