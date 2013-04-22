// Based on Cyclone physics engine by Ian Millington
// from the book Game Physics Engine Development
// http://procyclone.com
// and
// Jolt, a C# port of Cyclone, by Tim Jones
// https://github.com/roastedamoeba

using System;
using Sce.PlayStation.Core;

namespace PsmFramework.Engines.CyclonePhysics3d
{
	public abstract class Particle3dBase
	{
		#region Constructor
		
		public Particle3dBase(Vector3 position, Single mass, Single damping)
		{
			Position = position;
			Mass = mass;
			Damping = damping;
			Velocity = Vector3.Zero;
			Acceleration = Vector3.Zero;
		}
		
		#endregion
		
		#region Position
		
		public Vector3 Position { get; set; }
		
		#endregion
		
		#region Velocity
		
		public Vector3 Velocity { get; set; }
		
		#endregion
		
		#region Mass
		
		public Single Mass
		{
			get
			{
				return (InverseMass == 0.0f) ? Single.PositiveInfinity : 1.0f / InverseMass;
			}
			set
			{
				if (value == 0.0f)
					throw new ArgumentOutOfRangeException("value", "Mass cannot be 0.");
				InverseMass = 1.0f / value;
			}
		}
		
		public Single InverseMass { get; set; }
		
		public Boolean HasFiniteMass { get { return InverseMass >= 0.0f; } }
		
		#endregion
		
		#region Acceleration
		
		public Vector3 Acceleration { get; set; }
		
		#endregion
		
		#region Damping
		
		public Single Damping { get; set; }
		
		#endregion
		
		#region Integration
		
		public void Integrate(Single duration)
		{
			// Don't integrate things with zero mass.
			if (InverseMass <= 0.0f)
				return;
			
			if (duration <= 0.0f)
				throw new ArgumentOutOfRangeException("duration", "Duration must be greater than 0.");
			
			// Update linear position.
			Position += Velocity * duration;
			
			// Calculate the acceleration from the force.
			Vector3 resultingAcceleration = Acceleration;
			resultingAcceleration += ForceAccumulator * InverseMass;
			
			// Update linear velocity from the acceleration.
			Velocity += resultingAcceleration * duration;
			
			// Impose drag.
			Velocity *= (Single)System.Math.Pow(Damping, duration);
			
			// Clear the forces.
			ClearAccumulator();
		}
		
		#endregion
		
		#region Forces
		
		private Vector3 ForceAccumulator;
		
		public void AddForce(Vector3 force)
		{
			ForceAccumulator += force;
		}
		
		private void ClearAccumulator()
		{
			ForceAccumulator = Vector3.Zero;
		}
		
		#endregion
	}
}