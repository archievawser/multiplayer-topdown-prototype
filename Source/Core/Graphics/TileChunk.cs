using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;

namespace Rabid
{
	/// <summary>
	/// Stores a sparse set of tiles and their batched geometry
	/// </summary>
	class TileChunk
	{
		public TileChunk(Tilesheet sheet, Effect effect)
		{
			mDevice = Application.Instance.GraphicsDeviceManager.GraphicsDevice;
			mSheet = sheet;
			mEffect = effect;

			mTiles = new List<Tile>();

			Rebuild();
		}

		public void Rebuild()
		{
			mRebuildNeeded = false;

			if (IsEmpty())
			{
				if (mVertexBuffer != null && !mVertexBuffer.IsDisposed)
				{
					mVertexBuffer.Dispose();
					mIndexBuffer.Dispose();
				}

				return;
			}

			VertexPositionTexture[] vertices = new VertexPositionTexture[mTiles.Count * 4];
			UInt32[] indices = new UInt32[mTiles.Count * 6];

			int currentIndex = 0;
			int currentVertexIndex = 0;
			for (int i = 0; i < mTiles.Count; i++)
			{
				Vector3 offset = new Vector3(); // new Vector3(Globals.TileWidth * BatchWidth * BatchColumn, Globals.TileHeight * BatchHeight * BatchRow, 0);
				Vector2 uv = mSheet.Tileset.GetUv(mTiles[i].TilesetCoord);

				vertices[currentVertexIndex + 0] = new VertexPositionTexture(
					offset + Globals.TileWidth * new Vector3(mTiles[i].TilesheetCoord.Column, mTiles[i].TilesheetCoord.Row + 1, 0),
					uv + mSheet.Tileset.TileUv * new Vector2(0, 0)
				);
				vertices[currentVertexIndex + 1] = new VertexPositionTexture(
					offset + Globals.TileWidth * new Vector3(mTiles[i].TilesheetCoord.Column + 1, mTiles[i].TilesheetCoord.Row + 1, 0),
					uv + mSheet.Tileset.TileUv * new Vector2(1, 0)
				);
				vertices[currentVertexIndex + 2] = new VertexPositionTexture(
					offset + Globals.TileWidth * new Vector3(mTiles[i].TilesheetCoord.Column + 1, mTiles[i].TilesheetCoord.Row, 0),
					uv + mSheet.Tileset.TileUv * new Vector2(1, 1)
				);
				vertices[currentVertexIndex + 3] = new VertexPositionTexture(
					offset + Globals.TileWidth * new Vector3(mTiles[i].TilesheetCoord.Column, mTiles[i].TilesheetCoord.Row, 0),
					uv + mSheet.Tileset.TileUv * new Vector2(0, 1)
				);

				indices[currentIndex + 0] = (UInt32)currentVertexIndex + 0;
				indices[currentIndex + 1] = (UInt32)currentVertexIndex + 1;
				indices[currentIndex + 2] = (UInt32)currentVertexIndex + 2;
				indices[currentIndex + 3] = (UInt32)currentVertexIndex + 2;
				indices[currentIndex + 4] = (UInt32)currentVertexIndex + 3;
				indices[currentIndex + 5] = (UInt32)currentVertexIndex + 0;

				currentIndex += 6;
				currentVertexIndex += 4;
			}

			mVertexBuffer = new DynamicVertexBuffer(mDevice, VertexPositionTexture.VertexDeclaration, vertices.Length, BufferUsage.WriteOnly);
			mIndexBuffer = new DynamicIndexBuffer(mDevice, IndexElementSize.ThirtyTwoBits, indices.Length, BufferUsage.WriteOnly);

			mVertexBuffer.SetData(vertices);
			mIndexBuffer.SetData(indices);
		}

		private bool IsEmpty()
		{
			return mTiles.Count == 0;
		}

		public void Draw()
		{
			if (IsEmpty())
				return;

			// TODO: parallelize TileBatch rebuilding
			if (mRebuildNeeded)
				Rebuild();

			mDevice.Indices = mIndexBuffer;
			mDevice.SetVertexBuffer(mVertexBuffer);

			mEffect.CurrentTechnique.Passes[0].Apply();

			mDevice.SamplerStates[0] = SamplerState.PointClamp;
			mDevice.Textures[0] = mSheet.Tileset.Texture;
			mDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, mIndexBuffer.IndexCount / 3);
		}

		public void Edit(GridCoord tilesheetCoord, GridCoord tilesetCoord)
		{
			mRebuildNeeded = true;

			for (int i = 0; i < mTiles.Count; i++)
			{
				if (mTiles[i].TilesheetCoord.Column == tilesheetCoord.Column && mTiles[i].TilesheetCoord.Row == tilesheetCoord.Row)
				{
					mTiles[i] = new Tile(mTiles[i].TilesheetCoord, tilesetCoord);

					return;
				}
			}

			mTiles.Add(new Tile(new GridCoord(tilesheetCoord.Column, tilesheetCoord.Row), tilesetCoord));
		}

		/// <summary>
		/// Writes the following data to <b>stream</b><br></br>
		/// <b>String</b> - effect name<br></br>
		/// <b>int</b> - tile count<br></br>
		/// <b>Tile[tile count]</b> - serialized tiles<br></br>
		/// </summary>
		/// <param name="stream"></param>
		public void SerializeTo(BinaryWriter stream)
		{
			stream.Write(mEffect.Name);
			stream.Write(mTiles.Count);

			foreach (Tile tile in mTiles)
			{
				tile.SerializeTo(stream);
			}
		}

		public static TileChunk FromSerialized(BinaryReader reader, Tilesheet sheet)
		{
			Effect effect = AssetManager<Effect>.Retreive(reader.ReadString());
			TileChunk chunk = new TileChunk(sheet, effect);

			int numTiles = reader.ReadInt32();
			for (int i = 0; i < numTiles; i++)
			{
				chunk.mTiles.Add(Tile.FromSerialized(reader));
			}

			chunk.mRebuildNeeded = true;

			return chunk;
		}

		private GraphicsDevice mDevice;
		private List<Tile> mTiles;
		private Tilesheet mSheet;
		private Effect mEffect;
		private DynamicVertexBuffer mVertexBuffer;
		private DynamicIndexBuffer mIndexBuffer;
		private bool mRebuildNeeded = false;
	}
}
