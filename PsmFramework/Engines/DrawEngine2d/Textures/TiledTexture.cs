using System;
using System.Collections.Generic;
using Sce.PlayStation.Core.Graphics;

namespace PsmFramework.Engines.DrawEngine2d.Textures
{
	public sealed class TiledTexture : IDisposablePlus
	{
		#region Constructor, Dispose
		
		public TiledTexture(DrawEngine2d drawEngine2d, TextureCachePolicy cachePolicy, String path)
		{
			Initialize(drawEngine2d, cachePolicy, path);
		}
		
		public TiledTexture(DrawEngine2d drawEngine2d, TextureCachePolicy cachePolicy, String key, Texture2dPlus texture)
		{
			Initialize(drawEngine2d, cachePolicy, key, texture);
		}
		
		public void Dispose()
		{
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
			
			InitializeColumnIndex();
			InitializeGridIndex();
			InitializeNamedIndex();
		}
		
		private void Initialize(DrawEngine2d drawEngine2d, TextureCachePolicy cachePolicy, String key, Texture2dPlus texture)
		{
			InitializeDrawEngine2d(drawEngine2d, cachePolicy, key);
			InitializeTexture2d(texture);
			
			InitializeColumnIndex();
			InitializeGridIndex();
			InitializeNamedIndex();
		}
		
		private void Cleanup()
		{
			CleanupNamedIndex();
			CleanupGridIndex();
			CleanupColumnIndex();
			
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
		
		#region GetTextureCoordinates
		
		public Single[] GetTextureCoordinates(TiledTextureIndex index)
		{
			switch(index.Type)
			{
				case TiledTextureIndexType.Column:
					return ColumnIndex[index.Column].CoordinateArray;
					
				default:
					throw new InvalidOperationException("Unknown index type.");
			}
		}
		
		public Single[] GetTextureCoordinates(TiledTextureIndex index, out Int32 width, out Int32 height)
		{
			switch(index.Type)
			{
				case TiledTextureIndexType.Column:
					width = ColumnIndex[index.Column].Width;
					height = ColumnIndex[index.Column].Height;
					return GetTextureCoordinates(index);
					
				default:
					throw new InvalidOperationException("Unknown index type.");
			}
		}
		
		#endregion
		
		#region ColumnIndex
		
		//Right now we will only support a single ColumnIndex.
		//In the future, add support for an array of named ColumnIndexes,
		// built from different source areas within the texture.
		private void InitializeColumnIndex()
		{
			ColumnIndex = new Dictionary<Int32, Texture2dArea>();
		}
		
		private void CleanupColumnIndex()
		{
			ColumnIndex.Clear();
			ColumnIndex = null;
		}
		
		private Dictionary<Int32, Texture2dArea> ColumnIndex;
		
		//TODO: Add support for optional source area and padding.
		public void CreateColumnIndex(Int32 columns)
		{
			//TODO: for the moment, we only support a single ColumnIndex.
			if(ColumnIndex.Count > 0)
				throw new InvalidOperationException("Only one index supported currently.");
			
			if(columns < 1 || columns > Texture.Width)
				throw new ArgumentOutOfRangeException();
			
			if(Texture.Width % columns != 0)
				throw new ArgumentOutOfRangeException();
			
			Int32 tileWidth = Texture.Width / columns;
			Int32 tileHeight = Texture.Height;
			
			for(Int32 i = 0; i < columns; i++)
			{
				Int32 left = i * tileWidth;
				Int32 top = 0;
				Int32 right = left + tileWidth - 1; //zero indexed, so -1.
				Int32 bottom = tileHeight - 1; //zero indexed, so -1.
				
				Texture2dArea area = new Texture2dArea(left, top, right, bottom, Texture.Width, Texture.Height);
				
				ColumnIndex.Add(i, area);
			}
		}
		
		#endregion
		
		#region GridIndex
		
		private void InitializeGridIndex()
		{
		}
		
		private void CleanupGridIndex()
		{
		}
		
		#endregion
		
		#region NamedIndex
		
		private void InitializeNamedIndex()
		{
		}
		
		private void CleanupNamedIndex()
		{
		}
		
		#endregion
	}
}

