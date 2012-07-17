using System;

namespace PsmFramework.Modes
{
	public abstract class ModeBase : IDisposable
	{
		#region Constructor, Dispose
		
		protected ModeBase(AppManager mgr)
		{
			if (mgr == null)
				throw new ArgumentNullException();
			
			InitializeAppManager(mgr);
			
			InitializeInternal();
			Initialize();
		}
		
		public void Dispose()
		{
			Cleanup();
			CleanupInternal();
			
			CleanupAppManager();
		}
		
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
			Mgr = mgr;
		}
		
		private void CleanupAppManager()
		{
			Mgr = null;
		}
		
		public AppManager Mgr { get; private set; }
		
		#endregion
		
//		#region Fps Governor
//		
//		public virtual Boolean UseCustomFpsLimit { get { return false; } }
//		public virtual FpsPresets FpsLimit { get { return FpsPresets.Max60Fps; } }
//		
//		#endregion
	}
}

