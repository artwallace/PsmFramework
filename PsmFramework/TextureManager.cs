using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

namespace PsmFramework
{
	//TODO: Remove this class once GameEngine2d is removed.
	public sealed class TextureManager : IDisposable
	{
		public AppManager Mgr { get; private set; }
		
		#region Constructor, Dispose
		
		public TextureManager(AppManager mgr)
		{
			if (mgr == null)
				throw new ArgumentNullException();
			Mgr = mgr;
			
			InitializeTextures();
		}
		
		public void Dispose()
		{
			CleanupTextures();
			
			Mgr = null;
		}
		
		#endregion
		
		#region Initialize, Cleanup
		
		private void InitializeTextures()
		{
			TextureAssetsByUsers = new Dictionary<String, TextureUsers>();
		}
		
		private void CleanupTextures()
		{
			ClearTextures();
			TextureAssetsByUsers = null;
		}
		
		#endregion
		
		#region Texture Assets
		
		private Dictionary<String, TextureUsers> TextureAssetsByUsers;
		
		public void AddTextureAsset(String path, Object user)
		{
			AddTextureAsset(path, user, 1, 1);
		}
		
		public void AddTextureAsset(String path, Object user, Int32 u, Int32 v)
		{
			if (String.IsNullOrWhiteSpace(path))
				throw new ArgumentException();
			
			if (user == null)
				throw new ArgumentNullException();
			
			Vector2i tiles = new Vector2i(u, v);
			
			if (TextureAssetsByUsers.ContainsKey(path))
			{
				if (TextureAssetsByUsers[path].TextureInfo.NumTiles != tiles)
					throw new ArgumentException("Duplicate texture asset with different tiles: " + path);
				
				TextureAssetsByUsers[path].AddUser(user);
			}
			else
			{
				Texture2D t = new Texture2D(path, false);
				TextureInfo ti = new TextureInfo(t, tiles);
				
				TextureAssetsByUsers[path] = new TextureUsers(ti, user);
			}
		}
		
		public void RemoveTextureAsset(String path, Object user)
		{
			if (String.IsNullOrWhiteSpace(path))
				throw new ArgumentException();
			
			if (user == null)
				throw new ArgumentNullException();
			
			if (!TextureAssetsByUsers.ContainsKey(path))
				throw new ArgumentException("Unknown texture asset: " + path);
			
			TextureAssetsByUsers[path].RemoveUser(user);
			
			if (TextureAssetsByUsers[path].CountUsers() == 0)
			{
				TextureAssetsByUsers[path].Dispose();
				TextureAssetsByUsers.Remove(path);
			}
		}
		
		public void RemoveAllTexturesForUser(Object user)
		{
			if (user == null)
				throw new ArgumentNullException();
			
			List<String> pathsToRemove = new List<String>();
			
			foreach(KeyValuePair<String, TextureUsers> kvp in TextureAssetsByUsers)
			{
				if (kvp.Value.IsUsedBy(user))
				{
					kvp.Value.RemoveUser(user);
					
					if (kvp.Value.CountUsers() == 0)
						pathsToRemove.Add(kvp.Key);
				}
			}
			
			foreach (String path in pathsToRemove)
			{
				TextureAssetsByUsers[path].Dispose();
				TextureAssetsByUsers.Remove(path);
			}
		}
		
		private void ClearTextures()
		{
			foreach(KeyValuePair<String, TextureUsers> kvp in TextureAssetsByUsers)
				kvp.Value.Dispose();
			TextureAssetsByUsers.Clear();
		}
		
		#endregion
		
		#region TextureInfo
		
		public TextureInfo GetTextureInfo(String path)
		{
			if (String.IsNullOrWhiteSpace(path))
				throw new ArgumentException();
			
			if (!TextureAssetsByUsers.ContainsKey(path))
				throw new ArgumentException("Unknown texture asset: " + path);
			
			return TextureAssetsByUsers[path].TextureInfo;
		}
		
		#endregion
		
		#region CreateSpriteList
		
		public SpriteList CreateSpriteList(String path, Object user)
		{
			return CreateSpriteList(path, user, 1, 1);
		}
		
