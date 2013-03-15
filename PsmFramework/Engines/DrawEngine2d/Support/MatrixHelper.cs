using System;

namespace PsmFramework.Engines.DrawEngine2d.Support
{
	public static class MatrixHelper
	{
		//TODO: Verify that these are not in the PSM base libraries somewhere.
		
		#region Angle utilities
		
		//TODO: Verify that this formula is correct.
		private static Single DegreeToRadianValue = (Single)(Math.PI / 180D);
		
		public static Single GetRadianAngle(Single angle)
		{
			return angle * DegreeToRadianValue;
		}
		
		#endregion
	}
}

