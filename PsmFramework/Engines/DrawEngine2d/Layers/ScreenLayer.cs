using System;
using PsmFramework.Engines.DrawEngine2d.Cameras;

namespace PsmFramework.Engines.DrawEngine2d.Layers
{
	public sealed class ScreenLayer : LayerBase
	{
		#region Constructor, Dispose
		
		internal ScreenLayer(DrawEngine2d drawEngine2d, Int32 zIndex)
			: base(drawEngine2d, zIndex)
		{
		}
		
		#endregion
		
		#region Camera
		
		public override CameraBase Camera { get { return DrawEngine2d.ScreenCamera; } }
		
		#endregion
	}
}

