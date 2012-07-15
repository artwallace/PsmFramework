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
	public class AnchoredSpring3d : ForceGenerator3dBase
	{
		protected Sce.PlayStation.Core.Vector3 Anchor { get; private set; }
		protected Single SpringConstant { get; private set; }
		protected Single RestLength { get; private set; }

		public AnchoredSpring3d(Particle3dBase particle, Vector3 anchor, Single springConstant, Single restLength)
			: base(particle)
		{
			Anchor = anchor;
			SpringConstant = springConstant;
			RestLength = restLength;
		}

		public override void UpdateForce(Single duration)
		{
			// Calculate the vector of the spring.
			Vector3 force = Particle.Position - Anchor;

			// Calculate the magnitude of the force.
			Single magnitude = force.Length();
			magnitude = (RestLength - magnitude) * SpringConstant;

			// Calculate the final force and apply it.
			force.Normalize();
			force *= -magnitude;
			Particle.AddForce(force);
		}
	}
}