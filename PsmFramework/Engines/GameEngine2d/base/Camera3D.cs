/* SCE CONFIDENTIAL
 * PlayStation(R)Suite SDK 0.98.2
 * Copyright (C) 2012 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using Sce.PlayStation.Core;

namespace Sce.PlayStation.HighLevel.GameEngine2D.Base
{
	/// <summary>
	/// Frustum object, used by Camera2D and Camera3D. 
	/// It only deals with perspective.
	/// </summary>
	public class Frustum
	{
		bool m_is_fovy;
		float m_fov;

		/// <summary>Frustum constructor.</summary>
		public Frustum()
		{
			FovY = Math.Deg2Rad( 53.0f );
		}

		/// <summary>The projection as a matrix.</summary>
		public Matrix4 Matrix
		{
			get { return Matrix4.Perspective( FovY, Aspect, Znear, Zfar );}
		}

		/// <summary>Width/Height aspect ratio.</summary>
		public float Aspect = 1.0f;

		/// <summary>
		/// Projection's near z value.
		/// </summary>
		public float Znear = 0.1f;

		/// <summary>
		/// Projection's far z value.
		/// </summary>
		public float Zfar = 1000.0f;

		/// <summary>
		/// Field of view along X axis. 
		/// If you set the field of view with this property, X becomes the main fov direction for this Frustum, 
		/// and FovY value's correctness will depend on Aspect's value. 
		/// </summary>
		public float FovX 
		{
			set 
			{ 
				m_fov = value; 
				m_is_fovy = false;
			} 

			get
			{
				if ( !m_is_fovy )
					return m_fov;

				return 2.0f * FMath.Atan( FMath.Tan( m_fov * 0.5f ) * Aspect );
			}
		}

		/// <summary>
		/// Field of view along Y axis. 
		/// If you set the field of view with this property, Y becomes the main fov direction for this Frustum, 
		/// and FovX value's correctness will depend on Aspect's value. 
		/// </summary>
		public float FovY 
		{ 
			set 
			{ 
				m_fov = value; 
				m_is_fovy = true;
			} 

			get
			{
				if ( m_is_fovy )
					return m_fov;

				return 2.0f * FMath.Atan( FMath.Tan( m_fov * 0.5f ) / Aspect );
			}
		}

		/// <summary>
		/// Given a point in normalized screen coordinates (bottom left (-1,1) and upper right (1,1)), 
		/// and a z value, return the corresponding 3D point in view space.
		/// </summary>
		public Vector4 GetPoint( Vector2 screen_normalized_pos, float z )
		{
			float half_h = z * FMath.Tan( FovY * 0.5f );
			float half_w = Aspect * half_h;

			Vector4 ret = ( screen_normalized_pos * new Vector2( half_w, half_h ) ).Xy01;
			ret.Z = -z;

			return ret;
		}
	}

	/// <summary>The 3D camera here is quite primitive, as the library is mainly 2D.</summary>
	public class Camera3D : ICamera
	{
		GraphicsContextAlpha GL;
		DrawHelpers m_draw_helpers;
		int m_push_depth; // check push/pop errors
		Bounds2 m_last_2d_bounds; // for SetFromCamera2D

		/// <summary>
		/// </summary>
		/// <param name="gl">Needed for its matrix stack.</param>
		/// <param name="draw_helpers">Needed only for debug draw (DebugDraw).</param>
		public Camera3D( GraphicsContextAlpha gl, DrawHelpers draw_helpers )
		{
			GL = gl;
			m_draw_helpers = draw_helpers;

			m_push_depth = 0;

			Frustum = new Frustum();
		}

		/// <summary>Eye positions.</summary>
		public Vector3 Eye;

		/// <summary>View center/target position.</summary>
		public Vector3 Center;

		/// <summary>Up vector.</summary>
		public Vector3 Up;

		/// <summary>The perspective.</summary>
		public Frustum Frustum;

		/// <summary>
		/// This model matrix is used by NormalizedToWorld/GetTouchPos so we can
		/// define a 2d plane to raytrace touch direction against.
		/// </summary>
		public Matrix4 TouchPlaneMatrix = Matrix4.Identity;

		/// <summary>
		/// Position the camera and set the persective so that it matches
		/// exactly the 2D ortho view (when all sprites are drawn
		/// on the Z=0 plane anyway, which is the default.
		/// </summary>
		public void SetFromCamera2D( Camera2D cam2d )
		{
			Vector2 y = cam2d.Y();
			float eye_distance = y.Length() / FMath.Tan( 0.5f * Frustum.FovY );	// distance to view plane
			Eye = cam2d.Center.Xy0 + eye_distance * Math._001;
			Center = cam2d.Center.Xy0;
			Up = y.Xy0.Normalize();

			m_last_2d_bounds = cam2d.CalcBounds();
		}

		/// <summary>
		/// Update the aspect ratio based on current viewport.
		/// </summary>
		public void SetAspectFromViewport()
		{
			Frustum.Aspect = GL.GetViewportf().Aspect;
		}

		/// <summary>
		/// Calculate the camera transform matrix (positioning matrix), as a Matrix4.
		/// GetTransform().InverseOrthonormal() is what you push on the view matrix stack.
		/// Return an orthonormal matrix.
		/// </summary>
		public Matrix4 GetTransform()
		{
			return Math.LookAt( Eye, Center, Up );
		}

		/// <summary>
		/// Push all necessary matrices on the matrix stack.
		/// </summary>
		public void Push()
		{
			++m_push_depth;

			SetAspectFromViewport();

			GL.ProjectionMatrix.Push();
			GL.ProjectionMatrix.Set( Frustum.Matrix );

			GL.ViewMatrix.Push();
			GL.ViewMatrix.Set1( GetTransform().InverseOrthonormal() );

			GL.ModelMatrix.Push();
		}

		/// <summary>
		/// Pop all camera matrices from the matrix stack.
		/// </summary>
		public void Pop()
		{
			Common.Assert( m_push_depth > 0 );
			--m_push_depth;

			GL.ModelMatrix.Pop();

			GL.ViewMatrix.Pop();

			GL.ProjectionMatrix.Pop();
		}

		/// <summary>
		/// Draw a canonical debug grid.
		/// Note that DebugDraw() doesn't call Push()/Pop() internally. It is your responsability to call it between this Camera's Push()/Pop().
		/// </summary>
		public void DebugDraw( float step )
		{
//			m_draw_helpers.DrawDefaultGrid( CalcBounds(), step, Colors.Cyan * 0.5f );
			m_draw_helpers.DrawDefaultGrid( CalcBounds(), step );
		}

		// Struct returned by calc_view_ray.
		public struct Ray 
		{
			public Vector4 Start;
			public Vector4 Direction;
		}

		// Calculate a ray in camera space, given a point in normalized screen coordinates.
		Ray calc_view_ray( Vector2 bottom_left_minus_1_minus_1_top_left_1_1_normalized_screen_pos )
		{
			Matrix4 transform = GetTransform();
			Vector4 p = transform * Frustum.GetPoint( bottom_left_minus_1_minus_1_top_left_1_1_normalized_screen_pos, Frustum.Znear );

			return new Ray() { Start = p, Direction = ( p - transform.ColumnW ).Normalize()}; // perspective
//			return new Ray() { Start = p, Direction = -transform.ColumnZ ) }; // ortho
		}

		/// <summary>
		/// Note that unlike Camera2D.NormalizedToWorld, Camera3D.NormalizedToWorld might not return a
		/// valid position, since it's a ray/plane intersection.
		/// 
		/// The return point is in 2d, in touch plane local coordinates. This function uses the
		/// TouchPlaneMatrix property to know which plane to intersect, so TouchPlaneMatrix must
		/// have been set beforehand (use Node.NormalizedToWorld does it for you).
		/// </summary>
		public Vector2 NormalizedToWorld( Vector2 bottom_left_minus_1_minus_1_top_left_1_1_normalized_screen_pos )
		{
			Ray ray = calc_view_ray( bottom_left_minus_1_minus_1_top_left_1_1_normalized_screen_pos );

			Vector4 plane_base = TouchPlaneMatrix.ColumnW;
			Vector4 plane_normal = TouchPlaneMatrix.ColumnZ;

			float t = - ( ray.Start - plane_base ).Dot( plane_normal ) / ray.Direction.Dot( plane_normal );

			if ( t < 0.0f )
				return GameEngine2D.Base.Math._00;	// no hit

			return( TouchPlaneMatrix.InverseOrthonormal() * ( ray.Start + ray.Direction * t ) ).Xy;
		}

		/// <summary>
		/// Return the 'nth' touch position in world coordinates.
		/// The 'prev' flag is for internal use only.
		/// </summary>
		public Vector2 GetTouchPos( int nth = 0, bool prev = false )
		{
			return NormalizedToWorld( prev 
									 ? Input2.Touch.GetData(0)[nth].PreviousPos 
									 : Input2.Touch.GetData(0)[nth].Pos );
		}

		/// <summary>
		/// Return the most recent bounds set by SetFromCamera2D.
		/// </summary>
		public Bounds2 CalcBounds()
		{
			return m_last_2d_bounds;
		}

		/// <summary>
		/// Creates a 3D view that max the screen bounds in pixel size (in plane z=0)
		/// Exactly match the 3D frustum. Eye distance to z=0 plane is calculated based
		/// on current Frustum.FovY value.
		/// </summary>
		public void SetViewFromViewport()
		{
			Camera2D tmp = new Camera2D( GL, m_draw_helpers );
			tmp.SetViewFromViewport();
			SetFromCamera2D( tmp );
		}

		/// <summary>
		/// Based on current viewport size, get the size of a "screen pixel" in world coordinates.
		/// Can be used to determine scale factor needed to draw sprites or fonts 1:1 for example.
		/// Uses the most recent Bounds2 set by SetFromCamera2D.
		/// </summary>
		public float GetPixelSize()
		{
			Bounds2 bounds = GL.GetViewportf();
			return( m_last_2d_bounds.Size.X * 0.5f ) / ( bounds.Size.X * 0.5f );
		}

		/// <summary>
		/// Debug camera navigation.
		/// </summary>
		public void Navigate( int control )
		{
//			System.Console.WriteLine( Common.FrameCount + " Camera3D.Navigate is not implemented" );
		}

		/// <summary>
		/// Set the model plane matrix used in GetTouchPos and NormalizedToWorld.
		/// </summary>
		public void SetTouchPlaneMatrix( Matrix4 mat ) 
		{
			TouchPlaneMatrix = mat;
		}
	}
}

