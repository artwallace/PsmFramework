/* SCE CONFIDENTIAL
 * PlayStation(R)Suite SDK 0.98.2
 * Copyright (C) 2012 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

namespace Sce.PlayStation.HighLevel.GameEngine2D
{
	/// <summary>
	/// Base class for scenes transition nodes. Those nodes make a visual
	/// transition between 2 given scenes, the current one and the next
	/// one. During the transition, both scenes are potentially updated
	/// and rendered at the same time.
	/// </summary>
	public class TransitionScene : Scene
	{
		/// <summary>The previous scene we are transitioning from.</summary>
		public Scene PreviousScene;
		/// <summary>The scene we are transitioning to.</summary>
		public Scene NextScene;
		/// <summary>Transition duration in seconds.</summary>
		public float Duration = 0.0f;

		// remember the actions that calls ReplaceScene so that we can cancel it if needed
		Sequence m_seq = null;

		/// <summary>If true, keep updating both scenes during the transition (more expensive).</summary>
		public bool KeepRendering = true; // we could allow separate control for previous and next

		protected uint m_render_count = 0;

		/// <summary>Fade completion, [0,1] range.</summary>
		public float FadeCompletion { 
			get {
				return FMath.Clamp( (float)SceneTime / Duration, 0.0f, 1.0f );
			}
		}

		/// <summary>Returns true for all scene deriving from TransitionScene.</summary>
		public override bool IsTransitionScene()
		{
			return true;
		}

		/// <summary></summary>
		public TransitionScene( Scene next_scene ) 
		{
			PreviousScene = null; // will be set in on_scene_change
			NextScene = next_scene;
		}

		/// <summary></summary>
		public override void OnEnter()
		{
			base.OnEnter();

			m_seq = new Sequence();

			m_seq.Add( new DelayTime( this.Duration ) );
			m_seq.Add( new CallFunc( () =>
				{
//					#if DEBUG_SCENE_TRANSITIONS
//					System.Console.WriteLine( Common.FrameCount + " registered in OnEnter of node " + DebugInfo() + " -> " + NextScene.DebugInfo() + "(next frame)" );
//					#endif // #if DEBUG_SCENE_TRANSITIONS
					Director.Instance.ReplaceScene( NextScene );
				}
			) );

			ActionManager.Instance.AddAction( m_seq, this/*, false*/ );

			m_seq.Run();
		}

		// used by Director when this TransitionScene was interrupted/overridden by an other one
		internal void cancel_replace_scene()
		{
			StopAction( m_seq );
			m_seq = null;
		}
	}

	/// <summary>
	/// Base class for scenes transition doing a fade of some kind,
	/// requiring offscreen rendering for both scenes.
	/// </summary>
	public class TransitionFadeBase : TransitionScene
	{
		static bool m_graphics_resources_init = false;
		static FrameBuffer m_fbuf1;
		static FrameBuffer m_fbuf2;
		static uint m_last_update_scenes_render = unchecked((uint)-1);
		static protected TextureInfo m_previous_scene_render;
		static protected TextureInfo m_next_scene_render;

		/// <summary></summary>
		public TransitionFadeBase( Scene next_scene )
		: base( next_scene )
		{
			Vector2i size = new Vector2i( Director.Instance.GL.Context.GetViewport().Width,
										 Director.Instance.GL.Context.GetViewport().Height );

			if ( !m_graphics_resources_init )
			{
				m_previous_scene_render = new TextureInfo( new Texture2D( size.X, size.Y, false, PixelFormat.Rgba, PixelBufferOption.Renderable ) );
				m_next_scene_render = new TextureInfo( new Texture2D( size.X, size.Y, false, PixelFormat.Rgba, PixelBufferOption.Renderable ) );

				m_fbuf1 = new FrameBuffer();
				m_fbuf2 = new FrameBuffer();

				m_fbuf1.SetColorTarget( m_previous_scene_render.Texture, 0 );
				m_fbuf2.SetColorTarget( m_next_scene_render.Texture, 0 );

				m_graphics_resources_init = true;
			}
			else
			{
				Common.Assert( m_previous_scene_render.TextureSizei == size );
				Common.Assert( m_next_scene_render.TextureSizei == size );
			}
		}

		/// <summary>Dispose of static resources.</summary>
		static public void Terminate()
		{
			Common.DisposeAndNullify< FrameBuffer >( ref m_fbuf1 );
			Common.DisposeAndNullify< FrameBuffer >( ref m_fbuf2 );
			Common.DisposeAndNullify< TextureInfo >( ref m_previous_scene_render );
			Common.DisposeAndNullify< TextureInfo >( ref m_next_scene_render );
		}

		protected void update_scenes_render()
		{
			++m_render_count;

			if ( m_last_update_scenes_render == Common.FrameCount )
				return;	// prevent from calling more than once a frame

			m_last_update_scenes_render = Common.FrameCount;

			Director.Instance.GL.Context.SetFrameBuffer( m_fbuf1 );
			PreviousScene.render();

			// Note: we could delay NextScene capture one frame later, 
			// since result isn't used on first frame anyway 

			Director.Instance.GL.Context.SetFrameBuffer( m_fbuf2 );
			NextScene.render();

			Director.Instance.GL.Context.SetFrameBuffer( null );
		}

		/// <summary></summary>
		public override void OnEnter()
		{
			base.OnEnter();

//			update_scenes_render();
		}

		/// <summary>The draw function.</summary>
		public override void Draw()
		{
			if ( KeepRendering || m_render_count == 0 )
				update_scenes_render();
		}
	}

	public delegate float DTween( float t );

	/// <summary>
	/// TransitionSolidFade fades the current scene to black before fading the
	/// next scene in.
	/// </summary>
	public class TransitionSolidFade : TransitionFadeBase
	{
//		public Vector4 Color = Colors.Black; // fixme: need lerp shader, not just mul

		/// <summary></summary>
		public DTween Tween = ( x ) => GameEngine2D.Base.Math.PowEaseInOut( x, 4.0f );

		/// <summary></summary>
		public TransitionSolidFade( Scene next_scene )
		: base( next_scene )
		{
		}

		/// <summary>The draw function.</summary>
		public override void Draw()
		{
			base.Draw();

			TRS pos = new TRS( Director.Instance.CurrentScene.Camera.CalcBounds() );
			TRS uv = TRS.Quad0_1;
			Director.Instance.SpriteRenderer.DefaultShader.SetUVTransform( ref GameEngine2D.Base.Math.UV_TransformIdentity );

			Director.Instance.GL.SetBlendMode( BlendMode.None);

			if ( FadeCompletion < 0.5f )
			{
				float alpha = Tween( FadeCompletion / 0.5f );
				Vector4 color = new Vector4( 1.0f - alpha );
				Director.Instance.SpriteRenderer.DefaultShader.SetColor( ref color );
				Director.Instance.SpriteRenderer.BeginSprites( m_previous_scene_render, 1 );
				Director.Instance.SpriteRenderer.AddSprite( ref pos, ref uv );
				Director.Instance.SpriteRenderer.EndSprites(); 
			}
			else
			{
				float alpha = Tween( ( FadeCompletion - 0.5f ) / 0.5f );
				Vector4 color = new Vector4( alpha );
				Director.Instance.SpriteRenderer.DefaultShader.SetColor( ref color );
				Director.Instance.SpriteRenderer.BeginSprites( m_next_scene_render, 1 );
				Director.Instance.SpriteRenderer.AddSprite( ref pos, ref uv );
				Director.Instance.SpriteRenderer.EndSprites(); 
			}
		}
	}

	/// <summary>
	/// TransitionCrossFade uses a blend to cross fades the 2 transitioning scenes.
	/// You can set a DTween to control the fade.
	/// </summary>
	public class TransitionCrossFade
	: TransitionFadeBase
	{
		/// <summary></summary>
		public DTween Tween = ( x ) => GameEngine2D.Base.Math.PowEaseOut( x, 4.0f ); // calpha blend curve used for the cross fade

		/// <summary></summary>
		public TransitionCrossFade( Scene next_scene )
		: base( next_scene )
		{
		}

		/// <summary>The draw function.</summary>
		public override void Draw()
		{
			base.Draw();

			float alpha = Tween( FadeCompletion );

			TRS pos = new TRS( Director.Instance.CurrentScene.Camera.CalcBounds() );
			TRS uv = TRS.Quad0_1;
			Director.Instance.SpriteRenderer.DefaultShader.SetUVTransform( ref GameEngine2D.Base.Math.UV_TransformIdentity );

			{
				Director.Instance.GL.SetBlendMode( BlendMode.None);
				Vector4 color = new Vector4( 1.0f - alpha );
				Director.Instance.SpriteRenderer.DefaultShader.SetColor( ref color );
				Director.Instance.SpriteRenderer.BeginSprites( m_previous_scene_render, 1 );
				Director.Instance.SpriteRenderer.AddSprite( ref pos, ref uv );
				Director.Instance.SpriteRenderer.EndSprites(); 
			}

			{
				Director.Instance.GL.SetBlendMode( BlendMode.Additive );
				Vector4 color = new Vector4( alpha );
				Director.Instance.SpriteRenderer.DefaultShader.SetColor( ref color );
				Director.Instance.SpriteRenderer.BeginSprites( m_next_scene_render, 1 );
				Director.Instance.SpriteRenderer.AddSprite( ref pos, ref uv );
				Director.Instance.SpriteRenderer.EndSprites(); 
			}
		}
	}

	/// <summary>
	/// TransitionDirectionalFade uses a translating blend mask to cross fade between 2
	/// scenes. The old gets replaced by the new one as the cross fade line moves
	/// along a direction specified by user.
	/// </summary>
	public class TransitionDirectionalFade : TransitionFadeBase
	{
		/// <summary></summary>
		public float Width = 0.75f;	// width of the transition zone (normalized value, roughly 0,1)
		/// <summary></summary>
		public Vector2 Direction = GameEngine2D.Base.Math._10; // moving direction
		/// <summary></summary>
		public DTween Tween = ( x ) => GameEngine2D.Base.Math.PowEaseOut( x, 4.0f );

		public class SpriteShaderDirFade : ISpriteShader, System.IDisposable
		{
			public ShaderProgram m_shader_program;

			public SpriteShaderDirFade()
			{
				m_shader_program = Common.CreateShaderProgram("sprite_directional_fade.cgx");

				m_shader_program.SetUniformBinding( 0, "MVP" );
				m_shader_program.SetUniformBinding( 1, "UVTransform" );
				m_shader_program.SetUniformBinding( 2, "Plane" );
				m_shader_program.SetUniformBinding( 3, "OffsetRcp" );

				m_shader_program.SetAttributeBinding( 0, "vin_data" );

				Matrix4 identity = Matrix4.Identity;
				SetMVP( ref identity );
				SetUVTransform( ref GameEngine2D.Base.Math._0011 );
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
			public void SetUVTransform( ref Vector4 value ) { m_shader_program.SetUniformValue( 1, ref value );}
			public void SetPlane( ref Vector4 value ) { m_shader_program.SetUniformValue( 2, ref value );}
			public void SetOffsetRcp( float value ) { m_shader_program.SetUniformValue( 3, value );}
			public void SetColor( ref Vector4 value ) {} // dummy for public SpriteRenderer.ISpriteShader
		}

		static SpriteShaderDirFade m_shader;

		/// <summary></summary>
		public TransitionDirectionalFade( Scene next_scene )
		: base( next_scene )
		{
			if ( m_shader == null )
				m_shader = new SpriteShaderDirFade();
		}

		/// <summary>Dispose of static resources.</summary>
		static new public void Terminate()
		{
			Common.DisposeAndNullify< SpriteShaderDirFade >( ref m_shader );
		}

		/// <summary>The draw function.</summary>
		public override void Draw()
		{
			base.Draw();

			float alpha = Tween( FadeCompletion );

			TRS pos = new TRS( Director.Instance.CurrentScene.Camera.CalcBounds() );
			TRS uv = TRS.Quad0_1;
			m_shader.SetUVTransform( ref GameEngine2D.Base.Math.UV_TransformIdentity );

			Director.Instance.GL.Context.SetTexture( 1, m_next_scene_render.Texture );

			{
				Director.Instance.GL.SetBlendMode( BlendMode.None);

				Vector2 plane_normal = -Direction.Normalize();
				Vector2 plane_start = new Vector2( 0.0f, 0.0f );
				Vector2 plane_end = new Vector2( 1.0f, 1.0f );

				if ( plane_normal.X > 0.0f )
				{
					plane_start.X = 1.0f; 
					plane_end.X = 0.0f;
				}

				if ( plane_normal.Y > 0.0f )
				{
					plane_start.Y = 1.0f; 
					plane_end.Y = 0.0f;
				}

				plane_end -= plane_normal * Width;

				Vector4 plane = new Vector4( GameEngine2D.Base.Math.Lerp( plane_start, plane_end, alpha ), plane_normal );
				m_shader.SetPlane( ref plane );
				m_shader.SetOffsetRcp( 1.0f / Width );
				Director.Instance.SpriteRenderer.BeginSprites( m_previous_scene_render, (ISpriteShader)m_shader, 1 );
				Director.Instance.SpriteRenderer.AddSprite( ref pos, ref uv );
				Director.Instance.SpriteRenderer.EndSprites();
			}
		}
	}

} // namespace Sce.PlayStation.HighLevel.GameEngine2D

