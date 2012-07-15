using System;
using System.Collections.Generic;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

namespace PsmFramework
{
	//TODO: Remove this class once GameEngine2d is removed.
	public class TextureUsers : IDisposable
	{
		#region Constructor, Dispose
		
		public TextureUsers(TextureInfo textureInfo)
		{
			TextureInfo = textureInfo;
		}
		
		public TextureUsers(TextureInfo textureInfo, Object user)
		{
			TextureInfo = textureInfo;
			AddUser(user);
		}
		
		public void Dispose()
		{
			TextureInfo.Dispose();
			Users.Clear();
			Users = null;
		}
		
		#endregion
		
		#region TextureInfo
		
		public TextureInfo TextureInfo { get; private set; }
		
		#endregion
		
		#region Users
		
		private List<Object> Users = new List<Object>();
		
		public void AddUser(Object user)
		{
			if (user == null)
				throw new ArgumentNullException();
			
			//TODO: Should an object be able to register more than once?
			
			//if (Users.Contains(user))
				//return;
			
			Users.Add(user);
		}
		
		public void RemoveUser(Object user)
		{
			if (user == null)
				throw new ArgumentNullException();
			
			if (!Users.Contains(user))
				throw new ArgumentException("User not registered for this texture.");
			
			Users.Remove(user);
		}
		
		public Boolean IsUsedBy(Object user)
		{
			if (user == null)
				throw new ArgumentNullException();
			
			return Users.Contains(user);
		}
		
		public Int32 CountUsers()
		{
			return Users.Count;
		}
		
		#endregion
	}
}

