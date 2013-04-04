using System;

namespace PsmFramework.Engines.DrawEngine2d.Textures
{
	public sealed class GridKey : KeyBase
	{
		#region Constructor, Dispose
		
		internal GridKey(GridIndex index, Int32 column, Int32 row, Texture2dArea tile)
			: base(tile)
		{
			//Inits moved here because parameters need to be passed.
			InitializeIndex(index);
			InitializeKey(column, row);
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
		
		private void InitializeIndex(GridIndex index)
		{
			if (index == null)
				throw new ArgumentNullException();
			
			Index = index;
		}
		
		private void CleanupIndex()
		{
			Index = null;
		}
		
		public GridIndex Index { get; private set; }
		
		#endregion
		
		#region TiledTexture
		
		public override TiledTexture TiledTexture
		{
			get
			{
				if (IsDisposed)
					throw new ObjectDisposedException("GridKey");
				
				if (Index == null)
					throw new InvalidOperationException();
				
				return Index.TiledTexture;
			}
		}
		
		#endregion
		
		#region Key
		
		private void InitializeKey(Int32 column, Int32 row)
		{
			if (column < 0)
				throw new ArgumentOutOfRangeException();
			
			if (row < 0)
				throw new ArgumentOutOfRangeException();
			
			Column = column;
			Row = row;
		}
		
		private void CleanupKey()
		{
			Column = default(Int32);
			Row = default(Int32);
		}
		
		public Int32 Column { get; private set; }
		public Int32 Row { get; private set; }
		
		#endregion
	}
}

