using System;
using Demo.ImageViewer;
using Demo.Isometric;
using Demo.SpaceRockets;
using Demo.TwinStickShooter;
using Demo.Zombies;
using PsmFramework;
using PsmFramework.Engines.DrawEngine2d.Drawables;
using PsmFramework.Engines.DrawEngine2d.Layers;
using PsmFramework.Engines.DrawEngine2d.Support;
using PsmFramework.Engines.DrawEngine2d.Textures;
using PsmFramework.Modes;

namespace Demo.MainMenu
{
	public class MainMenuMode : DrawEngine2dModeBase
	{
		#region Constructor, Dispose
		
		public MainMenuMode(AppManager mgr)
			: base(mgr)
		{
		}
		
		#endregion
		
		#region Mode Factory Delegate
		
		public static ModeBase MainMenuModeFactory(AppManager mgr)
		{
			return new MainMenuMode(mgr);
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
			ScreenLayer ScreenLayer = DrawEngine2d.GetOrCreateScreenLayer(2);
			
			Single logoPstnX = ScreenLayer.Camera.Center.X;
			Single logoPstnY = ScreenLayer.Camera.Top + ScreenLayer.Camera.Height * 0.17f;
			Single logoH = ScreenLayer.Camera.Height * 0.15f;
			
			Single buttonRight = ScreenLayer.Camera.Width * 0.2f;
			Single buttonHeight = ScreenLayer.Camera.Height * 0.1f;
			
			Single modeLeft = buttonRight + ScreenLayer.Camera.Width * 0.05f;
			
			Single row1 = buttonHeight * 3.0f;
			Single row2 = buttonHeight * 4.5f;
			Single row3 = buttonHeight * 6.0f;
			Single row4 = buttonHeight * 7.5f;
			
			//Create the Psm Logo
			TiledTexture logoTT = DrawEngine2d.CreateTiledTexture(Assets.PsmLogo);
			ColumnKey logoKey = logoTT.CreateColumnIndex(1).GetKey(0);
			Sprite Logo = new Sprite(ScreenLayer, logoKey);
			Logo.SetDimensionsProportionallyFromHeight(logoH);
			Logo.SetPosition(logoPstnX, logoPstnY, PsmFramework.Engines.DrawEngine2d.Support.RelativePosition.Bottom);
			
			TiledTexture crossTT = DrawEngine2d.CreateTiledTexture(Assets.ButtonCross);
			ColumnKey crossKey = crossTT.CreateColumnIndex(1).GetKey(0);
			Sprite cross = new Sprite(ScreenLayer, crossKey);
			cross.SetDimensionsProportionallyFromHeight(buttonHeight);
			cross.SetPosition(buttonRight, row1, PsmFramework.Engines.DrawEngine2d.Support.RelativePosition.Right);
			
			TiledTexture squareTT = DrawEngine2d.CreateTiledTexture(Assets.ButtonSquare);
			ColumnKey squareKey = squareTT.CreateColumnIndex(1).GetKey(0);
			Sprite square = new Sprite(ScreenLayer, squareKey);
			square.SetDimensionsProportionallyFromHeight(buttonHeight);
			square.SetPosition(buttonRight, row2, PsmFramework.Engines.DrawEngine2d.Support.RelativePosition.Right);
			
			TiledTexture triangleTT = DrawEngine2d.CreateTiledTexture(Assets.ButtonTriangle);
			ColumnKey triangleKey = triangleTT.CreateColumnIndex(1).GetKey(0);
			Sprite triangle = new Sprite(ScreenLayer, triangleKey);
			triangle.SetDimensionsProportionallyFromHeight(buttonHeight);
			triangle.SetPosition(buttonRight, row3, PsmFramework.Engines.DrawEngine2d.Support.RelativePosition.Right);
			
			TiledTexture circleTT = DrawEngine2d.CreateTiledTexture(Assets.ButtonCircle);
			ColumnKey circleKey = circleTT.CreateColumnIndex(1).GetKey(0);
			Sprite circle = new Sprite(ScreenLayer, circleKey);
			circle.SetDimensionsProportionallyFromHeight(buttonHeight);
			circle.SetPosition(buttonRight, row4, PsmFramework.Engines.DrawEngine2d.Support.RelativePosition.Right);
			
			TiledTexture modesTT = DrawEngine2d.CreateTiledTexture(Assets.Modes);
			GridIndex modesIndex = modesTT.CreateGridIndex(1,4);
			
			GridKey tssKey = modesIndex.GetKey(0, 0);
			Sprite tss = new Sprite(ScreenLayer, tssKey);
			tss.SetDimensionsProportionallyFromHeight(buttonHeight);
			tss.SetPosition(modeLeft, row1,RelativePosition.Left);
			
			GridKey isoKey = modesIndex.GetKey(0, 1);
			Sprite iso = new Sprite(ScreenLayer, isoKey);
			iso.SetDimensionsProportionallyFromHeight(buttonHeight);
			iso.SetPosition(modeLeft, row2,RelativePosition.Left);
			
			GridKey ge2dKey = modesIndex.GetKey(0, 2);
			Sprite ge2d = new Sprite(ScreenLayer, ge2dKey);
			ge2d.SetDimensionsProportionallyFromHeight(buttonHeight);
			ge2d.SetPosition(modeLeft, row3,RelativePosition.Left);
			
			GridKey testKey = modesIndex.GetKey(0, 3);
			Sprite test = new Sprite(ScreenLayer, testKey);
			test.SetDimensionsProportionallyFromHeight(buttonHeight);
			test.SetPosition(modeLeft, row4,RelativePosition.Left);
		}
		
		private void CleanupLayersAndSprites()
		{
		}
		
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

