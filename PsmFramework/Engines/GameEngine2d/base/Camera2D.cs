/* SCE CONFIDENTIAL
 * PlayStation(R)Suite SDK 0.98.2
 * Copyright (C) 2012 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using Sce.PlayStation.Core;

namespace Sce.PlayStation.HighLevel.GameEngine2D.Base
{
	/// <summary>
	/// 2D view setup and navigation. Uses GraphicsContextAlpha + DrawHelpers
	/// for the matrix stack and rulers/axis drawing.
	/// </summary>
	public class Camera2D : ICamera
	{
		GraphicsContextAlpha GL;
		DrawHelpers m_draw_helpers;
		
		/// Pack some internal bits in a struct so we can copy easily
		internal struct Data
		{
			/// The support vector is the 2D world vector that maps to "from center of screen to middle of right screen edge"
			/// (or "to the middle of the top screen edge" if m_support_is_y is set to true). It is decomposed into
			/// a unit vector component 'm_support_unit_vec' and its len 'm_support_scale'.
			
			internal Vector2 m_support_unit_vec; 
			internal float m_support_scale;
			internal bool m_support_is_y;
			internal Vector2 m_center; // world coordinates of the screen center (view center)
			internal float m_aspect;
			internal float m_znear;
			internal float m_zfar;
		}

		Data m_data;

		int m_push_depth; // check push/pop errors

		// that's for dragging
		bool m_prev_touch_state;
		bool m_touch_state;
		int m_drag_mode; // 0:no drag 1:pan 2:zoom
		Vector2 m_drag_start_pos; // start pos in world space
		Vector2 m_drag_start_pos_ncs; // start pos in normalized screen coordinates
		Data m_data_start;

		/// <summary>
		/// </summary>
		/// <param name="gl">Needed for the matrix stack</param>
		/// <param name="draw_helpers">Needed only for debug draw</param>
		public Camera2D( GraphicsContextAlpha gl, DrawHelpers draw_helpers )
		{
			GL = gl;
			m_draw_helpers = draw_helpers;

			m_data.m_support_scale = 1.0f;
			m_data.m_support_unit_vec = Math._01;
			m_data.m_support_is_y = true;
			m_data.m_center = GameEngine2D.Base.Math._00;
			m_data.m_aspect = 1.0f;
			m_data.m_znear = -1.0f;
			m_data.m_zfar = 1.0f;
			
			m_push_depth = 0;
			
			m_prev_touch_state = false;
			m_touch_state = false;
			m_drag_mode = 0;
			m_drag_start_pos = GameEngine2D.Base.Math._00;
		}
		
		/// <summary>
		/// Define 2D view by specifying a view center and the X support vector that determines scale and rotation.
		/// The X support vector is the vector going from the center of the screen to the middle of the right
		/// edge of the screen, expressed in the viewed world coordinates.
		/// </summary>
		public void SetViewX( Vector2 support, Vector2 center )
		{
			m_data.m_support_scale = support.Length();
			m_data.m_support_unit_vec = support / m_data.m_support_scale;
			m_data.m_center = center;
			m_data.m_support_is_y = false;

			SetAspectFromViewport(); // if the viewport is always something sane, uncomment this
		}

		/// <summary>
		/// Define 2D view by specifying a view center and the Y support vector that determines scale and rotation.
		/// The Y support vector is the vector going from the center of the screen to the middle of the top
		/// edge of the screen, expressed in the viewed world coordinates.
		/// </summary>
		public void SetViewY( Vector2 support, Vector2 center )
		{
			m_data.m_support_scale = support.Length();
			m_data.m_support_unit_vec = support / m_data.m_support_scale;
			m_data.m_center = center;
			m_data.m_support_is_y = true;

			SetAspectFromViewport(); // if the viewport is always something sane, uncomment this
		}

		/// <summary>
		/// Define 2D view by specifying a view center (the world coordinate of the point that is 
		/// a the center of the screen), and the amount of world we can see along the screen width.
		/// 
		/// This is the same as doing SetViewX( new Vector2(width*0.5f,0.0f), center ).
		/// 
		/// Note that this call alone is enough to define your camera. SetViewFromHeightAndCenter 
		/// is not needed (the width is deduced from aspect ratio automatically).
		/// </summary>
		public void SetViewFromWidthAndCenter( float width, Vector2 center )
		{
			SetViewX( new Vector2( width * 0.5f, 0.0f ), center );
		}

		/// <summary>
		/// Define 2D view by specifying a view center (the world coordinate of the point that is 
		/// a the center of the screen), and the amount of world we can see along the screen height.
		/// 
		/// This is the same as doing SetViewY( new Vector2(0.0f,height*0.5f), center ).
		/// 
		/// Note that this call alone is enough to define your camera. SetViewFromWidthAndCenter 
		/// is not needed (the width is deduced from aspect ratio automatically).
		/// </summary>
		public void SetViewFromHeightAndCenter( float height, Vector2 center )
		{
			SetViewY( new Vector2( 0.0f, height * 0.5f ), center );
		}

		/// <summary>
		/// Define a 2D view by specifying the world coordinate of the bottom left of the screen, 
		/// and the amount of world we can see along the screen height.
		/// </summary>
		public void SetViewFromHeightAndBottomLeft( float height, Vector2 bottom_left )
		{
			SetAspectFromViewport();
			float width = height * Aspect;

			SetViewY( new Vector2( 0.0f, height * 0.5f ), bottom_left + new Vector2( width, height ) * 0.5f );
		}

		/// <summary>
		/// Define a 2D view by specifying the world coordinate of the bottom left of the screen, 
		/// and the amount of world we can see along the screen width.
		/// </summary>
		public void SetViewFromWidthAndBottomLeft( float width, Vector2 bottom_left )
		{
			SetAspectFromViewport();
			float height = width / Aspect;

			SetViewX( new Vector2( width * 0.5f, 0.0f ), bottom_left + new Vector2( width, height ) * 0.5f );
		}

		/// <summary>
		/// Define a 2D view that matches the viewport, if you want to work in pixel coordinates.
		/// Bottom left is 0,0, top right is the size of the screen.
		/// </summary>
		public void SetViewFromViewport()
		{
			Bounds2 vp = GL.GetViewportf(); 
			Common.Assert( vp.Size.Y != 0.0f );
			SetViewFromHeightAndCenter( vp.Size.Y, vp.Center );
			m_data.m_aspect = vp.Aspect;
		}

		/// <summary>
		/// Read current viewport to update aspect ratio.
		/// </summary>
		public void SetAspectFromViewport()
		{
			m_data.m_aspect = GL.GetViewportf().Aspect;
		}

		/// <summary>
		/// The world vector that maps to (screen center, middle of right screen edge)
		/// Aspect ratio is taken into account.
		/// </summary>
		public Vector2 X()
		{
			return( ( m_data.m_support_is_y ? -Math.Perp( m_data.m_support_unit_vec ) * m_data.m_aspect : m_data.m_support_unit_vec ) * m_data.m_support_scale );
		}

		/// <summary>
		/// The world vector that maps to (screen center, middle of top screen edge)
		/// Aspect ratio is taken into account. This is also the direction of the
		/// up vector if you were going to use a 3d LookAt.
		/// </summary>
		public Vector2 Y()
		{
			return Math.Perp( X() ) / m_data.m_aspect;
		}

		/// <summary>
		/// The world position that maps to screen center.
		/// </summary>
		public Vector2 Center
		{
			get { return m_data.m_center;}
			set { m_data.m_center = value;}
		}

		/// <summary>
		/// Aspect ratio width/height.
		/// </summary>
		public float Aspect
		{
			get { return m_data.m_aspect;}
			set { m_data.m_aspect = value;}
		}

		/// <summary>
		/// Projection's near z value.
		/// </summary>
		public float Znear
		{
			get { return m_data.m_znear;}
			set { m_data.m_znear = value;}
		}

		/// <summary>
		/// Projection's far z value.
		/// </summary>
		public float Zfar
		{
			get { return m_data.m_zfar;}
			set { m_data.m_zfar = value;}
		}

		/// <summary>
		/// Calculate the bounds (in world coordinates) of the portion of world visible on screen.
		/// This can be used for screen culling (it is used by DrawRulers for instance).
		/// If there is no rotation, the bounds matches the screen exactly.
		/// </summary>
		public Bounds2 CalcBounds()
		{
			Vector2 x = X();
			Vector2 y = Y();
			Vector2 c = Center;

			Bounds2 bounds = new Bounds2( c, c );

			bounds.Add( c - x - y );
			bounds.Add( c + x - y );
			bounds.Add( c + x + y );
			bounds.Add( c - x + y );

			return bounds;
		}

		/// <summary>
		/// Calculate the camera transform matrix (orthonormal positioning matrix), as a Matrix4.
		/// GetTransform().InverseOrthonormal() is what you push on the view matrix stack.
		/// </summary>
		public Matrix4 GetTransform()
		{
			return new Matrix4 ( X().Normalize().Xy00, Y().Normalize().Xy00, Math._0010, Center.Xy01 );
		}

		/// <summary>
		/// Return the NormalizedToWorld matrix, as a Matrix3.
		/// </summary>
		public Matrix3 NormalizedToWorldMatrix() 
		{
			return new Matrix3 ( X().Xy0, Y().Xy0, Center.Xy1 );
		}

		/// <summary>
		/// Given a point in normalized screen coordinates (-1->1), return its corresponding world position.
		/// </summary>
		public Vector2 NormalizedToWorld( Vector2 bottom_left_minus_1_minus_1_top_left_1_1_normalized_screen_pos )
		{
			return( NormalizedToWorldMatrix() * bottom_left_minus_1_minus_1_top_left_1_1_normalized_screen_pos.Xy1 ).Xy;
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

		float zoom_curve( float x, float a, float right_scale )
		{
			float scale = 1.0f;
			if ( x > 0.0f )
				scale = right_scale;
			float s = Math.Sign( x );
			return 1.0f + scale *s * ( 1.0f - FMath.Exp( - a * FMath.Abs( x / scale ) ) );
		}

		/// <summary>
		/// Debug navigation: drag/zoom using touch interface.
		/// </summary>
		public void Navigate( int control )
		{
			m_prev_touch_state = m_touch_state;
			m_touch_state = Input2.Touch00.Down;

			bool press = m_touch_state && !m_prev_touch_state;
			bool down = m_touch_state;

			if ( control == 1 )
			{
				if ( down )
				{
					if ( press )
					{
						m_drag_start_pos = GetTouchPos( 0 );
						m_data_start = m_data;
						m_drag_mode = 1;
					}

					if ( m_drag_mode == 1 )
					{
						Data boo = m_data;
						m_data = m_data_start;
						Vector2 tmp = GetTouchPos( 0, true && !press );
						m_data = boo;

						m_data.m_center = m_data_start.m_center + ( m_drag_start_pos - tmp );
					}
				}
				else m_drag_mode = 0;
			}

			if ( control == 2 )
			{
				if ( down )
				{
					if ( press )
					{
						m_drag_start_pos = GetTouchPos( 0 );
						m_drag_start_pos_ncs = Input2.Touch00.Pos;
						m_data_start = m_data;
						m_drag_mode = 2;
					}

					if ( m_drag_mode == 2 )
					{
						Data boo = m_data;
						m_data = m_data_start;
						Vector2 drag_pos = GetTouchPos( 0, !press );
						Vector2 drag_pos_ncs = ( !press ? Input2.Touch00.PreviousPos : Input2.Touch00.Pos );
						m_data = boo;

						Vector2 drag_offset = ( drag_pos_ncs - m_drag_start_pos_ncs );
						m_data.m_support_scale = m_data_start.m_support_scale * zoom_curve( -drag_offset.Y, 2.0f, 5.0f );

//						Vector2 drag_offset = ( drag_pos - m_drag_start_pos );
//						m_data.m_support_scale = m_data_start.m_support_scale - drag_offset .Y * 3.0f;

						m_data.m_support_scale = FMath.Clamp( m_data.m_support_scale
															 , m_data_start.m_support_scale / 100.0f
															 , m_data_start.m_support_scale * 10.0f );

						m_data.m_support_scale = FMath.Max( m_data.m_support_scale, 0.0001f );
					}
				}
				else m_drag_mode = 0;
			}
		}

		/// <summary>
		/// Push all necessary matrices on the matrix stack.
		/// </summary>
		public void Push()
		{
			Common.Assert( GetTransform().IsOrthonormal( 1.0e-4f ) );

			++m_push_depth;

			float hx = X().Length();
			float hy = Y().Length();

			GL.ProjectionMatrix.Push();
			GL.ProjectionMatrix.Set( Matrix4.Ortho( -hx, +hx, -hy, +hy, m_data.m_znear, m_data.m_zfar ) );

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
		/// Draw default grid, rulers spacing 1, grey grid + black axis,
		/// with coordinate system arrows on top.
		/// Note that DebugDraw() doesn't call Push()/Pop() internally. It is your responsability to call it between this Camera's Push()/Pop().
		/// </summary>
		public void DebugDraw( float step )
		{
/*
			if ( false )
			{
				// draw the Camera2D support vector, for debug

				DrawHelpers.ArrowParams ap = new DrawHelpers.ArrowParams() { Scale = 2.0f };

				// this should be a red arrow going from center of the screen to middle of right edge
				m_draw_helpers.SetColor( Colors.Red );
				m_draw_helpers.DrawArrow( Center, Center + X(), ap );

				// this should be a green arrow going from center of the screen to middle of top edge
				m_draw_helpers.SetColor( Colors.Green );
				m_draw_helpers.DrawArrow( Center, Center + Y(), ap );

				// this should be a white disk at the center of the screen
				m_draw_helpers.SetColor( Colors.White );
				m_draw_helpers.DrawDisk( Center, 0.2f, 32 );
			}
*/
			m_draw_helpers.DrawDefaultGrid( CalcBounds(), step );

			m_draw_helpers.DrawCoordinateSystem2D();
		}

		/// <summary>
		/// Based on current viewport size, get the size of a "screen pixel" in world coordinates.
		/// Can be used to determine scale factor needed to draw sprites or fonts 1:1 for example.
		/// </summary>
		public float GetPixelSize()
		{
			Bounds2 bounds = GL.GetViewportf();
			return X().Length() / ( bounds.Size.X * 0.5f );	// X() maps to half width
		}

		/// <summary>
		/// Interface shared by Camera3D, irrelevant for Camera2D.
		/// </summary>
		public void SetTouchPlaneMatrix( Matrix4 mat )
		{

		}
	}

} // namespace Sce.PlayStation.HighLevel.GameEngine2D.Base

