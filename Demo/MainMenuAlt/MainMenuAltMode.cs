using System;
using Demo.Fireworks;
using Demo.MainMenu;
using Demo.SpaceRockets;
using Demo.TwinStickShooter;
using Demo.Zombies;
using PsmFramework;
using PsmFramework.Engines.DrawEngine2d.Drawables;
using PsmFramework.Engines.DrawEngine2d.Layers;
using PsmFramework.Engines.DrawEngine2d.Support;
using PsmFramework.Engines.DrawEngine2d.Textures;
using PsmFramework.Modes;

namespace Demo.MainMenuAlt
{
	public class MainMenuAltMode : DrawEngine2dModeBase
	{
		#region Constructor, Dispose
		
		public MainMenuAltMode(AppManager mgr)
			: base(mgr)
		{
		}
		
		#endregion
		
		#region Mode Factory Delegate
		
		public static ModeBase MainMenuAltModeFactory(AppManager mgr)
		{
			return new MainMenuAltMode(mgr);
		}
		
		#endregion
		
		#region Mode Logic
		
		protected override void Initialize()
		{
			InitializeLayersAndSprites();
		}
		
		protected override void Cleanup()
		{
			CleanupLayersAndSprites();
		}
		
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
			
			if (Mgr.GamePad0_Cross_Pressed)
			{
				Mgr.GoToMode(TwinStickShooterMode.TwinStickShooterModeFactory);
				return;
			}
			
			if (Mgr.GamePad0_Triangle_Pressed)
			{
				Mgr.GoToMode(FireworksMode.FireworksModeFactory);
				return;
			}
			
			if (Mgr.GamePad0_Square_Pressed)
			{
				Mgr.GoToMode(ZombieMode.ZombieModeFactory);
				return;
			}
			
			if (Mgr.GamePad0_Circle_Pressed)
			{
				Mgr.GoToMode(SpaceRocketsMode.DrawEngineTestModeFactory);
				return;
			}
			
			if (Mgr.GamePad0_Start_Pressed)
			{
				Mgr.GoToMode(MainMenuMode.MainMenuModeFactory);
				return;
			}
		}
		
		#endregion
		
		#region Layers and Sprites
		
		private void InitializeLayersAndSprites()
		{
			DebugInfoEnabled = true;
			//DebugInfoForcesRender = false; //TODO: Remove this when swapbuffers bug is fixed.
			
			//Create the layer to draw sprites into.
			ScreenLayer = DrawEngine2d.GetOrCreateScreenLayer(2);
			
			//Load the logo png into a texture and create a single tile.
			Texture2dPlus LogoTexture = new Texture2dPlus(DrawEngine2d, TextureCachePolicy.DisposeAfterLastUse, Assets.Logo);
			TiledTexture LogoTiledTexture = new TiledTexture(DrawEngine2d, Assets.Logo, LogoTexture);
			ColumnIndex ci = LogoTiledTexture.CreateColumnIndex(1);
			IndexKey key = new IndexKey(ci, 0);
			
			//Create the sprite and add it to the layer.
			SpriteGroup LogoSpriteGroup = new SpriteGroup(ScreenLayer, LogoTiledTexture);
			SpriteGroupItem Logo = new SpriteGroupItem(LogoSpriteGroup, key);
			Logo.SetPositionFromCenter(ScreenLayer.Camera.Center);
		}
		
		private void CleanupLayersAndSprites()
		{
			ScreenLayer = null;
		}
		
		private const Int32 ScreenLayerId = 2;
		
		private ScreenLayer ScreenLayer;
		
		#endregion
	}
}

