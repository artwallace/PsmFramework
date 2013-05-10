using System;

namespace PsmFramework.Engines.DrawEngine2d
{
	public abstract class ManagerBase : IDisposablePlus
	{
		#region Constructor, Dispose
		
		public ManagerBase(DrawEngine2d drawEngine2d)
		{
			InitializeInternal(drawEngine2d);
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
		
		private void InitializeInternal(DrawEngine2d drawEngine2d)
		{
			InitializeDrawEngine2d(drawEngine2d);
		}
		
		private void CleanupInternal()
		{
			CleanupDrawEngine2d();
		}
		
		protected virtual void Initialize()
		{
		}
		
		protected virtual void Cleanup()
		{
		}
		
		#endregion
		
		#region DrawEngine
		
		private void InitializeDrawEngine2d(DrawEngine2d drawEngine2d)
		{
			DrawEngine2d = drawEngine2d;
		}
		
		private void CleanupDrawEngine2d()
		{
			DrawEngine2d = null;
		}
		
		public DrawEngine2d DrawEngine2d { get; private set; }
		
		#endregion
	}
}

