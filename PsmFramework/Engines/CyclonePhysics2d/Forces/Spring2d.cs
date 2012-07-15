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
	public class Spring2d : ForceGenerator2dBase
	{
		private readonly Particle2dBase _other;
		private readonly Single _springConstant;
		private readonly Single _restLength;
		
		public Spring2d(Particle2dBase particle, Particle2dBase other, Single springConstant, Single restLength) 
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
			
			// Calculate the magnitude of the force.
			Single magnitude = force.Length();
			magnitude = Math.Abs(magnitude - _restLength);
			magnitude *= _springConstant;
			
			// Calculate the final force and apply it.
			force.Normalize();
			force *= -magnitude;
			Particle.AddForce(force);
		}
	}
}