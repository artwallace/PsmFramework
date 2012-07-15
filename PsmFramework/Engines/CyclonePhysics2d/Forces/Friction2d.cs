//TODO: This isn't really how friction should be applied.

using System;
using Sce.PlayStation.Core;

namespace PsmFramework.Engines.CyclonePhysics2d.Forces
{
	//TODO: This isn't how friction is supposed to work in Cyclone but I'm not coding collisions.
	public class Friction2d : ForceGenerator2dBase
	{
		public Friction2d(Particle2dBase particle)
			: base(particle)
		{
		}
		
		public override void UpdateForce(Single duration)
		{
			// Check that we do not have infinite mass.
			if (!Particle.HasFiniteMass)
				return;
			
			//TODO: I just made this number up. Remove hardcoding.
			Particle.AddForce(Particle.Velocity * -1f);
		}
	}
}

