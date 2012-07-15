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
	/// <summary>A plane class in 2D.</summary>
	public struct Plane2 : ICollisionBasics
	{
		/// <summary>A base point on the plane.</summary>
		public Vector2 Base;

		/// <summary>The plane normal vector, assumed to be unit length. If this is not the case, some functions will have undefined behaviour.</summary>
		public Vector2 UnitNormal;

		/// <summary>Plane2 constructor</summary>
		public Plane2( Vector2 a_base, Vector2 a_unit_normal )
		{
			Base = a_base;
			UnitNormal = a_unit_normal;
		}

		/// <summary>
		/// Return true if 'point' is inside the primitive (in its negative space).
		/// </summary>
		public bool IsInside( Vector2 point )
		{
			return SignedDistance( point ) <= 0.0f;
		}

		/// <summary>
		/// Return the closest point to 'point' that lies on the surface of the primitive.
		/// If that point is inside the primitive, sign is negative.
		/// </summary>
		public void ClosestSurfacePoint( Vector2 point, out Vector2 ret, out float sign )
		{
			float d = SignedDistance( point );
			ret = point - d * UnitNormal;
			sign = d > 0.0f ? 1.0f : -1.0f;
		}

		/// <summary>
		/// Return the signed distance (penetration distance) from 'point' 
		/// to the surface of the primitive.
		/// </summary>
		public float SignedDistance( Vector2 point )
		{
			return( point - Base ).Dot( UnitNormal );
		}

		/// <summary>
		/// Project a point on this plane.
		/// </summary>
		public Vector2 Project( Vector2 point )
		{
			return point - SignedDistance( point ) * UnitNormal;
		}

		/// <summary>
		/// Assuming the primitive is convex, clip the segment AB against the primitive.
		/// Return false if AB is entirely in positive halfspace,
		/// else clip against negative space and return true.
		/// </summary>
		public bool NegativeClipSegment( ref Vector2 A, ref Vector2 B )
		{
			float dA = SignedDistance( A );
			float dB = SignedDistance( B );

			bool Ain = ( dA >= 0.0f );
			bool Bin = ( dB >= 0.0f );

			if ( Ain && Bin )
				return false;

			if ( Ain && (!Bin) )
			{
				Vector2 AB = B - A;
				float alpha = -dA / AB.Dot( UnitNormal );
				Vector2 I = A + alpha * AB;
				A = I;
			}
			else if ( (!Ain) && Bin )
			{
				Vector2 AB = B - A;
				float alpha = -dA / AB.Dot( UnitNormal );
				Vector2 I = A + alpha * AB;
				B = I;
			}

			return true;
		}
	}
}

