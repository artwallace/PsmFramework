// Based on Cyclone physics engine by Ian Millington
// from the book Game Physics Engine Development
// http://procyclone.com
// and
// Jolt, a C# port of Cyclone, by Tim Jones
// https://github.com/roastedamoeba

using System;
using Sce.PlayStation.Core;

namespace PsmFramework.Engines.CyclonePhysics3d.Forces
{
	public class Gravity3d : ForceGenerator3dBase
	{
		private readonly Vector3 _gravity;

		public Gravity3d(Particle3dBase particle, Vector3 gravity)
			: base(particle)
		{
			_gravity = gravity;
		}

		public override void UpdateForce(Single duration)
		{
			// Check that we do not have infinite mass.
			if (!Particle.HasFiniteMass)
				return;

			// Apply the mass-scaled force to the particle.
			Particle.AddForce(_gravity * Particle.Mass);
		}
	}
}