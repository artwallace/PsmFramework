using PsmFramework.Modes.TopDown2d;

namespace Demo.TwinStickShooter
{
//	public class Level1 : LevelBase
//	{
//		#region Constructor, Dispose
//		
//		protected Level1(TopDown2dModeBase mode)
//			: base(mode)
//		{
//		}
//		
//		#endregion
//		
//		#region Level Logic
//		
//		protected override void Initialize()
//		{
//			GoToRoom(Room1Factory);
//		}
//		
//		protected override void Cleanup()
//		{
//		}
//		
//		protected override void Update()
//		{
//			if (ReadyToAdvanceToNextRoom)
//			{
//				ReadyToAdvanceToNextRoom = false;
//				GoToRoom(Room2Factory);
//			}
//		}
//		
//		#endregion
//		
//		#region Rooms
//		
//		public CreateRoomDelegate Room1Factory = Level1Room1.Level1Room1Factory;
//		
//		public CreateRoomDelegate Room2Factory = Level1Room2.Level1Room2Factory;
//		
//		#endregion
//		
//		#region Level1 Factory Delegate
//		
//		public static LevelBase Level1Factory(TopDown2dModeBase mode)
//		{
//			return new Level1(mode);
//		}
//		
//		#endregion
//	}
}
