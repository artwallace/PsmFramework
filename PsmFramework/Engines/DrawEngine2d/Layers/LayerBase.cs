using System;
using System.Collections.Generic;
using PsmFramework.Engines.DrawEngine2d.Cameras;
using PsmFramework.Engines.DrawEngine2d.Drawables;

namespace PsmFramework.Engines.DrawEngine2d.Layers
{
	public abstract class LayerBase : IDisposablePlus
	{
		#region Constructor, Dispose
		
		public LayerBase(DrawEngine2d drawEngine2d, Int32 zIndex)
		{
			InitializeInternal(drawEngine2d, zIndex);
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
		
		private void InitializeInternal(DrawEngine2d drawEngine2d, Int32 zIndex)
		{
			InitializeZIndex(zIndex);
			InitializeDrawEngine2d(drawEngine2d);
			InitializeDrawables();
		}
		
		private void CleanupInternal()
		{
			CleanupDrawables();
			CleanupDrawEngine2d();
			CleanupZIndex();
		}
		
		protected virtual void Initialize()
		{
		}
		
		protected virtual void Cleanup()
		{
		}
		
		#endregion
		
		#region Render
		
		public void Render()
		{
			foreach(DrawableBase item in Items)
				item.Render();
		}
		
		#endregion
		
		#region DrawEngine
		
		private void InitializeDrawEngine2d(DrawEngine2d drawEngine2d)
		{
			DrawEngine2d = drawEngine2d;
			DrawEngine2d.AddLayer(this, ZIndex);
		}
		
		private void CleanupDrawEngine2d()
		{
			DrawEngine2d.RemoveLayer(this);
			DrawEngine2d = null;
		}
		
		internal DrawEngine2d DrawEngine2d;
		
		#endregion
		
		#region Camera
		
		public abstract CameraBase Camera { get; }
		
		#endregion
		
		#region ZIndex
		
		private void InitializeZIndex(Int32 zIndex)
		{
			ZIndex = zIndex;
		}
		
		private void CleanupZIndex()
		{
		}
		
		public Int32 ZIndex { get; private set; }
		
		#endregion
		
		#region Drawables
		
		private void InitializeDrawables()
		{
			Items = new List<DrawableBase>();
		}
		
		private void CleanupDrawables()
		{
			DrawableBase[] items = Items.ToArray();
			
			foreach(DrawableBase item in items)
				item.Dispose();
			Items.Clear();
			
			Items = null;
		}
		
		protected List<DrawableBase> Items { get; set; }
		
		internal void AddDrawable(DrawableBase item)
		{
			if(item == null)
				throw new ArgumentNullException();
			
			if(Items.Contains(item))
				throw new ArgumentException();
			
			Items.Add(item);
			DrawEngine2d.SetRenderRequired();
		}
		
		internal void RemoveDrawable(DrawableBase item)
		{
			if(item == null)
				throw new ArgumentNullException();
			
			if(!Items.Contains(item))
				throw new ArgumentException();
			
			Items.Remove(item);
			DrawEngine2d.SetRenderRequired();
		}
		
		#endregion
	}
}
