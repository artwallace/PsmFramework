using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace PsmFramework.Modes.TopDown2d
{
	public abstract class LevelBase : IDisposable
	{
		public TopDown2dModeBase Mode { get; private set; }
		
		#region Constructor, Dispose
		
		protected LevelBase(TopDown2dModeBase mode)
		{
			if (mode == null)
				throw new ArgumentNullException();
			
			Mode = mode;
			
			InitializeInternal();
		}
		
		public void Dispose()
		{
			CleanupInternal();
			
			Mode = null;
		}
		
		#endregion
		
		#region Initialize, Cleanup
		
		private void InitializeInternal()
		{
			InitializeRooms();
			
			Initialize();
		}
		
		private void CleanupInternal()
		{
			Cleanup();
			
			CleanupRooms();
		}
		
		#endregion
		
		#region Update
		
		internal void UpdateInternal()
		{
			//TODO: This is much too simple. Needs logic like used in AppManager ModeChange.
			Update();
			CurrentRoom.UpdateInternal();
		}
		
		#endregion
		
		#region Rooms
		
		public delegate RoomBase CreateRoomDelegate(LevelBase level);
		
		private RoomBase PreviousRoom { get; set; }
		private RoomBase CurrentRoom { get; set; }
		private RoomBase ReturnRoom { get; set; }
		
		private void InitializeRooms()
		{
			PreviousRoom = null;
			CurrentRoom = null;
			ReturnRoom = null;
		}
		
		private void CleanupRooms()
		{
			if (PreviousRoom != null)
			{
				PreviousRoom.Dispose();
				PreviousRoom = null;
			}
			
			if (CurrentRoom != null)
			{
				CurrentRoom.Dispose();
				CurrentRoom = null;
			}
			
			if (ReturnRoom != null)
			{
				ReturnRoom.Dispose();
				ReturnRoom = null;
			}
		}
		
		public Boolean RoomChanged
		{
			get { return PreviousRoom != null; }
		}
		
		public void GoToRoom(CreateRoomDelegate factory)
		{
			PreviousRoom = CurrentRoom;
			if (PreviousRoom != null)
				CleanupPreviousRoom();
			CurrentRoom = factory(this);
			ReturnRoom = null;
		}
		
		public void GoToRoomThenReturn(CreateRoomDelegate factory, RoomBase returnRoom)
		{
			PreviousRoom = CurrentRoom;
			CurrentRoom = factory(this);
			ReturnRoom = returnRoom;
		}
		
		public void ReturnToRoom()
		{
			PreviousRoom = CurrentRoom;
			CurrentRoom = ReturnRoom;
			ReturnRoom = null;
		}
		
		private void CleanupPreviousRoom()
		{
			PreviousRoom.Dispose();
			PreviousRoom = null;
			
			//TODO: Re-enable this after Node finalizer is fixed!!!
			//if (!Debugger.IsAttached)
			GC.Collect();
		}
		
		protected Boolean ReadyToAdvanceToNextRoom = false;
		
		public void AdvanceToNextRoom()
		{
			ReadyToAdvanceToNextRoom = true;
		}
		
		#endregion
		
		#region Level Logic
		
		protected abstract void Initialize();
		
		protected abstract void Cleanup();
		
		protected abstract void Update();
		
		#endregion
		
		#region Debug
		
		internal void GetDebugInfo(StringBuilder sb)
		{
			CurrentRoom.GetDebugInfo(sb);
		}
		
		#endregion
	}
}

