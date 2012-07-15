using System;

namespace PsmFramework.Engines.DrawEngine2d.Textures
{
	public struct TiledTextureIndex
	{
		#region Constructor
		
		public TiledTextureIndex(Int32 column)
		{
			if(column < 0)
				throw new ArgumentOutOfRangeException();
			
			Type = TiledTextureIndexType.Column;
			
			_Column = column;
			_Row = 0;
			_Name = null;
		}
		
		public TiledTextureIndex(Int32 column, Int32 row)
		{
			if(column < 0)
				throw new ArgumentOutOfRangeException();
			
			if(row < 0)
				throw new ArgumentOutOfRangeException();
			
			Type = TiledTextureIndexType.Grid;
			
			_Column = column;
			_Row = row;
			_Name = null;
		}
		
		public TiledTextureIndex(String name)
		{
			if(String.IsNullOrWhiteSpace(name))
				throw new ArgumentOutOfRangeException();
			
			Type = TiledTextureIndexType.Named;
			
			_Column = 0;
			_Row = 0;
			_Name = name;
		}
		
		#endregion
		
		#region Type
		
		public readonly TiledTextureIndexType Type;
		
		#endregion
		
		#region Column
		
		private Int32 _Column;
		public Int32 Column
		{
			get { return _Column; }
			set
			{
				if(Type != TiledTextureIndexType.Column && Type != TiledTextureIndexType.Grid)
					throw new InvalidOperationException();
				
				if(_Column == value)
					return;
				
				_Column = value;
			}
		}
		
		#endregion
		
		#region Row
		
		private Int32 _Row;
		public Int32 Row
		{
			get { return _Row; }
			set
			{
				if(Type != TiledTextureIndexType.Grid)
					throw new InvalidOperationException();
				
				if(_Row == value)
					return;
				
				_Row = value;
			}
		}
		
		#endregion
		
		#region Name
		
		public String _Name;
		public String Name
		{
			get { return _Name; }
			set
			{
				if(Type != TiledTextureIndexType.Named)
					throw new InvalidOperationException();
				
				if(_Name == value)
					return;
				
				_Name = value;
			}
		}
		
		#endregion
	}
}

