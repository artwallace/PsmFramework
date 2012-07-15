using System;
using Sce.PlayStation.Core.Graphics;

namespace PsmFramework.Engines.DrawEngine2d.Textures
{
	public class Texture2dPlus : Texture2D
	{
		#region Constructor, Dispose
		
		public Texture2dPlus(DrawEngine2d drawEngine2d, TextureCachePolicy cachePolicy, String path)
			: base(path, false)
		{
			Initialize(drawEngine2d, cachePolicy, path);
		}
		
		public Texture2dPlus(DrawEngine2d drawEngine2d, TextureCachePolicy cachePolicy, String key, Int32 width, Int32 height, PixelFormat pixelFormat)
			: base(width, height, false, pixelFormat)
		{
			Initialize(drawEngine2d, cachePolicy, key);
		}
		
		protected override void Dispose (bool disposing)
		{
			Cleanup();
			
			base.Dispose (disposing);
		}
		
		#endregion
		
		#region Initialize, Cleanup
		
		private void Initialize(DrawEngine2d drawEngine2d, TextureCachePolicy cachePolicy, String key)
		{
			InitializeDrawEngine2d(drawEngine2d, cachePolicy, key);
		}
		
		private void Cleanup()
		{
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
			
			DrawEngine2d.RegisterTexture2dPlus(Key, this, cachePolicy);
		}
		
		private void CleanupDrawEngine2d()
		{
			DrawEngine2d.UnregisterTexture2dPlus(Key);
			
			Key = null;
			DrawEngine2d = null;
		}
		
		private DrawEngine2d DrawEngine2d;
		
		public String Key { get; private set; }
		
		private TextureCachePolicy CachePolicy;
		
		#endregion
	}
}

