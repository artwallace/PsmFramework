using System;
using System.Collections.Generic;
using Sce.PlayStation.Core.Graphics;
using PsmFramework.Engines.DrawEngine2d.Textures;

namespace PsmFramework.Engines.DrawEngine2d.TiledTextures
{
	public sealed class TiledTexture : IDisposablePlus
	{
		#region Constructor, Dispose
		
		internal TiledTexture(TiledTextureManager tileMgr, String path, TextureCachePolicy cachePolicy = TextureCachePolicy.DisposeAfterLastUse)
		{
			Initialize(tileMgr, path, cachePolicy);
		}
		
		internal TiledTexture(TiledTextureManager tileMgr, String key, Texture2dPlus texture, TextureCachePolicy cachePolicy = TextureCachePolicy.DisposeAfterLastUse)
		{
			Initialize(tileMgr, key, texture, cachePolicy);
		}
		
		public void Dispose()
		{
			if(IsDisposed)
				return;
			
			Cleanup();
			IsDisposed = true;
		}
		
		public Boolean IsDisposed { get; private set; }
		
		#endregion
		
		#region Initialize, Cleanup
		
		private void Initialize(TiledTextureManager tileMgr, String path, TextureCachePolicy cachePolicy)
		{
			InitializeTiledTextureManager(tileMgr);
			InitializeKey(path, cachePolicy);
			InitializeTexture2d(path);
			InitializeIndexes();
		}
		
		private void Initialize(TiledTextureManager tileMgr, String key, Texture2dPlus texture, TextureCachePolicy cachePolicy)
		{
			InitializeTiledTextureManager(tileMgr);
			InitializeKey(key, cachePolicy);
			InitializeTexture2d(texture);
			InitializeIndexes();
		}
		
		private void Cleanup()
		{
			CleanupIndexes();
			CleanupTexture2d();
			CleanupTiledTextureManager();
		}
		
		#endregion
		
		#region TiledTextureManager
		
		private void InitializeTiledTextureManager(TiledTextureManager tileMgr)
		{
			if (tileMgr == null)
				throw new ArgumentNullException();
			
			TiledTextureManager = tileMgr;
		}
		
		private void CleanupTiledTextureManager()
		{
			TiledTextureManager = null;
		}
		
		private TiledTextureManager TiledTextureManager;
		
		#endregion
		
		#region Key
		
		private void InitializeKey(String key, TextureCachePolicy cachePolicy)
		{
			Key = key;
			CachePolicy = cachePolicy;
			
			TiledTextureManager.RegisterTiledTexture(Key, this, cachePolicy);
		}
		
		private void CleanupKey()
		{
			TiledTextureManager.UnregisterTiledTexture(Key);
			
			Key = null;
		}
		
		public String Key { get; private set; }
		
		private TextureCachePolicy CachePolicy;
		
		#endregion
		
		#region Texture2d
		
		private void InitializeTexture2d(String path)
		{
			if(Texture != null)
				throw new InvalidOperationException("Cannot initialize object more than once.");
			
			if(String.IsNullOrWhiteSpace(path))
				throw new ArgumentException();
			
			Path = path;
			Texture = TiledTextureManager.DrawEngine2d.Textures.CreateTexture(path, CachePolicy);
			
			RegisterAsUserOfTexture2d();
		}
		
		private void InitializeTexture2d(Texture2dPlus texture)
		{
			if(Texture != null)
				throw new InvalidOperationException("Cannot initialize object more than once.");
			
			if(texture == null)
				throw new ArgumentNullException();
			
			Path = null;
			Texture = texture;
			
			RegisterAsUserOfTexture2d();
		}
		
		private void CleanupTexture2d()
		{
			UnregisterAsUserOfTexture2d();
			
			Texture = null;
			Path = null;
		}
		
		public String Path { get; private set; }
		
		public Texture2dPlus Texture { get; private set; }
		
		private void RegisterAsUserOfTexture2d()
		{
			TiledTextureManager.DrawEngine2d.Textures.AddUser(Texture.Key, this);
		}
		
		private void UnregisterAsUserOfTexture2d()
		{
			TiledTextureManager.DrawEngine2d.Textures.RemoveUser(Texture.Key, this);
		}
		
		#endregion
		
		#region Indexes
		
		private void InitializeIndexes()
		{
			Indexes = new Dictionary<String, IndexBase>();
		}
		
		private void CleanupIndexes()
		{
			//TODO: should be more elegant.
			List<IndexBase> indexes = new List<IndexBase>(Indexes.Values);
			foreach(IndexBase index in indexes)
				index.Dispose();
			Indexes.Clear();
			Indexes = null;
			
			indexes.Clear();
			indexes = null;
		}
		
		private Dictionary<String, IndexBase> Indexes;
		
		internal void AddIndex(IndexBase index)
		{
			if (Indexes.ContainsKey(index.Name))
				throw new ArgumentException("Duplicate TiledTexture Index encountered (" + index.Name + ").");
			
			Indexes.Add(index.Name, index);
		}
		
		internal void RemoveIndex(String name)
		{
			if (!Indexes.ContainsKey(name))
				throw new ArgumentException("Unknown TiledTexture Index encountered (" + name + ").");
			
			Indexes.Remove(name);
		}
		
		public ColumnIndex CreateColumnIndex(Int32 columns = ColumnIndex.DefaultColumns, String name = ColumnIndex.DefaultName)
		{
			return new ColumnIndex(this, columns, name);
		}
		
		public GridIndex CreateGridIndex(Int32 columns = GridIndex.DefaultColumns, Int32 rows = GridIndex.DefaultRows, String name = GridIndex.DefaultName)
		{
			return new GridIndex(this, columns, rows, name);
		}
		
		#endregion
		
		#region GetTextureCoordinates
		
		public Single[] GetTextureCoordinates(Int32 column)
		{
			return GetTextureCoordinates(IndexBase.DefaultName, column);
		}
		
		public Single[] GetTextureCoordinates(String name, Int32 column)
		{
			ColumnIndex ci = Indexes[name] as ColumnIndex;
			if (ci == null)
				throw new InvalidOperationException();
			
			return ci.GetKey(column).TextureCoordinates;
		}
		
		public Single[] GetTextureCoordinates(Int32 column, Int32 row)
		{
			return GetTextureCoordinates(IndexBase.DefaultName, column, row);
		}
		
		public Single[] GetTextureCoordinates(String name, Int32 column, Int32 row)
		{
			GridIndex gi = Indexes[name] as GridIndex;
			if (gi == null)
				throw new InvalidOperationException();
			
			return gi.GetKey(column, row).TextureCoordinates;
		}
		
		public Single[] GetTextureCoordinates(String tileName)
		{
			return GetTextureCoordinates(IndexBase.DefaultName, tileName);
		}
		
		public Single[] GetTextureCoordinates(String name, String tileName)
		{
			NamedTileIndex nti = Indexes[name] as NamedTileIndex;
			if (nti == null)
				throw new InvalidOperationException();
			
			return nti.GetKey(tileName).TextureCoordinates;
		}
		
		#endregion
	}
}

