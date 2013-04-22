using System;
using Demo.ImageViewer;
using Demo.Isometric;
using Demo.SpaceRockets;
using Demo.TwinStickShooter;
using PsmFramework;
using PsmFramework.Engines.DrawEngine2d.Drawables;
using PsmFramework.Engines.DrawEngine2d.Layers;
using PsmFramework.Engines.DrawEngine2d.Support;
using PsmFramework.Engines.DrawEngine2d.Textures;
using PsmFramework.Modes;
using Sce.PlayStation.Core.Imaging;
using PsmFramework.Engines.DrawEngine2d.TiledTextures;

namespace Demo.MainMenu
{
	public class MainMenuMode : DrawEngine2dModeBase
	{
		#region Mode Factory Delegate
		
		public static ModeBase MainMenuModeFactory(AppManager mgr)
		{
			return new MainMenuMode(mgr);
		}
		
		#endregion
		
		#region Constructor, Dispose
		
		public MainMenuMode(AppManager mgr)
			: base(mgr)
		{
		}
		
		#endregion
		
		#region Initialize, Cleanup
		
		protected override void Initialize()
		{
			DebugInfoEnabled = true;
			//DebugInfoForcesRender = false;
			
			InitializeLayersAndSprites();
		}
		
		protected override void Cleanup()
		{
			CleanupLayersAndSprites();
		}
		
		#endregion
		
		#region Update
		
		public override void Update()
		{
			CheckForModeChange();
		}
		
		#endregion
		
		#region Change Modes
		
		private void CheckForModeChange()
		{
			if (!Mgr.ModeChangeAllowed)
				return;
			
			if (Mgr.GamePad0_Cross_Released)
			{
				Mgr.GoToMode(TwinStickShooterMode.TwinStickShooterModeFactory);
				return;
			}
			
			if (Mgr.GamePad0_Triangle_Released)
			{
				Mgr.GoToMode(ImageViewerMode.ImageViewerModeFactory);
				return;
			}
			
			if (Mgr.GamePad0_Square_Released)
			{
				Mgr.GoToMode(IsometricMode.IsometricModeFactory);
				return;
			}
			
			if (Mgr.GamePad0_Circle_Released)
			{
				Mgr.GoToMode(SpaceRocketsMode.DrawEngineTestModeFactory);
				return;
			}
			
			if (Mgr.GamePad0_Start_Released)
			{
				//Mgr.SetRunStateToEnding();
				return;
			}
			
			if (Mgr.GamePad0_Select_Pressed)
			{
				Mgr.FreeMemory();
				return;
			}
		}
		
		#endregion
		
		#region Layers and Sprites
		
