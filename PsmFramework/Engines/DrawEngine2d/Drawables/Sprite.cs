using System;
using PsmFramework.Engines.DrawEngine2d.Layers;
using PsmFramework.Engines.DrawEngine2d.Shaders;
using PsmFramework.Engines.DrawEngine2d.Support;
using PsmFramework.Engines.DrawEngine2d.Textures;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;

namespace PsmFramework.Engines.DrawEngine2d.Drawables
{
	public class Sprite : DrawableBase
	{
		#region Constructor, Dispose
		
		public Sprite(LayerBase layer, TiledTexture tiledTexture, TiledTextureIndex index)
			: base(layer)
		{
			SetTiledTexture(tiledTexture, index);
		}
		
		#endregion
		
		#region Initialize, Cleanup
		
		protected override void Initialize()
		{
			InitializeShaderProgram();
			InitializeTiledTexture();
			
			InitializePosition();
			InitializeDimensions();
			InitializeColors();
		}
		
		protected override void Cleanup()
		{
			CleanupColors();
			CleanupDimensions();
			CleanupPosition();
			
			CleanupTiledTexture();
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
		
		#region TiledTexture
		
		private void InitializeTiledTexture()
		{
			TiledTexture = null;
			TiledTextureIndex = default(TiledTextureIndex);
		}
		
		private void CleanupTiledTexture()
		{
			UnregisterAsUserOfTiledTexture();
			TiledTexture = null;
			TiledTextureIndex = default(TiledTextureIndex);
		}
		
		private TiledTexture TiledTexture;
		
		private TiledTextureIndex _TiledTextureIndex;
		public TiledTextureIndex TiledTextureIndex
		{
			get { return _TiledTextureIndex; }
			set
			{
				if (_TiledTextureIndex == value)
					return;
				
				_TiledTextureIndex = value;
				
				SetRecalcRequired();
			}
		}
		
		private void SetTiledTexture(TiledTexture tiledTexture, TiledTextureIndex index)
		{
			if(tiledTexture == null)
				throw new ArgumentNullException();
			
			if (TiledTexture != null)
			{
				UnregisterAsUserOfTiledTexture();
				TiledTexture = null;
			}
			
			TiledTexture = tiledTexture;
			TiledTextureIndex = index;
			
			RegisterAsUserOfTiledTexture();
		}
		
		private void RegisterAsUserOfTiledTexture()
		{
			DrawEngine2d.AddTiledTextureUser(TiledTexture.Key, this);
		}
		
		private void UnregisterAsUserOfTiledTexture()
		{
			DrawEngine2d.RemoveTiledTextureUser(TiledTexture.Key, this);
		}
		
//		public Single[] GetTiledTextureCoordinates(TiledTextureIndex index, out Int32 width, out Int32 height)
//		{
//			return TiledTexture.GetTextureCoordinates(index, out width, out height);
//		}
		
		#endregion
		
		#region Position
		
		private void InitializePosition()
		{
			Position = Coordinate2.X0Y0;
		}
		
		private void CleanupPosition()
		{
			Position = Coordinate2.X0Y0;
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
			throw new NotImplementedException();
		}
		
		#endregion
		
		#region Dimensions
		
		private void InitializeDimensions()
		{
			Width = 0.0f;
			Height = 0.0f;
		}
		
		private void CleanupDimensions()
		{
			Width = 0;
			Height = 0;
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
				
				SetRecalcRequired();
			}
		}
		
		private void SetDimensionsFromTile()
		{
			throw new NotImplementedException();
		}
		
		private void SetWidth()
		{
			throw new NotImplementedException();
		}
		
		private void SetHeight()
		{
			throw new NotImplementedException();
		}
		
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
			throw new NotImplementedException();
		}
		
		#endregion
		
		#region Recalc
		
		protected override void RecalcBounds()
		{
			throw new NotImplementedException();
		}
		
		protected override void RecalcHelper()
		{
			throw new NotImplementedException();
		}
		
		#endregion
	}
}

