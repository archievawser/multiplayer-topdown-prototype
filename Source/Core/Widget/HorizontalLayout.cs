using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rabid
{
	struct HorizontalLayoutSlot
	{
		/// <summary>
		/// Determines the slot's position in the horizontal layout
		/// </summary>
		public int Key;
		public Widget Element;
	}

	enum HorizontalLayoutSortType
	{
		LeftToRight,
		RightToLeft
	}

	class HorizontalLayout : Panel
	{
		public override void Render()
		{
			if (mRebuildNeeded)
				Rebuild();

			mSlots.ForEach(v => v.Element.Render());

			base.Render();
		}

		/// <summary>
		/// Adds widget to this layout as a HorizontalLayoutSlot
		/// </summary>
		/// <param name="key">The widget's order priority</param>
		/// <param name="element"></param>
		public void AddSlot(int key, Widget element)
		{
			base.AddWidget(element);

			mSlots.Add(new HorizontalLayoutSlot()
			{
				Key = key,
				Element = element
			});

			mRebuildNeeded = true;
		}

		private void Rebuild()
		{
			Sort();

			float xOffset = 0.0f;

			for(int i = 0; i < mSlots.Count; i++)
			{
				mSlots[i].Element.Position = new Vector2(xOffset, 0);
				xOffset += mSlots[i].Element.Size.X;
			}
		}

		private void Sort()
		{
			switch (SortType)
			{ 
				case HorizontalLayoutSortType.LeftToRight:
					{
						mSlots.Sort(LeftToRightComparator);
						break;
					}
				case HorizontalLayoutSortType.RightToLeft:
					{
						mSlots.Sort(LeftToRightComparator);
						break;
					}
			}
		}

		private int LeftToRightComparator(HorizontalLayoutSlot a, HorizontalLayoutSlot b)
		{
			return a.Key - b.Key;
		}

		private int RightToLeftComparator(HorizontalLayoutSlot a, HorizontalLayoutSlot b)
		{
			return b.Key - a.Key;
		}

		public HorizontalLayoutSortType SortType = HorizontalLayoutSortType.LeftToRight;

		private List<HorizontalLayoutSlot> mSlots = new List<HorizontalLayoutSlot>();
		private bool mRebuildNeeded = false;
	}
}
