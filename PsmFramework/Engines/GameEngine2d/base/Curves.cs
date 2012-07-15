/* SCE CONFIDENTIAL
 * PlayStation(R)Suite SDK 0.98.2
 * Copyright (C) 2012 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using Sce.PlayStation.Core;
using System.Collections;
using System.Collections.Generic;

namespace Sce.PlayStation.HighLevel.GameEngine2D.Base
{
	public class Curves
	{
		static Matrix4 HermiteBasis = new Matrix4(
			new Vector4( 2.0f,-2.0f, 1.0f, 1.0f),
			new Vector4(-3.0f, 3.0f,-2.0f,-1.0f),
			new Vector4( 0.0f, 0.0f, 1.0f, 0.0f),
			new Vector4( 1.0f, 0.0f, 0.0f, 0.0f)
			);

		static Matrix4 BezierBasis = new Matrix4(
			new Vector4(-1.0f, 3.0f,-3.0f, 1.0f),
			new Vector4( 3.0f,-6.0f, 3.0f, 0.0f),
			new Vector4(-3.0f, 3.0f, 0.0f, 0.0f),
			new Vector4( 1.0f, 0.0f, 0.0f, 0.0f)
			) ;

		/// <summary>
		/// </summary>
		/// Hermite spline evaluation given 2 scalar and 2 gradients. Parameter u is in [0,1].
		/// <param name="v">
		/// v.x = value at 0
		/// v.y = value at 1
		/// v.z = gradient at 0
		/// v.w = gradient at 1
		/// </param>
		/// <param name="u">Curve parameter in [0,1].</param>
		public static float Hermite( float u, Vector4 v )
		{
			float u_sqr = u * u;
			return( HermiteBasis * new Vector4( u * u_sqr, u_sqr, u, 1.0f ) ).Dot( v );
		}

		/// <summary>
		/// Hermite spline evaluation, given 2 points in xy and their tangents
		/// (p2's x value must be superior to p0's x value).
		/// </summary>
		/// <param name="x">The curve parameter.</param>
		/// <param name="p0">Left point.</param>
		/// <param name="p1">Right point.</param>
		/// <param name="t0">Tangent at p0.</param>
		/// <param name="t1">Tangent at p2.</param>
		public static float Hermite(float x, Vector2 p0, Vector2 p1, float t0, float t1)
		{
			float dx = ( p1.X - p0.X );
			Common.Assert( dx > 0.0f );
			return Hermite( ( x - p0.X ) / dx, new Vector4( p0.Y, p1.Y, dx * t0, dx * t1 ) );
		}

		/// <summary>
		/// </summary>
		/// Hermite spline evaluation, given 4 points in xy.
		/// <param name="x">Eval parameter.</param>
		/// <param name="p0">Start point.</param>
		/// <param name="p2">End point.</param>
		/// <param name="p01">Vector p0,p01 defines the tangent at p0 (lenght matters).</param>
		/// <param name="p21">Vector p2,p21 defines the tangent at p2 (lenght matters).</param>
		public static
		float Hermite( float x
					 , Vector2 p0
					 , Vector2 p2
					 , Vector2 p01
					 , Vector2 p21 )
		{
			Common.Assert( p2.X > p0.X );
			Common.Assert( p01.X > p0.X );
			Common.Assert( p21.X > p2.X );

			return Hermite(
				x, p0, p2,
				( p01.Y - p0.Y ) / ( p01.X - p0.X ),
				( p21.Y - p2.Y ) / ( p21.X - p2.X )
				);
		}

		/// <summary>Cubic bezier, Vector2 control points.</summary>
		public static
		Vector2 Bezier( float t, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3 )
		{
			float t_c = 1.0f-t; 
			return( t_c * t_c ) * ( p0 * t_c + p1 * 3.0f * t )
			+ ( t * t ) * ( p2 * 3.0f * t_c + p3 * t );
		}

		/// <summary>Cubic bezier, Vector3 control points.</summary>
		public static
		Vector3 Bezier( float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3 )
		{
			float t_c = 1.0f-t; 
			return( t_c * t_c ) * ( p0 * t_c + p1 * 3.0f * t )
			+ ( t * t ) * ( p2 * 3.0f * t_c + p3 * t );
		}

		/// <summary>Cubic bezier, Vector4 control points.</summary>
		public static
		Vector4 Bezier( float t, Vector4 p0, Vector4 p1, Vector4 p2, Vector4 p3 )
		{
			float t_c = 1.0f-t; 
			return( t_c * t_c ) * ( p0 * t_c + p1 * 3.0f * t )
			+ ( t * t ) * ( p2 * 3.0f * t_c + p3 * t );
		}

		/// <summary>Catmull-Rom, Vector2 control points.</summary>
		public static
		Vector2 CatmullRom( float t, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3 )
		{
			float t2 = t * t;
			float t3 = t2 * t;
			return 0.5f * ( ( 2.0f * p1 ) 
							+ ( -p0 + p2 ) * t 
							+ ( 2.0f * p0 - 5.0f * p1 + 4.0f * p2 - p3 ) * t2 
							+ ( -p0 + 3.0f * p1- 3.0f * p2 + p3 ) * t3 );
		}

		/// <summary>Catmull-Rom, Vector3 control points.</summary>
		public static
		Vector3 CatmullRom( float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3 )
		{
			float t2 = t * t;
			float t3 = t2 * t;
			return 0.5f * ( ( 2.0f * p1 ) 
							+ ( -p0 + p2 ) * t 
							+ ( 2.0f * p0 - 5.0f * p1 + 4.0f * p2 - p3 ) * t2 
							+ ( -p0 + 3.0f * p1- 3.0f * p2 + p3 ) * t3 );
		}

		/// <summary>Catmull-Rom, Vector4 control points.</summary>
		public static
		Vector4 CatmullRom( float t, Vector4 p0, Vector4 p1, Vector4 p2, Vector4 p3 )
		{
			float t2 = t * t;
			float t3 = t2 * t;
			return 0.5f * ( ( 2.0f * p1 ) 
							+ ( -p0 + p2 ) * t 
							+ ( 2.0f * p0 - 5.0f * p1 + 4.0f * p2 - p3 ) * t2 
							+ ( -p0 + 3.0f * p1- 3.0f * p2 + p3 ) * t3 );
		}

		/// <summary>
		/// Catmull-Rom curve evaluation for 4 Vector2 control points.
		/// Return a Vector4 with position in xy and tangent in zw. Just apply Math.Perp to the tangent to get the normal vector.
		/// </summary>
		public static
		Vector4 CatmullRomAndDerivative( float t, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3 )
		{
			Vector4 c;
			c.X = 1.0f;
			c.Y = t;
			c.Z = t * t;
			c.W = c.Z * t;

			Vector2 c0 = p1;
			Vector2 c1 = 0.5f * ( -p0 + p2 );
			Vector2 c2 = 0.5f * ( 2.0f * p0 - 5.0f * p1 + 4.0f * p2 - p3 );
			Vector2 c3 = 0.5f * ( -p0 + 3.0f * p1- 3.0f * p2 + p3 );

			Vector4 ret = new Vector4();

			ret.Xy = c0 + 
					 c1 * c.Y + 
					 c2 * c.Z + 
					 c3 * c.W ;

			ret.Zw = c1 + 
					 c2 * c.Y * 2.0f + 
					 c3 * c.Z * 3.0f ;

			return ret;
		}

		/// <summary>
		/// Piecewise cubic bezier curve.
		/// </summary>
		/// <param name="t">The curve parameter.</param>
		/// <param name="p0">Control point 0.</param>
		/// <param name="p1">Control point 1.</param>
		/// <param name="p2">Control point 2.</param>
		/// <param name="p3">Control point 3.</param>
		/// <param name="r">Tangent control.</param>
		public static
		Vector2 BezierAuto( float t, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3
							, float r = 1.0f/3.0f )
		{
			float len = ( p2 -p1 ).Length();

			Vector2 P11 = p1 + ( p2 - p0 ).Normalize() * len * r;
			Vector2 P22 = p2 - ( p3 - p1 ).Normalize() * len * r;

			return Bezier( t, p1, P11, P22, p2 );
		}

		static 
		Matrix4 UniformCubicBspline = new Matrix4( new Vector4( -1.0f,3.0f, -3.0f, 1.0f) / 6.0f,
												 new Vector4( 3.0f,-6.0f,3.0f,0.0f) / 6.0f,
												 new Vector4( -3.0f, 0.0f, 3.0f, 0.0f) / 6.0f,
												 new Vector4( 1.0f, 4.0f, 1.0f, 0.0f) / 6.0f );

		/// <summary>
		/// B-Spline curve evaluation for 4 Vector2 control points.
		/// </summary>
		public static 
		Vector2 Bspline( float t, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3 )
		{
			float t2 = t * t;
			Matrix4 m = new Matrix4( p0.Xy01, p1.Xy01, p2.Xy01, p3.Xy01 );
			return( m * ( UniformCubicBspline * new Vector4( t2 * t, t2, t, 1.0f ) ) ).Xy;
		}

		/// <summary>
		/// B-Spline curve evaluation for 4 Vector4 control points.
		/// </summary>
		public static 
		Vector4 Bspline( float t, Vector4 p0, Vector4 p1, Vector4 p2, Vector4 p3 )
		{
			float t2 = t * t;
			Matrix4 m = new Matrix4( p0, p1, p2, p3 );
			return m * ( UniformCubicBspline * new Vector4( t2 * t, t2, t, 1.0f ) );
		}

		/// <summary>
		/// B-Spline curve evaluation for 4 Vector2 control points.
		/// Return a Vector4 with position in xy and tangent in zw. Just apply Math.Perp to the tangent to get the normal vector.
		/// </summary>
		public static 
		Vector4 BsplineAndDerivative( float t, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3 )
		{
			float t2 = t * t;
			Matrix4 m = new Matrix4( p0.Xy01, p1.Xy01, p2.Xy01, p3.Xy01 );
			Vector4 ret = new Vector4();
			ret.Xy = ( m * ( UniformCubicBspline * new Vector4( t2 * t, t2, t, 1.0f ) ) ).Xy;
			ret.Zw = ( m * ( UniformCubicBspline * new Vector4( 3.0f * t2, 2.0f * t, 1.0f, 0.0f ) ) ).Xy;
			return ret;
		}

		internal struct CurveEvalSetup
		{
			internal float pf;
			internal int pi;
			internal float frac;

			internal CurveEvalSetup( float u, bool _4_points_setup, bool loop, float num_pointsf )
			{
				if ( _4_points_setup && !loop )
				{
					float h = 1.0f / ( num_pointsf - 1.0f );
					u = h + u * ( 1.0f - 2.0f * h );
				}
				pf = u * ( num_pointsf - ( loop ? 0.0f : 1.0f ) );
				pi = (int)FMath.Floor( pf );
				frac = pf - (float)pi;
			}
		}

		/// <summary>
		/// Catmull-Rom curve evaluation for n Vector2 control points (position only).
		/// </summary>
		public static 
		Vector2 CatmullRom( float t, List<Vector2> points, bool loop )
		{
			int n = points.Count;

			if ( n == 0 )
				return Math._00;

			Common.IndexWrapMode iwrap = Common.ClampIndex; 
			if ( loop )	iwrap = Common.WrapIndex;

			CurveEvalSetup setup = new CurveEvalSetup( t, true, loop, (float)n );

			return CatmullRom( setup.frac
							 , points[ iwrap( setup.pi - 1, n ) ]
							 , points[ iwrap( setup.pi - 0, n ) ]
							 , points[ iwrap( setup.pi + 1, n ) ]
							 , points[ iwrap( setup.pi + 2, n ) ] );
		}

		/// <summary>
		/// Catmull-Rom curve evaluation (with derivative) for n Vector2 control points (position only).
		/// Return position in xy, tangent in zw. Just apply Math.Perp to the tangent to get the normal.
		/// </summary>
		public static 
		Vector4 CatmullRomAndDerivative( float t, List<Vector2> points, bool loop )
		{
			int n = points.Count;

			if ( n == 0 )
				return Math._0000;

			Common.IndexWrapMode iwrap = Common.ClampIndex; 
			if ( loop )	iwrap = Common.WrapIndex;

			CurveEvalSetup setup = new CurveEvalSetup( t, true, loop, (float)n );

			return CatmullRomAndDerivative( setup.frac
											, points[ iwrap( setup.pi - 1, n ) ]
											, points[ iwrap( setup.pi - 0, n ) ]
											, points[ iwrap( setup.pi + 1, n ) ]
											, points[ iwrap( setup.pi + 2, n ) ] );
		}

		/// <summary>
		/// Bspline curve evaluation for n Vector2 control points (position only).
		/// </summary>
		public static 
		Vector2 Bspline( float t, List<Vector2> points, bool loop )
		{
			int n = points.Count;

			if ( n == 0 )
				return Math._00;

			Common.IndexWrapMode iwrap = Common.ClampIndex; 
			if ( loop )	iwrap = Common.WrapIndex;

			CurveEvalSetup setup = new CurveEvalSetup( t, true, loop, (float)n );

			return Bspline( setup.frac
							, points[ iwrap( setup.pi - 1, n ) ]
							, points[ iwrap( setup.pi - 0, n ) ]
							, points[ iwrap( setup.pi + 1, n ) ]
							, points[ iwrap( setup.pi + 2, n ) ] );
		}

		/// <summary>
		/// Bspline curve evaluation (with derivative) for n Vector2 control points.
		/// Return position in xy, tangent in zw. Just apply Math.Perp to the tangent to get the normal.
		/// </summary>
		public static 
		Vector4 BsplineAndDerivative( float t, List<Vector2> points, bool loop )
		{
			int n = points.Count;

			if ( n == 0 )
				return Math._0000;

			Common.IndexWrapMode iwrap = Common.ClampIndex; 
			if ( loop )	iwrap = Common.WrapIndex;

			CurveEvalSetup setup = new CurveEvalSetup( t, true, loop, (float)n );

			return BsplineAndDerivative( setup.frac
										 , points[ iwrap( setup.pi - 1, n ) ]
										 , points[ iwrap( setup.pi - 0, n ) ]
										 , points[ iwrap( setup.pi + 1, n ) ]
										 , points[ iwrap( setup.pi + 2, n ) ] );
		}
	}
}

