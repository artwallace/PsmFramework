using System;

namespace PsmFramework.Engines.DrawEngine2d.Support
{
	public struct Coordinate2i : IEquatable<Coordinate2i>
	{
		#region Constructor
		
		public Coordinate2i(Int32 x, Int32 y)
		{
			X = x;
			Y = y;
			
			HashCodeDirty = true;
			HashCode = 0;
		}
		
		#endregion
		
		#region XY
		
		public readonly Int32 X;
		
		public readonly Int32 Y;
		
		#endregion
		
		#region Static Presets
		
		public static readonly Coordinate2i X0Y0 = new Coordinate2i(0, 0);
		
		#endregion
		
		#region IEquatable, etc.
		
		public override Boolean Equals(Object o)
		{
			if (o is Coordinate2i)
				return this.Equals((Coordinate2i)o);
			
			return false;
		}
		
		public Boolean Equals(Coordinate2i o)
		{
			if(o == null)
				return false;
			
			return
				(X == o.X) &&
				(Y == o.Y)
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
			HashCode = X ^ Y;
			HashCodeDirty = false;
		}
		
		public static Boolean operator ==(Coordinate2i o1, Coordinate2i o2)
		{
			if (Object.ReferenceEquals(o1, o2))
				return true;
			
			if (((Object)o1 == null) || ((Object)o2 == null))
				return false;
			
			return o1.Equals(o2);
		}
		
		public static Boolean operator !=(Coordinate2i o1, Coordinate2i o2)
		{
			return !(o1 == o2);
		}
		
		#endregion
		
		#region ToString
		
		public override String ToString()
		{
			return X + "x" + Y;
		}
		
		#endregion
	}
}
