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
	/// Base class for single sprite nodes.
	/// This is an abstract class.
	/// </summary>
	public abstract class SpriteBase : Node
	{
		/// <summary>
		/// Sprite geometry in the node's local space. 
		/// A TRS defines an oriented rectangle.
		/// </summary>
		public TRS Quad = TRS.Quad0_1;
		/// <summary>If true, the sprite UV are flipped horizontally.</summary>
		public bool FlipU = false;
		/// <summary>If true, the sprite UV are flipped vertically.</summary>
		public bool FlipV = false;
		/// <summary>The sprite color.</summary>
		public Vector4 Color = Colors.White;
		/// <summary>The blend mode.</summary>
		public BlendMode BlendMode = BlendMode.Normal;
		/// <summary>
		/// This is used only if the Sprite is drawn standalone (not in a SpriteList).
		/// If Sprite is used in a SpriteList, then the SpriteList's TextureInfo is used.
		/// </summary>
		public TextureInfo TextureInfo;
		/// <summary>The shader.</summary>
		public ISpriteShader Shader = (ISpriteShader)Director.Instance.SpriteRenderer.DefaultShader;
		/// <summary>Return the dimensions of this sprite in pixels.</summary>
		abstract public Vector2 CalcSizeInPixels();

		/// <summary>SpriteBase constructor.</summary>
		public SpriteBase()
		{
		}

		/// <summary>SpriteBase constructor.</summary>
		public SpriteBase( TextureInfo texture_info )
		{
			TextureInfo = texture_info;
		}

		/// <summary>The draw function (expensive, standalone draw).</summary>
		public override void Draw()
		{
			Common.Assert( TextureInfo != null, "Sprite's TextureInfo is null" );
			Common.Assert( Shader != null, "Sprite's Shader is null" );

			//base.Draw(); // AdHocDraw

			////Common.Profiler.Push("SpriteBase.Draw");

			////Common.Profiler.Push("SpriteBase.Draw prelude");
			Director.Instance.GL.SetBlendMode( BlendMode );
			Shader.SetColor( ref Color );
			Shader.SetUVTransform( ref Math.UV_TransformFlipV );
			////Common.Profiler.Pop();
			Director.Instance.SpriteRenderer.BeginSprites( TextureInfo, Shader, 1 );
			////Common.Profiler.Push("SpriteBase.internal_draw()");
			internal_draw();
			////Common.Profiler.Pop();
			////Common.Profiler.Push("SpriteBase.Draw end");
			Director.Instance.SpriteRenderer.EndSprites(); 
			////Common.Profiler.Pop();

			////Common.Profiler.Pop();
		}

		/// <summary>
		/// The content local bounds is the smallest Bounds2 containing this 
		/// sprite's Quad, and Quad itself (the sprite rectangle) if there is 
		/// no rotation.
		/// </summary>
		public override bool GetlContentLocalBounds( ref Bounds2 bounds )
		{
			bounds = GetlContentLocalBounds();
			return true;
		}

		/// <summary>
		/// The content local bounds is the smallest Bounds2 containing this 
		/// sprite's Quad, and Quad itself (the sprite rectangle) if there is 
		/// no rotation.
		/// </summary>
		public Bounds2 GetlContentLocalBounds()
		{
			return Quad.Bounds2();
		}

		/// <summary>
		/// Stretch sprite Quad so that it covers the entire screen. The scene
		/// needs to have been set/started, since it uses CurrentScene.Camera.
		/// </summary>
		public void MakeFullScreen()
		{
			Quad = new TRS( Director.Instance.CurrentScene.Camera.CalcBounds() );
		}

		/// <summary>
		/// Translate sprite geometry so that center of the sprite becomes aligned 
		/// with the position of the Node.
		/// </summary>
		public void CenterSprite()
		{
			Quad.Centering( new Vector2( 0.5f, 0.5f ) );
//			Quad.Centering( TRS.Local.Center ); // same as above
		}

		/// <summary>
		/// Modify the center of the sprite geometry.
		/// </summary>
		/// <param name="new_center">
		/// The new center, specified in Node local coordinates.
		/// You can pass constants defined under TRS.Local for conveniency.
		/// </param>
		public void CenterSprite( Vector2 new_center )
		{
			Quad.Centering( new_center );
		}

		abstract internal void internal_draw();
		abstract internal void internal_draw_cpu_transform();
	}
}

