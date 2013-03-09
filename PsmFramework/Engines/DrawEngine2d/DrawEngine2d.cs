using System;
using System.Collections.Generic;
using PsmFramework.Engines.DrawEngine2d.Drawables;
using PsmFramework.Engines.DrawEngine2d.Shaders;
using PsmFramework.Engines.DrawEngine2d.Support;
using PsmFramework.Engines.DrawEngine2d.Textures;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;

namespace PsmFramework.Engines.DrawEngine2d
{
	public sealed class DrawEngine2d : IDisposablePlus
	{
		#region Constructor, Dispose
		
		public DrawEngine2d(GraphicsContext graphicsContext, CoordinateSystemMode coordinateSystemMode = CoordinateSystemMode.OriginAtUpperLeft)
		{
			if (graphicsContext == null)
				throw new ArgumentNullException();
			
			Initialize(graphicsContext, coordinateSystemMode);
		}
		
		public void Dispose()
		{
			Cleanup();
			IsDisposed = true;
		}
		
		public Boolean IsDisposed { get; private set; }
		
		#endregion
		
		#region Initialize, Cleanup
		
		private void Initialize(GraphicsContext graphicsContext, CoordinateSystemMode coordinateSystemMode)
		{
			InitializeGraphicsContext(graphicsContext, coordinateSystemMode);
			InitializeClearColor();
			InitializeWorldCamera();
			InitializeScreenCamera();
			InitializeRenderRequiredFlag();
			InitializeShaders();
			InitializeTexture2dManager();
			InitializeTiledTextureManager();
			InitializeDebugRuler();
			InitializeDebugFont();
			InitializePerformanceTracking();
			InitializeLayers();
		}
		
		private void Cleanup()
		{
			CleanupLayers();
			CleanupPerformanceTracking();
			CleanupDebugFont();
			CleanupDebugRuler();
			CleanupTiledTextureManager();
			CleanupTexture2dManager();
			CleanupShaders();
			CleanupRenderRequiredFlag();
			CleanupScreenCamera();
			CleanupWorldCamera();
			CleanupClearColor();
			CleanupGraphicsContext();
		}
		
		#endregion
		
		#region Render
		
		//TODO: Remove this stupid workaround.
		private Boolean SecondBufferFilled;
		
		public void Render()
		{
			if(RenderRequired)
			{
				SecondBufferFilled = false;
				RenderWork();
			}
			else if (!SecondBufferFilled)//TODO: Remove this stupid workaround.
			{
				SecondBufferFilled = true;
				RenderWork();
			}
		}
		
		//TODO: Remove this stupid workaround.
		private void RenderWork()
		{
			//Move this all back to the proper Render method
			// if/once the workaround isn't needed.
			ResetRenderRequired();
				
			GraphicsContext.Clear();
			
			foreach(LayerBase layer in Layers.Values)
				layer.Render();
		}
		
		public void RenderSwapBuffers()
		{
			GraphicsContext.SwapBuffers();
		}
		
		#endregion
		
		#region Performance Tracking
		
		private void InitializePerformanceTracking()
		{
			ResetDrawArrayCallsCounter();
		}
		
		private void CleanupPerformanceTracking()
		{
			ResetDrawArrayCallsCounter();
		}
		
		public Int32 DrawArrayCallsCounter { get; private set; }
		
		public void ResetDrawArrayCallsCounter()
		{
			DrawArrayCallsCounter = 0;
		}
		
		public void IncrementDrawArrayCallsCounter()
		{
			DrawArrayCallsCounter++;
		}
		
		#endregion
		
		#region GraphicsContext
		
		private void InitializeGraphicsContext(GraphicsContext graphicsContext, CoordinateSystemMode coordinateSystemMode)
		{
			GraphicsContext = graphicsContext;
			
			ScreenWidth = GraphicsContext.Screen.Rectangle.Width;
			ScreenHeight = GraphicsContext.Screen.Rectangle.Height;
			
			FrameBufferWidth = GraphicsContext.GetFrameBuffer().Width;
			FrameBufferWidthAsSingle = (Single)FrameBufferWidth;
			FrameBufferHeight = GraphicsContext.GetFrameBuffer().Height;
			FrameBufferHeightAsSingle = (Single)FrameBufferHeight;
			
			CoordinateSystemMode = coordinateSystemMode;
		}
		
