using System;
using Sce.PlayStation.Core;

namespace PsmFramework.Modes.Isometric2d
{
	public class DiamondBackground : BackgroundBase
	{
		#region Constructor
		
		public DiamondBackground(RoomBase room, Int32 columns, Int32 rows, String asset, Int32 assetColumns, Int32 assetRows, Int32 tileWidth, Int32 tileHeight, Int32 horizontalScreenPadding, Int32 verticalScreenPadding)
			: base(room, columns, rows, asset, assetColumns, assetRows, tileWidth, tileHeight, horizontalScreenPadding, verticalScreenPadding)
		{
		}
		
		#endregion
		
		#region Update
		
		private Boolean FirstUpdate = true;
		private Vector2 LastUpdateCameraPstn;
		
		internal override void Update()
		{
			//Only update if something's changed.
			if (LastUpdateCameraPstn == Mode.CameraLowerLeftPosition && !FirstUpdate)
				return;
			FirstUpdate = false;
			LastUpdateCameraPstn = Mode.CameraLowerLeftPosition;
		}
		
		#endregion
		
		#region Size
		
		protected override void CalcDimensions()
		{
			//TODO: Wrong Calculation!
			Width = Columns * AssetTileWidth;
			Height = Rows * AssetTileHeight;
		}
		
		#endregion
		
		#region SpriteList
		
		protected override void CalculateTilesNeededToFillScreen()
		{
			//TODO: Wrong Calculation!
			SpriteColumns = Convert.ToInt32(Mgr.ScreenWidth / AssetTileWidth * 2) + 2;
			SpriteRows = Convert.ToInt32(Mgr.ScreenHeight / AssetTileHeight) + 2;
		}
		
		#endregion
		
		#region Factory Delegate
		
		public static BackgroundBase DiamondBackgroundFactory(RoomBase room, Int32 columns, Int32 rows, String asset, Int32 assetColumns, Int32 assetRows, Int32 tileWidth, Int32 tileHeight, Int32 horizontalScreenPadding, Int32 verticalScreenPadding)
		{
			return new DiamondBackground(room, columns, rows, asset, assetColumns, assetRows, tileWidth, tileHeight, horizontalScreenPadding, verticalScreenPadding);
		}
		
		#endregion
	}
}

