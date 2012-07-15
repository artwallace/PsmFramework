// Based on Cyclone physics engine by Ian Millington
// from the book Game Physics Engine Development
// http://procyclone.com
// and
// Jolt, a C# port of Cyclone, by Tim Jones
// https://github.com/roastedamoeba

using System;

namespace PsmFramework.Engines.CyclonePhysics3d.Forces
{
	public abstract class ForceGenerator3dBase
	{
		public Particle3dBase Particle { get; private set; }

		protected ForceGenerator3dBase(Particle3dBase particle)
		{
			Particle = particle;
		}

		public abstract void UpdateForce(Single duration);
	}
}