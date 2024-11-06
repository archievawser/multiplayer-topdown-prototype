using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Rabid
{
	class Tilesheet : Entity, IDrawable
	{
		public Tilesheet(Effect effect)
		{
			Chunks = new SortedDictionary<int, Dictionary<int, Dictionary<int, TileChunk>>>();
			Tileset = Tileset.Get(0);
			mDevice = Application.Instance.GraphicsDeviceManager.GraphicsDevice;
			Effect = effect;
		}

		public override void Prepare()
		{
			ParentScene.DrawManager.Register(this);

			base.Prepare();
		}

		/// <summary>
		/// Draws the chunks of the sheet intersecting the camera's frustum
		/// </summary>
		public void Draw()
		{
			Rectangle frustum = Camera.Current.GetFrustum();

			// get chunk size in game units
			Vector2 chunkSize = new Vector2(ChunkSize * Globals.TileWidth, ChunkSize * Globals.TileHeight);

			// get top left & bottom right batch coordinate 
			Vector2 topLeft = new Vector2(frustum.Left, frustum.Top) / chunkSize;
			Vector2 bottomRight = new Vector2(frustum.Right, frustum.Bottom) / chunkSize;

			foreach (int layer in Chunks.Keys)
			{
				for(int x = (int)Math.Floor(topLeft.X); x < (int)Math.Ceiling(bottomRight.X); x++)
				{
					for(int y = (int)Math.Floor(topLeft.Y); y < (int)Math.Ceiling(bottomRight.Y); y++)
					{
						GetChunk(layer, x, y).Draw();
					}
				}
			}
		}

		/// <summary>
		/// Writes the following data to <b>stream</b><br></br>
		/// <b>int</b> - chunk size<br></br>
		/// <b>int</b> - chunk count<br></br>
		/// <b>String</b> - effect name<br></br>
		/// <b>Tileset</b> - tileset<br></br>
		/// <b>struct { int layer; TileChunk chunk; }[chunk count]</b> - serialized chunks<br></br>
		/// </summary>
		/// <param name="stream"></param>
		public void SerializeTo(BinaryWriter writer)
		{
			writer.Write(ChunkSize);
			writer.Write(ChunkCount);
			writer.Write(Effect.Name);

			foreach (var layer in Chunks.Keys)
			{
				foreach(var col in Chunks[layer].Keys)
				{
					foreach(var row in Chunks[layer][col].Keys)
					{
						writer.Write(layer);
						writer.Write(col);
						writer.Write(row);
						Chunks[layer][col][row].SerializeTo(writer);
					}
				}
			}
		}

		public static Tilesheet FromSerialized(BinaryReader reader)
		{
			int chunkSize = reader.ReadInt32();
			int chunkCount = reader.ReadInt32();
			Effect effect = AssetManager<Effect>.Retreive(reader.ReadString());

			Tileset tileset = Tileset.Get(0);
			Tilesheet sheet = new Tilesheet(effect);

			sheet.ChunkCount = chunkCount;
			sheet.ChunkSize = chunkSize;

			for(int i = 0; i < chunkCount; i++)
			{
				int layer = reader.ReadInt32();
				int col = reader.ReadInt32();
				int row = reader.ReadInt32();

				sheet.GetChunk(layer, col, row);
				sheet.Chunks[layer][col][row] = TileChunk.FromSerialized(reader, sheet);
			}

			return sheet;
		}

		/// <summary>
		/// Gets a chunk at the specified coordinates<br></br>
		/// If one doesn't already exist, one is created via CreateChunk
		/// </summary>
		/// <param name="layer"></param>
		/// <param name="column"></param>
		/// <param name="row"></param>
		/// <returns></returns>
		public TileChunk GetChunk(int layer, int column, int row)
		{
			if (!Chunks.ContainsKey(layer))
			{
				Chunks[layer] = new Dictionary<int, Dictionary<int, TileChunk>>();
				Chunks[layer][column] = new Dictionary<int, TileChunk>();

				return CreateChunk(layer, column, row);
			}
			else if (!Chunks[layer].ContainsKey(column))
			{
				Chunks[layer][column] = new Dictionary<int, TileChunk>();

				return CreateChunk(layer, column, row);
			} 
			else if (!Chunks[layer][column].ContainsKey(row))
			{
				return CreateChunk(layer, column, row);
			}

			return Chunks[layer][column][row];
		}

		/// <summary>
		/// Creates a chunk at the specified coordinates<br></br>
		/// Requires Chunks[layer][column] to be non-null
		/// </summary>
		/// <param name="layer"></param>
		/// <param name="column"></param>
		/// <param name="row"></param>
		/// <returns></returns>
		public TileChunk CreateChunk(int layer, int column, int row)
		{
			ChunkCount++;

			return Chunks[layer][column][row] = new TileChunk(this, Effect);
		}

		public void Edit(int layer, GridCoord tilesheetCoord, GridCoord tilesetCoord)
		{
			int chunkX = tilesheetCoord.Column / ChunkSize;
			int chunkY = tilesheetCoord.Row / ChunkSize;

			GetChunk(layer, chunkX, chunkY).Edit(tilesheetCoord, tilesetCoord);
		}

		public Effect Effect;
		public Tileset Tileset;
		public SortedDictionary<int, Dictionary<int, Dictionary<int, TileChunk>>> Chunks;
		public int ChunkSize = 24;
		public int ChunkCount = 0;
		public int ZIndex { get; set; }
		private GraphicsDevice mDevice;
		private bool mEmpty;
	}
}
