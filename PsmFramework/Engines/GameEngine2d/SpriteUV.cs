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
	/// SpriteUV is a sprite for which you set uvs manually. Uvs are stored as a TRS object. 
	/// Note that the cost of using SpriteUV alone is heavy, try as much as you can to 
	/// use then as children of SpriteList.
	/// </summary>
	public class SpriteUV 
	: SpriteBase
	{
		/// <summary>The UV is specified as a TRS, which lets you define any oriented rectangle in the UV domain.</summary>
		public TRS UV = TRS.Quad0_1;

		/// <summary>SpriteUV constructor.</summary>
		public SpriteUV()
		{
		}

		/// <summary>SpriteUV constructor.</summary>
		public SpriteUV( TextureInfo texture_info )
		: base( texture_info )
		{
		}

		/// <summary>
		/// Based on the uv and texture dimensions, return the corresponding size in pixels.
		/// For example you might want to do something like bob.Quad.S = bob.CalcSizeInPixels().
		/// If the uv is Quad0_1 (the 0,1 unit quad), then this will return thr texture size in pixels.
		/// </summary>
		override public Vector2 CalcSizeInPixels()
		{
			Common.Assert( TextureInfo != null );
			Common.Assert( TextureInfo.Texture != null );

			return new Vector2( UV.S.X * (float)TextureInfo.Texture.Width ,
								UV.S.Y * (float)TextureInfo.Texture.Height );
		}

		override internal void internal_draw()
		{
			Director.Instance.SpriteRenderer.FlipU = FlipU;
			Director.Instance.SpriteRenderer.FlipV = FlipV;
			Director.Instance.SpriteRenderer.AddSprite( ref Quad, ref UV );
		}

		override internal void internal_draw_cpu_transform()
		{
			Director.Instance.SpriteRenderer.FlipU = FlipU;
			Director.Instance.SpriteRenderer.FlipV = FlipV;
			Matrix3 trans = GetTransform(); // warning: ignored local Camera and VertexZ
			Director.Instance.SpriteRenderer.AddSprite( ref Quad, ref UV, ref trans );
		}
	}
}

