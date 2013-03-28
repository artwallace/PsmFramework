using System;
using PsmFramework.Engines.CyclonePhysics2d.Forces;
using PsmFramework.Modes.TopDown2d;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

namespace Demo.TwinStickShooter
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
			CreateShip();
			SetCameraSubject(Ship);
		}
		
		public override void Cleanup()
		{
			SetCameraSubject(null);
		}
		
		public override void Update()
		{
			//Actors are updated in base class. Just change room stuff here.
			
			//TODO: This shouldn't be here.
			ShipDrag.UpdateForce(Mgr.TimeSinceLastFrame.Ticks);
			//ShipFriction.UpdateForce(Mgr.TimeSinceLastFrame.Ticks);
			
			//This should be moved to an door actor
			if (Ship != null)
				if (Background.GetTileFromRoomPostion(Ship.Position) == DoorToNextRoom)
					Level.AdvanceToNextRoom();
		}
		
		private Vector2i DoorToNextRoom = new Vector2i(6,1);
		
		#endregion
		
		#region Background
		
		protected override String BackgroundAsset { get { return Assets.Image_BackgroundTileTest; } }
		protected override Int32 BackgroundAssetColumns { get { return Assets.Image_BackgroundTileTestColumns; } }
		protected override Int32 BackgroundAssetRows { get { return Assets.Image_BackgroundTileTestRows; } }
		protected override Int32 BackgroundAssetTileWidth { get { return Assets.Image_BackgroundTileTestWidth; } }
		protected override Int32 BackgroundAssetTileHeight { get { return Assets.Image_BackgroundTileTestHeight; } }
		
		protected override Int32 BackgroundTileColumns { get { return 40; } }
		protected override Int32 BackgroundTileRows { get { return 30; } }
		
		public override BackgroundTileData GetBackgroundTileData(Int32 column, Int32 row)
		{
			if (column < 0 || column >= BackgroundTileColumns)
				throw new ArgumentOutOfRangeException();
			if (row < 0 || row >= BackgroundTileRows)
				throw new ArgumentOutOfRangeException();
			
			Int32 assetCol = 0;
			Int32 assetRow = 0;
			
			if (column == 2 && row == 2)
			{
				assetCol = 1;
				assetRow = 0;
			}
			else if (column == DoorToNextRoom.X && row == DoorToNextRoom.Y)
			{
				assetCol = 3;
				assetRow = 0;
			}
			else if (column == 0 || column == BackgroundTileColumns - 1 || column == 15)
			{
				assetCol = 1;
				assetRow = 0;
			}
			else if (row == 0 || row == BackgroundTileRows - 1)
			{
				assetCol = 1;
				assetRow = 0;
			}
			
			BackgroundTileData data = new BackgroundTileData(assetCol, assetRow);
			
			return data;
		}
		
		#endregion
		
		#region Actors
		
		private Actor Ship;
		
		private Drag2d ShipDrag;
		
		//private Friction2d ShipFriction;
		
		private void CreateShip()
		{
			Mode.TextureManager.AddTextureAsset(Assets.Image_Ship64, this);
			Ship = new Actor(this, Assets.Image_Ship64, Background.GetTilePositionAtCenter(6, 2), 300f, 0.9999f);
			//Ship.SetPosition(Background.GetTilePositionAtCenter(6, 2));
			AddActor(Ship);
			Ship.SetMovementStrategy(new PlayerInputMovementStrategy(Ship, true, true));
			
			ShipDrag = new Drag2d(Ship, .01f, .05f);
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

