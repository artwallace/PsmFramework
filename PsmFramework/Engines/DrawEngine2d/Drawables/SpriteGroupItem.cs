using System;
using PsmFramework.Engines.DrawEngine2d.Support;
using PsmFramework.Engines.DrawEngine2d.Textures;

namespace PsmFramework.Engines.DrawEngine2d.Drawables
{
	//This is not a Drawable, the group is.
	public sealed class SpriteGroupItem : IDisposable
	{
		//TODO: Convert to a struct once we get it working properly.
		
		#region Constructor, Dispose
		
		public SpriteGroupItem(SpriteGroup spriteGroup, IndexKey key)
		{
			Initialize(spriteGroup, key);
		}
		
		public void Dispose()
		{
			Cleanup();
		}
		
		#endregion
		
		#region Initialize, Cleanup
		
		private void Initialize(SpriteGroup spriteGroup, IndexKey key)
		{
			InitializeSpriteGroup(spriteGroup);
			InitializeTextureIndexKey(key);
			InitializePosition();
			InitializeScale();
			InitializeRotation();
		}
		
		private void Cleanup()
		{
			CleanupRotation();
			CleanupScale();
			CleanupPosition();
			CleanupTextureIndexKey();
			CleanupSpriteGroup();
		}
		
		#endregion
		
		#region Update, Render
		
		public void Update()
		{
		}
		
		public void Render()
		{
		}
		
		#endregion
		
		#region SpriteGroup
		
		private void InitializeSpriteGroup(SpriteGroup spriteGroup)
		{
			SpriteGroup = spriteGroup;
			SpriteGroup.AddSprite(this);
		}
		
		private void CleanupSpriteGroup()
		{
			SpriteGroup.RemoveSprite(this);
			SpriteGroup = null;
		}
		
		private SpriteGroup SpriteGroup;
		
		private void InformSpriteGroupThatUpdateIsRequired()
		{
		}
		
		#endregion
		
		#region TextureIndexKey
		
		private void InitializeTextureIndexKey(IndexKey key)
		{
			TextureIndexKey = key;
		}
		
		private void CleanupTextureIndexKey()
		{
		}
		
		private IndexKey _TextureIndexKey;
		public IndexKey TextureIndexKey
		{
			get { return _TextureIndexKey; }
			set
			{
				if(_TextureIndexKey == value)
					return;
				
				_TextureIndexKey = value;
				
				UpdateCachedTextureCoordinates();
				
				SpriteGroup.Layer.DrawEngine2d.SetRenderRequired();
			}
		}
		
		private Single[] CachedTextureCoordinates;
		
		public Int32 TileWidth { get; private set; }
		
		public Int32 TileHeight { get; private set; }
		
		private void UpdateCachedTextureCoordinates()
		{
			Texture2dArea area = TextureIndexKey.GetTileDimensions();
			
			CachedTextureCoordinates = area.CoordinateArray;
			TileWidth = area.Width;
			TileHeight = area.Height;
		}
		
		#endregion
		
		#region Position
		
		private void InitializePosition()
		{
			Position = Coordinate2.X0Y0;
		}
		
		private void CleanupPosition()
		{
		}
		
		private Coordinate2 _Position;
		public Coordinate2 Position
		{
			get { return _Position; }
			set
			{
				if(_Position == value)
					return;
				
				_Position = value;
				
				//TODO: should be changed to SpriteGroup.SetRecalcRequired()
				//but this is a waste since all that will change in the future.
				SpriteGroup.Layer.DrawEngine2d.SetRenderRequired();
			}
		}
		
		public void SetPositionFromCenter(Coordinate2 position)
		{
			Single x = position.X - TileWidth / 2;
			Single y = position.Y - TileHeight / 2;
			
			Position = new Coordinate2(x, y);
		}
		
		public void SetPositionFromCenter(Single x, Single y)
		{
			Single xx = x - TileWidth / 2;
			Single yy = y - TileHeight / 2;
			
			Position = new Coordinate2(xx, yy);
		}
		
		#endregion
		
		#region Scale
		
		private void InitializeScale()
		{
			Scale = 1.0f;
		}
		
		private void CleanupScale()
		{
		}
		
		private Single _Scale;
		public Single Scale
		{
			get { return _Scale; }
			set
			{
				if(_Scale == value)
					return;
				
				_Scale = value;
				
				SpriteGroup.Layer.DrawEngine2d.SetRenderRequired();
			}
		}
		
		#endregion
		
		#region Rotation
		
		private void InitializeRotation()
		{
			Rotation = 0.0f;
		}
		
		private void CleanupRotation()
		{
		}
		
		private Single _Rotation;
		public Single Rotation
		{
			get { return _Rotation; }
			set
			{
				if(_Rotation == value)
					return;
				
				_Rotation = value;
				
				SpriteGroup.Layer.DrawEngine2d.SetRenderRequired();
			}
		}
		
		#endregion
	}
}
