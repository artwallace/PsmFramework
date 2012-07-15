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
	/// Integer version of Vector2.
	/// </summary>
	public struct Vector2i
	{
		/// <summary>X</summary>
		public int X;
		/// <summary>Y</summary>
		public int Y;

		/// <summary>Vector2i constructor.</summary>
		public Vector2i( int x, int y ) 
		{
			X = x;
			Y = y;
		}

		/// <summary>Return this as a Vector2 (cast to float).</summary>
		public Vector2 Vector2()
		{
			return new Vector2( (float)X, (float)Y );
		}

		/// <summary>Element wise max.</summary>
		public Vector2i Max( Vector2i value )
		{
			return new Vector2i( Common.Max( X, value.X ), Common.Max( Y, value.Y ) );
		}

		/// <summary>Element wise min.</summary>
		public Vector2i Min( Vector2i value )
		{
			return new Vector2i( Common.Min( X, value.X ) ,
								 Common.Min( Y, value.Y ) );
		}

		/// <summary>Element wise clamp.</summary>
		public Vector2i Clamp( Vector2i min, Vector2i max )
		{
			return new Vector2i( Common.Clamp( X, min.X, max.X ) ,
								 Common.Clamp( Y, min.Y, max.Y ) );
		}

		/// <summary>
		/// Element wise index clamp.
		/// X is clamped to [0,n.X-1]
		/// Y is clamped to [0,n.Y-1]
		/// <param name="n">The 2d size "this" components must be clamped against. The components of n are assumed to be positive (values of n.X or n.Y negative or zero will result in undefined behaviour).</param>
		/// </summary>
		public Vector2i ClampIndex( Vector2i n )
		{
			return new Vector2i( Common.ClampIndex( X, n.X ) ,
								 Common.ClampIndex( Y, n.Y ) );
		}

		/// <summary>
		/// Element wise index wrap. 
		/// X wraps around [0,n.X-1]
		/// Y wraps around [0,n.Y-1]
		/// This's (X,Y) is assumed to be a 2d index in a 2d table of size (n.X,n.Y).
		/// If X or Y are not in the valid array range, they are wrapped around [0,n.X-1] and [0,n.Y-1] respectively (-1 becomes n-1, n becomes 0, n+1 becomes 1 etc), else their value is unchanged.
		/// <param name="n">The 2d size "this" components must be wrapped around. The components of n are assumed to be positive (values of n.X or n.Y negative or zero will result in undefined behaviour).</param>
		/// </summary>
		public Vector2i WrapIndex( Vector2i n )
		{
			return new Vector2i( Common.WrapIndex( X, n.X ) ,
								 Common.WrapIndex( Y, n.Y ) );
		}

		/// <summary>Element wise addition.</summary>
		public static Vector2i operator + ( Vector2i a, Vector2i value )
		{
			return new Vector2i( a.X + value.X , 
								 a.Y + value.Y );
		}

		/// <summary>Element wise subtraction.</summary>
		public static Vector2i operator - ( Vector2i a, Vector2i value )
		{
			return new Vector2i( a.X - value.X , 
								 a.Y - value.Y );
		}

		/// <summary>Element wise multiplication.</summary>
		public static Vector2i operator * ( Vector2i a, Vector2i value )
		{
			return new Vector2i( a.X * value.X , 
								 a.Y * value.Y );
		}

		/// <summary>Element wise multiplication.</summary>
		public static Vector2i operator * ( Vector2i a, int value )
		{
			return new Vector2i( a.X * value , 
								 a.Y * value );
		}

		/// <summary>Element wise multiplication.</summary>
		public static Vector2i operator * ( int value, Vector2i a )
		{
			return new Vector2i( a.X * value , 
								 a.Y * value );
		}

		/// <summary>Element wise division.</summary>
		public static Vector2i operator / ( Vector2i a, Vector2i value )
		{
			return new Vector2i( a.X / value.X , 
								 a.Y / value.Y );
		}

		/// <summary>Unary minus operator</summary>
		public static Vector2i operator- ( Vector2i a )
		{
			return new Vector2i( -a.X , 
								 -a.Y );
		}

		/// <summary>Return true if all elements are equal.</summary>
		public static bool operator == ( Vector2i a, Vector2i value )
		{
			return( a.X == value.X ) && ( a.Y == value.Y );
		}

		/// <summary>Return true if at least one element is non equal.</summary>
		public static bool operator != ( Vector2i a, Vector2i value )
		{
			return( a.X != value.X ) || ( a.Y != value.Y );
		}

		/// <summary>Return the product of elements, X * Y</summary>
		public int Product()
		{
			return X * Y;
		}

		/// <summary>Equality test.</summary>
		public bool Equals( Vector2i v ) 
		{
			return( X == v.X ) && ( Y == v.Y );
		}

		/// <summary>Equality test.</summary>
		public override bool Equals( Object o ) 
		{
			return !(o is Vector2i) ? false : Equals((Vector2i)o);
		}

		/// <summary>Return the string representation of this vector.</summary>
		public override string ToString() 
		{
			return string.Format("({0},{1})", X, Y);
		}

		/// <summary>Gets the hash code for this vector.</summary>
		public override int GetHashCode() 
		{
			return(int)(X.GetHashCode() ^ Y.GetHashCode());
		}

//		public static Vector2i operator >> ( Vector2i a, Vector2i value )
//		{
//			return new Vector2i( a.X >> value.X , 
//								 a.Y >> value.Y );
//		}
// 	
//		public static Vector2i operator << ( Vector2i a, Vector2i value )
//		{
//			return new Vector2i( a.X << value.X , 
//								 a.Y << value.Y );
//		}


		public Vector2i Yx { get{ return new Vector2i( Y, X ); } }
	}
}

