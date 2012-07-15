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
	/// ImmediateModeQuads wraps ImmediateMode to deal with quad rendering only.
	/// This is used by SpriteRenderer and other places.
	/// </summary>
	public class ImmediateModeQuads<T> : System.IDisposable
	{
		ImmediateMode<T> m_imm;
		uint m_max_quads;

		bool m_disposed = false;
		/// <summary>Return true if this object been disposed.</summary>
		public bool Disposed { get { return m_disposed; } }

		/// <summary>
		/// Return the maximum (total) number of quads we can add per frame.
		/// </summary>
		public uint MaxQuads { get { return m_max_quads; } }

		/// <summary>
		/// </summary>
		public ImmediateModeQuads( GraphicsContextAlpha gl, uint max_quads, params VertexFormat[] formats )
		{
			#if IMMEDIATE_MODE_XPERIA_WORKAROUND_HACK
			// clamp to next power of 2 so that indices get the right number of vertices in VertexBufferPool
			#if IMMEDIATE_MODE_QUADS_USES_INDEXING
			{
				int p = Math.Log2( (int)max_quads * 4 );
				if ( ( 1 << p ) < max_quads * 4 )
					++p;
				max_quads = (uint)( ( 1 << p ) / 4 );
			}
			#else // #if IMMEDIATE_MODE_QUADS_USES_INDEXING
			{
				int p = Math.Log2( (int)max_quads * 6 );
				if ( ( 1 << p ) < max_quads * 6 ) ++p;
				max_quads = (uint)( ( 1 << p ) / 6 );
			}
			#endif // #if IMMEDIATE_MODE_QUADS_USES_INDEXING
			#endif
			
			m_max_quads = max_quads;
			
			ushort[] indices = null;
			
			#if IMMEDIATE_MODE_QUADS_USES_INDEXING
			
			// note: the lack of a sharable IndexBuffer objects forces us to copy indices for all VertexBuffers here
			
			indices = new ushort[ m_max_quads * 6 ];
			ushort[] quad_indices = new ushort[6] { 0,1,3,0,3,2};
			
			// manually repeat quad indexes for now

			for ( int q=0,i=0; q < (int)max_quads; ++q )
			{
				Common.Assert( i + 6 <= indices.Length );
				for ( int k=0; k<6; ++k )
					indices[i++] = (ushort)( q * 4 + quad_indices[k] );
			}

			m_imm = new ImmediateMode< T >( gl, max_quads * 4, indices, 4, 6, formats );
			#else // #if IMMEDIATE_MODE_QUADS_USES_INDEXING
			m_imm = new ImmediateMode< T >( gl, max_quads * 6, null, 0, 0, formats );
			#endif // #if IMMEDIATE_MODE_QUADS_USES_INDEXING
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
				m_disposed = true;
			}
		}

		/// <summary>Prepare for registering n quads for rendering.</summary>
		/// <param name="num_quads">The maximum number of quads you intend to add.</param>
		public void ImmBeginQuads( uint num_quads )
		{
			m_imm.ImmBegin(
				DrawMode.Triangles, 
				#if IMMEDIATE_MODE_QUADS_USES_INDEXING
				(uint)(num_quads * 4) );
				#else // #if IMMEDIATE_MODE_QUADS_USES_INDEXING
				(uint)(num_quads * 6) );
				#endif // #if IMMEDIATE_MODE_QUADS_USES_INDEXING
		}

		/// <summary>
		/// Add a quad
		/// 
		/// v2----v3
		/// 
		/// |  | 
		/// 
		/// v0----v1
		/// 
		/// </summary>
		public void ImmAddQuad( T v0, T v1, T v2, T v3 )
		{
			#if IMMEDIATE_MODE_QUADS_USES_INDEXING
			// indexing does 013-023
			// note: inlining didn't make much difference
			m_imm.ImmVertex( v0 );
			m_imm.ImmVertex( v1 );
			m_imm.ImmVertex( v2 );
			m_imm.ImmVertex( v3 );
			#else // #if IMMEDIATE_MODE_QUADS_USES_INDEXING
			// copy 2 more vertices to form 2 triangles
			m_imm.ImmVertex( v0 );
			m_imm.ImmVertex( v1 );
			m_imm.ImmVertex( v3 );
			m_imm.ImmVertex( v0 );
			m_imm.ImmVertex( v3 );
			m_imm.ImmVertex( v2 );
			#endif // #if IMMEDIATE_MODE_QUADS_USES_INDEXING
		}

		/// <summary>
		/// Add a quad
		/// 
		/// v[2]----v[3]
		/// 
		///  |   | 
		/// 
		/// v[0]----v[1]
		/// 
		/// </summary>
		public void ImmAddQuad( T[] v )
		{
			#if IMMEDIATE_MODE_QUADS_USES_INDEXING
			// indexing does 013-023
			// note: inlining didn't make much difference
			m_imm.ImmVertex( v[0] );
			m_imm.ImmVertex( v[1] );
			m_imm.ImmVertex( v[2] );
			m_imm.ImmVertex( v[3] );
			#else // #if IMMEDIATE_MODE_QUADS_USES_INDEXING
			// copy 2 more vertices to form 2 triangles
			m_imm.ImmVertex( v[0] );
			m_imm.ImmVertex( v[1] );
			m_imm.ImmVertex( v[3] );
			m_imm.ImmVertex( v[0] );
			m_imm.ImmVertex( v[3] );
			m_imm.ImmVertex( v[2] );
			#endif // #if IMMEDIATE_MODE_QUADS_USES_INDEXING
		}

		/// <summary>
		/// Draw all the quads added since the last ImmBeginQuads
		/// </summary>
		public void ImmEndQuads()
		{
			#if IMMEDIATE_MODE_QUADS_USES_INDEXING
			m_imm.ImmEndIndexing();
			#else // #if IMMEDIATE_MODE_QUADS_USES_INDEXING
			m_imm.ImmEnd();
			#endif // #if IMMEDIATE_MODE_QUADS_USES_INDEXING
		}
	}
}

