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
	public class NetBinaryReader : BinaryReader
	{
		public NetBinaryReader(Stream stream) : base(stream) { }

		public Vector2 ReadVector2()
		{
			return new Vector2(ReadSingle(), ReadSingle());
		}
	}
}
