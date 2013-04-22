using System;
using System.Collections.Generic;
using PsmFramework.Engines.DrawEngine2d.Layers;

namespace PsmFramework.Engines.DrawEngine2d.Layers
{
	public sealed class LayerManager : ManagerBase
	{
		#region Constructor, Dispose
		
		internal LayerManager(DrawEngine2d drawEngine2d)
			: base(drawEngine2d)
		{
		}
		
		#endregion
		
		#region Initialize, Cleanup
		
		protected override void Initialize()
		{
			InitializeLayers();
		}
		
		protected override void Cleanup()
		{
			CleanupLayers();
		}
		
		#endregion
		
		#region Layers
		
		private void InitializeLayers()
		{
			Layers = new SortedList<Int32, LayerBase>();
		}
		
		private void CleanupLayers()
		{
			ClearLayers();
			Layers = null;
		}
		
		private SortedList<Int32, LayerBase> Layers;
		
		internal void AddLayer(LayerBase layer)
		{
			if (layer == null)
				throw new ArgumentNullException();
			
			if (Layers.ContainsValue(layer))
				throw new ArgumentException("Duplicate layer added to DrawEngine2d.");
			
			if (!layer.IsDebugLayer)
			{
				if (layer.ZIndex > LayerMaxZIndex || layer.ZIndex < LayerMinZIndex)
					throw new InvalidOperationException();
			}
			
			if (Layers.ContainsKey(layer.ZIndex))
				throw new ArgumentException("Duplicate layer added to DrawEngine2d.");
			
			Layers.Add(layer.ZIndex, layer);
			
			DrawEngine2d.SetRenderRequired();
		}
		
		public void RemoveLayer(LayerBase layer)
		{
			if(layer == null)
				throw new ArgumentNullException();
			
			if(!Layers.ContainsValue(layer))
				throw new ArgumentException("Unknown layer requested for removal from DrawEngine2d.");
			
			Int32 valueLocation = Layers.IndexOfValue(layer);
			Int32 zIndex = Layers.Keys[valueLocation];
			Layers.Remove(zIndex);
			
			DrawEngine2d.SetRenderRequired();
		}
		
		public void RemoveLayer(Int32 zIndex)
		{
			if(zIndex > LayerMaxZIndex || zIndex < LayerMinZIndex)
				throw new ArgumentOutOfRangeException();
			
			if (!Layers.ContainsKey(zIndex))
				throw new ArgumentException("Unknown layer requested for removal from DrawEngine2d.");
			
			Layers.Remove(zIndex);
			
			DrawEngine2d.SetRenderRequired();
		}
		
		public void ClearLayers()
		{
			LayerBase[] layers = new LayerBase[Layers.Values.Count];
			Layers.Values.CopyTo(layers, 0);
			
			foreach(LayerBase layer in layers)
				layer.Dispose();
			
			Layers.Clear();
		}
		
		public Boolean CheckIfLayerExists(Int32 zIndex)
		{
				//This method can't be used to check if debug layers exist if this test remains.
				//But it does further hide the debug layers.
			if(zIndex > LayerMaxZIndex || zIndex < LayerMinZIndex)
				throw new ArgumentOutOfRangeException();
			
			return Layers.ContainsKey(zIndex);
		}
		
		#endregion
		
		#region Render
		
		internal void Render()
		{
			foreach(LayerBase layer in Layers.Values)
				layer.Render();
		}
		
		#endregion
		
		#region Normal Layers
		
		public LayerBase GetOrCreateLayer(Int32 zIndex, LayerType type)
		{
			if (zIndex < LayerMinZIndex || zIndex > LayerMaxZIndex)
				throw new ArgumentOutOfRangeException();
			
			if(Layers.ContainsKey(zIndex))
			{
				if (type == LayerType.World && Layers[zIndex] is WorldLayer)
					return (WorldLayer)Layers[zIndex];
				else if (type == LayerType.Screen && Layers[zIndex] is ScreenLayer)
					return (ScreenLayer)Layers[zIndex];
				else
					throw new ArgumentException("The requested layer exists but is the wrong type.");
			}
			
			if (type == LayerType.World)
				return new WorldLayer(DrawEngine2d, zIndex);
			else if (type == LayerType.Screen)
				return new ScreenLayer(DrawEngine2d, zIndex);
			else
				throw new ArgumentException("The requested layer is an unknown type.");
		}
		
		public WorldLayer GetOrCreateWorldLayer(Int32 zIndex)
		{
			if (zIndex < LayerMinZIndex || zIndex > LayerMaxZIndex)
				throw new ArgumentOutOfRangeException();
			
			if(Layers.ContainsKey(zIndex))
			{
				if(Layers[zIndex] is WorldLayer)
					return (WorldLayer)Layers[zIndex];
				else
					throw new ArgumentException("The requested layer is not a WorldLayer.");
			}
			else
				return new WorldLayer(DrawEngine2d, zIndex);
		}
		
		public ScreenLayer GetOrCreateScreenLayer(Int32 zIndex)
		{
			if (zIndex < LayerMinZIndex || zIndex > LayerMaxZIndex)
				throw new ArgumentOutOfRangeException();
			
			if(Layers.ContainsKey(zIndex))
			{
				if(Layers[zIndex] is ScreenLayer)
					return (ScreenLayer)Layers[zIndex];
				else
					throw new ArgumentException("The requested layer is not a ScreenLayer.");
			}
			else
				return new ScreenLayer(DrawEngine2d, zIndex);
		}
		
		public const Int32 LayerMinZIndex = -10000;
		public const Int32 LayerMaxZIndex = 10000;
		
		#endregion
		
		#region Hidden Debug Layers
		
		internal WorldDebugLayer GetOrCreateWorldDebugLayer()
		{
			if(Layers.ContainsKey(WorldDebugLayerZIndex))
				return (WorldDebugLayer)Layers[WorldDebugLayerZIndex];
			
			return new WorldDebugLayer(DrawEngine2d, WorldDebugLayerZIndex);
		}
		
		internal ScreenDebugLayer GetOrCreateScreenDebugLayer()
		{
			if(Layers.ContainsKey(ScreenDebugLayerZIndex))
				return (ScreenDebugLayer)Layers[ScreenDebugLayerZIndex];
			
			return new ScreenDebugLayer(DrawEngine2d, ScreenDebugLayerZIndex);
		}
		
		private const Int32 WorldDebugLayerZIndex = 10001;
		private const Int32 ScreenDebugLayerZIndex = 10002;
		
		#endregion
	}
}

