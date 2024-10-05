using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Rabid
{
	/// <summary>
	/// Extension of BinaryWriter which allows for the serialization of custom value types
	/// </summary>
	public class NetBinaryWriter : BinaryWriter
	{
		public NetBinaryWriter(Stream stream) : base(stream) { }

		public void Write(Vector2 data)
		{
			Write(data.X);
			Write(data.Y);
		}
	}
}
