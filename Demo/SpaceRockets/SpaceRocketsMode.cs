using System;
using Demo.MainMenu;
using PsmFramework;
using PsmFramework.Engines.DrawEngine2d;
using PsmFramework.Engines.DrawEngine2d.Drawables;
using PsmFramework.Engines.DrawEngine2d.Layers;
using PsmFramework.Engines.DrawEngine2d.Support;
using PsmFramework.Engines.DrawEngine2d.Textures;
using PsmFramework.Modes;
using PsmFramework.Modes.TopDown2dAlt;

namespace Demo.SpaceRockets
{
	public class SpaceRocketsMode : TopDown2dAltModeBase
	{
		#region Constructor
		
		public SpaceRocketsMode(AppManager mgr)
			: base(mgr)
		{
		}
		
		#endregion
		
		#region Mode Logic
		
		protected override void Initialize()
		{
			//TODO: Remove this after testing!
			DrawEngine2d.ClearColor = Colors.Blue;
			//EnableDebugInfo();
			
			String shipSprite = "/Application/TwinStickShooter/Images/Ship64.png";
			Texture2dPlus t2d = new Texture2dPlus(DrawEngine2d, TextureCachePolicy.DisposeAfterLastUse, shipSprite);
			TiledTexture tt = new TiledTexture(DrawEngine2d, TextureCachePolicy.DisposeAfterLastUse, shipSprite, t2d);
			tt.CreateColumnIndex(1);
			
			LayerBase l2 = DrawEngine2d.GetOrCreateWorldLayer(1);
			
			SpriteGroup sssg = new SpriteGroup(l2, tt);
			_Ship1 = new SpriteGroupItem(sssg, new TiledTextureIndex(0));
			_Ship1.SetPositionFromCenter(new Coordinate2(0f, 0f));
			SpriteGroupItem sss2 = new SpriteGroupItem(sssg, new TiledTextureIndex(0));
			sss2.SetPositionFromCenter(new Coordinate2(96f, 32f));
			//sss2.Rotation = 45.0f;
			
//			Texture2dPlus testT2d = DrawEngine2d.GetTexture(DebugFont.TextureKey);
//			TiledTexture ttTest = new TiledTexture(DrawEngine2d, TextureCachePolicy.DisposeAfterLastUse, "test", testT2d);
//			ttTest.CreateColumnIndex(1);
//			SpriteGroup testSG = new SpriteGroup(l2, ttTest);
//			SpriteGroupItem testSS = new SpriteGroupItem(testSG, new TiledTextureIndex(0));
//			testSS.Position = new Coordinate2(32f, 200f);
			
			LayerBase debugOverlay = DrawEngine2d.GetOrCreateScreenLayer(2);
			_DebugTextLabel = new DebugLabel(debugOverlay);
			_DebugTextLabel.Text = "This text is in a world layer\nbut it should be in a screen layer.";
			_DebugTextLabel.Position = new Coordinate2(100.0f, 100.0f);
		}
		
		protected override void Cleanup()
		{
			_DebugTextLabel.Dispose();
			_DebugTextLabel = null;
		}
		
		public override void Update()
		{
			if (Mgr.GamePad0_Start_Pressed && Mgr.ModeChangeAllowed)
			{
				Mgr.GoToMode(MainMenuMode.MainMenuModeFactory);
				return;
			}
			
			if (Mgr.GamePad0_Select_Pressed)
			{
				if (Mgr.RunState == RunState.Running)
					Mgr.SetRunStateToPaused();
				else
					Mgr.SetRunStateToRunning();
			}
			
			Single xMove = 0.0f;
			Single yMove = 0.0f;
			if(Math.Abs(Mgr.GamePadData.AnalogLeftX) > 0.1f)
				xMove = Mgr.GamePadData.AnalogLeftX * 2;
			if(Math.Abs(Mgr.GamePadData.AnalogLeftY) > 0.1f)
				yMove = Mgr.GamePadData.AnalogLeftY * 2;
			_Ship1.Position = new Coordinate2(_Ship1.Position.X + xMove, _Ship1.Position.Y + yMove);
			
			
			Single xCam = 0.0f;
			Single yCam = 0.0f;
			if (Mgr.GamePad0_Left)
				xCam = -1f;
			else if (Mgr.GamePad0_Right)
				xCam = 1f;
			if (Mgr.GamePad0_Up)
				yCam = -1f;
			else if (Mgr.GamePad0_Down)
				yCam = 1f;
			DrawEngine2d.WorldCamera.Center = new Coordinate2(DrawEngine2d.WorldCameraPosition.X + xCam, DrawEngine2d.WorldCameraPosition.Y + yCam);
			
			if (Mgr.GamePad0_L1)
				DrawEngine2d.WorldCamera.Rotation -= 1.0f;
			else if (Mgr.GamePad0_R1)
				DrawEngine2d.WorldCamera.Rotation += 1.0f;
			
			if (Mgr.GamePad0_Triangle)
				DrawEngine2d.WorldCamera.Zoom += 0.1f;
			else if (Mgr.GamePad0_Cross)
				DrawEngine2d.WorldCamera.Zoom -= 0.1f;
		}
		
		#endregion
		
		#region Mode Factory Delegate
		
		public static ModeBase DrawEngineTestModeFactory(AppManager mgr)
		{
			return new SpaceRocketsMode(mgr);
		}
		
		#endregion
		
		private DebugLabel _DebugTextLabel;
		
		private SpriteGroupItem _Ship1;
	}
}

