/* SCE CONFIDENTIAL
 * PlayStation(R)Suite SDK 0.98.2
 * Copyright (C) 2012 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System.Collections.Generic;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

namespace Sce.PlayStation.HighLevel.GameEngine2D
{
	/// <summary>
	/// The ActionManager is the singleton object that manages all Node's actions.
	/// Its main update loop is called inside Director.Update().
	/// </summary>
	public class ActionManager
	{
		List< ActionBase > m_cache = new List< ActionBase >();
		HashSet< Node > m_nodes = new HashSet< Node >();

		/// <summary>Add an action to the ActionManager.</summary>
		public void AddAction( ActionBase action, Node target /*, bool paused = false*/ )
		{
			// remove action from whatever list it is currently in
			RemoveAction( action ); 

			// add node to the set of nodes that have actions, 
			// new the node ActionBase list if necessary
			if ( !m_nodes.Contains( target ) )
			{
				m_nodes.Add( target );
				if ( target.m_action_entries == null )
					target.m_action_entries = new List< ActionBase >();
			}

			// add it to the new list
			action.set_target( target );
			target.m_action_entries.Add( action );
		}

		/// <summary>Remove all actions from the action manager.</summary>
		public void RemoveAllActions()
		{
			foreach ( Node node in m_nodes )
				remove_all_actions_from_target( node, false ); // set update_node_set so that we don't edit m_nodes as we iterate through it
			// we are going to clear it at the next line anyway 

			m_nodes.Clear();
		}

		/// <summary>Remove all actions involving Node 'target'.</summary>
		public void RemoveAllActionsFromTarget( Node target )
		{
			remove_all_actions_from_target( target, true );
		}

		// Remove all actions involving Node 'target'. If 'update_node_set' is true, remove 
		// node from m_nodes.
		public void remove_all_actions_from_target( Node target, bool update_node_set )
		{
			List< ActionBase > actions = target.m_action_entries;

			if ( actions != null )
			{
				foreach ( ActionBase action in actions )
				{
					action.Stop();
					action.set_target( null ); // "remove" Action from m_action_entries => nullify its target node
				}

				actions.Clear();
			}

			if ( update_node_set && m_nodes.Contains( target ) )
				m_nodes.Remove( target );
		}

		delegate bool DRemoveCondition( ActionBase a );

		void remove_action_from_target_if( Node target, DRemoveCondition cond )
		{
			List< ActionBase > actions = target.m_action_entries;

			if ( actions == null )
				return;

			for ( int i=0; i< actions.Count; ++i )
			{
				if ( cond( actions[i] ) )
				{
					actions[i].Stop();
					actions[i].set_target( null ); // "remove" Action from m_action_entries => nullify its target node
					actions.RemoveAt( i );
					break;
				}
			}

			if ( actions.Count == 0 )
				m_nodes.Remove( target );
		}

		/// <summary>Remove a single action.</summary>
		public void RemoveAction( ActionBase action )
		{
			action.Stop();

			if ( action.Target == null )
				return;

			remove_action_from_target_if( action.Target, ( a ) => a == action );
		}

		/// <summary>Find an action from tag and remove it.</summary>
		public void RemoveActionByTag( int tag, Node target )
		{
			remove_action_from_target_if( target, ( a ) => a.Tag == tag );
		}

		/// <summary>Get an action by tag.</summary>
		public ActionBase GetActionByTag( int tag, Node target, int ith = 0 )
		{
			List< ActionBase > actions = target.m_action_entries;

			if ( actions == null )
				return null;

			foreach ( ActionBase action in actions )
			{
				if ( action.Tag == tag && ( (ith--) == 0 ) )
					return action;
			}

			return null;
		}

		/// <summary>Count the number of actions involving Node 'target'.</summary>
		public int NumRunningActions( Node target )
		{
			List< ActionBase > actions = target.m_action_entries;

			if ( actions == null )
				return 0;

			int count=0;

			foreach ( ActionBase action in actions )
			{
				if ( action.IsRunning )
					++count;
			}
			return count;
		}

		/// <summary>count the total number of actions running.</summary>
		public int NumRunningActions()
		{
			int count=0;

			foreach ( Node node in m_nodes )
			{
				List< ActionBase > actions = node.m_action_entries;
				if ( actions == null )
					continue;

				foreach ( ActionBase action in actions )
					++count;
			}

			return count;
		}
/*
// use ActionsPaused directly
		public void PauseTarget( Node target )
		{
			target.ActionsPaused = true;
		}

		public void ResumeTarget( Node target )
		{
			target.ActionsPaused = false;
		}

		public bool IsTargetPaused( Node target )
		{
			return target.ActionsPaused;
		}
// 
*/
		public void Update( float dt )
		{
			m_cache.Clear();
			foreach ( Node node in m_nodes )
			{
				List< ActionBase > actions = node.m_action_entries;
				if ( actions == null )
					continue;

				for ( int i=0; i< actions.Count; )
				{
//					if ( )
//					{
//						actions[i] = actions[actions.Count-1];
//						actions.RemoveAt( actions.Count - 1 );
//					}
//					else
					{
						m_cache.Add( actions[i] ) ;
						++i;
					}
				}
			}

			foreach ( ActionBase action in m_cache )
			{
				if ( action.IsRunning == false)
					continue;
				
				if ( !action.Target.ActionsPaused )
					action.Update( dt );

				if ( action.IsRunning == false )
					RemoveAction( action );
			}
		}

		/// <summary>Print some debug information, content might vary in the future.</summary>
		public void Dump()
		{
			string prefix = Common.FrameCount + " ActionManager: ";

			int count=0;

			foreach ( Node node in m_nodes )
			{
				List< ActionBase > actions = node.m_action_entries;
				if ( actions == null )
					continue;

				System.Console.WriteLine( prefix + node.DebugInfo() );

				foreach ( ActionBase action in actions )
				{
					if ( action == null )
						System.Console.WriteLine( prefix + "\tnull" );
					else
						System.Console.WriteLine( prefix + "\t" + action.GetType().ToString() );

					++count;
				}
			}

			System.Console.WriteLine( prefix + "(" + count + " actions running)" );
		}

//		private static readonly ActionManager m_instance = new ActionManager();
		static internal ActionManager m_instance;

		/// <summary>The ActionManager singleton.</summary>
		public static ActionManager Instance
		{
			get { return m_instance; }
		}
	}
} // namespace Sce.PlayStation.HighLevel.GameEngine2D

