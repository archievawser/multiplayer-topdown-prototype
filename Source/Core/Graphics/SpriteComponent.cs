
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Steamworks.Data;

namespace Rabid
{
	[RequiresComponent(typeof(TransformComponent))]
	public class SpriteComponent : Component
	{
		public SpriteComponent(GridCoord texCoord, GridCoord size)
		{
			mElement.TexCoord = texCoord;
			mElement.Size = size;
		}

		public SpriteComponent(TilesetElement element)
			: this(element.TexCoord, element.Size)
		{
		}

		public SpriteComponent(GridCoord texCoord)
			: this(texCoord, new GridCoord(1, 1))
		{
		}

		public void SetTexture(string name)
		{
			mCurrentAnimName = "none";
			mCurrentAnimFrame = 0;

			mElement = Tileset.Get(0).Elements[name];
		}

		public void SetAnimation(string name, bool restart = true)
		{
			if(name == mCurrentAnimName)
				return;

			mCurrentAnimName = name;

			if(restart)
				mTimeSinceLastKeyframe = 0;

			mElement = Tileset.Get(0).Elements[name];
		}

		public override void Prepare()
		{
			mDrawHandle = ((DebugScene)ParentEntity.ParentScene).Batcher.Add(new BatchElement());

			base.Prepare();
		}

		public override void Start()
		{
			mTransformComponent = ParentEntity.GetComponent<TransformComponent>();
			
			base.Start();
		}

		public override void Update(float dt)
		{
			if(mElement.FrameCount > 1)
			{
				const double frameLength = 1f / 12f;
				mTimeSinceLastKeyframe += dt;
			
				if(mTimeSinceLastKeyframe > frameLength)
				{
					mTimeSinceLastKeyframe = 0;
					mCurrentAnimFrame++;

					if(mCurrentAnimFrame >= mElement.FrameCount)
					{
						mCurrentAnimFrame = 0;
					}
				}
			}

			mDrawHandle.Set(
				new Vector2(mTransformComponent.Position.X, mTransformComponent.Position.Y),
				new Vector2(mElement.Size.Column, mElement.Size.Row) * new Vector2(Globals.TileWidth, Globals.TileHeight),
				new Vector2(mElement.TexCoord.Column + mCurrentAnimFrame, mElement.TexCoord.Row)
			);

			base.Update(dt);
		}

		private string mCurrentAnimName;
		private int mCurrentAnimFrame = 0;
		private double mTimeSinceLastKeyframe;
		private TransformComponent mTransformComponent;
		private BatchElementHandle mDrawHandle;
		private TilesetElement mElement;
	}
}
