using System;
using Demo.Fireworks;
using Demo.SpaceRockets;
using Demo.TwinStickShooter;
using Demo.Zombies;
using PsmFramework;
using PsmFramework.Modes;
using Sce.PlayStation.Core;

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
		
		#region Mode Logic
		
		protected override void Initialize()
		{
			//EnableDebugInfo();
			
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
		}
		
		#endregion
		
		#region Logo
		
//		private SpriteUV LogoSprite;
		
		private void InitializeLogo()
		{
//			TextureManager.AddTextureAsset(Assets.Image_Logo, this);
//			LogoSprite = TextureManager.CreateSpriteUV(Assets.Image_Logo);
//			LogoSprite.Position = GameScene.Camera2D.Center;
//			AddToScene(LogoSprite);
		}
		
		private void CleanupLogo()
		{
//			RemoveFromScene(LogoSprite);
//			LogoSprite.Cleanup();
//			LogoSprite = null;
//			TextureManager.RemoveAllTexturesForUser(this);
		}
		
		#endregion
		
		#region Mode Factory Delegate
		
		public static ModeBase MainMenuAltModeFactory(AppManager mgr)
		{
			return new MainMenuAltMode(mgr);
		}
		
		#endregion
	}
}

