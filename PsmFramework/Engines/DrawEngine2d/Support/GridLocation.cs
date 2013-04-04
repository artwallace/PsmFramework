using System;

namespace PsmFramework.Engines.DrawEngine2d.Support
{
	internal struct GridLocation
	{
		public GridLocation(Int32 column, Int32 row)
		{
			Column = column;
			Row = row;
		}
		
		public readonly Int32 Column;
		public readonly Int32 Row;
	}
}

