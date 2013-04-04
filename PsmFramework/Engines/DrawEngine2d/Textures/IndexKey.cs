using System;

namespace PsmFramework.Engines.DrawEngine2d.Textures
{
	//TODO: This should probably be changed into a series of classes, like the indexes themselves.
	public struct IndexKey : IEquatable<IndexKey>
	{
		#region Constructor
		
		public IndexKey(ColumnIndex index, Int32 column)
		{
			if (index == null)
				throw new ArgumentNullException();
			
			if (column < 0)
				throw new ArgumentOutOfRangeException();
			
			Index = index;
			Type = IndexType.Column;
			
			Column = column;
			Row = default(Int32);
			Name = default(String);
			
			HashCodeDirty = true;
			HashCode = default(Int32);
		}
		
		public IndexKey(GridIndex index, Int32 column, Int32 row)
		{
			if (index == null)
				throw new ArgumentNullException();
			
			if (column < 0 || row < 0)
				throw new ArgumentOutOfRangeException();
			
			Index = index;
			Type = IndexType.Grid;
			
			Column = column;
			Row = row;
			Name = default(String);
			
			HashCodeDirty = true;
			HashCode = default(Int32);
		}
		
		public IndexKey(NamedTileIndex index, String name)
		{
			if (index == null)
				throw new ArgumentNullException();
			
			if (String.IsNullOrWhiteSpace(name))
				throw new ArgumentException();
			
			Index = index;
			Type = IndexType.NamedTiles;
			
			Column = default(Int32);
			Row = default(Int32);
			Name = name;
			
			HashCodeDirty = true;
			HashCode = default(Int32);
		}
		
		#endregion
		
		#region Index
		
		public readonly IndexBase Index;
		
		#endregion
		
		#region Type
		
		public readonly IndexType Type;
		
		#endregion
		
		#region Values
		
		public readonly Int32 Column;
		
		public readonly Int32 Row;
		
		public readonly String Name;
		
		#endregion
		
		#region TileCoordinates
		
		public Texture2dArea GetTileDimensions()
		{
			//TODO: This is stupid. Change from struct to classes.
			switch (Type)
			{
				case IndexType.Column:
					ColumnIndex ci = (ColumnIndex)Index;
					return ci.GetTextureCoordinates(Column);
				case IndexType.Grid:
					GridIndex gi = (GridIndex)Index;
					return gi.GetTextureCoordinates(Column, Row);
				case IndexType.NamedTiles:
					NamedTileIndex nti = (NamedTileIndex)Index;
					return nti.GetTextureCoordinates(Name);
				default:
					throw new NotImplementedException();
			}
		}
		
		public Single[] GetTileCoordinates()
		{
			return GetTileDimensions().CoordinateArray;
		}
		
		#endregion
		
		#region IEquatable, etc.
		
		public override Boolean Equals(Object o)
		{
			if (o is IndexKey)
				return this.Equals((IndexKey)o);
			
			return false;
		}
		
		public Boolean Equals(IndexKey o)
		{
			if(o == null)
				return false;
			
			if (Index != o.Index || Type != o.Type)
				return false;
			
			switch (Type)
			{
				case IndexType.Column :
					return (Column == o.Column);
				case IndexType.Grid :
					return (Column == o.Column) && (Row == o.Row);
				case IndexType.NamedTiles :
					return (Name == o.Name);
				default:
					throw new NotImplementedException();
			}
		}
		
		private Int32 HashCode;
		public override Int32 GetHashCode()
		{
			if(HashCodeDirty)
				UpdateHashCode();
			return HashCode;
		}
		private Boolean HashCodeDirty;
		private void UpdateHashCode()
		{
			switch (Type)
			{
				case IndexType.Column :
					HashCode = (Int32)Type ^ Column;
					break;
				case IndexType.Grid :
					HashCode = (Int32)Type ^ Column ^ Row;
					break;
				case IndexType.NamedTiles :
					HashCode = (Int32)Type ^ Name.GetHashCode();
					break;
				default:
					throw new NotImplementedException();
			}
			
			HashCodeDirty = false;
		}
		
		public static Boolean operator ==(IndexKey o1, IndexKey o2)
		{
			if (Object.ReferenceEquals(o1, o2))
				return true;
			
			if (((Object)o1 == null) || ((Object)o2 == null))
				return false;
			
			return o1.Equals(o2);
		}
		
		public static Boolean operator !=(IndexKey o1, IndexKey o2)
		{
			return !(o1 == o2);
		}
		
		#endregion
	}
}

