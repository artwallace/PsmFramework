using System;

namespace PsmFramework.Engines.DrawEngine2d.Textures
{
	public abstract class IndexBase : IDisposablePlus
	{
		#region Constructor, Dispose
		
		public IndexBase(String name, TiledTexture tiledTexture)
		{
			InitializeInternal(name, tiledTexture);
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
		
		private void InitializeInternal(String name, TiledTexture tiledTexture)
		{
			InitializeName(name);
			InitializeTiledTexture(tiledTexture);
		}
		
		private void CleanupInternal()
		{
			CleanupTiledTexture();
			CleanupName();
		}
		
		protected virtual void Initialize()
		{
		}
		
		protected virtual void Cleanup()
		{
		}
		
		#endregion
		
		#region Name
		
		private void InitializeName(String name)
		{
			if (String.IsNullOrWhiteSpace(name))
				throw new ArgumentException();
			
			Name = name;
		}
		
		private void CleanupName()
		{
			Name = null;
		}
		
		public String Name { get; private set; }
		
		public const String DefaultName = "Default";
		
		#endregion
		
		#region Type
		
		//This is a convenience that maybe saves casting time.
		public abstract IndexType Type { get; }
		
		#endregion
		
		#region TiledTexture
		
		private void InitializeTiledTexture(TiledTexture tiledTexture)
		{
			if (tiledTexture == null)
				throw new ArgumentNullException();
			
			TiledTexture = tiledTexture;
			RegisterWithTiledTexture();
		}
		
		private void CleanupTiledTexture()
		{
			UnregisterFromTiledTexture();
			TiledTexture = null;
		}
		
		public TiledTexture TiledTexture { get; protected set; }
		
		private void RegisterWithTiledTexture()
		{
			TiledTexture.AddIndex(this);
		}
		
		private void UnregisterFromTiledTexture()
		{
			TiledTexture.RemoveIndex(Name);
		}
		
		#endregion
		
//		#region Tiles
//		#endregion
	}
}

