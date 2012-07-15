using System;

namespace PsmFramework.Engines.DrawEngine2d.Support
{
	//TODO: Need to decide if we need a non-rectangular Area class. Probably not.
	public struct Area2i
	{
		#region Constructor
		
		public Area2i(Coordinate2i topLeft, Coordinate2i bottomLeft, Coordinate2i topRight, Coordinate2i bottomRight)
		{
			_TopLeft = topLeft;
			_BottomLeft = bottomLeft;
			_TopRight = topRight;
			_BottomRight = bottomRight;
		}
		
		#endregion
		
		#region Coordinates
		
		private Coordinate2i _TopLeft;
		public Coordinate2i TopLeft { get { return _TopLeft; } }
		
		private Coordinate2i _BottomLeft;
		public Coordinate2i BottomLeft { get { return _BottomLeft; } }
		
		private Coordinate2i _TopRight;
		public Coordinate2i TopRight { get { return _TopRight; } }
		
		private Coordinate2i _BottomRight;
		public Coordinate2i BottomRight { get { return _BottomRight; } }
		
		#endregion
		
		#region Static Presets
		
		public static readonly Area2i Zero = new Area2i(Coordinate2i.X0Y0, Coordinate2i.X0Y0, Coordinate2i.X0Y0, Coordinate2i.X0Y0);
		
		#endregion
	}
}

