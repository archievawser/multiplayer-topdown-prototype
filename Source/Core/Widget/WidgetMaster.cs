using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rabid
{
	class WidgetMaster : Entity
	{
		public override void PostUpdate()
		{
			Child.Render();

			base.PostUpdate();
		}

		private Widget Child;
	}
}
