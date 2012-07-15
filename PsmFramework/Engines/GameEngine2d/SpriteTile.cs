/* SCE CONFIDENTIAL
 * PlayStation(R)Suite SDK 0.98.2
 * Copyright (C) 2012 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

namespace Sce.PlayStation.HighLevel.GameEngine2D
{
	/// <summary>
	/// SpriteTile is a sprite for which you specify a tile index (1D or 2D) in a TextureInfo. 
	/// Note that the cost of using SpriteUV alone is heavy, try as much as you can to use 
	/// then as children of SpriteList.
	/// </summary>
	public class SpriteTile : SpriteBase
	{
		/// <summary>
		/// TileIndex2D defines the UV that will be used for this sprite. 
		/// Tiles are indexed in 2 dimensions.
		/// </summary>
		public Vector2i TileIndex2D = new Vector2i(0,0);

		/// <summary>
		/// Instead of TileIndex2D you can also work with a flattened 1d index, for animation, etc.
		/// In that case the set/get calculation depend on TextureInfo, so TextureInfo must have 
		/// been set properly.
		/// </summary>
		public int TileIndex1D
		{
			set 
			{ 
				Common.Assert( TextureInfo != null );
				TileIndex2D = new Vector2i( value % TextureInfo.NumTiles.X, value / TextureInfo.NumTiles.X );
			}

			get 
			{ 
				Common.Assert( TextureInfo != null );
				return TileIndex2D.X + TileIndex2D.Y * TextureInfo.NumTiles.X; 
			}
		}

		/// <summary>
		/// SpriteTile constructor.
		/// TileIndex2D is set to (0,0) by default.
		/// </summary>
		public SpriteTile()
		{
		}

		/// <summary>
		/// SpriteTile constructor.
		/// TileIndex2D is set to (0,0) by default.
		/// </summary>
		public SpriteTile( TextureInfo texture_info )
		: base( texture_info )
		{
		}

		/// <summary>
		/// SpriteTile constructor.
		/// TileIndex2D is set to (0,0) by default.
		/// </summary>
		/// <param name="texture_info">The tiled texture object.</param>
		/// <param name="index">2D tile index. (0,0) is the bottom left tile.</param>
		public SpriteTile( TextureInfo texture_info, Vector2i index )
		: base( texture_info )
		{
			TileIndex2D = index;
		}

		/// <summary>
		/// SpriteTile constructor.
		/// </summary>
		/// <param name="texture_info">The tiled texture object.</param>
		/// <param name="index">1D tile index. Flat indexing starts from bottom left tile, which is (0,0) in 2D.</param>
		public SpriteTile( TextureInfo texture_info, int index )
		: base( texture_info )
		{
			TileIndex1D = index;
		}

		/// <summary>
		/// Based on the uv and texture dimensions, return the corresponding size in pixels.
		/// For example you might want to do something like bob.Quad.S = bob.CalcSizeInPixels().
		/// </summary>
		override public Vector2 CalcSizeInPixels()
		{
			// in the tile case, all sprites have the same pixel size
			return TextureInfo.TileSizeInPixelsf;
		}

		override internal void internal_draw()
		{
			Director.Instance.SpriteRenderer.FlipU = FlipU;
			Director.Instance.SpriteRenderer.FlipV = FlipV;
			Director.Instance.SpriteRenderer.AddSprite( ref Quad, TileIndex2D );
		}

		override internal void internal_draw_cpu_transform()
		{
			Director.Instance.SpriteRenderer.FlipU = FlipU;
			Director.Instance.SpriteRenderer.FlipV = FlipV;
			Matrix3 trans = GetTransform(); // warning: ignored local Camera and VertexZ
			Director.Instance.SpriteRenderer.AddSprite( ref Quad, TileIndex2D, ref trans );
		}
	}
}

