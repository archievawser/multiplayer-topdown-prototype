using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rabid
{
	public struct TilesetElement
	{
		public TilesetElement(string name, GridCoord texCoord, GridCoord size)
		{
			Name = name;
			TexCoord = texCoord;
			Size = size;
		}

		public TilesetElement(GridCoord texCoord, GridCoord size)
		{
			TexCoord = texCoord;
			Size = size;
		}

		public string Name;
		public GridCoord TexCoord;
		public GridCoord Size;
	}
}
