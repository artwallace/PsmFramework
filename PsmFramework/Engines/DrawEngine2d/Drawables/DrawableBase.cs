using System;
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
			InitializeChanged();
			InitializeVisible();
			InitializeBounds();
		}
		
		private void CleanupInternal()
		{
			CleanupBounds();
			CleanupVisible();
			CleanupChanged();
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
		
		#region Render
		
		public abstract void Render();
		
		#endregion
		
		#region Changed
		
		private void InitializeChanged()
		{
			//TODO: force changed here?
		}
		
		private void CleanupChanged()
		{
		}
		
		private Boolean _Changed;
		protected Boolean Changed
		{
			get { return _Changed; }
			private set
			{
				if (_Changed == value)
					return;
				
				_Changed = value;
				
				if(_Changed)
					DrawEngine2d.SetRenderRequired();
				
				ChangedHelper();
			}
		}
		
		protected void MarkAsChanged()
		{
			Changed = true;
		}
		
		protected void ClearChanged()
		{
			Changed = false;
		}
		
		//Poor-man's OnChanged event.
		protected virtual void ChangedHelper()
		{
		}
		
		#endregion
		
		#region Visible
		
		private void InitializeVisible()
		{
			Visible = true;
		}
		
		private void CleanupVisible()
		{
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
				MarkAsChanged();
			}
		}
		
		#endregion
		
		#region Bounds
		
		private void InitializeBounds()
		{
		}
		
		private void CleanupBounds()
		{
		}
		
		private  RectangularArea2 _Bounds;
		public RectangularArea2 Bounds
		{
			get
			{
				if(Changed)
					UpdateBounds();
				return _Bounds;
			}
			protected set { _Bounds = value; }
		}
		
		protected abstract void UpdateBounds();
		
		#endregion
		
		#region OnScreen
		
		public Boolean DetermineIfOnScreen()
		{
			throw new NotImplementedException();
		}
		
		#endregion
	}
}

