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
			//InitializeLayers();
			Textures = new Dictionary<String, Texture2dPlus>();
			Texture2dPlusCachePolicies = new Dictionary<String, TextureCachePolicy>();
			Texture2dPlusUsers = new Dictionary<String, List<TiledTexture>>();
		}
		
		protected override void Cleanup()
		{
			//CleanupLayers();
			//Textures
			Texture2dPlus[] cleanup = new Texture2dPlus[Textures.Values.Count];
			Textures.Values.CopyTo(cleanup, 0);
			foreach(Texture2dPlus t in cleanup)
				t.Dispose();
			Textures.Clear();
			Textures = null;
			
			//Cache
			Texture2dPlusCachePolicies.Clear();
			Texture2dPlusCachePolicies = null;
			
			//Users
			foreach(List<TiledTexture> list in Texture2dPlusUsers.Values)
				list.Clear();
			Texture2dPlusUsers.Clear();
			Texture2dPlusUsers = null;
		}
		
		#endregion
		
		private Dictionary<String, Texture2dPlus> Textures;
		private Dictionary<String, TextureCachePolicy> Texture2dPlusCachePolicies;
		private Dictionary<String, List<TiledTexture>> Texture2dPlusUsers;
		
		internal void RegisterTexture2dPlus(String key, Texture2dPlus texture, TextureCachePolicy cachePolicy)
		{
			if(String.IsNullOrWhiteSpace(key))
				throw new ArgumentNullException();
			
			if(texture == null)
				throw new ArgumentNullException();
			
			if(Textures.ContainsKey(key))
				throw new ArgumentException("Attempt to register duplicate key.");
			
			Textures.Add(key, texture);
			Texture2dPlusCachePolicies.Add(key, cachePolicy);
			Texture2dPlusUsers.Add(key, new List<TiledTexture>());
		}
		
		internal void UnregisterTexture2dPlus(String key)
		{
			if(String.IsNullOrWhiteSpace(key))
				throw new ArgumentNullException();
			
			if(!Textures.ContainsKey(key))
				throw new ArgumentException("Attempt to unregister an unknown key.");
			
			Textures.Remove(key);
			Texture2dPlusCachePolicies.Remove(key);
			Texture2dPlusUsers[key].Clear();
			Texture2dPlusUsers[key] = null;
			Texture2dPlusUsers.Remove(key);
		}
		
		private void ApplyTexture2dPlusCachePolicyForRemovalOfUser(String key)
		{
			if(Texture2dPlusUsers[key].Count > 0)
				return;
			
			if(Texture2dPlusCachePolicies[key] != TextureCachePolicy.DisposeAfterLastUse)
				return;
			
			Textures[key].Dispose();
		}
		
		internal void AddTexture2dPlusUser(String key, TiledTexture user)
		{
			if(String.IsNullOrWhiteSpace(key))
				throw new ArgumentNullException();
			
			if(user == null)
				throw new ArgumentNullException();
			
			if(!Texture2dPlusUsers.ContainsKey(key))
				throw new ArgumentException("Attempt to add a user to an unknown key.");
			
			if(Texture2dPlusUsers[key] == null)
				Texture2dPlusUsers[key] = new List<TiledTexture>();
			
			if(Texture2dPlusUsers[key].Contains(user))
				throw new ArgumentException("Attempt to register a duplicate user.");
			
			Texture2dPlusUsers[key].Add(user);
		}
		
		internal void RemoveTexture2dPlusUser(String key, TiledTexture user)
		{
			if(String.IsNullOrWhiteSpace(key))
				throw new ArgumentNullException();
			
			if(user == null)
				throw new ArgumentNullException();
			
			if(!Texture2dPlusUsers.ContainsKey(key))
				throw new ArgumentException("Attempt to remove a user from an unknown key.");
			
			if(Texture2dPlusUsers[key] == null)
				throw new ArgumentException("Attempt to remove an unknown user.");
			
			if(!Texture2dPlusUsers[key].Contains(user))
				throw new ArgumentException("Attempt to remove an unknown user.");
			
			Texture2dPlusUsers[key].Remove(user);
			
			//Let the cache policy decide what to do.
			ApplyTexture2dPlusCachePolicyForRemovalOfUser(key);
		}
		
		public void SetOpenGlTexture(String key, Int32 index = 0)
		{
			DrawEngine2d.GraphicsContext.SetTexture(index, Textures[key]);
		}
		
		public Texture2dPlus CreateTexture(String path, TextureCachePolicy cachePolicy = TextureCachePolicy.DisposeAfterLastUse)
		{
			return new Texture2dPlus(DrawEngine2d, path, cachePolicy);
		}
		
		public Texture2dPlus CreateTexture(String key, Int32 width, Int32 height, PixelFormat pixelFormat, TextureCachePolicy cachePolicy = TextureCachePolicy.DisposeAfterLastUse)
		{
			return new Texture2dPlus(DrawEngine2d, key, width, height, pixelFormat, cachePolicy);
		}
		
		public Texture2dPlus GetTexture(String key)
		{
			if(String.IsNullOrWhiteSpace(key))
				throw new ArgumentNullException();
			
			if(!Textures.ContainsKey(key))
				throw new ArgumentException("Key is unknown.");
			
			return Textures[key];
		}
	}
}

