/* SCE CONFIDENTIAL
 * PlayStation(R)Suite SDK 0.98.2
 * Copyright (C) 2012 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics; // BlendMode
using Sce.PlayStation.Core.Imaging; // Font

namespace Sce.PlayStation.HighLevel.GameEngine2D.Base
{
	/// <summary>
	/// Augment Sce.PlayStation.Core.Graphics.GraphicsContext with a matrix stack and a couple of other functions.
	/// </summary>
	public class GraphicsContextAlpha : System.IDisposable
	{
		Sce.PlayStation.Core.Graphics.GraphicsContext m_context;
		bool m_context_must_be_disposed; // if true, m_context is disposed of in this class
		Texture2D m_white_texture;
		TextureInfo m_white_texture_info;

		/// <summary>
		/// The core graphics context.
		/// </summary>
		public Sce.PlayStation.Core.Graphics.GraphicsContext Context { get { return m_context; } }

		bool m_disposed = false;
		/// <summary>Return true if this object been disposed.</summary>
		public bool Disposed { get { return m_disposed; } }

		/// <summary>GraphicsContextAlpha constructor.</summary>
		public GraphicsContextAlpha( Sce.PlayStation.Core.Graphics.GraphicsContext context = null )
		{
			m_context = context;
			m_context_must_be_disposed = false;

			if ( m_context == null )
			{
				m_context = new Sce.PlayStation.Core.Graphics.GraphicsContext();
				m_context_must_be_disposed = true; // this class takes ownership of m_context
			}

			ModelMatrix = new MatrixStack(16);
			ViewMatrix = new MatrixStack(16);
			ProjectionMatrix = new MatrixStack(8);
			m_white_texture = CreateTextureUnicolor( 0xffffffff );
			m_white_texture_info = new TextureInfo( m_white_texture );
			DebugStats = new DebugStats_();
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
				if ( m_context_must_be_disposed )
					Common.DisposeAndNullify< Sce.PlayStation.Core.Graphics.GraphicsContext >( ref m_context );
				Common.DisposeAndNullify< Texture2D >( ref m_white_texture );
				m_disposed = true;
			}
		}

		/// <summary>
		/// The model matrix stack, similar to OpenGL.
		/// </summary>
		public MatrixStack ModelMatrix;

		/// <summary>
		/// The view matrix stack, similar to OpenGL.
		/// </summary>
		public MatrixStack ViewMatrix;

		/// <summary>
		/// The projection matrix stack, similar to OpenGL.
		/// </summary>
		public MatrixStack ProjectionMatrix;

		/// <summary>
		/// DebugStats at the moment only stores a DrawArrays calls counter.
		/// </summary>
		public class DebugStats_
		{
			uint m_current_frame = 0;

			/// <summary>
			/// Number of times DrawArrays got called by the GameEngine2D library (not including your own calls to that function). 
			/// Since DrawArrays is a costly function, this counter is a useful profiling information.
			/// The counter gets reset every frame.
			/// </summary>
			public uint DrawArraysCount = 0;

			public void OnDrawArray()
			{
				if ( m_current_frame != Common.FrameCount )
				{
					m_current_frame = Common.FrameCount;
					DrawArraysCount = 0;
				}
				++DrawArraysCount;
			}
		}

		/// <summary>
		/// DebugStats for simple profiling.
		/// </summary>
		public DebugStats_ DebugStats;
		
		// Return a small white texture with all pixels set to 0xffffffff.
		// Used as default in some shaders.
//		public Texture2D WhiteTexture 
//		{ 
//			get { return m_white_texture; }
//		}

		/// <summary>
		/// Return a small white texture with all pixels set to 0xffffffff, as a TextureInfo
		/// Used as default in some shaders.
		/// </summary>
		public TextureInfo WhiteTextureInfo
		{ 
			get { return m_white_texture_info; }
		}

		/// <summary>
		/// GetMVP() is a shortcut for ProjectionMatrix.Get() * ViewMatrix.Get() * ModelMatrix.Get().
		/// </summary>
		public Matrix4 GetMVP()
		{
			return ProjectionMatrix.Get() * ViewMatrix.Get() * ModelMatrix.Get();
		}

		/// <summary>
		/// GetAspect() is a shortcut that returns the viewport's aspect ratio (width/height).
		/// </summary>
		public float GetAspect()
		{
			ImageRect r = Context.GetViewport();
			Common.Assert( ( r.Width !=0 ) && ( r.Height != 0 ) );
			return(float)r.Width/(float)r.Height;
		}

		/// <summary>
		/// This function returns the viewport as a Bounds2 object.
		/// </summary>
		public Bounds2 GetViewportf()
		{
			ImageRect r = Context.GetViewport();
			return new Bounds2(
				new Vector2( r.X, r.Y ) ,
				new Vector2( r.X + r.Width, r.Y + r.Height )
				);
		}

		/// <summary>
		/// Set the depth write mask.
		/// </summary>
		public void SetDepthMask( bool write )
		{
			DepthFunc func = Context.GetDepthFunc();
			func.WriteMask = write;
			Context.SetDepthFunc( func );
		}

		/// <summary>
		/// Set the blend mode.
		/// </summary>
		public void SetBlendMode( BlendMode mode )
		{
			if ( !mode.Enabled ) Context.Disable( EnableMode.Blend );
			else
			{
				Context.Enable( EnableMode.Blend );
				Context.SetBlendFunc( mode.BlendFunc );
			}
		}

		/// <summary>
		/// Create a small texture where all pixels have the same color.
		/// </summary>
		public static Texture2D CreateTextureUnicolor( uint color )
		{
			int w = 16;
			int h = 16;
			uint[] data = new uint[ w * h ];
			for ( int i=0; i < data.Length; ++i )
				data[i] = color;
			var texture = new Texture2D( w, h, false, PixelFormat.Rgba );
			texture.SetPixels( 0, data );
			return texture;
		}

		/// <summary>
		/// Given a Font object and a text, create a texture representing text.
		/// </summary>
		public static Texture2D CreateTextureFromFont( string text, Font font, uint color = 0xffffffff )
		{
			int width = font.GetTextWidth( text, 0, text.Length );
			int height = font.Metrics.Height;

			var image = new Image(
				ImageMode.Rgba,
				new ImageSize( width, height ),
				new ImageColor(0,0,0,0)
				);

			image.DrawText(
				text,
				new ImageColor(
					(int)( ( color >> 16 ) & 0xff ),
					(int)( ( color >> 8 ) & 0xff ),
					(int)( ( color >> 0 ) & 0xff ),
					(int)( ( color >> 24 ) & 0xff )
					),
				font,
				new ImagePosition(0,0)
				);

			var texture = new Texture2D( width, height, false, PixelFormat.Rgba );
			texture.SetPixels( 0, image.ToBuffer() );
			image.Dispose();

			return texture;
		}
	}
}
