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
	/// <summary>Some extensions to the math/vector lib</summary>
	public static class Math
	{

		public static float Pi { get { return 3.141592654f;}}
		/// <summary>2pi</summary>
		public static float TwicePi { get { return 6.283185307f;}}
		/// <summary>pi/2</summary>
		public static float HalfPi { get { return 1.570796327f;}}

		/// <summary>Wrap System.Random and extand with vector random generation.</summary>
		public class RandGenerator
		{
			/// <summary>The raw random generator.</summary>
			public System.Random Random;
			
			public RandGenerator( int seed = 0 )
			{
				Random = new System.Random( seed );
			}
			/// <summary>Return a random float in 0,1.</summary>
			public float NextFloat0_1()
			{
				return (float)Random.NextDouble();
			}
			/// <summary>Return a random float in -1,1.</summary>
			public float NextFloatMinus1_1()
			{
				return ( NextFloat0_1() * 2.0f ) - 1.0f;
			}
			/// <summary>Return a random float in mi,ma.</summary>
			public float NextFloat( float mi, float ma )
			{
				return mi + ( ma - mi ) * NextFloat0_1();
			}
			/// <summary>Return a random Vector2 -1,1.</summary>
			public Vector2 NextVector2Minus1_1()
			{
				return new Vector2( NextFloat( -1.0f, 1.0f ), 
									NextFloat( -1.0f, 1.0f ) );
			}
			/// <summary>Return a random Vector2 in mi,ma.</summary>
			public Vector2 NextVector2( Vector2 mi, Vector2 ma )
			{
				return new Vector2( NextFloat( mi.X, ma.X ), 
									NextFloat( mi.Y, ma.Y ) );
			}
			/// <summary>Return a random Vector2 in mi,ma.</summary>
			public Vector2 NextVector2( float mi, float ma )
			{
				return new Vector2( NextFloat( mi, ma ), 
									NextFloat( mi, ma ) );
			}
			/// <summary>Return a random Vector3 in mi,ma.</summary>
			public Vector3 NextVector3( Vector3 mi, Vector3 ma )
			{
				return new Vector3( NextFloat( mi.X, ma.X ),
									NextFloat( mi.Y, ma.Y ),
									NextFloat( mi.Z, ma.Z ) );
			}
			/// <summary>Return a random Vector4 in mi,ma.</summary>
			public Vector4 NextVector4( Vector4 mi, Vector4 ma )
			{
				return new Vector4( NextFloat( mi.X, ma.X ),
									NextFloat( mi.Y, ma.Y ),
									NextFloat( mi.Z, ma.Z ), 
									NextFloat( mi.W, ma.W ) );
			}
			/// <summary>Return a random Vector4 in mi,ma.</summary>
			public Vector4 NextVector4( float mi, float ma )
			{
				return new Vector4( NextFloat( mi, ma ),
									NextFloat( mi, ma ),
									NextFloat( mi, ma ), 
									NextFloat( mi, ma ) );
			}
		}

		/// <summary>
		/// Compute a lookat matrix for the camera. The vector (eye, center)
		/// maps to -z (since OpenGL looks downward z), and up maps to y.
		/// </summary>
		public static Matrix4 LookAt( Vector3 eye, Vector3 center, Vector3 _Up )
		{
			Vector3 up = _Up.Normalize();

			Common.Assert( up.IsUnit(1.0e-3f) );

			Vector3 x,y,z;

			float EPSILON=1.0e-5f;

			if ( ( eye - center ).Length() > EPSILON )
			{
				z = ( eye - center ).Normalize();

				if ( FMath.Abs( z.Dot( up ) ) > 0.9999f )
				{
					y = z.Perpendicular();
					x = y.Cross( z );
				}
				else
				{
					x = up.Cross( z ).Normalize();
					y = z.Cross( x );
				}
			}
			else
			{
				y = up;
				x = y.Perpendicular();	// use any vector perpendicular to y
				z = x.Cross( y );
			}

			Matrix4 retval = new Matrix4() 
			{ 
				ColumnX = x.Xyz0, 
				ColumnY = y.Xyz0, 
				ColumnZ = z.Xyz0, 
				ColumnW = eye.Xyz1
			};

			Common.Assert( retval.IsOrthonormal( 1.0e-3f ) );

			return retval;
		}

		/// <summary>Fast build of a Matrix3 TRS matrix.</summary>
		public static void TranslationRotationScale( ref Matrix3 ret, Vector2 translation, Vector2 rotation, Vector2 scale )
		{
			ret.X = new Vector3( rotation.X * scale.X, rotation.Y * scale.X, 0.0f );
			ret.Y = new Vector3( -rotation.Y * scale.Y, rotation.X * scale.Y, 0.0f );
			ret.Z = translation.Xy1;
		}

		/// <summary>Return the determinant formed by 2 vectors.</summary>
		public static
		float Det( Vector2 value1, Vector2 value2 ) 
		{
			return value1.X * value2.Y - value1.Y * value2.X;
		}

		/// <summary>Return the sign of x (returns 0.0f is x=0.0f).</summary>
		public static float Sign( float x ) 
		{
			if ( x < 0.0f ) return -1.0f;
			if ( x > 0.0f ) return 1.0f;
			return 0.0f;
		}

		/// <summary>Return value rotated by pi/2.</summary>
		public static Vector2 Perp( Vector2 value )
		{
			return new Vector2( -value.Y, value.X );
		}

		/// <summary>Set alpha (can be inlined in math expressions).</summary>
		public static Vector4 SetAlpha( Vector4 value, float w )
		{
			value.W = w;
			return value;
		}

		/// <summary>SaveAcos checks that x is in [-1,1], and if x is off by an epsilon it clamps it.</summary>
		public static float SafeAcos( float x )
		{
			Common.Assert( FMath.Abs( x ) - 1.0f < 1.0e-5f );
			return FMath.Acos( FMath.Clamp( x, -1.0f, 1.0f ) );	// clamp if necessary (we have checked that we are in in [-1,1] by an epsilon)
		}

		/// <summary>Return the absolute 2d angle formed by (1,0) and value, in range -pi,pi</summary>
		public static float Angle( Vector2 value )
		{
			float angle = SafeAcos( value.Normalize().X );
			return value.Y < 0.0f ? -angle : angle;
		}

		/// <summary>Rotate 'point' around rotation center 'pivot' by an angle 'angle' (radians).</summary>
		public static Vector2 Rotate( Vector2 point, float angle, Vector2 pivot )
		{
			return pivot + ( point - pivot ).Rotate( angle );
		}

		/// <summary>Degree to radians.</summary>
		public static float Deg2Rad( float value )
		{
			return value * 0.01745329251f;
		}

		/// <summary>Radians to degrees.</summary>
		public static float Rad2Deg( float value )
		{
			return value * 57.29577951308f;
		}

		/// <summary>Element wise degree to radians.</summary>
		public static Vector2 Deg2Rad( Vector2 value )
		{
			return value * 0.01745329251f;
		}

		/// <summary>Element wise radians to degrees.</summary>
		public static Vector2 Rad2Deg( Vector2 value )
		{
			return value * 57.29577951308f;
		}

		public static float Lerp( float a, float b, float x )
		{
			return a + x * ( b - a );
		}

		public static Vector2 Lerp( Vector2 a, Vector2 b, float x )
		{
			return a + x * ( b - a );
		}

		public static Vector3 Lerp( Vector3 a, Vector3 b, float x )
		{
			return a + x * ( b - a );
		}

		public static Vector4 Lerp( Vector4 a, Vector4 b, float x )
		{
			return a + x * ( b - a );
		}

		/// <summary>Lerp 2 (assumed) unit vectors (shortest path).</summary>
		public static Vector2 LerpUnitVectors( Vector2 va, Vector2 vb, float x )
		{
			return va.Rotate( va.Angle( vb ) * x );
		}

		/// <summary>Lerp 2 angle values (shortest path).</summary>
		public static float LerpAngles( float a, float b, float x )
		{
			return Angle(LerpUnitVectors( Vector2.Rotation( a ), Vector2.Rotation( b ), x ) );
		}

		/// <summary>A "safe" sine function taking uint values.</summary>
		public static float Sin( uint period, float phase, uint mstime )
		{
			return FMath.Sin( ( ( ( (float)( mstime % period ) ) / period ) + phase ) * Pi * 2.0f );
		}

		/// <summary>A "safe" sine function taking ulong values.</summary>
		public static float Sin( ulong period, float phase, ulong mstime )
		{
			return FMath.Sin( ( ( ( (float)( mstime % period ) ) / period ) + phase ) * Pi * 2.0f );
		}

		/// <summary>This is just f(x)=x, named so that code is more explicit when it is passed as a tween function.</summary>
		public static float Linear( float x )
		{
			return x;
		}

		/// <summary>
		/// A very controlable s curve, lets you do polynomial ease in/out curves
		/// with little code.
		/// </summary>
		/// <param name="x">Asssumed to be in 0,1.</param>
		/// <param name="p1">Controls the ease in exponent (if >1).</param>
		/// <param name="p2">Controls the ease out exponent (if >1.,</param>
		/// (p1,p2)=(1,1) just gives f(x)=x
		public static float PowerfulScurve( float x, float p1, float p2 )
		{
			return FMath.Pow( 1.0f - FMath.Pow( 1.0f - x, p2 ), p1 );
		}

		/// <summary>Ease in curve using Pow.</summary>
		public static float PowEaseIn( float x, float p )
		{
			return FMath.Pow(x,p);
		}

		/// <summary>Ease out curve using Pow.</summary>
		public static 
		float PowEaseOut( float x, float p )
		{
			return 1.0f - PowEaseIn( 1.0f - x, p );
		}

		/// <summary>
		/// PowEaseIn/PowEaseOut mirrored around 0.5,0.5.
		/// Same exponent in and out.
		/// </summary>
		public static float PowEaseInOut( float x, float p )
		{
			if ( x < 0.5f )	return 0.5f * PowEaseIn( x * 2.0f, p );
			return 0.5f + 0.5f * PowEaseOut( ( x - 0.5f ) * 2.0f, p );
		}

		/// <summary>
		/// Ease out curve using a 1-exp(-a*x) exponential,
		/// but normalized so that we reach 1 when x=1.
		/// </summary>
		public static float ExpEaseOut( float x, float a )
		{
			return( 1.0f - FMath.Exp( - x * a ) ) / ( 1.0f - FMath.Exp( - a ) );
		}

		/// <summary>Ease in curve using an exponential.</summary>
		public static float ExpEaseIn( float x, float a )
		{
			return 1.0f - ExpEaseOut( 1.0f - x, a );
		}

		/// <summary>BackEaseIn function (see http://www.robertpenner.com)</summary>
		public static float BackEaseIn( float x, float a )
		{
			return x * x * ( ( a + 1.0f ) * x - a );
		}

		/// <summary>BackEaseOut function (see http://www.robertpenner.com)</summary>
		public static float BackEaseOut( float x, float a )
		{
			return 1.0f - BackEaseIn( 1.0f - x, a );
		}

		/// <summary>BackEaseIn/BackEaseOut mirrored around 0.5,0.5.</summary>
		public static float BackEaseInOut( float x, float p )
		{
			if ( x < 0.5f )	return 0.5f * BackEaseIn( x * 2.0f, p );
			return 0.5f + 0.5f * BackEaseOut( ( x - 0.5f ) * 2.0f, p );
		}

		/// <summary>Impulse function (source Inigo Quilez).</summary>
		public static float Impulse( float x, float b )
		{
			float h = b * x;
			return h * FMath.Exp( 1.0f - h );
		}

		/// <summary>Travelling wave function.</summary>
		public static float ShockWave( float d, float time, float wave_half_width, float wave_speed, float wave_fade, float d_scale )
		{
			d *= d_scale;
			float travelled = time * wave_speed;
			float x = FMath.Clamp( d - travelled, -wave_half_width, wave_half_width ) / wave_half_width; // -1,1 parameter
			float wave = ( 1.0f + FMath.Cos( Pi * x ) ) * 0.5f;
			return wave * FMath.Exp( -d * wave_fade );
		}

		/// <summary>Return the log of v in base 2.</summary>
		public static int Log2( int v )
		{
			int r;
			int shift;
			r = (v > 0xFFFF?1:0) << 4; v >>= r;
			shift = (v > 0xFF ?1:0) << 3; v >>= shift; r |= shift;
			shift = (v > 0xF ?1:0) << 2; v >>= shift; r |= shift;
			shift = (v > 0x3 ?1:0) << 1; v >>= shift; r |= shift;
			r |= (v >> 1);
			return r;
		}

		/// <summary>Return true if 'i' is a power of 2.</summary>
		public static bool IsPowerOf2( int i )
		{
			return ( 1 << Log2(i) ) == i;
		}

		/// <summary>Return the closest greater or equal power of 2.</summary>
		public static int GreatestOrEqualPowerOf2( int i )
		{
			int p = ( 1 << Log2(i) );
			return p < i ? 2 * p : p;
		}

		// some constants

		public static Vector2i _00i = new Vector2i(0,0);
		public static Vector2i _10i = new Vector2i(1,0);
		public static Vector2i _01i = new Vector2i(0,1);
		public static Vector2i _11i = new Vector2i(1,1);

		public static Vector3i _000i = new Vector3i(0,0,0);
		public static Vector3i _100i = new Vector3i(1,0,0);
		public static Vector3i _010i = new Vector3i(0,1,0);
		public static Vector3i _110i = new Vector3i(1,1,0);
		public static Vector3i _001i = new Vector3i(0,0,1);
		public static Vector3i _101i = new Vector3i(1,0,1);
		public static Vector3i _011i = new Vector3i(0,1,1);
		public static Vector3i _111i = new Vector3i(1,1,1);

		public static Vector2 _00 = new Vector2(0.0f,0.0f);
		public static Vector2 _10 = new Vector2(1.0f,0.0f);
		public static Vector2 _01 = new Vector2(0.0f,1.0f);
		public static Vector2 _11 = new Vector2(1.0f,1.0f);

		public static Vector3 _000 = new Vector3(0.0f,0.0f,0.0f);
		public static Vector3 _100 = new Vector3(1.0f,0.0f,0.0f);
		public static Vector3 _010 = new Vector3(0.0f,1.0f,0.0f);
		public static Vector3 _110 = new Vector3(1.0f,1.0f,0.0f);
		public static Vector3 _001 = new Vector3(0.0f,0.0f,1.0f);
		public static Vector3 _101 = new Vector3(1.0f,0.0f,1.0f);
		public static Vector3 _011 = new Vector3(0.0f,1.0f,1.0f);
		public static Vector3 _111 = new Vector3(1.0f,1.0f,1.0f);


		public static Vector4 _0000 = new Vector4(0.0f,0.0f,0.0f,0.0f);
		public static Vector4 _1000 = new Vector4(1.0f,0.0f,0.0f,0.0f);
		public static Vector4 _0100 = new Vector4(0.0f,1.0f,0.0f,0.0f);
		public static Vector4 _1100 = new Vector4(1.0f,1.0f,0.0f,0.0f);
		public static Vector4 _0010 = new Vector4(0.0f,0.0f,1.0f,0.0f);
		public static Vector4 _1010 = new Vector4(1.0f,0.0f,1.0f,0.0f);
		public static Vector4 _0110 = new Vector4(0.0f,1.0f,1.0f,0.0f);
		public static Vector4 _1110 = new Vector4(1.0f,1.0f,1.0f,0.0f);
		public static Vector4 _0001 = new Vector4(0.0f,0.0f,0.0f,1.0f);
		public static Vector4 _1001 = new Vector4(1.0f,0.0f,0.0f,1.0f);
		public static Vector4 _0101 = new Vector4(0.0f,1.0f,0.0f,1.0f);
		public static Vector4 _1101 = new Vector4(1.0f,1.0f,0.0f,1.0f);
		public static Vector4 _0011 = new Vector4(0.0f,0.0f,1.0f,1.0f);
		public static Vector4 _1011 = new Vector4(1.0f,0.0f,1.0f,1.0f);
		public static Vector4 _0111 = new Vector4(0.0f,1.0f,1.0f,1.0f);
		public static Vector4 _1111 = new Vector4(1.0f,1.0f,1.0f,1.0f);

		/// <summary>
		/// UV transform stored as (offset, scale) in a Vector4.
		/// offset=0,0 scale=1,1 means identity.
		/// </summary>
		public static Vector4 UV_TransformIdentity = new Vector4( 0.0f, 0.0f, 1.0f, 1.0f ); 

		/// <summary>
		/// UV transform stored as (offset, scale) in a Vector4
		/// UV_TransformFlipV v into 1-v, and leaves u unchanged.
		/// </summary>
		public static Vector4 UV_TransformFlipV = new Vector4( 0.0f, 1.0f, 1.0f, -1.0f );

		/// <summary>Return the closest point to P that's on segment [A,B].</summary>
		public static 
		Vector2 ClosestSegmentPoint( Vector2 P, Vector2 A, Vector2 B )
		{
			Vector2 AB = B - A;

			if ( ( P - A ).Dot( AB ) <= 0.0f ) return A;
			if ( ( P - B ).Dot( AB ) >= 0.0f ) return B;

			return P.ProjectOnLine( A, AB );
		}
	}
}
