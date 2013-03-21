using System;

namespace PsmFramework.Engines.DrawEngine2d.Support
{
	public struct Angle2 : IEquatable<Angle2>
	{
		#region Constructor
		
		public Angle2(Single degree)
		{
			_Degree = degree;
			_NormalizedDegree = GetNormalizedDegree(degree);
			
			_HashCodeDirty = true;
			_HashCode = 0;
		}
		
		#endregion
		
		#region Degree
		
		private Single _Degree;
		public Single Degree
		{
			get { return _Degree; }
			set
			{
				_Degree = value;
				_NormalizedDegree = GetNormalizedDegree(value);
				_HashCodeDirty = true;
			}
		}
		
		private Single _NormalizedDegree;
		public Single NormalizedDegree
		{
			get { return _NormalizedDegree; }
		}
		
		public static Single GetNormalizedDegree(Single degree)
		{
			if(degree < 0.0f || degree > 360.0f)
				degree = degree % 360.0f;
			
			if(degree < 0.0f)
				degree = 360.0f - Math.Abs(degree);
			
			return degree;
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
			_HashCode = (Int32)Degree;
			_HashCodeDirty = false;
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
	}
}

