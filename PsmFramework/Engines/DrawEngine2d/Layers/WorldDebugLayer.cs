using System;
using PsmFramework.Engines.DrawEngine2d.Cameras;

namespace PsmFramework.Engines.DrawEngine2d.Layers
{
	//TODO: Move to a single DebugLayer?
	internal sealed class WorldDebugLayer : LayerBase
	{
		#region Constructor, Dispose
		
		public WorldDebugLayer(DrawEngine2d drawEngine2d, Int32 zIndex)
			: base(drawEngine2d, zIndex)
		{
		}
		
		#endregion
		
		#region Camera
		
		public override CameraBase Camera { get { return DrawEngine2d.WorldCamera; } }
		
		#endregion
	}
}

