using System;

namespace PsmFramework.Modes.TurnBasedIsometric2d
{
	public abstract class TurnBasedIsometric2dModeBase : DrawEngine2dModeBase
	{
		#region Constructor, Dispose
		
		protected TurnBasedIsometric2dModeBase(AppManager mgr)
			: base(mgr)
		{
		}
		
		#endregion
	}
}

