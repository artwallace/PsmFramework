using System;
using System.Collections.Generic;
using PsmFramework.Engines.DrawEngine2d.Layers;
using PsmFramework.Engines.DrawEngine2d.Shaders;
using PsmFramework.Engines.DrawEngine2d.Support;
using PsmFramework.Engines.DrawEngine2d.Textures;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Imaging;

namespace PsmFramework.Engines.DrawEngine2d.Drawables
{
	//Slow, one-off image drawing class with zoom and pan features.
	public class Image : DrawableBase
	{
		#region Constructor, Dispose
		
		public Image(LayerBase layer, String path = null)
			: base(layer)
		{
			SetSourceImage(path);
		}
		
		#endregion
		
		#region Initialize, Cleanup
		
		protected override void Initialize()
		{
			InitializeShaderProgram();
			InitializeTexture();
			InitializeSourceImage();
			
			InitializePosition();
			InitializeDimensions();
			InitializeColors();
		}
		
		protected override void Cleanup()
		{
			CleanupColors();
			CleanupDimensions();
			CleanupPosition();
			
			CleanupSourceImage();
			CleanupTexture();
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
		
		#region Texture
		
		private void InitializeTexture()
		{
			//Texture = null;
		}
		
		private void CleanupTexture()
		{
			ClearTexture();//TODO: should we move the code to this method?
		}
		
		//private Texture2dPlus Texture;
		
		private void SetTexture()
		{
		}
		
		private void ClearTexture()
		{
		}
		
		#endregion
		
		#region SourceImage
		
		private void InitializeSourceImage()
		{
			SetSourceImage(null);
		}
		
		private void CleanupSourceImage()
		{
			ClearSourceImage();
		}
		
		public void SetSourceImage(String path)
		{
			Path = path;
			//TODO: Do something with the texture here.
			SetTexture();
			
			Sce.PlayStation.Core.Imaging.Image i = new Sce.PlayStation.Core.Imaging.Image(path);
			//i.DrawImage();
		}
		
		public void ClearSourceImage()
		{
			Path = null;
			//TODO: Do something with the texture here.
			ClearTexture();
		}
		
		public String Path { get; private set; }
		
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
		
		public void SetPositionFromCenter()
		{
		}
		
		public void SetPositionFromUpperLeft()
		{
		}
		
		public void SetPositionFromLowerLeft()
		{
		}
		
//		private void SetPosition()
//		{
//		}
		
		#endregion
		
		#region Dimensions
		
		private void InitializeDimensions()
		{
		}
		
		private void CleanupDimensions()
		{
		}
		
		private Coordinate2 _Width;
		public Coordinate2 Width
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
		
		private Coordinate2 _Height;
		public Coordinate2 Height
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
		}
		
		#endregion
		
		#region Recalc
		
		protected override void RecalcBounds()
		{
			//TODO: throw new NotImplementedException();
		}
		
		protected override void RecalcHelper()
		{
			//TODO: throw new NotImplementedException();
		}
		
		#endregion
	}
}

