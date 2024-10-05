using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rabid
{
	public class BatchElementHandle
	{
		public BatchElementHandle(int key, DynamicBatcherLayer layer)
		{
			Key = key;
			Layer = layer;
		}

		~BatchElementHandle()
		{
			Layer.Remove(this);
		}

		public void Set(Vector2 position, Vector2 size, Vector2 uvOffset)
		{
			Layer.Set(Key, new BatchElement
			{
				Position = position,
				Size = size,
				UvOffset = uvOffset
			});
		}

		public bool Valid;
		public int Key;
		DynamicBatcherLayer Layer;
	}
}
