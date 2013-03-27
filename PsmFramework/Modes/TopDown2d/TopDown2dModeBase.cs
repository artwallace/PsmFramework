using PsmFramework;

namespace PsmFramework.Modes.TopDown2d
{
	public abstract class TopDown2dModeBase : GameEngine2dModeBase
	{
		#region Constructor, Dispose
		
		protected TopDown2dModeBase(AppManager mgr)
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
		
		internal override void UpdateInternalPre()
		{
			base.UpdateInternalPre();
			
			if (!Mgr.ModeChanged)
				if (Mgr.RunState == RunState.Running)
					Level.UpdateInternal();
		}
		
		#endregion
		
		#region Levels
		
		public delegate LevelBase CreateLevelFactory(TopDown2dModeBase mode);
		
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

