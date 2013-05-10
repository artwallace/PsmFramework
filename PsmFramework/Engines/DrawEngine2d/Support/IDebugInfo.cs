using System;

namespace PsmFramework.Engines.DrawEngine2d.Support
{
	public interface IDebugInfo : IDisposableStatus
	{
		void ParentUpdated();
		
		Boolean RefreshForcesRender { get; set; }
		TimeSpan RefreshInterval { get; set; }
		Boolean RefreshNeeded { get; }
		DateTime LastRefresh { get; }
		void CalcIfRefreshNeeded(DateTime updateTime);
		
		Boolean Visible { get; set; }
		RelativePosition RelativePosition { get; set; }
		PlacementPosition PlacementPosition { get; set; }
		TextAlignment TextAlignment { get; set; }
		
		Color ForegroundColor { get; set; }
		Color BackgroundColor { get; set; }
		
		void AddDebugInfoLine(String name, String data);
		void AddDebugInfoLine(String name, Int32 data);
		void AddDebugInfoLine(String name, Int64 data);
		void AddDebugInfoLine(String name, Single data);
		void AddDebugInfoLine(String name, Coordinate2 data);
		void AddDebugInfoLine(String name, Coordinate2i data);
		void AddDebugInfoLine(String name, RectangularArea2 data);
		void AddDebugInfoLine(String name, RectangularArea2i data);
		void AddDebugInfoLine(String name, Angle2 data);
	}
}

