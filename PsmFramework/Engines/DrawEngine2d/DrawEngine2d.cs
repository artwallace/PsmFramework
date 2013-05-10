using System;
using PsmFramework.Engines.DrawEngine2d.Cameras;
using PsmFramework.Engines.DrawEngine2d.Layers;
using PsmFramework.Engines.DrawEngine2d.Shaders;
using PsmFramework.Engines.DrawEngine2d.Support;
using PsmFramework.Engines.DrawEngine2d.Textures;
using PsmFramework.Engines.DrawEngine2d.TiledTextures;
using Sce.PlayStation.Core.Graphics;

namespace PsmFramework.Engines.DrawEngine2d
{
	public sealed class DrawEngine2d : IDisposablePlus
	{
		#region Constructor, Dispose
		
		internal DrawEngine2d(GraphicsContext graphicsContext, CoordinateSystemMode coordinateSystemMode = CoordinateSystemMode.OriginAtUpperLeft)
		{
			if (graphicsContext == null)
				throw new ArgumentNullException();
			
			Initialize(graphicsContext, coordinateSystemMode);
		}
		
		public void Dispose()
		{
			if(IsDisposed)
				return;
			
			Cleanup();
			IsDisposed = true;
		}
		
		public Boolean IsDisposed { get; private set; }
		
		#endregion
		
		#region Initialize, Cleanup
		
		private void Initialize(GraphicsContext graphicsContext, CoordinateSystemMode coordinateSystemMode)
		{
			InitializeGraphicsContext(graphicsContext, coordinateSystemMode);
			InitializeRender();
			InitializeClearColor();
			InitializeOpenGlBlendMode();
			InitializeWorldCamera();
			InitializeScreenCamera();
			InitializeRenderRequiredFlag();
			InitializeShaders();
			InitializeTextures();
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
			CleanupTextures();
			CleanupShaders();
			CleanupRenderRequiredFlag();
			CleanupScreenCamera();
			CleanupWorldCamera();
			CleanupOpenGlBlendMode();
			CleanupClearColor();
			CleanupRender();
			CleanupGraphicsContext();
		}
		
		#endregion
		
		#region Render
		
		private void InitializeRender()
		{
			ResetDrawArrayCallsCounter();
		}
		
		private void CleanupRender()
		{
		}
		
