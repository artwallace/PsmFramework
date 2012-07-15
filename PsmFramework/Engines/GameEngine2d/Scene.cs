/* SCE CONFIDENTIAL
 * PlayStation(R)Suite SDK 0.98.2
 * Copyright (C) 2012 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using Sce.PlayStation.Core;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

namespace Sce.PlayStation.HighLevel.GameEngine2D
{
	/// <summary>Scene is the type of a root node in the scene graph. You can use the 
	/// Director singleton to manipulate the scene stack, can the current scene can be 
	/// acessed with Director.Instance.CurrentScene. Only one scene is active at a time, except 
	/// during scene transition when 2 scenes might be active at the same time to 
	/// allow blending.
	/// 
	/// Scene also holds the main Camera.
	/// </summary>
	public class Scene : Node
	{
		internal double m_elapsed = 0.0;

		/// <summary>
		/// The total time elapse since the scene started, in seconds.
		/// </summary>
		public double SceneTime { get { return m_elapsed; } }

		/// <summary>
		/// Normally, the scene clears the screen in its internal render function.
		/// You can suppress this automatic clear by setting NoClear to true.
		/// Note that if you disable the automatic scene clear, some TransitionScene effects won't work properly
		/// (since some scene transitions need to render the 2 transitioned scenes offscreen to blend them).
		/// </summary>
		public bool NoClear = false;

		/// <summary>
		/// Cell size of the debug grid drawn when DebugFlags.DrawGrid is set in Director.Instance.DebugFlags.
		/// When the value is -1.0f (the default) the debug grid cell size is automatically ajusted to never 
		/// show more than DrawGridAutoStepMaxCells subdivisions along the longest axis viewed.
		/// </summary>
		public float DrawGridStep = -1.0f;

		/// <summary>
		/// When DrawGridStep is set to -1, the debug grid cell size is calculated automatically.
		/// The cell size increases in powers of 2 as you zoom out, in a way there are never more 
		/// than DrawGridAutoStepMaxCells subdivisions along the longest axis viewed. 
		/// Warning: increasing this value too much might cause too many lines to be drawn, and 
		/// overflow the vertex array used in DrawHelpers' immediate mode. 
		/// The debug grid stops being drawn if the automatically calculated step size is bigger than 2^15.
		/// </summary>
		public float DrawGridAutoStepMaxCells = 32;

		/// <summary>
		/// Scene constructor.
		/// </summary>
		public Scene()
		{
			Camera2D default_camera = new Camera2D( Director.Instance.GL, Director.Instance.DrawHelpers );
			default_camera.SetViewFromHeightAndCenter( 16.0f, GameEngine2D.Base.Math._00 );

			Camera = (ICamera)default_camera;
		}

		/// <summary>
		/// This function gets called when the scene is started by the Director.
		/// </summary>
		public override void OnEnter()
		{
			base.OnEnter();
			m_elapsed = 0.0;
		}

		/// <summary>
		/// Scene's OnExit calls Cleanup() by default. 
		/// </summary>
		public override void OnExit()
		{
			Cleanup();
			base.OnExit();
		}

		/// <summary>
		/// Return true if the scene is of TransitionScene type.
		/// </summary>
		public virtual bool IsTransitionScene()
		{
			return false;
		}

// 	public override void PushTransform()
// 	{
// 	}
//
// 	public override void PopTransform()
// 	{
// 	}

		// internal render function...
		// could split and move bits to PushTransform/PopTransform?
		public void render()
		{
			Camera.SetAspectFromViewport(); // probably useless as resolution never changes

			if ( !NoClear )
				Director.Instance.GL.Context.Clear(); // in order for transitions to work, GL.Context.Clear must be done here and not in Director

			Camera.Push();
			
//			#if DEBUG
			if ( ( Director.Instance.DebugFlags & DebugFlags.DrawGrid ) != 0 )
			{
				float scale = DrawGridStep;

				Bounds2 view_bounds = Camera.CalcBounds();
				float max_dist = view_bounds.Size.X > view_bounds.Size.Y ? view_bounds.Size.X : view_bounds.Size.Y;

				if ( max_dist == 0.0f )
					scale = 0.0f;

				else if ( scale == -1.0f )
				{
					// try to set an automatic grid size

					scale = 1.0f;

//					#if 0
//					while ( (int) ( max_dist / scale ) + 1 > 32 )
//					scale *= 2;
//					#else
					// p such that max_dist / 2^p < max_viewed_cells
					int p = (int)FMath.Floor( FMath.Log( max_dist / (float)DrawGridAutoStepMaxCells ) / FMath.Log( 2.0f ) ) + 1;
					p = p < 1 ? 0 : p;
					scale = (float)(1<<p);
//					#endif

					if ( p > 15 )
						scale = 0.0f;
				}

				if ( scale != 0.0f )
					Camera.DebugDraw( scale );
			}
//			#endif
/*
//			#if DEBUG
			if ( ( Director.Instance.DebugFlags & DebugFlags.DrawGrid ) != 0 )
			{
				Director.Instance.DrawHelpers.SetColor( Colors.Blue );
				Director.Instance.DrawHelpers.DrawBounds2( Camera.CalcBounds() );

				if ( Input2.Touch00.Down ) // debug touch pos
				{
					Director.Instance.DrawHelpers.SetColor( Colors.Green );
					Director.Instance.DrawHelpers.DrawCircle( Camera.GetTouchPos(), 0.3f * scale, 32 );
			}
			}
//			#endif
*/
			DrawHierarchy();

//			#if DEBUG
			if ( ( Director.Instance.DebugFlags & DebugFlags.DrawContentWorldBounds ) != 0 )
				draw_content_world_bounds();
//			#endif

			Camera.Pop();
		}

		/// Render all nodes's content bounding boxes in world space. This is mostly
		/// to debug GetContentWorldBounds. Note that this function ignores Plane3D 
		/// at the moment, so if your scene has any Plane3D, the debug primitive 
		/// drawn will look confusing.
		internal void draw_content_world_bounds()
		{
			Traverse( 

				( node, depth ) =>

				{ 
					Bounds2 bounds = new Bounds2();

					if ( node.GetContentWorldBounds( ref bounds ) )
					{
						Director.Instance.GL.SetBlendMode( BlendMode.Additive );
						Director.Instance.DrawHelpers.SetColor( Colors.Grey20 );
						Director.Instance.DrawHelpers.DrawBounds2( bounds );
					}

					return true;

				}

			, 0 );
		}
	}

} // namespace Sce.PlayStation.HighLevel.GameEngine2D

