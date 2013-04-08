using System;

namespace PsmFramework.Engines.DrawEngine2d.Support
{
	public struct RectangularArea2i : IEquatable<RectangularArea2i>
	{
		#region Constructor
		
		public RectangularArea2i(Int32 left, Int32 top, Int32 right, Int32 bottom)
		{
			Left = left;
			Top = top;
			Right = right;
			Bottom = bottom;
			
			HashCodeDirty = true;
			HashCode = 0;
			
			TopLeft = new Coordinate2i(left, top);
			BottomLeft = new Coordinate2i(left, bottom);
			TopRight = new Coordinate2i(right, top);
			BottomRight = new Coordinate2i(right, bottom);
			
			Width = right - left;
			Height = top - bottom;
		}
		
		#endregion
		
		#region Boundaries
		
		public readonly Int32 Left;
		
		public readonly Int32 Top;
		
		public readonly Int32 Right;
		
		public readonly Int32 Bottom;
		
		#endregion
		
		#region Coordinates
		
		public readonly Coordinate2i TopLeft;
		
		public readonly Coordinate2i BottomLeft;
		
		public readonly Coordinate2i TopRight;
		
		public readonly Coordinate2i BottomRight;
		
		#endregion
		
		#region Dimensions
		
		public readonly Int32 Width;
		
		public readonly Int32 Height;
		
		#endregion
		
		#region Static Presets
		
		public static readonly RectangularArea2i Zero = new RectangularArea2i(0, 0, 0, 0);
		
		#endregion
		
		#region IEquatable, etc.
		
		public override Boolean Equals(Object o)
		{
			if (o is RectangularArea2i)
				return this.Equals((RectangularArea2i)o);
			
			return false;
		}
		
		public Boolean Equals(RectangularArea2i o)
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
			HashCode = Left ^ Top ^ Right ^ Bottom;
			HashCodeDirty = false;
		}
		
		public static Boolean operator ==(RectangularArea2i o1, RectangularArea2i o2)
		{
			if (Object.ReferenceEquals(o1, o2))
				return true;
			
			if (((Object)o1 == null) || ((Object)o2 == null))
				return false;
			
			return o1.Equals(o2);
		}
		
		public static Boolean operator !=(RectangularArea2i o1, RectangularArea2i o2)
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

