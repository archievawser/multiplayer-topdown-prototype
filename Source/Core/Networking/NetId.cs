using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Rabid
{
	[StructLayout(LayoutKind.Explicit, Size = 1)]
	public struct NetId
	{
		public NetId(byte value)
		{
			Value = value;
		}

		public NetId(int value)
		{
			Value = (byte)value;
		}

		public static implicit operator NetId(byte value) => new NetId(value);
		public static implicit operator NetId(int value) => new NetId(value);

		[FieldOffset(0)]
		public byte Value;
	}
}
