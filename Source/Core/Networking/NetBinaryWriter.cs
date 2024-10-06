using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
			base.Write(data.X);
			base.Write(data.Y);
		}

		public void Write(NetMessageType data) => Write((byte)data);

		public void Write(NetId data) => base.Write(data.Value);
	}
}
