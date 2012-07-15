/* SCE CONFIDENTIAL
 * PlayStation(R)Suite SDK 0.98.2
 * Copyright (C) 2012 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System.IO;

namespace Sce.PlayStation.HighLevel.GameEngine2D.Base
{
	public static class Common
	{
		public class AssertFailed : System.Exception
		{
		}

//		[Conditional("DEBUG")]
		public static void Assert( bool cond, string message )
		{
			if ( !cond )
			{
				System.Console.WriteLine( " *** error: " + message );
				throw new AssertFailed();
			}
		}

//		[Conditional("DEBUG")]
		public static void Assert( bool cond )
		{
			if ( !cond )
			{
				throw new AssertFailed();
			}
		}

		private static uint g_frame_counter = 0;

		/// <summary>Global frame counter.</summary>
		public static 
		uint FrameCount { get { return g_frame_counter; } }

		public static 
		void OnSwap()
		{
			++g_frame_counter;
		}

		/// <summary>
		/// Loop index in [0,n-1], works with negative indexes too (-1 returns n-1 etc).
		/// Note that n is a size (for example, the size of an array object) and therefore its value is assumed to be > 0.
		/// If n is negative or zero the result is undefined.
		/// </summary>
		public static int WrapIndex( int i, int n )
		{
			return((i=(i%n))<0?i+n:i);
		}
		
		/// <summary>
		/// Clamp index in [0,n-1]
		/// Same as Clamp( i, 0, n-1 ).
		/// </summary>
		public static int ClampIndex( int i, int n )
		{
			return i<0?0:(i>n-1?n-1:i);
		}

		public static int Clamp( int i, int min, int max )
		{
			return i<min?min:(i>max?max:i);
		}

		public static int Min( int i, int value )
		{
			return i < value ? i : value;
		}

		public static int Max(int i, int value)
		{
			return i > value ? i : value;
		}

		public delegate int IndexWrapMode( int i, int n );

		static 
		public void Swap<T>(ref T a, ref T b)
		{
			T tmp = a;
			a = b;
			b = tmp;
		}

		public static Profiler Profiler = new Profiler();

		/// <summary>
		/// Get an embedded file's binary data, used for shaders mostly.
		/// </summary>
		 
		public static System.Byte[] GetEmbeddedResource( string filename )
		{
//			System.Console.WriteLine( "GetEmbeddedResource("+filename+")" );

			System.Reflection.Assembly resource_assembly = System.Reflection.Assembly.GetExecutingAssembly();

			//embedded filenames are prefixed with namespace and have / replaced to . apparently
			string embedded_filename = "PsmFramework.Engines.GameEngine2d.shaders." + filename.Replace("/", ".");
			if ( resource_assembly.GetManifestResourceInfo( embedded_filename ) == null )
			{
				// didn't find an embedded version of the file
				// list up all the filenames we had
				string[] all_embedded_names = resource_assembly.GetManifestResourceNames();
				for ( int i=0; i < all_embedded_names.Length; ++i )
					System.Console.WriteLine( "all_embedded_names[i]="+all_embedded_names[i] );

				System.Console.WriteLine( "embedded filename=" + resource_assembly );

				Common.Assert(false,filename);
				throw new FileNotFoundException("File not found.", filename);
			}
			else
			{
//				System.Console.WriteLine( "Found embedded file: " + filename );
			}

			Stream stream = resource_assembly.GetManifestResourceStream( embedded_filename );
			System.Byte[] data = new System.Byte[stream.Length];
			stream.Read( data, 0, data.Length );
			return data;
		}
		
		// Wrap the creation of shader programs so we can switch bewteen file/embedded data.
		public static Sce.PlayStation.Core.Graphics.ShaderProgram CreateShaderProgram( string filename )
		{
//			System.Console.WriteLine( "CreateShaderProgram("+filename+")" );

			// try load embedded version
			System.Byte[] shaderfile = Common.GetEmbeddedResource( filename );
			Sce.PlayStation.Core.Graphics.ShaderProgram ret = new Sce.PlayStation.Core.Graphics.ShaderProgram( shaderfile );

			//try load from disk
//			ret = new Sce.PlayStation.Core.Graphics.ShaderProgram( filename );

			return ret;
		}
		
		public static void DisposeAndNullify<T>( ref T a )
		{
			if ( a == null )
				return;
			
//			System.Console.WriteLine( "DisposeAndNullify: " + a );
			
			((System.IDisposable)a).Dispose();
			a = default(T);
		}

//		static 
//			public void DisposeAndNullifyIfIDisposable< T >( ref T a )
//		{
//			if ( a == null )
//				return;
//
//			System.Type[] types = a.GetType().GetInterfaces();
//			foreach ( System.Type type in types )
//			{
//				if ( type != typeof(System.IDisposable) ) 
//					continue;
//
//				((System.IDisposable)a).Dispose();
//				a = default(T);
//				break;
//			}
//		}
	}
}

