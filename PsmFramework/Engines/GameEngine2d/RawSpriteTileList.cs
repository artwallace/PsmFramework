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
	/// Draw sprites in batch to reduce number of draw calls, state setup etc.
	/// Unlike SpriteList, instead of holding a list of Node objects, this holds 
	/// a list of RawSpriteTile, which is more lightweight. In effect this is a 
	/// thin wrap of SpriteRenderer, usable as a Node.
	/// </summary>
	public class RawSpriteTileList : Node
	{
		/// <summary>The list of RawSpriteTile objects to render.</summary>
		public List<RawSpriteTile> Sprites = new List<RawSpriteTile>();
		
		/// <summary>The color that will be used for all sprites in the Children list.</summary>
		public Vector4 Color = Colors.White;
		
		/// <summary>The blend mode that will be used for all sprites in the Children list.</summary>
		public BlendMode BlendMode = BlendMode.Normal;
		
		/// <summary>The TextureInfo object that will be used for all sprites in the Children list.</summary>
		public TextureInfo TextureInfo;
		
		/// <summary>The shader that will be used for all sprites in the Children list.</summary>
		public ISpriteShader Shader = (ISpriteShader)Director.Instance.SpriteRenderer.DefaultShader;

		/// <summary>
		/// RawSpriteTileList constructor.
		/// TextureInfo must be specified in constructor since there is no default for it.
		/// </summary>
		public RawSpriteTileList( TextureInfo texture_info )
		{
			TextureInfo = texture_info;
		}

		/// <summary>The draw function, draws all sprites in Sprites list.</summary>
		public override void Draw()
		{
			Director.Instance.GL.SetBlendMode( BlendMode );
			Shader.SetColor( ref Color );
			Shader.SetUVTransform( ref Math.UV_TransformFlipV );
			Director.Instance.SpriteRenderer.BeginSprites( TextureInfo, Shader, Sprites.Count );

			//System.Console.WriteLine( Sprites.Count );

			foreach ( RawSpriteTile sprite in Sprites )
			{
				Director.Instance.SpriteRenderer.FlipU = sprite.FlipU;
				Director.Instance.SpriteRenderer.FlipV = sprite.FlipV;
				TRS copy = sprite.Quad;
				Director.Instance.SpriteRenderer.AddSprite( ref copy, sprite.TileIndex2D );
			}

			Director.Instance.SpriteRenderer.EndSprites(); 
		}

		/// <summary>
		/// Based on the tile size and texture dimensions, return the corresponding size in pixels.
		/// For example you might want to do something like bob.Quad.S = bob.CalcSizeInPixels().
		/// </summary>
		public Vector2 CalcSizeInPixels()
		{
			// in the tile case, all sprites have the same pixel size
			return TextureInfo.TileSizeInPixelsf;
		}
	}
}