		public void Render()
		{
			ResetDrawArrayCallsCounter();
			
			if(RenderRequired)
			{
				ResetWorkaroundBuffersFilled();
				RenderWork();
			}
			else if (!AreWorkaroundBuffersFilled)//TODO: Remove this stupid workaround.
			{
				IncrementWorkaroundBuffersFilled();
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
			
			Layers.Render();
			
			//Console.WriteLine("DrawEngine2d RenderWork " + DateTime.Now.Ticks);
		}
		
		public void RenderSwapBuffers()
		{
			GraphicsContext.SwapBuffers();
		}
		
		//TODO: Remove this stupid workaround.
		//This workaround is required because the SwapBuffers command
		// is responsible for the fps limiting delay. So we have to ensure that
		// both buffers are filled so we can swap them (to get the fps delay)
		// even when there is nothing new to render. Therefore both buffers
		// contain the exact same image.
		private Int32 WorkaroundBuffersFilled;
		
		//Value of 2 needed to fix bug where PSM discards first buffer.
		//If that bug is fixed, value can be dropped to 1,
		// which is needed to workaround stupid SwapBuffers behaviour.
		private const Int32 WorkaroundBuffersNeeded = 1;
		
		private void IncrementWorkaroundBuffersFilled()
		{
			WorkaroundBuffersFilled++;
		}
		
		private void ResetWorkaroundBuffersFilled()
		{
			WorkaroundBuffersFilled = 0;
		}
		
		private Boolean AreWorkaroundBuffersFilled
		{
			get
			{
				return WorkaroundBuffersFilled >= WorkaroundBuffersNeeded;
			}
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
			CoordinateSystemMode = coordinateSystemMode;
			
			ScreenWidth = GraphicsContext.Screen.Rectangle.Width;
			ScreenHeight = GraphicsContext.Screen.Rectangle.Height;
			
			FrameBuffer fb = GraphicsContext.GetFrameBuffer();
			
			FrameBufferWidth = fb.Width;
			FrameBufferHeight = fb.Height;
			
			FrameBufferWidthAsSingle = (Single)FrameBufferWidth;
			FrameBufferHeightAsSingle = (Single)FrameBufferHeight;
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
		
		public Int32 FrameBufferWidth { get; private set; }
		public Int32 FrameBufferHeight { get; private set; }
		
		public Single FrameBufferWidthAsSingle { get; private set; }
		public Single FrameBufferHeightAsSingle { get; private set; }
		
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
			Layers = new LayerManager(this);
		}
		
		private void CleanupLayers()
		{
			Layers.Dispose();
			Layers = null;
		}
		
		public LayerManager Layers { get; private set; }
		
		#endregion
		
		#region Render Required
		
		private void InitializeRenderRequiredFlag()
		{
			SetRenderRequired();
		}
		
		private void CleanupRenderRequiredFlag()
		{
		}
		
		public Boolean RenderRequired { get; private set; }
		
		public Boolean RenderRequiredLastFrame { get; private set; }
		
		public void SetRenderRequired()
		{
			RenderRequired = true;
		}
		
		private void ResetRenderRequired()
		{
			RenderRequiredLastFrame = RenderRequired;
			RenderRequired = false;
		}
		
		#endregion
		
		#region Textures
		
		private void InitializeTextures()
		{
			Textures = new TextureManager(this);
		}
		
		private void CleanupTextures()
		{
			Textures.Dispose();
			Textures = null;
		}
		
		public TextureManager Textures { get; private set; }
		
		#endregion
		
		#region TiledTextures
		
		private void InitializeTiledTextureManager()
		{
			TiledTextures = new TiledTextureManager(this);
		}
		
		private void CleanupTiledTextureManager()
		{
			TiledTextures.Dispose();
			TiledTextures = null;
		}
		
		public TiledTextureManager TiledTextures { get; private set; }
		
		#endregion
		
		#region WorldCamera
		
		//TODO: Need to ensure that all camera changes are done before the rendering phase starts.
		
		private void InitializeWorldCamera()
		{
			WorldCamera = new WorldCamera(this);
		}
		
		private void CleanupWorldCamera()
		{
			WorldCamera.Dispose();
			WorldCamera = null;
		}
		
		public WorldCamera WorldCamera { get; private set; }
		
		#endregion
		
		#region ScreenCamera
		
		private void InitializeScreenCamera()
		{
			ScreenCamera = new ScreenCamera(this);
		}
		
		private void CleanupScreenCamera()
		{
			ScreenCamera.Dispose();
			ScreenCamera = null;
		}
		
		public ScreenCamera ScreenCamera { get; private set; }
		
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
		
		internal DebugFont DebugFont { get; private set; }
		
		#endregion
		
		#region Debug Ruler
		
		//TODO: do something with this.
		
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
		
		#region Shaders
		
		//TODO: Rework this. Hardcode common shaders and also turn it into a list so other shaders can be added.
		
		private void InitializeShaders()
		{
			SpriteShader = new SpriteShader(this);
			FontShader = new FontShader(this);
		}
		
		private void CleanupShaders()
		{
			SpriteShader.Dispose();
			SpriteShader = null;
			
			FontShader.Dispose();
			FontShader = null;
		}
		
		internal SpriteShader SpriteShader;
		internal FontShader FontShader;
		
		#endregion
		
		#region OpenGlBlendMode
		
		private void InitializeOpenGlBlendMode()
		{
			//These are copied from GameEngine2d. I only have a vague idea what they do.
			NoBlendFunc = new BlendFunc(BlendFuncMode.Add, BlendFuncFactor.One, BlendFuncFactor.One);
			NormalBlendFunc = new BlendFunc(BlendFuncMode.Add, BlendFuncFactor.SrcAlpha, BlendFuncFactor.OneMinusSrcAlpha);
			AdditiveBlendFunc = new BlendFunc(BlendFuncMode.Add, BlendFuncFactor.One, BlendFuncFactor.One);
			MultiplicativeBlendFunc = new BlendFunc(BlendFuncMode.Add, BlendFuncFactor.DstColor, BlendFuncFactor.Zero);
			PremultipliedAlphaBlendFunc = new BlendFunc(BlendFuncMode.Add, BlendFuncFactor.One, BlendFuncFactor.OneMinusSrcAlpha);
		}
		
		private void CleanupOpenGlBlendMode()
		{
			NoBlendFunc = default(BlendFunc);
			NormalBlendFunc = default(BlendFunc);
			AdditiveBlendFunc = default(BlendFunc);
			MultiplicativeBlendFunc = default(BlendFunc);
			PremultipliedAlphaBlendFunc = default(BlendFunc);
		}
		
		private BlendFunc NoBlendFunc;
		private BlendFunc NormalBlendFunc;
		private BlendFunc AdditiveBlendFunc;
		private BlendFunc MultiplicativeBlendFunc;
		private BlendFunc PremultipliedAlphaBlendFunc;
		
		public void SetBlendModeToNone()
		{
			SetBlendMode(NoBlendFunc);
		}
		
		public void SetBlendModeToNormal()
		{
			SetBlendMode(NormalBlendFunc);
		}
		
		public void SetBlendModeToAdditive()
		{
			SetBlendMode(AdditiveBlendFunc);
		}
		
		public void SetBlendModeToMultiplicative()
		{
			SetBlendMode(MultiplicativeBlendFunc);
		}
		
		public void SetBlendModeToPremultipliedAlpha()
		{
			SetBlendMode(PremultipliedAlphaBlendFunc);
		}
		
		public void DisableBlendMode()
		{
			GraphicsContext.Disable(EnableMode.Blend);
			SetRenderRequired();
		}
		
		private void SetBlendMode(BlendFunc blendFunc)
		{
			//Checking just as slow as setting?
			//if(!GraphicsContext.IsEnabled(EnableMode.Blend))
			GraphicsContext.Enable(EnableMode.Blend);
			GraphicsContext.SetBlendFunc(blendFunc);
			SetRenderRequired();
		}
		
		#endregion
	}
}