		private void CleanupGraphicsContext()
		{
			CoordinateSystemMode = default(CoordinateSystemMode);
			
			ScreenWidth = 0;
			ScreenHeight = 0;
			
			FrameBufferWidth = 0;
			FrameBufferWidthAsSingle = 0f;
			FrameBufferHeight = 0;
			FrameBufferHeightAsSingle = 0f;
			
			GraphicsContext = null;
		}
		
		//TODO: Make this private with wrappers for common functions.
		//Prevent drawables from doing crazy stuff.
		internal GraphicsContext GraphicsContext;
		
		//TODO: We'll need a system for orientation changes
		// once that is added to psm sdk.
		public Int32 ScreenWidth { get; private set; }
		public Int32 ScreenHeight { get; private set; }
		
		private Int32 FrameBufferWidth;
		private Int32 FrameBufferHeight;
		
		private Single FrameBufferWidthAsSingle;
		private Single FrameBufferHeightAsSingle;
		
		public CoordinateSystemMode CoordinateSystemMode { get; private set; }
		
		#endregion
		
		#region Clear Color
		
		private void InitializeClearColor()
		{
			ClearColor = Colors.Black;
		}
		
		private void CleanupClearColor()
		{
			ClearColor = Colors.Black;
		}
		
		private Color _ClearColor;
		public Color ClearColor
		{
			get { return _ClearColor; }
			set
			{
				_ClearColor = value;
				GraphicsContext.SetClearColor(_ClearColor.AsVector4);
				SetRenderRequired();
			}
		}
		
		#endregion
		
		#region Layers
		
		private void InitializeLayers()
		{
			Layers = new SortedList<Int32, LayerBase>();
		}
		
		private void CleanupLayers()
		{
			LayerBase[] layers = new LayerBase[Layers.Values.Count];
			Layers.Values.CopyTo(layers, 0);
			
			foreach(LayerBase layer in layers)
				layer.Dispose();
			Layers.Clear();
			
			Layers = null;
		}
		
		private SortedList<Int32, LayerBase> Layers { get; set; }
		
		public WorldLayer GetOrCreateWorldLayer(Int32 zIndex)
		{
			if(Layers.ContainsKey(zIndex))
			{
				if(Layers[zIndex] is WorldLayer)
					return (WorldLayer)Layers[zIndex];
				else
					throw new ArgumentException("The requested layer is not a WorldLayer.");
			}
			else
				return new WorldLayer(this, zIndex);
		}
		
		public ScreenLayer GetOrCreateScreenLayer(Int32 zIndex)
		{
			if(Layers.ContainsKey(zIndex))
			{
				if(Layers[zIndex] is ScreenLayer)
					return (ScreenLayer)Layers[zIndex];
				else
					throw new ArgumentException("The requested layer is not a ScreenLayer.");
			}
			else
				return new ScreenLayer(this, zIndex);
		}
		
		internal void AddLayer(LayerBase layer, Int32 zIndex)
		{
			if(layer == null)
				throw new ArgumentNullException();
			
			if(Layers.ContainsValue(layer))
				throw new ArgumentException("Duplicate layer added to DrawEngine2d.");
			
			SetRenderRequired();
			Layers.Add(zIndex, layer);
		}
		
		public void RemoveLayer(LayerBase layer)
		{
			if(layer == null)
				throw new ArgumentNullException();
			
			if(!Layers.ContainsValue(layer))
				throw new ArgumentException("Unknown layer removal requested from DrawEngine2d.");
			
			SetRenderRequired();
			Int32 valueLocation = Layers.IndexOfValue(layer);
			Int32 zIndex = Layers.Keys[valueLocation];
			Layers.Remove(zIndex);
		}
		
		public Boolean CheckIfLayerExists(Int32 zIndex)
		{
			return Layers.ContainsKey(zIndex);
		}
		
		#endregion
		
		#region Render Required
		
