using System;
using PsmFramework.Engines.DrawEngine2d.Support;
using Sce.PlayStation.Core;

namespace PsmFramework.Engines.DrawEngine2d.Cameras
{
	public class ScreenCamera : CameraBase
	{
		#region Constructor, Dispose
		
		public ScreenCamera(DrawEngine2d drawEngine2d)
			: base(drawEngine2d)
		{
		}
		
		#endregion
		
		#region ProjectionMatrix
		
		protected override void RecalcProjectionMatrixHelper()
		{
			ProjectionMatrix = Matrix4.Ortho(Left, Right, Bottom, Top, Near, Far);
		}
		
		#endregion
		
		#region Center
		
		protected override void InitializeCenter()
		{
			_Center = new Coordinate2(DrawEngine2d.FrameBufferWidth/2, DrawEngine2d.FrameBufferHeight/2);
			SetRecalcRequired();
		}
		
		protected override void CleanupCenter()
		{
			_Center = default(Coordinate2);
		}
		
		private Coordinate2 _Center;
		public override Coordinate2 Center { get { return _Center; } }
		
		#endregion
		
		#region Dimensions
		
		public override Single Width { get { return DrawEngine2d.FrameBufferWidthAsSingle; } }
		public override Single Height { get { return DrawEngine2d.FrameBufferHeightAsSingle; } }
		
		#endregion
		
		#region Bounds
		
		protected override void InitializeBounds()
		{
			RecalcBounds();
		}
		
		protected override void CleanupBounds()
		{
			_Top = default(Single);
			_Bottom = default(Single);
			_Left = default(Single);
			_Right = default(Single);
			
			_Bounds = default(RectangularArea2);
		}
		
		private void RecalcBounds()
		{
			switch(DrawEngine2d.CoordinateSystemMode)
			{
				case(CoordinateSystemMode.OriginAtUpperLeft):
					_Top = 0.0f;
					_Bottom = DrawEngine2d.FrameBufferHeightAsSingle;
					break;
				case(CoordinateSystemMode.OriginAtLowerLeft):
					_Top = DrawEngine2d.FrameBufferHeightAsSingle;
					_Bottom = 0.0f;
					break;
				default:
					throw new NotSupportedException();
			}
			
			_Left = 0.0f;
			_Right = DrawEngine2d.FrameBufferWidthAsSingle;
			
			_Bounds = new RectangularArea2(_Left, _Top, _Right, _Bottom);
			
			SetRecalcRequired();
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
	}
}

