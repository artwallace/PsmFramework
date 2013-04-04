using System;
using System.Collections.Generic;

namespace PsmFramework.Engines.DrawEngine2d.Textures
{
	public sealed class ColumnIndex : IndexBase
	{
		#region Constructor, Dispose
		
		public ColumnIndex(TiledTexture tiledTexture, Int32 columns = DefaultColumns, String name = DefaultName)
			: base(name, tiledTexture)
		{
			BuildKeyList(columns);
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
			Keys = new Dictionary<Int32, ColumnKey>();
		}
		
		private void CleanupKeys()
		{
			//TODO: Not sure if this is a good idea. Leaves drawables with disposed keys, right or wrong.
			foreach(ColumnKey key in Keys.Values)
				key.Dispose();
			Keys.Clear();
			Keys = null;
		}
		
		public const Int32 DefaultColumns = 1;
		
		private Dictionary<Int32, ColumnKey> Keys;
		
		private void BuildKeyList(Int32 columns)
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
				
				ColumnKey key = new ColumnKey(this, c, area);
				Keys.Add(c, key);
			}
		}
		
		public ColumnKey GetKey(Int32 column)
		{
			return Keys[column];
		}
		
		#endregion
	}
}

