using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;

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
			
			R = r;
			G = g;
			B = b;
			A = a;
			
			Ri = (Int32)(r * 255);
			Gi = (Int32)(g * 255);
			Bi = (Int32)(b * 255);
			Ai = (Int32)(a * 255);
			
			ImageColor = new ImageColor(Ri, Gi, Bi, Ai);
			AsVector4 = new Vector4(R, G, B, A);
			
			HashCodeDirty = true;
			HashCode = 0;
		}
		
		public Color(Int32 r, Int32 g, Int32 b, Int32 a)
		{
			if (r < 0 || r > 255)
				throw new ArgumentOutOfRangeException();
			if (g < 0 || g > 255)
				throw new ArgumentOutOfRangeException();
			if (b < 0 || b > 255)
				throw new ArgumentOutOfRangeException();
			if (a < 0 || a > 255)
				throw new ArgumentOutOfRangeException();
			
			Ri = r;
			Gi = g;
			Bi = b;
			Ai = a;
			
			R = r / 255;
			G = g / 255;
			B = b / 255;
			A = a / 255;
			
			ImageColor = new ImageColor(Ri, Gi, Bi, Ai);
			AsVector4 = new Vector4(R, G, B, A);
			
			HashCodeDirty = true;
			HashCode = 0;
		}
		
		#endregion
		
		#region RGBA
		
		//From 0.0f-1.0f
		public readonly Single R;
		public readonly Single G;
		public readonly Single B;
		public readonly Single A;
		
		//From 0-256
		public readonly Int32 Ri;
		public readonly Int32 Gi;
		public readonly Int32 Bi;
		public readonly Int32 Ai;
		
		#endregion
		
		#region ImageColor
		
		public readonly ImageColor ImageColor;
		
		#endregion
		
		#region AsVector4
		
		public readonly Vector4 AsVector4;
		
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
			//TODO: I'm not sure how accurate this formula is. Only gives 2 dec point precision to colors.
			HashCode = (Int32)(R * HashCodeMultiple) ^ (Int32)(G * HashCodeMultiple) ^ (Int32)(B * HashCodeMultiple) ^ (Int32)(A * HashCodeMultiple);
			HashCodeDirty = false;
		}
		private const Int32 HashCodeMultiple = 1000;
		
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

