using System;
using PsmFramework.Engines.DrawEngine2d.Layers;
using PsmFramework.Engines.DrawEngine2d.Shaders;
using PsmFramework.Engines.DrawEngine2d.Textures;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;

namespace PsmFramework.Engines.DrawEngine2d.Drawables
{
	public sealed class Sprite : SinglePositionDrawableBase
	{
		#region Constructor, Dispose
		
		public Sprite(LayerBase layer, KeyBase key)
			: base(layer)
		{
			InitializeCustom(key);
		}
		
		#endregion
		
		#region Initialize, Cleanup
		
		protected override void Initialize()
		{
			InitializeShaderProgram();
			InitializeTiledTextureIndex();
		}
		
		//Needed because of parameters.
		private void InitializeCustom(KeyBase key)
		{
			SetTiledTexture(key);
		}
		
		protected override void Cleanup()
		{
			CleanupTiledTextureIndex();
			CleanupShaderProgram();
		}
		
		#endregion
		
		#region Shader Program
		
		private void InitializeShaderProgram()
		{
			Shader = DrawEngine2d.SpriteShader;
		}
		
		private void CleanupShaderProgram()
		{
			Shader = null;
		}
		
		private SpriteShader Shader;
		
		#endregion
		
		#region TiledTextureIndex
		
		private void InitializeTiledTextureIndex()
		{
			Key = default(KeyBase);
		}
		
		private void CleanupTiledTextureIndex()
		{
			UnregisterAsUserOfTiledTexture();
			
			Key = default(KeyBase);
		}
		
		private KeyBase _Key;
		public KeyBase Key
		{
			get { return _Key; }
			set
			{
				if (_Key == value)
					return;
				
				_Key = value;
				
				SetRecalcRequired();
			}
		}
		
		private void SetTiledTexture(KeyBase key, Boolean updateDimensions = true)
		{
			if(key == null)
				throw new ArgumentNullException();
			
			if (Key != null)
				UnregisterAsUserOfTiledTexture();
			
			Key = key;
			
			SetNaturalDimensionsFromTile();
			
			if (updateDimensions)
				SetDimensions();
			
			RegisterAsUserOfTiledTexture();
		}
		
		private void RegisterAsUserOfTiledTexture()
		{
			if (Key == null)
				throw new InvalidOperationException();
			
			if (Key.IsDisposed)
				throw new InvalidOperationException();
			
			if (Key.TiledTexture == null)
				throw new InvalidOperationException();
			
			DrawEngine2d.AddTiledTextureUser(Key.TiledTexture.Key, this);
		}
		
		private void UnregisterAsUserOfTiledTexture()
		{
			if (Key.TiledTexture == null)
				return;
			
			DrawEngine2d.RemoveTiledTextureUser(Key.TiledTexture.Key, this);
		}
		
//		public Single[] GetTiledTextureCoordinates(TiledTextureIndex index, out Int32 width, out Int32 height)
//		{
//			return TiledTexture.GetTextureCoordinates(index, out width, out height);
//		}
		
		#endregion
		
		#region Dimensions
		
		private void SetNaturalDimensionsFromTile()
		{
			if (Key == null || Key.IsDisposed)
				SetNaturalDimensions(0.0f, 0.0f);
			else
				SetNaturalDimensions(Key.Tile.Width, Key.Tile.Height);
		}
		
		#endregion
		
		#region Render
		
		public override void RenderHelper()
		{
			DrawEngine2d.GraphicsContext.SetShaderProgram(Shader.ShaderProgram);
			DrawEngine2d.SetOpenGlTexture(Key.TiledTexture.Key);
			
			Shader.VertexBuffer.SetVertices(1, Key.TextureCoordinates);
			DrawEngine2d.GraphicsContext.SetVertexBuffer(0, Shader.VertexBuffer);
			
			Matrix4 cm = Matrix4.Translation(-RotationCenterX,  -RotationCenterY, RotationCenterZ);
			Matrix4 tm = Matrix4.Translation(Position.X + HalfWidth, Position.Y + HalfHeight, 0.0f);
			Matrix4 rm = Matrix4.RotationZ(Rotation.Radian);
			Matrix4 sm = Matrix4.Scale(Width, Height, 0.0f);
			Matrix4 modelMatrix = tm * rm * sm * cm;
			
			Matrix4 worldViewProj = Layer.Camera.ProjectionMatrix * modelMatrix;
			
			Shader.ShaderProgram.SetUniformValue(0, ref worldViewProj);
			
			//TODO: this needs to be changed to be an array of VBOs, like ge2d.
			Layer.DrawEngine2d.IncrementDrawArrayCallsCounter();
			Layer.DrawEngine2d.GraphicsContext.DrawArrays(DrawMode.TriangleStrip, 0, SpriteShader.IndexCount);
		}
		
		#endregion
		
		#region Recalc
		
		protected override void RecalcBounds()
		{
			//throw new NotImplementedException();
		}
		
		protected override void RecalcHelper()
		{
			//throw new NotImplementedException();
		}
		
		#endregion
	}
}

