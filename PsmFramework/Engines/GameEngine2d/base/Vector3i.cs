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
	/// Integer version of Vector3.
	/// </summary>
	public struct Vector3i
	{
		/// <summary>X</summary>
		public int X;
		/// <summary>Y</summary>
		public int Y;
		/// <summary>Z</summary>
		public int Z;

		/// <summary>Vector3i constructor.</summary>
		public Vector3i( int x, int y, int z ) 
		{
			X = x;
			Y = y;
			Z = z;
		}

		/// <summary>Vector3i constructor.</summary>
		public Vector3i( Vector2i xy, int z ) 
		{
			X = xy.X;
			Y = xy.Y;
			Z = z;
		}

		/// <summary>Return this as a Vector3 (cast to float).</summary>
		public Vector3 Vector3()
		{
			return new Vector3( (float)X, (float)Y, (float)Z );
		}

		/// <summary>Element wise max.</summary>
		public Vector3i Max( Vector3i value )
		{
			return new Vector3i( Common.Max( X, value.X ) ,
								 Common.Max( Y, value.Y ) ,
								 Common.Max( Z, value.Z ) );
		}

		/// <summary>Element wise min.</summary>
		public Vector3i Min( Vector3i value )
		{
			return new Vector3i( Common.Min( X, value.X ) ,
								 Common.Min( Y, value.Y ),
								 Common.Min( Z, value.Z ) );
		}

		/// <summary>Element wise clamp.</summary>
		public Vector3i Clamp( Vector3i min, Vector3i max )
		{
			return new Vector3i( Common.Clamp( X, min.X, max.X ) ,
								 Common.Clamp( Y, min.Y, max.Y ),
								 Common.Clamp( Z, min.Z, max.Z ) );
		}

		/// <summary>
		/// Element wise index clamp.
		/// X is clamped to [0,n.X-1]
		/// Y is clamped to [0,n.Y-1]
		/// Z is clamped to [0,n.Z-1]
		/// <param name="n">The 3d size "this" components must be clamped against. The components of n are assumed to be positive (values of n.X, n.Y or n.Z negative or zero will result in undefined behaviour).</param>
		/// </summary>
		public Vector3i ClampIndex( Vector3i n )
		{
			return new Vector3i( Common.ClampIndex( X, n.X ) ,
								 Common.ClampIndex( Y, n.Y ),
								 Common.ClampIndex( Z, n.Z ) );
		}

		/// <summary>
		/// Element wise index wrap. 
		/// X wraps around [0,n.X-1]
		/// Y wraps around [0,n.Y-1]
		/// Z wraps around [0,n.Z-1]
		/// This's (X,Y,Z) is assumed to be a 3d index in a 3d table of size (n.X,n.Y.n.Z).
		/// If X, Y or Z are not in the valid array range, they are wrapped around [0,n.X-1], [0,n.Y-1], [0,n.Z-1] respectively (-1 becomes n-1, n becomes 0, n+1 becomes 1 etc), else their value is unchanged.
		/// <param name="n">The 2d size "this" components must be wrapped around. The components of n are assumed to be positive (values of n.X, n.Y or n.Z negative or zero will result in undefined behaviour).</param>
		/// </summary>
		public Vector3i WrapIndex( Vector3i n )
		{
			return new Vector3i( Common.WrapIndex( X, n.X ) ,
								 Common.WrapIndex( Y, n.Y ) ,
								 Common.WrapIndex( Z, n.Z ) );
		}

		/// <summary>Element wise addition.</summary>
		public static Vector3i operator + ( Vector3i a, Vector3i value )
		{
			return new Vector3i( a.X + value.X , 
								 a.Y + value.Y, 
								 a.Z + value.Z );
		}

		/// <summary>Element wise subtraction.</summary>
		public static Vector3i operator - ( Vector3i a, Vector3i value )
		{
			return new Vector3i( a.X - value.X , 
								 a.Y - value.Y, 
								 a.Z - value.Z );
		}

		/// <summary>Element wise multiplication.</summary>
		public static Vector3i operator * ( Vector3i a, Vector3i value )
		{
			return new Vector3i( a.X * value.X , 
								 a.Y * value.Y , 
								 a.Z * value.Z );
		}

		/// <summary>Element wise division.</summary>
		public static Vector3i operator / ( Vector3i a, Vector3i value )
		{
			return new Vector3i( a.X / value.X , 
								 a.Y / value.Y, 
								 a.Z / value.Z );
		}

		/// <summary>Return true if all elements are equal.</summary>
		public static bool operator == ( Vector3i a, Vector3i value )
		{
			return( a.X == value.X ) && ( a.Y == value.Y ) && ( a.Z == value.Z );
		}

		/// <summary>Return true if at least one element is non equal.</summary>
		public static bool operator != ( Vector3i a, Vector3i value )
		{
			return( a.X != value.X ) || ( a.Y != value.Y ) || ( a.Z != value.Z );
		}

		/// <summary>Return the product of elements, X * Y * Z</summary>
		public int Product()
		{
			return X * Y * Z;
		}

		/// <summary>Equality test.</summary>
		public bool Equals( Vector3i v ) 
		{
			return( X == v.X ) && ( Y == v.Y ) && ( Z == v.Z );
		}

		/// <summary>Equality test.</summary>
		public override bool Equals( Object o ) 
		{
			return !(o is Vector3i) ? false : Equals((Vector3i)o);
		}

		/// <summary>Return the string representation of this vector.</summary>
		public override string ToString() 
		{
			return string.Format("({0},{1},{2})", X, Y,Z);
		}

		/// <summary>Gets the hash code for this vector.</summary>
		public override int GetHashCode() 
		{
			return(int)(X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode());
		}

//		public static Vector3i operator >> ( Vector3i a, Vector3i value )
//		{
//			return new Vector3i( a.X >> value.X , 
//								 a.Y >> value.Y , 
//								 a.Z >> value.Z );
//		}
// 	
//		public static Vector3i operator << ( Vector3i a, Vector3i value )
//		{
//			return new Vector3i( a.X << value.X , 
//								 a.Y << value.Y , 
//								 a.Z << value.Z );
//		}


		public Vector2i Xx { get { return new Vector2i( X, X ); } }

		public Vector2i Xy { get { return new Vector2i( X, Y ); } }

		public Vector2i Xz { get { return new Vector2i( X, Z ); } }

		public Vector2i Yx { get { return new Vector2i( Y, X ); } }

		public Vector2i Yy { get { return new Vector2i( Y, Y ); } }

		public Vector2i Yz { get { return new Vector2i( Y, Z ); } }

		public Vector2i Zx { get { return new Vector2i( Z, X ); } }

		public Vector2i Zy { get { return new Vector2i( Z, Y ); } }

		public Vector2i Zz { get { return new Vector2i( Z, Z ); } }


		public Vector3i Xxx { get { return new Vector3i( X, X, X ); } }

		public Vector3i Xxy { get { return new Vector3i( X, X, Y ); } }

		public Vector3i Xxz { get { return new Vector3i( X, X, Z ); } }

		public Vector3i Xyx { get { return new Vector3i( X, Y, X ); } }

		public Vector3i Xyy { get { return new Vector3i( X, Y, Y ); } }

		public Vector3i Xyz { get { return new Vector3i( X, Y, Z ); } }

		public Vector3i Xzx { get { return new Vector3i( X, Z, X ); } }

		public Vector3i Xzy { get { return new Vector3i( X, Z, Y ); } }

		public Vector3i Xzz { get { return new Vector3i( X, Z, Z ); } }

		public Vector3i Yxx { get { return new Vector3i( Y, X, X ); } }

		public Vector3i Yxy { get { return new Vector3i( Y, X, Y ); } }

		public Vector3i Yxz { get { return new Vector3i( Y, X, Z ); } }

		public Vector3i Yyx { get { return new Vector3i( Y, Y, X ); } }

		public Vector3i Yyy { get { return new Vector3i( Y, Y, Y ); } }

		public Vector3i Yyz { get { return new Vector3i( Y, Y, Z ); } }

		public Vector3i Yzx { get { return new Vector3i( Y, Z, X ); } }

		public Vector3i Yzy { get { return new Vector3i( Y, Z, Y ); } }

		public Vector3i Yzz { get { return new Vector3i( Y, Z, Z ); } }

		public Vector3i Zxx { get { return new Vector3i( Z, X, X ); } }

		public Vector3i Zxy { get { return new Vector3i( Z, X, Y ); } }

		public Vector3i Zxz { get { return new Vector3i( Z, X, Z ); } }

		public Vector3i Zyx { get { return new Vector3i( Z, Y, X ); } }

		public Vector3i Zyy { get { return new Vector3i( Z, Y, Y ); } }

		public Vector3i Zyz { get { return new Vector3i( Z, Y, Z ); } }

		public Vector3i Zzx { get { return new Vector3i( Z, Z, X ); } }

		public Vector3i Zzy { get { return new Vector3i( Z, Z, Y ); } }

		public Vector3i Zzz { get { return new Vector3i( Z, Z, Z ); } }
	}
}

