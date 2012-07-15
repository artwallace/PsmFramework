/* SCE CONFIDENTIAL
 * PlayStation(R)Suite SDK 0.98.2
 * Copyright (C) 2012 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;

namespace Sce.PlayStation.HighLevel.GameEngine2D.Base
{
/*
	/// <summary>
	/// An axis aligned box class in 2D (integers).
	/// </summary>
	public struct Bounds2i
	{
		/// <summary>Minimum point (lower left).</summary>
		public Vector2i Min;

		/// <summary>Maximum point (upper right)</summary>
		public Vector2i Max;

		/// <summary>The Width/Height ratio.</summary>
		public float Aspect { get { Vector2 size = Size.Vector2(); return size.X / size.Y;}} 

		/// <summary>The center of the bounds (Max+Min)/2.</summary>
		public Vector2 Center { get { return( Max + Min ).Vector2() * 0.5f;}} 

		/// <summary>The Size the bounds (Max-Min).</summary>
		public Vector2i Size { get { return( Max - Min );}} 

		/// <summary>Return true if the size is (0,0).</summary>
		public bool IsEmpty()
		{
			return Max == Min;
		}

		/// <summary>
		/// Bounds2i constructor.
		/// All functions in Bounds2i assume that Min is less or equal Max. If it is not the case, the user takes responsability for it.
		/// SafeBounds will ensure this is the case whatever the input is, but the default constructor will just blindly
		/// takes anything the user passes without trying to fix it.
		/// </summary>
		/// <param name="min">The bottom left point. Min is set to that value without further checking.</param>
		/// <param name="max">The top right point. Max is set to that value without further checking.</param>
		public Bounds2i( Vector2i min, Vector2i max )
		{
			Min = min; 
			Max = max;
		}

		/// <summary>
		/// Bounds2i constructor. 
		/// Return a zero size bounds. You can then use Add to expand it. 
		/// </summary>
		/// <param name="point">Location of the Bounds2.</param>
		public Bounds2i( Vector2i point )
		{
			Min = point; 
			Max = point;
		}

		/// <summary>
		/// Create a Bounds2i that goes through 2 points, the min and max are recalculated.
		/// </summary>
		/// <param name="min">First point.</param>
		/// <param name="max">Second point.</param>
		static public Bounds2i SafeBounds( Vector2i min, Vector2i max )
		{
			return new Bounds2i( min.Min( max ), min.Max( max ) );
		}

		/// <summary>(0,0) -> (0,0) box.</summary>
		static public Bounds2i Zero = new Bounds2i( new Vector2i(0,0), new Vector2i(0,0) );

		/// <summary>Translate bounds.</summary>
		public static Bounds2i operator + ( Bounds2i bounds, Vector2i value )
		{
			return new Bounds2i( bounds.Min + value, 
								bounds.Max + value );
		}

		/// <summary>Translate bounds.</summary>
		public static Bounds2i operator - ( Bounds2i bounds, Vector2i value )
		{
			return new Bounds2i( bounds.Min - value, 
								bounds.Max - value );
		}

		/// <summary>Return true if this and 'bounds' overlap.</summary>
		public bool Overlaps( Bounds2i bounds )
		{
			if ( Min.X > bounds.Max.X || bounds.Min.X > Max.X )	return false;
			if ( Min.Y > bounds.Max.Y || bounds.Min.Y > Max.Y )	return false;

			return true;
		}

		/// <summary>Return the Bounds2i resulting from the intersection of 2 bounds.</summary>
		public Bounds2i Intersection( Bounds2i bounds ) 
		{
			Vector2i mi = Min.Max( bounds.Min );
			Vector2i ma = Max.Min( bounds.Max );

			Vector2i dim = ma - mi;

			if ( dim.X < 0.0f || dim.Y < 0.0f )
				return Zero;

			return new Bounds2i( mi, ma );
		} 

		/// <summary>Add the contribution of 'point' to this Bounds2.</summary>
		public void Add( Vector2i point )
		{
			Min = Min.Min( point );
			Max = Max.Max( point );
		}

		/// <summary>Add the contribution of 'bounds' to this Bounds2.</summary>
		public void Add( Bounds2i bounds )
		{
			Add( bounds.Min );
			Add( bounds.Max );
		}

		// Note about PointXX: first column is x, second is y (0 means min, 1 means max, you can also see those as 'uv')

		/// <summary>The botton left point (which is also Min).</summary>
		public Vector2i Point00 { get { return Min;}}
		/// <summary>The top right point (which is also Max).</summary>
		public Vector2i Point11 { get { return Max;}}
		/// <summary>The bottom right point.</summary>
		public Vector2i Point10 { get { return new Vector2i(Max.X,Min.Y);}}
		/// <summary>The top left point.</summary>
		public Vector2i Point01 { get { return new Vector2i(Min.X,Max.Y);}}

		/// <summary>Return the string representation of this Bounds2.</summary>
		public override string ToString()
		{
			return Min.ToString() + " " + Max.ToString();
		}


		public bool IsInside( Vector2i point )
		{
			return point == point.Max( Min ).Min( Max );
		}
	}
*/
}

