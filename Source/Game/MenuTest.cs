using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rabid
{
	public class HotbarSlot : Widget
	{
		public HotbarSlot()
		{
			Size = new Vector2(90, 90);
			Padding = new Vector2(10, 10);
		}

		public override void Render()
		{
			Master.Batcher.Draw(
				AssetManager<Texture2D>.Retreive("Textures/White"),
				new Rectangle((int)ComputedPosition.X, (int)ComputedPosition.Y, (int)Size.X, (int)Size.Y),
				Color.White
			);
		}
	}

	public class MenuTest : WidgetMaster
	{
		public MenuTest()
		{
			HorizontalLayout layout = Canvas.AddWidget<HorizontalLayout>();
			
			for (int i = 0; i < 6; i++)
			{
				layout.AddSlot(i, new HotbarSlot());
			}

			layout.Position = new Vector2(Globals.WindowWidth / 2, Globals.WindowHeight);
			layout.Anchor = new Vector2(0.5f, 1f);

			/*Image e = new Image(Tileset.Get(0).Elements["Grass"]);
			e.Size = new Vector2(100, 100);
			Canvas.AddWidget(e);*/
		}
	}
}
