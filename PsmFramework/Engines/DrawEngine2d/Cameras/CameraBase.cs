using System;
using Sce.PlayStation.Core;

namespace PsmFramework.Engines.DrawEngine2d.Cameras
{
	public abstract class CameraBase
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
			InitializeBounds();
			InitializeProjectionMatrix();
		}
		
		private void CleanupInternal()
		{
			CleanupProjectionMatrix();
			CleanupBounds();
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
		
		protected void ClearRecalcRequired()
		{
			RecalcRequired = false;
		}
		
		#endregion
		
		#region Bounds
		
		private void InitializeBounds()
		{
		}
		
		private void CleanupBounds()
		{
		}
		
		#endregion
		
		#region ProjectionMatrix
		
		private void InitializeProjectionMatrix()
		{
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
					RecalcProjectionMatrixInternal();
				
				return _ProjectionMatrix;
			}
			private set
			{
				_ProjectionMatrix = value;
			}
		}
		
		private void RecalcProjectionMatrixInternal()
		{
		}
		
		protected abstract void RecalcProjectionMatrix();
		
		#endregion
	}
}

