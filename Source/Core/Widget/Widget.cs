using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rabid
{
	class Widget
	{
		public virtual void Render()
		{

		}

		public virtual bool Contains(Vector2 point)
		{
			Vector2 halfSize = Size / 2.0f;

			return (point.X > Position.X - halfSize.X && point.X < Position.X + halfSize.X)
				&& (point.Y > Position.Y - halfSize.Y && point.Y < Position.Y + halfSize.Y);
		}

		public Vector2 Position;
		public Vector2 Size;
		protected SpriteBatch Batcher;
	}
}
