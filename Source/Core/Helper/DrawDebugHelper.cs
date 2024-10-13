using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rabid
{
	static class DrawDebugHelper
	{
		public static void Setup(SpriteBatch batcher)
		{
			sRasterizer = new RasterizerState();
			sRasterizer.FillMode = FillMode.WireFrame;
			sBatcher = batcher;
		}

		public static void DrawWireQuad(Rectangle rectangle, Color color)
		{
			
		}

		private static RasterizerState sRasterizer;
		private static SpriteBatch sBatcher;
	}
}
