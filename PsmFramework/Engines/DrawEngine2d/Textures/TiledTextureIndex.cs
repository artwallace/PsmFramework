using System;

namespace PsmFramework.Engines.DrawEngine2d.Textures
{
	public struct TiledTextureIndex : IEquatable<TiledTextureIndex>
	{
		#region Constructor
		
		public TiledTextureIndex(Int32 column)
		{
			if(column < 0)
				throw new ArgumentOutOfRangeException();
			
			Type = IndexType.Column;
			
			_Column = column;
			_Row = 0;
			_Name = null;
			
			_HashCodeDirty = true;
			_HashCode = 0;
		}
		
		public TiledTextureIndex(Int32 column, Int32 row)
		{
			if(column < 0)
				throw new ArgumentOutOfRangeException();
			
			if(row < 0)
				throw new ArgumentOutOfRangeException();
			
			Type = IndexType.Grid;
			
			_Column = column;
			_Row = row;
			_Name = null;
			
			_HashCodeDirty = true;
			_HashCode = 0;
		}
		
		public TiledTextureIndex(String name)
		{
			if(String.IsNullOrWhiteSpace(name))
				throw new ArgumentOutOfRangeException();
			
			Type = IndexType.Name;
			
			_Column = 0;
			_Row = 0;
			_Name = name;
			
			_HashCodeDirty = true;
			_HashCode = 0;
		}
		
		#endregion
		
		#region Type
		
		public readonly IndexType Type;
		
		#endregion
		
		#region Column
		
		private Int32 _Column;
		public Int32 Column
		{
			get { return _Column; }
			set
			{
				if(Type != IndexType.Column && Type != IndexType.Grid)
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
				if(Type != IndexType.Grid)
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
				if(Type != IndexType.Name)
					throw new InvalidOperationException();
				
				if(_Name == value)
					return;
				
				_Name = value;
			}
		}
		
		#endregion
		
		#region IEquatable, etc.
		
		public override Boolean Equals(Object o)
		{
			if (o is TiledTextureIndex)
				return this.Equals((TiledTextureIndex)o);
			
			return false;
		}
		
		public Boolean Equals(TiledTextureIndex o)
		{
			if(o == null)
				return false;
			
			return
				(Type == o.Type) &&
				(_Column == o._Column) &&
				(_Row == o._Row) &&
				(_Name == o._Name)
				;
		}
		
		private Int32 _HashCode;
		public override Int32 GetHashCode()
		{
			if(_HashCodeDirty)
				UpdateHashCode();
			return _HashCode;
		}
		private Boolean _HashCodeDirty;
		private void UpdateHashCode()
		{
			_HashCode = (Int32)Type ^ _Column ^ _Row ^ _Name.GetHashCode();
			_HashCodeDirty = false;
		}
		
		public static Boolean operator ==(TiledTextureIndex o1, TiledTextureIndex o2)
		{
			if (Object.ReferenceEquals(o1, o2))
				return true;
			
			if (((Object)o1 == null) || ((Object)o2 == null))
				return false;
			
			return o1.Equals(o2);
		}
		
		public static Boolean operator !=(TiledTextureIndex o1, TiledTextureIndex o2)
		{
			return !(o1 == o2);
		}
		
		#endregion
	}
}

