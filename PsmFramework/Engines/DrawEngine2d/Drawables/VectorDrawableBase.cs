using System;
using PsmFramework.Engines.DrawEngine2d.Layers;
using PsmFramework.Engines.DrawEngine2d.Shaders;
using PsmFramework.Engines.DrawEngine2d.Support;
using Sce.PlayStation.Core;

namespace PsmFramework.Engines.DrawEngine2d.Drawables
{
	public abstract class VectorDrawableBase : DrawableBase
	{
		#region Constructor, Dispose
		
		public VectorDrawableBase(LayerBase layer)
			: base(layer)
		{
		}
		
		#endregion
	}
}

