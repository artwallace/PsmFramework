using System;
using PsmFramework.Modes.Isometric2d;
using Sce.PlayStation.Core;

namespace Demo.Zombies
{
	public class Level1Room1 : RoomBase
	{
		#region Constructor, Dispose
		
		protected Level1Room1(LevelBase level)
			: base(level)
		{
		}
		
		#endregion
		
		#region Room Logic
		
		public override void Initialize()
		{
			//CreateShip();
			//SetCameraSubject(Ship);
		}
		
		public override void Cleanup()
		{
			//SetCameraSubject(null);
		}
		
		public override void Update()
		{
			//Actors are updated in base class. Just change room stuff here.
			MoveCameraFromGamePad();
		}
		
		private void MoveCameraFromGamePad()
		{
			Vector2 move = Vector2.Zero;
			
			if (Mgr.GamePad0_Up)
				move.Y += 1f * Mgr.TimeSinceLastFrame.Ticks;
			else if (Mgr.GamePad0_Down)
				move.Y -= 1f * Mgr.TimeSinceLastFrame.Ticks;
			
			if (Mgr.GamePad0_Left)
				move.X -= 1f * Mgr.TimeSinceLastFrame.Ticks;
			else if (Mgr.GamePad0_Right)
				move.X += 1f * Mgr.TimeSinceLastFrame.Ticks;
			
			MoveCamera(move);
		}
		
		#endregion
		
		#region Background
		
		protected override CreateBackgroundDelegate BackgroundDelegate { get { return StaggeredBackground.StaggeredBackgroundFactory; } }
		
		protected override String BackgroundAsset { get { return Assets.Zombie_Background; } }
		protected override Int32 BackgroundAssetColumns { get { return Assets.Zombie_BackgroundTileTestColumns; } }
		protected override Int32 BackgroundAssetRows { get { return Assets.Zombie_BackgroundTileTestRows; } }
		protected override Int32 BackgroundAssetTileWidth { get { return Assets.Zombie_BackgroundTileTestWidth; } }
		protected override Int32 BackgroundAssetTileHeight { get { return Assets.Zombie_BackgroundTileTestHeight; } }
		protected override Int32 BackgroundTileColumns { get { return 14; } }
		protected override Int32 BackgroundTileRows { get { return 6; } }
		protected override Int32 BackgroundHorizontalScreenPadding { get { return 15; } }
		protected override Int32 BackgroundVerticalScreenPadding { get { return 15; } }
		
		public override BackgroundTileData GetBackgroundTileData(Int32 column, Int32 row)
		{
			if (column < 0 || column >= BackgroundTileColumns)
				throw new ArgumentOutOfRangeException();
			if (row < 0 || row >= BackgroundTileRows)
				throw new ArgumentOutOfRangeException();
			
			Int32 assetCol = 1;
			Int32 assetRow = 0;
			
			if (column == 3 && row == 1)
			{
				assetCol = 3;
				assetRow = 0;
			}
			else if (column == 0 || column == BackgroundTileColumns - 1 || column == 15)
			{
				assetCol = 2;
				assetRow = 0;
			}
			else if (column % 2 == 0 && row == 0)
			{
				assetCol = 2;
				assetRow = 0;
			}
			else if (column % 2 != 0 && row == BackgroundTileRows - 1)
			{
				assetCol = 2;
				assetRow = 0;
			}
			
			BackgroundTileData data = new BackgroundTileData(assetCol, assetRow);
			
			return data;
		}
		
		#endregion
		
		#region Actors
		
		//private Actor Ship;
		
		private void CreateShip()
		{
			//Mode.TextureManager.AddTextureAsset(AppAssets.Image_Ship64, this);
			//Ship = new Actor(this, AppAssets.Image_Ship64, Background.GetTilePositionAtCenter(6, 2), 300f, 0.9999f);
			//Ship.SetPosition(Background.GetTilePositionAtCenter(6, 2));
			//AddActor(Ship);
			//Ship.SetMovementStrategy(new PlayerInputMovementStrategy(Ship, true, true));
			
			//ShipDrag = new Drag2d(Ship, .5f, 1f);
			//ShipFriction = new Friction2d(Ship);
		}
		
		#endregion
		
		#region Room Factory Delegate
		
		public static RoomBase Level1Room1Factory(LevelBase level)
		{
			return new Level1Room1(level);
		}
		
		#endregion
	}
}
