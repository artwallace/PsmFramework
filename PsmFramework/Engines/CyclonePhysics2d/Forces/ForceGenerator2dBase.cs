// Based on Cyclone physics engine by Ian Millington
// from the book Game Physics Engine Development
// http://procyclone.com
// and
// Jolt, a C# port of Cyclone, by Tim Jones
// https://github.com/roastedamoeba

using System;

namespace PsmFramework.Engines.CyclonePhysics2d.Forces
{
	public abstract class ForceGenerator2dBase
	{
		public Particle2dBase Particle { get; private set; }
		
		protected ForceGenerator2dBase(Particle2dBase particle)
		{
			Particle = particle;
		}
		
		public abstract void UpdateForce(Single duration);
	}
}