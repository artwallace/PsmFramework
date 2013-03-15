using System;

namespace PsmFramework.Engines.DrawEngine2d.Layers
{
	public sealed class ScreenLayer : LayerBase
	{
		#region Constructor, Dispose
		
		public ScreenLayer(DrawEngine2d drawEngine2d, Int32 zIndex)
			: base(drawEngine2d, zIndex)
		{
		}
		
		#endregion
	}
}

