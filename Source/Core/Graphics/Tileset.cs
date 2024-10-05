using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Rabid
{
	public class Tileset
	{
		public Tileset(Texture2D atlas)
		{
			if (!IsValid(atlas))
				throw new ArgumentException("Tileset received invalid texture atlas");

			Columns = (uint)atlas.Width / (uint)Globals.TileWidth;
			Rows = (uint)atlas.Height / Globals.TileHeight;

			TileUv = new Vector2(1f / Columns, 1f / Rows);
			Texture = atlas;

			TryLoadMetadata();

			Tilesets.Add(this);
		}

		public static Tileset Get(int index) => Tilesets[index];

		/// <summary>
		/// Convert the coordinates of a sprite on the tileset to UV coordinates
		/// </summary>
		/// <param name="minimum">The top left sprite coordinate</param>
		/// <param name="maximum">The top right sprite coordinate</param>
		/// <returns>First and second tuple elements are the minimum and maximum UV coordinates respectively</returns>
		public Tuple<Vector2, Vector2> GetUv(GridCoord minimum, GridCoord maximum)
		{
			return new Tuple<Vector2, Vector2> (
				new Vector2(minimum.Column * TileUv.X, minimum.Row * TileUv.Y),
				new Vector2(maximum.Column * TileUv.X, maximum.Row * TileUv.Y)
			);
		}

		/// <summary>
		/// Convert the coordinate of a sprite on the tileset to UV coordinates
		/// </summary>
		/// <param name="coord"></param>
		/// <returns></returns>
		public Vector2 GetUv(GridCoord coord)
		{
			return new Vector2(coord.Column, coord.Row) * TileUv;
		}

		public void TryLoadMetadata()
		{
			string metaName = Texture.Name + ".meta.xml";

			if (NativeAssetManager.Exists(metaName))
			{
				XmlDocument doc = new XmlDocument();
				doc.Load(NativeAssetManager.Load(metaName));

				foreach(XmlNode node in doc.SelectNodes("//element"))
				{
					TilesetElement element = ReadElement(node);
					Elements.Add(element.Name, element);
				}
			}
		}

		public TilesetElement ReadElement(XmlNode node)
		{
			return new TilesetElement(
				node.Attributes.GetNamedItem("key").Value,
				new GridCoord(
					Int32.Parse(node.SelectSingleNode("./position").Attributes.GetNamedItem("col").Value), 
					Int32.Parse(node.SelectSingleNode("./position").Attributes.GetNamedItem("row").Value)
				),
				new GridCoord(
					Int32.Parse(node.SelectSingleNode("./span").Attributes.GetNamedItem("col").Value),
					Int32.Parse(node.SelectSingleNode("./span").Attributes.GetNamedItem("row").Value)
				)
			);
		}

		public void SerializeTo(BinaryWriter stream)
		{
			stream.Write(Columns);
			stream.Write(Rows);
			stream.Write(Texture.Name);
		}
		public static Tileset FromSerialized(BinaryReader reader)
		{
			uint cols = reader.ReadUInt32();
			uint rows = reader.ReadUInt32();
			Texture2D tilesetTexture = AssetManager<Texture2D>.Retreive(reader.ReadString());
			Tileset result = new Tileset(tilesetTexture);
			result.Columns = cols;
			result.Rows = rows;

			return result;
		}
		
		private bool IsValid(Texture2D atlas)
		{
			return float.IsInteger((float)atlas.Height / (float)Globals.TileHeight)
				&& float.IsInteger((float)atlas.Width / (float)Globals.TileWidth);
		}

		public static List<Tileset> Tilesets = new List<Tileset>();

		public Dictionary<string, TilesetElement> Elements = new Dictionary<string, TilesetElement>();
		public uint Columns
		{ get; private set; }
		public uint Rows
		{ get; private set; }

		public Vector2 TileUv;
		public Texture2D Texture;
	}
}
