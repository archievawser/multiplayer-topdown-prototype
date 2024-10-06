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
				if(type.IsSubclassOf(typeof(BaseScene)))
				{
					if(type.GetCustomAttribute<SceneInfo>() is SceneInfo sceneInfo and not null)
					{
						Scenes[sceneInfo.Name] = type;
					}
				}
				else if(type.GetCustomAttribute<RegisterEntityType>() is RegisterEntityType entityTypeInfo and not null)
				{
					EntityTypeRegistry[entityTypeInfo.Name] = type;
				}
			}

			Networker = new NetDevice();

			Instance = this;
		}

		public void LoadScene(string sceneName)
		{
			CurrentScene = (BaseScene)Activator.CreateInstance(Scenes[sceneName]);
			CurrentScene.Prepare();
			CurrentScene.Start();
		}

		/// <summary>
		/// Allows the instantiation of an Entity using its registered type name (RegisterEntityType)
		/// so that the creation of entities can be coordinated across the network.
		/// </summary>
		/// <param name="typeName"></param>
		/// <returns></returns>
		public Entity Instantiate(string typeName)
		{
			return CurrentScene.AddEntity((Entity)Activator.CreateInstance(EntityTypeRegistry[typeName]));
		}

		public void PreUpdate() => CurrentScene.PreUpdate();

		public void Update(float dt) 
		{
			Networker.Poll();
			CurrentScene.Update(dt);
		}

		public void PostUpdate() => CurrentScene.PostUpdate();

		public static World Instance;
		public static Dictionary<string, Type> Scenes = new Dictionary<string, Type>();
		public static Dictionary<string, Type> EntityTypeRegistry = new Dictionary<string, Type>();
		public NetDevice Networker;
		public BaseScene CurrentScene;
	}
}
