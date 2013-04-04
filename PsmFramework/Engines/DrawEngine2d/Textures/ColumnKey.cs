using System;

namespace PsmFramework.Engines.DrawEngine2d.Textures
{
	public sealed class ColumnKey : KeyBase
	{
		#region Constructor, Dispose
		
		internal ColumnKey(ColumnIndex index, Int32 column, Texture2dArea tile)
			: base(tile)
		{
			//Inits moved here because parameters need to be passed.
			InitializeIndex(index);
			InitializeKey(column);
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
		
		private void InitializeIndex(ColumnIndex index)
		{
			if (index == null)
				throw new ArgumentNullException();
			
			Index = index;
		}
		
		private void CleanupIndex()
		{
			Index = null;
		}
		
		public ColumnIndex Index { get; private set; }
		
		#endregion
		
		#region TiledTexture
		
		public override TiledTexture TiledTexture
		{
			get
			{
				if (IsDisposed)
					throw new ObjectDisposedException("ColumnKey");
				
				if (Index == null)
					throw new InvalidOperationException();
				
				return Index.TiledTexture;
			}
		}
		
		#endregion
		
		#region Key
		
		private void InitializeKey(Int32 column)
		{
			if (column < 0)
				throw new ArgumentOutOfRangeException();
			
			Column = column;
		}
		
		private void CleanupKey()
		{
			Column = default(Int32);
		}
		
		public Int32 Column { get; private set; }
		
		#endregion
	}
}

