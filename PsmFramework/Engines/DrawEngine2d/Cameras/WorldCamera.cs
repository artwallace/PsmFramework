using System;
using PsmFramework.Engines.DrawEngine2d.Support;
using Sce.PlayStation.Core;

namespace PsmFramework.Engines.DrawEngine2d.Cameras
{
	//TODO: Need a helper to rotate on a specified point.
	
	public class WorldCamera : CameraBase
	{
		#region Constructor, Dispose
		
		internal WorldCamera(DrawEngine2d drawEngine2d)
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
		
		protected override void RecalcProjectionMatrixHelper()
		{
			_Width = DrawEngine2d.FrameBufferWidthAsSingle;
			_Height = DrawEngine2d.FrameBufferHeightAsSingle;
			
			if(Zoom != DefaultZoom)
			{
				_Width *= Zoom;
				_Height *= Zoom;
			}
			
			_Left = Center.X - (_Width / 2);
			_Right = _Left + _Width;
			
			switch(DrawEngine2d.CoordinateSystemMode)
			{
				case(CoordinateSystemMode.OriginAtUpperLeft):
					_Top = Center.Y - (_Height / 2);
					_Bottom = _Top + _Height;
					break;
				case(CoordinateSystemMode.OriginAtLowerLeft):
					_Bottom = Center.Y - (_Height / 2);
					_Top = _Bottom + _Height;
					break;
				default:
					throw new NotSupportedException();
			}
			
			_Bounds = new RectangularArea2(_Left, _Top, _Right, _Bottom);
			
			ProjectionMatrix = Matrix4.Ortho(Left, Right, Bottom, Top, Near, Far);
			
			if(Rotation != DefaultRotation)
			{
				if(RotationPoint != DefaultRotationPoint)
					throw new NotImplementedException();
				
				ProjectionMatrix *= Matrix4.RotationZ(Rotation.Radian);
			}
		}
		
		#endregion
		
		#region Center
		
		protected override void InitializeCenter()
		{
			SetCenterAtScreenCenter();
		}
		
		protected override void CleanupCenter()
		{
			SetCenterAtOrigin();
		}
		
		private Coordinate2 _Center;
		public override Coordinate2 Center { get { return _Center; } }
		
		public void SetCenter(Coordinate2 center)
		{
			if (_Center == center)
				return;
			
			_Center = center;
			
			SetRecalcRequired();
		}
		
		public void SetCenter(Single x, Single y)
		{
			SetCenter(new Coordinate2(x, y));
		}
		
		public void SetCenterAtOrigin()
		{
			SetCenter(Coordinate2.X0Y0);
		}
		
		public void SetCenterAtScreenCenter()
		{
			SetCenter(new Coordinate2(DrawEngine2d.FrameBufferWidth/2, DrawEngine2d.FrameBufferHeight/2));
		}
		
		public void SetCenterToPointWithinBounds(Coordinate2 point, RectangularArea2 bounds)
		{
			//TODO
			throw new NotImplementedException();
		}
		
		public void AdjustCenter(Single horizontal, Single vertical)
		{
			SetCenter(new Coordinate2(_Center.X + horizontal, _Center.Y + vertical));
		}
		
		#endregion
		
		#region Dimensions
		
		protected override void InitializeDimensions()
		{
			SetRecalcRequired();
		}
		
		protected override void CleanupDimensions()
		{
			_Width = default(Single);
			_Height = default(Single);
		}
		
		private Single _Width;
		public override Single Width
		{
			get
			{
				if(RecalcRequired)
					RecalcProjectionMatrix();
				
				return _Width;
			}
		}
		
		private Single _Height;
		public override Single Height
		{
			get
			{
				if(RecalcRequired)
					RecalcProjectionMatrix();
				
				return _Height;
			}
		}
		
		#endregion
		
		#region Bounds
		
		protected override void InitializeBounds()
		{
			SetRecalcRequired();
		}
		
		protected override void CleanupBounds()
		{
			_Top = default(Single);
			_Bottom = default(Single);
			_Left = default(Single);
			_Right = default(Single);
			
			_Bounds = default(RectangularArea2);
		}
		
		private Single _Top;
		public override Single Top
		{
			get
			{
				if(RecalcRequired)
					RecalcProjectionMatrix();
				
				return _Top;
			}
		}
		
		private Single _Bottom;
		public override Single Bottom
		{
			get
			{
				if(RecalcRequired)
					RecalcProjectionMatrix();
				
				return _Bottom;
			}
		}
		
		private Single _Left;
		public override Single Left
		{
			get
			{
				if(RecalcRequired)
					RecalcProjectionMatrix();
				
				return _Left;
			}
		}
		
		private Single _Right;
		public override Single Right
		{
			get
			{
				if(RecalcRequired)
					RecalcProjectionMatrix();
				
				return _Right;
			}
		}
		
		private RectangularArea2 _Bounds;
		public override RectangularArea2 Bounds
		{
			get
			{
				if(RecalcRequired)
					RecalcProjectionMatrix();
				
				return _Bounds;
			}
		}
		
		#endregion
		
		#region Zoom
		
		private void InitializeZoom()
		{
			SetZoomToDefault();
		}
		
		private void CleanupZoom()
		{
			SetZoomToDefault();
		}
		
		private Single _Zoom;
		public Single Zoom
		{
			get { return _Zoom; }
			set
			{
				Single newZoom = value;
				
				if (newZoom < MinZoom)
					newZoom = MinZoom;
				
				if(_Zoom == newZoom)
					return;
				
				_Zoom = newZoom;
				
				SetRecalcRequired();
			}
		}
		
		public void SetZoomToDefault()
		{
			Zoom = DefaultZoom;
		}
		
		public const Single DefaultZoom = 1.0f;
		
		public const Single MinZoom = 0.01f;
		
		#endregion
		
		#region Rotation
		
		private void InitializeRotation()
		{
			SetRotationToDefault();
		}
		
		private void CleanupRotation()
		{
			SetRotationToDefault();
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
		
		private Coordinate2 _RotationPoint;
		public Coordinate2 RotationPoint
		{
			get { return _RotationPoint; }
			private set
			{
				if(_RotationPoint == value)
					return;
				
				_RotationPoint = value;
				
				SetRecalcRequired();
			}
		}
		
		public void SetRotationToDefault()
		{
			Rotation = DefaultRotation;
			RotationPoint = DefaultRotationPoint;
		}
		
		public readonly Angle2 DefaultRotation = Angle2.Zero;
		public readonly Coordinate2 DefaultRotationPoint = Coordinate2.X0Y0;
		
		#endregion
	}
}

