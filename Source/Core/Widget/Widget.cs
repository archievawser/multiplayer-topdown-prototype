using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Rabid
{
	class Widget
	{
		public virtual void Render()
		{

		}

		public void SetMaster(WidgetMaster master)
		{
			Master = master;
		}

		/// <summary>
		/// Checks if a point is contained within this widget's bounding box
		/// </summary>
		public virtual bool Contains(Vector2 point)
		{
			Vector2 halfSize = Size / 2.0f;

			return (point.X > Position.X - halfSize.X && point.X < Position.X + halfSize.X)
				&& (point.Y > Position.Y - halfSize.Y && point.Y < Position.Y + halfSize.Y);
		}

		public Vector2 Position
		{
			get => _Position;
			set
			{
				_Position = value - Anchor * Size;
			}
		}
		private Vector2 _Position;

		/// <summary>
		/// Determines the origin of the widget's transformations
		/// 0, 0 = top left
		/// 1, 1 = bottom right
		/// </summary>
		public Vector2 Anchor;

		/// <summary>
		/// Size, in game units, of the widget's bounding box
		/// </summary>
		public Vector2 Size;

		/// <summary>
		/// Indicates to layout classes like HorizontalLayout whether this widget wants to
		/// take up as much width as possible
		/// </summary>
		public bool WidthGreedy = false;

		/// <summary>
		/// Indicates to layout classes like HorizontalLayout whether  this widget wants to
		/// take up as much height as possible
		/// </summary>
		public bool HeightGreedy = false;


		public WidgetMaster Master;
	}
}
