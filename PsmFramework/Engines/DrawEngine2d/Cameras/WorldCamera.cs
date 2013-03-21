using System;
using PsmFramework.Engines.DrawEngine2d.Support;
using Sce.PlayStation.Core;

namespace PsmFramework.Engines.DrawEngine2d.Cameras
{
	//TODO: Need a helper to rotate on a specified point.
	
	public class WorldCamera : CameraBase
	{
		#region Constructor, Dispose
		
		public WorldCamera(DrawEngine2d drawEngine2d)
			: base(drawEngine2d)
		{
		}
		
		#endregion
		
		#region Initialize, Cleanup
		
		protected override void Initialize()
		{
			InitializeZoom();
			InitializeRotation();
		}
		
		protected override void Cleanup()
		{
			CleanupRotation();
			CleanupZoom();
		}
		
		#endregion
		
		#region ProjectionMatrix
		
		protected override void RecalcProjectionMatrix()
		{
			throw new NotImplementedException();
		}
		
		#endregion
		
		#region Center
		
		protected override void InitializeCenter()
		{
			SetCenterAtScreenCenter();
		}
		
		protected override void CleanupCenter()
		{
			_Center = default(Coordinate2);
		}
		
		private Coordinate2 _Center;
		public override Coordinate2 Center { get { return _Center; } }
		
		public void SetCenter(Coordinate2 center)
		{
			throw new NotImplementedException();
		}
		
		public void SetCenter(Single x, Single y)
		{
			throw new NotImplementedException();
		}
		
		public void SetCenterAtOrigin()
		{
			_Center = Coordinate2.X0Y0;
			SetRecalcRequired();
		}
		
		public void SetCenterAtScreenCenter()
		{
			_Center = new Coordinate2(DrawEngine2d.FrameBufferWidth/2, DrawEngine2d.FrameBufferHeight/2);
			SetRecalcRequired();
		}
		
		public void SetCenterToPointWithinBounds(Coordinate2 point, RectangularArea2 bounds)
		{
			throw new NotImplementedException();
		}
		
		#endregion
		
		#region Dimensions
		
		protected override void InitializeDimensions()
		{
			throw new NotImplementedException();
		}
		
		protected override void CleanupDimensions()
		{
			throw new NotImplementedException();
		}
		
		public override Single Width
		{
			get
			{
				throw new NotImplementedException();
			}
		}
		public override Single Height
		{
			get
			{
				throw new NotImplementedException();
			}
		}
		
		#endregion
		
		#region Bounds
		
		protected override void InitializeBounds()
		{
			throw new NotImplementedException();
			
//			switch(CoordinateSystemMode)
//			{
//				case(CoordinateSystemMode.OriginAtUpperLeft):
//					_Top = 0.0f;
//					_Bottom = DrawEngine2d.FrameBufferHeightAsSingle;
//					break;
//				case(CoordinateSystemMode.OriginAtLowerLeft):
//					_Top = DrawEngine2d.FrameBufferHeightAsSingle;
//					_Bottom = 0.0f;
//					break;
//				default:
//					throw new NotSupportedException();
//			}
//			
//			_Left = 0.0f;
//			_Right = DrawEngine2d.FrameBufferWidthAsSingle;
//			
//			_Bounds = new RectangularArea2(_Left, _Top, _Right, _Bottom);
			
			SetRecalcRequired();
		}
		
		protected override void CleanupBounds()
		{
			throw new NotImplementedException();
			
			_Top = default(Single);
			_Bottom = default(Single);
			_Left = default(Single);
			_Right = default(Single);
			
			_Bounds = default(RectangularArea2);
		}
		
		private Single _Top;
		public override Single Top { get { return _Top; } }
		
		private Single _Bottom;
		public override Single Bottom { get { return _Bottom; } }
		
		private Single _Left;
		public override Single Left { get { return _Left; } }
		
		private Single _Right;
		public override Single Right { get { return _Right; } }
		
		private RectangularArea2 _Bounds;
		public override RectangularArea2 Bounds { get { return _Bounds; } }
		
		#endregion
		
		#region Zoom
		
		private void InitializeZoom()
		{
			throw new NotImplementedException();
		}
		
		private void CleanupZoom()
		{
			throw new NotImplementedException();
		}
		
		private Single _Zoom;
		public Single Zoom
		{
			get { return _Zoom; }
			set
			{
				if(_Zoom == value)
					return;
				
				_Zoom = value;
				
				SetRecalcRequired();
			}
		}
		
		public const Single DefaultZoom = 1.0f;
		
		public void SetZoomToDefault()
		{
			Zoom = DefaultZoom;
		}
		
		#endregion
		
		#region Rotation
		
		private void InitializeRotation()
		{
			throw new NotImplementedException();
		}
		
		private void CleanupRotation()
		{
			throw new NotImplementedException();
		}
		
		private Angle2 _Rotation;
		public Angle2 Rotation
		{
			get { return _Rotation; }
			set
			{
				if(_Rotation == value)
					return;
				
				_Rotation = value;
				
				SetRecalcRequired();
			}
		}
		
		public readonly Angle2 DefaultRotation = Angle2.Zero;
		
		public void SetRotationToDefault()
		{
			Rotation = DefaultRotation;
		}
		
		#endregion
	}
}

