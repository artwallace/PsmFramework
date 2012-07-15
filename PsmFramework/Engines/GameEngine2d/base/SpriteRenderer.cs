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
	/// SpriteRenderer wraps batch rendering of sprites in a simple BeginSprites / AddSprite x N / EndSprite API.
	/// It also provides some text rendering functions that uses FontMap.
	/// </summary>
	public class SpriteRenderer : System.IDisposable
	{
		GraphicsContextAlpha GL;
		ImmediateModeQuads<Vector4> m_imm_quads; // V2F_T2F 
		Vector4 m_v0;
		Vector4 m_v1;
		Vector4 m_v2;
		Vector4 m_v3;
		TextureInfo m_current_texture_info;
		TextureInfo m_embedded_font_texture_info; // used by DrawTextDebug

		/// <summary>
		/// Flag that will swap the U coordinates (horizontally) of all rendered sprites/quads.
		/// </summary>
		public bool FlipU = false;

		/// <summary>
		/// Flag that will swap the V coordinates (vertically) of all rendered sprites/quads.
		/// </summary>
		public bool FlipV = false;

		/// <summary>
		/// Sprites's default shader: texture modulated by a color.
		/// </summary>
		public class DefaultShader_ : ISpriteShader, System.IDisposable
		{
			public ShaderProgram m_shader_program;

			public DefaultShader_()
			{
				m_shader_program = Common.CreateShaderProgram( "sprite.cgx" );
				
				m_shader_program.SetUniformBinding( 0, "MVP" );
				m_shader_program.SetUniformBinding( 1, "Color" );
				m_shader_program.SetUniformBinding( 2, "UVTransform" );

				m_shader_program.SetAttributeBinding( 0, "vin_data" );

				Matrix4 identity = Matrix4.Identity;
				SetMVP( ref identity );
				SetColor( ref Colors.White );
				SetUVTransform( ref Math.UV_TransformFlipV );
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
					Common.DisposeAndNullify< ShaderProgram >( ref m_shader_program );
				}
			}

			public ShaderProgram GetShaderProgram() { return m_shader_program;}
			public void SetMVP( ref Matrix4 value ) { m_shader_program.SetUniformValue( 0, ref value );}
			public void SetColor( ref Vector4 value ) { m_shader_program.SetUniformValue( 1, ref value );}
			public void SetUVTransform( ref Vector4 value ) { m_shader_program.SetUniformValue( 2, ref value );}
		}

		DefaultShader_ m_default_shader = new DefaultShader_();
		/// <summary>The default shader used by SpriteRenderer.</summary>
		public DefaultShader_ DefaultShader { get { return m_default_shader; } }

		/// <summary>
		/// Font's default shader.
		/// </summary>
		public class DefaultFontShader_ : ISpriteShader, System.IDisposable
		{
			public ShaderProgram m_shader_program;

			public DefaultFontShader_()
			{
				m_shader_program = Common.CreateShaderProgram("font.cgx");

				m_shader_program.SetUniformBinding( 0, "MVP" );
				m_shader_program.SetUniformBinding( 1, "Color" );
				m_shader_program.SetUniformBinding( 2, "UVTransform" );

				m_shader_program.SetAttributeBinding( 0, "vin_data" );

				Matrix4 identity = Matrix4.Identity;
				SetMVP( ref identity );
				SetColor( ref Colors.White );
				SetUVTransform( ref Math.UV_TransformFlipV );
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
					Common.DisposeAndNullify<ShaderProgram>(ref m_shader_program);
				}
			}

			public ShaderProgram GetShaderProgram()
			{
				return m_shader_program;
			}
			
			public void SetMVP( ref Matrix4 value )
			{
				m_shader_program.SetUniformValue( 0, ref value );
			}
			
			public void SetColor( ref Vector4 value )
			{
				m_shader_program.SetUniformValue( 1, ref value );
			}
			
			public void SetUVTransform( ref Vector4 value )
			{
				m_shader_program.SetUniformValue( 2, ref value );
			}
		}

		DefaultFontShader_ m_default_font_shader = new DefaultFontShader_();
		/// <summary>The default font shader used by SpriteRenderer.</summary>
		public DefaultFontShader_ DefaultFontShader { get { return m_default_font_shader; } }

		bool m_disposed = false;
		/// <summary>Return true if this object been disposed.</summary>
		public bool Disposed { get { return m_disposed; } }

		/// <summary>SpriteRenderer constructor.</summary>
		public SpriteRenderer(GraphicsContextAlpha gl, uint max_sprites)
		{
			GL = gl;

			m_imm_quads = new ImmediateModeQuads< Vector4 >( GL, (uint)max_sprites, VertexFormat.Float4 );
		
			{
				// init the font texture used by DrawTextDebug

				Texture2D font_texture = EmbeddedDebugFontData.CreateTexture();

				m_embedded_font_texture_info = new TextureInfo();

				m_embedded_font_texture_info.Initialize(
					font_texture,
					new Vector2i( EmbeddedDebugFontData.NumChars, 1 ),
					new TRS(
						new Bounds2(
							new Vector2(0.0f, 0.0f),
							new Vector2(
								(EmbeddedDebugFontData.CharSizei.X * EmbeddedDebugFontData.NumChars ) / (float)font_texture.Width,
								(EmbeddedDebugFontData.CharSizei.Y / (float)font_texture.Height)
								)
							)
						)
					);
			}
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
				m_imm_quads.Dispose();
				m_embedded_font_texture_info.Dispose();

				Common.DisposeAndNullify< DefaultFontShader_ >( ref m_default_font_shader );
				Common.DisposeAndNullify< DefaultShader_ >( ref m_default_shader );
				m_disposed = true;
			}
		}

		/// <summary>
		/// Debug text draw function, using a small embedded font suitable for on screen debug prints.
		/// Since DrawTextDebug uses very small font data, it only work with ascii text (characters
		/// from ' ' to '~', 32 to 126) Any character outside this range will be displayed as '?'.
		/// For instance "こんにちは" will be displayed as "?????".
		/// </summary>
		/// <param name="str">The text to draw.</param>
		/// <param name="bottom_left_start_pos">The bottom left of the text rectangle, in world space/units.</param>
		/// <param name="char_height">The character height in world space/units.</param>
		/// <param name="draw">If false, don't draw anything, just return the Bounds2 used by the text.</param>
		/// <param name="shader">If no shader is specified, DefaultFontShader is used.</param>
		/// <returns>The rectangle area covered by rendered text (call with draw=false when you want to know the covered area before actually drawing it).</returns>
		public Bounds2 DrawTextDebug(
			string str,
			Vector2 bottom_left_start_pos,
			float char_height,
			bool draw = true,
			ISpriteShader shader = null
			)
		{
			if (null == shader)
				shader = (ISpriteShader)m_default_font_shader;

			float scale = ( char_height / EmbeddedDebugFontData.CharSizei.Y );

			Vector2 spacing_in_pixels = new Vector2( -1.0f, 1.0f );
			Vector2 spacing = spacing_in_pixels * scale;
			
			//1 character sprite bounding box in world space,
			//  could use Bounds2 instead really.
			//R and S stay unchaged, T gets incrementd as we draw chars
			TRS char_box;
			
			char_box.R = Math._10;
			char_box.S = EmbeddedDebugFontData.CharSizef * scale;
			char_box.T = bottom_left_start_pos;

			Vector2 max = bottom_left_start_pos;
			float left = char_box.T.X;

			if ( draw )
			{
				DefaultFontShader.SetUVTransform(ref Math.UV_TransformFlipV);
				BeginSprites(m_embedded_font_texture_info, DefaultFontShader, str.Length);
			}

			for ( int i=0; i < str.Length; ++i )
			{
				if ( str[i] == '\n')
				{
					char_box.T -= new Vector2( 0.0f, char_box.S.Y + spacing.Y );
					char_box.T.X = left;
					continue;
				}
				
				if (str[i] == '\r')
					continue;
				
				if ( draw )
				{
					int char_index = (int)str[i] - 32;

					if ( char_index < 0 || char_index >= EmbeddedDebugFontData.NumChars )
						char_index = (int)'?' - 32;

					AddSprite( ref char_box, new Vector2i( char_index, 0 ) );
				}

				char_box.T += new Vector2( char_box.S.X + spacing.X, 0.0f );

				max.X = FMath.Max( char_box.T.X, max.X );
				max.Y = FMath.Min( char_box.T.Y, max.Y );
			}

			if ( draw )
				EndSprites();

			return Bounds2.SafeBounds( bottom_left_start_pos + new Vector2( 0.0f, char_box.S.Y ), max );
		}

		/// <summary>This text draw function uses a FontMap object.</summary>
		/// <param name="str">The text to draw.</param>
		/// <param name="bottom_left_start_pos">The bottom left of the text rectangle, in world space/units.</param>
		/// <param name="char_height">The character height in world space/units.</param>
		/// <param name="draw">If false, don't draw anything, just return the Bounds2 used by the text.</param>
		/// <param name="fontmap">the fontmap object (that holds the texture).</param>
		/// <param name="shader">The shader defaults to SpriteRenderer.DefaultFontShader.</param>
		public Bounds2 DrawTextWithFontMap(
			string str,
			Vector2 bottom_left_start_pos,
			float char_height,
			bool draw,
			FontMap fontmap,
			ISpriteShader shader
			)
		{
			float scale = ( char_height / fontmap.CharPixelHeight );

			Vector2 spacing_in_pixels = new Vector2( 1.0f, 1.0f );
			Vector2 spacing = spacing_in_pixels * scale;

			Vector2 turtle = bottom_left_start_pos;

			Vector2 max = bottom_left_start_pos;
			float left = bottom_left_start_pos.X;

			if ( draw )
			{
				shader.SetUVTransform( ref Math.UV_TransformFlipV );
				BeginSprites( new TextureInfo( fontmap.Texture ), shader, str.Length );
			}

			for ( int i=0; i < str.Length; ++i )
			{
				if ( str[i] == '\n' )
				{
					turtle -= new Vector2( 0.0f, char_height + spacing.Y );
					turtle.X = left;
					continue;
				}

				FontMap.CharData cdata;
				if ( !fontmap.TryGetCharData( str[i], out cdata ) )
					continue;

				Vector2 scaled_char_size = cdata.PixelSize * scale;

				if ( draw )
					AddSprite( turtle, new Vector2(scaled_char_size.X,0.0f), cdata.UV );

				turtle += new Vector2( scaled_char_size.X + spacing.X, 0.0f );

				max.X = FMath.Max( turtle.X, max.X );
				max.Y = FMath.Min( turtle.Y, max.Y );
			}

			if ( draw )
				EndSprites();

			return Bounds2.SafeBounds( bottom_left_start_pos + new Vector2( 0.0f, char_height ), max );
		}

		/// <summary>This text draw function uses a FontMap object and SpriteRenderer's DefaultShader.</summary>
		/// <param name="str">The text to draw.</param>
		/// <param name="bottom_left_start_pos">The bottom left of the text rectangle, in world space/units.</param>
		/// <param name="char_height">The character height in world space/units.</param>
		/// <param name="draw">If false, don't draw anything, just return the Bounds2 used by the text.</param>
		/// <param name="fontmap">the fontmap object (that holds the texture).</param>
		public Bounds2 DrawTextWithFontMap(
			string str,
			Vector2 bottom_left_start_pos,
			float char_height,
			bool draw,
			FontMap fontmap
			)
		{
			return DrawTextWithFontMap(
				str,
				bottom_left_start_pos,
				char_height,
				draw,
				fontmap,
				(ISpriteShader)DefaultFontShader
				);
		}

		/// <summary>Start batch rendering of sprites.</summary>
		/// <param name="texture_info">The texture object.</param>
		/// <param name="shader">The shader object.</param>
		/// <param name="num_sprites">The maximum number of sprite you intend to draw.</param>
		public void BeginSprites( TextureInfo texture_info, ISpriteShader shader, int num_sprites )
		{
			////Common.Profiler.Push("SpriteRenderer.BeginSprites");

			Matrix4 mvp = GL.GetMVP();
			shader.SetMVP( ref mvp );

			GL.Context.SetShaderProgram( shader.GetShaderProgram() );

			Common.Assert( !texture_info.Disposed, "This TextureInfo object has been disposed" );
			GL.Context.SetTexture( 0, texture_info.Texture );

			m_current_texture_info = texture_info;

			m_imm_quads.ImmBeginQuads( (uint)num_sprites );

			////Common.Profiler.Pop();
		}

		/// <summary>Start batch rendering of sprites.</summary>
		/// <param name="texture_info">The texture object.</param>
		/// <param name="num_sprites">The maximum number of sprite you intend to draw.</param>
		public void BeginSprites( TextureInfo texture_info, int num_sprites )
		{
			BeginSprites( texture_info, (ISpriteShader)DefaultShader, num_sprites );
		}

		/// <summary>
		/// End batch rendering of sprites.
		/// </summary>
		public void EndSprites()
		{
			m_imm_quads.ImmEndQuads();

//			GL.Context.SetShaderProgram( null );

			m_current_texture_info = null;
		}

		/// <summary>
		/// Add a sprite to batch rendering of sprites, must be called between BeginSprites and EndSprites.
		/// </summary>
		/// <param name="quad">The sprite geometry.</param>
		/// <param name="tile_index">Sprite UVs are specified by a tile index.</param>
		public void AddSprite( ref TRS quad, Vector2i tile_index )
		{
			Vector2 posX = quad.X;
			Vector2 posY = quad.Y;

			TextureInfo.CachedTileData uvs = m_current_texture_info.GetCachedTiledData( ref tile_index );

			m_v0 = new Vector4( quad.T , uvs.UV_00 );
			m_v1 = new Vector4( quad.T + posX , uvs.UV_10 );
			m_v2 = new Vector4( quad.T + posY , uvs.UV_01 );
			m_v3 = new Vector4( quad.T + posX + posY , uvs.UV_11 );

			add_quad();
		}

		/// <summary>
		/// Add a sprite to batch rendering of sprites, must be called between BeginSprites and EndSprites.
		/// </summary>
		/// <param name="quad">The sprite geometry.</param>
		/// <param name="tile_index">Sprite UVs are specified by a tile index.</param>
		/// <param name="mat">A per sprite transform matrix.</param>
		public void AddSprite( ref TRS quad, Vector2i tile_index, ref Matrix3 mat )
		{
			Vector2 posX = quad.X;
			Vector2 posY = quad.Y;

			TextureInfo.CachedTileData uvs = m_current_texture_info.GetCachedTiledData( ref tile_index );

			m_v0 = new Vector4( transform_point( ref mat, quad.T ), uvs.UV_00 );
			m_v1 = new Vector4( transform_point( ref mat, quad.T + posX ), uvs.UV_10 );
			m_v2 = new Vector4( transform_point( ref mat, quad.T + posY ), uvs.UV_01 );
			m_v3 = new Vector4( transform_point( ref mat, quad.T + posX + posY ), uvs.UV_11 );

			add_quad();
		}

		/// <summary>
		/// Add a sprite to batch rendering of sprites, must be called between BeginSprites and EndSprites.
		/// </summary>
		/// <param name="quad">The sprite geometry.</param>
		/// <param name="uv">Sprite UVs are specified directly using a TRS object.</param>
		public void AddSprite( ref TRS quad, ref TRS uv )
		{
			Vector2 posX = quad.X;
			Vector2 posY = quad.Y;

			Vector2 uvX = uv.X; 
			Vector2 uvY = uv.Y;

			m_v0 = new Vector4( quad.T , uv.T );
			m_v1 = new Vector4( quad.T + posX , uv.T + uvX );
			m_v2 = new Vector4( quad.T + posY , uv.T + uvY );
			m_v3 = new Vector4( quad.T + posX + posY , uv.T + uvX + uvY );

			add_quad();
		}

		/// <summary>
		/// Add a sprite to batch rendering of sprites, must be called between BeginSprites and EndSprites.
		/// </summary>
		/// <param name="quad">The sprite geometry.</param>
		/// <param name="uv">Sprite UVs are specified directly using a TRS object.</param>
		/// <param name="mat">A per sprite transform matrix.</param>
		public void AddSprite( ref TRS quad, ref TRS uv, ref Matrix3 mat )
		{
			Vector2 quadX = quad.X;
			Vector2 quadY = quad.Y;

			Vector2 uvX = uv.X; 
			Vector2 uvY = uv.Y;

			m_v0 = new Vector4( transform_point( ref mat, quad.T ), uv.T );
			m_v1 = new Vector4( transform_point( ref mat, quad.T + quadX ), uv.T + uvX );
			m_v2 = new Vector4( transform_point( ref mat, quad.T + quadY ), uv.T + uvY );
			m_v3 = new Vector4( transform_point( ref mat, quad.T + quadX + quadY ), uv.T + uvX + uvY );

			add_quad();
		}

		/// <summary>
		/// Add a sprite to batch rendering of sprites, must be called between BeginSprites and EndSprites.
		/// One vector is enough to determine the orientation and scale of the sprite. The aspect ratio is
		/// by default the same was the size of the 'uv' domain covered (in texels).
		/// </summary>
		/// <param name="x">The len and direction of the bottom edge of the sprite.</param>
		/// <param name="bottom_left_start_pos">The bottom left point of the sprite.</param>
		/// <param name="uv_bounds">The uv bounds (Bounds2 in uv domain).</param>
		public void AddSprite( Vector2 bottom_left_start_pos, Vector2 x, Bounds2 uv_bounds )
		{
			Vector2 ssize = uv_bounds.Size.Abs() * m_current_texture_info.TextureSizef;	// sprite size in texel units - if bounds is invalid, use uv_bounds.Size.Abs()

			Vector2 y = Math.Perp( x ) * ssize.Y / ssize.X;

			m_v0 = new Vector4( bottom_left_start_pos , uv_bounds.Point00 );
			m_v1 = new Vector4( bottom_left_start_pos + x , uv_bounds.Point10 );
			m_v2 = new Vector4( bottom_left_start_pos + y , uv_bounds.Point01 );
			m_v3 = new Vector4( bottom_left_start_pos + x + y , uv_bounds.Point11 );

			add_quad();
		}

		/// <summary>
		/// Add a sprite to batch rendering of sprites, must be called between BeginSprites and EndSprites.
		/// In this version user specify 4 vertices as Vector4, where each Vector4's xy is the position of 
		/// the vertex, and zw is the UV.
		/// </summary>
		public void AddSprite( Vector4 v0, Vector4 v1, Vector4 v2, Vector4 v3 )
		{
			m_v0 = v0;
			m_v1 = v1;
			m_v2 = v2;
			m_v3 = v3;

			add_quad();
		}

		void add_quad()
		{
			if ( FlipU )
			{
				Common.Swap( ref m_v0.X, ref m_v1.X );
				Common.Swap( ref m_v2.X, ref m_v3.X );
			}

			if ( FlipV )
			{
				Common.Swap( ref m_v0.Y, ref m_v2.Y );
				Common.Swap( ref m_v1.Y, ref m_v3.Y );
			}

			m_imm_quads.ImmAddQuad( m_v0, m_v1, m_v2, m_v3 );
		}

		Vector2 transform_point( ref Matrix3 mat, Vector2 pos )
		{
			return 
				mat.X.Xy * pos.X + 
				mat.Y.Xy * pos.Y + 
				mat.Z.Xy;
		}
	}
}

