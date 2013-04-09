using Demo.MainMenu;
using PsmFramework;
using Sce.PlayStation.Core.Graphics;

namespace Demo
{
	public static class App
	{
		#region Main
		
		public static void Main (string[] launchArgs)
		{
			Initialize();
			Mgr.AppLoop();
			Cleanup();
		}
		
		#endregion
		
		#region Initialize & Cleanup
		
		private static void Initialize()
		{
			Opts = new AppOptions();
			Mgr = new AppManager(Opts, GenerateGraphicsContext(), MainMenuMode.MainMenuModeFactory);
			
			Mgr.GoToTitleScreenMode();
		}
		
		private static void Cleanup()
		{
			Mgr.Dispose();
			Mgr = null;
			
			//Persist your options here if you haven't already.
			Opts = null;
		}
		
		private static AppManager Mgr;
		private static AppOptions Opts;
		
		#endregion
		
		#region GenerateGraphicsContext
		
		private static GraphicsContext GenerateGraphicsContext()
		{
			//Return a custom GC if debugging.
			//if (Debugger.IsAttached)
				//return new GraphicsContext(w, h, PixelFormat.Rgba, PixelFormat.Depth16, MultiSampleMode.Msaa2x);
			
			return new GraphicsContext();
		}
		
		#endregion
	}
}

