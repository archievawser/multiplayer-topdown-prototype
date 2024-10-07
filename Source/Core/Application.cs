using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rabid
{
	class Application : Game
	{
		public Application()
		{
			if (Instance != null)
				throw new Exception("Multiple Application instances instantiated");

			Trace.WriteLine("Game start");

			Content.RootDirectory = "Assets";
			GraphicsDeviceManager = new GraphicsDeviceManager(this);
			GraphicsDeviceManager.PreferredBackBufferWidth = 1920;
			GraphicsDeviceManager.PreferredBackBufferHeight = 1080;
			IsMouseVisible = true;
			Instance = this;
			Harmony = new Harmony("rabid.game.harmony");

			Harmony.PatchAll();
		}

		protected override void Initialize()
		{
			SteamClient.Init(480);
			SteamNetworkingUtils.InitRelayNetworkAccess();

			UnboundRpcService.PatchRpcs();

			Camera = new Camera(1920, 1080);

			const string atlasName = "Textures/Atlas";
			new Tileset(AssetManager<Texture2D>.Retreive(atlasName));

			GameWorld = new World();
			GameWorld.LoadScene("Debug");

			base.Initialize();
		}

		protected override void Update(GameTime gameTime)
		{
			GraphicsDeviceManager.GraphicsDevice.Clear(Color.Black);

			SteamClient.RunCallbacks();
			GameWorld.PreUpdate();
			GameWorld.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
			GameWorld.PostUpdate();

			base.Update(gameTime);
		}

		public static Application Instance;
		public Harmony Harmony;
		public Camera Camera;
		public GraphicsDeviceManager GraphicsDeviceManager;
		public World GameWorld;
		private Effect mDebugEffect;
		private Tilesheet mSheet;
	}
}
