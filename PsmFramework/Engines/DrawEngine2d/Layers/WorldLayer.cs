using System;
using PsmFramework.Engines.DrawEngine2d.Cameras;

namespace PsmFramework.Engines.DrawEngine2d.Layers
{
	//TODO: Add scales with world
	//TODO: Add rotates with world
	public sealed class WorldLayer : LayerBase
	{
		#region Constructor, Dispose
		
		public WorldLayer(DrawEngine2d drawEngine2d, Int32 zIndex)
			: base(drawEngine2d, zIndex)
		{
		}
		
		#endregion
		
		#region Camera
		
		public override CameraBase Camera { get { return DrawEngine2d.WorldCamera; } }
		
		#endregion
	}
}

