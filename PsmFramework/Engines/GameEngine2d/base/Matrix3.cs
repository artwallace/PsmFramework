/* SCE CONFIDENTIAL
 * PlayStation(R)Suite SDK 0.98.2
 * Copyright (C) 2012 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using Sce.PlayStation.Core;

namespace Sce.PlayStation.HighLevel.GameEngine2D.Base
{
	/// <summary>
	/// A 3x3 matrix class for 2D operations (similar to Matrix4).
	/// </summary>
	public struct Matrix3
	{
		/// <summary>First column.</summary>
		public Vector3 X;
		/// <summary>Second column.</summary>
		public Vector3 Y;
		/// <summary>Third column.</summary>
		public Vector3 Z;

		/// <summary>
		/// </summary>
		/// <param name="valx">First column.</param>
		/// <param name="valy">Second column.</param>
		/// <param name="valz">Third column.</param>
		public Matrix3( Vector3 valx, Vector3 valy, Vector3 valz )
		{
			X = valx;
			Y = valy;
			Z = valz;
		}

		/// <summary>Matrix/vector multiplication.</summary>
		public static Vector3 operator * ( Matrix3 m, Vector3 v )
		{
			return 
			v.X * m.X + 
			v.Y * m.Y + 
			v.Z * m.Z ;
		}

		/// <summary>Matrix/matrix multiplication.</summary>
		public static Matrix3 operator * ( Matrix3 m1, Matrix3 m2 )
		{
			return new Matrix3( m1 * m2.X ,
								m1 * m2.Y ,
								m1 * m2.Z );
		}

//		/// <summary></summary>
//		public static Matrix3 operator + ( Matrix3 m1, Matrix3 m2 )
//		{
//			return new Matrix3( m1.X + m2.X ,
//								m1.Y + m2.Y ,
//								m1.Z + m2.Z );
//		}
//
//		/// <summary></summary>
//		public static Matrix3 operator - ( Matrix3 m1, Matrix3 m2 )
//		{
//			return new Matrix3( m1.X - m2.X ,
//								m1.Y - m2.Y ,
//								m1.Z - m2.Z );
//		}

		/// <summary>Return the transpose.</summary>
		public Matrix3 Transpose()
		{
			return new Matrix3( new Vector3( X.X, Y.X, Z.X ) ,
								new Vector3( X.Y, Y.Y, Z.Y ) ,
								new Vector3( X.Z, Y.Z, Z.Z ) );
		}

		/// <summary>Return the identity.</summary>
		public static Matrix3 Identity = new Matrix3( Math._100 ,
													 Math._010 ,
													 Math._001 );

		/// <summary>Return the matrix with all elements set to zero.</summary>
		public static Matrix3 Zero = new Matrix3( Math._000 , 
												 Math._000 , 
												 Math._000 );

		/// <summary>Return a translation matrix.</summary>
		public static Matrix3 Translation( Vector2 value )
		{
			return new Matrix3( Math._100 ,
								Math._010 ,
								value.Xy1 );
		}

		/// <summary>Return a scale matrix.</summary>
		public static Matrix3 Scale( Vector2 value )
		{
			return new Matrix3( Math._100 * value.X ,
								Math._010 * value.Y ,
								Math._001 );
		}

		/// <summary>
		/// Return a skew matrix.
		/// </summary>
		/// <param name="value">The (tan(skewx),tan(skewy)) vector, where skewx and skewy are the skew angles.</param>
		public static Matrix3 Skew( Vector2 value )
		{
			return new Matrix3( new Vector3( 1.0f, value.X, 0.0f ) ,
								new Vector3( value.Y, 1.0f, 0.0f ) ,
								Math._001 );
		}

		/// <summary>
		/// Return a rotation matrix.
		/// </summary>
		/// <param name="unit_vector">A (cos(angle),sin(angle)) unit vector, where angle is the amount you want to rotate by.</param>
		public static Matrix3 Rotation( Vector2 unit_vector )
		{
			return new Matrix3( unit_vector.Xy0, 
								Math.Perp( unit_vector ).Xy0, 
								Math._001 ); 
		}

		/// <summary>
		/// Return a rotation matrix.
		/// </summary>
		/// <param name="angle">Rotation angle in radians.</param>
		public static Matrix3 Rotation( float angle )
		{
			return Rotation( Vector2.Rotation( angle ) );
		}

		/// <summary>
		/// Return a Translation * Rotation * Scale transform node.
		/// </summary>
		/// <param name="translation">The translation amount.</param>
		/// <param name="unit_vector">A (cos(angle),sin(angle)) unit vector, where angle is the amount you want to rotate by.</param>
		/// <param name="scale">The scale amount.</param>
		public static Matrix3 TRS( Vector2 translation, Vector2 unit_vector, Vector2 scale )
		{
			Matrix3 ret = Rotation( unit_vector );
			ret.Z = translation.Xy1;
			ret.X *= scale.X;
			ret.Y *= scale.Y;
			return ret;
		}

		/// <summary>Return the determinant.</summary>
		public float Determinant()
		{
//			#if 1
			return X.Dot( Y.Cross( Z ) );
//			#else
//			return 
//			X.X * Y.Y * Z.Z+
//			Y.X * Z.Y * X.Z+
//			Z.X * X.Y * Y.Z-
// 	
//			Z.X * Y.Y * X.Z-
//			Y.X * X.Y * Z.Z-
//			X.X * Z.Y * Y.Z;
//			#endif
		}

		/// <summary>Return the general inverse.</summary>
		public Matrix3 Inverse()
		{
			//if |M|!=0, Inverse(M)=adjoint(M)/|M|, where |M| is the Determinant

			float det_rcp = Determinant();

			if ( det_rcp == 0.0f )
				return Identity;

			det_rcp = 1.0f / det_rcp;

			return new Matrix3( new Vector3( ( Y.Y * Z.Z - Z.Y * Y.Z ) * det_rcp ,
											 ( -( X.Y * Z.Z - Z.Y * X.Z ) ) * det_rcp ,
											 ( X.Y * Y.Z - Y.Y * X.Z ) * det_rcp ),
								new Vector3( ( -( Y.X * Z.Z - Z.X * Y.Z ) ) * det_rcp ,
											 ( X.X * Z.Z - Z.X * X.Z ) * det_rcp ,
											 ( -( X.X * Y.Z - Y.X * X.Z ) ) * det_rcp ),
								new Vector3( ( Y.X * Z.Y - Z.X * Y.Y ) * det_rcp ,
											 ( -( X.X * Z.Y - Z.X * X.Y ) ) * det_rcp ,
											 ( X.X * Y.Y - Y.X * X.Y ) * det_rcp ) );

//			halt_assert( (retval * (*this)).Equals( Identity ) );
		}

		/// <summary>Special inverse that assumes the matrix is a TxR (orthonormal).</summary>
		public Matrix3 InverseOrthonormal()
		{
			// inv(T(t)xR)=inv(R)xinv(T(t))=trans(R)xT(-t)

			return new Matrix3( new Vector3( X.X, Y.X, 0.0f ),
								new Vector3( X.Y, Y.Y, 0.0f ), Math._001 ) * Translation( -Z.Xy );

//			halt_assert( (retval * (*this)).Equals( Identity ) );
		}

		/// <summary>
		/// Assuming this matrix is a transform matrix (last row=0,0,1 etc), return a Matrix4 version of it.
		/// </summary>
		public Matrix4 Matrix4()
		{
			return new Matrix4( X.Xyz0 , 
								Y.Xyz0 , 
								Math._0010 , 
								new Vector4( Z.Xy, 0.0f, 1.0f ) );
		}

		static uint XNotUnitLen=2;
		static uint YNotUnitLen=4;
		static uint XYNotPerpendicular=8;
		static uint LastRowNot001=16;
		static uint coord_sys_error = 0;

		/// <summary>
		/// Get the last error returned by isCoordSys() as a string.
		/// </summary>
		public string GetCoordSysError()
		{
			string retval = "";

			if ( 0 != ( coord_sys_error & XNotUnitLen ) ) retval+="XNotUnitLen";
			if ( 0 != ( coord_sys_error & YNotUnitLen ) ) retval+="YNotUnitLen";
			if ( 0 != ( coord_sys_error & XYNotPerpendicular ) ) retval+="XYNotPerpendicular";
			if ( 0 != ( coord_sys_error & LastRowNot001 ) ) retval+="LastRowNot001";

			return retval;
		}

		/// <summary>
		/// Return true if the matrix represents a valid right-handed, orthogonal 2d coordinate system.
		/// </summary>
		public bool IsOrthonormal( float epsilon )
		{
			coord_sys_error=0;

			if ( ! ( Z.Z == 1.0f && X.Z == 0.0f && Y.Z == 0.0f ) )
			{
				coord_sys_error |= LastRowNot001;
				return false;
			}

			if ( ! ( FMath.Abs( X.Length() - 1.0f ) < epsilon ) )
			{
				coord_sys_error |= XNotUnitLen;
				return false;
			}

			if ( ! ( FMath.Abs( Y.Length() - 1.0f ) < epsilon ) )
			{
				coord_sys_error |= YNotUnitLen;
				return false;
			}

			if ( ! ( FMath.Abs( X.Dot( Y ) ) < epsilon ) )
			{
				coord_sys_error |= XYNotPerpendicular;
				return false;
			}

			return true;
		}

		/// <summary>Equality test.</summary>
		public bool Equals ( ref Matrix3 m, float epsilon )
		{
			if ( ! X.Equals( m.X, epsilon ) ) return false;
			if ( ! Y.Equals( m.Y, epsilon ) ) return false;
			if ( ! Z.Equals( m.Z, epsilon ) ) return false;

			return true;
		}

		/// <summary>Equality test.</summary>
		public bool Equals ( ref Matrix3 m )
		{
			return Equals( ref m, 1.0e-6f );
		}

		/// <summary></summary>
		public override string ToString()
		{
			return string.Format("({0},{1},{2})", X, Y, Z);
		}
	}
}

