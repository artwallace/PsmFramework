using System;
using Demo.Fireworks;
using Demo.SpaceRockets;
using Demo.TwinStickShooter;
using Demo.Zombies;
using PsmFramework;
using PsmFramework.Modes;
using Sce.PlayStation.Core;
using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

namespace Demo.MainMenu
{
	public class MainMenuMode : GameEngine2dModeBase
	{
		protected override UInt32 SpritesCapacity { get { return 50; } }
		protected override UInt32 DrawHelpersCapacity { get { return 50; } }
		protected override Vector4 ClearColor { get { return Colors.Black; } }
		protected override Boolean DrawDebugGrid { get { return false; } }
		
		#region Constructor, Dispose
		
		protected MainMenuMode(AppManager mgr)
			: base(mgr)
		{
		}
		
		#endregion
		
		#region Mode Logic
		
		protected override void Initialize()
		{
			EnableDebugInfo();
			
			InitializeLogo();
		}
		
		protected override void Cleanup()
		{
			CleanupLogo();
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
				Mgr.GoToTitleScreenMode();
				return;
			}
		}
		
		#endregion
		
		#region Logo
		
		private SpriteUV LogoSprite;
		
		private void InitializeLogo()
		{
			TextureManager.AddTextureAsset(Assets.Image_Logo, this);
			LogoSprite = TextureManager.CreateSpriteUV(Assets.Image_Logo);
			LogoSprite.Position = GameScene.Camera2D.Center;
			AddToScene(LogoSprite);
		}
		
		private void CleanupLogo()
		{
			RemoveFromScene(LogoSprite);
			LogoSprite.Cleanup();
			LogoSprite = null;
			TextureManager.RemoveAllTexturesForUser(this);
		}
		
		#endregion
		
		#region Mode Factory Delegate
		
		public static ModeBase MainMenuModeFactory(AppManager mgr)
		{
			return new MainMenuMode(mgr);
		}
		
		#endregion
	}
}
