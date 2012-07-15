using System;

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

