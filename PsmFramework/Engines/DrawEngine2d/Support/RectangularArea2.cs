using System;

namespace PsmFramework.Engines.DrawEngine2d.Support
{
	public struct RectangularArea2 : IEquatable<RectangularArea2>
	{
		#region Constructor
		
		public RectangularArea2(Single left, Single top, Single right, Single bottom)
		{
			_Left = left;
			_Top = top;
			_Right = right;
			_Bottom = bottom;
			
			_HashCodeDirty = true;
			_HashCode = 0;
			
			_TopLeft = new Coordinate2(left, top);
			_BottomLeft = new Coordinate2(left, bottom);
			_TopRight = new Coordinate2(right, top);
			_BottomRight = new Coordinate2(right, bottom);
			
			_Width = right - left;
			_Height = top - bottom;
		}
		
		#endregion
		
		#region Boundaries
		
		private Single _Left;
		public Single Left { get { return _Left; } }
		
		private Single _Top;
		public Single Top { get { return _Top; } }
		
		private Single _Right;
		public Single Right { get { return _Right; } }
		
		private Single _Bottom;
		public Single Bottom { get { return _Bottom; } }
		
		#endregion
		
		#region Coordinates
		
		private Coordinate2 _TopLeft;
		public Coordinate2 TopLeft { get { return _TopLeft; } }
		
		private Coordinate2 _BottomLeft;
		public Coordinate2 BottomLeft { get { return _BottomLeft; } }
		
		private Coordinate2 _TopRight;
		public Coordinate2 TopRight { get { return _TopRight; } }
		
		private Coordinate2 _BottomRight;
		public Coordinate2 BottomRight { get { return _BottomRight; } }
		
		#endregion
		
		#region Dimensions
		
		private Single _Width;
		public Single Width { get { return _Width; } }
		
		private Single _Height;
		public Single Height { get { return _Height; } }
		
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
			_HashCode = (Int32)Left ^ (Int32)Top ^ (Int32)Right ^ (Int32)Bottom;
			_HashCodeDirty = false;
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
	}
}

