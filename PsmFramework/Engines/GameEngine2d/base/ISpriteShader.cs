/* SCE CONFIDENTIAL
 * PlayStation(R)Suite SDK 0.98.2
 * Copyright (C) 2012 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;

namespace Sce.PlayStation.HighLevel.GameEngine2D.Base
{
	/// <summary>
	/// That's all the interface we require from the shaders set by user.
	/// </summary>
	public interface ISpriteShader
	{
		/// <summary>
		/// The Projection * View * Model matrix.
		/// </summary>
		void SetMVP( ref Matrix4 value );
		
		/// <summary>
		/// Global color.
		/// </summary>
		void SetColor( ref Vector4 value );
		
		/// <summary>
		/// Set the uv transform: offset in Xy and scale in Zw, (0,0,1,1) means UV is unchanged.
		/// Shader code example: transformed_uv = UVTransform.xy + uv * UVTransform.zw
		/// </summary>
		void SetUVTransform( ref Vector4 value );
		
		/// <summary>
		/// Get the ShaderProgram object.
		/// </summary>
		ShaderProgram GetShaderProgram();
	}
}

