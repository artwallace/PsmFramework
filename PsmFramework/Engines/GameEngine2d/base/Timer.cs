/* SCE CONFIDENTIAL
 * PlayStation(R)Suite SDK 0.98.2
 * Copyright (C) 2012 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

namespace Sce.PlayStation.HighLevel.GameEngine2D.Base
{
	/// <summary>A simple timer.</summary>
	public class Timer
	{
//		System.DateTime m_start; // DateTime can't be trusted for profiling
		System.Diagnostics.Stopwatch m_stop_watch = new System.Diagnostics.Stopwatch();

		/// <summary>Timer constructor.</summary>
		public Timer()
		{
			Reset();
		}

		/// <summary>
		/// Reset the timer.
		/// </summary>
		public void Reset()
		{
//			m_start = System.DateTime.Now;
			m_stop_watch.Reset();
			m_stop_watch.Start();
		}

		/// <summary>
		/// Return time elapsed (in milliseconds) since constructor as called or since last call to Reset().
		/// </summary>
		public double Milliseconds()
		{
//			return( System.DateTime.Now - m_start ).TotalMilliseconds;
//			return ( ( m_stop_watch.ElapsedTicks * 1000 ) / System.Diagnostics.Stopwatch.Frequency );
			return m_stop_watch.Elapsed.TotalMilliseconds;
		}

		/// <summary>
		/// Return time elapsed (in seconds) since constructor as called or since last call to Reset().
		/// </summary>
		public double Seconds()
		{
//			return( System.DateTime.Now - m_start ).TotalSeconds;
			return m_stop_watch.Elapsed.TotalSeconds;
		}
	}
}

