using System;
using System.Collections.Generic;

namespace PsmFramework.Engines.DrawEngine2d.Textures
{
	public class GridIndex : IndexBase
	{
		#region Constructor, Dispose
		
		public GridIndex(TiledTexture tiledTexture, Int32 columns = DefaultColumns, Int32 rows = DefaultRows, String name = DefaultName)
			: base(name, tiledTexture)
		{
			BuildTileList(columns, rows);
		}
		
		#endregion
		
		#region Initialize, Cleanup
		
		protected override void Initialize()
		{
			InitializeTiles();
		}
		
		protected override void Cleanup()
		{
			CleanupTiles();
		}
		
		#endregion
		
		#region Type
		
		public override IndexType Type { get { return IndexType.Column; } }
		
		#endregion
		
		#region Tiles
		
		private void InitializeTiles()
		{
			Tiles = new Dictionary<GridLocation, Texture2dArea>();
		}
		
		private void CleanupTiles()
		{
			Tiles.Clear();
			Tiles = null;
		}
		
		private Dictionary<GridLocation, Texture2dArea> Tiles;
		
		private void BuildTileList(Int32 columns, Int32 rows)
		{
			//At least 1
			if(columns < 1 || rows < 1)
				throw new ArgumentOutOfRangeException();
			
			//Can't be greater than number of pixels
			if(columns > TiledTexture.Texture.Width || rows > TiledTexture.Texture.Height)
				throw new ArgumentOutOfRangeException();
			
			//Must be evenly divisible
			if(TiledTexture.Texture.Width % columns != 0 || TiledTexture.Texture.Height % rows != 0)
				throw new ArgumentOutOfRangeException();
			
			Int32 tileWidth = TiledTexture.Texture.Width / columns;
			Int32 tileHeight = TiledTexture.Texture.Height / rows;
			
			for(Int32 r = 0; r < rows; r++)
			{
				for(Int32 c = 0; c < columns; c++)
				{
					Int32 left = c * tileWidth;
					Int32 right = left + tileWidth;
					
					Int32 top = r * tileHeight;
					Int32 bottom = top + tileHeight;
					
					Texture2dArea area = new Texture2dArea(left, top, right, bottom, TiledTexture.Texture.Width, TiledTexture.Texture.Height);
					
					GridLocation loc = new GridLocation(c, r);
					
					Tiles.Add(loc, area);
				}
			}
		}
		
		public Texture2dArea GetTextureCoordinates(Int32 column, Int32 row)
		{
			GridLocation loc = new GridLocation(column, row);
			
			return Tiles[loc];
		}
		
		public const Int32 DefaultColumns = 1;
		
		public const Int32 DefaultRows = 1;
		
		#endregion
		
		#region GridLocation
		
		private struct GridLocation
		{
			public GridLocation(Int32 column, Int32 row)
			{
				Column = column;
				Row = row;
			}
			
			public readonly Int32 Column;
			public readonly Int32 Row;
		}
		
		#endregion
	}
}

