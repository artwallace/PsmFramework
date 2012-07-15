using PsmFramework;

namespace PsmFramework.Modes.Isometric2d
{
	public abstract class Isometric2dModeBase : GameEngine2dModeBase
	{
		#region Constructor, Dispose
		
		protected Isometric2dModeBase(AppManager mgr)
			: base(mgr)
		{
		}
		
		#endregion
		
		#region Initialize, Cleanup
		
		protected override void InitializeInternal()
		{
			base.InitializeInternal();
			
			InitializeLevels();
		}
		
		protected override void CleanupInternal()
		{
			CleanupLevels();
			
			base.CleanupInternal();
		}
		
		#endregion
		
		#region Update, Render
		
		internal override void UpdateInternal()
		{
			base.UpdateInternal();
			
			if (!Mgr.ModeChanged)
				if (Mgr.RunState == RunState.Running)
					Level.UpdateInternal();
		}
		
		#endregion
		
		#region Levels
		
		public delegate LevelBase CreateLevelFactory(Isometric2dModeBase mode);
		
		protected LevelBase Level;
		
		private void InitializeLevels()
		{
		}
		
		private void CleanupLevels()
		{
			if (Level != null)
			{
				Level.Dispose();
				Level = null;
			}
		}
		
		#endregion
		
		#region Debug
		
		protected override void GetAdditionalDebugInfo()
		{
			base.GetAdditionalDebugInfo();
			Level.GetDebugInfo(DebugInfo);
		}
		
		#endregion
	}
}

