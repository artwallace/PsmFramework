using System;

namespace PsmFramework.Engines.DrawEngine2d.Support
{
	public interface IDebuggable : IBounded
	{
		Boolean DebugInfoEnabled { get; set; }
		IDebugInfo DebugInfo { get; }
		void RefreshDebugInfo();
		//TODO: These better here or in IDebugInfo?
		//TimeSpan DebugInfoRefreshInterval { get; }
		//Boolean DebugInfoRefreshForcesRender { get; }
	}
}

