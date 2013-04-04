using System;
using System.Collections.Generic;
using Sce.PlayStation.Core.Graphics;

namespace PsmFramework.Engines.DrawEngine2d.Textures
{
	public sealed class TiledTexture : IDisposablePlus
	{
		#region Constructor, Dispose
		
		public TiledTexture(DrawEngine2d drawEngine2d, String path, TextureCachePolicy cachePolicy = TextureCachePolicy.DisposeAfterLastUse)
		{
			Initialize(drawEngine2d, cachePolicy, path);
		}
		
		public TiledTexture(DrawEngine2d drawEngine2d, String key, Texture2dPlus texture, TextureCachePolicy cachePolicy = TextureCachePolicy.DisposeAfterLastUse)
		{
			Initialize(drawEngine2d, cachePolicy, key, texture);
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
		
		private void Initialize(DrawEngine2d drawEngine2d, TextureCachePolicy cachePolicy, String path)
		{
			InitializeDrawEngine2d(drawEngine2d, cachePolicy, path);
			InitializeTexture2d(path);
			InitializeIndexes();
		}
		
		private void Initialize(DrawEngine2d drawEngine2d, TextureCachePolicy cachePolicy, String key, Texture2dPlus texture)
		{
			InitializeDrawEngine2d(drawEngine2d, cachePolicy, key);
			InitializeTexture2d(texture);
			InitializeIndexes();
		}
		
		private void Cleanup()
		{
			CleanupIndexes();
			CleanupTexture2d();
			CleanupDrawEngine2d();
		}
		
		#endregion
		
		#region DrawEngine2d
		
		private void InitializeDrawEngine2d(DrawEngine2d drawEngine2d, TextureCachePolicy cachePolicy, String key)
		{
			if (drawEngine2d == null)
				throw new ArgumentNullException();
			
			DrawEngine2d = drawEngine2d;
			Key = key;
			CachePolicy = cachePolicy;
			
			DrawEngine2d.RegisterTiledTexture(Key, this, cachePolicy);
		}
		
		private void CleanupDrawEngine2d()
		{
			DrawEngine2d.UnregisterTiledTexture(Key);
			
			Key = null;
			DrawEngine2d = null;
		}
		
		private DrawEngine2d DrawEngine2d;
		
		public String Key { get; private set; }
		
		private TextureCachePolicy CachePolicy;
		
		#endregion
		
		#region Texture2d
		
		private void InitializeTexture2d(String path)
		{
			if(Texture != null)
				throw new NotSupportedException("Cannot initialize object more than once.");
			
			if(String.IsNullOrWhiteSpace(path))
				throw new ArgumentException();
			
			Path = path;
			Texture = new Texture2dPlus(DrawEngine2d, TextureCachePolicy.DisposeAfterLastUse, path);
			
			RegisterAsUserOfTexture2d();
		}
		
		private void InitializeTexture2d(Texture2dPlus texture)
		{
			if(Texture != null)
				throw new NotSupportedException("Cannot initialize object more than once.");
			
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
			DrawEngine2d.AddTexture2dPlusUser(Texture.Key, this);
		}
		
		private void UnregisterAsUserOfTexture2d()
		{
			DrawEngine2d.RemoveTexture2dPlusUser(Texture.Key, this);
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
		
//		public Single[] GetTextureCoordinates(TiledTextureIndex index)
//		{
//			switch(index.Type)
//			{
//				case IndexType.Column:
//					return oldColumnIndex[index.Column].CoordinateArray;
//					
//				default:
//					throw new InvalidOperationException("Unknown index type.");
//			}
//		}
		
//		public Single[] GetTextureCoordinates(TiledTextureIndex index, out Int32 width, out Int32 height)
//		{
//			switch(index.Type)
//			{
//				case IndexType.Column:
//					width = oldColumnIndex[index.Column].Width;
//					height = oldColumnIndex[index.Column].Height;
//					return GetTextureCoordinates(index);
//					
//				default:
//					throw new InvalidOperationException("Unknown index type.");
//			}
//		}
		
//		public Single[] GetTextureCoordinates(IndexKey key)
//		{
//			switch (key.Type)
//			{
//				case IndexType.Column:
//					
//					return GetTextureCoordinates(key.Column);
//				case IndexType.Grid:
//					return GetTextureCoordinates(key.Column, key.Row);
//				case IndexType.NamedTiles:
//					return GetTextureCoordinates(key.Name);
//				default:
//					throw new NotImplementedException();
//			}
//		}
		
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
		
//		public void GetTileDimensions(IndexKey key)
//		{
//			switch (key.Type)
//			{
//				case IndexType.Column:
//					return GetTextureCoordinates(name, key.Column);
//				case IndexType.Grid:
//					return GetTextureCoordinates(name, key.Column, key.Row);
//				case IndexType.NamedTiles:
//					return GetTextureCoordinates(name, key.Name);
//				default:
//					throw new NotImplementedException();
//			}
//		}
		
		#endregion
		
	}
}

