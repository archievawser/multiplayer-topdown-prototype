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
	class Panel : Widget
	{
		public override void Render()
		{
			mWidgets.ForEach(x => x.Render());

			base.Render();
		}

		protected T AddWidget<T>() where T : Widget, new()
		{
			T result = new T();
			mWidgets.Add(result);
			return result;
		}

		protected T AddWidget<T>(T widget) where T : Widget
		{
			mWidgets.Add(widget);
			return widget;
		}

		protected void RemoveWidget<T>(T widget) where T : Widget
		{
			mWidgets.Remove(widget);
		}

		protected List<Widget> mWidgets
		{ get; set; }
	}
}
