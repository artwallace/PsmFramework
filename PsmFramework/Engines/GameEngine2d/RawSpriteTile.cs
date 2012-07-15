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
	/// <summary>Data struct used by RawSpriteTileList.</summary>
	public struct RawSpriteTile
	{
		/// <summary>Sprite geometry (position, rotation, scale define a rectangle).</summary>
		public TRS Quad;
		/// <summary>The tile index.</summary>
		public Vector2i TileIndex2D;
		/// <summary>If true, the sprite UV are flipped horizontally.</summary>
		public bool FlipU;
		/// <summary>If true, the sprite UV are flipped vertically.</summary>
		public bool FlipV;
//		float VertexZ;

		/// <summary>RawSpriteTile constructor.</summary>
		public RawSpriteTile( TRS positioning, Vector2i tile_index, bool flipu=false, bool flipv=false )
		{
			Quad = positioning;
			TileIndex2D = tile_index;
			FlipU = flipu;
			FlipV = flipv;
//			VertexZ = vertexz;
		}
	}
}

