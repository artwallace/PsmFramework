using System;

namespace PsmFramework.Engines.DrawEngine2d.Support
{
	public struct Coordinate2 : IEquatable<Coordinate2>
	{
		#region Constructor
		
		public Coordinate2(Single x, Single y)
		{
			_X = x;
			_Y = y;
			
			_HashCodeDirty = true;
			_HashCode = 0;
		}
		
		#endregion
		
		#region XY
		
		private Single _X;
		public Single X
		{
			get { return _X; }
			set
			{
				_X = value;
				_HashCodeDirty = true;
			}
		}
		
		private Single _Y;
		public Single Y
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
		
		public static readonly Coordinate2 X0Y0 = new Coordinate2(0f, 0f);
		
		#endregion
		
		#region IEquatable, etc.
		
		public override Boolean Equals(Object o)
		{
			if (o is Coordinate2)
				return this.Equals((Coordinate2)o);
			
			return false;
		}
		
		public Boolean Equals(Coordinate2 o)
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
			_HashCode = (Int32)X ^ (Int32)Y;
			_HashCodeDirty = false;
		}
		
		public static Boolean operator ==(Coordinate2 o1, Coordinate2 o2)
		{
			if (Object.ReferenceEquals(o1, o2))
				return true;
			
			if (((Object)o1 == null) || ((Object)o2 == null))
				return false;
			
			return o1.Equals(o2);
		}
		
		public static Boolean operator !=(Coordinate2 o1, Coordinate2 o2)
		{
			return !(o1 == o2);
		}
		
		#endregion
	}
}
