using System;
using PsmFramework.Engines.DrawEngine2d.Layers;
using PsmFramework.Engines.DrawEngine2d.Shaders;
using PsmFramework.Engines.DrawEngine2d.Support;
using PsmFramework.Engines.DrawEngine2d.Textures;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;

namespace PsmFramework.Engines.DrawEngine2d.Drawables
{
	public class Label : SpriteDrawableBase
	{
		#region Constructor, Dispose
		
		public Label(LayerBase layer)
			: base(layer)
		{
		}
		
		#endregion
		
		#region Initialize, Cleanup
		
		protected override void Initialize()
		{
		}
		
		protected override void Cleanup()
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
			//TODO: throw new NotImplementedException();
		}
		
		protected override void RecalcHelper()
		{
			//TODO: throw new NotImplementedException();
		}
		
		#endregion
		
		#region Text
		
		private String _Text;
		public String Text
		{
			get { return _Text; }
			set
			{
				if (_Text == value)
					return;
				
				_Text = value;
				
				SetRecalcRequired();
			}
		}
		
		#endregion
	}
}

