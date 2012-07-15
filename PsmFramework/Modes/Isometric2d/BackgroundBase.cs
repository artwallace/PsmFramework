using System;
using System.Collections.Generic;
using System.Text;
using Sce.PlayStation.Core;
using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

namespace PsmFramework.Modes.Isometric2d
{
	public abstract class BackgroundBase : IDisposable
	{
		public RoomBase Room { get; private set; }
		public Isometric2dModeBase Mode { get { return Room.Mode; } }
		public AppManager Mgr { get { return Room.Mgr; } }
		
		#region Constructor, Dispose
		
		public BackgroundBase(RoomBase room, Int32 columns, Int32 rows, String asset, Int32 assetColumns, Int32 assetRows, Int32 tileWidth, Int32 tileHeight, Int32 horizontalScreenPadding, Int32 verticalScreenPadding)
		{
			if (room == null)
				throw new ArgumentNullException();
			
			Initialize(room, columns, rows, asset, assetColumns, assetRows, tileWidth, tileHeight, horizontalScreenPadding, verticalScreenPadding);
		}
		
		public void Dispose()
		{
			Cleanup();
		}
		
		#endregion
		
		#region Initialize, Cleanup
		
		private void Initialize(RoomBase room, Int32 columns, Int32 rows, String asset, Int32 assetColumns, Int32 assetRows, Int32 tileWidth, Int32 tileHeight, Int32 horizontalScreenPadding, Int32 verticalScreenPadding)
		{
			Room = room;
			
			Asset = asset;
			AssetColumns = assetColumns;
			AssetRows = assetRows;
			AssetTileWidth = tileWidth;
			AssetTileHeight = tileHeight;
			
			HorizontalScreenPadding = horizontalScreenPadding;
			VerticalScreenPadding = verticalScreenPadding;
			
			Columns = columns;
			Rows = rows;
			
			CalcDimensions();
			
			InitializeSpriteList();
			InitializeSprites();
			InitializeTiles();
		}
		
		private void Cleanup()
		{
			CleanupTiles();
			CleanupSprites();
			CleanupSpriteList();
			
			Asset = null;
			Room = null;
		}
		
		#endregion
		
		#region Asset
		
		private String Asset;// { get; set; }
		
		private Int32 AssetColumns;// { get; set; }
		private Int32 AssetRows;// { get; set; }
		
		public Int32 AssetTileWidth { get; private set; }
		public Int32 AssetTileHeight { get; private set; }
		
		#endregion
		
		#region Size
		
		public Int32 Columns { get; private set; }
		public Int32 Rows { get; private set; }
		
		public Int32 HorizontalScreenPadding { get; private set; }
		public Int32 VerticalScreenPadding { get; private set; }
		
		public Int32 Width { get; protected set; }
		public Int32 Height { get; protected set; }
		
		protected abstract void CalcDimensions();
		
		#endregion
		
		#region SpriteList
		
		private Int32 InSpriteListCount;
		
		protected Int32 SpriteColumns;
		protected Int32 SpriteRows;
		
		protected RawSpriteTileArray TileSpriteList;
		
		private void InitializeSpriteList()
		{
			InSpriteListCount = 0;
			CalculateTilesNeededToFillScreen();
			
			TileSpriteList = Mode.TextureManager.CreateRawSpriteTileList(Asset, this, AssetColumns, AssetRows, SpriteColumns * SpriteRows);
			TileSpriteList.BlendMode = BlendMode.PremultipliedAlpha;
			
			Room.AddToScene(TileSpriteList, DrawLayers.Backgroundi);
		}
		
		private void CleanupSpriteList()
		{
			Room.RemoveFromScene(TileSpriteList);
			
			TileSpriteList.Cleanup();
			TileSpriteList = null;
		}
		
		protected abstract void CalculateTilesNeededToFillScreen();
		
		public void AddToSpriteList(RawSpriteTile sprite)
		{
			//TODO: This is set for ARRAY!
			TileSpriteList.Sprites[InSpriteListCount] = sprite;
			InSpriteListCount++;
			//TileSpriteList.Sprites.Add(sprite);
		}
		
		public void RemoveFromSpriteList(RawSpriteTile sprite, Boolean doCleanup = false)
		{
			//TODO: This is set for ARRAY!
			InSpriteListCount--;
			//TileSpriteList.Sprites.Remove(sprite);
		}
		
		#endregion
		
		#region Sprites
		
