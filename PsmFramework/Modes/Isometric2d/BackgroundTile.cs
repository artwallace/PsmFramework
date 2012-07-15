using System;
using PsmFramework;
using Sce.PlayStation.Core;
using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

namespace PsmFramework.Modes.Isometric2d
{
	public struct BackgroundTile
	{
		public Int32 Column;
		public Int32 Row;
		
		public Int32 AssetIndexColumn;
		public Int32 AssetIndexRow;
		public Vector2i AssetIndex;
		
		public Boolean Passable;
		public Single Friction;
		
		#region Constructors
		
		public BackgroundTile(Int32 column, Int32 row, Int32 assetIndexCol, Int32 assetIndexRow, Boolean passable, Single friction)
		{
			Column = column;
			Row = row;
			AssetIndexColumn = assetIndexCol;
			AssetIndexRow = assetIndexRow;
			AssetIndex = new Vector2i(assetIndexCol, assetIndexRow);
			Passable = passable;
			Friction = friction;
		}
		
		public BackgroundTile(Int32 column, Int32 row, BackgroundTileData data)
		{
			Column = column;
			Row = row;
			AssetIndexColumn = data.AssetIndexColumn;
			AssetIndexRow = data.AssetIndexRow;
			AssetIndex = new Vector2i(data.AssetIndexColumn, data.AssetIndexRow);
			Passable = data.Passable;
			Friction = data.Friction;
		}
		
		#endregion
	}
}

