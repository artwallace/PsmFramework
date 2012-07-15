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
	public class Bungee2d : ForceGenerator2dBase
	{
		private readonly Particle2dBase _other;
		private readonly Single _springConstant;
		private readonly Single _restLength;

		public Bungee2d(Particle2dBase particle, Particle2dBase other, Single springConstant, Single restLength) 
			: base(particle)
		{
			_other = other;
			_springConstant = springConstant;
			_restLength = restLength;
		}

		public override void UpdateForce(Single duration)
		{
			// Calculate the vector of the spring.
			Vector2 force = Particle.Position - _other.Position;

			// Check if the bungee is compressed.
			Single magnitude = force.Length();
			if (magnitude <= _restLength)
				return;

			// Calculate the magnitude of the force.
			magnitude = _springConstant * (_restLength - magnitude);

			// Calculate the final force and apply it.
			force.Normalize();
			force *= -magnitude;
			Particle.AddForce(force);
		}
	}
}