using System;
using PsmFramework;
using PsmFramework.Modes;
using PsmFramework.Modes.TopDown2d;
using Sce.PlayStation.Core;

namespace Demo.TwinStickShooter
{
	public class TwinStickShooterMode : TopDown2dModeBase
	{
		#region Mode Factory Delegate
		
		public static ModeBase TwinStickShooterModeFactory(AppManager mgr)
		{
			return new TwinStickShooterMode(mgr);
		}
		
		#endregion
		
		#region Constructor, Dispose
		
		protected TwinStickShooterMode(AppManager mgr)
			: base(mgr)
		{
		}
		
		#endregion
		
		#region Initialize, Cleanup
		
		protected override void Initialize()
		{
//			EnableDebugInfo();
//			
//			Level = Level1.Level1Factory(this);
		}
		
		protected override void Cleanup()
		{
//			Level.Dispose();
		}
		
		#endregion
		
		#region Update
		
		public override void Update()
		{
			if (Mgr.GamePad0_Start_Pressed && Mgr.ModeChangeAllowed)
			{
				Mgr.GoToTitleScreenMode();
				return;
			}
			
			if (Mgr.GamePad0_Select_Pressed)
			{
				if (Mgr.RunState == RunState.Running)
					Mgr.SetRunStateToPaused();
				else
					Mgr.SetRunStateToRunning();
			}
		}
		
		#endregion
	}
}
