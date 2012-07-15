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
	/// A simple particle system effect.
	/// </summary>
	public class ParticleSystem : System.IDisposable
	{
		/// <summary>
		/// Default shader, texture modulated by a color.
		/// </summary>
		public class ParticleShaderDefault : IParticleShader, System.IDisposable
		{
			public ShaderProgram m_shader_program;

			public ParticleShaderDefault()
			{
				m_shader_program = Common.CreateShaderProgram( "particles.cgx" );

				m_shader_program.SetUniformBinding( 0, "MVP" );
				m_shader_program.SetUniformBinding( 1, "Color" );
//				m_shader_program.SetUniformBinding( 2, "WorldScale" );

				m_shader_program.SetAttributeBinding( 0, "vin_data" );
				m_shader_program.SetAttributeBinding( 1, "vin_color" );

				Matrix4 identity = Matrix4.Identity;
				SetMVP( ref identity );
				SetColor( ref Colors.White );
//				SetWorldScale( ref );
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
//			public void SetWorldScale( ref float value ) { m_shader_program.SetUniformValue( 2, ref value );}
		}

		/// <summary>
		/// Particle object returned by CreateParticle.
		/// </summary>
		public class Particle
		{
			/// <summary>
			/// Position.
			/// </summary>
			public Vector2 Position;
			/// <summary>
			/// Velocity.
			/// </summary>
			public Vector2 Velocity;
			/// <summary>
			/// Age (particle dies when Age >= LifeSpan).
			/// </summary>
			public float Age;
			/// <summary>
			/// Life span
			/// </summary>
			public float LifeSpan;
			/// <summary>
			/// Life span reciprocal - if you don't use the automatic init, you must make sure to set this to 1.0f/LifeSpan
			/// </summary>
			public float LifeSpanRcp;
			/// <summary>
			/// Rotation angle
			/// </summary>
			public float Angle;	// Vector2?
			/// <summary>
			/// Angular velocity
			/// </summary>
			public float AngularVelocity; // Vector2?
			/// <summary>
			/// Scale when Age=0.0
			/// </summary>
			public float ScaleStart;
			/// <summary>
			/// ScaleDelta must be initialized to ( scale_end - ScaleStart ) / LifeSpan
			/// </summary>
			public float ScaleDelta;
			/// <summary>
			/// Color when Age=0.0
			/// </summary>
			public Vector4 ColorStart;
			/// <summary>
			/// ColorDelta must be initialized to ( color_end - ColorStart ) / LifeSpan
			/// </summary>
			public Vector4 ColorDelta;
			/// <summary>
			/// Scale
			/// </summary>
			public float Scale { get { return ScaleStart + ScaleDelta * Age;}}
			/// <summary>
			/// Color
			/// </summary>
			public Vector4 Color { get { return ColorStart + ColorDelta * Age;}}
			/// <summary>
			/// Check if the particle is still alive
			/// </summary>
			public bool Dead { get { return Age >= LifeSpan;}}

			/// <summary>Return the string representation of this Particle.</summary>
			public override string ToString() 
			{
				return "Position=" + Position + " Velocity=" + Velocity + " Age=" + Age + " LifeSpan=" + LifeSpan + " Scale=" + Scale + " Color=" + Color;
			}
		};

		// particle data for the VertexBuffer
		struct Vertex
		{
			public Vector4 XYUV;
			public Vector4 Color;

			public Vertex( Vector4 a_data1, Vector4 a_data2 )
			{
				XYUV = a_data1;
				Color = a_data2;
			}
		}

		/// <summary>ParticleSystem's shader interface.</summary>
		public interface IParticleShader
		{
			/// <summary>Set the Projection*View*Model matrix.</summary>
			void SetMVP( ref Matrix4 value );
			/// <summary>Set a global color.</summary>
			void SetColor( ref Vector4 value );
			/// <summary>Return the shader's ShaderProgram object.</summary>
			ShaderProgram GetShaderProgram();
		}

		/// <summary>
		/// This class regroups all the parameters needed to initialize a particle.
		/// Most values have a variance associated to them (-Var suffix), to give a 
		/// randomization range. When the variance is a "relative" value (-RelVar suffix), 
		/// a value between 0,1 is expected. For example: 0.2f means the corresponding 
		/// value will be randomized -+20% at creation time.
		/// </summary>
		public class EmitterParams
		{
			/// <summary>
			/// The generated position, velocity, angle are transformed by this.
			/// </summary>
			public Matrix3 Transform = Matrix3.Identity;
			/// <summary>
			/// The transform matrix we use to estimate a velocity and an angular velocity.
			/// You probably want to set it everyframe to something relevant (the current 
			/// transform of the object node for example).
			/// </summary>
			public Matrix3 TransformForVelocityEstimate = Matrix3.Identity;
			/// <summary>
			/// Control how much of the "observed velocity" we add to the created particles.
			/// </summary>
			public float ForwardMomentum = 0.0f;
			/// <summary>
			/// Control how much of the "observed angular velocity" we add to the created particles.
			/// </summary>
			public float AngularMomentun = 0.0f;
			/// <summary>
			/// The time to wait until the next particle gets created, in seconds.
			/// </summary>
			public float WaitTime = 1.0f;
			/// <summary>
			/// WaitTime's variance (relative)
			/// </summary>
			public float WaitTimeRelVar = 0.15f;
			/// <summary>
			/// Created particles's life span in seconds.
			/// </summary>
			public float LifeSpan = 5.0f;
			/// <summary>
			/// LifeSpan's variance (relative)
			/// </summary>
			public float LifeSpanRelVar;
			/// <summary>
			/// Created particles's initial position (see also InLocalSpace)
			/// </summary>
			public Vector2 Position = GameEngine2D.Base.Math._00;
			/// <summary>
			/// Position's variance
			/// </summary>
			public Vector2 PositionVar = GameEngine2D.Base.Math._11 * 1.5f;
			/// <summary>
			/// Created particles's initial velocity given to created particles (see also InLocalSpace) 
			/// </summary>
			public Vector2 Velocity = GameEngine2D.Base.Math._01;
			/// <summary>
			/// Velocity's variance
			/// </summary>
			public Vector2 VelocityVar = GameEngine2D.Base.Math._11 * 0.2f;
			/// <summary>
			/// Created particles's initial angular velocity, in radians (see also InLocalSpace) 
			/// </summary>
			public float AngularVelocity;
			/// <summary>
			/// AngularVelocity's variance
			/// </summary>
			public float AngularVelocityVar;
			/// <summary>
			/// Created particles's initial rotation angle in radians (see also InLocalSpace) 
			/// </summary>
			public float Angle;
			/// <summary>
			/// Angle's variance
			/// </summary>
			public float AngleVar;
			/// <summary>
			/// Created particles's initial color
			/// </summary>
			public Vector4 ColorStart = Colors.White;
			/// <summary>
			/// ColorStart's variance
			/// </summary>
			public Vector4 ColorStartVar = GameEngine2D.Base.Math._0000;
			/// <summary>
			/// Color the particle will have when they reach their life span
			/// </summary>
			public Vector4 ColorEnd = Colors.White;
			/// <summary>
			/// ColorEnd's variance
			/// </summary>
			public Vector4 ColorEndVar = GameEngine2D.Base.Math._0000;
			/// <summary>
			/// Created particles's initial size 
			/// </summary>
			public float ScaleStart = 1.0f;
			/// <summary>
			/// ScaleStart's variance (relative)
			/// </summary>
			public float ScaleStartRelVar;
			/// <summary>
			/// Size the particle will have when they reach their life span
			/// </summary>
			public float ScaleEnd = 1.0f;
			/// <summary>
			/// ScaleEnd's variance (relative)
			/// </summary>
			public float ScaleEndRelVar;

			/// <summary>EmitterParams constructor.</summary>
			public EmitterParams()
			{
			}

			/// <summary>Return the string representation of this EmitterParams.</summary>
			/// <param name="prefix">A prefix string added to the beginning of each line.</param> 
			public string ToString( string prefix ) 
			{
				return 
				prefix + "WaitTime      " + WaitTime      + "\n" +
				prefix + "WaitTimeVar    " + WaitTimeRelVar   + "\n" +
				prefix + "Position      " + Position      + "\n" +
				prefix + "PositionVar    " + PositionVar    + "\n" +
				prefix + "Velocity      " + Velocity      + "\n" +
				prefix + "VelocityVar    " + VelocityVar    + "\n" +
				prefix + "AngularVelocity  " + AngularVelocity  + "\n" +
				prefix + "AngularVelocityVar " + AngularVelocityVar + "\n" +
				prefix + "Angle       " + Angle       + "\n" +
				prefix + "AngleVar      " + AngleVar      + "\n" +
				prefix + "ColorStart     " + ColorStart     + "\n" +
				prefix + "ColorStartVar   " + ColorStartVar   + "\n" +
				prefix + "ColorEnd      " + ColorEnd      + "\n" +
				prefix + "ColorEndVar    " + ColorEndVar    + "\n" +
				prefix + "ScaleStart     " + ScaleStart     + "\n" +
				prefix + "ScaleStartRelVar  " + ScaleStartRelVar  + "\n" +
				prefix + "ScaleEnd      " + ScaleEnd      + "\n" +
				prefix + "ScaleEndRelVar   " + ScaleEndRelVar   + "\n" +
				prefix + "LifeSpan      " + LifeSpan      + "\n" +
				prefix + "LifeSpanVar    " + LifeSpanRelVar   + "\n" ;
			}
		}

		/// <summary>
		/// SimulationParams regroups all the parameters needed by the Update and Draw 
		/// to advance and render the particles.
		/// </summary>
		public class SimulationParams
		{
			/// <summary>
			/// A global friction applied to all particles.
			/// </summary>
			public float Friction = 0.99f;
			/// <summary>
			/// Gravity direction (unit vector).
			/// </summary>
			public Vector2 GravityDirection = -GameEngine2D.Base.Math._01;
			/// <summary>
			/// Gravity amount.
			/// </summary>
			public float Gravity = 0.8f;
			/// <summary>
			/// Wind direction (unit vector).
			/// </summary>
			public Vector2 WindDirection;
			/// <summary>
			/// Amount of wind.
			/// </summary>
			public float Wind;
			/// <summary>
			/// Amount of brownian motion you want to add to each particle.
			/// For each particle, a different random direction is added to 
			/// the wind everyframe. You can use that to add a touch of gaz/dust 
			/// effects.
			/// </summary>
			public float BrownianScale = 5.0f;
			/// <summary>
			/// The Fade value is a value in 0,1 that controls how fast you want the 
			/// particle to fade in/fade out (when it dies)
			/// The particle's color's alpha value is multiplied by a symmetric fade
			/// curve; Fade is the lenght of the start and end fade areas, for example 
			/// if Fade is 0.25, the particle will be fully visible at 25% of its age.
			/// </summary>
			public float Fade = 0.1f;
		}

		/// <summary>
		/// The texture information for this particle system. All particles use the same image.
		/// </summary>
		public TextureInfo TextureInfo;
		/// <summary>
		/// A global multiplier color for the particle system.
		/// </summary>
		public Vector4 Color = Colors.White;
		/// <summary>
		/// The blend mode used by the particle system.
		/// </summary>
		public BlendMode BlendMode = BlendMode.Normal;
		/// <summary>
		/// Parameters used for controlling the emission and initial parameters of particles.
		/// </summary>
		public EmitterParams Emit;
		/// <summary>
		/// Parameters used for controlling the simulation.
		/// </summary>
		public SimulationParams Simulation;
		/// <summary>
		/// The modelview matrix used for rendering the particle system.
		/// RenderTransform overrides base class's Node.GetTransform() to render the local geometry.
		/// </summary>
		public Matrix3 RenderTransform = Matrix3.Identity;
		/// <summary>
		/// The maximum number of particles the system can deal with.
		/// </summary>
		public int MaxParticles { get { return m_particles.Length;}}
		/// <summary>
		/// The current number of particles alive.
		/// </summary>
		public int ParticlesCount { get { return m_particles_count;}}
		/// <summary>
		/// Return true if the number of active particles has reached maximum capacity.
		/// </summary>
		public bool IsFull { get { return MaxParticles == m_particles_count;}}
		/// particle system's default shader (texture * color).
		static ParticleShaderDefault m_default_shader;
		/// <summary>
		/// The particle system's default shader (texture * color).
		/// </summary>
		static public ParticleShaderDefault DefaultShader 
		{ 
			get 
			{
				if ( m_default_shader == null )
					m_default_shader = new ParticleShaderDefault();
				return m_default_shader;
			}
		}
		/// <summary>
		/// User can set an external shader. 
		/// The Label class won't dispose of shaders set by user (other than ParticleSystem.DefaultShader).
		/// </summary>
		public IParticleShader Shader;

		bool m_disposed = false;
		/// <summary>Return true if this object been disposed.</summary>
		public bool Disposed { get { return m_disposed; } }

		ImmediateModeQuads< Vertex > m_imm_quads;
		double m_elapsed;
		float m_emit_timer;
		Vector2 m_observed_velocity;
		Matrix3 m_tracking_transform_prev = Matrix3.Identity;
		float m_observed_angular_velocity;
		GameEngine2D.Base.Math.RandGenerator m_random;
		Vertex m_v0;
		Vertex m_v1;
		Vertex m_v2;
		Vertex m_v3;
		int m_particles_count;
		Particle[] m_particles;
		GraphicsContextAlpha GL;

		/// <summary>
		/// ParticleSystem constructor.
		/// </summary>
		/// <param name="max_particles">The maximum number of particles for this particle system.</param>
		public ParticleSystem( int max_particles )
		{
			GL = Director.Instance.GL;
			m_imm_quads = new ImmediateModeQuads< Vertex >( GL, (uint)max_particles, VertexFormat.Float4, VertexFormat.Float4 );
			m_particles = new Particle[ max_particles ];
			for ( int i=0; i < m_particles.Length; ++i )
				m_particles[i] = new Particle();
			m_random = new GameEngine2D.Base.Math.RandGenerator();
			Shader = DefaultShader;
			Emit = new EmitterParams();
			Simulation = new SimulationParams();

			m_v0.XYUV.Zw = GameEngine2D.Base.Math._00;
			m_v1.XYUV.Zw = GameEngine2D.Base.Math._10;
			m_v2.XYUV.Zw = GameEngine2D.Base.Math._01;
			m_v3.XYUV.Zw = GameEngine2D.Base.Math._11;
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
//				System.Console.WriteLine( "ParticleSystem Dispose() called!" );
				m_imm_quads.Dispose();
				m_disposed = true;
			}
		}

		/// <summary>Dispose of static resources.</summary>
		static public void Terminate()
		{
			Common.DisposeAndNullify< ParticleShaderDefault >( ref m_default_shader );
		}

		// initialize a particle that has just been created
		void init_auto_particle( Particle p )
		{
			p.LifeSpan = FMath.Max( 0.0f, Emit.LifeSpan * ( 1.0f + Emit.LifeSpanRelVar * m_random.NextFloatMinus1_1() ) );
			p.Age = 0.0f;
			p.LifeSpanRcp = 1.0f / p.LifeSpan;

			p.Position = Emit.Position + Emit.PositionVar * m_random.NextVector2( -1.0f, 1.0f );
			p.Position = ( Emit.Transform * p.Position.Xy1 ).Xy;

			p.Velocity = Emit.Velocity + Emit.VelocityVar * m_random.NextVector2( -1.0f, 1.0f );
			p.Velocity = ( Emit.Transform * p.Velocity.Xy0 ).Xy;

			p.Angle = FMath.Max( 0.0f, Emit.Angle + Emit.AngleVar * m_random.NextFloatMinus1_1() );
			p.Angle += GameEngine2D.Base.Math.Angle( Emit.Transform.X.Xy.Normalize() );

			p.AngularVelocity = FMath.Max( 0.0f, Emit.AngularVelocity + Emit.AngularVelocityVar * m_random.NextFloatMinus1_1() );

			p.Velocity += Emit.ForwardMomentum * m_observed_velocity;
			p.AngularVelocity += Emit.AngularMomentun * m_observed_angular_velocity;

			p.ScaleStart = FMath.Max( 0.0f, Emit.ScaleStart * ( 1.0f + Emit.ScaleStartRelVar * m_random.NextFloatMinus1_1() ) );
			float scale_end = FMath.Max( 0.0f, Emit.ScaleEnd * ( 1.0f + Emit.ScaleEndRelVar * m_random.NextFloatMinus1_1() ) );
			p.ScaleDelta = ( scale_end - p.ScaleStart ) / p.LifeSpan;

			p.ColorStart = ( Emit.ColorStart + Emit.ColorStartVar * m_random.NextFloatMinus1_1() ).Clamp( 0.0f, 1.0f );
			Vector4 color_end = ( Emit.ColorEnd + Emit.ColorEndVar * m_random.NextFloatMinus1_1() ).Clamp( 0.0f, 1.0f );
			p.ColorDelta = ( color_end - p.ColorStart ) / p.LifeSpan;
		}

		void update( Particle p, float dt, Vector2 forces ) 
		{
			p.Velocity += ( forces - p.Velocity * Simulation.Friction ) * dt;
			p.Position += p.Velocity * dt;
			p.Angle += p.AngularVelocity * dt;
			p.Age += dt;
		}

		/// <summary>
		/// Particles get created automatically using Emit parameters,
		/// but if needed you can also create and init particles yourself.
		/// </summary>
		/// <param name="skip_auto_init">
		/// If false, the created particle gets initialized using the Emit parameters, just 
		/// like the particles randomly created everyframe (this is in case you want to use 
		/// a random variation as a starting point).
		/// </param>
		/// <returns>A handle to the particle just created</returns>
		public Particle CreateParticle( bool skip_auto_init = false )
		{
			if ( IsFull ) 
				return null;
			
			if ( !skip_auto_init ) 
				init_auto_particle( m_particles[ m_particles_count ] );

			++m_particles_count;

			return m_particles[ m_particles_count - 1 ];
		}

		/// <summary>The update function.</summary>
		public void Update( float dt )
		{
			m_observed_velocity = ( Emit.TransformForVelocityEstimate.Z - m_tracking_transform_prev.Z ).Xy / dt;
			m_observed_angular_velocity = ( m_tracking_transform_prev.X.Xy.Normalize().Angle( Emit.TransformForVelocityEstimate.X.Xy.Normalize() ) ) / dt;
			m_tracking_transform_prev = Emit.TransformForVelocityEstimate;

//			System.Console.WriteLine( m_observed_velocity );
//			System.Console.WriteLine( Emit.TransformForVelocityEstimate );

			m_elapsed += (double)dt;

			float emit_wait_time = FMath.Max( 0.0f, Emit.WaitTime * ( 1.0f + Emit.WaitTimeRelVar * m_random.NextFloatMinus1_1() ) );

			m_emit_timer += dt;
			while ( m_emit_timer > emit_wait_time && !IsFull )
			{
				init_auto_particle( m_particles[ m_particles_count ] );
				++m_particles_count;
				m_emit_timer -= emit_wait_time;

				// generate a new wait time
				emit_wait_time = FMath.Max( 0.0f, Emit.WaitTime * ( 1.0f + Emit.WaitTimeRelVar * m_random.NextFloatMinus1_1() ) );
			}

			Vector2 global_forces = Simulation.Gravity * Simulation.GravityDirection + Simulation.Wind * Simulation.WindDirection;

			for ( int i=0; i < m_particles_count; )
			{
				if ( m_particles[i].Dead )
				{
					// this will shuffle the draw order but reduces the number of copies
					Common.Swap( ref m_particles[i], ref m_particles[m_particles_count-1] );
					--m_particles_count;
				}
				else
				{
					Vector2 forces = global_forces;
					if ( Simulation.BrownianScale != 0.0f )
						forces += m_random.NextVector2Minus1_1() * Simulation.BrownianScale;

					update( m_particles[i], dt, forces );
					++i;
				}
			}

//			Dump();
		}

		// x is the particle age normalized to 0,1
		// dx is the len of the linear fade in/out zone at the beginning and the end (the curve is symmetric)
		// particle's Color alpha gets multiplied by age_to_alpha
		float age_to_alpha( float x, float dx )
		{
			x = FMath.Abs( x - 0.5f );
			float mi = 0.5f - dx;
			return 1.0f - FMath.Max( 0.0f, ( x - mi ) * ( 1.0f / dx ) );
		}

		/// <summary>The draw function.</summary>
		public void Draw()
		{
			////Common.Profiler.Push("ParticleSystem.BeginParticles");
		
			GL.SetDepthMask( false );

			GL.ModelMatrix.Push();
			GL.ModelMatrix.Set( RenderTransform.Matrix4() );

			Matrix4 mvp = GL.GetMVP();

			Shader.SetMVP( ref mvp );
			Shader.SetColor( ref Color );

			GL.SetBlendMode( BlendMode );

			Common.Assert( TextureInfo != null, "TextureInfo has not been set." );

			GL.Context.SetShaderProgram( Shader.GetShaderProgram() );
			GL.Context.SetTexture( 0, TextureInfo.Texture );

			m_imm_quads.ImmBeginQuads( (uint)m_particles_count );

			for ( int i=0; i<m_particles_count; ++i )
			{
				Vector2 x = Vector2.Rotation( m_particles[i].Angle ) * m_particles[i].Scale;
				Vector2 y = GameEngine2D.Base.Math.Perp( x );
				Vector2 p = m_particles[i].Position - ( x + y ) * 0.5f;
				Vector4 color = m_particles[i].Color;

				color.W *= age_to_alpha( m_particles[i].Age * m_particles[i].LifeSpanRcp, Simulation.Fade );

				m_v0.XYUV.Xy = p; m_v0.Color = color;
				m_v1.XYUV.Xy = p + x; m_v1.Color = color;
				m_v2.XYUV.Xy = p + y; m_v2.Color = color;
				m_v3.XYUV.Xy = p + x + y; m_v3.Color = color;

				m_imm_quads.ImmAddQuad( m_v0, m_v1, m_v2, m_v3 );
			}

			m_imm_quads.ImmEndQuads();

			////Common.Profiler.Pop();

			GL.SetDepthMask( true );

			GL.ModelMatrix.Pop();
		}

		void Dump()
		{
			string prefix = Common.FrameCount + " ";
			System.Console.WriteLine( prefix + "ParticlesCount " + ParticlesCount + "/" + MaxParticles );
			System.Console.WriteLine( prefix + "Emit" + Emit.ToString( prefix ) );
			for ( int i=0; i < m_particles_count; ++i )
				System.Console.WriteLine( prefix + "[" +i +"]"+ " " +m_particles[i] );
		}
	}

	/// <summary>
	/// Particles wraps a single ParticleSystem into a scenegraph node.
	/// </summary>
	public class Particles : Node, System.IDisposable 
	{
		/// <summary>
		/// The actual ParticleSystem object used by this Particles node.
		/// </summary>
		public ParticleSystem ParticleSystem;

		/// <summary>
		/// ParticleSystem constructor.
		/// Note that the constructor calls ScheduleUpdate().
		/// </summary>
		/// <param name="max_particles">The maximum number of particles for this particle system.</param>
		public Particles( int max_particles )
		{
			ParticleSystem = new ParticleSystem( max_particles );
			ScheduleUpdate();
		}

		/// <summary>The update function.</summary>
		public override void Update( float dt )
		{
			base.Update( dt );
			ParticleSystem.Update( dt );
		}

		/// <summary>The draw function.</summary>
		public override void Draw()
		{
			base.Draw();
			ParticleSystem.Draw();
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
				Common.DisposeAndNullify< ParticleSystem >( ref ParticleSystem );
			}
		}
	}

} // namespace Sce.PlayStation.HighLevel.GameEngine2D.Base

