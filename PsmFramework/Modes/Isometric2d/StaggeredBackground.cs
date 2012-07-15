using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

namespace PsmFramework.Modes.Isometric2d
{
	public class StaggeredBackground : BackgroundBase
	{
		#region Constructor
		
		public StaggeredBackground(RoomBase room, Int32 columns, Int32 rows, String asset, Int32 assetColumns, Int32 assetRows, Int32 tileWidth, Int32 tileHeight, Int32 horizontalScreenPadding, Int32 verticalScreenPadding)
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
			
			Vector2i lowerLeftPstn = GetTileFromRoomPostion(Mode.CameraLowerLeftPosition);
			
			Int32 lowX = System.Math.Max(lowerLeftPstn.X, 0);
			Int32 lowY = System.Math.Max(lowerLeftPstn.Y, 0);
			
			Int32 highX = System.Math.Min(lowX + SpriteColumns, Columns);
			Int32 highY = System.Math.Min(lowY + SpriteRows, Rows);
			
			Int32 spriteIndex = 0;
			
			for (Int32 c = lowX; c < highX; c++)
			{
				for (Int32 r = lowY; r < highY; r++)
				{
					UpdateSprite(spriteIndex, c, r);
					spriteIndex++;
				}
			}
			
			//Move the leftover sprites offscreen.
			for (Int32 i = spriteIndex; i < TileSpriteList.Sprites.Length; i++)
				HideSpiteOffscreen(i);
		}
		
		#endregion
		
		#region Size
		
		protected override void CalcDimensions()
		{
			//TODO: Wrong Calculation!
			Width = AssetTileWidth + ((Columns - 1) * (AssetTileWidth / 2)) + (HorizontalScreenPadding * 2);
			Height = Rows * AssetTileHeight;
		}
		
		#endregion
		
		#region SpriteList
		
		protected override void CalculateTilesNeededToFillScreen()
		{
			//TODO: Wrong Calculation!
			Single c = Mgr.ScreenWidth / (AssetTileWidth / 2) + 2;
			
			SpriteColumns = System.Math.Min(Convert.ToInt32(c), Columns);
			SpriteRows = System.Math.Min(Convert.ToInt32(Mgr.ScreenHeight / AssetTileHeight) + 2, Rows);
		}
		
		#endregion
		
		#region Factory Delegate
		
		public static BackgroundBase StaggeredBackgroundFactory(RoomBase room, Int32 columns, Int32 rows, String asset, Int32 assetColumns, Int32 assetRows, Int32 tileWidth, Int32 tileHeight, Int32 horizontalScreenPadding, Int32 verticalScreenPadding)
		{
			return new StaggeredBackground(room, columns, rows, asset, assetColumns, assetRows, tileWidth, tileHeight, horizontalScreenPadding, verticalScreenPadding);
		}
		
		#endregion
	}
}

