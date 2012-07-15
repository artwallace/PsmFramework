using System;

namespace PsmFramework.Engines.DrawEngine2d.Support
{
	public struct Coordinate2i : IEquatable<Coordinate2i>
	{
		#region Constructor
		
		public Coordinate2i(Int32 x, Int32 y)
		{
			_X = x;
			_Y = y;
			
			_HashCodeDirty = true;
			_HashCode = 0;
		}
		
		#endregion
		
		#region XY
		
		private Int32 _X;
		public Int32 X
		{
			get { return _X; }
			set
			{
				_X = value;
				_HashCodeDirty = true;
			}
		}
		
		private Int32 _Y;
		public Int32 Y
		{
			get { return _Y; }
			set
			{
				_Y = value;
				_HashCodeDirty = true;
			}
		}
		
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
			_HashCode = X ^ Y;
			_HashCodeDirty = false;
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
	}
}
