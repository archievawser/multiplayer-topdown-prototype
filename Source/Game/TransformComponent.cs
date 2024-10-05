using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rabid
{
	public class TransformComponent : Component
	{
		public TransformComponent()
		{
		}

		public Vector2 Position
		{
			get
			{
				return new Vector2(LocalModel.Translation.X, LocalModel.Translation.Y);
			}
			set
			{
				mShouldRecalculateWorldModel = true;
				LocalModel.Translation = new Vector3(value, 0);
			}
		}
		public Matrix WorldModel 
		{
			get
			{
				if (Parent != null)
				{
					if(mShouldRecalculateWorldModel)
					{
						_WorldModel = LocalModel * Parent.WorldModel;
					}

					return _WorldModel;
				}

				return LocalModel;
			}
		}
		public TransformComponent Parent;

		private bool mShouldRecalculateWorldModel = true;
		private Matrix _WorldModel;
		private Matrix LocalModel;
	}
}
