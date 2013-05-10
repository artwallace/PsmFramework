using System;
using PsmFramework.Engines.DrawEngine2d.Layers;
using PsmFramework.Engines.DrawEngine2d.Shaders;
using PsmFramework.Engines.DrawEngine2d.Support;
using PsmFramework.Engines.DrawEngine2d.TiledTextures;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;

namespace PsmFramework.Engines.DrawEngine2d.Drawables
{
	public sealed class Sprite : SpriteDrawableBase
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
			InitializeTiledTextureIndex();
			InitializeMatrices();
		}
		
		//Needed because of parameters.
		private void InitializeCustom(KeyBase key)
		{
			SetTiledTexture(key);
		}
		
		protected override void Cleanup()
		{
			CleanupMatrices();
			CleanupTiledTextureIndex();
		}
		
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
			
			DrawEngine2d.TiledTextures.AddUser(Key.TiledTexture.Key, this);
		}
		
		private void UnregisterAsUserOfTiledTexture()
		{
			if (Key.TiledTexture == null)
				return;
			
			DrawEngine2d.TiledTextures.RemoveUser(Key.TiledTexture.Key, this);
		}
		
		#endregion
		
		#region Position
		
		public new void SetPosition(Coordinate2 position, RelativePosition relativeTo = RelativePosition.Center)
		{
			base.SetPosition(position, relativeTo);
		}
		
		public new void SetPosition(Single x, Single y, RelativePosition relativeTo = RelativePosition.Center)
		{
			base.SetPosition(x, y, relativeTo);
		}
		
		public new void AdjustPosition(Single horizontal, Single vertical)
		{
			base.AdjustPosition(horizontal, vertical);
		}
		
		#endregion
		
		#region Rotation
		
		public new void SetRotation(Angle2 angle)
		{
			base.SetRotation(angle);
		}
		
		public new void AdjustRotation(Angle2 angle)
		{
			base.AdjustRotation(angle);
		}
		
		#endregion
		
		#region Dimensions
		
		public new void SetDimensionsByScale(Single scale)
		{
			base.SetDimensionsByScale(scale);
		}
		
		public new void SetDimensionsProportionallyByWidth(Single width)
		{
			base.SetDimensionsProportionallyByWidth(width);
		}
		
		public new void SetDimensionsProportionallyByHeight(Single height)
		{
			base.SetDimensionsProportionallyByHeight(height);
		}
		
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
			DrawEngine2d.Textures.SetOpenGlTexture(Key.TiledTexture.Key);
			
			Shader.VertexBuffer.SetVertices(1, Key.TextureCoordinates);
			DrawEngine2d.GraphicsContext.SetVertexBuffer(0, Shader.VertexBuffer);
			
//			GenerateModelMatrix();
//			Matrix4 wvp = GenerateWorldViewProjectionMatrix();
			
			Matrix4 cm = Matrix4.Translation(-RotationCenterX, -RotationCenterY, RotationCenterZ);
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
			//TODO: support coordinatesystemmode here.
			if (Rotation == DefaultRotation)
				Bounds = new RectangularArea2(Position.X, Position.Y, Position.X + Width, Position.Y + Height);
			else
			{
				
				//Bounds = new RectangularArea2();
			}
		}
		
		protected override void RecalcHelper()
		{
			//throw new NotImplementedException();
		}
		
		#endregion
		
		#region Matrices
		
		private void InitializeMatrices()
		{
			//As of now, RotationCenter cannot be changed so this can be computed once.
			CenterMatrix = Matrix4.Translation(-RotationCenterX, -RotationCenterY, RotationCenterZ);
		}
		
		private void CleanupMatrices()
		{
		}
		
		private Matrix4 CenterMatrix;
		private Matrix4 TranslationMatrix;
		private Matrix4 RotationMatrix;
		private Matrix4 ScaleMatrix;
		
		private Matrix4 ModelMatrix { get; set; }
		
		private Matrix4 WorldViewProjectionMatrix { get; set; }
		
		private void GenerateModelMatrix()
		{
			//TODO: These should only be recalced if the values have changed.
			TranslationMatrix = Matrix4.Translation(Position.X + HalfWidth, Position.Y + HalfHeight, 0.0f);
			ScaleMatrix = Matrix4.Scale(Width, Height, 0.0f);
			
			ModelMatrix = TranslationMatrix * ScaleMatrix * CenterMatrix;
			
			if (Rotation != DefaultRotation)
			{
				RotationMatrix = Matrix4.RotationZ(Rotation.Radian);
				ModelMatrix *= RotationMatrix;
			}
		}
		
		private Matrix4 GenerateWorldViewProjectionMatrix()
		{
			WorldViewProjectionMatrix = Layer.Camera.ProjectionMatrix * ModelMatrix;
			return WorldViewProjectionMatrix;
		}
		
		#endregion
	}
}

