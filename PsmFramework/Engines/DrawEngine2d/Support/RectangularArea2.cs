using System;

namespace PsmFramework.Engines.DrawEngine2d.Support
{
	public struct RectangularArea2 : IEquatable<RectangularArea2>
	{
		#region Constructor
		
		public RectangularArea2(Single left, Single top, Single right, Single bottom)
		{
			Left = left;
			Top = top;
			Right = right;
			Bottom = bottom;
			
			HashCodeDirty = true;
			HashCode = 0;
			
			TopLeft = new Coordinate2(left, top);
			BottomLeft = new Coordinate2(left, bottom);
			TopRight = new Coordinate2(right, top);
			BottomRight = new Coordinate2(right, bottom);
			
			Width = right - left;
			Height = top - bottom;
		}
		
		#endregion
		
		#region Boundaries
		
		public readonly Single Left;
		
		public readonly Single Top;
		
		public readonly Single Right;
		
		public readonly Single Bottom;
		
		#endregion
		
		#region Coordinates
		
		public readonly Coordinate2 TopLeft;
		
		public readonly Coordinate2 BottomLeft;
		
		public readonly Coordinate2 TopRight;
		
		public readonly Coordinate2 BottomRight;
		
		#endregion
		
		#region Dimensions
		
		public readonly Single Width;
		
		public readonly Single Height;
		
		#endregion
		
		#region Static Presets
		
		public static readonly RectangularArea2 Zero = new RectangularArea2(0.0f, 0.0f, 0.0f, 0.0f);
		
		#endregion
		
		#region IEquatable, etc.
		
		public override Boolean Equals(Object o)
		{
			if (o is RectangularArea2)
				return this.Equals((RectangularArea2)o);
			
			return false;
		}
		
		public Boolean Equals(RectangularArea2 o)
		{
			if(o == null)
				return false;
			
			return
				(Left == o.Left) &&
				(Top == o.Top) &&
				(Right == o.Right) &&
				(Bottom == o.Bottom)
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
			HashCode = (Int32)Left ^ (Int32)Top ^ (Int32)Right ^ (Int32)Bottom;
			HashCodeDirty = false;
		}
		
		public static Boolean operator ==(RectangularArea2 o1, RectangularArea2 o2)
		{
			if (Object.ReferenceEquals(o1, o2))
				return true;
			
			if (((Object)o1 == null) || ((Object)o2 == null))
				return false;
			
			return o1.Equals(o2);
		}
		
		public static Boolean operator !=(RectangularArea2 o1, RectangularArea2 o2)
		{
			return !(o1 == o2);
		}
		
		#endregion
		
		#region ToString
		
		public override String ToString()
		{
			return Left + "x" + Top  + "x" + Right + "x" + Bottom;
		}
		
		#endregion
	}
}

