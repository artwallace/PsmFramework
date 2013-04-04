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
		
		public Sprite(LayerBase layer, IndexKey key)
			: base(layer)
		{
			SetTiledTexture(key);
		}
		
		#endregion
		
		#region Initialize, Cleanup
		
		protected override void Initialize()
		{
			InitializeShaderProgram();
			InitializeTiledTextureIndex();
			
			InitializePosition();
			InitializeDimensions();
			InitializeColors();
		}
		
		protected override void Cleanup()
		{
			CleanupColors();
			CleanupDimensions();
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
		}
		
		private void CleanupTiledTextureIndex()
		{
			UnregisterAsUserOfTiledTexture();
			
			Key = default(IndexKey);
		}
		
		private IndexKey _Key;
		public IndexKey Key
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
		
		private void SetTiledTexture(IndexKey key)
		{
			if(key.Index == null)
				throw new ArgumentNullException();
			
			if (Key.Index != null)
				UnregisterAsUserOfTiledTexture();
			
			Key = key;
			
			RegisterAsUserOfTiledTexture();
		}
		
		private void RegisterAsUserOfTiledTexture()
		{
			if (Key.Index == null)
				throw new InvalidOperationException();
			
			if (Key.Index.TiledTexture == null)
				throw new InvalidOperationException();
			
			DrawEngine2d.AddTiledTextureUser(Key.Index.TiledTexture.Key, this);
		}
		
		private void UnregisterAsUserOfTiledTexture()
		{
			if (Key.Index.TiledTexture == null)
				return;
			
			DrawEngine2d.RemoveTiledTextureUser(Key.Index.TiledTexture.Key, this);
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
		
		private void SetDimensionsFromWidth()
		{
			throw new NotImplementedException();
		}
		
		private void SetDimensionsFromHeight()
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

