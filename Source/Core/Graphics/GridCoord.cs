using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


namespace Rabid
{

	[Serializable]
	[StructLayout(LayoutKind.Explicit)]
	public struct GridCoord
	{
		public GridCoord(int column, int row)
		{
			Column = column;
			Row = row;
		}
		public void SerializeTo(BinaryWriter stream)
		{
			stream.Write(Column);
			stream.Write(Row);
		}

		public static GridCoord FromSerialized(BinaryReader stream)
		{
			int col = stream.ReadInt32();
			int row = stream.ReadInt32();
			return new GridCoord(col, row);
		}

		[FieldOffset(0)]
		public int Column;

		[FieldOffset(4)]
		public int Row;
	}
}
