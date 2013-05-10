using System;
using PsmFramework.Engines.DrawEngine2d.Layers;
using PsmFramework.Engines.DrawEngine2d.Support;

namespace PsmFramework.Engines.DrawEngine2d.Drawables
{
	public abstract class DrawableBase : IDisposablePlus, IDebuggable
	{
		#region Constructor, Dispose
		
		public DrawableBase(LayerBase layer)
		{
			InitializeInternal(layer);
			InitializeIntermediary();
			Initialize();
		}
		
		public void Dispose()
		{
			if(IsDisposed)
				return;
			
			Cleanup();
			CleanupIntermediary();
			CleanupInternal();
			IsDisposed = true;
		}
		
		public Boolean IsDisposed { get; private set; }
		
		#endregion
		
		#region Initialize, Cleanup
		
		private void InitializeInternal(LayerBase layer)
		{
			InitializeLayer(layer);
			
			InitializeRecalcRequired();
			InitializeVisible();
			InitializeBounds();
			
			InitializeDebugInfo();
		}
		
		private void CleanupInternal()
		{
			CleanupDebugInfo();
			
			CleanupBounds();
			CleanupVisible();
			CleanupRecalcRequired();
			
			CleanupLayer();
		}
		
		//TODO: This is dumb. Replace these with overrides.
		protected virtual void InitializeIntermediary()
		{
		}
		
		//TODO: This is dumb. Replace these with overrides.
		protected virtual void CleanupIntermediary()
		{
		}
		
		protected virtual void Initialize()
		{
		}
		
		protected virtual void Cleanup()
		{
		}
		
		#endregion
		
		#region Layer
		
		private void InitializeLayer(LayerBase layer)
		{
			Layer = layer;
			Layer.AddDrawable(this);
		}
		
		private void CleanupLayer()
		{
			Layer.RemoveDrawable(this);
			Layer = null;
		}
		
		public LayerBase Layer { get; private set; }
		
		public void ChangeLayer(LayerBase layer)
		{
			throw new NotImplementedException();
			//remember to update the debugInfo's layer too.
		}
		
		#endregion
		
		#region DrawEngine2d
		
		public DrawEngine2d DrawEngine2d
		{
			get
			{
				if (IsDisposed || Layer == null || Layer.IsDisposed)
					return null;
				
				return Layer.DrawEngine2d;
			}
		}
		
		#endregion
		
		#region RecalcRequired
		
		private void InitializeRecalcRequired()
		{
			SetRecalcRequired();
		}
		
		private void CleanupRecalcRequired()
		{
			ClearRecalcRequired();
		}
		
		protected Boolean RecalcRequired { get; private set; }
		
		protected void SetRecalcRequired()
		{
			RecalcRequired = true;
			DrawEngine2d.SetRenderRequired();
		}
		
		private void ClearRecalcRequired()
		{
			RecalcRequired = false;
		}
		
		#endregion
		
		#region Render
		
		public void Render()
		{
			if(RecalcRequired)
				Recalc();
			
			if(!Visible)
				return;
			
			RenderHelper();
		}
		
		public abstract void RenderHelper();
		
		#endregion
		
		#region Visible
		
		private void InitializeVisible()
		{
			Visible = true;
		}
		
		private void CleanupVisible()
		{
			Visible = default(Boolean);
		}
		
		private Boolean _Visible;
		public Boolean Visible
		{
			get { return _Visible; }
			set
			{
				if (_Visible == value)
					return;
				
				_Visible = value;
				
				SetRecalcRequired();
			}
		}
		
		#endregion
		
		#region Bounds
		
		private void InitializeBounds()
		{
			Bounds = RectangularArea2.Zero;
		}
		
		private void CleanupBounds()
		{
			Bounds = RectangularArea2.Zero;
		}
		
		private  RectangularArea2 _Bounds;
		public RectangularArea2 Bounds
		{
			get
			{
				if(RecalcRequired)
					Recalc();
				
				return _Bounds;
			}
			protected set { _Bounds = value; }
		}
		
		#endregion
		
		#region IsOnScreen
		
		public Boolean IsOnScreen()
		{
			throw new NotImplementedException();
		}
		
		#endregion
		
		#region Recalc
		
		protected void Recalc()
		{
			ClearRecalcRequired();
			
			if (IsDisposed)
				return;
			
			RecalcBounds();
			RecalcHelper();
		}
		
		protected abstract void RecalcBounds();
		
		protected abstract void RecalcHelper();
		
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
		
		//TODO: This needs to be made generic and moved to DebugLabel.
		public Boolean DebugInfoEnabled
		{
			get { return DebugInfo != null; }
			set
			{
				if (DebugInfoEnabled == value)
					return;
				
				if (value && !IsDisposed)
				{
					DebugLabel l = DebugLabel.CreateDebugLabel(DrawEngine2d, Layer.Type, this);
					DebugInfoDisposer = l;
					DebugInfo = l;
				}
				else
				{
					DebugInfoDisposer.Dispose();
					DebugInfoDisposer = null;
					DebugInfo = null;
				}
				
				SetRecalcRequired();
			}
		}
		
		public virtual void RefreshDebugInfo()
		{
			//Example:
			//base.RefreshDebugInfo();
			//DebugInfo.AddDebugInfoLine("Position", Position);
		}
		
		#endregion
	}
}

