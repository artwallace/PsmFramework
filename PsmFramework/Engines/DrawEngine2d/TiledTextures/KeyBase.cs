using System;
using PsmFramework.Engines.DrawEngine2d.Textures;

namespace PsmFramework.Engines.DrawEngine2d.TiledTextures
{
	public abstract class KeyBase : IDisposablePlus
	{
		#region Constructor, Dispose
		
		internal KeyBase(Texture2dArea tile)
		{
			InitializeInternal(tile);
			Initialize();
		}
		
		public void Dispose()
		{
			if(IsDisposed)
				return;
			
			Cleanup();
			CleanupInternal();
			IsDisposed = true;
		}
		
		public Boolean IsDisposed { get; private set; }
		
		#endregion
		
		#region Initialize, Cleanup
		
		private void InitializeInternal(Texture2dArea tile)
		{
			InitializeTile(tile);
		}
		
		private void CleanupInternal()
		{
			CleanupTile();
		}
		
		protected virtual void Initialize()
		{
		}
		
		protected virtual void Cleanup()
		{
		}
		
		#endregion
		
		#region TiledTexture
		
		public abstract TiledTexture TiledTexture { get; }
		
		#endregion
		
		#region Tile
		
		private void InitializeTile(Texture2dArea tile)
		{
			Tile = tile;
		}
		
		private void CleanupTile()
		{
			Tile = default(Texture2dArea);
		}
		
		private Texture2dArea _Tile;
		public Texture2dArea Tile
		{
			get
			{
				if (IsDisposed)
					throw new ObjectDisposedException("KeyBase");
				
				return _Tile;
			}
			private set { _Tile = value; }
		}
		
		public Single[] TextureCoordinates
		{
			get
			{
				if (IsDisposed)
					throw new ObjectDisposedException("KeyBase");
				
				return Tile.CoordinateArray;
			}
		}
		
		#endregion
	}
}

