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
	/// <summary>
	/// An axis aligned box class in 2D.
	/// </summary>
	public struct Bounds2 : ICollisionBasics
	{
		/// <summary>Minimum point (lower left).</summary>
		public Vector2 Min;

		/// <summary>Maximum point (upper right)</summary>
		public Vector2 Max;

		/// <summary>The Width/Height ratio.</summary>
		public float Aspect { get { Vector2 size = Size; return size.X / size.Y;}} 

		/// <summary>The center of the bounds (Max+Min)/2.</summary>
		public Vector2 Center { get { return( Max + Min ) * 0.5f;}} 

		/// <summary>The Size the bounds (Max-Min).</summary>
		public Vector2 Size { get { return( Max - Min );}} 

		/// <summary>Return true if the size is (0,0).</summary>
		public bool IsEmpty()
		{
			return Max == Min;
		}

		/// <summary>
		/// Bounds2 constructor.
		/// All functions in Bounds2 assume that Min is less or equal Max. If it is not the case, the user takes responsability for it.
		/// SafeBounds will ensure this is the case whatever the input is, but the default constructor will just blindly
		/// takes anything the user passes without trying to fix it.
		/// </summary>
		/// <param name="min">The bottom left point. Min is set to that value without further checking.</param>
		/// <param name="max">The top right point. Max is set to that value without further checking.</param>
		public Bounds2( Vector2 min, Vector2 max )
		{
			Min = min; 
			Max = max;
		}

		/// <summary>
		/// Bounds2 constructor. 
		/// Return a zero size bounds. You can then use Add to expand it. 
		/// </summary>
		/// <param name="point">Location of the Bounds2.</param>
		public Bounds2( Vector2 point )
		{
			Min = point; 
			Max = point;
		}

		/// <summary>
		/// Create a Bounds2 that goes through 2 points, the min and max are recalculated.
		/// </summary>
		/// <param name="min">First point.</param>
		/// <param name="max">Second point.</param>
		static public Bounds2 SafeBounds( Vector2 min, Vector2 max )
		{
			return new Bounds2( min.Min( max ), min.Max( max ) );
		}

		/// <summary>(0,0) -> (0,0) box.</summary>
		static public Bounds2 Zero = new Bounds2( new Vector2(0.0f,0.0f), new Vector2(0.0f,0.0f) );

		/// <summary>(0,0) -> (1,1) box.</summary>
		static public Bounds2 Quad0_1 = new Bounds2( new Vector2(0.0f,0.0f), new Vector2(1.0f,1.0f) );

		/// <summary>(-1,-1) -> (1,1) box.</summary>
		static public Bounds2 QuadMinus1_1 = new Bounds2( new Vector2(-1.0f,-1.0f), new Vector2(1.0f,1.0f) );

		/// <summary>
		/// Return a box that goes from (-h,-h) to (h,h).
		/// We don't check for sign.
		/// </summary>
		/// <param name="h">Half size of the square.</param>
		static public Bounds2 CenteredSquare( float h )
		{
			Vector2 half_vec = new Vector2( h, h );
			return new Bounds2( -half_vec, half_vec );
		}

		/// <summary>Translate bounds.</summary>
		public static Bounds2 operator + ( Bounds2 bounds, Vector2 value )
		{
			return new Bounds2( bounds.Min + value, 
								bounds.Max + value );
		}

		/// <summary>Translate bounds.</summary>
		public static Bounds2 operator - ( Bounds2 bounds, Vector2 value )
		{
			return new Bounds2( bounds.Min - value, 
								bounds.Max - value );
		}

		/// <summary>Return true if this and 'bounds' overlap.</summary>
		public bool Overlaps( Bounds2 bounds )
		{
			if ( Min.X > bounds.Max.X || bounds.Min.X > Max.X )	return false;
			if ( Min.Y > bounds.Max.Y || bounds.Min.Y > Max.Y )	return false;

			return true;
		}

		/// <summary>Return the Bounds2 resulting from the intersection of 2 bounds.</summary>
		public Bounds2 Intersection( Bounds2 bounds ) 
		{
			Vector2 mi = Min.Max( bounds.Min );
			Vector2 ma = Max.Min( bounds.Max );

			Vector2 dim = ma - mi;

			if ( dim.X < 0.0f || dim.Y < 0.0f )
				return Zero;

			return new Bounds2( mi, ma );
		} 

		/// <summary>
		/// Scale bounds around a given pivot.
		/// </summary>
		/// <param name="scale">Amount of scale.</param>
		/// <param name="center">Scale center.</param>
		public Bounds2 Scale( Vector2 scale, Vector2 center )
		{
			return new Bounds2( ( Min - center ) * scale + center, 
								( Max - center ) * scale + center );
		}

		/// <summary>Add the contribution of 'point' to this Bounds2.</summary>
		public void Add( Vector2 point )
		{
			Min = Min.Min( point );
			Max = Max.Max( point );
		}

		/// <summary>Add the contribution of 'bounds' to this Bounds2.</summary>
		public void Add( Bounds2 bounds )
		{
			Add( bounds.Min );
			Add( bounds.Max );
		}

		// Note about PointXX: first column is x, second is y (0 means min, 1 means max, you can also see those as 'uv')

		/// <summary>The botton left point (which is also Min).</summary>
		public Vector2 Point00 { get { return Min;}}
		/// <summary>The top right point (which is also Max).</summary>
		public Vector2 Point11 { get { return Max;}}
		/// <summary>The bottom right point.</summary>
		public Vector2 Point10 { get { return new Vector2(Max.X,Min.Y);}}
		/// <summary>The top left point.</summary>
		public Vector2 Point01 { get { return new Vector2(Min.X,Max.Y);}}

		/// <summary>Return the string representation of this Bounds2.</summary>
		public override string ToString()
		{
			return Min.ToString() + " " + Max.ToString();
		}

		/// <summary>
		/// Return true if 'point' is inside the primitive (in its negative space).
		/// </summary>
		public bool IsInside( Vector2 point )
		{
			return point == point.Max( Min ).Min( Max );
		}

		/// <summary>
		/// Return the closest point to 'point' that lies on the surface of the primitive.
		/// If that point is inside the primitive, sign is negative.
		/// </summary>
		public void ClosestSurfacePoint( Vector2 point, out Vector2 ret, out float sign )
		{
			Vector2 closest = point.Max( Min ).Min( Max );

			if ( closest != point )
			{
				ret = closest;
				sign = 1.0f;
				return;
			}

			Vector2 l = closest; l.X = Min.X; float dl = closest.X - Min.X; 
			Vector2 r = closest; r.X = Max.X; float dr = Max.X - closest.X; 
			Vector2 t = closest; t.Y = Min.Y; float dt = closest.Y - Min.Y; 
			Vector2 b = closest; b.Y = Max.Y; float db = Max.Y - closest.Y; 

			ret = l; 
			float d = dl;

			if ( d > dr )
			{
				ret = r; 
				d = dr;
			}

			if ( d > dt )
			{
				ret = t; 
				d = dt;
			}

			if ( d > db )
			{
				ret = b; 
				d = db;
			}

			sign = -1.0f;
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

		/// <summary>
		/// Assuming the primitive is convex, clip the segment AB against the primitive.
		/// Return false if AB is entirely in positive halfspace,
		/// else clip against negative space and return true.
		/// </summary>
		public bool NegativeClipSegment( ref Vector2 A, ref Vector2 B )
		{
			bool ret = true;

			ret &= ( new Plane2( Min, -Math._10 ) ).NegativeClipSegment( ref A, ref B );
			ret &= ( new Plane2( Min, -Math._01 ) ).NegativeClipSegment( ref A, ref B );
			ret &= ( new Plane2( Max, Math._10 ) ).NegativeClipSegment( ref A, ref B );
			ret &= ( new Plane2( Max, Math._01 ) ).NegativeClipSegment( ref A, ref B );

			return ret;
		}

		/// <summary>
		/// Swap y coordinates for top and bottom, handy for hacking uvs
		/// in system that use 0,0 as top left. Also, this will generate
		/// an invalid Bounds2 and all functions in that class will break
		/// (intersections, add etc.)
		/// 
		/// Functions like Point00, Point10 etc can still be used.
		/// </summary>
		public Bounds2 OutrageousYTopBottomSwap()
		{
			Bounds2 ret = this;
			float y = Min.Y;
			ret.Min.Y = ret.Max.Y;
			ret.Max.Y = y;
			return ret;
		}

		/// <summary>
		/// Similar to OutrageousYTopBottomSwap, but instead of
		/// swapping top and bottom y, it just does y=1-y. Same
		/// comment as OutrageousYTopBottomSwap.
		/// </summary>
		public Bounds2 OutrageousYVCoordFlip()
		{
			Bounds2 ret = this;
			ret.Min.Y = 1.0f - ret.Min.Y;
			ret.Max.Y = 1.0f - ret.Max.Y;
			return ret;
		}
	}
}
