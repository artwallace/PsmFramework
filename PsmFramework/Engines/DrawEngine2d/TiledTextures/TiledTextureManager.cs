using System;
using System.Collections.Generic;
using PsmFramework.Engines.DrawEngine2d.Textures;
using PsmFramework.Engines.DrawEngine2d.Drawables;

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
			//InitializeLayers();
			TiledTextures = new Dictionary<String, TiledTexture>();
			TiledTextureCachePolicies = new Dictionary<String, TextureCachePolicy>();
			TiledTextureUsers = new Dictionary<String, List<DrawableBase>>();
		}
		
		protected override void Cleanup()
		{
			//CleanupLayers();
			
			//Textures
			TiledTexture[] cleanup = new TiledTexture[TiledTextures.Values.Count];
			TiledTextures.Values.CopyTo(cleanup, 0);
			foreach(TiledTexture t in cleanup)
				t.Dispose();
			TiledTextures.Clear();
			TiledTextures = null;
			
			//Cache
			TiledTextureCachePolicies.Clear();
			TiledTextureCachePolicies = null;
			
			//Users
			foreach(List<DrawableBase> list in TiledTextureUsers.Values)
				list.Clear();
			TiledTextureUsers.Clear();
			TiledTextureUsers = null;
		}
		
		#endregion
		
		private Dictionary<String, TiledTexture> TiledTextures;
		private Dictionary<String, TextureCachePolicy> TiledTextureCachePolicies;
		private Dictionary<String, List<DrawableBase>> TiledTextureUsers;
		
		internal void RegisterTiledTexture(String key, TiledTexture texture, TextureCachePolicy cachePolicy)
		{
			if(String.IsNullOrWhiteSpace(key))
				throw new ArgumentNullException();
			
			if(texture == null)
				throw new ArgumentNullException();
			
			if(TiledTextures.ContainsKey(key))
				throw new ArgumentException("Attempt to register duplicate key.");
			
			TiledTextures.Add(key, texture);
			TiledTextureCachePolicies.Add(key, cachePolicy);
			TiledTextureUsers.Add(key, new List<DrawableBase>());
		}
		
		internal void UnregisterTiledTexture(String key)
		{
			if(String.IsNullOrWhiteSpace(key))
				throw new ArgumentNullException();
			
			if(!TiledTextures.ContainsKey(key))
				throw new ArgumentException("Attempt to unregister an unknown key.");
			
			TiledTextures.Remove(key);
			TiledTextureCachePolicies.Remove(key);
			TiledTextureUsers[key].Clear();
			TiledTextureUsers[key] = null;
			TiledTextureUsers.Remove(key);
		}
		
		private void ApplyTiledTextureCachePolicyForRemovalOfUser(String key)
		{
			if(TiledTextureUsers[key].Count > 0)
				return;
			
			if(TiledTextureCachePolicies[key] != TextureCachePolicy.DisposeAfterLastUse)
				return;
			
			TiledTextures[key].Dispose();
		}
		
		internal void AddTiledTextureUser(String key, DrawableBase user)
		{
			if(String.IsNullOrWhiteSpace(key))
				throw new ArgumentNullException();
			
			if(user == null)
				throw new ArgumentNullException();
			
			if(!TiledTextureUsers.ContainsKey(key))
				throw new ArgumentException("Attempt to add a user to an unknown key.");
			
			if(TiledTextureUsers[key] == null)
				TiledTextureUsers[key] = new List<DrawableBase>();
			
			if(TiledTextureUsers[key].Contains(user))
				throw new ArgumentException("Attempt to register a duplicate user.");
			
			TiledTextureUsers[key].Add(user);
		}
		
		internal void RemoveTiledTextureUser(String key, DrawableBase user)
		{
			if(String.IsNullOrWhiteSpace(key))
				throw new ArgumentNullException();
			
			if(user == null)
				throw new ArgumentNullException();
			
			if(!TiledTextureUsers.ContainsKey(key))
				throw new ArgumentException("Attempt to remove a user from an unknown key.");
			
			if(TiledTextureUsers[key] == null)
				throw new ArgumentException("Attempt to remove an unknown user.");
			
			if(!TiledTextureUsers[key].Contains(user))
				throw new ArgumentException("Attempt to remove an unknown user.");
			
			TiledTextureUsers[key].Remove(user);
			
			//Let the cache policy decide what to do.
			ApplyTiledTextureCachePolicyForRemovalOfUser(key);
		}
		
		public TiledTexture CreateTiledTexture(String path, TextureCachePolicy cachePolicy = TextureCachePolicy.DisposeAfterLastUse)
		{
			return new TiledTexture(DrawEngine2d, path, cachePolicy);
		}
		
		public TiledTexture CreateTiledTexture(String key, Texture2dPlus texture, TextureCachePolicy cachePolicy = TextureCachePolicy.DisposeAfterLastUse)
		{
			return new TiledTexture(DrawEngine2d, key, texture, cachePolicy);
		}
		
		public TiledTexture GetTiledTexture(String key)
		{
			if(String.IsNullOrWhiteSpace(key))
				throw new ArgumentNullException();
			
			return TiledTextures[key];
		}
	}
}

