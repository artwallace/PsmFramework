/* SCE CONFIDENTIAL
 * PlayStation(R)Suite SDK 0.98.2
 * Copyright (C) 2012 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using Sce.PlayStation.Core;

namespace Sce.PlayStation.HighLevel.GameEngine2D.Base
{
	/// <summary>
	/// A simple, OpenGL like, transform stack,
	/// with some optimization hints for dealing
	/// with orthonormal matrices.
	/// </summary>
	public class MatrixStack
	{
		internal struct Entry
		{
			public Matrix4 m_value; // this level's matrix
			public Matrix4 m_inverse; // this level's matrix's inverse
			public bool m_inverse_dirty; // if true, m_inverse needs to be recalculated
			public bool m_orthonormal; // flag that says if this matrix is orthonormal (if it is, inverse calculation can use a faster method)
		}

		Entry[] m_stack;

		uint m_index;
		uint m_capacity;
		uint m_tag;

		/// <summary>
		/// Tag is a number that gets incremented everytime the top matrix content changes.
		/// </summary>
		public uint Tag
		{
			get { return m_tag;}
		}

		/// <summary>MatrixStack constructor.</summary>
		/// <param name="capacity">Maximum depth of the stack.</param>
		public MatrixStack( uint capacity )
		{
			m_index = 0;
			m_capacity = capacity;

			m_stack = new Entry[capacity];

			m_stack[m_index].m_value = Matrix4.Identity;
			m_stack[m_index].m_inverse = Matrix4.Identity;
			m_stack[m_index].m_inverse_dirty = false;
			m_stack[m_index].m_orthonormal = true;

			m_tag = 0;
		}

		/// <summary>
		/// Size of current stack.
		/// </summary>
		public uint Size
		{
			get { return m_index + 1; } 
		}

		/// <summary>
		/// Maximum stack size.
		/// </summary>
		public uint Capacity
		{
			get { return m_capacity; } 
		}

		/// <summary>
		/// Push matrix pushes a copy of current top matrix.
		/// </summary>
		public void Push()
		{
			m_index++;

			Common.Assert( m_index < m_capacity );

			m_stack[m_index] = m_stack[m_index-1];

			m_tag++;
		}

		/// <summary>
		/// Pop the top matrix.
		/// </summary>
		public void Pop()
		{
			Common.Assert( m_index >= 0 );

			m_index--;

			m_tag++;
		}

		/// <summary>
		/// Right multiply top matrix by 'mat'.
		/// </summary>
		public void Mul( Matrix4 mat )
		{
			m_stack[m_index].m_value = m_stack[m_index].m_value * mat;
			m_stack[m_index].m_orthonormal = false;	// we don't know
			m_stack[m_index].m_inverse_dirty = true;
			m_tag++;
		}

		/// <summary>
		/// Right Multiply top matrix by an orthonormal matrix 'mat'. 
		/// If you know that your matrix is orthonormal, you can use 
		/// Mul1 instead of Mul. As long as you keep operating on 
		/// orthonormal matrices, inverse calculations will be faster.
		/// </summary>
		public void Mul1( Matrix4 mat )
		{
			m_stack[m_index].m_value = m_stack[m_index].m_value * mat;
			//m_stack[m_index].m_orthonormal is unchanged
			m_stack[m_index].m_inverse_dirty = true;
			m_tag++;
		}

		/// <summary>
		/// Set the current matrix (top of the matrix stack).
		/// </summary>
		public void Set( Matrix4 mat )
		{
			m_stack[m_index].m_value = mat;
			m_stack[m_index].m_orthonormal = false; // we don't know
			m_stack[m_index].m_inverse_dirty = true;
			m_tag++;
		}

		/// <summary>
		/// Set an orthonormal matrix. If you know that your matrix
		/// is orthonormal, you can use Set1 instead of Set. As long
		/// as you keep operating on orthonormal matrices, inverse
		/// calculations will be faster.
		/// </summary>
		public void Set1( Matrix4 mat )
		{
			m_stack[m_index].m_value = mat;
			m_stack[m_index].m_orthonormal = true;
			m_stack[m_index].m_inverse_dirty = true;
			m_tag++;
		}

		/// <summary>
		/// Get the current matrix (top of the matrix stack).
		/// </summary>
		public Matrix4 Get()
		{
			return m_stack[m_index].m_value;
		}

		/// <summary>
		/// Update (if necessary) and return the cached inverse matrix.
		/// </summary>
		public Matrix4 GetInverse()
		{
			if ( m_stack[m_index].m_inverse_dirty )
			{
				if ( m_stack[m_index].m_orthonormal )
					m_stack[m_index].m_inverse = m_stack[m_index].m_value.InverseOrthonormal();
				else
					m_stack[m_index].m_inverse	= m_stack[m_index].m_value.Inverse();

				m_stack[m_index].m_inverse_dirty = false;
			}
			return m_stack[m_index].m_inverse;
		}

		/// <summary>
		/// Set the top matrix to identity.
		/// </summary>
		public void SetIdentity()
		{
			m_stack[m_index].m_value = Matrix4.Identity;
			m_stack[m_index].m_inverse = Matrix4.Identity;
			m_stack[m_index].m_orthonormal = true;
			m_stack[m_index].m_inverse_dirty = false;
			m_tag++;
		}

		/// <summary>
		/// Rotate the top matrix around X axis.
		/// </summary>
		public void RotateX( float angle ) 
		{
			m_stack[m_index].m_value = m_stack[m_index].m_value * Matrix4.RotationX( angle );
			//m_stack[m_index].m_orthonormal is unchanged
			m_stack[m_index].m_inverse_dirty = true;
			m_tag++;
		}

		/// <summary>
		/// Rotate the top matrix around Y axis.
		/// </summary>
		public void RotateY( float angle ) 
		{
			m_stack[m_index].m_value = m_stack[m_index].m_value * Matrix4.RotationY( angle );
			//m_stack[m_index].m_orthonormal is unchanged
			m_stack[m_index].m_inverse_dirty = true;
			m_tag++;
		}

		/// <summary>
		/// Rotate the top matrix around Z axis.
		/// </summary>
		public void RotateZ( float angle ) 
		{
			m_stack[m_index].m_value = m_stack[m_index].m_value * Matrix4.RotationZ( angle );
			//m_stack[m_index].m_orthonormal is unchanged
			m_stack[m_index].m_inverse_dirty = true;
			m_tag++;
		}

		/// <summary>
		/// Rotate the top matrix around a user given axis.
		/// </summary>
		public void Rotate( Vector3 axis, float angle ) 
		{
			m_stack[m_index].m_value = m_stack[m_index].m_value * Matrix4.RotationAxis( axis, angle );
			//m_stack[m_index].m_orthonormal is unchanged
			m_stack[m_index].m_inverse_dirty = true;
			m_tag++;
		}

		/// <summary>
		/// Scale the top matrix.
		/// </summary>
		public void Scale( Vector3 value ) 
		{
			if ( value == Math._111 ) return;

			m_stack[m_index].m_value = m_stack[m_index].m_value * Matrix4.Scale( value );
			m_stack[m_index].m_orthonormal = false;
			m_stack[m_index].m_inverse_dirty = true;

			m_tag++;
		}

		/// <summary>
		/// Translate the top matrix.
		/// </summary>
		public void Translate( Vector3 value ) 
		{
			if ( value == Math._000 ) return;

			m_stack[m_index].m_value = m_stack[m_index].m_value * Matrix4.Translation( value );
			//m_stack[m_index].m_orthonormal is unchanged
			m_stack[m_index].m_inverse_dirty = true;

			m_tag++;
		}
	}
}
