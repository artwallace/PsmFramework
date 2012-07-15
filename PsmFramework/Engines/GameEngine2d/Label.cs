/* SCE CONFIDENTIAL
 * PlayStation(R)Suite SDK 0.98.2
 * Copyright (C) 2012 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using Sce.PlayStation.Core;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

namespace Sce.PlayStation.HighLevel.GameEngine2D
{
	/// <summary>
	/// Draw text primitive. 2 types of font data supported:
	/// - One using SpriteRenderer's embedded debug font
	/// - The other using a FontMap object
	/// </summary>
	public class Label : Node
	{
		/// <summary>The text to display.</summary>
		public string Text = "";
		/// <summary>The text color.</summary>
		public Vector4 Color = Colors.White;
		/// <summary>The text blend mode.</summary>
		public BlendMode BlendMode = BlendMode.Normal;
		/// <summary>A scale factor applied to the character's pixel height during rendering.</summary>
		public float HeightScale = 1.0f;
		/// <summary>The fontmap used to display this Label (the character set has to match).</summary>
		public FontMap FontMap;	
		/// <summary>
		/// User can set an external shader. 
		/// The Label class won't dispose of shaders set by user.
		/// </summary>
		public ISpriteShader Shader = (ISpriteShader)Director.Instance.SpriteRenderer.DefaultFontShader;

		/// <summary>The font character height in pixels.</summary>
		float FontHeight
		{
			get
			{
				if ( FontMap == null ) return EmbeddedDebugFontData.CharSizei.Y;
				return FontMap.CharPixelHeight;
			}
		}

		/// <summary>The character height in world coordinates = FontHeight * HeightScale.</summary>
		public float CharWorldHeight
		{
			get { return FontHeight * HeightScale; }
		}

		/// <summary>Label constructor.</summary>
		public Label()
		{
		}

		/// <summary>Label constructor.</summary>
		/// <param name="text">The text to render.</param>
		/// <param name="fontmap">The font data used for rendering the text. If null, an embedded debug font will be used.</param>
		public Label( string text, FontMap fontmap = null )
		{
			Text = text;
			FontMap = fontmap;
		}

		/// <summary>The draw function.</summary>
		public override void Draw()
		{
			base.Draw();

			Director.Instance.GL.SetBlendMode( BlendMode );
			Shader.SetColor( ref Color );

			if ( FontMap == null ) Director.Instance.SpriteRenderer.DrawTextDebug( Text, GameEngine2D.Base.Math._00, CharWorldHeight, true );
			else Director.Instance.SpriteRenderer.DrawTextWithFontMap( Text, GameEngine2D.Base.Math._00, CharWorldHeight, true, FontMap, Shader );
		}

		/// <summary>Return the Bounds2 object containing the text, in local space.</summary>
		public override bool GetlContentLocalBounds( ref Bounds2 bounds )
		{
			bounds = GetlContentLocalBounds();
			return true;
		}

		/// <summary>Return the Bounds2 object containing the text, in local space.</summary>
		public Bounds2 GetlContentLocalBounds()
		{
			if ( FontMap == null ) return Director.Instance.SpriteRenderer.DrawTextDebug( Text, GameEngine2D.Base.Math._00, CharWorldHeight, false );
			else return Director.Instance.SpriteRenderer.DrawTextWithFontMap( Text, GameEngine2D.Base.Math._00, CharWorldHeight, false, FontMap, Shader );
		}
	}
} // namespace Sce.PlayStation.HighLevel.GameEngine2D