		private void InitializeSprites()
		{
			OffScreenTileHidingPlace = GetTilePositionAtLowerLeft(-2, -2);
			
			TRS initialPlacement = new TRS(new Bounds2(new Vector2(32f, 32f), new Vector2(1f, 1f)));
			
			for (Int32 c = 0; c < SpriteColumns; c++)
			{
				for (Int32 r = 0; r < SpriteRows; r++)
				{
					RawSpriteTile rst = Mode.TextureManager.CreateRawSpriteTile(Asset, 0, 0, initialPlacement);
					AddToSpriteList(rst);
				}
			}
		}
		
		private void CleanupSprites()
		{
		}
		
		protected void UpdateSprite(Int32 spriteIndex, Int32 column, Int32 row)
		{
			TileSpriteList.Sprites[spriteIndex].Quad.T = GetTilePositionAtLowerLeft(column, row);
			TileSpriteList.Sprites[spriteIndex].TileIndex2D = Tiles[column,row].AssetIndex;
		}
		
		protected void HideSpiteOffscreen(Int32 spriteIndex)
		{
			TileSpriteList.Sprites[spriteIndex].Quad.T = OffScreenTileHidingPlace;
		}
		
		private Vector2 OffScreenTileHidingPlace;
		
		#endregion
		
		#region Tiles
		
		private BackgroundTile[,] Tiles;
		
		private void InitializeTiles()
		{
			Tiles = new BackgroundTile[Columns, Rows];
			
			for (Int32 column = 0; column < Columns; column++)
				for (Int32 row = 0; row < Rows; row++)
					InitializeTile(column, row);
		}
		
		private void CleanupTiles()
		{
			Tiles = new BackgroundTile[0,0];
//			for (Int32 column = 0; column < Columns; column++)
//			{
//				for (Int32 row = 0; row < Rows; row++)
//				{
//					Tiles[column, row].Dispose();
//					Tiles[column, row] = null;
//				}
//			}
		}
		
		private void InitializeTile(Int32 column, Int32 row)
		{
			BackgroundTileData tileData = Room.GetBackgroundTileData(column, row);
			Tiles[column, row] = new BackgroundTile(column, row, tileData);
		}
		
		public Vector2i GetTileFromRoomPostion(Vector2 position)
		{
			//TODO: Wrong calculation!
			Int32 column = (Int32)System.Math.Floor(position.X / AssetTileWidth);
			Int32 row = (Int32)System.Math.Floor(position.Y / AssetTileHeight);
			return new Vector2i(column, row);
		}
		
		private BackgroundTileData GetTileDataFromRoomPosition(Vector2 position)
		{
			Vector2i v = GetTileFromRoomPostion(position);
			return Room.GetBackgroundTileData(v.X, v.Y);
		}
		
		private Vector2 GetTilePosition(Int32 column, Int32 row)
		{
			return new Vector2(AssetTileWidth * column, AssetTileHeight * row);
		}
		
		public Vector2 GetTilePositionAtCenter(Int32 column, Int32 row)
		{
			Int32 colAdjust = AssetTileWidth / 2;
			Int32 rowAdjust = AssetTileHeight / 2;
			return new Vector2(AssetTileWidth * column + colAdjust, AssetTileHeight * row + rowAdjust);
		}
		
		private Vector2 GetTilePositionAtLowerLeft(Int32 column, Int32 row)
		{
			Int32 x = (column * (AssetTileWidth / 2)) + HorizontalScreenPadding;
			
			if(column % 2 == 0)
				return new Vector2(x, (row * AssetTileHeight) + VerticalScreenPadding);
			else
				return new Vector2(x, (AssetTileHeight * row) + (AssetTileHeight / 2) + VerticalScreenPadding);
		}
		
		#endregion
		
		#region Names
		
		private Dictionary<String, Vector2i> Names;
		
		private void InitializeNames()
		{
			Names = new Dictionary<string, Vector2i>();
		}
		
		private void CleanupNames()
		{
			Names.Clear();
			Names = null;
		}
		
		internal void AddName(String name, Int32 column, Int32 row)
		{
			if(String.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException();
			
			if(Names.ContainsKey(name))
				throw new ArgumentException("Duplicate name encountered.");
			
			Names[name] = new Vector2i(column, row);
		}
		
		#endregion
		
		#region Update
		
		internal abstract void Update();
		
		#endregion
		
		#region Debug
		
		internal void GetDebugInfo(StringBuilder sb)
		{
			sb.Append("Background Tiles: ");
			sb.AppendLine(Tiles.Length.ToString());
			
			sb.Append("Background Tiles Drawn: ");
			sb.AppendLine(InSpriteListCount.ToString() + ", " + SpriteColumns + " x " + SpriteRows);
		}
		
		#endregion
	}
}

