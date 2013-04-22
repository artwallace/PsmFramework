using System;
using Sce.PlayStation.Core;

namespace PsmFramework.Modes.TopDown2d
{
	public class PlayerInputMovementStrategy : MovementStrategyBase
	{
		private Boolean UseLeftAnalog;
		private Boolean UseDPad;
		
		#region Constructor
		
		public PlayerInputMovementStrategy(Actor actor, Boolean useLeftAnalog, Boolean useDPad)
			: base(actor)
		{
			UseLeftAnalog = useLeftAnalog;
			UseDPad = useDPad;
		}
		
		#endregion
		
		#region Move
		
		public override void Move()
		{
			if (UseLeftAnalog)
				Move_LeftAnalog();
			if (UseDPad)
				Move_DPad();
		}
		
		private void Move_LeftAnalog()
		{
			if(Mgr.GamePad0_LeftStick_X > 0.1f || Mgr.GamePad0_LeftStick_X < -0.1f)
			{
				//Testing.
			}
		}
		
		private void Move_DPad()
		{
			//TODO: Need 
			if (Mgr.GamePad0_Up)
				Actor.AddForce(Actor.Heading.Perpendicular().Multiply(1.5f));
			else if (Mgr.GamePad0_Down)
				Actor.AddForce(Actor.Heading.Perpendicular().Negate());
			
			if (Mgr.GamePad0_Left)
				Actor.AddRotationToHeading(0.0000011f * Mgr.TimeSinceLastFrame.Ticks);
			else if (Mgr.GamePad0_Right)
				Actor.AddRotationToHeading(-0.0000011f * Mgr.TimeSinceLastFrame.Ticks);
		}
		
		#endregion
	}
}

