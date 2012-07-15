using System;

namespace PsmFramework.Engines.DrawEngine2d.Support
{
	public struct RectangularArea2i : IEquatable<RectangularArea2i>
	{
		#region Constructor
		
		public RectangularArea2i(Int32 left, Int32 top, Int32 right, Int32 bottom)
		{
			_Left = left;
			_Top = top;
			_Right = right;
			_Bottom = bottom;
			
			_HashCodeDirty = true;
			_HashCode = 0;
			
			_TopLeft = new Coordinate2i(left, top);
			_BottomLeft = new Coordinate2i(left, bottom);
			_TopRight = new Coordinate2i(right, top);
			_BottomRight = new Coordinate2i(right, bottom);
			
			_Width = right - left + 1; //zero based
			_Height = top - bottom + 1; //zero based
		}
		
		#endregion
		
		#region Boundaries
		
		private Int32 _Left;
		public Int32 Left { get { return _Left; } }
		
		private Int32 _Top;
		public Int32 Top { get { return _Top; } }
		
		private Int32 _Right;
		public Int32 Right { get { return _Right; } }
		
		private Int32 _Bottom;
		public Int32 Bottom { get { return _Bottom; } }
		
		#endregion
		
		#region Coordinates
		
		private Coordinate2i _TopLeft;
		public Coordinate2i TopLeft { get { return _TopLeft; } }
		
		private Coordinate2i _BottomLeft;
		public Coordinate2i BottomLeft { get { return _BottomLeft; } }
		
		private Coordinate2i _TopRight;
		public Coordinate2i TopRight { get { return _TopRight; } }
		
		private Coordinate2i _BottomRight;
		public Coordinate2i BottomRight { get { return _BottomRight; } }
		
		#endregion
		
		#region Dimensions
		
		private Int32 _Width;
		public Int32 Width { get { return _Width; } }
		
		private Int32 _Height;
		public Int32 Height { get { return _Height; } }
		
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
			_HashCode = Left ^ Top ^ Right ^ Bottom;
			_HashCodeDirty = false;
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
	}
}

