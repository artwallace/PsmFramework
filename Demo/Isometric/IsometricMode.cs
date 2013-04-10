using System;
using PsmFramework;
using PsmFramework.Engines.DrawEngine2d;
using PsmFramework.Engines.DrawEngine2d.Drawables;
using PsmFramework.Engines.DrawEngine2d.Layers;
using PsmFramework.Engines.DrawEngine2d.Support;
using PsmFramework.Engines.DrawEngine2d.Textures;
using PsmFramework.Modes;
using Sce.PlayStation.Core;

namespace Demo.Isometric
{
	public class IsometricMode : DrawEngine2dModeBase
	{
		#region Constructor
		
		public IsometricMode(AppManager mgr)
			: base(mgr)
		{
		}
		
		#endregion
		
		#region Mode Factory Delegate
		
		public static ModeBase IsometricModeFactory(AppManager mgr)
		{
			return new IsometricMode(mgr);
		}
		
		#endregion
		
		#region Initialize, Cleanup
		
		protected override void Initialize()
		{
			DebugInfoEnabled = true;
		}
		
		protected override void Cleanup()
		{
		}
		
		#endregion
		
		#region Update
		
		public override void Update()
		{
			if (Mgr.GamePad0_Start_Released && Mgr.ModeChangeAllowed)
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

