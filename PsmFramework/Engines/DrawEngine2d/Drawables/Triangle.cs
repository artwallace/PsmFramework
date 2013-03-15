using System;
using PsmFramework.Engines.DrawEngine2d.Layers;

namespace PsmFramework.Engines.DrawEngine2d.Drawables
{
	public class Triangle : DrawableBase
	{
		#region Constructor, Dispose
		
		public Triangle(LayerBase layer)
			: base(layer)
		{
		}
		
		#endregion
		
		#region Render
		
		public override void Render()
		{
		}
		
		#endregion
		
		#region Bounds
		
		protected override void UpdateBounds()
		{
			throw new NotImplementedException();
		}
		
		#endregion
	}
}

