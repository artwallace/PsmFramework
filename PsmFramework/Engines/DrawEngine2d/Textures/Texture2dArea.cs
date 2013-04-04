using System;
using PsmFramework.Engines.DrawEngine2d.Support;

namespace PsmFramework.Engines.DrawEngine2d.Textures
{
	public struct Texture2dArea
	{
		#region Constructor
		
		public Texture2dArea(Int32 left, Int32 top, Int32 right, Int32 bottom, Int32 textureWidth, Int32 textureHeight)
		{
			Left = left;
			Top = top;
			Right = right;
			Bottom = bottom;
			
			TopLeft = new Coordinate2i(left, top);
			BottomLeft = new Coordinate2i(left, bottom);
			TopRight = new Coordinate2i(right, top);
			BottomRight = new Coordinate2i(right, bottom);
			
			Width = right - left;
			Height = bottom - top;
			
			CoordinateArray = new Single[8];
			//TopLeft
			CoordinateArray[0] = (Single)left / textureWidth;
			CoordinateArray[1] = (Single)top / textureHeight;
			//BottomLeft
			CoordinateArray[2] = (Single)left / textureWidth;
			CoordinateArray[3] = (Single)bottom / textureHeight;
			//TopRight
			CoordinateArray[4] = (Single)right / textureWidth;
			CoordinateArray[5] = (Single)top / textureHeight;
			//BottomRight
			CoordinateArray[6] = (Single)right / textureWidth;
			CoordinateArray[7] = (Single)bottom / textureHeight;
		}
		
		#endregion
		
		#region Boundaries
		
		public readonly Int32 Left;
		
		public readonly Int32 Top;
		
		public readonly Int32 Right;
		
		public readonly Int32 Bottom;
		
		#endregion
		
		#region Dimensions
		
		public readonly Int32 Width;
		
		public readonly Int32 Height;
		
		#endregion
		
		#region Coordinates
		
		public readonly Coordinate2i TopLeft;
		
		public readonly Coordinate2i BottomLeft;
		
		public readonly Coordinate2i TopRight;
		
		public readonly Coordinate2i BottomRight;
		
		#endregion
		
		#region CoordinateArray
		
		//TODO: This is semi-dangerous because this array would allow 
		// the coordinates to be changed without updating the other 
		// stored coordinate properties.
		public readonly Single[] CoordinateArray;
		
		#endregion
	}
}

