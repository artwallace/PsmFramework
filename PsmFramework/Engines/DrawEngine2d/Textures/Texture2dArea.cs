using System;
using PsmFramework.Engines.DrawEngine2d.Support;

namespace PsmFramework.Engines.DrawEngine2d.Textures
{
	public struct Texture2dArea
	{
		#region Constructor
		
		public Texture2dArea(Int32 left, Int32 top, Int32 right, Int32 bottom, Int32 textureWidth, Int32 textureHeight)
		{
			_Left = left;
			_Top = top;
			_Right = right;
			_Bottom = bottom;
			
			_TopLeft = new Coordinate2i(left, top);
			_BottomLeft = new Coordinate2i(left, bottom);
			_TopRight = new Coordinate2i(right, top);
			_BottomRight = new Coordinate2i(right, bottom);
			
			_Width = right - left;
			_Height = bottom - top;
			
			_CoordinateArray = new Single[8];
			//TopLeft
			_CoordinateArray[0] = (Single)left / textureWidth;
			_CoordinateArray[1] = (Single)top / textureHeight;
			//BottomLeft
			_CoordinateArray[2] = (Single)left / textureWidth;
			_CoordinateArray[3] = (Single)bottom / textureHeight;
			//TopRight
			_CoordinateArray[4] = (Single)right / textureWidth;
			_CoordinateArray[5] = (Single)top / textureHeight;
			//BottomRight
			_CoordinateArray[6] = (Single)right / textureWidth;
			_CoordinateArray[7] = (Single)bottom / textureHeight;
			
		}
		
		#endregion
		
		#region Boundaries
		
		private Int32 _Left;
		public Int32 Left { get { return _Left; } }
		
		private Int32 _Top;
		public Int32 Top { get { return _Top; } }
		
		private Int32 _Right;
		public Int32 Right { get { return _Right; } }
		
		private Int32 _Bottom;
		public Int32 Bottom { get { return _Bottom; } }
		
		#endregion
		
		#region Dimensions
		
		private Int32 _Width;
		public Int32 Width { get { return _Width; } }
		
		private Int32 _Height;
		public Int32 Height { get { return _Height; } }
		
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
		
		#region CoordinateArray
		
		//TODO: This array would allow the coordinates to be changed
		// without updating the other stored coordinate properties.
		private Single[] _CoordinateArray;
		public Single[] CoordinateArray { get { return _CoordinateArray; } }
		
		#endregion
	}
}

