using System;
using PsmFramework.Engines.DrawEngine2d.Cameras;

namespace PsmFramework.Engines.DrawEngine2d.Layers
{
	//All world layers use the same camera.
	//Perhaps in future I will allow custom cameras for certain layers.
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

