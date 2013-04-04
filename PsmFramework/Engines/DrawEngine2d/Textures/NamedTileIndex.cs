using System;
using System.Collections.Generic;

namespace PsmFramework.Engines.DrawEngine2d.Textures
{
	public class NamedTileIndex : IndexBase
	{
		#region Constructor, Dispose
		
		public NamedTileIndex(TiledTexture tiledTexture, String name = DefaultName)
			: base(name, tiledTexture)
		{
			throw new NotImplementedException();
			
			BuildTileList();
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
		
		public override IndexType Type { get { return IndexType.NamedTiles; } }
		
		#endregion
		
		#region Tiles
		
		private void InitializeTiles()
		{
			Tiles = new Dictionary<String, Texture2dArea>();
		}
		
		private void CleanupTiles()
		{
			Tiles.Clear();
			Tiles = null;
		}
		
		private Dictionary<String, Texture2dArea> Tiles;
		
		private void BuildTileList()
		{
		}
		
		public Texture2dArea GetTextureCoordinates(string name)
		{
			return Tiles[name];
		}
		
		#endregion
	}
}

