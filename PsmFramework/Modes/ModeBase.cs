using System;
using System.Diagnostics;

namespace PsmFramework.Modes
{
	public abstract class ModeBase : IDisposablePlus
	{
		#region Constructor, Dispose
		
		protected ModeBase(AppManager mgr)
		{
			InitializeAppManager(mgr);
			
			InitializePerformanceTracking();
			
			InitializeInternal();
			Initialize();
		}
		
		public void Dispose()
		{
			Cleanup();
			CleanupInternal();
			
			CleanupPerformanceTracking();
			
			CleanupAppManager();
			
			IsDisposed = true;
		}
		
		public Boolean IsDisposed { get; private set; }
		
		#endregion
		
		#region Initialize, Cleanup
		
		protected abstract void InitializeInternal();
		
		protected abstract void CleanupInternal();
		
		#endregion
		
		#region Update, Render
		
		internal abstract void UpdateInternal();
		
		internal abstract void RenderInternal();
		
		#endregion
		
		#region Mode Logic
		
		protected abstract void Initialize();
		
		protected abstract void Cleanup();
		
		public abstract void Update();
		
		#endregion
		
		#region App Manager
		
		private void InitializeAppManager(AppManager mgr)
		{
			if (mgr == null)
				throw new ArgumentNullException();
			
			Mgr = mgr;
		}
		
		private void CleanupAppManager()
		{
			Mgr = null;
		}
		
		public AppManager Mgr { get; private set; }
		
		#endregion
		
		#region Performance Tracking
		
		private void InitializePerformanceTracking()
		{
			DrawTimer = new Stopwatch();
			SwapBuffersTimer = new Stopwatch();
		}
		
		private void CleanupPerformanceTracking()
		{
			DrawTimer.Stop();
			DrawTimer = null;
			
			SwapBuffersTimer.Stop();
			SwapBuffersTimer = null;
		}
		
		private Stopwatch DrawTimer;
		private Stopwatch SwapBuffersTimer;
		
		public TimeSpan DrawLength { get; private set; }
		public TimeSpan SwapBuffersLength { get; private set; }
		
		protected void StartDrawTimer()
		{
			DrawTimer.Reset();
			DrawTimer.Start();
		}
		
		protected void CompleteDrawTimer()
		{
			DrawTimer.Stop();
			DrawLength = DrawTimer.Elapsed;
		}
		
		protected void StartSwapBuffersTimer()
		{
			SwapBuffersTimer.Reset();
			SwapBuffersTimer.Start();
		}
		
		protected void CompleteSwapBuffersTimer()
		{
			SwapBuffersTimer.Stop();
			SwapBuffersLength = SwapBuffersTimer.Elapsed;
		}
		
		#endregion
	}
}

