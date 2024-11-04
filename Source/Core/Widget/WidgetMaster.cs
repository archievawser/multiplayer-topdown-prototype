using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rabid
{
	public class WidgetMaster : Entity
	{
		public WidgetMaster()
		{
			Canvas = new Panel();
			Batcher = Application.Instance.Batcher;

			Canvas.SetMaster(this);
		}

		public override void PostUpdate()
		{
			Canvas.Render();

			base.PostUpdate();
		}

		public SpriteBatch Batcher;
		public Panel Canvas;
	}
}
