using System;
using System.Text;
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
			InitializeDebugInfo();
		}
		
		protected override void CleanupInternal()
		{
			CleanupDebugInfo();
			CleanupDrawEngine2d();
		}
		
		#endregion
		
		#region Update, Render
		
		internal override void UpdateInternalPre()
		{
			//This is going to get data from the previous frame.
			if (DebugInfoEnabled)
				GetDebugInfo();
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
		
		#region Debug
		
		private void InitializeDebugInfo()
		{
			DebugInfoEnabled = false;
			DebugInfoForcesRender = true;
			DebugInfo = new StringBuilder();
		}
		
		private void CleanupDebugInfo()
		{
			DebugInfoEnabled = false;
			DebugInfoForcesRender = false;
			
			DebugInfo.Clear();
			DebugInfo.Capacity = 0;
			DebugInfo = null;
		}
		
		public Boolean DebugInfoEnabled;
		
		public Boolean DebugInfoForcesRender;
		
		private StringBuilder DebugInfo;
		
		private const String DebugInfoSeparator = ": ";
		
		private void GetDebugInfo()
		{
			if (!DrawEngine2d.RenderRequired && !DebugInfoForcesRender)
				return;
			
			DrawEngine2d.SetRenderRequired();
			
			DebugInfo.Clear();
			
			AddDebugInfoLine("RAM Used", (System.Math.Round(GC.GetTotalMemory(false) / 1048576d, 2)).ToString() + " MiB");
			AddDebugInfoLine("Update Ticks", Mgr.UpdateLength.Ticks);
			AddDebugInfoLine("Render Ticks", DrawLength.Ticks);
			AddDebugInfoLine("Swap Buffers Ticks", SwapBuffersLength.Ticks);
			AddDebugInfoLine("FPS", Mgr.FramesPerSecond);
			AddDebugInfoLine("OpenGL Draws", DrawEngine2d.DrawArrayCallsCounter);//and +1 for this
			
			GetAdditionalDebugInfo();
		}
		
		//TODO: needs a better name.
		protected virtual void GetAdditionalDebugInfo()
		{
		}
		
		protected void AddDebugInfoLine(String name, String data)
		{
			DebugInfo.Append(name);
			DebugInfo.Append(DebugInfoSeparator);
			DebugInfo.AppendLine(data);
		}
		
		protected void AddDebugInfoLine(String name, Int32 data)
		{
			DebugInfo.Append(name);
			DebugInfo.Append(DebugInfoSeparator);
			DebugInfo.Append(data);
			DebugInfo.AppendLine();
		}
		
		protected void AddDebugInfoLine(String name, Int64 data)
		{
			DebugInfo.Append(name);
			DebugInfo.Append(DebugInfoSeparator);
			DebugInfo.Append(data);
			DebugInfo.AppendLine();
		}
		
		#endregion
	}
}

