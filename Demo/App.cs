using System;
using System.Diagnostics;
using Demo.MainMenu;
using PsmFramework;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Graphics;

namespace Demo
{
	public static class App
	{
		#region Variables
		
		//private const FpsPresets cDefaultFpsLimit = FpsPresets.Max60Fps;
		
		private static AppManager Mgr;
		private static AppOptions Opts;
		
		#endregion
		
		#region Main, Initialize & Cleanup
		
		public static void Main (string[] launchArgs)
		{
			Initialize();
			Mgr.AppLoop();
			Cleanup();
		}
		
		private static void Initialize()
		{
			Opts = new AppOptions();
			Mgr = new AppManager(Opts, GenerateGraphicsContext());//, cDefaultFpsLimit);
			
			Mgr.GoToMode(MainMenuMode.MainMenuModeFactory);
		}
		
		private static void Cleanup()
		{
			Mgr.Dispose();
			Mgr = null;
			
			//Persist your options here if you haven't already.
			Opts = null;
		}
		
		#endregion
		
		#region GenerateGraphicsContext
		
		private static Int32 DebugMinScreenWidth = 854;
		private static Int32 DebugMinScreenHeight = 480;
		
		private static Int32 DebugMaxScreenWidth = 1024;
		private static Int32 DebugMaxScreenHeight = 700;
		
		private static GraphicsContext GenerateGraphicsContext()
		{
			//Return a normal GC if not debugging.
			if (!Debugger.IsAttached)
				return new GraphicsContext();
			
			Random r = new Random(System.Environment.TickCount);
			
			Int32 w = r.Next(DebugMinScreenWidth, DebugMaxScreenWidth + 1);
			Int32 h = r.Next(DebugMinScreenHeight, DebugMaxScreenHeight + 1);
			
			return new GraphicsContext(w, h, PixelFormat.Rgba, PixelFormat.Depth16, MultiSampleMode.Msaa2x);
		}
		
		#endregion
	}
}

