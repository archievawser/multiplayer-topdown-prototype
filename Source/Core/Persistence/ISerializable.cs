using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rabid
{
	public interface ISerializable<T>
	{
		public void WriteSerialized(BinaryWriter writer);
		public abstract static T FromSerialized(BinaryReader reader);
	}
}
