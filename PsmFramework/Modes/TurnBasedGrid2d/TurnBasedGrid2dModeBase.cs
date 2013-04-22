using System;

namespace PsmFramework.Modes.TurnBasedGrid2d
{
	public abstract class TurnBasedGrid2dModeBase : DrawEngine2dModeBase
	{
		#region Constructor, Dispose
		
		protected TurnBasedGrid2dModeBase(AppManager mgr)
			: base(mgr)
		{
		}
		
		#endregion
	}
}
