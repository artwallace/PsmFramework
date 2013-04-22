using System;
using System.Collections.Generic;

namespace PsmFramework.Engines.DrawEngine2d.TiledTextures
{
	public sealed class NamedTileIndex : IndexBase
	{
		#region Constructor, Dispose
		
		public NamedTileIndex(TiledTexture tiledTexture, String name = DefaultName)
			: base(name, tiledTexture)
		{
			BuildKeyList();
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
			Keys = new Dictionary<String, NamedTileKey>();
		}
		
		private void CleanupKeys()
		{
			foreach(NamedTileKey key in Keys.Values)
				key.Dispose();
			Keys.Clear();
			Keys = null;
		}
		
		private Dictionary<String, NamedTileKey> Keys;
		
		private void BuildKeyList()
		{
			throw new NotImplementedException();
		}
		
		public NamedTileKey GetKey(String name)
		{
			throw new NotImplementedException();
		}
		
		#endregion
	}
}

