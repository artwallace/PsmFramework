using System;
using PsmFramework.Engines.DrawEngine2d;

namespace PsmFramework.Modes
{
	public abstract class DrawEngine2dModeBase : ModeBase
	{
		#region Constructor, Dispose
		
		protected DrawEngine2dModeBase(AppManager mgr)
			: base(mgr)
		{
		}
		
		#endregion
		
		#region Initialize, Cleanup
		
		protected override void InitializeInternal()
		{
			InitializeDrawEngine2d();
		}
		
		protected override void CleanupInternal()
		{
			CleanupDrawEngine2d();
		}
		
		#endregion
		
		#region Update, Render
		
		internal override void UpdateInternal()
		{
//			if (!Mgr.ModeChanged)
//				DrawEngine2d.Update();
		}
		
		internal override void RenderInternal()
		{
			if (!Mgr.ModeChanged)
				DrawEngine2d.Render();
		}
		
		#endregion
		
		#region DrawEngine2d
		
		private void InitializeDrawEngine2d()
		{
			DrawEngine2d = new DrawEngine2d(Mgr.GraphicsContext);
		}
		
		private void CleanupDrawEngine2d()
		{
			DrawEngine2d.Dispose();
			DrawEngine2d = null;
		}
		
		protected DrawEngine2d DrawEngine2d;
		
		#endregion
	}
}

