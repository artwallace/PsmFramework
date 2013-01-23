using System;
using Demo.MainMenu;
using PsmFramework;
using PsmFramework.Engines.DrawEngine2d;
using PsmFramework.Engines.DrawEngine2d.Drawables;
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
			SpriteGroupItem sss1 = new SpriteGroupItem(sssg, new TiledTextureIndex(0));
			sss1.SetPositionFromCenter(new Coordinate2(32f, 32f));
			SpriteGroupItem sss2 = new SpriteGroupItem(sssg, new TiledTextureIndex(0));
			sss2.SetPositionFromCenter(new Coordinate2(96f, 32f));
			//sss2.Rotation = 45.0f;
			
			Texture2dPlus testT2d = DrawEngine2d.GetTexture(DebugFont.TextureKey);
			TiledTexture ttTest = new TiledTexture(DrawEngine2d, TextureCachePolicy.DisposeAfterLastUse, "test", testT2d);
			ttTest.CreateColumnIndex(1);
			SpriteGroup testSG = new SpriteGroup(l2, ttTest);
			SpriteGroupItem testSS = new SpriteGroupItem(testSG, new TiledTextureIndex(0));
			testSS.Position = new Coordinate2(32f, 200f);
			
			LayerBase debugOverlay = DrawEngine2d.GetOrCreateScreenLayer(2);
			_DebugTextLabel = new DebugLabel(debugOverlay);
			_DebugTextLabel.Text = "I";
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
		}
		
		#endregion
		
		#region Mode Factory Delegate
		
		public static ModeBase DrawEngineTestModeFactory(AppManager mgr)
		{
			return new SpaceRocketsMode(mgr);
		}
		
		#endregion
		
		private DebugLabel _DebugTextLabel;
	}
}

