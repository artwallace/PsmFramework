using System;
using Demo.MainMenu;
using PsmFramework;
using PsmFramework.Modes;
using PsmFramework.Modes.FixedFront2d;
using Sce.PlayStation.Core;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

namespace Demo.Fireworks
{
	public class FireworksMode : FixedFront2dModeBase
	{
		protected override UInt32 SpritesCapacity { get { return 400; } }
		protected override UInt32 DrawHelpersCapacity { get { return 500; } }
		protected override Vector4 ClearColor { get { return Colors.Black; } }
		protected override Boolean DrawDebugGrid { get { return true; } }
		
		#region Constructor
		
		public FireworksMode(AppManager mgr)
			: base(mgr)
		{
		}
		
		#endregion
		
		#region Mode Logic
		
		protected override void Initialize()
		{
			EnableDebugInfo();
		}
		
		protected override void Cleanup()
		{
		}
		
		public override void Update()
		{
			if (Mgr.GamePad0_Start_Pressed && Mgr.ModeChangeAllowed)
			{
				Mgr.GoToMode(MainMenuMode.MainMenuModeFactory);
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
		
		#region Mode Factory Delegate
		
		public static ModeBase FireworksModeFactory(AppManager mgr)
		{
			return new FireworksMode(mgr);
		}
		
		#endregion
	}
}

