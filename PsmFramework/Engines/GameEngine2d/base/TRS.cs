/* SCE CONFIDENTIAL
 * PlayStation(R)Suite SDK 0.98.2
 * Copyright (C) 2012 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using Sce.PlayStation.Core;

namespace Sce.PlayStation.HighLevel.GameEngine2D.Base
{
	/// <summary>
	/// TRS comes from scenegraph terminology and is used to store a
	/// Translate/Rotate/Scale 2d transform in a canonical way. It also
	/// defines an oriented bounding box. We use it for storing both 
	/// sprite positionning/size and sprite UV.
	/// </summary>
	public struct TRS
	{
		/// <summary>Translation.</summary>
		public Vector2 T;
		/// <summary>Rotation - stored as a unit vector (cos,sin).</summary>
		public Vector2 R;
		/// <summary>Scale (or Size).</summary>
		public Vector2 S;

		/// <summary>The support X vector, which goes from bottom left to bottom right.</summary>
		public Vector2 X { get { return R * S.X;}}
		/// <summary>The support Y vector, which goes from bottom left to top left.</summary>
		public Vector2 Y { get { return Math.Perp( R ) * S.Y;}}

		// Note about PointXX: first column is x, second is y (0 means min, 1 means max, you can also see those as 'uv')

		/// <summary>The bottom left point (the base point), (0,0) in 'local' coordinates.</summary>
		public Vector2 Point00 { get { return T;}}
		/// <summary>The bottom right point, (1,0) in 'local' coordinates.</summary>
		public Vector2 Point10 { get { return T + X;}}
		/// <summary>The top left point, (0,1) in 'local' coordinates.</summary>
		public Vector2 Point01 { get { return T + Y;}}
		/// <summary>The top right point, (1,1) in 'local' coordinates.</summary>
		public Vector2 Point11 { get { return T + X + Y;}}

		/// <summary>Return the center of the oriented box defined by this TRS.</summary>
		public Vector2 Center { get { return T + ( X + Y ) * 0.5f;}}

		/// <summary>
		/// RotationNormalize is like Rotation, but it normalizes on set,
		/// to prevent the unit vector from drifting because of accumulated numerical imprecision.
		/// </summary>
		public Vector2 RotationNormalize { get { return R ;} set { R = value.Normalize(); }}

		/// <summary>Rotate the object by an angle.</summary>
		/// <param name="angle">Rotation angle in radian.</param>
		public void Rotate( float angle ) { R = R.Rotate( angle );}

		/// <summary>Rotate the object by an angle.</summary>
		/// <param name="rotation">The (cos(angle),sin(angle)) unit vector representing the rotation.</param>
		/// This lets you precompute the cos,sin needed during rotation.
		public void Rotate( Vector2 rotation ) { R = R.Rotate( rotation );}

		/// <summary>
		/// This property lets you set/get rotation as a angle. This is expensive and brings the usual
		/// angle discontinuity problems. The angle is always stored and returned in the the range -pi,pi.
		/// </summary>
		public float Angle { get { return Math.Angle( R );} set { R = Vector2.Rotation( value );}}

		/// <summary>A TRS that covers the unit quad that goes from (0,0) to (1,1).</summary>
		public static TRS Quad0_1 = new TRS() 
		{ 
			T = Math._00, 
			R = Math._10, 
			S = Math._11
		};

		/// <summary>A TRS that covers the quad that goes from (-1,-1) to (1,1).</summary>
		public static TRS QuadMinus1_1 = new TRS() 
		{ 
			T = -Math._11, 
			R = Math._10, 
			S = Math._11*2
		};

		/// <summary>Convert from Bounds2: a_bounds.Min becomes T and a_bounds.Size becomes S (no rotation).</summary>
		public TRS( Bounds2 a_bounds )
		{
			T = a_bounds.Min;
			R = Math._10;
			S = a_bounds.Size;
		}

		/// <summary>
		/// Convert to Bounds2. Note that end points won't match if there is a Rotation,
		/// but in all cases the returned bounds fully contains the TRS.
		/// </summary>
		public Bounds2 Bounds2()
		{
			Bounds2 ret = new Bounds2( Point00, Point00 );

			ret.Add( Point10 );
			ret.Add( Point01 );
			ret.Add( Point11 );

			return ret;
		}

		//use new TRS { T=a_T, R=a_S, S=a_S } instead?
//		public TRS( Vector2 a_T ,
//					Vector2 a_R ,
//					Vector2 a_S )
//		{
//			T = a_T;
//			R = a_R;
//			S = a_S;
//		}

		/// <summary>
		/// Get a subregion from source_area, given a number of tiles and a tile index,
		/// assuming evenly spaced subdivision. Typically source_area will be Quad0_1
		/// (the unit quad, means the whole texture) and we return the uv info for a 
		/// given tile in the tiled texture.
		/// </summary>
		static public TRS Tile( Vector2i num_tiles, Vector2i tile_index, TRS source_area )
		{
			Vector2 num_tiles_f = num_tiles.Vector2();
			Vector2 tile_index_f = tile_index.Vector2();
			
			Vector2 tile_size = source_area.S / num_tiles_f;
			
			Vector2 X = source_area.X;
			Vector2 Y = source_area.Y;
			
			TRS ret = new TRS();
			
			ret.T = source_area.T + tile_index_f * tile_size;
			ret.R = source_area.R;
			ret.S = tile_size;
			
			return ret;
		}

//		/// <summary>
//		/// Make the 'bottom left' base point T be "upper left".
//		/// This requires to flip the sign of S.Y.
//		/// </summary>
//
//		public TRS OutrageousYTopBottomSwap()
//		{
//			TRS ret = this;
// 	
//			ret.T += Y; // move base point to upper left
//			ret.S.Y = -S.Y;
// 	
//			return ret;
//		}

		/// <summary>Some aliases for commonly used points that can be passed to Centering.</summary>
		static public class Local 
		{
			/// <summary></summary>
			static public Vector2 TopLeft = new Vector2(0.0f,1.0f);
			/// <summary></summary>
			static public Vector2 MiddleLeft = new Vector2(0.0f,0.5f);
			/// <summary></summary>
			static public Vector2 BottomLeft = new Vector2(0.0f,0.0f);
			/// <summary></summary>
			static public Vector2 TopCenter = new Vector2(0.5f,1.0f);
			/// <summary></summary>
			static public Vector2 Center = new Vector2(0.5f,0.5f);
			/// <summary></summary>
			static public Vector2 BottomCenter = new Vector2(0.5f,0.0f);
			/// <summary></summary>
			static public Vector2 TopRight = new Vector2(1.0f,1.0f);
			/// <summary></summary>
			static public Vector2 MiddleRight = new Vector2(1.0f,0.5f);
			/// <summary></summary>
			static public Vector2 BottomRight = new Vector2(1.0f,0.0f);
		}

		/// <summary>
		/// Translate the TRS so that the normalized point given in input becomes (0,0). 
		/// There are a few predefined normalized points in TRS.Local.
		/// </summary>
		/// <param name="normalized_pos">The normalized position that will become the new center. 
		/// For example (0.5,0.5) represents the center of the TRS, regardless of the actual size, 
		/// position and orientation of the TRS. (0,0) is the bottom left point, (1,1) is the top
		/// right point etc.</param>
		public void Centering( Vector2 normalized_pos )
		{
			T = - X * normalized_pos.X - Y * normalized_pos.Y;
		}

		/// <summary></summary>
		public override string ToString() 
		{
			if ( R.X == 0.0f && R.Y == 0.0f )
				return string.Format("Invalid TRS (R length is zero)");

			return string.Format("(T={0},R={1}={2} degrees,S={3})", T, R, Math.Rad2Deg(Angle), S);
			//return string.Format("(T={0},R={1},S={2})", T, R, S);
		}
	}
}

