using System;

namespace PsmFramework.Engines.DrawEngine2d.Textures
{
	public class KeyBase : IDisposablePlus
	{
		#region Constructor, Dispose
		
		public KeyBase()
		{
			InitializeInternal();
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
		
		private void InitializeInternal()
		{
		}
		
		private void CleanupInternal()
		{
		}
		
		protected virtual void Initialize()
		{
		}
		
		protected virtual void Cleanup()
		{
		}
		
		#endregion
	}
}

