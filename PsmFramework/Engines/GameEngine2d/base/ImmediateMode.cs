/* SCE CONFIDENTIAL
 * PlayStation(R)Suite SDK 0.98.2
 * Copyright (C) 2012 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

#define IMMEDIATE_MODE_QUADS_USES_INDEXING
#define IMMEDIATE_MODE_XPERIA_WORKAROUND_HACK // special hack for xepria, where it seems we need one VertexBuffer per DrawArray or performance is abysmal
#define IMMEDIATE_MODE_XPERIA_WORKAROUND_HACK_DISCARD_UNUSED_VERTEX_BUFFER

using System;
using System.Collections.Generic;

using Sce.PlayStation.Core.Graphics;

namespace Sce.PlayStation.HighLevel.GameEngine2D.Base
{
	/// <summary>
	/// An immediate mode vertex array that you can write everyframe
	/// using a ImmBegin()/ImmVertex()/ImmEnd() OpenGL style interface.
	/// </summary>
	public class ImmediateMode<T> : System.IDisposable
	{
		GraphicsContextAlpha GL;
		#if IMMEDIATE_MODE_XPERIA_WORKAROUND_HACK
		VertexBufferPool m_vbuf_pool;
		#else // #if IMMEDIATE_MODE_XPERIA_WORKAROUND_HACK
		VertexBuffer[] m_vertex_buffers; // n buffer, to deal with xperia GL drivers(?) problems
		int m_current_vertex_buffer_index;
		#endif // #if IMMEDIATE_MODE_XPERIA_WORKAROUND_HACK
		VertexBuffer m_current_vertex_buffer;
		T[] m_vertices_tmp;
		uint m_max_vertices; // the maximum number of total vertices written within one frame between ImmBegin/ImmEnd
		uint m_max_indices;
		DrawMode m_prim; // current ImmBegin draw primitive
		uint m_prim_start; // m_pos when ImmBegin is called... ( m_pos - m_prim_start ) is the number of vertices sent
		uint m_pos; // current position in written array
		uint m_frame_count; // last frame when an ImmBegin was called
		uint m_max_vertices_intended; // maximum number of vertices expected in the current ImmBegin/ImmEnd
		bool m_active; // true if we are between an immBegin and an ImmEnd
		int m_vertices_per_primitive; // the number of vertices each ImmBegin is expected to have (relevant only if indices is set, 0 if not used)
		int m_indices_per_primitive; // the number of indices we want to draw for each primitive (relevant only if indices is set, 0 if not used)

		bool m_disposed = false;
		/// <summary>Return true if this object been disposed.</summary>
		public bool Disposed { get { return m_disposed; } }

		/// <summary>
		/// ImmediateMode constructor.
		/// 
		/// If indices is not null, vertices_per_primitive and indices_per_primitive must follow the constraints below:
		/// 
		/// - vertices_per_primitive must not be 0
		/// - indices_per_primitive must not be 0
		/// - max_vertices must be a multiple of vertices_per_primitive
		/// - indices.Length must be a multiple of indices_per_primitive
		///	 - max_vertices / vertices_per_primitive must be equal to indices.Length / indices_per_primitive
		/// 
		/// If any of those constraints is not met, the constructor will assert.
		/// </summary>
		/// <param name="gl">The core graphics context.</param> 
		/// <param name="max_vertices">The maximum number of vertices you can have per frame.</param>
		/// <param name="indices">The array of indices (can be null), assuming a static setup.</param>
		/// <param name="vertices_per_primitive">If indices is not null, this must be set to the number of vertices each ImmBegin is expected to have. If indices is null, just set to 0.</param>
		/// <param name="indices_per_primitive">If indices is not null, this must be set to the number of indices you want to draw for each primitive. If indices is null, just set to 0.</param>
		/// <param name="formats">The vertex format, passed to VertexBuffer as it is.</param>
		public ImmediateMode( GraphicsContextAlpha gl, uint max_vertices, ushort[] indices, int vertices_per_primitive, int indices_per_primitive, params VertexFormat[] formats )
		{
/*
			#if IMMEDIATE_MODE_XPERIA_WORKAROUND_HACK
			{
				// clamp to next power of 2
				int p = Math.Log2( max_vertices );
				if ( ( 1 << p ) < max_vertices ) ++p;
				max_vertices = ( 1 << p );
			}
			#endif
*/
			GL = gl;

			m_max_vertices = max_vertices;
			m_vertices_per_primitive = vertices_per_primitive;
			m_indices_per_primitive = indices_per_primitive;
			m_vertices_tmp = new T[ m_max_vertices ];
			m_frame_count = unchecked((uint)-1);
			m_pos = 0;
			
			if ( indices != null )
			{
				 m_max_indices = (uint)indices.Length;

				Common.Assert( m_vertices_per_primitive != 0 );
				Common.Assert( m_indices_per_primitive != 0 );
				Common.Assert( ( m_max_vertices / m_vertices_per_primitive ) * m_vertices_per_primitive == m_max_vertices );
				Common.Assert( ( m_max_indices / m_indices_per_primitive ) * m_indices_per_primitive == m_max_indices );
				Common.Assert( ( m_max_vertices / m_vertices_per_primitive ) == ( m_max_indices / m_indices_per_primitive ) );
			}
			else 
			{
				m_max_indices = 0;
			}

			#if IMMEDIATE_MODE_XPERIA_WORKAROUND_HACK
			m_vbuf_pool = new VertexBufferPool( indices, m_vertices_per_primitive, m_indices_per_primitive, formats );
			#else // #if IMMEDIATE_MODE_XPERIA_WORKAROUND_HACK
