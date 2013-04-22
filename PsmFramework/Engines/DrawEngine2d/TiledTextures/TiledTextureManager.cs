using System;
using System.Collections.Generic;
using PsmFramework.Engines.DrawEngine2d.Drawables;
using PsmFramework.Engines.DrawEngine2d.Textures;

namespace PsmFramework.Engines.DrawEngine2d.TiledTextures
{
	public sealed class TiledTextureManager : ManagerBase
	{
		#region Constructor, Dispose
		
		internal TiledTextureManager(DrawEngine2d drawEngine2d)
			: base(drawEngine2d)
		{
		}
		
		#endregion
		
		#region Initialize, Cleanup
		
		protected override void Initialize()
		{
			InitializeTiledTextures();
			InitializeCachePolicies();
			InitializeUsers();
			
		}
		
		protected override void Cleanup()
		{
			CleanupUsers();
			CleanupCachePolicies();
			CleanupTiledTextures();
		}
		
		#endregion
		
		#region TiledTextures
		
		private void InitializeTiledTextures()
		{
			TiledTextures = new Dictionary<String, TiledTexture>();
		}
		
		private void CleanupTiledTextures()
		{
			TiledTexture[] cleanup = new TiledTexture[TiledTextures.Values.Count];
			TiledTextures.Values.CopyTo(cleanup, 0);
			foreach(TiledTexture t in cleanup)
				t.Dispose();
			TiledTextures.Clear();
			TiledTextures = null;
		}
		
		private Dictionary<String, TiledTexture> TiledTextures;
		
		internal void RegisterTiledTexture(String key, TiledTexture texture, TextureCachePolicy cachePolicy)
		{
			if(String.IsNullOrWhiteSpace(key))
				throw new ArgumentNullException();
			
			if(texture == null)
				throw new ArgumentNullException();
			
			if(TiledTextures.ContainsKey(key))
				throw new ArgumentException("Attempt to register duplicate key.");
			
			TiledTextures.Add(key, texture);
			CachePolicies.Add(key, cachePolicy);
			Users.Add(key, new List<DrawableBase>());
		}
		
		internal void UnregisterTiledTexture(String key)
		{
			if(String.IsNullOrWhiteSpace(key))
				throw new ArgumentNullException();
			
			if(!TiledTextures.ContainsKey(key))
				throw new ArgumentException("Attempt to unregister an unknown key.");
			
			TiledTextures.Remove(key);
			CachePolicies.Remove(key);
			Users[key].Clear();
			Users[key] = null;
			Users.Remove(key);
		}
		
		public TiledTexture CreateTiledTexture(String path, TextureCachePolicy cachePolicy = TextureCachePolicy.DisposeAfterLastUse)
		{
			return new TiledTexture(this, path, cachePolicy);
		}
		
		public TiledTexture CreateTiledTexture(String key, Texture2dPlus texture, TextureCachePolicy cachePolicy = TextureCachePolicy.DisposeAfterLastUse)
		{
			return new TiledTexture(this, key, texture, cachePolicy);
		}
		
		public TiledTexture GetTiledTexture(String key)
		{
			if(String.IsNullOrWhiteSpace(key))
				throw new ArgumentNullException();
			
			return TiledTextures[key];
		}
		
		#endregion
		
		#region CachePolicies
		
		private void InitializeCachePolicies()
		{
			CachePolicies = new Dictionary<String, TextureCachePolicy>();
		}
		
		private void CleanupCachePolicies()
		{
			CachePolicies.Clear();
			CachePolicies = null;
		}
		
		private Dictionary<String, TextureCachePolicy> CachePolicies;
		
		private void ApplyCachePolicyForRemovalOfUser(String key)
		{
			if(Users[key].Count > 0)
				return;
			
			if(CachePolicies[key] != TextureCachePolicy.DisposeAfterLastUse)
				return;
			
			TiledTextures[key].Dispose();
		}
		
		#endregion
		
		#region Users
		
		private void InitializeUsers()
		{
			Users = new Dictionary<String, List<DrawableBase>>();
		}
		
		private void CleanupUsers()
		{
			foreach(List<DrawableBase> list in Users.Values)
				list.Clear();
			Users.Clear();
			Users = null;
		}
		
		private Dictionary<String, List<DrawableBase>> Users;
		
		internal void AddUser(String key, DrawableBase user)
		{
			if(String.IsNullOrWhiteSpace(key))
				throw new ArgumentNullException();
			
			if(user == null)
				throw new ArgumentNullException();
			
			if(!Users.ContainsKey(key))
				throw new ArgumentException("Attempt to add a user to an unknown key.");
			
			if(Users[key] == null)
				Users[key] = new List<DrawableBase>();
			
			if(Users[key].Contains(user))
				throw new ArgumentException("Attempt to register a duplicate user.");
			
			Users[key].Add(user);
		}
		
		internal void RemoveUser(String key, DrawableBase user)
		{
			if(String.IsNullOrWhiteSpace(key))
				throw new ArgumentNullException();
			
			if(user == null)
				throw new ArgumentNullException();
			
			if(!Users.ContainsKey(key))
				throw new ArgumentException("Attempt to remove a user from an unknown key.");
			
			if(Users[key] == null)
				throw new ArgumentException("Attempt to remove an unknown user.");
			
			if(!Users[key].Contains(user))
				throw new ArgumentException("Attempt to remove an unknown user.");
			
			Users[key].Remove(user);
			
			//Let the cache policy decide what to do.
			ApplyCachePolicyForRemovalOfUser(key);
		}
		
		#endregion
	}
}

