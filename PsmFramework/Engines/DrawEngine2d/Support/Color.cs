using System;
using Sce.PlayStation.Core;

namespace PsmFramework.Engines.DrawEngine2d.Support
{
	public struct Color : IEquatable<Color>
	{
		#region Constructor
		
		public Color(Single r, Single g, Single b, Single a)
		{
			if (r < 0f || r > 1f)
				throw new ArgumentOutOfRangeException();
			if (g < 0f || g > 1f)
				throw new ArgumentOutOfRangeException();
			if (b < 0f || b > 1f)
				throw new ArgumentOutOfRangeException();
			if (a < 0f || a > 1f)
				throw new ArgumentOutOfRangeException();
			
			_R = r;
			_G = g;
			_B = b;
			_A = a;
			
			_HashCodeDirty = true;
			_HashCode = 0;
			
			_AsVector4 = new Vector4(_R, _G, _B, _A);
		}
		
		#endregion
		
		#region RGBA
		
		private Single _R;
		public Single R { get { return _R; } }
		
		private Single _G;
		public Single G { get { return _G; } }
		
		private Single _B;
		public Single B { get { return _B; } }
		
		private Single _A;
		public Single A { get { return _A; } }
		
		#endregion
		
		#region AsVector4
		
		private Vector4 _AsVector4;
		public Vector4 AsVector4 { get { return _AsVector4; } }
		
		#endregion
		
		#region IEquatable, etc.
		
		public override Boolean Equals(Object o)
		{
			if (o is Color)
				return this.Equals((Color)o);
			
			return false;
		}
		
		public Boolean Equals(Color o)
		{
			if(o == null)
				return false;
			
			return
				(R == o.R) &&
				(G == o.G) &&
				(B == o.B) &&
				(A == o.A)
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
			//TODO: I'm not sure how accurate this formula is. Only gives 2 dec point precision to colors.
			_HashCode = (Int32)(R * HashCodeMultiple) ^ (Int32)(G * HashCodeMultiple) ^ (Int32)(B * HashCodeMultiple) ^ (Int32)(A * HashCodeMultiple);
			_HashCodeDirty = false;
		}
		private const Int32 HashCodeMultiple = 100;
		
		public static Boolean operator ==(Color o1, Color o2)
		{
			if (Object.ReferenceEquals(o1, o2))
				return true;
			
			if (((Object)o1 == null) || ((Object)o2 == null))
				return false;
			
			return o1.Equals(o2);
		}
		
		public static Boolean operator !=(Color o1, Color o2)
		{
			return !(o1 == o2);
		}
		
		#endregion
	}
}

