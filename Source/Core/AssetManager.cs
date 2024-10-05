using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Rabid
{
	public static class NativeAssetManager
	{ 
		public static FileStream Load(string path)
		{
			return File.OpenRead(ResolvePath(path));
		}

		public static bool Exists(string path)
		{
			return File.Exists(ResolvePath(path));
		}

		public static string ResolvePath(string path)
		{
			return Application.Instance.Content.RootDirectory + "/" + path;
		}
	}


	public static class AssetManager<T>
	{
		/// <summary>
		/// Retreives an asset
		/// Use instead of ContentManager.Load
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public static T Retreive(string name)
		{
			return mAssets.ContainsKey(name) 
				? mAssets[name]
				: Load(name);
		}
		
		/// <summary>
		/// Loads a resource with MonoGame's native ContentManager 
		/// </summary>
		private static T Load(string name)
		{
			T asset = Application.Instance.Content.Load<T>(name);
			mAssets.Add(name, asset);
			return asset;
		}

		private static Dictionary<string, T> mAssets = new Dictionary<string, T>();
	}
}
