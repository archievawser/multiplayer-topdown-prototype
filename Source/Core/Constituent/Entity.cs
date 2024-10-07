using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Rabid
{
	public unsafe class Entity : IGameEvents
	{
		public Entity()
		{}

		public virtual void Prepare()
		{
			foreach(Component component in mComponents.Values)
			{
				component.Prepare();
			}
		}
		public virtual void Start()
		{
			foreach (Component component in mComponents.Values)
			{
				component.Start();
			}
		}		

		public virtual void PreUpdate()
		{
			foreach (Component component in mComponents.Values)
			{
				component.PreUpdate();
			}
		}

		public virtual void Update(float dt)
		{
			foreach(Component component in mComponents.Values)
			{
				component.Update(dt);
			}
		}
		
		public virtual void PostUpdate()
		{
			foreach (Component component in mComponents.Values)
			{
				component.PostUpdate();
			}
		}

		public T AddComponent<T>(T component) where T : Component
		{
			component.ParentEntity = this;
			mComponents.Add(typeof(T), component);
			return component;
		}

		public T GetComponent<T>() where T : Component
		{
			return (T)mComponents[typeof(T)];
		}

		public BaseScene ParentScene;
		protected Dictionary<Type, Component> mComponents = new Dictionary<Type, Component>();
	}
}
