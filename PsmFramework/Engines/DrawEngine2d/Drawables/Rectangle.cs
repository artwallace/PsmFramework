using System;
using PsmFramework.Engines.DrawEngine2d.Layers;

namespace PsmFramework.Engines.DrawEngine2d.Drawables
{
	public class Rectangle : DrawableBase
	{
		#region Constructor, Dispose
		
		public Rectangle(LayerBase layer)
			: base(layer)
		{
		}
		
		#endregion
		
		#region Render
		
		public override void RenderHelper()
		{
		}
		
		#endregion
		
		#region Recalc
		
		protected override void RecalcBounds()
		{
			throw new NotImplementedException();
		}
		
		protected override void RecalcHelper()
		{
			throw new NotImplementedException();
		}
		
		#endregion
	}
}

