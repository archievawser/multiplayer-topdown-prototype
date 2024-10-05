using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rabid
{
	interface IGameEvents
	{
		public void Prepare();
		public void Start();
		public void PreUpdate();
		public void Update(float dt);
		public void PostUpdate();
	}
}