		//TODO: Need a better name. dirty?
		
		private void InitializeRenderRequiredFlag()
		{
			//Ensure first past is rendered.
			RenderRequired = true;
		}
		
		private void CleanupRenderRequiredFlag()
		{
		}
		
		private Boolean RenderRequired;
		
		public void SetRenderRequired()
		{
			RenderRequired = true;
		}
		
		private void ResetRenderRequired()
		{
			RenderRequired = false;
		}
		
		#endregion
		
		#region Texture2D Manager
		
		private void InitializeTexture2dManager()
		{
			Textures = new Dictionary<String, Texture2dPlus>();
			Texture2dPlusCachePolicies = new Dictionary<String, TextureCachePolicy>();
			Texture2dPlusUsers = new Dictionary<String, List<TiledTexture>>();
		}
		
		private void CleanupTexture2dManager()
		{
			//Textures
			Texture2dPlus[] cleanup = new Texture2dPlus[Textures.Values.Count];
			Textures.Values.CopyTo(cleanup, 0);
			foreach(Texture2dPlus t in cleanup)
				t.Dispose();
			Textures.Clear();
			Textures = null;
			
			//Cache
			Texture2dPlusCachePolicies.Clear();
			Texture2dPlusCachePolicies = null;
			
			//Users
			foreach(List<TiledTexture> list in Texture2dPlusUsers.Values)
				list.Clear();
			Texture2dPlusUsers.Clear();
			Texture2dPlusUsers = null;
		}
		
		private Dictionary<String, Texture2dPlus> Textures;
		private Dictionary<String, TextureCachePolicy> Texture2dPlusCachePolicies;
		private Dictionary<String, List<TiledTexture>> Texture2dPlusUsers;
		
		internal void RegisterTexture2dPlus(String key, Texture2dPlus texture, TextureCachePolicy cachePolicy)
		{
			if(String.IsNullOrWhiteSpace(key))
				throw new ArgumentNullException();
			
			if(texture == null)
				throw new ArgumentNullException();
			
			if(Textures.ContainsKey(key))
				throw new ArgumentException("Attempt to register duplicate key.");
			
			Textures.Add(key, texture);
			Texture2dPlusCachePolicies.Add(key, cachePolicy);
			Texture2dPlusUsers.Add(key, new List<TiledTexture>());
		}
		
		internal void UnregisterTexture2dPlus(String key)
		{
			if(String.IsNullOrWhiteSpace(key))
				throw new ArgumentNullException();
			
			if(!Textures.ContainsKey(key))
				throw new ArgumentException("Attempt to unregister an unknown key.");
			
			Textures.Remove(key);
			Texture2dPlusCachePolicies.Remove(key);
			Texture2dPlusUsers[key].Clear();
			Texture2dPlusUsers[key] = null;
			Texture2dPlusUsers.Remove(key);
		}
		
		private void ApplyTexture2dPlusCachePolicyForRemovalOfUser(String key)
		{
			if(Texture2dPlusUsers[key].Count > 0)
				return;
			
			if(Texture2dPlusCachePolicies[key] != TextureCachePolicy.DisposeAfterLastUse)
				return;
			
			Textures[key].Dispose();
		}
		
		internal void AddTexture2dPlusUser(String key, TiledTexture user)
		{
			if(String.IsNullOrWhiteSpace(key))
				throw new ArgumentNullException();
			
			if(user == null)
				throw new ArgumentNullException();
			
			if(!Texture2dPlusUsers.ContainsKey(key))
				throw new ArgumentException("Attempt to add a user to an unknown key.");
			
			if(Texture2dPlusUsers[key] == null)
				Texture2dPlusUsers[key] = new List<TiledTexture>();
			
			if(Texture2dPlusUsers[key].Contains(user))
				throw new ArgumentException("Attempt to register a duplicate user.");
			
			Texture2dPlusUsers[key].Add(user);
		}
		
