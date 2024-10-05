using Rabid.Netcode.Steam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Rabid
{
	class World
	{
		public World()
		{
			if (Instance != null)
				throw new Exception("Multiple instances of singleton World constructed");

			Assembly assembly = GetType().Assembly;

			foreach(Type type in assembly.GetTypes())
			{
				if(type.BaseType == typeof(BaseScene))
				{
					if(type.GetCustomAttribute<SceneInfo>() is SceneInfo sceneInfo and not null)
					{
						Scenes[sceneInfo.Name] = type;
					}
				}
			}

			Networker = new NetDevice();
			Networker.Connect();

			Instance = this;
		}

		public void LoadScene(string sceneName)
		{
			CurrentScene = (BaseScene)Activator.CreateInstance(Scenes[sceneName]);
			CurrentScene.Prepare();
			CurrentScene.Start();
		}

		public void PreUpdate() => CurrentScene.PreUpdate();

		public void Update(float dt) => CurrentScene.Update(dt);

		public void PostUpdate() => CurrentScene.PostUpdate();

		public static World Instance;
		public static Dictionary<string, Type> Scenes = new Dictionary<string, Type>();
		public NetDevice Networker;
		public BaseScene CurrentScene;
	}
}