		private void InitializeLayersAndSprites()
		{
			//Create the layer to draw sprites into.
			ScreenLayer ScreenLayer = DrawEngine2d.Layers.GetOrCreateScreenLayer(2);
			
			Single logoPstnX = ScreenLayer.Camera.Center.X;
			Single logoPstnY = ScreenLayer.Camera.Top + ScreenLayer.Camera.Height * 0.23f;
			Single logoH = ScreenLayer.Camera.Height * 0.15f;
			
			Single rowHeight = ScreenLayer.Camera.Height * 0.1f;
			
			Single buttonRight = ScreenLayer.Camera.Width * 0.2f;
			Single modeLeft = buttonRight + ScreenLayer.Camera.Width * 0.05f;
			
			Single row1 = rowHeight * 3.2f;
			Single row2 = rowHeight * 4.7f;
			Single row3 = rowHeight * 6.2f;
			Single row4 = rowHeight * 7.7f;
			
			//Create the Psm Logo
			TiledTexture logoTT = DrawEngine2d.TiledTextures.CreateTiledTexture(Assets.PsmLogo);
			ColumnKey logoKey = logoTT.CreateColumnIndex(1).GetKey(0);
			Sprite Logo = new Sprite(ScreenLayer, logoKey);
			Logo.SetDimensionsProportionallyByHeight(logoH);
			Logo.SetPosition(logoPstnX, logoPstnY, PsmFramework.Engines.DrawEngine2d.Support.RelativePosition.Bottom);
			
			TiledTexture crossTT = DrawEngine2d.TiledTextures.CreateTiledTexture(Assets.ButtonCross);
			ColumnKey crossKey = crossTT.CreateColumnIndex(1).GetKey(0);
			Sprite cross = new Sprite(ScreenLayer, crossKey);
			cross.SetDimensionsProportionallyByHeight(rowHeight);
			cross.SetPosition(buttonRight, row1, PsmFramework.Engines.DrawEngine2d.Support.RelativePosition.Right);
			
			TiledTexture squareTT = DrawEngine2d.TiledTextures.CreateTiledTexture(Assets.ButtonSquare);
			ColumnKey squareKey = squareTT.CreateColumnIndex(1).GetKey(0);
			Sprite square = new Sprite(ScreenLayer, squareKey);
			square.SetDimensionsProportionallyByHeight(rowHeight);
			square.SetPosition(buttonRight, row2, PsmFramework.Engines.DrawEngine2d.Support.RelativePosition.Right);
			
			TiledTexture triangleTT = DrawEngine2d.TiledTextures.CreateTiledTexture(Assets.ButtonTriangle);
			ColumnKey triangleKey = triangleTT.CreateColumnIndex(1).GetKey(0);
			Sprite triangle = new Sprite(ScreenLayer, triangleKey);
			triangle.SetDimensionsProportionallyByHeight(rowHeight);
			triangle.SetPosition(buttonRight, row3, PsmFramework.Engines.DrawEngine2d.Support.RelativePosition.Right);
			
			TiledTexture circleTT = DrawEngine2d.TiledTextures.CreateTiledTexture(Assets.ButtonCircle);
			ColumnKey circleKey = circleTT.CreateColumnIndex(1).GetKey(0);
			Sprite circle = new Sprite(ScreenLayer, circleKey);
			circle.SetDimensionsProportionallyByHeight(rowHeight);
			circle.SetPosition(buttonRight, row4, PsmFramework.Engines.DrawEngine2d.Support.RelativePosition.Right);
			
			TiledTexture modesTT = DrawEngine2d.TiledTextures.CreateTiledTexture(Assets.Modes);
			GridIndex modesIndex = modesTT.CreateGridIndex(1,4);
			
			GridKey tssKey = modesIndex.GetKey(0, 0);
			Sprite tss = new Sprite(ScreenLayer, tssKey);
			tss.SetDimensionsProportionallyByHeight(rowHeight);
			tss.SetPosition(modeLeft, row1,RelativePosition.Left);
			
			GridKey isoKey = modesIndex.GetKey(0, 1);
			Sprite iso = new Sprite(ScreenLayer, isoKey);
			iso.SetDimensionsProportionallyByHeight(rowHeight);
			iso.SetPosition(modeLeft, row2,RelativePosition.Left);
			
			GridKey ge2dKey = modesIndex.GetKey(0, 2);
			Sprite ge2d = new Sprite(ScreenLayer, ge2dKey);
			ge2d.SetDimensionsProportionallyByHeight(rowHeight);
			ge2d.SetPosition(modeLeft, row3,RelativePosition.Left);
			
			GridKey testKey = modesIndex.GetKey(0, 3);
			Sprite test = new Sprite(ScreenLayer, testKey);
			test.SetDimensionsProportionallyByHeight(rowHeight);
			test.SetPosition(modeLeft, row4,RelativePosition.Left);
			
			Font f = new Font(FontAlias.System, (Int32)rowHeight, FontStyle.Regular);
			Int32 i = f.Metrics.Height;
			String t = "test";
		}
		
		private void CleanupLayersAndSprites()
		{
		}
		
		//private 
		
		#endregion
		
		#region DebugInfo
		
//		protected override void GetAdditionalDebugInfo()
//		{
//			AddDebugInfoLine("Camera Center", DrawEngine2d.WorldCamera.Center);
//			AddDebugInfoLine("Camera Bounds", DrawEngine2d.WorldCamera.Bounds);
//			AddDebugInfoLine("Camera Width", DrawEngine2d.WorldCamera.Bounds.Width);
//		}
		
		#endregion
	}
}

