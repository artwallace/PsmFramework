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
		
		protected override void RecalcProjectionMatrix()
		{
			Left = 0f;
			Right = DrawEngine2d.FrameBufferWidthAsSingle;
			
			switch(DrawEngine2d.CoordinateSystemMode)
			{
				case(CoordinateSystemMode.OriginAtUpperLeft):
					Bottom = DrawEngine2d.FrameBufferHeightAsSingle;
					Top = 0f;
					break;
				case(CoordinateSystemMode.OriginAtLowerLeft):
					Top = DrawEngine2d.FrameBufferHeightAsSingle;
					Bottom = 0f;
					break;
				default:
					throw new InvalidProgramException();
			}
			
			ProjectionMatrix = Matrix4.Ortho(Left, Right, Bottom, Top, Near, Far);
		}
		
		#endregion
		
		#region Center
		
		public override void SetDefaultCenter()
		{
			Center = new Coordinate2(DrawEngine2d.FrameBufferWidth/2, DrawEngine2d.FrameBufferHeight/2);
		}
		
		#endregion
		
		#region Dimensions
		
		public override void SetDefaultDimensions()
		{
			Width = DrawEngine2d.FrameBufferWidthAsSingle;
			Height = DrawEngine2d.FrameBufferHeightAsSingle;
		}
		
		#endregion
	}
}

