using System;

namespace PsmFramework.Modes.TurnBasedGrid2d
{
	public abstract class TurnBasedGrid2dModeBase : GameEngine2dModeBase
	{
		#region Constructor, Dispose
		
		protected TurnBasedGrid2dModeBase(AppManager mgr)
			: base(mgr)
		{
		}
		
		#endregion
		
		#region Initialize, Cleanup
		
		protected override void InitializeInternal()
		{
			base.InitializeInternal();
			
			//InitializeLevels();
		}
		
		protected override void CleanupInternal()
		{
			//CleanupLevels();
			
			base.CleanupInternal();
		}
		
		#endregion
	}
}
