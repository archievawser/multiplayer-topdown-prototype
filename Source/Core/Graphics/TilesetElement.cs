using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rabid
{
	public struct TilesetElement
	{
		public TilesetElement(string name, GridCoord texCoord, GridCoord size, int frameCount = 1)
		{
			FrameCount = frameCount;
			Name = name;
			TexCoord = texCoord;
			Size = size;
		}

		public TilesetElement(GridCoord texCoord, GridCoord size, int frameCount = 1)
		{
			FrameCount = frameCount;
			TexCoord = texCoord;
			Size = size;
		}

		public string Name;
		public int FrameCount;
		public GridCoord TexCoord;
		public GridCoord Size;
	}
}
