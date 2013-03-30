using System;
using PsmFramework.Engines.DrawEngine2d.Layers;
using PsmFramework.Engines.DrawEngine2d.Support;

namespace PsmFramework.Engines.DrawEngine2d.Drawables
{
	public abstract class DrawableBase : IDisposablePlus
	{
		#region Constructor, Dispose
		
		public DrawableBase(LayerBase layer)
		{
			InitializeInternal(layer);
			Initialize();
		}
		
		public void Dispose()
		{
			if(IsDisposed)
				return;
			
			Cleanup();
			CleanupInternal();
			IsDisposed = true;
		}
		
		public Boolean IsDisposed { get; private set; }
		
		#endregion
		
		#region Initialize, Cleanup
		
		private void InitializeInternal(LayerBase layer)
		{
			InitializeLayer(layer);
			InitializeDrawEngine2d();
			
			InitializeRecalcRequired();
			InitializeVisible();
			InitializeBounds();
		}
		
		private void CleanupInternal()
		{
			CleanupBounds();
			CleanupVisible();
			CleanupRecalcRequired();
			
			CleanupDrawEngine2d();
			CleanupLayer();
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
		
		public LayerBase Layer;
		
		#endregion
		
		#region DrawEngine2d
		
		private void InitializeDrawEngine2d()
		{
			DrawEngine2d = Layer.DrawEngine2d;
		}
		
		private void CleanupDrawEngine2d()
		{
			DrawEngine2d = null;
		}
		
		public DrawEngine2d DrawEngine2d { get; private set; }
		
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
				
				//if(_Visible)//TODO: It may be better to recalc always. This could be dangerous.
				SetRecalcRequired();
			}
		}
		
		#endregion
		
		#region Bounds
		
		private void InitializeBounds()
		{
		}
		
		private void CleanupBounds()
		{
			_Bounds = default(RectangularArea2);
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
		
		#region OnScreen
		
		public Boolean IsOnScreen()
		{
			throw new NotImplementedException();
		}
		
		#endregion
		
		#region Recalc
		
		protected void Recalc()
		{
			if (IsDisposed)
				return;
			
			ClearRecalcRequired();
			
			RecalcBounds();
			RecalcHelper();
		}
		
		protected abstract void RecalcBounds();
		
		protected abstract void RecalcHelper();
		
		#endregion
	}
}

