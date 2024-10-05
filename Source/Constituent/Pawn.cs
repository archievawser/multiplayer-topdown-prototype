using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rabid
{
	public class Pawn : Entity
	{
		public Pawn() 
		{
			Input = AddComponent(new InputComponent());
		}

		public InputComponent Input;
	}
}
