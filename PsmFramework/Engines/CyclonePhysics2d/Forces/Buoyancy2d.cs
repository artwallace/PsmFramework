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
	public class Buoyancy2d : ForceGenerator2dBase
	{
		private readonly Single _maxDepth;
		private readonly Single _volume;
		private readonly Single _waterHeight;
		private readonly Single _liquidDensity;

		public Buoyancy2d(Particle2dBase particle, Single maxDepth, Single volume, Single waterHeight, Single liquidDensity = 1000.0f) 
			: base(particle)
		{
			_maxDepth = maxDepth;
			_volume = volume;
			_waterHeight = waterHeight;
			_liquidDensity = liquidDensity;
		}

		public override void UpdateForce(Single duration)
		{
			// Calculate the submersion depth.
			Single depth = Particle.Position.Y;

			// Check if we're out of the water.
			if (depth >= _waterHeight + _maxDepth)
				return;

			Vector2 force = Vector2.Zero;

			// Check if we're at maximum depth.
			if (depth <= _waterHeight - _maxDepth)
			{
				force.Y = _liquidDensity * _volume;
				Particle.AddForce(force);
				return;
			}

			// Otherwise we are partly submerged.
			force.Y = _liquidDensity * _volume * (depth - _maxDepth - _waterHeight) / 2 * _maxDepth;
			Particle.AddForce(force);
		}
	}
}