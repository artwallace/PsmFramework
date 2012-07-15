/* SCE CONFIDENTIAL
 * PlayStation(R)Suite SDK 0.98.2
 * Copyright (C) 2012 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

//#define ENABLE_PROFILE
//#define DEBUG_SCENE_TRANSITIONS

using System;
using System.Collections.Generic;

using Sce.PlayStation.HighLevel.GameEngine2D.Base;

namespace Sce.PlayStation.HighLevel.GameEngine2D
{
	/// <summary>
	/// The flag bits used by Director.Instance.DebugFlags. 
	/// By default they are all turned off.
	/// </summary>
	public static class DebugFlags
	{
		/// <summary>
		/// Draw each node's transform matrix as a red arrow + green arrow.
		/// </summary>
		public static uint DrawTransform = 1;
		/// <summary>
		/// Show the pivot for all nodes.
		/// </summary>
		public static uint DrawPivot = 2;
		/// <summary>
		/// Show the content local bounds of nodes that defined GetlContentLocalBounds.
		/// Note that the content local bounds is transformed by the parent.
		/// </summary>
		public static uint DrawContentLocalBounds = 4;
		/// <summary>
		/// Show the content world bounds of nodes that defined GetlContentLocalBounds.
		/// </summary>
		public static uint DrawContentWorldBounds = 8;
		/// <summary>
		/// Draw a debug world grid, with axis in black and rulers in grey.
		/// </summary>
		public static uint DrawGrid = 16;
		/// <summary>
		/// Enable debug camera navigation (workd with Camera2D only.)
		/// </summary>
		public static uint Navigate = 32;
		// enable runtime log (not very refined at the moment)
		internal static uint Log = 64;
	}
}

