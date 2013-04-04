using System;
using System.Collections.Generic;
using PsmFramework.Engines.DrawEngine2d.Support;

namespace PsmFramework.Engines.DrawEngine2d.Textures
{
	public sealed class GridIndex : IndexBase
	{
		#region Constructor, Dispose
		
		public GridIndex(TiledTexture tiledTexture, Int32 columns = DefaultColumns, Int32 rows = DefaultRows, String name = DefaultName)
			: base(name, tiledTexture)
		{
			BuildKeyList(columns, rows);
		}
		
		#endregion
		
		#region Initialize, Cleanup
		
		protected override void Initialize()
		{
			InitializeKeys();
		}
		
		protected override void Cleanup()
		{
			CleanupKeys();
		}
		
		#endregion
		
		#region Keys
		
		private void InitializeKeys()
		{
			Keys = new Dictionary<GridLocation, GridKey>();
		}
		
		private void CleanupKeys()
		{
			//TODO: Not sure if this is a good idea. Leaves drawables with disposed keys, right or wrong.
			foreach(GridKey key in Keys.Values)
				key.Dispose();
			Keys.Clear();
			Keys = null;
		}
		
		public const Int32 DefaultColumns = 1;
		public const Int32 DefaultRows = 1;
		
		private Dictionary<GridLocation, GridKey> Keys;
		
		private void BuildKeyList(Int32 columns, Int32 rows)
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
					
					GridKey key = new GridKey(this, c, r, area);
					Keys.Add(loc, key);
				}
			}
		}
		
		public GridKey GetKey(Int32 column, Int32 row)
		{
			throw new NotImplementedException();
		}
		
		#endregion
	}
}

