// Based on Cyclone physics engine by Ian Millington
// from the book Game Physics Engine Development
// http://procyclone.com
// and
// Jolt, a C# port of Cyclone, by Tim Jones
// https://github.com/roastedamoeba

using System;
using System.Collections.Generic;
using PsmFramework.Engines.CyclonePhysics2d.Forces;

namespace PsmFramework.Engines.CyclonePhysics2d
{
	public abstract class ParticleWorld2dBase
	{
		private readonly List<Particle2dBase> _Particles;
		private readonly List<ForceGenerator2dBase> _ForceGenerators;
		
		public ParticleWorld2dBase()
		{
			_Particles = new List<Particle2dBase>();
			_ForceGenerators = new List<ForceGenerator2dBase>();
		}
		
		public void AddParticle(Particle2dBase particle)
		{
			_Particles.Add(particle);
		}
		
		public void AddForceGenerator(ForceGenerator2dBase forceGenerator)
		{
			_ForceGenerators.Add(forceGenerator);
		}
		
		public void UpdateForces(Single duration)
		{
			foreach (ForceGenerator2dBase forceGenerator in _ForceGenerators)
				forceGenerator.UpdateForce(duration);
		}
	}
}