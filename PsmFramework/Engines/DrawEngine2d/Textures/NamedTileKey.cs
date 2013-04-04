using System;

namespace PsmFramework.Engines.DrawEngine2d.Textures
{
	public sealed class NamedTileKey : KeyBase
	{
		#region Constructor, Dispose
		
		internal NamedTileKey(NamedTileIndex index, String name, Texture2dArea tile)
			: base(tile)
		{
			//Inits moved here because parameters need to be passed.
			InitializeIndex(index);
			InitializeKey(name);
		}
		
		#endregion
		
		#region Initialize, Cleanup
		
//		protected override void Initialize()
//		{
//		}
		
		protected override void Cleanup()
		{
			CleanupKey();
			CleanupIndex();
		}
		
		#endregion
		
		#region Index
		
		private void InitializeIndex(NamedTileIndex index)
		{
			if (index == null)
				throw new ArgumentNullException();
			
			Index = index;
		}
		
		private void CleanupIndex()
		{
			Index = null;
		}
		
		public NamedTileIndex Index { get; private set; }
		
		#endregion
		
		#region TiledTexture
		
		public override TiledTexture TiledTexture
		{
			get
			{
				if (IsDisposed)
					throw new ObjectDisposedException("NamedTileKey");
				
				if (Index == null)
					throw new InvalidOperationException();
				
				return Index.TiledTexture;
			}
		}
		
		#endregion
		
		#region Key
		
		private void InitializeKey(String name)
		{
			if (String.IsNullOrWhiteSpace(name))
				throw new ArgumentOutOfRangeException();
			
			Name = name;
		}
		
		private void CleanupKey()
		{
			Name = default(String);
		}
		
		public String Name { get; private set; }
		
		#endregion
	}
}

