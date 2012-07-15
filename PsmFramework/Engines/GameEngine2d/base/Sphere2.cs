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
	/// <summary>A sphere class in 2D.</summary>
	public struct Sphere2 : ICollisionBasics
	{
		/// <summary>Sphere center.</summary>
		public Vector2 Center;

		/// <summary>Sphere radius.</summary>
		public float Radius; 

		/// <summary>Sphere2 constructor.</summary>
		public Sphere2( Vector2 center, float radius )
		{
			Center = center;
			Radius = radius;
		}

		/// <summary>
		/// Return true if 'point' is inside the primitive (in its negative space).
		/// </summary>
		public bool IsInside( Vector2 point )
		{
			return( point - Center ).LengthSquared() <= Radius * Radius;
		}

		/// <summary>
		/// Return the closest point to 'point' that lies on the surface of the primitive.
		/// If that point is inside the primitive, sign is negative.
		/// </summary>
		public void ClosestSurfacePoint( Vector2 point, out Vector2 ret, out float sign )
		{
			Vector2 r = point - Center;
			float len = r.Length();
			float d = len - Radius;

			if ( len < 0.00001f )
			{
				ret = Center + new Vector2( 0.0f, Radius );	// degenerate case, pick any separation direction
				sign = -1.0f;
				return;
			}

			ret = point - d * ( r / len );
			sign = d > 0.0f ? 1.0f : -1.0f;
		}

		/// <summary>
		/// Return the signed distance (penetration distance) from 'point' 
		/// to the surface of the primitive.
		/// </summary>
		public float SignedDistance( Vector2 point )
		{
			return( point - Center ).Length() - Radius;
		}

		/// <summary>
		/// Assuming the primitive is convex, clip the segment AB against the primitive.
		/// Return false if AB is entirely in positive halfspace,
		/// else clip against negative space and return true.
		/// </summary>
		public bool NegativeClipSegment( ref Vector2 A, ref Vector2 B )
		{
			Vector2 AB = B - A;
			float r_sqr = Radius * Radius;

			float epsilon = 0.00000001f;
			if ( AB.LengthSquared() <= epsilon )
			{
				// A and B are the same point
				if ( ( A - Center ).LengthSquared() >= r_sqr )
					return false;
			}

			Vector2 p = Center.ProjectOnLine( A, AB );
			float d_sqr = ( p - Center ).LengthSquared();
			
			if ( d_sqr >= r_sqr )
				return false;

			float e = FMath.Sqrt( FMath.Max( 0.0f, r_sqr - d_sqr ) );
			Vector2 v = AB.Normalize();
			Vector2 A2 = p - e * v;
			Vector2 B2 = p + e * v;

			if ( ( A - B2 ).Dot( AB ) >= 0.0f ) return false;
			if ( ( B - A2 ).Dot( AB ) <= 0.0f ) return false;
			 				 
			if ( ( A - A2 ).Dot( AB ) < 0.0f ) A = A2;
			if ( ( B - B2 ).Dot( AB ) > 0.0f ) B = B2;

			return true;
		}
	}
}

