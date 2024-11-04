using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rabid
{
	public class Image : Widget
	{
		public Image(TilesetElement source)
		{
			Source = source;
		}

		public override void Render()
		{
			Master.Batcher.Draw(
				Tileset.Get(0).Texture,
				new Rectangle((int)ComputedPosition.X, (int)ComputedPosition.Y, (int)Size.X, (int)Size.Y),
				new Rectangle(Source.TexCoord.Column * Globals.TileWidth, Source.TexCoord.Row * Globals.TileHeight, Globals.TileWidth * Source.Size.Column, Globals.TileHeight * Source.Size.Column),
				Color.White
			);

			base.Render();
		}

		public TilesetElement Source;
	}
}
