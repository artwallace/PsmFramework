using System;

namespace PsmFramework.Engines.DrawEngine2d.Shaders
{
	public class SpriteShader : ShaderBase
	{
		#region Constructor
		
		public SpriteShader(DrawEngine2d drawEngine2d)
			: base(drawEngine2d)
		{
		}
		
		#endregion
		
		#region Path
		
		public override String Path
		{
			get { return BasePath + "Sprite.cgx"; }
		}
		
		#endregion
		
		#region ShaderProgram
		
		protected override void InitializeShaderProgram()
		{
			ShaderProgram.SetUniformBinding(0, "u_WorldMatrix");
		}
		
		protected override void CleanupShaderProgram()
		{
		}
		
		#endregion
	}
}

