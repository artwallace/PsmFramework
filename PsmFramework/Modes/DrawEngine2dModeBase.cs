using System;
using System.Text;
using PsmFramework.Engines.DrawEngine2d;
using PsmFramework.Engines.DrawEngine2d.Drawables;
using PsmFramework.Engines.DrawEngine2d.Support;

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
			FirstPass = true;
		}
		
		protected override void CleanupInternal()
		{
			CleanupDebugInfo();
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
			{
				if (DrawEngine2d.RenderRequired || DebugInfoForcesRender || DrawEngine2d.RenderRequiredLastFrame)
					GetDebugInfo();
			}
		}
		
		internal override void UpdateInternalPost()
		{
			//This is going to get data from the previous frame.
			if (DebugInfoEnabled)
			{
				if (DrawEngine2d.RenderRequired || DebugInfoForcesRender)
					DrawDebugInfo();
			}
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
		
		#region DebugInfo
		
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
		
		private Boolean _DebugInfoEnabled;
		public Boolean DebugInfoEnabled
		{
			get { return _DebugInfoEnabled; }
			set
			{
				if(_DebugInfoEnabled == value)
					return;
				
				_DebugInfoEnabled = value;
				
				//This is necessary until DebugLabel supports better draw method.
				ToggleDebugInfo();
			}
		}
		
		//This is necessary until DebugLabel supports better draw method.
		private void ToggleDebugInfo()
		{
			if(IsDisposed)
				return;
			
			if(_DebugInfoEnabled)
				CreateDebugInfoLabel();
			else
				RemoveDebugInfoLabel();
			
			DrawEngine2d.SetRenderRequired();
		}
		
		private readonly Coordinate2 DebugInfoPstn = new Coordinate2(0f, 0f);
		
		private void CreateDebugInfoLabel()
		{
			//This shouldn't happen but delete it if it's there.
			if(DebugInfoLabel != null)
				DebugInfoLabel.Dispose();
			
			DebugInfoLabel = new DebugLabel(DrawEngine2d.GetOrCreateScreenDebugLayer());
			DebugInfoLabel.Position = DebugInfoPstn;
		}
		
		private void RemoveDebugInfoLabel()
		{
			//This shouldn't happen but whatever.
			if(DebugInfoLabel == null)
				return;
			
			DebugInfoLabel.Dispose();
			DebugInfoLabel = null;
		}
		
		//This is necessary until DebugLabel supports better draw method.
		private DebugLabel DebugInfoLabel;
		
		public Boolean DebugInfoForcesRender;
		
		private StringBuilder DebugInfo;
		
		private const String DebugInfoSeparator = ": ";
		
		private void GetDebugInfo()
		{
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
		
		protected void AddDebugInfoLine(String name, Single data)
		{
			DebugInfo.Append(name);
			DebugInfo.Append(DebugInfoSeparator);
			DebugInfo.Append(data.ToString());
			DebugInfo.AppendLine();
		}
		
		protected void AddDebugInfoLine(String name, Coordinate2 data)
		{
			DebugInfo.Append(name);
			DebugInfo.Append(DebugInfoSeparator);
			DebugInfo.Append(data.ToString());
			DebugInfo.AppendLine();
		}
		
		protected void AddDebugInfoLine(String name, Coordinate2i data)
		{
			DebugInfo.Append(name);
			DebugInfo.Append(DebugInfoSeparator);
			DebugInfo.Append(data.ToString());
			DebugInfo.AppendLine();
		}
		
		protected void AddDebugInfoLine(String name, RectangularArea2 data)
		{
			DebugInfo.Append(name);
			DebugInfo.Append(DebugInfoSeparator);
			DebugInfo.Append(data.ToString());
			DebugInfo.AppendLine();
		}
		
		protected void AddDebugInfoLine(String name, RectangularArea2i data)
		{
			DebugInfo.Append(name);
			DebugInfo.Append(DebugInfoSeparator);
			DebugInfo.Append(data.ToString());
			DebugInfo.AppendLine();
		}
		
		protected void AddDebugInfoLine(String name, Angle2 data)
		{
			DebugInfo.Append(name);
			DebugInfo.Append(DebugInfoSeparator);
			DebugInfo.Append(data.ToString());
			DebugInfo.AppendLine();
		}
		
		private void DrawDebugInfo()
		{
			//DrawEngine2d.SetRenderRequired();
			DebugInfoLabel.Text = DebugInfo.ToString();
		}
		
		#endregion
	}
}

