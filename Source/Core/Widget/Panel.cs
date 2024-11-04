using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rabid
{
	/// <summary>
	/// A panel handles the containing of any number of widgets without interfering with their behaviour
	///	<br></br>
	/// Allows layout classes to contain a runtime-variable number of elements
	/// </summary>
	public class Panel : Widget
	{
		public override void Render()
		{
			mWidgets.ForEach(x => x.Render());

			base.Render();
		}

		public T AddWidget<T>() where T : Widget, new()
		{
			T result = new T();
			result.SetMaster(Master);
			mWidgets.Add(result);
			return result;
		}

		public T AddWidget<T>(T widget) where T : Widget
		{
			widget.SetMaster(Master);
			widget.SetParent(this);
			mWidgets.Add(widget);
			return widget;
		}

		public void RemoveWidget<T>(T widget) where T : Widget
		{
			mWidgets.Remove(widget);
		}

		protected List<Widget> mWidgets = new List<Widget>();
	}
}
