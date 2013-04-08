using System;

namespace PsmFramework.Engines.DrawEngine2d.Support
{
	//TODO: Need to decide if we need a non-rectangular Area class. Probably not.
	public struct Area2i
	{
		#region Constructor
		
		public Area2i(Coordinate2i topLeft, Coordinate2i bottomLeft, Coordinate2i topRight, Coordinate2i bottomRight)
		{
			TopLeft = topLeft;
			BottomLeft = bottomLeft;
			TopRight = topRight;
			BottomRight = bottomRight;
		}
		
		#endregion
		
		#region Coordinates
		
		public readonly Coordinate2i TopLeft;
		
		public readonly Coordinate2i BottomLeft;
		
		public readonly Coordinate2i TopRight;
		
		public readonly Coordinate2i BottomRight;
		
		#endregion
		
		#region Static Presets
		
		public static readonly Area2i Zero = new Area2i(Coordinate2i.X0Y0, Coordinate2i.X0Y0, Coordinate2i.X0Y0, Coordinate2i.X0Y0);
		
		#endregion
	}
}

