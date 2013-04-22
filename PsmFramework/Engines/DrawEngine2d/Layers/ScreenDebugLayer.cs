using System;
using PsmFramework.Engines.DrawEngine2d.Cameras;
using PsmFramework.Engines.DrawEngine2d.Drawables;

namespace PsmFramework.Engines.DrawEngine2d.Layers
{
	internal sealed class ScreenDebugLayer : LayerBase
	{
		#region Constructor, Dispose
		
		internal ScreenDebugLayer(DrawEngine2d drawEngine2d, Int32 zIndex)
			: base(drawEngine2d, zIndex)
		{
		}
		
		#endregion
		
		#region Camera
		
		public override CameraBase Camera { get { return DrawEngine2d.ScreenCamera; } }
		
		#endregion
		
		#region Type
		
		public override LayerType Type { get { return LayerType.Screen; } }
		
		internal override Boolean IsDebugLayer { get { return true; } }
		
		#endregion
	}
}

