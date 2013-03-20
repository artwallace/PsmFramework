using System;

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
		}
		
		#endregion
	}
}

