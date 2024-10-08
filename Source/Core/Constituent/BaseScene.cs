using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rabid
{
	public abstract class BaseScene : IGameEvents
	{
		public BaseScene()
		{

		}
		
		public virtual void Prepare()
		{
			DrawManager = AddEntity(new DrawManager());

			foreach (Entity entity in mEntities)
			{
				entity.Prepare();
			}
		}

		public virtual void Start()
		{
			foreach(Entity entity in mEntities)
			{
				entity.Start();
			}
		}

		public virtual void PreUpdate()
		{
			foreach (Entity entity in mEntities)
			{
				entity.PreUpdate();
			}
		}

		public virtual void Update(float dt)
		{
			for(int i = 0; i < mEntities.Count; i++)
			{
				mEntities[i].Update(dt);
			}
		}
		
		public virtual void PostUpdate()
		{
			foreach (Entity entity in mEntities)
			{
				entity.PostUpdate();
			}
		}

		public T AddEntity<T>(T entity) where T : Entity
		{
			entity.ParentScene = this;
			mEntities.Add(entity);
			return entity;
		}

		public void RemoveEntity(Entity entity)
		{
			mEntities.Remove(entity);
		}

		public DrawManager DrawManager;
		public string Name;
		private List<Entity> mEntities = new List<Entity>();
	}
}
