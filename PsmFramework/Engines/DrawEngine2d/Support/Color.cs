using System;
using Sce.PlayStation.Core;

namespace PsmFramework.Engines.DrawEngine2d.Support
{
	public struct Color
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
	}
}

