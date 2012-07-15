// Based on Cyclone physics engine by Ian Millington
// from the book Game Physics Engine Development
// http://procyclone.com
// and
// Jolt, a C# port of Cyclone, by Tim Jones
// https://github.com/roastedamoeba

using System;
using System.Collections.Generic;
using PsmFramework.Engines.CyclonePhysics3d.Forces;

namespace PsmFramework.Engines.CyclonePhysics3d
{
	public class ParticleWorld3dBase
	{
		private readonly List<Particle3dBase> _Particles;
		private readonly List<ForceGenerator3dBase> _ForceGenerators;

		public ParticleWorld3dBase()
		{
			_Particles = new List<Particle3dBase>();
			_ForceGenerators = new List<ForceGenerator3dBase>();
		}

		public void AddParticle(Particle3dBase particle)
		{
			_Particles.Add(particle);
		}

		public void AddParticleForceGenerator(ForceGenerator3dBase forceGenerator)
		{
			_ForceGenerators.Add(forceGenerator);
		}

		public void UpdateForces(Single duration)
		{
			foreach (var forceGenerator in _ForceGenerators)
				forceGenerator.UpdateForce(duration);
		}
	}
}