//			m_vertex_buffers = new VertexBuffer[40]; // brute force test
			m_vertex_buffers = new VertexBuffer[2];
//			m_vertex_buffers = new VertexBuffer[1];

			for ( int i=0; i < m_vertex_buffers.Length; ++i )
			{
				m_vertex_buffers[i] = new VertexBuffer( (int)m_max_vertices, (int)m_max_indices, formats );

				if ( indices != null )
					m_vertex_buffers[i].SetIndices( indices, 0, 0, (int)m_max_indices );
			}
			#endif // #if IMMEDIATE_MODE_XPERIA_WORKAROUND_HACK
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
				#if IMMEDIATE_MODE_XPERIA_WORKAROUND_HACK
				m_vbuf_pool.Dispose();
				#else // #if IMMEDIATE_MODE_XPERIA_WORKAROUND_HACK
				for ( int i=0; i < m_vertex_buffers.Length; ++i )
					m_vertex_buffers[i].Dispose();
				m_vertex_buffers.Clear();
				m_current_vertex_buffer_index = -1;
				#endif // #if IMMEDIATE_MODE_XPERIA_WORKAROUND_HACK
				m_disposed = true;
			}
		}

		/// <summary>Begin a draw primitive.</summary>
		/// <param name="mode">The draw primitive type.</param>
		/// <param name="max_vertices_intended">The maximum number of vertices you intend to write.</param>
		public void ImmBegin( DrawMode mode, uint max_vertices_intended )
		{
			#if IMMEDIATE_MODE_XPERIA_WORKAROUND_HACK
			if ( m_frame_count != Common.FrameCount )
			{
				m_frame_count = Common.FrameCount;
				m_vbuf_pool.OnFrameChanged();
			}
			// get a new VertexBuffer for each DrawArray
			m_current_vertex_buffer = m_vbuf_pool.GetAVertexBuffer( (int)max_vertices_intended );
			m_pos = 0;
			#else // #if IMMEDIATE_MODE_XPERIA_WORKAROUND_HACK
			if ( m_frame_count != Common.FrameCount )
			{
				m_frame_count = Common.FrameCount;
				m_current_vertex_buffer_index = ( m_current_vertex_buffer_index + 1 ) % m_vertex_buffers.Length;
				m_current_vertex_buffer = m_vertex_buffers[ m_current_vertex_buffer_index ];
				m_pos = 0;
			}
			#endif // #if IMMEDIATE_MODE_XPERIA_WORKAROUND_HACK

			m_prim_start = m_pos;
			m_prim = mode;
			m_max_vertices_intended = max_vertices_intended;
			m_active = true;
		}

		/// <summary>
		/// Add a vertex, must be called between ImmBegin and ImmEnd.
		/// </summary>
		public void ImmVertex( T vertex )
		{
			#if DEBUG
			Common.Assert(m_pos<m_max_vertices,"ImmediateMode capacity overflow (m_max_vertices="+m_max_vertices+")");
			#endif

			m_vertices_tmp[m_pos++] = vertex;
		}

		void imm_end_prelude()
		{
			Common.Assert( m_pos - m_prim_start <= m_max_vertices_intended, "You added more vertices than you said you would." ); 

//			if ( m_current_vertex_buffer != GL.GetVertexBuffer( 0 ) )
				GL.Context.SetVertexBuffer( 0, m_current_vertex_buffer );

//			System.Console.WriteLine( Common.FrameCount + " " + ( m_pos - m_prim_start ) );

			m_current_vertex_buffer.SetVertices(m_vertices_tmp,(int)m_prim_start,(int)m_prim_start,(int)(m_pos-m_prim_start));
		}

		/// <summary>
		/// End draw primitive and draw.
		/// </summary>
		public void ImmEnd()
		{
			////Common.Profiler.Push("ImmediateMode<T>.ImmEnd");

			imm_end_prelude();

			GL.Context.DrawArrays( m_prim, (int)m_prim_start,(int)(m_pos-m_prim_start) );

			GL.DebugStats.OnDrawArray(); // count the number of DrawArrays per frame

			////Common.Profiler.Pop();

			m_active = false;
		}

		/// <summary>
		/// Special version of ImmEnd that uses the 'vertices_per_primitive' and 'indices_per_primitive' arguments
		/// passed to ImmediateMode's constructor. It is assumed that in that case all primitives consume the same
		/// amount of vertices.
		/// </summary>
		public void ImmEndIndexing()
		{
			////Common.Profiler.Push("ImmediateMode<T>.ImmEnd");
			
			#if DEBUG
			Common.Assert( ( ( m_pos - m_prim_start ) / m_vertices_per_primitive ) * m_vertices_per_primitive == ( m_pos - m_prim_start ) );
			Common.Assert( ( ( m_prim_start ) / m_vertices_per_primitive ) * m_vertices_per_primitive == m_prim_start );
			#endif

			imm_end_prelude();

			GL.Context.DrawArrays(
				m_prim,
				((int)m_prim_start / m_vertices_per_primitive) * m_indices_per_primitive,
				((int)(m_pos-m_prim_start) / m_vertices_per_primitive) * m_indices_per_primitive
				);

			GL.DebugStats.OnDrawArray(); // count the number of DrawArrays per frame

			////Common.Profiler.Pop();
		}

		/// <summary>
		/// Return true if we are in the middle of an ImmBegin()/ImmEnd().
		/// </summary>
		public bool ImmActive { get { return m_active;}}

		/// <summary>
		/// Return the maximum (total) number of vertices we can add per frame.
		/// </summary>
		public uint MaxVertices { get { return m_max_vertices; } }
	}
}