		internal void RemoveTexture2dPlusUser(String key, TiledTexture user)
		{
			if(String.IsNullOrWhiteSpace(key))
				throw new ArgumentNullException();
			
			if(user == null)
				throw new ArgumentNullException();
			
			if(!Texture2dPlusUsers.ContainsKey(key))
				throw new ArgumentException("Attempt to remove a user from an unknown key.");
			
			if(Texture2dPlusUsers[key] == null)
				throw new ArgumentException("Attempt to remove an unknown user.");
			
			if(!Texture2dPlusUsers[key].Contains(user))
				throw new ArgumentException("Attempt to remove an unknown user.");
			
			Texture2dPlusUsers[key].Remove(user);
			
			//Let the cache policy decide what to do.
			ApplyTexture2dPlusCachePolicyForRemovalOfUser(key);
		}
		
		public void SetOpenGlTexture(String key, Int32 index = 0)
		{
			GraphicsContext.SetTexture(index, Textures[key]);
		}
		
		public Texture2dPlus GetTexture(String key)
		{
			if(String.IsNullOrWhiteSpace(key))
				throw new ArgumentNullException();
			
			if(!Textures.ContainsKey(key))
				throw new ArgumentException("Key is unknown.");
			
			return Textures[key];
		}
		
		#endregion
		
		#region TiledTexture Manager
		
		private void InitializeTiledTextureManager()
		{
			TiledTextures = new Dictionary<String, TiledTexture>();
			TiledTextureCachePolicies = new Dictionary<String, TextureCachePolicy>();
			TiledTextureUsers = new Dictionary<String, List<DrawableBase>>();
		}
		
		private void CleanupTiledTextureManager()
		{
			//Textures
			TiledTexture[] cleanup = new TiledTexture[TiledTextures.Values.Count];
			TiledTextures.Values.CopyTo(cleanup, 0);
			foreach(TiledTexture t in cleanup)
				t.Dispose();
			TiledTextures.Clear();
			TiledTextures = null;
			
			//Cache
			TiledTextureCachePolicies.Clear();
			TiledTextureCachePolicies = null;
			
			//Users
			foreach(List<DrawableBase> list in TiledTextureUsers.Values)
				list.Clear();
			TiledTextureUsers.Clear();
			TiledTextureUsers = null;
		}
		
		private Dictionary<String, TiledTexture> TiledTextures;
		private Dictionary<String, TextureCachePolicy> TiledTextureCachePolicies;
		private Dictionary<String, List<DrawableBase>> TiledTextureUsers;
		
		internal void RegisterTiledTexture(String key, TiledTexture texture, TextureCachePolicy cachePolicy)
		{
			if(String.IsNullOrWhiteSpace(key))
				throw new ArgumentNullException();
			
			if(texture == null)
				throw new ArgumentNullException();
			
			if(TiledTextures.ContainsKey(key))
				throw new ArgumentException("Attempt to register duplicate key.");
			
			TiledTextures.Add(key, texture);
			TiledTextureCachePolicies.Add(key, cachePolicy);
			TiledTextureUsers.Add(key, new List<DrawableBase>());
		}
		
		internal void UnregisterTiledTexture(String key)
		{
			if(String.IsNullOrWhiteSpace(key))
				throw new ArgumentNullException();
			
			if(!TiledTextures.ContainsKey(key))
				throw new ArgumentException("Attempt to unregister an unknown key.");
			
			TiledTextures.Remove(key);
			TiledTextureCachePolicies.Remove(key);
			TiledTextureUsers[key].Clear();
			TiledTextureUsers[key] = null;
			TiledTextureUsers.Remove(key);
		}
		
		private void ApplyTiledTextureCachePolicyForRemovalOfUser(String key)
		{
			if(TiledTextureUsers[key].Count > 0)
				return;
			
			if(TiledTextureCachePolicies[key] != TextureCachePolicy.DisposeAfterLastUse)
				return;
			
			TiledTextures[key].Dispose();
		}
		
		internal void AddTiledTextureUser(String key, DrawableBase user)
		{
			if(String.IsNullOrWhiteSpace(key))
				throw new ArgumentNullException();
			
			if(user == null)
				throw new ArgumentNullException();
			
			if(!TiledTextureUsers.ContainsKey(key))
				throw new ArgumentException("Attempt to add a user to an unknown key.");
			
			if(TiledTextureUsers[key] == null)
				TiledTextureUsers[key] = new List<DrawableBase>();
			
			if(TiledTextureUsers[key].Contains(user))
				throw new ArgumentException("Attempt to register a duplicate user.");
			
			TiledTextureUsers[key].Add(user);
		}
		
