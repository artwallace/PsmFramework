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
		
		#region ProjectionMatrix
		
		private void InitializeProjectionMatrix()
		{
			SetMatrixRecalcRequired();
		}
		
		private void CleanupProjectionMatrix()
		{
			ClearMatrixRecalcRequired();
			
			ProjectionMatrix = default(Matrix4);
		}
		
		protected Boolean MatrixRecalcRequired { get; private set; }
		
		protected void SetMatrixRecalcRequired()
		{
			MatrixRecalcRequired = true;
			DrawEngine2d.SetRenderRequired();
		}
		
		protected void ClearMatrixRecalcRequired()
		{
			MatrixRecalcRequired = false;
		}
		
		private Matrix4 _ProjectionMatrix;
		public Matrix4 ProjectionMatrix
		{
			get
			{
				if(MatrixRecalcRequired)
					RecalcProjectionMatrixInternal();
				
				return _ProjectionMatrix;
			}
			private set
			{
				_ProjectionMatrix = value;
			}
		}
		
		public Single Near { get; private set; }
		public Single Far { get; private set; }
		
		private void RecalcProjectionMatrixInternal()
		{
			ClearMatrixRecalcRequired();
			
			RecalcProjectionMatrix();
		}
		
		protected abstract void RecalcProjectionMatrix();
		
		#endregion
		
		#region Center
		
		private void InitializeCenter()
		{
			SetDefaultCenter();
		}
		
		private void CleanupCenter()
		{
			Center = default(Coordinate2);
		}
		
		private Coordinate2 _Center;
		public Coordinate2 Center
		{
			get { return _Center; }
			protected set
			{
				SetMatrixRecalcRequired();
				
				_Center = value;
			}
		}
		
		public virtual void SetDefaultCenter()
		{
			Center = Coordinate2.X0Y0;
		}
		
		#endregion
		
		#region Dimensions
		
		private void InitializeDimensions()
		{
			SetDefaultDimensions();
		}
		
		private void CleanupDimensions()
		{
			Width = default(Single);
			Height = default(Single);
		}
		
		private Single _Width;
		public Single Width
		{
			get { return _Width; }
			protected set
			{
				SetMatrixRecalcRequired();
				
				_Width = value;
			}
		}
		
		private Single _Height;
		public Single Height
		{
			get { return _Height; }
			protected set
			{
				SetMatrixRecalcRequired();
				
				_Height = value;
			}
		}
		
		protected virtual void SetDefaultDimensions()
		{
			Width = DrawEngine2d.FrameBufferWidthAsSingle;
			Height = DrawEngine2d.FrameBufferHeightAsSingle;
		}
		
		#endregion
		
		#region Bounds
		
		private void InitializeBounds()
		{
			
		}
		
		private void CleanupBounds()
		{
		}
		
		public Single Top { get; private set; }
		public Single Bottom { get; private set; }
		public Single Left { get; private set; }
		public Single Right { get; private set; }
		
		public RectangularArea2 Bounds { get; private set; }
		
		private void RecalcBounds()
		{
		}
		
		#endregion
		
//		public virtual void SetToDefault()
//		{
//			Center = new Coordinate2();
//			
//			if(DrawEngine2d.CoordinateSystemMode == CoordinateSystemMode.OriginAtUpperLeft)
//			{
//				Top = DrawEngine2d.FrameBufferHeightAsSingle;
//				Bottom = WorldCameraPosition.Y + (FrameBufferHeightAsSingle / 2);
//			}
//			else
//			{
//				throw new NotSupportedException();
//			}
//			
//			Near = -1.0f;
//			Far = 1.0f;
//		}
	}
}

