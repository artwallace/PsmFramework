using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PsmFramework.Engines.CyclonePhysics2d;
using Sce.PlayStation.Core;
using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

namespace PsmFramework.Modes.TopDown2d
{
	//TODO: Bad guys radio for help.
	//TODO: Add enemy perception zones with differenct sensitivities, such as blind from rear unless attacked.
	//TODO: Add time effects, so that unit experiences time at different rate than rest of game. ticksSinceLastUpdate * timeRate
	public class Actor : Particle2dBase, IDisposable
	{
		public RoomBase Room { get; private set; }
		protected LevelBase Level { get { return Room.Level; } }
		protected TopDown2dModeBase Mode { get { return Room.Level.Mode; } }
		public AppManager Mgr { get { return Room.Level.Mode.Mgr; } }
		
		#region Constructor, Dispose
		
		public Actor(RoomBase room, String spriteSrc, Vector2 position, Single mass, Single damping)
			: base(position, mass, damping)
		{
			if (room == null)
				throw new ArgumentNullException();
			if (String.IsNullOrWhiteSpace(spriteSrc))
				throw new ArgumentNullException();
			
			Room = room;
			
			Initialize(spriteSrc);
		}
		
		public virtual void Dispose()
		{
			Cleanup();
			
			Room = null;
		}
		
		#endregion
		
		#region Initialize, Cleanup
		
		private void Initialize(String spriteSrc)
		{
			InitializePhysics();
			InitializeSprite(spriteSrc);
			
			//TODO: Remove these!
			//SetPosition(new Vector2(100f, 100f));
			SetHeading(new Vector2(1f, 0f));
		}
		
		private void Cleanup()
		{
			CleanupPhysics();
			CleanupSprite();
		}
		
		#endregion
		
		#region Debug Info
		
		private StringBuilder DebugInfo = new StringBuilder();
		
		public String GetDebugInfo()
		{
			DebugInfo.Clear();
			//DebugInfo.Append("Heading: ");
			//DebugInfo.AppendLine(Heading.ToString());
			return DebugInfo.ToString();
		}
		
		#endregion
		
		#region Sprite
		
		//TODO: Add support for SpriteLists!!!!!!!
		
		public SpriteUV Sprite { get; private set; }
		
		private void InitializeSprite(String spriteSrc)
		{
			Sprite = Mode.TextureManager.CreateSpriteUV(spriteSrc);
			//InScene = true;
		}
		
		private void CleanupSprite()
		{
			//TODO: Sprite.Cleanup necessary?
			Sprite.Cleanup();
			Sprite = null;
		}
		
//		protected Boolean _InScene;
//		protected Boolean InScene
//		{
//			get { return _InScene; }
//			set
//			{
//				_InScene = value;
//				
//				if (_InScene)
//					Mode.AddToScene(Sprite);
//				else
//					Mode.RemoveFromScene(Sprite);
//			}
//		}
		
		#endregion
		
		#region Physics
		
		private void InitializePhysics()
		{
//			InitializeMassEffects();
//			InitializeFrictionEffects();
		}
		
		private void CleanupPhysics()
		{
//			CleanupMassEffects();
//			CleanupFrictionEffects();
		}
		
//		#region Position
//		
//		public Vector2 Position { get; private set; }
//		
		public void SetPosition(Vector2 position)
		{
			Position = position;
		}
//		
//		#endregion
		
		#region Heading
		
		public Vector2 Heading { get; private set; }
		
		public void SetHeading(Vector2 heading)
		{
			Heading = heading;
		}
		
		public void GenerateRandomHeading()
		{
			SetHeading(Mgr.RandomGenerator.NextVector2(-1f, 1f));
		}
		
		public void AddRotationToHeading(Single rotation)
		{
			Heading = Heading.Rotate(rotation);
		}
		
		#endregion
		
//		#region Velocity
//		
//		public Vector2 Velocity { get; private set; }
//		
//		public void AddMotionToVelocity(Vector2 motion)
//		{
//			Position = Position.Add(motion);
//		}
//		
//		public void AddImpulse(Vector2 movement)
//		{
//		}
//		
//		#endregion
		
//		#region AngularVelocity
//		
//		
//		
//		#endregion
		
//		#region Mass
//		
//		public Single Mass { get; private set; }
//		
//		//public List<MassEffect> MassEffects { get; private set; }
//		
//		public void InitializeMassEffects()
//		{
//			//MassEffects = new List<MassEffect>();
//		}
//		
//		public void CleanupMassEffects()
//		{
//			//MassEffects.ForEach(e => { e.Dispose(); } );
//			//MassEffects.Clear();
//			//MassEffects = null;
//		}
//		
//		public void AddMassEffect(Actor source, String name, Int32 num, Int32 duration)
//		{
//			//MassEffect m = new MassEffect(source, name, num, duration);
//			//MassEffects.Add(m);
//			CalculateMassEffects();
//		}
//		
//		public void RemoveMassEffect(Actor source)
//		{
//			//IEnumerable<MassEffect> removals =
//			//	from e in MassEffects
//			//	where e.Source == source
//			//	select e;
//			//foreach (MassEffect e in removals)
//			//{
//			//	MassEffects.Remove(e);
//			//	e.Dispose();
//			//}
//			
//			CalculateMassEffects();
//		}
//		
//		public void RemoveExpiredMassEffects()
//		{
//			//IEnumerable<MassEffect> removals =
//			//	from e in MassEffects
//			//	where e.TicksRemaining == 0
//			//	select e;
//			//foreach (MassEffect e in removals)
//			//{
//			//	MassEffects.Remove(e);
//			//	e.Dispose();
//			//}
//		}
//		
//		public void CalculateMassEffects()
//		{
//			Mass = 0;
//			//MassEffects.ForEach(e => { Mass += e.Number; } );
//		}
//		
//		#endregion
		
//		#region Friction
//		
//		//public List<FrictionEffect> FrictionEffects { get; private set; }
//		
//		public void InitializeFrictionEffects()
//		{
//			//FrictionEffects = new List<FrictionEffect>();
//		}
//		
//		public void CleanupFrictionEffects()
//		{
//			//FrictionEffects.ForEach(e => { e.Dispose(); } );
//			//FrictionEffects.Clear();
//			//FrictionEffects = null;
//		}
//		
//		#endregion
		
//		#region Momentum
//		
//		public Vector2 Momentum
//		{
//			get { return Velocity.Multiply(Mass); }
//		}
//		
//		#endregion
		
		//Acceleration
		//Force
		//Inertia
		//Torque
		
		#endregion
		
		#region MovementStrategy
		
		public MovementStrategyBase MovementStrategy { get; private set; }
		
		public void SetMovementStrategy(MovementStrategyBase strategy)
		{
			MovementStrategy = strategy;
		}
		
		#endregion
		
		#region Update
		
		//TODO: Separate movement and integration.
		public void Update()
		{
			if (MovementStrategy != null)
				MovementStrategy.Move();
			
			Integrate(Mgr.TimeSinceLastFrame.Ticks);
			
			Sprite.Position = Position;
			Sprite.Rotation = Heading;
		}
		
		#endregion
		
		#region Camera
		
		//TODO: This should be adjusted so that when an actor is moving quickly,
		//the camera is positioned in front of the actor so that the player can see
		//more of what is in front of them.
		public Vector2 CameraPostion
		{
			get
			{
				return Position;
			}
		}
		
		#endregion
	}
}

