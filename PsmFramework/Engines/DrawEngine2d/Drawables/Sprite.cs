using System;
using PsmFramework.Engines.DrawEngine2d.Layers;
using PsmFramework.Engines.DrawEngine2d.Shaders;
using PsmFramework.Engines.DrawEngine2d.Support;
using PsmFramework.Engines.DrawEngine2d.Textures;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;

namespace PsmFramework.Engines.DrawEngine2d.Drawables
{
	public sealed class Sprite : DrawableBase
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
			
			InitializePosition();
			InitializeRotation();
			InitializeDimensions();
			InitializeColors();
		}
		
		//Needed because of parameters.
		private void InitializeCustom(KeyBase key)
		{
			SetTiledTexture(key);
		}
		
		protected override void Cleanup()
		{
			CleanupColors();
			CleanupDimensions();
			CleanupRotation();
			CleanupPosition();
			
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
			
			if (updateDimensions)
				SetDimensionsFromTile();
			
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
		
		#region Position
		
		private void InitializePosition()
		{
			Position = DefaultPosition;
		}
		
		private void CleanupPosition()
		{
			Position = DefaultPosition;
		}
		
		private Coordinate2 _Position;
		public Coordinate2 Position
		{
			get { return _Position; }
			private set
			{
				if(_Position == value)
					return;
				
				_Position = value;
				
				SetRecalcRequired();
			}
		}
		
		public void SetPosition(Coordinate2 position, RelativePosition relativeTo = RelativePosition.Center)
		{
			//TODO: Add support for CoordinateSystemMode
			if (DrawEngine2d.CoordinateSystemMode != CoordinateSystemMode.OriginAtUpperLeft)
				throw new NotImplementedException();
			
			switch (relativeTo)
			{
				case RelativePosition.Center :
					Position = new Coordinate2(position.X - HalfWidth, position.Y - HalfHeight);
					break;
				
				case RelativePosition.TopLeft :
					Position = position;
					break;
				
				case RelativePosition.Top :
					Position = new Coordinate2(position.X - HalfWidth, position.Y);
					break;
				
				case RelativePosition.TopRight :
					Position = new Coordinate2(position.X - Width, position.Y);
					break;
				
				case RelativePosition.Left :
					Position = new Coordinate2(position.X, position.Y - HalfHeight);
					break;
				
				case RelativePosition.Right :
					Position = new Coordinate2(position.X - Width, position.Y - HalfHeight);
					break;
				
				case RelativePosition.BottomLeft :
					Position = new Coordinate2(position.X, position.Y - Height);
					break;
				
				case RelativePosition.Bottom :
					Position = new Coordinate2(position.X - HalfWidth, position.Y - Height);
					break;
				
				case RelativePosition.BottomRight :
					Position = new Coordinate2(position.X - Width, position.Y - Height);
					break;
				
				default :
					throw new NotImplementedException();
			}
		}
		
		public void AdjustPosition(Single horizontal, Single vertical)
		{
			Position = new Coordinate2(Position.X + horizontal, Position.Y + vertical);
		}
		
		public readonly Coordinate2 DefaultPosition = Coordinate2.X0Y0;
		
		#endregion
		
		#region Rotation
		
		private void InitializeRotation()
		{
			Rotation = DefaultRotation;
		}
		
		private void CleanupRotation()
		{
			Rotation = DefaultRotation;
		}
		
		private Angle2 _Rotation;
		public Angle2 Rotation
		{
			get { return _Rotation; }
			set
			{
				if(_Rotation == value)
					return;
				
				_Rotation = value;
				
				SetRecalcRequired();
			}
		}
		
		public readonly Angle2 DefaultRotation = Angle2.Zero;
		
		public void AdjustRotation(Single angle)
		{
			Rotation = new Angle2(Rotation.Degree + angle);
		}
		
		public const Single RotationCenterX = 0.5f;
		public const Single RotationCenterY = 0.5f;
		public const Single RotationCenterZ = 0.0f;
		
		#endregion
		
		#region Dimensions
		
		private void InitializeDimensions()
		{
			Width = DefaultWidth;
			Height = DefaultHeight;
		}
		
		private void CleanupDimensions()
		{
			Width = DefaultWidth;
			Height = DefaultHeight;
		}
		
		private Single _Width;
		public Single Width
		{
			get { return _Width; }
			set
			{
				if(_Width == value)
					return;
				
				_Width = value;
				HalfWidth = _Width / 2;
				
				SetRecalcRequired();
			}
		}
		
		private Single _Height;
		public Single Height
		{
			get { return _Height; }
			set
			{
				if(_Height == value)
					return;
				
				_Height = value;
				HalfHeight = _Height / 2;
				
				SetRecalcRequired();
			}
		}
		
		private Single HalfWidth;
		private Single HalfHeight;
		
//		private void SetDimensionsFromTile()
//		{
//			if (Key == null)
//			{
//				Width = DefaultWidth;
//				Height = DefaultHeight;
//				return;
//			}
//			
//			Width = Key.Tile.Width;
//			Height = Key.Tile.Height;
//		}
		
		private void SetDimensionsFromTile(Single scale = 1.000f)
		{
			if (Key == null)
			{
				Width = DefaultWidth;
				Height = DefaultHeight;
				return;
			}
			
			Width = Key.Tile.Width * scale;
			Height = Key.Tile.Height * scale;
		}
		
		private void SetDimensionsProportionallyFromWidth()
		{
			throw new NotImplementedException();
		}
		
		private void SetDimensionsProportionallyFromHeight()
		{
			throw new NotImplementedException();
		}
		
		public Int32 TileWidth
		{
			get
			{
				if (Key == null || Key.IsDisposed)
					return 0;
				
				return Key.Tile.Width;
			}
		}
		
		public Int32 TileHeight
		{
			get
			{
				if (Key == null || Key.IsDisposed)
					return 0;
				
				return Key.Tile.Height;
			}
		}
		
		private const Single DefaultWidth = 0.0f;
		
		private const Single DefaultHeight = 0.0f;
		
		#endregion
		
		#region Colors
		
		private void InitializeColors()
		{
			SetColorsToDefault();
		}
		
		private void CleanupColors()
		{
			SetColorsToDefault();
		}
		
		private Color _BackgroundColor;
		public Color BackgroundColor
		{
			get { return _BackgroundColor; }
			set
			{
				if(_BackgroundColor == value)
					return;
				
				_BackgroundColor = value;
				
				SetRecalcRequired();
			}
		}
		
		private Color _ForegroundColor;
		public Color ForegroundColor
		{
			get { return _ForegroundColor; }
			set
			{
				if(_ForegroundColor == value)
					return;
				
				_ForegroundColor = value;
				
				SetRecalcRequired();
			}
		}
		
		public void SetColorsToDefault()
		{
			BackgroundColor = Colors.Black;
			ForegroundColor = Colors.White;
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

