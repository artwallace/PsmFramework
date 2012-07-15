using System;

namespace PsmFramework.Modes.Isometric2d
{
	public struct BackgroundTileData
	{
		public Int32 AssetIndexColumn;
		public Int32 AssetIndexRow;
		public Boolean Passable;
		public Single Friction;
		
		public BackgroundTileData(Int32 assetColumn, Int32 assetRow, Boolean passable = true, Single friction = 0f)
		{
			AssetIndexColumn = assetColumn;
			AssetIndexRow = assetRow;
			Passable = passable;
			Friction = friction;
		}
	}
}

