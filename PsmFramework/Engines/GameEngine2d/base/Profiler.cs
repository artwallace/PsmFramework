/* SCE CONFIDENTIAL
 * PlayStation(R)Suite SDK 0.98.2
 * Copyright (C) 2012 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

//using System; // C# Action will conflict with ours
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;

using Sce.PlayStation.Core;

namespace Sce.PlayStation.HighLevel.GameEngine2D.Base
{
	// An ad hoc profiler (high overhead)
	public class Profiler
	{
		public class Node
		{
			public string Name;
			public float Duration;
			public int Depth;
			public Timer Timer;
		}

		int m_nodes_size = 0;
		List<Node> m_nodes = new List<Node>();
		List<Node> m_stack = new List<Node>();

		public void HeartBeat()
		{
			m_stack.Clear();
			m_nodes_size = 0;
		}

		public void Push( string name )
		{
			if ( m_nodes_size >= m_nodes.Count )
				m_nodes.Add( new Node(){ Timer = new Timer() } );

			Node node = m_nodes[ m_nodes_size ];

			node.Name = name;
			node.Depth = m_stack.Count;
			node.Timer.Reset();

			m_stack.Add( node );

			++m_nodes_size;
		}

		public void Pop()
		{
			m_stack[m_stack.Count-1].Duration = (float)m_stack[m_stack.Count-1].Timer.Milliseconds();

			m_stack.RemoveAt( m_stack.Count-1 );
		}

		public void Dump()
		{
			System.Console.WriteLine( "" );
			System.Console.WriteLine( "--- frame " + Common.FrameCount + "'s timers:" );

			Common.Assert( m_stack.Count == 0, "number of Profiler Push/Push doesn't match" );

			for ( int i=0; i < m_nodes_size; ++i )
				System.Console.WriteLine( new System.String('\t',m_nodes[i].Depth) + m_nodes[i].Name + " " + m_nodes[i].Duration + " ms" );

			Dictionary< string, float > timers_totals = new Dictionary< string, float >();
			Dictionary< string, int > timers_count = new Dictionary< string, int >();
			for ( int i=0; i < m_nodes_size; ++i )
			{
				Node node = m_nodes[i];

				if ( !timers_totals.ContainsKey( node.Name ) )
				{
					timers_totals.Add( node.Name, 0.0f );
					timers_count.Add( node.Name, 0 );
				}

				timers_totals[ node.Name ] += node.Duration;
				timers_count[ node.Name ] += 1;
			}

			System.Console.WriteLine( "" );
			System.Console.WriteLine( "--- frame " + Common.FrameCount + "'s timers totals:" );

			foreach ( KeyValuePair<string, float> kvp in timers_totals )
			{
				System.Console.WriteLine("total for {0} = {1} ms ({2} calls)", kvp.Key, kvp.Value, timers_count[kvp.Key] );
			}
		}
	}
}

