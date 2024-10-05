using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rabid
{
	public struct BatchElement
	{
		public BatchElement()
		{
			RebuildNeeded = true;
		}

		public const int IndexCount = 6;
		public const int VertexCount = 4;
		public Vector2 Position;
		public Vector2 Size;
		public Vector2 UvOffset;
		public bool RebuildNeeded;
	}
}
