using System;

namespace PsmFramework.Engines.DrawEngine2d.Cameras
{
	public class WorldCamera : CameraBase
	{
		#region Constructor, Dispose
		
		public WorldCamera(DrawEngine2d drawEngine2d)
			: base(drawEngine2d)
		{
		}
		
		#endregion
		
		#region ProjectionMatrix
		
		protected override void RecalcProjectionMatrix()
		{
		}
		
		#endregion
		
		#region Center
		
//		public override void SetDefaultCenter()
//		{
//			Center = Coordinate2.X0Y0;
//		}
		
		#endregion
		
		#region Dimensions
		
//		public override void SetDefaultDimensions()
//		{
//			Width = DrawEngine2d.FrameBufferWidthAsSingle;
//			Height = DrawEngine2d.FrameBufferHeightAsSingle;
//		}
		
		#endregion
		
		#region Zoom
		
		private Single _Zoom;
		public Single Zoom
		{
			get { return _Zoom; }
			set
			{
				_Zoom = value;
				SetRecalcRequired();
			}
		}
		
		#endregion
		
		#region Rotation
		#endregion
	}
}