		internal void RemoveTiledTextureUser(String key, DrawableBase user)
		{
			if(String.IsNullOrWhiteSpace(key))
				throw new ArgumentNullException();
			
			if(user == null)
				throw new ArgumentNullException();
			
			if(!TiledTextureUsers.ContainsKey(key))
				throw new ArgumentException("Attempt to remove a user from an unknown key.");
			
			if(TiledTextureUsers[key] == null)
				throw new ArgumentException("Attempt to remove an unknown user.");
			
			if(!TiledTextureUsers[key].Contains(user))
				throw new ArgumentException("Attempt to remove an unknown user.");
			
			TiledTextureUsers[key].Remove(user);
			
			//Let the cache policy decide what to do.
			ApplyTiledTextureCachePolicyForRemovalOfUser(key);
		}
		
		public TiledTexture GetTiledTexture(String key)
		{
			if(String.IsNullOrWhiteSpace(key))
				throw new ArgumentNullException();
			
			return TiledTextures[key];
		}
		
		#endregion
		
		#region WorldCamera
		
		private void InitializeWorldCamera()
		{
			WorldCameraAtOrigin = new Coordinate2(0f, 0f);
			WorldCameraAtScreenCenter = new Coordinate2(FrameBufferWidth/2, FrameBufferHeight/2);
			
			WorldCameraPosition = WorldCameraAtScreenCenter;
			SetWorldCameraZoomToNormal();
			SetWorldCameraRotationToNormal();
		}
		
		private void CleanupWorldCamera()
		{
			WorldCameraPosition = WorldCameraAtScreenCenter;
			SetWorldCameraZoomToNormal();
			SetWorldCameraRotationToNormal();
		}
		
		private Coordinate2 WorldCameraAtOrigin;
		private Coordinate2 WorldCameraAtScreenCenter;
		
		private Coordinate2 _WorldCameraPosition;
		public Coordinate2 WorldCameraPosition
		{
			get { return _WorldCameraPosition; }
			set
			{
				_WorldCameraPosition = value;
				RecalcWorldCamera();
			}
		}
		
		private Single WorldCameraZoom;
		private Single WorldCameraRotation;
		
		public Matrix4 WorldCameraProjectionMatrix { get; private set; }
		
		private Single WorldCameraProjectionMatrixLeft;
		private Single WorldCameraProjectionMatrixRight;
		private Single WorldCameraProjectionMatrixBottom;
		private Single WorldCameraProjectionMatrixTop;
		private Single WorldCameraProjectionMatrixNear;
		private Single WorldCameraProjectionMatrixFar;
		
		public void SetWorldCameraAtOrigin()
		{
			WorldCameraPosition = WorldCameraAtOrigin;
		}
		
		public void SetWorldCameraAtScreenCenter()
		{
			WorldCameraPosition = WorldCameraAtScreenCenter;
		}
		
		public void SetWorldCameraZoomToNormal()
		{
			SetRenderRequired();
			
			WorldCameraZoom = 1.0f;
		}
		
		public void SetWorldCameraRotationToNormal()
		{
			WorldCameraRotation = 0.0f;
		}
		
