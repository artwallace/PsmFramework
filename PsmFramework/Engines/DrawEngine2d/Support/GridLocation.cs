using System;

namespace PsmFramework.Engines.DrawEngine2d.Support
{
	internal struct GridLocation : IEquatable<GridLocation>
	{
		#region Constructor
		
		public GridLocation(Int32 column, Int32 row)
		{
			Column = column;
			Row = row;
			
			HashCodeDirty = true;
			HashCode = 0;
		}
		
		#endregion
		
		#region Column and Row
		
		public readonly Int32 Column;
		public readonly Int32 Row;
		
		#endregion
		
		#region Static Presets
		
		public static readonly GridLocation C0R0 = new GridLocation(0, 0);
		
		#endregion
		
		#region IEquatable, etc.
		
		public override Boolean Equals(Object o)
		{
			if (o is GridLocation)
				return this.Equals((GridLocation)o);
			
			return false;
		}
		
		public Boolean Equals(GridLocation o)
		{
			if(o == null)
				return false;
			
			return
				(Column == o.Column) &&
				(Row == o.Row)
				;
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
			HashCode = Column ^ Row;
			HashCodeDirty = false;
		}
		
		public static Boolean operator ==(GridLocation o1, GridLocation o2)
		{
			if (Object.ReferenceEquals(o1, o2))
				return true;
			
			if (((Object)o1 == null) || ((Object)o2 == null))
				return false;
			
			return o1.Equals(o2);
		}
		
		public static Boolean operator !=(GridLocation o1, GridLocation o2)
		{
			return !(o1 == o2);
		}
		
		#endregion
		
		#region ToString
		
		public override String ToString()
		{
			return Column + "x" + Row;
		}
		
		#endregion
	}
}

