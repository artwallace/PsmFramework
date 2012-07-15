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
#if IMMEDIATE_MODE_XPERIA_WORKAROUND_HACK

	// As the name implies, VertexBufferPool is a pool of VertexBuffer objects, 
	// so we can allocate/reuse at runtime. The reason this class exists is 
	// because of a problem on xperia/OpenGL(?) where render seems to get stuck 
	// if we try to reuse a VertexBuffer for multipled DrawArrays calls (!). 
	// To work around that, we "simply" allocate a VertexBuffer object for 
	// each DrawArrays call(!) - in order to do that we must be able to quickly 
	// get a VertexBuffer of any size at anytime at runtime. In this pool 
	// each queried size is rounded to the next power of 2. Note that not 
	// only this is wasting lots of memory (because of the padding) and 
	// causing a slight fragmentation risk because of the runtime allocations, 
	// but on other platforms (Vita) it ruins some small optimizations that 
	// were relying on "1 VertexBuffer, N DrawArrays" situations. But the 
	// decision was made to have one code path for all patforms.

	internal class VertexBufferPool : System.IDisposable
	{
		bool m_disposed = false;
		//Return true if this object been disposed.
		public bool Disposed { get { return m_disposed; } }

		class Entry
		{
			internal VertexBuffer m_vertex_buffer;
			internal uint m_frame_count; // remember last used frame so we can discard old items
		}

		// A list of VertexBuffer objects of a given size.

		class PerSizeList
		{
			internal VertexBufferPool m_parent;
			internal int m_max_vertices; // this is a power of 2
			internal List< Entry > m_active_list;
			internal List< Entry > m_free_list;

			public PerSizeList( int max_vertices )
			{
				m_max_vertices = max_vertices;
				m_active_list = new List< Entry >();
				m_free_list = new List< Entry >();
			}

			public VertexBuffer GetAVertexBuffer()
			{
				if ( m_free_list.Count == 0 )
				{
					int num_indices = 0;

					VertexBuffer vb;

//					#ifdef DEBUG
//					System.Console.WriteLine( Common.FrameCount + " VertexBufferPool: create VertexBuffer, size=" + m_max_vertices );
//					#endif

					if ( null == m_parent.m_indices )
					{
						vb = new VertexBuffer( (int)m_max_vertices, 0, m_parent.m_format );
					}
					else
					{
						num_indices = ( m_max_vertices / m_parent.m_vertices_per_primitive ) * m_parent.m_indices_per_primitive;
						vb = new VertexBuffer( (int)m_max_vertices, num_indices, m_parent.m_format );

						// note: the real number of indices we are going to need is not the one based on the power of 2 snapped 
						// number of vertices, but SetIndices must be passed a size that matches the created VertexBuffer size 
						// I think? (or you get a native function error)
						Common.Assert( num_indices <= m_parent.m_indices.Length);

						vb.SetIndices( m_parent.m_indices, 0, 0, num_indices );
					}

					m_free_list.Add( new Entry() { m_vertex_buffer = vb, m_frame_count = Common.FrameCount } );
				}

				Entry ret = m_free_list[ m_free_list.Count - 1 ];
				m_free_list.RemoveAt( m_free_list.Count - 1 );
				m_active_list.Add( ret );
				return ret.m_vertex_buffer;
			}
		}

		List< PerSizeList > m_per_size_lists;

		ushort[] m_indices;
		int m_vertices_per_primitive; // the number of vertices each ImmBegin is expected to have (relevant only if indices is set, 0 if not used)
		int m_indices_per_primitive; // the number of indices we want to draw for each primitive (relevant only if indices is set, 0 if not used)
		VertexFormat[] m_format;
		public int DisposeInterval = 30 * 60;

		public VertexBufferPool( ushort[] indices_model
								 , int vertices_per_primitive
								 , int indices_per_primitive
								 , params VertexFormat[] formats )
		{
			m_per_size_lists = new List< PerSizeList >();

			m_indices = indices_model;
			m_vertices_per_primitive = vertices_per_primitive;
			m_indices_per_primitive = indices_per_primitive;
			m_format = formats;

			for ( int p=0; p < 20; ++p )
			{
				m_per_size_lists.Add( new PerSizeList(1<<p) );
				m_per_size_lists[ m_per_size_lists.Count - 1 ].m_parent = this;
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if(disposing)
			{
				foreach ( PerSizeList entry in m_per_size_lists )
				{
					foreach ( Entry v in entry.m_active_list )
						Common.DisposeAndNullify< VertexBuffer >( ref v.m_vertex_buffer );
					entry.m_active_list.Clear();

					foreach ( Entry v in entry.m_free_list )
						Common.DisposeAndNullify< VertexBuffer >( ref v.m_vertex_buffer );
					entry.m_free_list.Clear();
				}

				m_disposed = true;
			}
		}

		// Call this when you know the frame has changed, so we can free all VertexBuffer objects again.
		public void OnFrameChanged()
		{
			foreach ( PerSizeList entry in m_per_size_lists )
			{
				foreach ( Entry v in entry.m_active_list )
				{
					#if IMMEDIATE_MODE_XPERIA_WORKAROUND_HACK_DISCARD_UNUSED_VERTEX_BUFFER
					// Discard the older VerteBuffer objects that haven't been used in a long time. Note: 
					// we could spread those discard more across frames, or clamp them to n per frame etc
					// (if that ever becomes a problem).

					if ( Common.FrameCount - v.m_frame_count > DisposeInterval )
					{
//						System.Console.WriteLine( Common.FrameCount+ " Disposing of a " + entry.m_max_vertices + " VertexBuffer" );
						Common.DisposeAndNullify< VertexBuffer >( ref v.m_vertex_buffer );
					}
					else 
					#endif
					{
						entry.m_free_list.Add( v );
					}
				}
				entry.m_active_list.Clear();
			}

//			Dump();
		}

		public VertexBuffer GetAVertexBuffer( int max_vertices )
		{
			int p = Math.Log2( max_vertices );
			if ( ( 1 << p ) < max_vertices ) ++p;

//			#ifdef DEBUG
//			System.Console.WriteLine( "max_vertices=" + max_vertices + " next=" + (1<<p) + " index=" + p );
//			#endif

			return m_per_size_lists[ p ].GetAVertexBuffer();
		}

		public void Dump()
		{
			foreach ( PerSizeList entry in m_per_size_lists )
			{
				if ( entry.m_free_list.Count == 0 && entry.m_active_list.Count == 0 )
					continue;

				System.Console.WriteLine( Common.FrameCount + " " + entry.m_max_vertices + " vertices : " + entry.m_free_list.Count + entry.m_active_list.Count );
			}
		}
	}

	#endif // #if IMMEDIATE_MODE_XPERIA_WORKAROUND_HACK
}

