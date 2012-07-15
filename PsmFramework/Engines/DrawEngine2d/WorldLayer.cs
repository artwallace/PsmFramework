using System;

namespace PsmFramework.Engines.DrawEngine2d
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
	}
}

