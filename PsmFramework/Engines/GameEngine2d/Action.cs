/* SCE CONFIDENTIAL
 * PlayStation(R)Suite SDK 0.98.2
 * Copyright (C) 2012 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

namespace Sce.PlayStation.HighLevel.GameEngine2D
{
	/// <summary>
	/// The base class for all actions.
	/// </summary>
	public class ActionBase
	{
		bool m_running = false;
		Node m_target_node;

		/// <summary>A tag value that can be used for searching this action.</summary>
		public int Tag;

		/// <summary>IsRunning is true when the action is active.</summary>
		public bool IsRunning { get { return m_running;}}

		// not in Target set property for visibility
		internal void set_target( Node value )
		{
			Common.Assert( !IsRunning );
			m_target_node = value;
		}

		/// <summary>The node affected by this action.</summary>
		public Node Target { get { return m_target_node; } }

		/// <summary>Kick the action.</summary>
		public virtual void Run() 
		{
			m_running = true;
		}

		/// <summary>Stop the action (some types of actions stop themselves).</summary>
		public virtual void Stop()
		{
			m_running = false;
		}

		/// <summary>The update function for this action, called every frame.</summary>
		public virtual void Update( float dt )
		{
		}
	}

	/// <summary>
	/// The base class for actions with a finite duration.
	/// This is an abstract class.
	/// </summary>
	public abstract class ActionWithDuration : ActionBase
	{
		protected float m_elapsed = 0.0f;

		/// <summary>This action's duration in seconds.</summary>
		public float Duration = 0.0f;

		/// <summary>Kick this action.</summary>
		public override void Run()
		{
			base.Run();
			m_elapsed = 0.0f;
		}
	}

	/// <summary>
	/// A one shot action that calls a user function.
	/// </summary>
	public class CallFunc : ActionBase
	{
		/// <summary>The function this action calls.</summary>
		public System.Action Func;

		/// <summary>CallFunc constructor.</summary>
		public CallFunc( System.Action func ) 
		{
			Func = func;
		}

		/// <summary>The update function.</summary>
		public override void Update( float dt )
		{
			if ( !IsRunning )
				return;

			if ( Func != null )
				Func();

			Stop();
		}
	}

	/// <summary>
	/// A wait action, that ends after a user specified duration.
	/// </summary>
	public class DelayTime : ActionWithDuration
	{
		/// <summary>DelayTime constructor.</summary>
		public DelayTime()
		{
		}

		/// <summary>DelayTime constructor.</summary>
		/// <param name="duration">The time to wait, in seconds.</param>
		public DelayTime( float duration )
		{
			Duration = duration;
		}

		/// <summary>Kick this action.</summary>
		public override void Run()
		{
			base.Run();
		}

		/// <summary>The update function.</summary>
		public override void Update( float dt )
		{
			if ( !IsRunning )
				return;

			if ( Duration <= m_elapsed )
				Stop();

			m_elapsed += dt;
		}
	}

	/// <summary>
	/// The base class for generic interpolating actions, you just pass set/get functions.
	/// This is an abstract class.
	/// </summary>
	public abstract class ActionTweenGeneric<T> : ActionWithDuration
	{
		/// <summary>The target value.</summary>
		public T TargetValue;

		/// <summary>If true, the target value is an offset.</summary>
		public bool IsRelative = false;

		protected T m_start_value;

		/// <summary>
		/// The tween/interpolation function, basically a f(x) function where f(x) goes from
		/// 0 to 1 when x goes from 0 to 1.
		/// 
		/// Example of function you can set DTween to:
		/// 
		/// (t) => Math.Linear(t);
		/// 
		/// (t) => Math.ExpEaseIn(t,1.0f);
		/// 
		/// (t) => Math.ExpEaseOut(t,1.0f);
		/// 
		/// (t) => Math.PowEaseIn(t,4.0f);
		/// 
		/// (t) => Math.PowEaseOut(t,4.0f);
		/// 
		/// (t) => Math.PowEaseInOut(t,4.0f);
		/// 
		/// (t) => Math.BackEaseIn(t,1.0f);
		/// 
		/// (t) => Math.BackEaseOut(t,1.0f);
		/// 
		/// (t) => Math.BackEaseInOut(t,1.0f);
		/// 
		/// (t) => Math.PowerfulScurve(t,3.7f,3.7f);
		/// 
		/// (t) => Math.Impulse(t,10.0f);
		/// 
		/// (t) => FMath.Sin(t*Math.Pi*0.5f); // for sin curve tween
		/// 
		/// More about tweens can be found int he cocos2d documentaiton which itselfs points to:
		/// 
		/// http://www.robertpenner.com/easing/easing_demo.html
		/// 
		/// http://www.robertpenner.com/easing/penner_chapter7_tweening.pdf
		/// 
		/// </summary>
		public DTween Tween = (t) => Math.PowEaseOut(t,4.0f);

		/// <summary>
		/// Target value set function delegate.
		/// </summary>
		public delegate void DSet( T value );

		/// <summary>
		/// Target value get function delegate.
		/// </summary>
		public delegate T DGet();

		/// <summary>
		/// The function that sets the current value as we execute the tween.
		/// </summary>
		public DSet Set;

		/// <summary>
		/// The function that gets the current value as we execute the tween.
		/// </summary>
		public DGet Get;

		public abstract void lerp( float alpha );

		/// <summary>ActionTweenGeneric constructor.</summary>
		public ActionTweenGeneric() 
		{
		}

		/// <summary>ActionTweenGeneric constructor.</summary>
		public ActionTweenGeneric( DGet dget, DSet dset ) 
		{
			Get = dget;
			Set = dset;
		}

		/// <summary>Kick this action.</summary>
		public override void Run()
		{
			base.Run();

			m_start_value = Get();
		}

		/// <summary>The update function.</summary>
		public override void Update( float dt )
		{
			if ( !IsRunning )
				return;

			float t = FMath.Clamp( m_elapsed / Duration, 0.0f, 1.0f );

			float alpha = Tween( t );

			lerp( alpha );

			if ( m_elapsed / Duration > 1.0f ) // note: set m_running *before* += dt
				Stop();

			m_elapsed += dt;
		}
	}

	// we have to manually instanciate ActionTweenGeneric*

	/// <summary>
	/// ActionTweenGenericVector2 does a simple element wise blend from start value to target value.
	/// </summary>
	public class ActionTweenGenericVector2 : ActionTweenGeneric<Vector2>
	{
		public override void lerp( float alpha )
		{
			if ( IsRelative == false ) Set( Math.Lerp( m_start_value, TargetValue, alpha ) );
			else Set( m_start_value + TargetValue * alpha );
		}
	}

	/// <summary>
	/// ActionTweenGenericVector4 does a simple element wise blend from start value to target value.
	/// </summary>
	public class ActionTweenGenericVector4 : ActionTweenGeneric<Vector4>
	{
		public override void lerp( float alpha )
		{
			if ( IsRelative == false ) Set( Math.Lerp( m_start_value, TargetValue, alpha ) );
			else Set( m_start_value + TargetValue * alpha );
		}
	}

	/// <summary>
	/// ActionTweenGenericVector2Scale is similar to ActionTweenGenericVector2, 
	/// but acts on scale values (multiplicative).
	/// </summary>
	public class ActionTweenGenericVector2Scale : ActionTweenGeneric<Vector2>
	{
		public override void lerp( float alpha )
		{
			if ( IsRelative == false ) Set( Math.Lerp( m_start_value, TargetValue, alpha ) );
			else Set( Math.Lerp( m_start_value, m_start_value * TargetValue, alpha ) );
		}
	}

	/// <summary>
	/// ActionTweenGenericVector2Rotation interpolates a unit vector 
	/// (a unit vector interpreted as a rotation).
	/// </summary>
	public class ActionTweenGenericVector2Rotation : ActionTweenGeneric<Vector2>
	{
		public override void lerp( float alpha )
		{
			if ( IsRelative == false ) Set( Math.LerpUnitVectors( m_start_value, TargetValue, alpha ) );
			else Set( m_start_value.Rotate( Math.Angle( TargetValue ) * alpha ) );
		}
	}

	/// <summary>An action that gradually adds an offset to the current position.</summary>
	public class MoveBy : ActionTweenGenericVector2
	{
		/// <summary>MoveBy constructor.</summary>
		public MoveBy( Vector2 target, float duration )
		{ 
			TargetValue = target; 
			Duration = duration; 
			IsRelative = true; 
			Get = () => { return Target.Position;}; 
			Set = ( Vector2 value ) => { Target.Position = value;};
		}
	}

	/// <summary>An action that gradually moves position to the specified target.</summary>
	public class MoveTo : ActionTweenGenericVector2
	{
		/// <summary>MoveTo constructor.</summary>
		public MoveTo( Vector2 target, float duration )
		{ 
			TargetValue = target; 
			Duration = duration; 
			IsRelative = false; 
			Get = () => { return Target.Position;}; 
			Set = ( Vector2 value ) => { Target.Position = value;};
		}
	}

	/// <summary>An action that gradually applies an extra scale to the current scale.</summary>
	public class ScaleBy : ActionTweenGenericVector2Scale
	{
		/// <summary>ScaleBy constructor.</summary>
		public ScaleBy( Vector2 target, float duration )
		{ 
			TargetValue = target; 
			Duration = duration; 
			IsRelative = true; 
			Get = () => { return Target.Scale;}; 
			Set = ( Vector2 value ) => { Target.Scale = value;};
		}
	}

	/// <summary>An action that gradually sets the scale to the specified value.</summary>
	public class ScaleTo : ActionTweenGenericVector2Scale
	{
		/// <summary>ScaleTo constructor.</summary>
		public ScaleTo( Vector2 target, float duration )
		{ 
			TargetValue = target; 
			Duration = duration; 
			IsRelative = false; 
			Get = () => { return Target.Scale;}; 
			Set = ( Vector2 value ) => { Target.Scale = value;};
		}
	}

	/// <summary>An action that gradually adds an offset to the current skew.</summary>
	public class SkewBy : ActionTweenGenericVector2
	{
		/// <summary>SkewBy constructor.</summary>
		public SkewBy( Vector2 target, float duration )
		{ 
			TargetValue = target; 
			Duration = duration; 
			IsRelative = true; 
			Get = () => { return Target.Skew;}; 
			Set = ( Vector2 value ) => { Target.Skew = value;};
		}
	}

	/// <summary>An action that gradually sets the skew to the specified value.</summary>
	public class SkewTo : ActionTweenGenericVector2
	{
		/// <summary>SkewTo constructor.</summary>
		public SkewTo( Vector2 target, float duration )
		{ 
			TargetValue = target; 
			Duration = duration; 
			IsRelative = false; 
			Get = () => { return Target.Skew;}; 
			Set = ( Vector2 value ) => { Target.Skew = value;};
		}
	}
/*
	/// <summary></summary>
	public class MovePivotBy : ActionTweenGenericVector2 
	{ 
		/// <summary></summary>
		public MovePivotBy( Vector2 target, float duration ) 
		{ 
			TargetValue = target;
			Duration = duration;
			IsRelative = true; 
			Get = () => { return Target.Pivot; }; 
			Set = ( Vector2 value ) => { Target.Pivot = value; }; 
		} 
	}

	/// <summary></summary>
	public class MovePivotTo : ActionTweenGenericVector2 
	{ 
		/// <summary></summary>
		public MovePivotTo( Vector2 target, float duration ) 
		{ 
			TargetValue = target;
			Duration = duration;
			IsRelative = false; 
			Get = () => { return Target.Pivot; }; 
			Set = ( Vector2 value ) => { Target.Pivot = value; }; 
		} 
	}
*/
	/// <summary>An action that gradually adds an offset to the current rotation.</summary>
	public class RotateBy : ActionTweenGenericVector2Rotation
	{
		/// <summary>RotateBy constructor.</summary>
		public RotateBy( Vector2 target, float duration )
		{ 
			TargetValue = target; 
			Duration = duration; 
			IsRelative = true; 
			Get = () => { return Target.Rotation;}; 
			Set = ( Vector2 value ) => { Target.Rotation = value;};
		}
	}

	/// <summary>An action that gradually sets the rotation to the specified value.</summary>
	public class RotateTo : ActionTweenGenericVector2Rotation
	{
		/// <summary>RotateTo constructor.</summary>
		public RotateTo( Vector2 target, float duration )
		{ 
			TargetValue = target; 
			Duration = duration; 
			IsRelative = false; 
			Get = () => { return Target.Rotation;}; 
			Set = ( Vector2 value ) => { Target.Rotation = value;};
		}
	}

	/// <summary>An action that gradually adds an offset to the color.</summary>
	public class TintBy : ActionTweenGenericVector4
	{
		/// <summary>TintBy constructor.</summary>
		public TintBy( Vector4 target, float duration )
		{ 
			TargetValue = target; 
			Duration = duration; 
			IsRelative = true; 
			Get = () => { return((SpriteBase)Target).Color;}; 
			Set = ( Vector4 value ) => { ((SpriteBase)Target).Color = value;};
		}
	}

	/// <summary>An action that gradually sets the color to the specified value.</summary>
	public class TintTo : ActionTweenGenericVector4
	{
		/// <summary>TintTo constructor.</summary>
		public TintTo( Vector4 target, float duration )
		{ 
			TargetValue = target; 
			Duration = duration; 
			IsRelative = false; 
			Get = () => { return((SpriteBase)Target).Color;}; 
			Set = ( Vector4 value ) => { ((SpriteBase)Target).Color = value;};
		}
	}

	/// <summary>
	/// An action that runs a sequence of other actions, in order.
	/// </summary>
	public class Sequence : ActionBase
	{
		List< ActionBase > m_actions = new List<ActionBase>();
		int m_current = 0;

		/// <summary>Sequence constructor.</summary>
		public Sequence() 
		{
		}

		/// <summary>Add an action this actions sequence.</summary>
		public void Add( ActionBase action )
		{
			Common.Assert( !IsRunning );
			m_actions.Add( action );
		}

		/// <summary>Kick this action.</summary>
		public override void Run()
		{
			base.Run();

			m_current = 0;

			if ( m_actions.Count == 0 )
			{
				Stop();
				return;
			}

			// start first action
			Target.RunAction( m_actions[ m_current ] );
		}

		/// <summary>Stop this action.</summary>
		public override void Stop()
		{
			base.Stop();

			foreach ( ActionBase action in m_actions )
			{
				if ( action != null )
					action.Stop();
			}
		}

		// maybe Sequence should just add/remove from the Action manager for consistency? when pausing etc.

		/// <summary>The update function.</summary>
		public override void Update( float dt )
		{
			// kalin: why would we be Update()'d if we are not running?
			if ( !IsRunning )
				return;

//			System.Console.WriteLine( "Sequence.Update m_current="+ m_current);

			if ( m_actions[ m_current ].IsRunning == false )
			{
				// go to action n+1, stop if this was the last

				if ( m_current == m_actions.Count - 1 )
				{
					Stop();
					return;
				}

				m_current++;
				Target.RunAction( m_actions[ m_current ] );
			}
		}
	}

	/// <summary>
	/// An action that repeats an other action forever.
	/// </summary>
	public class RepeatForever : ActionBase
	{
		/// <summary>The action to repeat.</summary>
		public ActionBase InnerAction;

		/// <summary>Kick this action.</summary>
		public override void Run()
		{
			base.Run();

			if ( InnerAction == null )
			{
				Stop();
				return;
			}

			Target.RunAction( InnerAction );
		}

		/// <summary>Stop this action.</summary>
		public override void Stop()
		{
			base.Stop();

			if ( InnerAction != null )
				InnerAction.Stop();
		}

		/// <summary>The update function.</summary>
		public override void Update( float dt )
		{
			if ( !IsRunning || !InnerAction.IsRunning )
				Run();
		}
	}

	/// <summary>
	/// An action that repeats an action a finite number of times.
	/// </summary>
	public class Repeat : ActionBase
	{
		/// <summary>The action to repeat.</summary>
		public ActionBase InnerAction;
		/// <summary>The number of times we want to repeat.</summary>
		public int Times = 0;

		int m_count = 0;

		/// <summary>
		/// Repeat constructor.
		/// </summary>
		/// <param name="inner_action">The action to repeat.</param>
		/// <param name="times">The number of times the action must be repeated.</param>
		public Repeat( ActionBase inner_action, int times ) 
		{
			InnerAction = inner_action;
			Times = times;
		}

		/// <summary>Kick this action.</summary>
		public override void Run()
		{
			base.Run();

			if ( InnerAction == null )
			{
				Stop();
				return;
			}

			m_count = 0;

			Target.RunAction( InnerAction );
		}

		/// <summary>Stop this action.</summary>
		public override void Stop()
		{
			base.Stop();

			if ( InnerAction != null )
				InnerAction.Stop();
		}

		/// <summary>The update function.</summary>
		public override void Update( float dt )
		{
			if ( ( !IsRunning || !InnerAction.IsRunning ) 
				 && m_count < Times )
			{
				Target.RunAction( InnerAction ); // re-kick InnerAction

				++m_count;
			}
		}
	}

} // namespace Sce.PlayStation.HighLevel.GameEngine2D

