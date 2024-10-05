using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Rabid
{
	[Serializable]
	[StructLayout(LayoutKind.Explicit)]
	struct Tile
	{
		public Tile(int tilesheetRow, int tilesheetColumn, int tilesetRow, int tilesetColumn)
		{
			TilesheetCoord = new GridCoord(tilesheetColumn, tilesheetRow);
			TilesetCoord = new GridCoord(tilesetColumn, tilesetRow);
		}

		public Tile(GridCoord tilesheetCoord, GridCoord tilesetCoord)
		{
			TilesheetCoord = tilesheetCoord;
			TilesetCoord = tilesetCoord;
		}

		public void SerializeTo(BinaryWriter stream)
		{
			TilesheetCoord.SerializeTo(stream);
			TilesetCoord.SerializeTo(stream);
		}

		public static Tile FromSerialized(BinaryReader reader)
		{
			GridCoord tilesheetCoord = GridCoord.FromSerialized(reader);
			GridCoord tilesetCoord = GridCoord.FromSerialized(reader);
			return new Tile(tilesheetCoord, tilesetCoord);
		}

		[FieldOffset(0)]
		public GridCoord TilesheetCoord;

		[FieldOffset(8)]
		public GridCoord TilesetCoord;
	}
}
