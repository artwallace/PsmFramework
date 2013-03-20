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
	}
}

