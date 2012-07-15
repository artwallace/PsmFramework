using System;
using Sce.PlayStation.Core;

namespace PsmFramework
{
	public class RandomGenerator : IDisposable
	{
		#region Constructor, Dispose
		
		public RandomGenerator(Int32 seed = 0)
		{
			Initialize(seed);
		}
		
		public void Dispose()
		{
			//Dispose doesn't really do anything in this class but whatever.
			Cleanup();
		}
		
		#endregion
		
		#region Initialize, Cleanup
		
		public void Initialize(Int32 seed)
		{
			InitializeRandomBase(seed);
		}
		
		public void Cleanup()
		{
			CleanupRandomBase();
		}
		
		#endregion
		
		#region Random Base
		
		private void InitializeRandomBase(Int32 seed)
		{
			RandomBase = new Random(seed);
		}
		
		private void CleanupRandomBase()
		{
			RandomBase = null;
		}
		
		public Random RandomBase;
		
		#endregion
		
		#region Single
		
		public Single NextSingle_ZeroToOne()
		{
			return (Single)RandomBase.NextDouble();
		}
		
		public Single NextSingle_NegativeOneToOne()
		{
			return (NextSingle_ZeroToOne() * 2.0f) - 1.0f;
		}
		
		public Single NextSingle(Single min, Single max)
		{
			return min + (max - min) * NextSingle_ZeroToOne();
		}
		
		#endregion
		
		#region Vectors
		
		public Vector2 NextVector2_NegativeOneToOne()
		{
			return new Vector2(
				NextSingle(-1.0f, 1.0f),
				NextSingle(-1.0f, 1.0f)
				);
		}
		
		public Vector2 NextVector2(Vector2 mi, Vector2 ma)
		{
			return new Vector2(
				NextSingle(mi.X, ma.X),
				NextSingle(mi.Y, ma.Y)
				);
		}
		
		public Vector2 NextVector2(Single mi, Single ma)
		{
			return new Vector2(
				NextSingle(mi, ma),
				NextSingle(mi, ma)
				);
		}
		
		public Vector3 NextVector3(Vector3 mi, Vector3 ma)
		{
			return new Vector3(
				NextSingle(mi.X, ma.X),
				NextSingle(mi.Y, ma.Y),
				NextSingle(mi.Z, ma.Z)
				);
		}
		
		public Vector4 NextVector4(Vector4 mi, Vector4 ma)
		{
			return new Vector4(
				NextSingle(mi.X, ma.X),
				NextSingle(mi.Y, ma.Y),
				NextSingle(mi.Z, ma.Z),
				NextSingle(mi.W, ma.W)
				);
		}
		
		public Vector4 NextVector4(Single mi, Single ma)
		{
			return new Vector4(
				NextSingle(mi, ma),
				NextSingle(mi, ma),
				NextSingle(mi, ma),
				NextSingle(mi, ma)
				);
		}
		
		#endregion
		
		#region Coordinates from DrawEngine2d
		#endregion
		
		#region Colors from DrawEngine2d
		#endregion
	}
}

