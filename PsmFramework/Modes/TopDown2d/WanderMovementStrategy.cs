using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;
using PssMath = Sce.PlayStation.HighLevel.GameEngine2D.Base.Math;

namespace PsmFramework.Modes.TopDown2d
{
	public class WanderMovementStrategy : MovementStrategyBase
	{
		private Bounds2 Bounds;
		
		private Int32 MinDuration;
		private Int32 MaxDuration;
		
		private Int64 DurationInTicks;
		
		#region Constructor
		
		public WanderMovementStrategy(Actor actor, Int32 minDuration, Int32 maxDuration)
			: base(actor)
		{
			MinDuration = minDuration;
			MaxDuration = maxDuration;
		}
		
		public WanderMovementStrategy(Actor actor, Int32 minDuration, Int32 maxDuration, Bounds2 bounds)
			: base(actor)
		{
			MinDuration = minDuration;
			MaxDuration = maxDuration;
			Bounds = bounds;
		}
		
		#endregion
		
		#region Move
		
		public override void Move()
		{
			Boolean needNewDuration = false;
			Boolean needNewHeading = false;
			
			if (DurationInTicks <= 0)
			{
				needNewHeading = true;
				needNewDuration = true;
			}
			else
				DurationInTicks -= Mgr.TimeSinceLastFrame.Ticks;
			
			//TODO: if collision, needNewHeading = true;
			
			if (!Bounds.IsInside(Actor.Position))
			{
				needNewHeading = true;
				needNewDuration = true;
				
				Vector2 newPstn;
				Single sign;
				Bounds.ClosestSurfacePoint(Actor.Position, out newPstn, out sign);
				Actor.SetPosition(newPstn);
			}
			
			if (needNewHeading)
				Actor.GenerateRandomHeading();
			if (needNewDuration)
				DurationInTicks = Mgr.RandomGenerator.RandomBase.Next(MinDuration, MaxDuration + 1);
			
			//TODO: Add Impulse!!!!
			
		}
		
		#endregion
	}
}

