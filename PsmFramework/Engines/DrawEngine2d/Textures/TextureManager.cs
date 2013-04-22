using System;
using System.Collections.Generic;
using PsmFramework.Engines.DrawEngine2d.TiledTextures;
using Sce.PlayStation.Core.Graphics;

namespace PsmFramework.Engines.DrawEngine2d.Textures
{
	public sealed class TextureManager : ManagerBase
	{
		#region Constructor, Dispose
		
		internal TextureManager(DrawEngine2d drawEngine2d)
			: base(drawEngine2d)
		{
		}
		
		#endregion
		
		#region Initialize, Cleanup
		
		protected override void Initialize()
		{
			InitializeTextures();
			InitializeCachePolicies();
			InitializeUsers();
		}
		
		protected override void Cleanup()
		{
			CleanupUsers();
			CleanupCachePolicies();
			CleanupTextures();
		}
		
		#endregion
		
		#region Textures
		
		private void InitializeTextures()
		{
			Textures = new Dictionary<String, Texture2dPlus>();
		}
		
		private void CleanupTextures()
		{
			Texture2dPlus[] cleanup = new Texture2dPlus[Textures.Values.Count];
			Textures.Values.CopyTo(cleanup, 0);
			foreach(Texture2dPlus t in cleanup)
				t.Dispose();
			Textures.Clear();
			Textures = null;
		}
		
		private Dictionary<String, Texture2dPlus> Textures;
		
		internal void RegisterTexture(String key, Texture2dPlus texture, TextureCachePolicy cachePolicy)
		{
			if(String.IsNullOrWhiteSpace(key))
				throw new ArgumentNullException();
			
			if(texture == null)
				throw new ArgumentNullException();
			
			if(Textures.ContainsKey(key))
				throw new ArgumentException("Attempt to register duplicate key.");
			
			Textures.Add(key, texture);
			CachePolicies.Add(key, cachePolicy);
			Users.Add(key, new List<TiledTexture>());
		}
		
		internal void UnregisterTexture(String key)
		{
			if(String.IsNullOrWhiteSpace(key))
				throw new ArgumentNullException();
			
			if(!Textures.ContainsKey(key))
				throw new ArgumentException("Attempt to unregister an unknown key.");
			
			Textures.Remove(key);
			CachePolicies.Remove(key);
			Users[key].Clear();
			Users[key] = null;
			Users.Remove(key);
		}
		
		public Texture2dPlus CreateTexture(String path, TextureCachePolicy cachePolicy = TextureCachePolicy.DisposeAfterLastUse)
		{
			return new Texture2dPlus(this, path, cachePolicy);
		}
		
		public Texture2dPlus CreateTexture(String key, Int32 width, Int32 height, PixelFormat pixelFormat, TextureCachePolicy cachePolicy = TextureCachePolicy.DisposeAfterLastUse)
		{
			return new Texture2dPlus(this, key, width, height, pixelFormat, cachePolicy);
		}
		
		public Texture2dPlus GetTexture(String key)
		{
			if(String.IsNullOrWhiteSpace(key))
				throw new ArgumentNullException();
			
			if(!Textures.ContainsKey(key))
				throw new ArgumentException("Key is unknown.");
			
			return Textures[key];
		}
		
		public void SetOpenGlTexture(String key, Int32 index = 0)
		{
			DrawEngine2d.GraphicsContext.SetTexture(index, Textures[key]);
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
			
			Textures[key].Dispose();
		}
		
		#endregion
		
		#region Users
		
		private void InitializeUsers()
		{
			Users = new Dictionary<String, List<TiledTexture>>();
		}
		
		private void CleanupUsers()
		{
			foreach(List<TiledTexture> list in Users.Values)
				list.Clear();
			Users.Clear();
			Users = null;
		}
		
		private Dictionary<String, List<TiledTexture>> Users;
		
		internal void AddUser(String key, TiledTexture user)
		{
			if(String.IsNullOrWhiteSpace(key))
				throw new ArgumentNullException();
			
			if(user == null)
				throw new ArgumentNullException();
			
			if(!Users.ContainsKey(key))
				throw new ArgumentException("Attempt to add a user to an unknown key.");
			
			if(Users[key] == null)
				Users[key] = new List<TiledTexture>();
			
			if(Users[key].Contains(user))
				throw new ArgumentException("Attempt to register a duplicate user.");
			
			Users[key].Add(user);
		}
		
		internal void RemoveUser(String key, TiledTexture user)
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

