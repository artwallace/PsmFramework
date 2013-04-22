using System;
using Sce.PlayStation.Core.Graphics;

namespace PsmFramework.Engines.DrawEngine2d.Textures
{
	public sealed class Texture2dPlus : Texture2D
	{
		#region Constructor, Dispose
		
		internal Texture2dPlus(TextureManager textureMgr, String path, TextureCachePolicy cachePolicy = TextureCachePolicy.DisposeAfterLastUse)
			: base(path, false)
		{
			Initialize(textureMgr, cachePolicy, path);
		}
		
		internal Texture2dPlus(TextureManager textureMgr, String key, Int32 width, Int32 height, PixelFormat pixelFormat, TextureCachePolicy cachePolicy = TextureCachePolicy.DisposeAfterLastUse)
			: base(width, height, false, pixelFormat)
		{
			Initialize(textureMgr, cachePolicy, key);
		}
		
		protected override void Dispose(bool disposing)
		{
			Cleanup();
			
			base.Dispose(disposing);
		}
		
		#endregion
		
		#region Initialize, Cleanup
		
		private void Initialize(TextureManager textureMgr, TextureCachePolicy cachePolicy, String key)
		{
			InitializeTextureManager(textureMgr, cachePolicy, key);
		}
		
		private void Cleanup()
		{
			CleanupTextureManager();
		}
		
		#endregion
		
		#region TextureManager
		
		private void InitializeTextureManager(TextureManager textureMgr, TextureCachePolicy cachePolicy, String key)
		{
			if (textureMgr == null)
				throw new ArgumentNullException();
			
			TextureManager = textureMgr;
			Key = key;
			CachePolicy = cachePolicy;
			
			TextureManager.RegisterTexture(Key, this, cachePolicy);
		}
		
		private void CleanupTextureManager()
		{
			TextureManager.UnregisterTexture(Key);
			
			Key = null;
			TextureManager = null;
		}
		
		private TextureManager TextureManager;
		
		public String Key { get; private set; }
		
		private TextureCachePolicy CachePolicy;
		
		#endregion
	}
}

