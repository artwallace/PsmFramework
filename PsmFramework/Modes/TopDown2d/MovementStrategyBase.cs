using System;

namespace PsmFramework.Modes.TopDown2d
{
	//Remaining to implement:
	//Follow,
	//Orbit,
	//Bodyguard,
	//SentryFromPoint,
	//PatrolInBounds,
	//Hunter,
	//PresetPath,
	//DeathSpasm
	
	public abstract class MovementStrategyBase
	{
		protected Actor Actor;
		protected RoomBase Room { get { return Actor.Room; } }
		protected AppManager Mgr { get { return Actor.Mgr; } }
		
		#region Constructor, Dispose
		
		public MovementStrategyBase(Actor actor)
		{
			Actor = actor;
			Initialize();
		}
		
		public void Dispose()
		{
			Cleanup();
			Actor = null;
		}
		
		#endregion
		
		#region Initialize, Cleanup
		
		public virtual void Initialize()
		{
		}
		
		public virtual void Cleanup()
		{
		}
		
		#endregion
		
		#region Move
		
		public abstract void Move();
		
		#endregion
	}
}

