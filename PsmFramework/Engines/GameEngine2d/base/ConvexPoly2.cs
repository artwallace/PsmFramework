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
	/// <summary>A convex polygon class in 2D.</summary>
	public struct ConvexPoly2 : ICollisionBasics
	{
		/// <summary>
		/// The convex poly is stored as a list of planes assumed to define a 
		/// convex region. Plane base points are also polygon vertices.
		/// </summary>
		public Plane2[] Planes; 

		Sphere2 m_sphere;

		/// <summary>Bounding sphere, centered at center of mass.</summary>
		public Sphere2 Sphere{ get { return m_sphere; } }

		/// <summary>
		/// ConvexPoly2 constructor.
		/// Assumes input points define a convex region.
		/// </summary>
		public ConvexPoly2( Vector2[] points )
		{
			Vector2 center = GameEngine2D.Base.Math._00;
			Planes = new Plane2[ points.Length ];

			for ( int n=points.Length,i=n-1,i_next=0; i_next < n; i=i_next++ )
			{
				Vector2 p1 = points[i];
				Vector2 p2 = points[i_next];

				Planes[i] = new Plane2( p1, -Math.Perp( p2 - p1 ).Normalize() );

				center += p1;
			}

			center /= (float)points.Length;

			float radius = 0.0f;
			for ( int i = 0; i != points.Length; ++i )
				radius = FMath.Max( radius, ( points[i] - center ).Length() );

			m_sphere = new Sphere2( center, radius );
		}


		public void MakeBox( Bounds2 bounds )
		{
			Planes = new Plane2[ 4 ];

			Planes[0] = new Plane2( bounds.Point00, -Math._10 );
			Planes[1] = new Plane2( bounds.Point10, -Math._01 );
			Planes[2] = new Plane2( bounds.Point11, Math._10 );
			Planes[3] = new Plane2( bounds.Point01, Math._01 );

			m_sphere = new Sphere2( bounds.Center, bounds.Size.Length() * 0.5f );
		}


		public void MakeRegular( uint num, float r )
		{
			Planes = new Plane2[ num ];

			float a2 = Math.TwicePi * 0.5f / (float)num;

			for ( uint i = 0; i != num; ++i )
			{
				float a = Math.TwicePi * (float)i / (float)num;
				Vector2 p = Vector2.Rotation( a + a2 );
				Vector2 n = Vector2.Rotation( a );
				Planes[i] = new Plane2( p * r, n );
			}

			m_sphere = new Sphere2( GameEngine2D.Base.Math._00, r );
		}

		/// <summary>Return the number of vertices (or faces)</summary>
		public uint Size()
		{
			return(uint)Planes.Length;
		}

		/// <summary>Get a vertex position.</summary>
		/// <param name="index">The vertex index.</param>
		public Vector2 GetPoint( int index )
		{
			return Planes[index].Base;
		}

		/// <summary>Get the normal vector of a face of this poly.</summary>
		/// <param name="index">The face index.</param>
		public Vector2 GetNormal( int index )
		{
			return Planes[index].UnitNormal;
		}

		/// <summary>Get the plane formed by a face of this poly.</summary>
		/// <param name="index">The face index.</param>
		public Plane2 GetPlane( int index )
		{
			return Planes[index];
		}

		/// <summary>Calculate the bounds of this poly.</summary>
		public Bounds2 CalcBounds()
		{
			if ( Size() == 0 )
				return Bounds2.Zero;

			Bounds2 retval = new Bounds2( GetPoint( 0 ), GetPoint( 0 ) );
			for ( int i = 1; i != (int)Size(); ++i )
				retval.Add( GetPoint( i ) );
			return retval;
		}

		/// <summary>Calculate the gravity center of this poly.</summary>
		public Vector2 CalcCenter()
		{
			Vector2 center = GameEngine2D.Base.Math._00;
			float area = 0.0f;

			for ( int n=(int)Size(),i=n-1,i_next=0; i_next < n; i=i_next++ )
			{
				Vector2 A = GetPoint( i );
				Vector2 B = GetPoint( i_next );
				float det = Math.Det( A, B );
				area += det;
				center += det * ( A + B );
			}

			area /= 2.0f;
			center /= ( 6.0f * area );

			return center;
		}

		/// <summary>Calculate the area of this convex poly.</summary>
		public float CalcArea()
		{
			float area = 0.0f;

			for ( int n=(int)Size(),i=n-1,i_next=0; i_next < n; i=i_next++ )
				area += Math.Det( GetPoint( i ), GetPoint( i_next ) );

			area /= 2.0f;

			return area;
		}

		/// <summary>
		/// Return true if 'point' is inside the primitive (in its negative space).
		/// </summary>
		public bool IsInside( Vector2 point ) 
		{
			foreach ( Plane2 plane in Planes )
			{
				if ( plane.SignedDistance( point ) > 0.0f )
					return false;
			}

			return true;
		}

		/// <summary>
		/// Return the closest point to 'point' that lies on the surface of the primitive.
		/// If that point is inside the primitive, sign is negative.
		/// </summary>
		public void ClosestSurfacePoint( Vector2 point, out Vector2 ret, out float sign )
		{
			ret = GameEngine2D.Base.Math._00;

			float max_neg_d = -100000.0f;
			int max_neg_d_plane_index = -1;
			bool outside = false;

			for ( int i=0; i != Planes.Length; ++i )
			{
				float d = Planes[i].SignedDistance( point );

				if ( d > 0.0f )	outside = true;
				else if ( max_neg_d < d )
				{
					max_neg_d = d;
					max_neg_d_plane_index = i;
				}
			}

			if ( !outside )
			{
				sign = -1.0f;
				ret = point - max_neg_d * Planes[max_neg_d_plane_index].UnitNormal;
				return;
			}

			// brute force

			float d_sqr_min = 0.0f;

			for ( int n=(int)Size(),i=n-1,i_next=0; i_next < n; i=i_next++ )
			{
				Vector2 p = Math.ClosestSegmentPoint( point, GetPoint( i ), GetPoint( i_next ) );
				float d_sqr = ( p - point ).LengthSquared();

				if ( i==n-1 || d_sqr < d_sqr_min )
				{
					ret = p;
					d_sqr_min = d_sqr;
				}
			}

			sign = 1.0f;
		}

		/// <summary>
		/// Return the signed distance (penetration distance) from 'point' 
		/// to the surface of the primitive.
		/// </summary>
		public float SignedDistance( Vector2 point )
		{
			Vector2 p;
			float s = 0.0f;
			ClosestSurfacePoint( point, out p, out s );
			return s * ( p - point ).Length();
		}


		public void Translate( Vector2 dx, ConvexPoly2 poly )
		{
			Planes = new Plane2[ poly.Planes.Length ];

			for ( int i=0; i != poly.Planes.Length; ++i )
			{
				Planes[i] = poly.Planes[i];
				Planes[i].Base += dx;
			}

			m_sphere = poly.m_sphere;
			m_sphere.Center += dx;
		}

		/// <summary>
		/// Assuming the primitive is convex, clip the segment AB against the primitive.
		/// Return false if AB is entirely in positive halfspace,
		/// else clip against negative space and return true.
		/// </summary>
		public bool NegativeClipSegment( ref Vector2 A, ref Vector2 B ) 
		{
			for ( int i=0; i != Planes.Length; ++i )
			{
				if ( !Planes[i].NegativeClipSegment( ref A, ref B ) )
					return false;
			}

			return true;
		}
	}
}

