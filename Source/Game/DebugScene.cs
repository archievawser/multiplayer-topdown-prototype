using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rabid
{
	[SceneInfo(name: "Debug")]
	public class DebugScene : BaseScene
	{
		public override void Prepare()
		{
			Batcher = AddEntity(new DynamicBatcherLayer(AssetManager<Effect>.Retreive("Effects/FlatTextured")));
			//mPlayers = AddEntity(new Player());
			mSheet = AddEntity(new Tilesheet(AssetManager<Effect>.Retreive("Effects/GrassTest")));
			Menu = AddEntity<MenuTest>();

			base.Prepare();
		}

		public override void Start()
		{
			for (int i = -11; i < 11; i++) for (int j = -6; j < 6; j++)
				mSheet.Edit(0, new GridCoord(i, j), new GridCoord(4, j == -6 ? 1 : 0));

			World.Instance.Networker.Host();

			RpcTesting.CreatePlayer(IdHelper.GetNextId(), true);

			base.Start();
		}

		public override void Update(float dt)
		{
			Batcher.Effect.Parameters["mvp"].SetValue(Application.Instance.Camera.Transform * Application.Instance.Camera.Projection);
			AssetManager<Effect>.Retreive("Effects/GrassTest").Parameters["mvp"].SetValue(Application.Instance.Camera.Transform * Application.Instance.Camera.Projection);

			base.Update(dt);
		}

		public DynamicBatcherLayer Batcher;
		public MenuTest Menu;
		//private Player mPlayers;
		private Tilesheet mSheet;
	}
}
