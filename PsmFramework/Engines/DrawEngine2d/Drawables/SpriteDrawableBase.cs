using System;
using PsmFramework.Engines.DrawEngine2d.Layers;
using PsmFramework.Engines.DrawEngine2d.Shaders;
using PsmFramework.Engines.DrawEngine2d.Support;
using Sce.PlayStation.Core;

namespace PsmFramework.Engines.DrawEngine2d.Drawables
{
	public abstract class SpriteDrawableBase : DrawableBase
	{
		#region Constructor, Dispose
		
		public SpriteDrawableBase(LayerBase layer)
			: base(layer)
		{
		}
		
		#endregion
		
		#region Initialize, Cleanup
		
		protected override void InitializeIntermediary()
		{
			InitializeShaderProgram();
			InitializePosition();
			InitializeRotation();
			InitializeDimensions();
			InitializeColors();
		}
		
		protected override void CleanupIntermediary()
		{
			CleanupColors();
			CleanupDimensions();
			CleanupRotation();
			CleanupPosition();
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
		
		protected SpriteShader Shader;
		
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
		
		public void SetPosition(Single x, Single y, RelativePosition relativeTo = RelativePosition.Center)
		{
			SetPosition(new Coordinate2(x, y), relativeTo);
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
			private set
			{
				if(_Rotation == value)
					return;
				
				_Rotation = value;
				
				SetRecalcRequired();
			}
		}
		
		protected void SetRotation(Angle2 angle)
		{
			Rotation = angle;
		}
		
		protected void AdjustRotation(Angle2 angle)
		{
			Rotation = new Angle2(Rotation.Degree + angle.Degree);
		}
		
		public readonly Angle2 DefaultRotation = Angle2.Zero;
		
		protected const Single RotationCenterX = 0.5f;
		protected const Single RotationCenterY = 0.5f;
		protected const Single RotationCenterZ = 0.0f;
		
		#endregion
		
		#region Dimensions
		
		private void InitializeDimensions()
		{
			Width = DefaultWidth;
			Height = DefaultHeight;
			
			NaturalWidth = DefaultNaturalWidth;
			NaturalHeight = DefaultNaturalHeight;
		}
		
		private void CleanupDimensions()
		{
			Width = DefaultWidth;
			Height = DefaultHeight;
			
			NaturalWidth = DefaultNaturalWidth;
			NaturalHeight = DefaultNaturalHeight;
		}
		
		private Single _Width;
		public Single Width
		{
			get { return _Width; }
			private set
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
			private set
			{
				if(_Height == value)
					return;
				
				_Height = value;
				HalfHeight = _Height / 2;
				
				SetRecalcRequired();
			}
		}
		
		protected Single HalfWidth { get; private set; }
		protected Single HalfHeight { get; private set; }
		
		public Single NaturalWidth { get; private set; }
		public Single NaturalHeight { get; private set; }
		
		protected void SetNaturalDimensions(Single width, Single height)
		{
			if (width < 0.0f || height < 0.0f)
				throw new ArgumentOutOfRangeException();
			
			NaturalWidth = width;
			NaturalHeight = height;
			
			SetRecalcRequired();
		}
		
		protected void SetDimensions()
		{
			Width = NaturalWidth;
			Height = NaturalHeight;
		}
		
		protected void SetDimensionsByScale(Single scale)
		{
			if (scale < 0.0f)
				throw new ArgumentOutOfRangeException();
			
			Width = NaturalWidth * scale;
			Height = NaturalHeight * scale;
		}
		
		protected void SetDimensionsProportionallyByWidth(Single width)
		{
			if (width < 1f || NaturalWidth < 1f)
			{
				Width = DefaultWidth;
				Height = DefaultHeight;
				return;
			}
			
			Single scale = width / NaturalWidth;
			
			Width = width;
			Height = NaturalHeight * scale;
		}
		
		protected void SetDimensionsProportionallyByHeight(Single height)
		{
			if (height < 1f || NaturalHeight < 1f)
			{
				Width = DefaultWidth;
				Height = DefaultHeight;
				return;
			}
			
			Single scale = height / NaturalHeight;
			
			Width = NaturalWidth * scale;
			Height = height;
		}
		
		protected const Single DefaultWidth = 0.0f;
		protected const Single DefaultHeight = 0.0f;
		
		protected const Single DefaultNaturalWidth = 0.0f;
		protected const Single DefaultNaturalHeight = 0.0f;
		
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
	}
}

