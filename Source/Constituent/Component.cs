using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rabid
{
	public class Component : IGameEvents
	{
		public Component()
		{
		}

		public virtual void Prepare()
		{

		}

		public virtual void Start()
		{
			
		}

		public virtual void PreUpdate()
		{

		}

		public virtual void Update(float dt)
		{
			
		}
		
		public virtual void PostUpdate()
		{

		}

		public Entity ParentEntity;
	}
}
