using System;

namespace PsmFramework.Modes.Isometric2d
{
	public struct BackgroundAsset
	{
		public String ImagePath;
		
		public Int32 Columns;
		public Int32 Rows;
		
		public Int32 TileWidth;
		public Int32 TileHeight;
		
		#region Constructor
		
		public BackgroundAsset (String imagePath, Int32 columns, Int32 rows, Int32 tileWidth, Int32 tileHeight)
		{
			ImagePath = imagePath;
			
			Columns = columns;
			Rows = rows;
			
			TileWidth = tileWidth;
			TileHeight = tileHeight;
		}
		
		#endregion
	}
}

