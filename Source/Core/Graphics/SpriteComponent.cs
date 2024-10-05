
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Rabid
{
	[RequiresComponent(typeof(TransformComponent))]
	public class SpriteComponent : Component
	{
		public SpriteComponent(GridCoord texCoord, GridCoord size)
		{
			mTilesetCoord = texCoord;
			mSize = size;
		}

		public SpriteComponent(TilesetElement element)
			: this(element.TexCoord, element.Size)
		{
		}

		public SpriteComponent(GridCoord texCoord)
			: this(texCoord, new GridCoord(1, 1))
		{
		}

		public void SetTexture(TilesetElement element)
		{
			mTilesetCoord = element.TexCoord;
			mSize = element.Size;
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
			mDrawHandle.Set(
				new Vector2(mTransformComponent.Position.X, mTransformComponent.Position.Y),
				new Vector2(mSize.Column, mSize.Row) * new Vector2(Globals.TileWidth, Globals.TileHeight),
				new Vector2(mTilesetCoord.Column, mTilesetCoord.Row)
			);

			base.Update(dt);
		}

		private TransformComponent mTransformComponent;
		private BatchElementHandle mDrawHandle;
		private GridCoord mSize;
		private GridCoord mTilesetCoord;
	}
}