		public void RecalcWorldCamera()
		{
			//TODO: Include world zoom and rotate here?
			
			SetRenderRequired();
			
			WorldCameraProjectionMatrixNear = -1.0f;
			WorldCameraProjectionMatrixFar = 1.0f;
			
			WorldCameraProjectionMatrixRight = WorldCameraPosition.X + (FrameBufferWidthAsSingle / 2);
			WorldCameraProjectionMatrixLeft = WorldCameraProjectionMatrixRight - FrameBufferWidthAsSingle;
			
			switch(CoordinateSystemMode)
			{
				case(CoordinateSystemMode.OriginAtUpperLeft):
					WorldCameraProjectionMatrixBottom = WorldCameraPosition.Y + (FrameBufferHeightAsSingle / 2);
					WorldCameraProjectionMatrixTop = WorldCameraProjectionMatrixBottom - FrameBufferHeightAsSingle;
					break;
				case(CoordinateSystemMode.OriginAtLowerLeft):
					WorldCameraProjectionMatrixTop = WorldCameraPosition.Y + (FrameBufferHeightAsSingle / 2);
					WorldCameraProjectionMatrixBottom = WorldCameraProjectionMatrixTop - FrameBufferHeightAsSingle;
					break;
			}
			
			WorldCameraProjectionMatrix = Matrix4.Ortho(
				WorldCameraProjectionMatrixLeft,
				WorldCameraProjectionMatrixRight,
				WorldCameraProjectionMatrixBottom,
				WorldCameraProjectionMatrixTop,
				WorldCameraProjectionMatrixNear,
				WorldCameraProjectionMatrixFar
				);
		}
		
//			ModelViewMatrixEye = new Vector3(0.0f, FrameBufferHeightAsSingle - 1, 0.0f);
//			ModelViewMatrixCenter = new Vector3(0.0f, FrameBufferHeightAsSingle - 1, 1.0f);
//			ModelViewMatrixUp = new Vector3(0.0f, -1.0f, 0.0f);
//			
//			ModelViewMatrix = Matrix4.LookAt(
//				ModelViewMatrixEye,
//				ModelViewMatrixCenter,
//				ModelViewMatrixUp
//				);
//		
//		public Matrix4 ModelViewMatrix { get; private set; }
//		
//		private Vector3 ModelViewMatrixEye;
//		private Vector3 ModelViewMatrixCenter;
//		private Vector3 ModelViewMatrixUp;
		
		#endregion
		
		#region ScreenCamera
		
		private void InitializeScreenCamera()
		{
		}
		
		private void CleanupScreenCamera()
		{
		}
		
		#endregion
		
		#region Debug Ruler
		
		private void InitializeDebugRuler()
		{
//			EnableDebugRuler = false;
//			DebugRulerAxisColor = Colors.Black;
//			DebugRulerAxisThickness = 1.0f;
//			DebugRulerGridColor = Colors.Grey60;
//			DebugRulerGridThickness = 1.0f;
		}
		
		private void CleanupDebugRuler()
		{
		}
		
//		private Boolean _EnableDebugRuler;
//		public Boolean EnableDebugRuler
//		{
//			get { return _EnableDebugRuler; }
//			set
//			{
//				_EnableDebugRuler = value;
//				SetRenderRequired();
//			}
//		}
//		
//		private Color _DebugRulerAxisColor;
//		public Color DebugRulerAxisColor
//		{
//			get { return _DebugRulerAxisColor; }
//			set
//			{
//				_DebugRulerAxisColor = value;
//				SetRenderRequired();
//			}
//		}
//		
//		private Single DebugRulerAxisThickness;
//		
//		public Color _DebugRulerGridColor;
//		public Color DebugRulerGridColor
//		{
//			get { return _DebugRulerGridColor; }
//			set
//			{
//				_DebugRulerGridColor = value;
//				SetRenderRequired();
//			}
//		}
//		
//		private Single DebugRulerGridThickness;
//		
//		private void DrawDebugRulers()
//		{
//			//GraphicsContext.SetLineWidth(DebugRulerAxisThickness);
//			
//			//GraphicsContext.SetLineWidth(1.0f);
//		}
		
		#endregion
		
		#region Debug Font
		
		private void InitializeDebugFont()
		{
			DebugFont = new DebugFont(this);
		}
		
		private void CleanupDebugFont()
		{
			DebugFont.Dispose();
			DebugFont = null;
		}
		
		public DebugFont DebugFont { get; private set; }
		
		#endregion
		
		#region Shaders
		
		private void InitializeShaders()
		{
			FontShader = new FontShader(this);
		}
		
		private void CleanupShaders()
		{
			FontShader.Dispose();
			FontShader = null;
		}
		
		internal FontShader FontShader;
		
		#endregion
	}
}
