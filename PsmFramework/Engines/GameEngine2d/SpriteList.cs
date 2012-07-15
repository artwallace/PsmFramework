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
	/// Draw sprites in batch to reduce the number of draw calls, state setup etc.
	/// 
	/// Just adding SpriteUV or SpriteTile objects as children of a SpriteList with AddChild()
	/// will enable batch rendering, with the limitation that the TextureInfo, BlendMode, 
	/// and Color property of the sprites will be ignored in favor of the parent SpriteList's 
	/// TextureInfo, BlendMode, and Color properties.
	/// 
	/// Important: some functions in SpriteUV and SpriteTile use their local TextureInfo
	/// instead of the parent's SpriteTile one, so you probably want to set both to be safe.
	/// </summary>
	public class SpriteList : Node
	{
		/// <summary>
		/// If EnableLocalTransform flag is true, the children sprite's local transform matrices get used,
		/// but vertices get partly transformed on the cpu. You can turn this behavior off to ignore the local 
		/// transform matrix to save a little bit of cpu processing (and rely on Sprite's Quad only 
		/// to position the sprite). In that case (EnableLocalTransform=false) the Position, Scale, Skew, Pivot 
		/// will be ignored.
		/// </summary>
		public bool EnableLocalTransform = true;
		/// <summary>The color that will be used for all sprites in the Children list.</summary>
		public Vector4 Color = Colors.White;
		/// <summary>The blend mode that will be used for all sprites in the Children list.</summary>
		public BlendMode BlendMode = BlendMode.Normal;
		/// <summary>The TextureInfo object that will be used for all sprites in the Children list.</summary>
		public TextureInfo TextureInfo; 
		/// <summary>The shader that will be used for all sprites in the Children list.</summary>
		public ISpriteShader Shader = (ISpriteShader)Director.Instance.SpriteRenderer.DefaultShader;

		/// <summary>
		/// SpriteList constructor.
		/// TextureInfo must be specified in constructor since there is no default for it.
		/// </summary>
		public SpriteList( TextureInfo texture_info )
		{
			TextureInfo = texture_info;
		}

		public override void DrawHierarchy()
		{
			if ( !Visible )
				return;

//			#if DEBUG
			if ( ( Director.Instance.DebugFlags & DebugFlags.DrawTransform ) != 0 )
				DebugDrawTransform();
//			#endif

			////Common.Profiler.Push("DrawHierarchy's PushTransform");
			PushTransform();
			////Common.Profiler.Pop();

			{
				Director.Instance.GL.SetBlendMode( BlendMode );
				Shader.SetColor( ref Color );
				Shader.SetUVTransform( ref Math.UV_TransformFlipV );
				Director.Instance.SpriteRenderer.BeginSprites( TextureInfo, Shader, Children.Count );
			}

			int index=0;
			for ( ; index < Children.Count; ++index )
			{
				if ( Children[index].Order >= 0 ) break;

				if ( !EnableLocalTransform ) ((SpriteBase)Children[index]).internal_draw();
				else ((SpriteBase)Children[index]).internal_draw_cpu_transform();
			}

			////Common.Profiler.Push("DrawHierarchy's PostDraw");
			Draw();
			////Common.Profiler.Pop();

			for ( ; index < Children.Count; ++index )
			{
				if ( !EnableLocalTransform ) ((SpriteBase)Children[index]).internal_draw();
				else ((SpriteBase)Children[index]).internal_draw_cpu_transform();
			}

			{
				Director.Instance.SpriteRenderer.EndSprites(); 
			}

//			#if DEBUG
			if ( ( Director.Instance.DebugFlags & DebugFlags.DrawPivot ) != 0 )
				DebugDrawPivot();

			if ( ( Director.Instance.DebugFlags & DebugFlags.DrawContentLocalBounds ) != 0 )
				DebugDrawContentLocalBounds();
//			#endif

			////Common.Profiler.Push("DrawHierarchy's PopTransform");
			PopTransform();
			////Common.Profiler.Pop();
		}
	}
}

