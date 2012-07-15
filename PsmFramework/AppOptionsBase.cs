using System;

namespace PsmFramework
{
	public abstract class AppOptionsBase : IDisposable
	{
		#region Constructor, Dispose
		
		public AppOptionsBase()
		{
		}
		
		public void Dispose()
		{
		}
		
		#endregion
		
		#region Volume
		
		public Int32 SoundEffectsVolume { get; set; }
		
		public Int32 MusicVolume { get; set; }
		
		#endregion
	}
}

