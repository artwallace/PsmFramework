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
	public sealed class DrawEngine2d : IDisposable
	{
		#region Constructor, Dispose
		
		public DrawEngine2d(GraphicsContext graphicsContext)
		{
			if (graphicsContext == null)
				throw new ArgumentNullException();
			
			Initialize(graphicsContext);
		}
		
		public void Dispose()
		{
			Cleanup();
		}
		
		#endregion
		
		#region Initialize, Cleanup
		
		private void Initialize(GraphicsContext graphicsContext)
		{
			InitializeGraphicsContext(graphicsContext);
			InitializeGraphics();
			InitializeClearColor();
			InitializeCamera();
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
			CleanupCamera();
			CleanupClearColor();
			CleanupGraphics();
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
			
			GraphicsContext.SwapBuffers();
		}
		
		//TODO: Remove this stupid workaround.
		private void RenderWork()
		{
			//Move this all back to the proper Render method
			// once the workaround isn't needed.
			ResetRenderRequired();
				
			GraphicsContext.Clear();
			
			foreach(LayerBase layer in Layers.Values)
				layer.Render();
		}
		
		#endregion
		
		#region GraphicsContext
		
		private void InitializeGraphicsContext(GraphicsContext graphicsContext)
		{
			GraphicsContext = graphicsContext;
			
			ScreenWidth = GraphicsContext.Screen.Rectangle.Width;
			ScreenHeight = GraphicsContext.Screen.Rectangle.Height;
		}
		
		private void CleanupGraphicsContext()
		{
			GraphicsContext = null;
			
			ScreenWidth = 0;
			ScreenHeight = 0;
		}
		
		//TODO: Make this private with wrappers for common functions.
		//Prevent drawables from doing crazy stuff.
		internal GraphicsContext GraphicsContext;
		
		public Single ScreenWidth { get; private set; }
		public Single ScreenHeight { get; private set; }
		
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
		
		#region OpenGL Graphics
		
		//http://en.wikibooks.org/wiki/OpenGL_Programming/Modern_OpenGL_Tutorial_2D
		
		//Most OpenGL programs tend to use a perspective projection matrix
		// to transform the model-space coordinates of a cartesian model into
		// the "view coordinate" space of the screen.
		
		//One of the most common matrices used for orthographic projection
		// can be defined by a 6-tuple, (left, right, bottom, top, near, far),
		// which defines the clipping planes. These planes form a box with
		// the minimum corner at (left, bottom, near) and the maximum
		// corner at (right, top, far).
		//The box is translated so that its center is at the origin, then it is
		// scaled to the unit cube which is defined by having a minimum corner
		// at (-1,-1,-1) and a maximum corner at (1,1,1).
		
		//OpenGL has a special rule to draw fragments at the center of screen pixels,
		// called "diamond rule" [2] [3]. Consequently, it is recommended to add a
		// small translation in X,Y before drawing 2D sprite.
		// glm::translate(glm::mat4(1), glm::vec3(0.375, 0.375, 0.));
		//or
		// glMatrixMode (GL_MODELVIEW);
		// glLoadIdentity ();
		// glTranslatef (0.375, 0.375, 0.);
		
		//http://www.opengl.org/archives/resources/faq/technical/transformations.htm#tran0030
		
		//OpenGL works in the following way: You start of a local coordinate system
		// (of arbitrary units). This coordinate system is transformed to so called
		// eye space coordinates by the modelview matrix (it is called modelview
		// matrix, because it combinnes model and view transformations).
		//The eye space is then transformed to clip space by the projection matrix,
		// immediately followed by the perspective divide to obtain normalized
		// device coordinates ( NDC{x,y,z} = Clip{x,y,z}/Clip_w ).
		// The range [-1,1]^3 in NDC space is mapped to the viewport (x and y)
		// and the set depth range (z).
		//So if you leave your transformation matrices (modelview and projection)
		// identity, then indeed the coordinate ranges [-1,1] will map to the viewport.
		// However by choosing apropriate transformation and projection you can map
		// from modelspace units to viewport units arbitrarily.
		
		//Essentially, a projection matrix is matrix that projects a vertex on a 2D space.
		
		private void InitializeGraphics()
		{
			FrameBufferWidth = GraphicsContext.GetFrameBuffer().Width;
			FrameBufferWidthAsSingle = (Single)FrameBufferWidth;
			FrameBufferHeight = GraphicsContext.GetFrameBuffer().Height;
			FrameBufferHeightAsSingle = (Single)FrameBufferHeight;
			
			//TODO: I'm not sure if the ZNear and ZFar are correct.
			
			//TODO: Is this one pixel too tall and wide?
			ProjectionMatrixLeft = 0.0f;
			ProjectionMatrixRight = FrameBufferWidthAsSingle - 1;
			ProjectionMatrixBottom = 0.0f;
			ProjectionMatrixTop = FrameBufferHeightAsSingle - 1;
			ProjectionMatrixNear = -1.0f;
			ProjectionMatrixFar = 1.0f;
			
			ModelViewMatrixEye = new Vector3(0.0f, FrameBufferHeightAsSingle - 1, 0.0f);
			ModelViewMatrixCenter = new Vector3(0.0f, FrameBufferHeightAsSingle - 1, 1.0f);
			ModelViewMatrixUp = new Vector3(0.0f, -1.0f, 0.0f);
			
			ProjectionMatrix = Matrix4.Ortho(
				ProjectionMatrixLeft,
				ProjectionMatrixRight,
				ProjectionMatrixBottom,
				ProjectionMatrixTop,
				ProjectionMatrixNear,
				ProjectionMatrixFar
				);
			
			ModelViewMatrix = Matrix4.LookAt(
				ModelViewMatrixEye,
				ModelViewMatrixCenter,
				ModelViewMatrixUp
				);
		}
		
		private void CleanupGraphics()
		{
		}
		
		public Matrix4 ProjectionMatrix { get; private set; }
		public Matrix4 ModelViewMatrix { get; private set; }
		
		private Int32 FrameBufferWidth;
		private Int32 FrameBufferHeight;
		
		private Single FrameBufferWidthAsSingle;
		private Single FrameBufferHeightAsSingle;
		
		private Single ProjectionMatrixLeft;
		private Single ProjectionMatrixRight;
		private Single ProjectionMatrixBottom;
		private Single ProjectionMatrixTop;
		private Single ProjectionMatrixNear;
		private Single ProjectionMatrixFar;
		
		private Vector3 ModelViewMatrixEye;
		private Vector3 ModelViewMatrixCenter;
		private Vector3 ModelViewMatrixUp;
		
		#endregion
		
		#region Camera
		
		private void InitializeCamera()
		{
			CameraPosition = Coordinate2.X0Y0;
			CameraZoom = 1.0f;
			CameraRotation = 0.0f;
		}
		
		private void CleanupCamera()
		{
			CameraPosition = Coordinate2.X0Y0;
			CameraZoom = 1.0f;
			CameraRotation = 0.0f;
		}
		
		private Coordinate2 CameraPosition;
		
		public void SetCameraPosition()
		{
		}
		
		//Switch to an enum instead of separate methods?
		public void SetCameraPositionFromBottomLeft()
		{
		}
		
		public void SetCameraPositionFromTopLeft()
		{
		}
		
		private Single CameraZoom;
		
		private Single CameraRotation;
		
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
			DebugFont.CreateFont(this);
		}
		
		private void CleanupDebugFont()
		{
			DebugFont.RemoveFont(this);
		}
		
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
		
		#region Performance Tracking
		
		private void InitializePerformanceTracking()
		{
			ResetDrawArrayCallsCounter();
		}
		
		private void CleanupPerformanceTracking()
		{
		}
		
		public Int32 DrawArrayCallsCounter;
		
		public void ResetDrawArrayCallsCounter()
		{
			DrawArrayCallsCounter = 0;
		}
		
		public void IncrementDrawArrayCallsCounter()
		{
			DrawArrayCallsCounter++;
		}
		
		public Int32 GetDrawArrayCallsCount()
		{
			return DrawArrayCallsCounter;
		}
		
		#endregion
	}
}
