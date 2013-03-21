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
					RecalcProjectionMatrixInternal();
				
				return _ProjectionMatrix;
			}
			protected set { _ProjectionMatrix = value; }
		}
		
		public Single Near { get; private set; }
		public Single Far { get; private set; }
		
		private void RecalcProjectionMatrixInternal()
		{
			ClearRecalcRequired();
			
			RecalcProjectionMatrix();
			RecalcBounds();
		}
		
		/// <summary>
		/// Responsible for calculating the ProjectionMatrix and left/top/right/bottom vars.
		/// </summary>
		protected abstract void RecalcProjectionMatrix();
		
		#endregion
		
		#region Center
		
		//TODO: Altering these in the wrong manner could result in an endless loop of recalcs.
		
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
				if(_Center == value)
					return;
				
				SetRecalcRequired();
				
				_Center = value;
			}
		}
		
		public virtual void SetDefaultCenter()
		{
			Center = Coordinate2.X0Y0;
		}
		
		#endregion
		
		#region Dimensions
		
		//TODO: Altering these in the wrong manner could result in an endless loop of recalcs.
		
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
				if(_Width == value)
					return;
				
				SetRecalcRequired();
				
				_Width = value;
			}
		}
		
		private Single _Height;
		public Single Height
		{
			get { return _Height; }
			protected set
			{
				if(_Height == value)
					return;
				
				SetRecalcRequired();
				
				_Height = value;
			}
		}
		
		public virtual void SetDefaultDimensions()
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
		
		private Single _Top;
		public Single Top
		{
			get
			{
				if(RecalcRequired)
					RecalcProjectionMatrixInternal();
				
				return _Top;
			}
			protected set { _Top = value; }
		}
		
		private Single _Bottom;
		public Single Bottom
		{
			get
			{
				if(RecalcRequired)
					RecalcProjectionMatrixInternal();
				
				return _Bottom;
			}
			protected set { _Bottom = value; }
		}
		
		private Single _Left;
		public Single Left
		{
			get
			{
				if(RecalcRequired)
					RecalcProjectionMatrixInternal();
				
				return _Left;
			}
			protected set { _Left = value; }
		}
		
		private Single _Right;
		public Single Right
		{
			get
			{
				if(RecalcRequired)
					RecalcProjectionMatrixInternal();
				
				return _Right;
			}
			protected set { _Right = value; }
		}
		
		private RectangularArea2 _Bounds;
		public RectangularArea2 Bounds
		{
			get
			{
				if(RecalcRequired)
					RecalcProjectionMatrixInternal();
				
				return _Bounds;
			}
			protected set { _Bounds = value; }
		}
		
		private void RecalcBounds()
		{
			Bounds = new RectangularArea2(Left, Top, Right, Bottom);
		}
		
//		public Single Width
//		{
//			get { return Right - Left; }
//		}
//		
//		public Single Height
//		{
//			get
//			{
//				switch(DrawEngine2d.CoordinateSystemMode)
//				{
//					case(CoordinateSystemMode.OriginAtUpperLeft):
//						return Bottom - Top;
//					case(CoordinateSystemMode.OriginAtLowerLeft):
//						return Top - Bottom;
//					default:
//						throw new InvalidProgramException();
//				}
//			}
//		}
		
		#endregion
	}
}

