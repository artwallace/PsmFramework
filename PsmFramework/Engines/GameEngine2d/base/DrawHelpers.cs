/* SCE CONFIDENTIAL
 * PlayStation(R)Suite SDK 0.98.2
 * Copyright (C) 2012 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;

namespace Sce.PlayStation.HighLevel.GameEngine2D.Base
{
	/// <summary>
	/// Some basic drawing functionalities (2D/3D)
	/// This class is mostly an ImmediateMode object coupled with a debug shader. 
	/// You shouldn't use DrawHelpers for anything else than visual debugging, 
	/// as by nature it is not performance friendly at all.
	/// </summary>
	public class DrawHelpers : System.IDisposable
	{
		/// <summary>
		/// The vertex type used by DrawHelpers (V4F_C4F)
		/// </summary>
		public struct Vertex
		{
			/// <summary>The vertex position. 2D positions should have (z,w) set to (0,1).</summary>
			public Vector4 Position;
			/// <summary>Color, each element in 0,1 range (but values don't get clamped).</summary>
			public Vector4 Color;

			/// <summary></summary>
			public Vertex( Vector4 pos, Vector4 col )
			{
				Position = pos;
				Color = col;
			}

			/// <summary>
			/// </summary>
			/// <param name="pos">The position is expended to 3d by setting (z,w) to (0,1).</param>
			/// <param name="col">The color.</param>
			public Vertex( Vector2 pos, Vector4 col )
			{
				Position = pos.Xy01;
				Color = col;
			}
		}

		GraphicsContextAlpha GL;
		ImmediateMode< Vertex > m_imm;
		ShaderProgram m_shader_program; // a simple shader for debug primitives
		Vector4 m_current_color; // the last color set with .SetColor
		uint m_shader_depth; // allow nested shader push/pop 
		uint m_view_matrix_tag;	// check for ViewMatrix update
		uint m_model_matrix_tag; // check for ModelMatrix update
		uint m_projection_matrix_tag; // check for ProjectionMatrix update

		bool m_disposed = false;
		/// <summary>Return true if this object been disposed.</summary>
		public bool Disposed { get { return m_disposed; } }

		/// <summary>
		/// </summary>
		/// <param name="gl">The core graphics context.</param>
		/// <param name="max_vertices">The maximum number of vertices you will be able to
		/// write in one frame with this DrawHelpers object.</param>
		public DrawHelpers( GraphicsContextAlpha gl, uint max_vertices )
		{
			GL = gl;

			{
				m_shader_program = Common.CreateShaderProgram("default.cgx");

				m_shader_program.SetUniformBinding( 0, "MVP" ) ;
				m_shader_program.SetAttributeBinding( 0, "p" ) ;
				m_shader_program.SetAttributeBinding( 1, "vin_color" ) ;
			}

			m_current_color = Colors.Magenta;
			m_shader_depth = 0;
			m_view_matrix_tag = unchecked((uint)-1);
			m_model_matrix_tag = unchecked((uint)-1);
			m_projection_matrix_tag = unchecked((uint)-1);

			m_imm = new ImmediateMode<Vertex>( gl, max_vertices, null, 0, 0, VertexFormat.Float4, VertexFormat.Float4 );
		}

		/// <summary>
		/// Dispose implementation.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if(disposing)
			{
				m_imm.Dispose();

				Common.DisposeAndNullify< ShaderProgram >( ref m_shader_program );
				m_disposed = true;
			}
		}

		/// <summary>
		/// Start a new immediate primitive. 
		/// </summary>
		/// <param name="mode">The draw primive type.</param>
		/// <param name="max_vertices_intended">You must specify the maximum number of 
		/// vertices you intend to write with ImmVertex(): the number of ImmVertex() calls 
		/// following this function must be inferior or equal to 'max_vertices_intended'.
		/// </param>
		public void ImmBegin( DrawMode mode, uint max_vertices_intended )
		{
			m_imm.ImmBegin( mode, max_vertices_intended );
		}

		/// <summary>
		/// Add a vertex to current primitive.
		/// </summary>
		public void ImmVertex( Vertex v )
		{
			m_imm.ImmVertex( v );
		}

		/// <summary>
		/// Add a vertex to current primitive, using the most recent color set by SetColor().
		/// </summary>
		public void ImmVertex( Vector4 pos )
		{
			m_imm.ImmVertex( new Vertex( pos, m_current_color ) );
		}

		/// <summary>
		/// Add a vertex to current primitive, using the most recent color set by SetColor().
		/// (z,w) is set to (0,1).
		/// </summary>
		public void ImmVertex( Vector2 pos )
		{
			m_imm.ImmVertex( new Vertex( pos.Xy01, m_current_color ) );
		}

		/// <summary>
		/// Finish current primitive (this function triggers the actual draw call).
		/// </summary>
		public void ImmEnd()
		{
			m_imm.ImmEnd();
		}

		/// <summary>
		/// ShaderPush() reads current MVP matrix and sets the current shader.
		/// For DrawHelpers we allow nesting (shader parameters get updated
		/// internally accordingly).
		/// </summary>
		public void ShaderPush()
		{
			// check if MVP needs update... not sure how useful that actually is
			if ( m_view_matrix_tag != GL.ViewMatrix.Tag
				 || m_model_matrix_tag != GL.ModelMatrix.Tag 
				 || m_projection_matrix_tag != GL.ProjectionMatrix.Tag )
			{
				m_model_matrix_tag = GL.ModelMatrix.Tag;
				m_view_matrix_tag = GL.ViewMatrix.Tag;
				m_projection_matrix_tag = GL.ProjectionMatrix.Tag;

				Matrix4 mvp = GL.GetMVP();

				m_shader_program.SetUniformValue( 0, ref mvp );
			}

			if ( m_shader_depth++ != 0 )
				return;

			GL.Context.SetShaderProgram( m_shader_program );
		}

		/// <summary>
		/// "Pop" the shader. Number of ShaderPush() calls must match the number of ShaderPush().
		/// </summary>
		public void ShaderPop()
		{
			Common.Assert( m_shader_depth > 0 );

			if ( --m_shader_depth != 0 )
				return;

//			GL.Context.SetShaderProgram( null ); // would like to go back to clean state but this asserts
		}

		/// <summary>
		/// Set the color to be used by the next calls to ImmVertex.
		/// </summary>
		public void SetColor( Vector4 value )
		{
			m_current_color = value;
		}

		/// <summary>
		/// Draw a filled axis aligned rectangle.
		/// </summary>
		public void DrawBounds2Fill( Bounds2 bounds )
		{
			ShaderPush();

			ImmBegin( DrawMode.TriangleStrip, 4 );
//			ImmVertex( new Vertex( bounds.Point01.Xy01, Colors.Green ) ); // debug
//			ImmVertex( new Vertex( bounds.Point00.Xy01, Colors.Black ) ); // debug
//			ImmVertex( new Vertex( bounds.Point11.Xy01, Colors.Yellow ) ); // debug
//			ImmVertex( new Vertex( bounds.Point10.Xy01, Colors.Red ) ); // debug
			ImmVertex( new Vertex( bounds.Point01.Xy01, m_current_color ) );
			ImmVertex( new Vertex( bounds.Point00.Xy01, m_current_color ) );
			ImmVertex( new Vertex( bounds.Point11.Xy01, m_current_color ) );
			ImmVertex( new Vertex( bounds.Point10.Xy01, m_current_color ) );
			ImmEnd();

			ShaderPop();
		}

		/// <summary>
		/// Draw a wireframe axis aligned rectangle.
		/// </summary>
		public void DrawBounds2( Bounds2 bounds )
		{
			ShaderPush();

			ImmBegin( DrawMode.LineStrip, 5 );
			ImmVertex( new Vertex( bounds.Point00.Xy01, m_current_color ) );
			ImmVertex( new Vertex( bounds.Point10.Xy01, m_current_color ) );
			ImmVertex( new Vertex( bounds.Point11.Xy01, m_current_color ) );
			ImmVertex( new Vertex( bounds.Point01.Xy01, m_current_color ) );
			ImmVertex( new Vertex( bounds.Point00.Xy01, m_current_color ) );
			ImmEnd();

			ShaderPop();
		}

		/// <summary>
		/// Draw convex polygon.
		/// </summary>
		public void DrawConvexPoly2( ConvexPoly2 convex_poly )
		{
			if ( convex_poly.Planes.Length == 0 )
				return;

			ShaderPush();
 
			ImmBegin( DrawMode.LineStrip, (uint)convex_poly.Planes.Length + 1 );
			foreach ( Plane2 p in convex_poly.Planes )
				ImmVertex( new Vertex( p.Base, m_current_color ) );
			ImmVertex( new Vertex( convex_poly.Planes[0].Base, m_current_color ) );
			ImmEnd();

			ShaderPop();
		}

		/// <summary>
		/// Draw a filled disk.
		/// </summary>
		/// <param name="center">The center.</param>
		/// <param name="radius">The radius.</param>
		/// <param name="n">Tesselation.</param>
		public void DrawDisk( Vector2 center, float radius, uint n )
		{
			ShaderPush();

			ImmBegin( DrawMode.TriangleFan, n );
			for ( uint i=0;i<n;i++ )
			{
				Vector2 u = Vector2.Rotation( ((float)i/(float)(n-1)) * Math.TwicePi );
				ImmVertex( new Vertex( ( center + u * radius ).Xy01, m_current_color ) );
			}
			ImmEnd();

			ShaderPop();
		}

		/// <summary>
		/// Draw a filled circle.
		/// </summary>
		/// <param name="center">The center.</param>
		/// <param name="radius">The radius.</param>
		/// <param name="n">Tesselation.</param>
		public void DrawCircle( Vector2 center, float radius, uint n )
		{
			ShaderPush();

			ImmBegin( DrawMode.LineStrip, n );
			for ( uint i=0;i<n;i++ )
			{
				Vector2 u = Vector2.Rotation( ((float)i/(float)(n-1)) * Math.TwicePi );
				ImmVertex( new Vertex( ( center + u * radius ).Xy01, m_current_color ) );
			}
			ImmEnd();

			ShaderPop();
		}

		/// <summary>
		/// Draw the coordinate system represented by a transformation matrix 
		/// using arrows. The x vector is represented by a red arrow, and the y 
		/// vector is represented by a green arrow.
		/// </summary>
		public void DrawCoordinateSystem2D( Matrix3 mat, ArrowParams ap = null )
		{
			if ( ap == null ) 
				ap = new ArrowParams();

			ShaderPush();

			ImmBegin( DrawMode.Triangles, 9 * 2 );

			SetColor( Colors.Red );
			DrawArrow( mat.Z.Xy, mat.Z.Xy + mat.X.Xy, ap );

			SetColor( Colors.Green );
			DrawArrow( mat.Z.Xy, mat.Z.Xy + mat.Y.Xy, ap );

			ImmEnd();

			ShaderPop();
		}

		/// <summary>
		/// Draw a unit len arrow on x and y axis. Color is set to vector
		/// coordinates, so the x arrow is red (1,0,0), and the y arrow is
		/// green (0,1,0).
		/// </summary>
		public void DrawCoordinateSystem2D()
		{
			DrawCoordinateSystem2D( Matrix3.Identity );
		}

		/// <summary>
		/// Arrow parameters passed to DrawArrow.
		/// </summary>
		public class ArrowParams
		{
			/// <summary>
			/// Lenght of the base of the arrow's head.
			/// </summary>
			public float HeadRadius;
			/// <summary>
			/// Lenght of the arrow's head.
			/// </summary>
			public float HeadLen;
			/// <summary>
			/// Arrow's body's radius.
			/// </summary>
			public float BodyRadius;
			/// <summary>
			/// A scale factor applied to HeadRadius, HeadLen, BodyRadius.
			/// </summary>
			public float Scale;
			/// <summary>
			/// You can display half of the arrow (and select which side) handy for debugging half edge graphs for example.
			/// </summary>
			public uint HalfMask;
			/// <summary>
			/// Arrow end points can be offset along the perpendicular direction.
			/// </summary>
			public float Offset;

			/// <summary>
			/// ArrowParams's constructor.
			/// </summary>
			public ArrowParams( float r = 1.0f )
			{
				HeadRadius = 0.06f * r;
				HeadLen = 0.2f * r;
				BodyRadius = 0.02f * r;
				Scale = 1.0f;
				HalfMask = 3;
				Offset = 0.0f;
			}
		}

		/// <summary>
		/// Draw a 2d arrow. This function can be wrapped by ImmBegin()/ImmEnd(), 
		/// if you need to draw several arrows but want to limit the number of 
		/// draw calls. Each arrow consumes at most 9 vertices.
		/// </summary>
		/// <param name="start_point">Arrow's start point.</param>
		/// <param name="end_point">Arrow's tip.</param>
		/// <param name="ap">Arrow geometry parameters.</param>
		public void DrawArrow( Vector2 start_point, Vector2 end_point, ArrowParams ap )
		{
			Vector2 x = end_point - start_point;
			Vector2 x1 = x.Normalize();
			Vector2 y1 = Math.Perp( x1 );

			start_point += y1 * ap.Offset;
			end_point += y1 * ap.Offset;

			ap.BodyRadius *= ap.Scale;
			ap.HeadRadius *= ap.Scale;
			ap.HeadLen *= ap.Scale;

			float r2p = ap.HeadRadius;
			float r2n = ap.HeadRadius;
			float r1p = ap.BodyRadius;
			float r1n = ap.BodyRadius;

			if ( ( ap.HalfMask & 1 ) == 0 )
			{
				r2n=0.0f;
				r1n=0.0f;
			}

			if ( ( ap.HalfMask & 2 ) == 0 )
			{
				r2p=0.0f;
				r1p=0.0f;
			}

			ShaderPush();

			bool imm_was_active = m_imm.ImmActive;

			if ( ap.BodyRadius == 0.0f 
				 && !imm_was_active )
			{
				ImmBegin( DrawMode.Lines, 2 );
				ImmVertex( start_point );
				ImmVertex( end_point );
				ImmEnd();
			}

			if ( !imm_was_active ) 
				ImmBegin( DrawMode.Triangles, 9 );

			if ( ap.BodyRadius != 0.0f )
			{
				Vector2 p01 = start_point + r1p * y1;
				Vector2 p00 = start_point - r1n * y1;
				Vector2 p11 = end_point - x1 * ap.HeadLen + r1p * y1;
				Vector2 p10 = end_point - x1 * ap.HeadLen - r1n * y1;

				ImmVertex( p01 );
				ImmVertex( p00 );
				ImmVertex( p11 );
				ImmVertex( p00 );
				ImmVertex( p10 );
				ImmVertex( p11 );
			}

			ImmVertex( end_point - x1 * ap.HeadLen - r2n * y1 );
			ImmVertex( end_point );
			ImmVertex( end_point - x1 * ap.HeadLen + r2p * y1 );

			if ( !imm_was_active ) 
				ImmEnd();

			ShaderPop();
		}

		/// <summary>
		/// Draw a single line segment.
		/// This is expensive, if you draw many lines, don't use this function.
		/// </summary>
		public void DrawLineSegment( Vector2 A, Vector2 B )
		{
			ShaderPush();

			ImmBegin( DrawMode.Lines, 2 );
			ImmVertex( A );
			ImmVertex( B );
			ImmEnd();

			ShaderPop();
		}

		/// <summary>
		/// Draw a single line segment.
		/// This is expensive, if you draw many lines, don't use this function.
		/// </summary>
		public void DrawInfiniteLine( Vector2 A, Vector2 v )
		{
			ShaderPush();

			v *= 10000.0f;

			ImmBegin( DrawMode.Lines, 2 );
			ImmVertex( A - v );
			ImmVertex( A + v );
			ImmEnd();

			ShaderPop();
		}

		/// <summary>
		/// Draw all the vertical and horizontal lines in a given rectangle, regularly spaced.
		/// Since the smaller step_x or step_y are, the more lines primitives are generated, 
		/// it is easy to overflow the immediate draw mode vertex buffer. For that reason care 
		/// must be taken when setting the step values respective to the the bounds clip area.
		/// </summary>
		/// <param name="step_x">X spacing (starts at 0).</param>
		/// <param name="step_y">Y spacing (starts at 0).</param> 
		/// <param name="bounds">Clipping rectangle.</param>
		public void DrawRulers( Bounds2 bounds, float step_x, float step_y )
		{
			step_x = FMath.Max( step_x, 0.0f );
			step_y = FMath.Max( step_y, 0.0f );

			if ( step_x < float.Epsilon ) return;
			if ( step_y < float.Epsilon ) return;

			float left = bounds.Min.X;
			float right = bounds.Max.X;
			float bottom = bounds.Min.Y;
			float top = bounds.Max.Y;

			int l = (int)( left / step_x );
			int r = (int)( right / step_x );
			int b = (int)( bottom / step_y );
			int t = (int)( top / step_y );

			ShaderPush();

			bool safe_x = (r-l+1)<1000;
			bool safe_y = (t-b+1)<1000;

			ImmBegin( DrawMode.Lines, ( safe_x ? (uint)((r-l+1)*2) : 0 ) + ( safe_y ? (uint)((t-b+1)*2) : 0 ) );

			if ( safe_x )
			{
				for ( int i=l;i<=r;++i )
				{
					ImmVertex( new Vector2( (float)i * step_x, bottom ) );
					ImmVertex( new Vector2( (float)i * step_x, top ) );
				}
			}
//			else
//			{
//				System.Console.WriteLine( "skip drawing of x rulers lines, too many lines" );
//			}

			if ( safe_y )
			{
				for ( int i=b;i<=t;++i )
				{
					ImmVertex( new Vector2( left, (float)i * step_y ) );
					ImmVertex( new Vector2( right, (float)i * step_y ) );
				}
			}
// 		else
// 		{
// 			System.Console.WriteLine( "skip drawing of y rulers lines, too many lines" );
// 		}

			ImmEnd();

			ShaderPop();
		}

		/// <summary>
		/// Draw axis lines (x=0 and y=0 lines) with a thickness.
		/// </summary>
		public void DrawAxis( Bounds2 clipping_bounds, float thickness )
		{
			GL.Context.SetLineWidth( thickness );

			float x = 0.0f;
			float y = 0.0f;

			ShaderPush();

			ImmBegin( DrawMode.Lines, 4 );

			// x=0
			ImmVertex( new Vector2( clipping_bounds.Min.X /*left*/, y ) );
			ImmVertex( new Vector2( clipping_bounds.Max.X/*right*/, y ) );

			// y=0
			ImmVertex( new Vector2( x, clipping_bounds.Min.Y/*bottom*/ ) );
			ImmVertex( new Vector2( x, clipping_bounds.Max.Y/*top*/ ) );

			ImmEnd();

			ShaderPop();

			GL.Context.SetLineWidth( 1.0f );
		}

		/// <summary>
		/// This function draws all the vertical and horizontal lines (rulers) regularly placed 
		/// at multiples of 'step' distances that are inside the rectangle 'clipping_bounds'. 
		/// It also draws the the 2 thick axis lines. All lines drawn are clipped again 
		/// 'clipping_bounds'. Blend mode is untouched when drawing the rulers, then blend is 
		/// disabled when drawing axis lines.
		/// </summary>
		/// <param name="clipping_bounds">Clipping rectangle.</param>
		/// <param name="step">Horizontal and vertical spacing between rulers.</param>
		/// <param name="rulers_color">Color of rulers lines.</param>
		/// <param name="axis_color">Color of axis lines.</param>
		public void DrawDefaultGrid( Bounds2 clipping_bounds, Vector2 step, Vector4 rulers_color, Vector4 axis_color )
		{
			//Common.Profiler.Push("DrawHelpers.DrawDefaultGrid");

			ShaderPush();

			SetColor( rulers_color );
			DrawRulers( clipping_bounds, step.X, step.Y );

			GL.Context.Disable( EnableMode.Blend );

			SetColor( axis_color );
			DrawAxis( clipping_bounds, 2.0f );

			ShaderPop();

			//Common.Profiler.Pop();
		}

		/// <summary>
		/// DrawDefaultGrid with a default color/blend.
		/// </summary>
		public void DrawDefaultGrid( Bounds2 clipping_bounds, float step )
		{
			GL.Context.Enable( EnableMode.Blend );
			GL.Context.SetBlendFunc( new BlendFunc( BlendFuncMode.Add, BlendFuncFactor.SrcAlpha, BlendFuncFactor.One ) );

			DrawDefaultGrid( clipping_bounds, new Vector2( step ), Colors.Grey30 * 0.5f, Colors.Black );
		}
	}
}

