/* SCE CONFIDENTIAL
 * PlayStation(R)Suite SDK 0.98.2
 * Copyright (C) 2012 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;

namespace Sce.PlayStation.HighLevel.GameEngine2D.Base
{
	/// <summary>Common interface for Bounds2, Sphere2, ConvexPoly2.</summary>
	public interface ICollisionBasics
	{
		/// <summary>
		/// Return true if 'point' is inside the primitive (in its negative space).
		/// </summary>
		bool IsInside( Vector2 point );
		/// <summary>
		/// Return the closest point to 'point' that lies on the surface of the primitive.
		/// If that point is inside the primitive, sign is negative.
		/// </summary>
		void ClosestSurfacePoint( Vector2 point, out Vector2 ret, out float sign );	
		/// <summary>
		/// Return the signed distance (penetration distance) from 'point' 
		/// to the surface of the primitive.
		/// </summary>
		float SignedDistance( Vector2 point );
		/// <summary>
		/// Assuming the primitive is convex, clip the segment AB against the primitive.
		/// Return false if AB is entirely in positive halfspace,
		/// else clip against negative space and return true.
		/// </summary>
		bool NegativeClipSegment( ref Vector2 A, ref Vector2 B );
	}
}

