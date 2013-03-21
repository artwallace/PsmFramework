using System;
using PsmFramework.Engines.DrawEngine2d.Support;
using Sce.PlayStation.Core;

namespace PsmFramework.Engines.DrawEngine2d.Cameras
{
	public abstract class CameraBase : IDisposablePlus
	{
		#region Constructor, Dispose
		
		public CameraBase(DrawEngine2d drawEngine2d)
		{
			InitializeInternal(drawEngine2d);
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
		
		private void InitializeInternal(DrawEngine2d drawEngine2d)
		{
			InitializeDrawEngine2d(drawEngine2d);
			
			InitializeRecalcRequired();
			InitializeProjectionMatrix();
			InitializeBounds();
			
			InitializeCenter();
			InitializeDimensions();
		}
		
		private void CleanupInternal()
		{
			CleanupDimensions();
			CleanupCenter();
			
			CleanupBounds();
			CleanupProjectionMatrix();
			CleanupRecalcRequired();
			
			CleanupDrawEngine2d();
		}
		
		protected virtual void Initialize()
		{
		}
		
		protected virtual void Cleanup()
		{
		}
		
		#endregion
		
		#region DrawEngine
		
		private void InitializeDrawEngine2d(DrawEngine2d drawEngine2d)
		{
			DrawEngine2d = drawEngine2d;
		}
		
		private void CleanupDrawEngine2d()
		{
			DrawEngine2d = null;
		}
		
		protected DrawEngine2d DrawEngine2d;
		
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
		
		#region ProjectionMatrix
		
		private void InitializeProjectionMatrix()
		{
			Near = -1.0f;
			Far = 1.0f;
		}
		
		private void CleanupProjectionMatrix()
		{
			ProjectionMatrix = default(Matrix4);
		}
		
		private Matrix4 _ProjectionMatrix;
		public Matrix4 ProjectionMatrix
		{
			get
			{
				if(RecalcRequired)
					RecalcProjectionMatrix();
				
				return _ProjectionMatrix;
			}
			protected set { _ProjectionMatrix = value; }
		}
		
		public Single Near { get; private set; }
		public Single Far { get; private set; }
		
		protected void RecalcProjectionMatrix()
		{
			ClearRecalcRequired();
			
			RecalcProjectionMatrixHelper();
		}
		
		/// <summary>
		/// Responsible for calculating the ProjectionMatrix and 
		/// any necessary left/top/right/bottom/height/width/bounds vars.
		/// </summary>
		protected abstract void RecalcProjectionMatrixHelper();
		
		#endregion
		
		#region Center
		
		protected virtual void InitializeCenter()
		{
		}
		
		protected virtual void CleanupCenter()
		{
		}
		
		public abstract Coordinate2 Center { get; }
		
		#endregion
		
		#region Dimensions
		
		protected virtual void InitializeDimensions()
		{
		}
		
		protected virtual void CleanupDimensions()
		{
		}
		
		public abstract Single Width { get; }
		public abstract Single Height { get; }
		
		#endregion
		
		#region Bounds
		
		protected virtual void InitializeBounds()
		{
		}
		
		protected virtual void CleanupBounds()
		{
		}
		
		public abstract Single Top { get; }
		public abstract Single Bottom { get; }
		public abstract Single Left { get; }
		public abstract Single Right { get; }
		
		public abstract RectangularArea2 Bounds { get; }
		
		#endregion
	}
}

