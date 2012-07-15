/* SCE CONFIDENTIAL
 * PlayStation(R)Suite SDK 0.98.2
 * Copyright (C) 2012 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using Sce.PlayStation.Core;

namespace Sce.PlayStation.HighLevel.GameEngine2D.Base
{
	/// <summary>
	/// Pitch/roll rotations helper object/functions.
	/// Pitch(x) -> roll(y) rotation order
	/// roll in -pi, pi
	/// pitch in -pi/2,pi/2
	/// </summary>
	public class PitchRoll
	{
		/// <summary>
		/// Return pitch in x, roll in y.
		/// </summary>
		public static
		Vector2 FromVector( Vector3 v )
		{
			float roll = Math.Angle( v.Zx );
			v = v.RotateY( -roll );
			Common.Assert( FMath.Abs( v.X ) < 1.0e-3f );
			float pitch = - Math.Angle( v.Zy );
			return new Vector2( pitch, roll );
		}

		/// <summary>
		/// Return z=(0,0,1) rotated by roll->pitch.
		/// </summary>
		public static
		Vector3 ToVector( Vector2 a )
		{
			return Math._001.RotateX( a.X ).RotateY( a.Y );
		}

		/// <summary>x: pitch y: roll (radians)</summary>
		public Vector2 Data;
		/// <summary></summary>
		public PitchRoll() { Data = GameEngine2D.Base.Math._00;}
		/// <summary></summary>
		public PitchRoll( Vector2 v ) { Data = v;}
		/// <summary></summary>
		public PitchRoll( Vector3 v ) { Data = FromVector( v );}
		/// <summary></summary>
		public Vector3 ToVector() { return ToVector( Data );}
		/// <summary></summary>
		public Matrix4 ToMatrix()
		{
			Vector3 z = ToVector();
			Vector2 tmp = Data;
			tmp.X -= Math.Pi * 0.5f;
			Vector3 y = new PitchRoll( tmp ).ToVector();
			Vector3 x = y.Cross(z).Normalize();
			y = z.Cross(x);

			return new Matrix4()
			{
				ColumnX = x.Xyz0 ,
				ColumnY = y.Xyz0 ,
				ColumnZ = z.Xyz0 ,
				ColumnW = Math._0001

			}.InverseOrthonormal();
		}
	}

	/// <summary>
	/// Pitch/roll rotations helper object/functions.
	/// Roll(y) -> pitch(x) rotation order
	/// pitch in -pi, pi
	/// roll in -pi/2,pi/2
	/// </summary>
	public class RollPitch
	{
		/// <summary>
		/// Return pitch in x, roll in y.
		/// </summary>
		public static
		Vector2 FromVector( Vector3 v )
		{
			float pitch = - Math.Angle( v.Zy );
			v = v.RotateX( -pitch );
			Common.Assert( FMath.Abs( v.Y ) < 1.0e-3f );
			float roll = Math.Angle( v.Zx );
			return new Vector2( pitch, roll );
		}

		/// <summary>
		/// Return z=(0,0,1) rotated by roll->pitch.
		/// </summary>
		public static
		Vector3 ToVector( Vector2 a )
		{
			return Math._001.RotateY( a.Y ).RotateX( a.X );
		}

		/// <summary>x: pitch y: roll (radians)</summary>
		public Vector2 Data;
		/// <summary></summary>
		public RollPitch() { Data = GameEngine2D.Base.Math._00;}
		/// <summary></summary>
		public RollPitch( Vector2 v ) { Data = v;}
		/// <summary></summary>
		public RollPitch( Vector3 v ) { Data = FromVector( v );}
		/// <summary></summary>
		public Vector3 ToVector() { return ToVector( Data );}
		/// <summary></summary>
		public Matrix4 ToMatrix()
		{
			Vector3 z = ToVector();
			Vector2 tmp = Data;
			tmp.X -= Math.Pi * 0.5f;
			Vector3 y = new RollPitch( tmp ).ToVector();
			Vector3 x = y.Cross(z).Normalize();
			y = z.Cross(x);

			return new Matrix4()
			{
				ColumnX = x.Xyz0 ,
				ColumnY = y.Xyz0 ,
				ColumnZ = z.Xyz0 ,
				ColumnW = Math._0001

			}.InverseOrthonormal();
		}
	}
}

