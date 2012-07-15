/* SCE CONFIDENTIAL
 * PlayStation(R)Suite SDK 0.98.2
 * Copyright (C) 2012 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using Sce.PlayStation.Core.Graphics;

namespace Sce.PlayStation.HighLevel.GameEngine2D.Base
{
	/// <summary>
	/// BlendMode wraps the blend state (BlendFunc+'enabled' bool) and provides some human friendly blend mode names.
	/// </summary>
	public struct BlendMode 
	{
		/// <summary>Blend enabled flag.</summary>
		public bool Enabled;
		
		/// <summary>Blend function.</summary>
		public BlendFunc BlendFunc;
		
		public BlendMode( bool enabled, BlendFunc blend_func )
		{
			Enabled = enabled;
			BlendFunc = blend_func;
		}
		
		/// <summary>No alpha blend: dst = src</summary>
		public static BlendMode None = new BlendMode( false, new BlendFunc( BlendFuncMode.Add, BlendFuncFactor.One, BlendFuncFactor.One ) );
		
		/// <summary>Normal alpha blend: dst = lerp( dst, src, src.a )</summary>
		public static BlendMode Normal = new BlendMode( true, new BlendFunc( BlendFuncMode.Add, BlendFuncFactor.SrcAlpha, BlendFuncFactor.OneMinusSrcAlpha ) );
		
		/// <summary>Additive alpha blend: dst = dst + src</summary>
		public static BlendMode Additive = new BlendMode( true, new BlendFunc( BlendFuncMode.Add, BlendFuncFactor.One, BlendFuncFactor.One ) );
		
		/// <summary>Multiplicative alpha blend: dst = dst * src</summary>
		public static BlendMode Multiplicative = new BlendMode( true, new BlendFunc( BlendFuncMode.Add, BlendFuncFactor.DstColor, BlendFuncFactor.Zero ) );
		
		/// <summary>Premultiplied alpha blend: dst = dst * (1-src.a ) + src</summary>
		public static BlendMode PremultipliedAlpha = new BlendMode( true, new BlendFunc( BlendFuncMode.Add, BlendFuncFactor.One, BlendFuncFactor.OneMinusSrcAlpha ) );
	}
}

