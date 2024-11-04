using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Rabid
{
	public class Widget
	{
		public virtual void Render()
		{ }

		public void SetParent(Widget parent)
		{
			Parent = parent;
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

			return (point.X > ComputedPosition.X - halfSize.X && point.X < ComputedPosition.X + halfSize.X)
				&& (point.Y > ComputedPosition.Y - halfSize.Y && point.Y < ComputedPosition.Y + halfSize.Y);
		}

		public Vector2 Position
		{
			get => _Position;
			set
			{
				_Position = value;

				Vector2 parentOffset = new Vector2(0f);
				
				if(Parent != null)
				{
					parentOffset = Parent.ComputedPosition;
				}

				ComputedPosition = value + parentOffset - Size * Anchor;
			}
		}
		private Vector2 _Position;

		public Vector2 ComputedPosition;

		/// <summary>
		/// Determines the origin of the widget's transformations
		/// 0, 0 = top left
		/// 1, 1 = bottom right
		/// </summary>
		public Vector2 Anchor
		{
			get => _Anchor;
			set
			{
				_Anchor = value;
				Position = _Position;
			}
		}
		public Vector2 _Anchor;

		/// <summary>
		/// Size, in game units, of the widget's bounding box
		/// </summary>
		public Vector2 Size
		{
			get => _Size;
			set
			{
				_Size = value;
				Anchor = _Anchor;
			}
		}
		private Vector2 _Size;

		/// <summary>
		/// Pads this widget if controlled by a layout like HorizontalLayout which considers padding
		/// </summary>
		public Vector2 Padding;

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

		public Widget Parent;
		public WidgetMaster Master;
	}
}
