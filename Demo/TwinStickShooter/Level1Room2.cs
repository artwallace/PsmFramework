using System;
using PsmFramework.Modes.TopDown2d;

namespace Demo.TwinStickShooter
{
	public class Level1Room2 : RoomBase
	{
		#region Constructor, Dispose
		
		protected Level1Room2(LevelBase level)
			: base(level)
		{
		}
		
		#endregion
		
		#region Room Logic
		
		public override void Initialize()
		{
			CreateShip();
		}
		
		public override void Cleanup()
		{
			SetCameraSubject(null);
		}
		
		public override void Update()
		{
			//Actors are updated in base class. Just change room stuff here.
		}
		
		#endregion
		
		#region Background
		
		protected override String BackgroundAsset { get { return Assets.Image_BackgroundTileTest; } }
		protected override Int32 BackgroundAssetColumns { get { return Assets.Image_BackgroundTileTestColumns; } }
		protected override Int32 BackgroundAssetRows { get { return Assets.Image_BackgroundTileTestRows; } }
		protected override Int32 BackgroundAssetTileWidth { get { return Assets.Image_BackgroundTileTestWidth; } }
		protected override Int32 BackgroundAssetTileHeight { get { return Assets.Image_BackgroundTileTestHeight; } }
		
		protected override Int32 BackgroundTileColumns { get { return 8; } }
		protected override Int32 BackgroundTileRows { get { return 7; } }
		
		public override BackgroundTileData GetBackgroundTileData(Int32 column, Int32 row)
		{
			if (column < 0 || column > BackgroundTileColumns)
				throw new ArgumentOutOfRangeException();
			if (row < 0 || row > BackgroundTileRows)
				throw new ArgumentOutOfRangeException();
			
			Int32 assetCol = 1;
			Int32 assetRow = 0;
			
			if (column == 2 && row == 2)
			{
				assetCol = 3;
				assetRow = 0;
			}
			else if (column == 0 || column == BackgroundTileColumns - 1)
			{
				assetCol = 0;
				assetRow = 0;
			}
			else if (row == 0 || row == BackgroundTileRows - 1)
			{
				assetCol = 0;
				assetRow = 0;
			}
			
			BackgroundTileData data = new BackgroundTileData(assetCol, assetRow);
			
			return data;
		}
		
		#endregion
		
		#region Actors
		
		private void CreateShip()
		{
			Mode.TextureManager.AddTextureAsset(Assets.Image_Ship64, this);
			Actor actor = new Actor(this, Assets.Image_Ship64, Background.GetTilePositionAtCenter(4, 2), 200f, 0.99f);
			//actor.SetPosition(Background.GetTilePositionAtCenter(4, 2));
			AddActor(actor);
			actor.SetMovementStrategy(new PlayerInputMovementStrategy(actor, true, true));
			
			SetCameraSubject(actor);
		}
		
		#endregion
		
		#region Room Factory Delegate
		
		public static RoomBase Level1Room2Factory(LevelBase level)
		{
			return new Level1Room2(level);
		}
		
		#endregion
	}
}

