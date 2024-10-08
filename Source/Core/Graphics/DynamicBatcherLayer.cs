using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rabid
{
	/// <summary>
	/// The dynamic batcher builds an array of BatchElement into a vertex and index buffer.
	/// The vertex and index buffer will only be rebuilt if a BatchElement has been added, modified or removed.
	/// </summary>
	public class DynamicBatcherLayer : Entity, IDrawable
	{
		public DynamicBatcherLayer(Effect e)
		{
			Effect = e;
			ZIndex = -10;
		}

		public override void Prepare()
		{
			mTileset = Tileset.Get(0);
			ParentScene.DrawManager.Register(this);
		}

		private void Rebuild()
		{
			EnsureBufferSizes();

			bool dataChanged = false;

			int index = 0;

			if(mFullRebuildNeeded)
			{
				dataChanged = true;

				foreach (int key in mElements.Keys)
				{
					WriteQuadToUser(index++, new Vector3(mElements[key].Position, 0), mElements[key].Size, mElements[key].UvOffset);
				}
			}
			else
			{
				foreach (int key in mElements.Keys)
				{
					if (mElements[key].RebuildNeeded)
					{
						dataChanged = true;
						mElements[key] = mElements[key] with { RebuildNeeded = false };
						WriteQuadToUser(index++, new Vector3(mElements[key].Position, 0), mElements[key].Size, mElements[key].UvOffset);
					}
				}
			}

			if(dataChanged || mFullRebuildNeeded)
			{
				WriteToBuffers();
			}

			mFullRebuildNeeded = false;
		}

		public void WriteToBuffers()
		{
			mVertexBuffer.SetData(mVertices);
			mIndexBuffer.SetData(mIndices);
		}

		public BatchElementHandle Add(BatchElement element)
		{
			int id = IdHelper.GetNextId();
			mElements[id] = element;
			return new BatchElementHandle(id, this);
		}

		public void Set(int key, BatchElement element)
		{
			mElements[key] = element;
		}

		public void Remove(BatchElementHandle handle)
		{
			handle.Valid = false;
			mElements.Remove(handle.Key);
		}

		// TODO: use SIMD to generate quads || GPU accelerate
		private void WriteQuadToUser(int elementIndex, Vector3 positionOffset, Vector2 size, Vector2 uvOffset)
		{
			int vertexOffset = BatchElement.VertexCount * elementIndex;
			int indexOffset = BatchElement.IndexCount * elementIndex;

			Vector3 positionMin = positionOffset - new Vector3(size / 2.0f, 0);
			Vector3 positionMax = positionOffset + new Vector3(size / 2.0f, 0);
			Vector2 uvMin = uvOffset * mTileset.TileUv;
			Vector2 uvMax = uvMin + (size / new Vector2(Globals.TileWidth, Globals.TileHeight)) * mTileset.TileUv;

			mVertices[vertexOffset + 0] = new VertexPositionTexture(new Vector3(positionMin.X, positionMax.Y, 0), new Vector2(uvMin.X, uvMin.Y));
			mVertices[vertexOffset + 1] = new VertexPositionTexture(new Vector3(positionMax.X, positionMax.Y, 0), new Vector2(uvMax.X, uvMin.Y));
			mVertices[vertexOffset + 2] = new VertexPositionTexture(new Vector3(positionMax.X, positionMin.Y, 0), new Vector2(uvMax.X, uvMax.Y));
			mVertices[vertexOffset + 3] = new VertexPositionTexture(new Vector3(positionMin.X, positionMin.Y, 0), new Vector2(uvMin.X, uvMax.Y));

			mIndices[indexOffset] = (uint)vertexOffset;
			mIndices[indexOffset + 1] = ((uint)vertexOffset + 1);
			mIndices[indexOffset + 2] = ((uint)vertexOffset + 2);
			mIndices[indexOffset + 3] = ((uint)vertexOffset + 2);
			mIndices[indexOffset + 4] = ((uint)vertexOffset + 3);
			mIndices[indexOffset + 5] = ((uint)vertexOffset);

			if (elementIndex == 1)
				elementIndex = 1;
		}

		private void EnsureBufferSizes()
		{
			int requiredVertices = BatchElement.VertexCount * mElements.Count;
			int requiredIndices = BatchElement.IndexCount * mElements.Count;

			if(requiredVertices > mVertices.Length)
			{
				Array.Resize(ref mVertices, requiredVertices);
			}

			if(requiredIndices > mIndices.Length)
			{
				Array.Resize(ref mIndices, requiredIndices);
			}

			if(requiredVertices != mVertexBuffer?.VertexCount)
			{
				mFullRebuildNeeded = true;
				mVertexBuffer = new DynamicVertexBuffer(Application.Instance.GraphicsDeviceManager.GraphicsDevice, VertexPositionTexture.VertexDeclaration, requiredVertices, BufferUsage.WriteOnly);
			}

			if(requiredIndices != mIndexBuffer?.IndexCount)
			{
				mFullRebuildNeeded = true;
				mIndexBuffer = new DynamicIndexBuffer(Application.Instance.GraphicsDeviceManager.GraphicsDevice, IndexElementSize.ThirtyTwoBits, requiredIndices, BufferUsage.WriteOnly);
			}
		}

		public void Draw()
		{
			if (mElements.Count == 0)
				return;

			Rebuild();

			GraphicsDevice device = Application.Instance.GraphicsDeviceManager.GraphicsDevice;
			
			device.Indices = mIndexBuffer;
			device.SetVertexBuffer(mVertexBuffer);

			Effect.CurrentTechnique.Passes[0].Apply();

			device.SamplerStates[0] = SamplerState.PointClamp;
			device.Textures[0] = mTileset.Texture;
			device.BlendState = BlendState.AlphaBlend;
			device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, mIndexBuffer.IndexCount / 3);
		}

		private VertexPositionTexture[] mVertices = new VertexPositionTexture[0];
		private DynamicVertexBuffer mVertexBuffer;
		private DynamicIndexBuffer mIndexBuffer;
		public Effect Effect;
		private Dictionary<int, BatchElement> mElements = new Dictionary<int, BatchElement>();
		private Tileset mTileset;
		private uint[] mIndices = new uint[0];
		private bool mFullRebuildNeeded = true;

		public int ZIndex { get; set; }
	}
}
