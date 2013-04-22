using System;
using PsmFramework.Engines.DrawEngine2d.Cameras;

namespace PsmFramework.Engines.DrawEngine2d.Layers
{
	internal sealed class WorldDebugLayer : LayerBase
	{
		#region Constructor, Dispose
		
		internal WorldDebugLayer(DrawEngine2d drawEngine2d, Int32 zIndex)
			: base(drawEngine2d, zIndex)
		{
		}
		
		#endregion
		
		#region Camera
		
		public override CameraBase Camera { get { return DrawEngine2d.WorldCamera; } }
		
		#endregion
		
		#region Type
		
		public override LayerType Type { get { return LayerType.World; } }
		
		internal override Boolean IsDebugLayer { get { return true; } }
		
		#endregion
	}
}

