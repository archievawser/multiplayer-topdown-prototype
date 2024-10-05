using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rabid
{
	public interface IDrawable
	{
		public void Draw();
		public int ZIndex { get; set;  }
	}
}
