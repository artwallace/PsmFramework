using System;
using Sce.PlayStation.Core;

namespace PsmFramework.Engines.DrawEngine2d.Support
{
	public struct Angle2 : IEquatable<Angle2>
	{
		#region Factory
		
		public static Angle2 GetAngleFromOrigin(Single x, Single y)
		{
			return new Angle2(FMath.Atan2(x, y) * FMath.RadToDeg);
		}
		
		#endregion
		
		#region Constructor
		
		public Angle2(Single degree)
		{
			Degree = GetNormalizedDegree(degree);
			Radian = GetRadianAngle(Degree);
			
			HashCodeDirty = true;
			HashCode = 0;
		}
		
		#endregion
		
		#region Degree
		
		/// <summary>
		/// Always stored as 0 =< x < 360.
		/// </summary>
		public readonly Single Degree;
		
		public static Single GetNormalizedDegree(Single degree)
		{
			if(degree < 0.0f || degree > 360.0f)
				degree = degree % 360.0f;
			
			if(degree < 0.0f)
				degree = 360.0f - FMath.Abs(degree);
			
			return degree;
		}
		
		#endregion
		
		#region Radian
		
		public readonly Single Radian;
		
		public static Single GetRadianAngle(Single degree)
		{
			return degree * FMath.DegToRad;
		}
		
		#endregion
		
		#region Static Presets
		
		public static readonly Angle2 Zero = new Angle2(0.0f);
		
		#endregion
		
		#region IEquatable, etc.
		
		public override Boolean Equals(Object o)
		{
			if (o is Angle2)
				return this.Equals((Angle2)o);
			
			return false;
		}
		
		public Boolean Equals(Angle2 o)
		{
			if(o == null)
				return false;
			
			return
				(Degree == o.Degree)
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
			HashCode = (Int32)Degree;
			HashCodeDirty = false;
		}
		
		public static Boolean operator ==(Angle2 o1, Angle2 o2)
		{
			if (Object.ReferenceEquals(o1, o2))
				return true;
			
			if (((Object)o1 == null) || ((Object)o2 == null))
				return false;
			
			return o1.Equals(o2);
		}
		
		public static Boolean operator !=(Angle2 o1, Angle2 o2)
		{
			return !(o1 == o2);
		}
		
		#endregion
		
		#region ToString
		
		public override String ToString()
		{
			 return Degree.ToString();
		}
		
		#endregion
	}
}

