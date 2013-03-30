using System;
using System.Collections.Generic;

namespace PsmFramework.Engines.DrawEngine2d.Textures
{
	public sealed class ColumnIndex : IndexBase
	{
		#region Constructor, Dispose
		
		public ColumnIndex(TiledTexture tiledTexture, String name = DefaultName, Int32 columns = 1)
			: base(tiledTexture, name)
		{
			BuildTileList(columns);
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
			Tiles = new Dictionary<Int32, Texture2dArea>();
		}
		
		private void CleanupTiles()
		{
			Tiles.Clear();
			Tiles = null;
		}
		
		private Dictionary<Int32, Texture2dArea> Tiles;
		
		private void BuildTileList(Int32 columns)
		{
			if(columns < 1 || columns > TiledTexture.Texture.Width)
				throw new ArgumentOutOfRangeException();
			
			if(TiledTexture.Texture.Width % columns != 0)
				throw new ArgumentOutOfRangeException();
			
			Int32 tileWidth = TiledTexture.Texture.Width / columns;
			Int32 tileHeight = TiledTexture.Texture.Height;
			
			for(Int32 c = 0; c < columns; c++)
			{
				Int32 left = c * tileWidth;
				Int32 top = 0;
				Int32 right = left + tileWidth;
				Int32 bottom = tileHeight;
				
				Texture2dArea area = new Texture2dArea(left, top, right, bottom, TiledTexture.Texture.Width, TiledTexture.Texture.Height);
				
				Tiles.Add(c, area);
			}
		}
		
		public Texture2dArea GetTextureCoordinates(Int32 column)
		{
			return Tiles[column];
		}
		
		#endregion
	}
}