		public SpriteList CreateSpriteList(String path, Object user, Int32 u, Int32 v)
		{
			if (String.IsNullOrWhiteSpace(path))
				throw new ArgumentException();
			
			AddTextureAsset(path, user, u, v);
			
			SpriteList sl = new SpriteList(GetTextureInfo(path));
			return sl;
		}
		
		#endregion
		
		#region CreateSpriteUV
		
		public SpriteUV CreateSpriteUV(String path)
		{
			return CreateSpriteUV(path, true, Vector2.Zero);
		}
		
		public SpriteUV CreateSpriteUV(String path, Vector2 center)
		{
			return CreateSpriteUV(path, false, center);
		}
		
		private SpriteUV CreateSpriteUV(String path, Boolean centerToSprite, Vector2 center)
		{
			if (String.IsNullOrWhiteSpace(path))
				throw new ArgumentException();
			
			if (!TextureAssetsByUsers.ContainsKey(path))
				throw new ArgumentException("Unknown texture asset: " + path);
			
			TextureInfo ti = GetTextureInfo(path);//TextureAssetsByUsers[path].TextureInfo;
			
			SpriteUV s = new SpriteUV(ti);
			s.Quad.S = ti.TextureSizef;
			
			if (centerToSprite)
				s.CenterSprite();
			else
				s.CenterSprite(center);
			
			return s;
		}
		
		#endregion
		
		#region CreateSpriteTile
		
		public SpriteTile CreateSpriteTile(String path, Int32 u, Int32 v)
		{
			return CreateSpriteTile(path, u, v, false, Vector2.Zero);
		}
		
		public SpriteTile CreateSpriteTile(String path, Int32 u, Int32 v, Boolean centerToSprite)
		{
			return CreateSpriteTile(path, u, v, centerToSprite, Vector2.Zero);
		}
		
		public SpriteTile CreateSpriteTile(String path, Int32 u, Int32 v, Vector2 center)
		{
			return CreateSpriteTile(path, u, v, false, center);
		}
		
		private SpriteTile CreateSpriteTile(String path, Int32 u, Int32 v, Boolean centerToSprite, Vector2 center)
		{
			if (String.IsNullOrWhiteSpace(path))
				throw new ArgumentException();
			
			if (!TextureAssetsByUsers.ContainsKey(path))
				throw new ArgumentException("Unknown texture asset: " + path);
			
			TextureInfo ti = GetTextureInfo(path);//TextureAssets[path];
			
			SpriteTile s = new SpriteTile(ti, new Vector2i(u, v));
			s.Quad.S = new Vector2(ti.Texture.Width / ti.NumTiles.X, ti.Texture.Height / ti.NumTiles.Y);
			
			if (centerToSprite)
				s.CenterSprite();
			else
				s.CenterSprite(center);
			
			return s;
		}
		
		#endregion
		
		#region CreateRawSpriteTileList
		
		public RawSpriteTileArray CreateRawSpriteTileList(String path, Object user, Int32 u, Int32 v, Int32 size)
		{
			if (String.IsNullOrWhiteSpace(path))
				throw new ArgumentException();
			
			AddTextureAsset(path, user, u, v);
			
			RawSpriteTileArray rsl = new RawSpriteTileArray(GetTextureInfo(path), size);
			return rsl;
		}
		
		#endregion
		
		#region CreateRawSpriteTile
		
		public RawSpriteTile CreateRawSpriteTile(String path, Int32 u, Int32 v, TRS trs)
		{
			if (String.IsNullOrWhiteSpace(path))
				throw new ArgumentException();
			
			if (!TextureAssetsByUsers.ContainsKey(path))
				throw new ArgumentException("Unknown texture asset: " + path);
			
			TextureInfo ti = GetTextureInfo(path);
			
			RawSpriteTile s = new RawSpriteTile(trs, new Vector2i(u, v));
			s.Quad.S = new Vector2(ti.Texture.Width / ti.NumTiles.X, ti.Texture.Height / ti.NumTiles.Y);
			
			return s;
		}
		
		#endregion
	}
}

