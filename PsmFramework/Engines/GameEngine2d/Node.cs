/* SCE CONFIDENTIAL
 * PlayStation(R)Suite SDK 0.98.2
 * Copyright (C) 2012 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System.Collections.Generic;
using System.Diagnostics; // for [Conditional("DEBUG")]
using Sce.PlayStation.Core;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

namespace Sce.PlayStation.HighLevel.GameEngine2D
{
	/// <summary>
	/// Node is the base class for all scenegraph nodes. It holds a standard 2D transform, 
	/// a list of children and a handle to its parent (a node can have at most 1 parent).
	/// </summary>
	public class Node
	{
		Vector2 m_position;
		Vector2 m_rotation; // stored as (cos,sin), not angle
		Vector2 m_scale;
		Vector2 m_skew;
		Vector2 m_skew_tan;
		Vector2 m_pivot;
		int m_order; // nodes with m_order < 0 will be drawn before their Parent (same as cocos2d logic)
		bool m_cached_local_transform_info_is_identity;
		bool m_cached_local_transform_info_is_orthonormal;
		bool m_cached_local_transform_info_is_dirty;
		Matrix3 m_cached_local_transform;
		bool m_is_running;
		byte m_scheduler_and_action_manager_pause_flag;	// pause flag used by ActionManager.Instance and Director.Instance.Sheduler
		internal List< Scheduler.Entry > m_scheduler_entries; // used by Scheduler (null if unused)
		internal List< ActionBase > m_action_entries; // used by ActionManager (null if unused)

		/// <summary>See GetTransform() for details about how the transform matrix is constructed.</summary>
		public Vector2 Position { get { return m_position ;} set { m_position = value; m_cached_local_transform_info_is_dirty = true;}}

		/// <summary>
		/// Rotation is directly stored as a (cos,sin) unit vector. This the code to be potentially cos,sin calls free, and as a side
		/// effect we get the direction vector for free, and also avoid all the usual angle wrapping problems.
		/// See GetTransform() for details about how the transform matrix is constructed.
		/// </summary>
		public Vector2 Rotation { get { return m_rotation ;} set { m_rotation = value; m_cached_local_transform_info_is_dirty = true;}}

		/// <summary>
		/// RotationNormalize is like Rotation, but it normalizes on set,
		/// to prevent the unit vector from drifting because of accumulated numerical imprecision.
		/// See GetTransform() for details about how the transform matrix is constructed.
		/// </summary>
		public Vector2 RotationNormalize { get { return m_rotation ;} set { m_rotation = value.Normalize(); m_cached_local_transform_info_is_dirty = true;}}

		/// <summary>
		/// Rotate the object by an angle 'angle'.
		/// Note that this function simply affects the the Rotation/Angle property (it simply "increments" the angle, regardless of Pivot and Position; and all those are combined in GetTransform().)
		/// See GetTransform() for details about how the transform matrix is constructed.
		/// </summary>
		public void Rotate( float angle ) { Rotation = Rotation.Rotate( angle );}

		/// <summary>
		/// Rotate the object by an angle, the angle is given as a unit vector 'rotation'
		/// This lets you precompute the cos,sin needed during rotation.
		/// Note that this function simply affects the the Rotation/Angle property (it simply "increments" the angle, regardless of Pivot and Position; and all those are combined in GetTransform().)
		/// See GetTransform() for details about how the transform matrix is constructed.
		/// </summary>
		public void Rotate( Vector2 rotation ) { Rotation = Rotation.Rotate( rotation );}

		/// <summary>
		/// This property lets you set/get rotation as a angle. This is expensive and brings the usual
		/// angle discontinuity problems. The angle is always stored and returned in the the range -pi,pi.
		/// See GetTransform() for details about how the transform matrix is constructed.
		/// </summary>
		public float Angle { get { return Math.Angle( Rotation );} set { Rotation = Vector2.Rotation( value );}}

		/// <summary>
		/// See GetTransform() for details about how the transform matrix is constructed.
		/// </summary>
		public Vector2 Scale { get { return m_scale;} set { m_scale = value; m_cached_local_transform_info_is_dirty = true;}}

		/// <summary>
		/// See GetTransform() for details about how the transform matrix is constructed.
		/// </summary>
		public Vector2 Skew { get { return m_skew;} 

			set
			{ 
				m_skew = value;
				// cache the tan too
				m_skew_tan = new Vector2( FMath.Tan( m_skew.X ), FMath.Tan( m_skew.Y ) );
				m_cached_local_transform_info_is_dirty = true;
			}
		}

		/// <summary>
		/// Pivot is the pivot used for scale and rotation, and is expressed in this Node's local 'normalized' space.
		/// Which means that (0.5,0.5) is always the center of the object, regardless of the Scale for example.
		/// See GetTransform() for details about how the transform matrix is constructed.
		/// </summary>
		public Vector2 Pivot { get { return m_pivot;} set { m_pivot = value; m_cached_local_transform_info_is_dirty = true;}}

		/// <summary>
		/// VertexZ is the value set as the z coordinate during drawing. Note that by default ortho view only
		/// shows the [-1,1] Z range, just set Camera.Znear and Camera.Zfar if you want more.
		/// </summary>
		public float VertexZ; 

		/// <summary>If Visible is false, this node and its children are not drawn.</summary>
		public bool Visible;

		protected Node m_parent;
		/// <summary>The parent node in the scenegraph. A node can only be the child of at most one parent node.</summary>
		public Node Parent { get { return m_parent; } }

		protected List< Node > m_children;
		/// <summary>The list of children nodes.</summary>
		public List< Node > Children { get { return m_children; } }

		/// <summary>
		/// The delegate type used by AdHocDraw property.
		/// </summary>
		public delegate void DDraw();

		/// <summary>
		/// If set, AdHocDraw gets called in the base Draw function. This is used mostly so we can setup simple scenes 
		/// without always having to derive just so we can define a Draw function.
		/// </summary>
		public event DDraw AdHocDraw;

		/// <summary>
		/// You can use Node.Camera as a workaround to the fact there is normally only one camera in the scene.
		/// If Node.Camera is set, all transforms up to this node are ignored, and Node.Camera is push/pop
		/// everytime we draw this node.
		/// </summary>
		public ICamera Camera = null; 

		/// <summary>Shortcut to get the camera as a Camera2D.</summary>
		public Camera2D Camera2D { get { return(Camera2D)Camera;}}

		/// <summary>Shortcut to get the camera as a Camera3D.</summary>
		public Camera3D Camera3D { get { return(Camera3D)Camera;}}

		/// <summary>This property is true when this node is between its OnEnter()/OnExit() calls.</summary>
		public bool IsRunning { get { return m_is_running;}}

		/// <summary>The draw order value that was set in ReorderChild() or AddChild().</summary>
		public int Order { get { return m_order; }}

		/// <summary>Identifier for user.</summary>
		public string Name;

		/// <summary>Node constructor.</summary>
		public Node()
		{
			Position = GameEngine2D.Base.Math._00;
			Rotation = Math._10;
			Scale = Math._11;
			Skew = GameEngine2D.Base.Math._00;
			Pivot = GameEngine2D.Base.Math._00;
			VertexZ = 0.0f;
			m_order = 0;
			m_children = new List< Node >();
			Visible = true;
			m_parent = null;
			m_cached_local_transform_info_is_identity = true; // note: matrix might be identity even if this is false 
			m_cached_local_transform_info_is_orthonormal = true;
			m_cached_local_transform_info_is_dirty = false;
			m_cached_local_transform = Matrix3.Identity;
			m_is_running = false;
			m_scheduler_and_action_manager_pause_flag = 0; // we make the Scheduler and ActionManager pause flags 
														 // intrusive to Node, so that we don't need to hash (hope 
														 // it's the correct behavior)

//			Director.Instance.DebugLog( " Node construtor " + DebugInfo() );
		}

//		public virtual void OnParentChanged()
//		{
//		}

		~Node()
		{
//			Director.Instance.DebugLog( " Node destructor " + DebugInfo() );

			RemoveAllChildren( true );
			Cleanup();
		}

		/// <summary>
		/// This is called before drawing the node and its children.
		/// </summary>
		public virtual void PushTransform()
		{
			if ( Camera != null ) 
				Camera.Push();

			Director.Instance.GL.ModelMatrix.Push();

			if ( m_cached_local_transform_info_is_orthonormal )
				Director.Instance.GL.ModelMatrix.Mul1( GetTransform().Matrix4() );
			else Director.Instance.GL.ModelMatrix.Mul( GetTransform().Matrix4() );

			if ( VertexZ != 0.0f )
				Director.Instance.GL.ModelMatrix.Translate( new Vector3( 0.0f, 0.0f, VertexZ ) );
		}

		/// <summary>
		/// This is called after drawing the node and its children.
		/// </summary>
		public virtual void PopTransform()
		{
			Director.Instance.GL.ModelMatrix.Pop();

			if ( Camera != null ) 
				Camera.Pop();
		}

		/// <summary>
		/// This function gets called when the scene is started by the Director.Instance.
		/// </summary>
		public virtual void OnEnter()
		{
			Director.Instance.DebugLog( " OnEnter " + DebugInfo() );

			foreach ( Node child in Children )
				child.OnEnter();

			ResumeSchedulerAndActions();
			m_is_running = true;
		}

		/// <summary>Delegate for OnExit() events.</summary>
		public delegate void DOnExitEvent();

		/// <summary>
		/// List of events to perform when OnExit gets called.
		/// The list gets cleared after it is executed.
		/// </summary>
		public event DOnExitEvent OnExitEvents;

		/// <summary>
		/// This function gets called when we exit the Scene or when a child is explicitely removed 
		/// with RemoveChild() or RemoveAllChildren().
		/// </summary>
		public virtual void OnExit()
		{
			Director.Instance.DebugLog( " OnExit " + DebugInfo() );

			PauseSchedulerAndActions();
			m_is_running = false;

			foreach ( Node child in Children )
				child.OnExit();

			if ( OnExitEvents != null )
			{
//				System.Console.WriteLine( DebugInfo()+ " OnExitEvents got called" );
				OnExitEvents();
				OnExitEvents = null;
			}
		}

//		void ListIDisposable( ref List< System.IDisposable > list )
//		{
//		}

		// list up all disposable objects in the subtree 
		// starting at this (including this)
		void ListIDisposable( ref List< Node > list )
		{
			System.Type[] types = this.GetType().GetInterfaces();
			foreach ( System.Type type in types )
			{
				if ( type == typeof(System.IDisposable) ) 
				{
					list.Add( this );
					break;
				}
			}

			foreach ( Node child in Children )
				child.ListIDisposable( ref list );
		}

		/// <summary>
		/// Recurse through all the subtree (including this node)
		/// and register Dispose() functions for all the disposable
		/// objects. Cleanup is called first to make sure we
		/// don't Dispose() of running objects.
		/// </summary>
		public void RegisterDisposeOnExitRecursive()
		{
			OnExitEvents += () => 
				{ 
					Cleanup();
				};

			List< Node > list = new List< Node >();
			ListIDisposable( ref list );
			foreach ( Node disposable in list )
			{
//				System.Console.WriteLine( disposable.DebugInfo() + "..." );
				RegisterDisposeOnExit( (System.IDisposable)disposable );
			}
		}

		/// <summary>
		/// Register a call to Dispose() in the OnExit() function of this node.
		/// For example, when you want to Dispose() of several objects (TextureInfo,
		/// FontMap, etc) when you exit a Scene node.
		/// </summary>
		public void RegisterDisposeOnExit( System.IDisposable disposable )
		{
			OnExitEvents += () => 
				{ 
//					System.Console.WriteLine( "registered dispose called!" );
					disposable.Dispose(); 
				};

//			System.Console.WriteLine( DebugInfo()+ " RegisterDisposeOnExit registered a disposable object" );
		}

//		/// <summary>
//		/// DisposeTraverse calls Dispose( flags ) on this node and all its subtree.
//		/// </summary>
//		public void DisposeTraverse()
//		{
//			// if any dispose is happening, make sure nothing is keeping references 
//			// in Scheduler and ActionManager, since are going to Dispose/break some
//			// resources: cleanup all subtree
//
//			Cleanup();
//
//			// the recurse to call Dispose() on each subnode
//			Traverse( ( node, depth ) =>{ Common.DisposeAndNullifyIfIDisposable< Node >( node ); return true;}, 0 );
//		}

		void insert_child( Node child, int order )
		{
			Node last = null;
			if ( Children.Count != 0 )
				last = Children[ Children.Count - 1 ];

			if ( last == null || last.m_order <= order )
				Children.Add( child );
			else
			{
				int index = 0;
				foreach ( Node c in Children )
				{
					Common.Assert( c != null );

					if ( c.m_order > order )
					{
						Children.Insert( index, child );
						break;
					}
					index++;
				}
			}

			child.m_order = order;
		}

		/// <summary>
		/// Add a child with draw priority. 
		/// </summary>
		/// <param name="child">The child to add.</param>
		/// <param name="order">The added node's draw priority. Draw order follows order numerical order, 
		/// negative priorities mean this child node will be drawn before its parent, and children 
		/// with positive priorities get drawn after their parent.</param>
		public void AddChild( Node child, int order )
		{
			Common.Assert( child != this, "Trying to add " + child + " as child of itself." );
			Common.Assert( child != null, "Trying to add a null child.");
			Common.Assert( child.Parent == null, "Child " + child + " alreay has a parent, it can't be added somewhere else." );

//			Director.Instance.DebugLog( " AddChild " + child.DebugInfo() + " to " + DebugInfo() );

			insert_child( child, order );

			child.m_parent = this;
//			OnParentChanged();

			if ( m_is_running )
				child.OnEnter();
		}

		/// <summary>
		/// Add a child to this node, using its current order.
		/// </summary>
		public void AddChild( Node child )
		{
			AddChild( child, child.m_order );
		}

		/// <summary>
		/// Remove a child from this node.
		/// </summary>
		/// <param name="child">The child to remove.</param>
		/// <param name="do_cleanup">Do we call Cleanup for the removed node.</param>
		public void RemoveChild( Node child, bool do_cleanup/*, bool dispose */ )
		{
//			Director.Instance.DebugLog( " RemoveChild " + child.DebugInfo() + " from " + DebugInfo() );

			if ( child == null )
				return;

			if ( Children.Contains( child ) )
			{
				child.on_remove( do_cleanup/*, dispose*/ );
				Children.Remove( child );
			}
		}

		/// <summary>
		/// This is equivalent to calling RemoveChild( dispose_flags ) for all children.
		/// </summary>
		public void RemoveAllChildren( bool do_cleanup/*, bool dispose */ )
		{
//			Director.Instance.DebugLog( " RemoveAllChildren(" + do_cleanup + ") from " + DebugInfo() );

			foreach ( Node child in Children )
			{
				child.on_remove( do_cleanup/*, dispose*/ );
				child.m_parent = null;
//				OnParentChanged();
			}

			Children.Clear();
		}

		void on_remove( bool do_cleanup/*, bool dispose */ )
		{
			if ( m_is_running )
				OnExit();

//			if ( dispose )
//				DisposeTraverse();

			if ( do_cleanup )
				Cleanup();

			m_parent = null;
		}

		/// <summary>
		/// Change order of a child within the Children list.
		/// </summary>
		void ReorderChild( Node child, int order )
		{
			// fixme: wasteful
			Children.Remove( child );
			insert_child( child, order );
		}

		/// <summary>
		/// Change the draw order value for this node (see AddChild for details about the draw order).
		/// </summary>
		void Reorder( int order )
		{
			if ( Parent != null )
				Parent.ReorderChild( this, order );
		}

		/// <summary>
		/// Function type to pass to the .Traverse method.
		/// </summary>
		public delegate bool DVisitor( Node node, int depth );

		/// <summary>
		/// Call the 'visitor' function for this node and all its children, recursively.
		/// Interrupt traversing if visitor returns false.
		/// </summary>
		public virtual void Traverse( DVisitor visitor, int depth )
		{
			if ( !visitor( this, depth ) )
				return;

			foreach ( Node child in Children )
				child.Traverse( visitor, depth + 1 );
		}

		/// <summary>
		/// This called by Director only, but PushTransform, Draw,
		/// and PopTransform can be overriden.
		/// </summary>
		virtual public void DrawHierarchy()
		{
			if ( !Visible )
				return;

			////Common.Profiler.Push("DrawHierarchy's PushTransform");
			PushTransform();
			////Common.Profiler.Pop();

			int index=0;
			for ( ; index < Children.Count; ++index )
			{
				if ( Children[index].Order >= 0 )	break;
				Children[index].DrawHierarchy();
			}

			////Common.Profiler.Push("DrawHierarchy's PostDraw");
			Draw();
			////Common.Profiler.Pop();

			for ( ; index < Children.Count; ++index )
				Children[index].DrawHierarchy();

//			#if DEBUG
			if ( ( Director.Instance.DebugFlags & DebugFlags.DrawPivot ) != 0 )
				DebugDrawPivot();

			if ( ( Director.Instance.DebugFlags & DebugFlags.DrawContentLocalBounds ) != 0 )
				DebugDrawContentLocalBounds();
//			#endif

			////Common.Profiler.Push("DrawHierarchy's PopTransform");
			PopTransform();
			////Common.Profiler.Pop();

//			#if DEBUG
			if ( ( Director.Instance.DebugFlags & DebugFlags.DrawTransform ) != 0 )
				DebugDrawTransform();
//			#endif
		}

		/// <summary>
		/// Renders what's *inside* the PushTransform / PopTransform.
		/// </summary>
		public virtual void Draw() 
		{
			if ( AdHocDraw != null )
				AdHocDraw();
		}

		/// <summary>
		/// The update function.
		/// The Director decides how many times a frame this function should be called, and with which delta time. 
		/// At the moment, Update functions are called once using the frame delta time as it is.
		/// </summary>
		/// <param name="dt">Delta time in seconds.</param>
		public virtual void Update( float dt )
		{
		}

		/// <summary>
		/// Draw bounds of local content and pivot, in Node local space.
		/// Normally you don't have to override this function, you just 
		/// override GetlContentLocalBounds() and this function shows it
		/// when DebugFlags.DrawContentLocalBounds is set for example. 
		/// </summary>
		public virtual void DebugDrawContentLocalBounds()
		{
			var content_local_bounds = new Bounds2();
			GetlContentLocalBounds( ref content_local_bounds );

			Director.Instance.DrawHelpers.SetColor( Colors.Yellow );
			Director.Instance.DrawHelpers.DrawBounds2( content_local_bounds );
		}

		public void DebugDrawPivot()
		{
			Director.Instance.DrawHelpers.SetColor( Colors.White );
			Director.Instance.DrawHelpers.DrawDisk( Pivot, 0.1f, 12 );
		}

		/// <summary>
		/// A scale factor used by DebugDrawTransform to draw arrows.
		/// By default this is 1.0f, which means that unit length arrows are of length 1 on screen. 
		/// he game world showed on screen is too big, arrows of length one might be less then 1 pixel,
		/// and you won't be able to see them even through they are being drawn. In that case you can 
		/// scale them with DebugDrawTransform.
		/// </summary>
		public static float DebugDrawTransformScale = 1.0f;

		/// <summary>
		/// Draw the local coordinate system, as arrows, in Parent Node's local space.
		/// </summary>
		public void DebugDrawTransform()
		{
			Matrix3 mat = GetTransform();

			mat.X.Xy *= DebugDrawTransformScale;
			mat.Y.Xy *= DebugDrawTransformScale;

			Director.Instance.DrawHelpers.DrawCoordinateSystem2D( mat, new DrawHelpers.ArrowParams( DebugDrawTransformScale ) );
		}
		
		/// <summary>Start an action on this node.</summary>
		public void RunAction( ActionBase action )
		{
			ActionManager.Instance.AddAction( action, this/*, !m_is_running*/ );
			action.Run();
		}

		/// <summary>Stop all actions on this node.</summary>
		public void StopAllActions()
		{
			if (ActionManager.Instance != null)
				ActionManager.Instance.RemoveAllActionsFromTarget( this );
		}

		/// <summary>Stop an action on this node.</summary>
		public void StopAction( ActionBase action )
		{
			Common.Assert( action.Target == this || action.Target == null );
			ActionManager.Instance.RemoveAction( action );
		}

		/// <summary>Search for the first action acting on this node with tag value 'tag' and stop/remove it it.</summary>
		public void StopActionByTag( int tag )
		{
			ActionManager.Instance.RemoveActionByTag( tag, this );
		}

		/// <summary>Return the 'ith' action with tag 'tag' acting on this node.</summary>
		public ActionBase GetActionByTag( int tag, int ith = 0 )
		{
			return ActionManager.Instance.GetActionByTag( tag, this, ith );
		}

		/// <summary>Get the number of action acting on this node.</summary>
		public int NumRunningActions()
		{
			return ActionManager.Instance.NumRunningActions( this );
		}

		/// <summary>Recursively stop all actions and scheduled functions on this node.</summary>
		public virtual void Cleanup()
		{
			StopAllActions();
			UnscheduleAll();

			foreach ( Node child in Children )
				child.Cleanup();
		}

		/// <summary>Register this node's update function to the scheduler, it will get called everyframe.</summary>
		public void ScheduleUpdate( int priority = Scheduler.DefaultPriority )
		{
			Scheduler.Instance.ScheduleUpdateForTarget( this, priority, !m_is_running );
		}

		/// <summary>Remove the update function from the scheduler.</summary>
		public void UnscheduleUpdate()
		{
			Scheduler.Instance.UnscheduleUpdateForTarget( this );
		}

		/// <summary>Schedule node function 'func', it will get called everyframe.</summary>
		public void Schedule( DSchedulerFunc func, int priority = Scheduler.DefaultPriority )
		{
			Scheduler.Instance.Schedule( this, func, 0.0f, !m_is_running, priority );
		}

		/// <summary>Schedule node function 'func' so it gets called every 'interval' seconds.</summary>
		public void ScheduleInterval( DSchedulerFunc func, float interval, int priority = Scheduler.DefaultPriority )
		{
			Scheduler.Instance.Schedule( this, func, interval, !m_is_running, priority );
		}

		/// <summary>Unschedule node function 'func'.</summary>
		public void Unschedule( DSchedulerFunc func )
		{
			Scheduler.Instance.Unschedule( this, func );
		}

		/// <summary>Unschedule all functions related to this node.</summary>
		public void UnscheduleAll()
		{
			if (Scheduler.Instance != null)
				Scheduler.Instance.UnscheduleAll( this );
		}

		/// <summary>All actions related to this node can be paused on and off with this flag.</summary>
		public bool ActionsPaused
		{
			set {
				if ( value ) m_scheduler_and_action_manager_pause_flag |= 1; // pause off
				else m_scheduler_and_action_manager_pause_flag &= unchecked((byte)~1); // pause on
			}

			get { return( m_scheduler_and_action_manager_pause_flag & 1 ) != 0;}
		}

		/// <summary>All scheduled functions related to this node can be paused on and off with this flag.</summary>
		public bool SchedulerPaused
		{
			set {
				if ( value ) m_scheduler_and_action_manager_pause_flag |= 2; // pause off
				else m_scheduler_and_action_manager_pause_flag &= unchecked((byte)~2); // pause on
			}

			get { return( m_scheduler_and_action_manager_pause_flag & 2 ) != 0;}
		}

		/// <summary>Sets SchedulerPaused and ActionsPaused to true.</summary>
		public void ResumeSchedulerAndActions()
		{
			SchedulerPaused = false; // Scheduler.Instance.ResumeTarget( this );
			ActionsPaused = false; // ActionManager.Instance.ResumeTarget( this);
		}

		/// <summary>Sets SchedulerPaused and ActionsPaused to false.</summary>
		public void PauseSchedulerAndActions()
		{
			SchedulerPaused = true;	// Scheduler.Instance.PauseTarget( this );
			ActionsPaused = true; // ActionManager.Instance.PauseTarget( this);
		}

		/// <summary>
		/// Return the transform matrix of this node, expressed in its parent space.
		/// 
		/// The node transform matrix is formed using the data accessed with the Position, Scale, Skew,
		/// Rotation/Angle/RotationNormalize, Pivot properties. The transform matrix is equivalent to:
		/// 
		/// 	 Matrix3.Translation( Position )
		/// 	* Matrix3.Translation( Pivot )
		/// 	* Matrix3.Rotation( Rotation )
		/// 	* Matrix3.Scale( Scale )
		/// 	* Matrix3.Skew( Skew )
		/// 	* Matrix3.Translation( -Pivot )
		/// 
		/// Node that the transform matrix returned is a pure 2D transform. 
		/// VertexZ is applied separately in the PushTransform function.
		/// </summary>
		public Matrix3 GetTransform()
		{
			if ( m_cached_local_transform_info_is_dirty )
			{
				// Note that the Pivot is in LOCAL space

//				m_cached_local_transform = Matrix3.Translation( Position ) 
//										 * Matrix3.Translation( Pivot ) 
//										 * Matrix3.Rotation( Rotation ) 
//										 * Matrix3.Scale( Scale )
//										 * Matrix3.Skew( m_skew_tan )
//										 * Matrix3.Translation( -Pivot )
//										 ;

				// this should be the exact same as above
				Math.TranslationRotationScale( ref m_cached_local_transform, Position + Pivot, Rotation, Scale );

//				m_cached_local_transform = m_cached_local_transform
//										 * Matrix3.Skew( m_skew_tan )
//										 * Matrix3.Translation( -Pivot );

				m_cached_local_transform = m_cached_local_transform 
										 // form the Matrix3.Skew( m_skew_tan ) * Matrix3.Translation( -Pivot ) matrix directly:
										 * new Matrix3( new Vector3( 1.0f, m_skew_tan.X, 0.0f ) ,
														 new Vector3( m_skew_tan.Y, 1.0f, 0.0f ) ,
														 new Vector3( -Pivot * ( Math._11 + m_skew_tan.Yx ), 1.0f ) );

				m_cached_local_transform_info_is_identity = false; // we don't know, so false
				m_cached_local_transform_info_is_orthonormal = ( Scale == Math._11 && Skew == GameEngine2D.Base.Math._00 );
				m_cached_local_transform_info_is_dirty = false;
			}
			return m_cached_local_transform;
		}

		/// <summary>
		/// Get the inverse of this node 's transform matrix.
		/// </summary>
		public Matrix3 GetTransformInverse()
		{
			if ( m_cached_local_transform_info_is_orthonormal )
				return GetTransform().InverseOrthonormal();
			return GetTransform().Inverse();
		}

		/// <summary>
		/// Return the transform matrix of this node, expressed in its world/parent Scene space.
		/// </summary>
		public Matrix3 GetWorldTransform()
		{
			Matrix3 ret = GetTransform();

			for ( Node parent = Parent; parent != null; parent = parent.Parent )
				ret = parent.GetTransform() * ret;

			return ret;
		}

		/// <summary>
		/// Get the inverse of this node's world transform matrix.
		/// </summary>
		public Matrix3 CalcWorldTransformInverse()
		{
			Matrix3 ret = GetTransformInverse();

			for ( Node parent = Parent; parent != null; parent = parent.Parent )
				ret = ret * parent.GetTransformInverse();

			return ret;
		}

		/// <summary>
		/// LocalToWorld Should return the same as ( GetWorldTransform() * world_point.Xy1 ).Xy.
		/// The local space of the node is the space in which its geometry is defined, 
		/// i.e one level below GetWorldTransform().
		/// </summary>
		public Vector2 LocalToWorld( Vector2 local_point )
		{
			Vector3 p = GetTransform() * local_point.Xy1;

			for ( Node parent = Parent; parent != null; parent = parent.Parent )
			{
				if ( parent.m_cached_local_transform_info_is_identity == false )
					p = parent.GetTransform() * p;
			}

			return p.Xy;
		}

		/// <summary>
		/// Should return the same as ( CalcWorldTransformInverse() * world_point.Xy1 ).Xy.
		/// The local space of the node is the space in which its geometry is defined, 
		/// i.e one level below GetWorldTransform().
		/// </summary>
		public Vector2 WorldToLocal( Vector2 world_point )
		{
			Matrix3 m = GetTransformInverse();

			for ( Node parent = Parent; parent != null; parent = parent.Parent )
			{
				if ( parent.m_cached_local_transform_info_is_identity == false )
					m = m * parent.GetTransformInverse();
			}

			return( m * world_point.Xy1 ).Xy;
		}

		/// <summary>
		/// Get the bounds for the content/geometry of this node (only), in node space (no recursion).
		/// Nodes that don't have any content just return false.
		/// </summary>
		public virtual bool GetlContentLocalBounds( ref Bounds2 bounds )
		{
			return false;
		}

		/// <summary>
		/// Get the bounds for the content of this node (only), in world space (no recursion).
		/// Nodes that don't have any content just return false and don't touch bounds.
		/// </summary>
		public virtual bool GetContentWorldBounds( ref Bounds2 bounds )
		{
			Bounds2 lbounds = new Bounds2();

			if ( !GetlContentLocalBounds( ref lbounds ) )
				return false; // this node had no content

			Matrix3 m = GetWorldTransform();

			bounds = new Bounds2( ( m * lbounds.Point00.Xy1 ).Xy );
			bounds.Add( ( m * lbounds.Point10.Xy1 ).Xy );
			bounds.Add( ( m * lbounds.Point01.Xy1 ).Xy );
			bounds.Add( ( m * lbounds.Point11.Xy1 ).Xy );

			return true;
		}

		/// <summary>
		/// Return true if 'world_position' is inside the content oriented bounding box.
		/// </summary>
		public bool IsWorldPointInsideContentLocalBounds( Vector2 world_position )
		{
			Bounds2 bounds = new Bounds2();

			if ( !GetlContentLocalBounds( ref bounds ) )
				return false;

			return bounds.IsInside( WorldToLocal( world_position ) );
		}

		/// <summary>
		/// Follow parent hierarchy until we find a Plane3D node,
		/// and set 'mat' to the Plane3D's plane matrix.
		/// </summary>
		public virtual void FindParentPlane( ref Matrix4 mat )
		{
			if ( Parent != null ) 
				Parent.FindParentPlane( ref mat );
		}

		/// <summary>
		/// Like Director.Instance.CurrentScene.Camera.NormalizedToWorld, but deals with
		/// the case when there is a Plane3D among ancestors in the scenegraph.
		/// </summary>
		Vector2 NormalizedToWorld( Vector2 bottom_left_minus_1_minus_1_top_left_1_1_normalized_screen_pos )
		{
			if ( Camera != null ) return Camera.NormalizedToWorld( bottom_left_minus_1_minus_1_top_left_1_1_normalized_screen_pos );
			Matrix4 plane_mat = Matrix4.Identity;
			FindParentPlane( ref plane_mat );
			Director.Instance.CurrentScene.Camera.SetTouchPlaneMatrix( plane_mat );
			return Director.Instance.CurrentScene.Camera.NormalizedToWorld( bottom_left_minus_1_minus_1_top_left_1_1_normalized_screen_pos );
		}

		/// <summary>
		/// Like Director.Instance.CurrentScene.Camera.GetTouchPos, but deals with
		/// the case when there is a Plane3D among ancestors in the scenegraph.
		/// </summary>
		public Vector2 GetTouchPos( int nth = 0, bool prev = false )
		{
			if ( Camera != null ) return Camera.GetTouchPos( nth, prev );
			Matrix4 plane_mat = Matrix4.Identity;
			FindParentPlane( ref plane_mat );
			Director.Instance.CurrentScene.Camera.SetTouchPlaneMatrix( plane_mat );
			return Director.Instance.CurrentScene.Camera.GetTouchPos( nth, prev );
		}

		public virtual string DebugInfo()
		{
			return "{" + GetType().Name + ":" + Name + "}";
//			return "{" + GetType().Name + ":" + Name + "} " + GetHashCode();
		}
	}
} // namespace Sce.PlayStation.HighLevel.GameEngine2D

