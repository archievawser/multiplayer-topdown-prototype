using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rabid
{
	public class DrawManager : Entity
	{
		public DrawManager()
		{
		}

		public override void Prepare()
		{

			base.Prepare();
		}

		public void Register(IDrawable drawable)
		{
			mDrawQueue.Add(drawable);
			mOrdered = false;
		}

		public int DrawOrderSorter(IDrawable a, IDrawable b)
		{
			return b.ZIndex - a.ZIndex;
		}

		public override void PostUpdate()
		{
			if (!mOrdered)
			{
				mDrawQueue.Sort(DrawOrderSorter);
			}

			foreach (IDrawable drawable in mDrawQueue)
			{
				drawable.Draw();
			}
		}

		private bool mOrdered = false;
		private List<IDrawable> mDrawQueue = new List<IDrawable>();
	}
}
