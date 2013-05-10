using System;
using PsmFramework.Engines.DrawEngine2d;
using PsmFramework.Engines.DrawEngine2d.Drawables;
using PsmFramework.Engines.DrawEngine2d.Layers;
using PsmFramework.Engines.DrawEngine2d.Support;

namespace PsmFramework.Modes
{
	public abstract class DrawEngine2dModeBase : ModeBase, IDebuggable
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
			InitializeBounds();
			InitializeDebugInfo();
			FirstPass = true;
		}
		
		protected override void CleanupInternal()
		{
			CleanupDebugInfo();
			CleanupBounds();
			CleanupDrawEngine2d();
		}
		
		//This is necessary to force debug info to draw at least once (if enabled).
		private Boolean FirstPass;
		
		#endregion
		
		#region Update, Render
		
		internal override void UpdateInternalPre()
		{
			//This is necessary to force debug info to draw at least once (if enabled).
			if (FirstPass)
			{
				FirstPass = false;
				DrawEngine2d.SetRenderRequired();
			}
			
			//This is going to get data from the previous frame.
			if (DebugInfoEnabled)
				DebugInfo.CalcIfRefreshNeeded(Mgr.UpdateTime);
		}
		
		internal override void UpdateInternalPost()
		{
			//This is going to get data from the previous frame.
//			if (DebugInfoEnabled)
//			{
				//TODO: Re-enable or move
//				if (DrawEngine2d.RenderRequired || DebugInfo.RefreshForcesRender)
//					DrawDebugInfo();
//			}
		}
		
		internal override void RenderInternal()
		{
			if (Mgr.ModeChanged)
				return;
			
			StartDrawTimer();
			DrawEngine2d.Render();
			CompleteDrawTimer();
			
			StartSwapBuffersTimer();
			DrawEngine2d.RenderSwapBuffers();
			CompleteSwapBuffersTimer();
		}
		
		#endregion
		
		#region DrawEngine2d
		
		private void InitializeDrawEngine2d()
		{
			DrawEngine2d = new DrawEngine2d(Mgr.GraphicsContext);
			
			DrawEngine2d.SetBlendModeToNormal();
		}
		
		private void CleanupDrawEngine2d()
		{
			DrawEngine2d.Dispose();
			DrawEngine2d = null;
		}
		
		protected DrawEngine2d DrawEngine2d;
		
		#endregion
		
		#region Bounds for IDebuggable
		
		private void InitializeBounds()
		{
		}
		
		private void CleanupBounds()
		{
		}
		
		public RectangularArea2 Bounds
		{
			get
			{
				if (IsDisposed || DrawEngine2d.ScreenCamera == null)
					return RectangularArea2.Zero;
				
				return DrawEngine2d.ScreenCamera.Bounds;
			}
		}
		
		#endregion
		
		#region DebugInfo
		
		private void InitializeDebugInfo()
		{
			DebugInfoEnabled = false;
		}
		
		private void CleanupDebugInfo()
		{
			DebugInfoEnabled = false;
		}
		
		private IDisposablePlus DebugInfoDisposer;
		public IDebugInfo DebugInfo { get; private set; }
		
		public Boolean DebugInfoEnabled
		{
			get { return DebugInfo != null; }
			set
			{
				if (DebugInfoEnabled == value)
					return;
				
				if (value && !IsDisposed)
				{
					DebugLabel l = DebugLabel.CreateDebugLabel(DrawEngine2d, LayerType.Screen, this);
					DebugInfoDisposer = l;
					DebugInfo = l;
				}
				else
				{
					DebugInfoDisposer.Dispose();
					DebugInfoDisposer = null;
					DebugInfo = null;
				}
				
				//SetRecalcRequired();
				DrawEngine2d.SetRenderRequired();
			}
		}
		
		public virtual void RefreshDebugInfo()
		{
			DebugInfo.AddDebugInfoLine("RAM Used", (System.Math.Round(GC.GetTotalMemory(false) / 1048576d, 2)).ToString() + " MiB");
			DebugInfo.AddDebugInfoLine("Update Ticks", Mgr.UpdateLength.Ticks);
			DebugInfo.AddDebugInfoLine("Render Ticks", DrawLength.Ticks);
			DebugInfo.AddDebugInfoLine("Swap Buffers Ticks", SwapBuffersLength.Ticks);
			DebugInfo.AddDebugInfoLine("FPS", Mgr.FramesPerSecond);
			DebugInfo.AddDebugInfoLine("OpenGL Draws", DrawEngine2d.DrawArrayCallsCounter);//and +1 for this
		}
		
		#endregion
	}
}

