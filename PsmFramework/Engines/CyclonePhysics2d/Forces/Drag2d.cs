// Based on Cyclone physics engine by Ian Millington
// from the book Game Physics Engine Development
// http://procyclone.com
// and
// Jolt, a C# port of Cyclone, by Tim Jones
// https://github.com/roastedamoeba

using System;
using Sce.PlayStation.Core;

namespace PsmFramework.Engines.CyclonePhysics2d.Forces
{
	public class Drag2d : ForceGenerator2dBase
	{
		private readonly Single _k1;
		private readonly Single _k2;

		public Drag2d(Particle2dBase particle, Single k1, Single k2)
			: base(particle)
		{
			_k1 = k1;
			_k2 = k2;
		}

		public override void UpdateForce(Single duration)
		{
			Vector2 force = Particle.Velocity;

			// Calculate the total drag coefficient.
			Single dragCoefficient = force.Length();
			dragCoefficient = _k1 * dragCoefficient + _k2 * dragCoefficient * dragCoefficient;

			// Calculate the final force and apply it.
			force.Normalize();
			force *= -dragCoefficient;
			Particle.AddForce(force);
		}
	}
}