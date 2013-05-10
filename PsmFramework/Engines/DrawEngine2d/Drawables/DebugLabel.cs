using System;
using System.Text;
using PsmFramework.Engines.DrawEngine2d.Layers;
using PsmFramework.Engines.DrawEngine2d.Shaders;
using PsmFramework.Engines.DrawEngine2d.Support;
using PsmFramework.Engines.DrawEngine2d.TiledTextures;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;

namespace PsmFramework.Engines.DrawEngine2d.Drawables
{
	/// <summary>
	/// A text label class that draws strings using the hardcoded font data.
	/// DebugLabel is always drawn on top of every other Drawable in a special Render pass.
	/// </summary>
	public sealed class DebugLabel : SpriteDrawableBase, IDebugInfo
	{
		//TODO: Needs an auto Scale based on screen ppi.
		
		#region Factory
		
		internal static DebugLabel CreateDebugLabel(DrawEngine2d drawEngine2d, LayerType type, IDebuggable parent = null)
		{
			if (drawEngine2d == null || drawEngine2d.IsDisposed)
				throw new ArgumentNullException();
			
			LayerBase layer;
			
			switch (type)
			{
				case LayerType.Screen :
					layer = drawEngine2d.Layers.GetOrCreateScreenDebugLayer();
					break;
				case LayerType.World :
					layer = drawEngine2d.Layers.GetOrCreateWorldDebugLayer();
					break;
				default :
					throw new ArgumentException();
			}
			
			return new DebugLabel(layer, parent);
		}
		
		#endregion
		
		#region ToggleDebugInfo
		
//		public static void ToggleDebugInfo(Boolean enabled, IDebuggable source, ref IDebugInfo debugInfo, ref IDisposablePlus debugInfoDisposer)
//		{
//		}
		
		#endregion
		
		#region Constructor, Dispose
		
		private DebugLabel(LayerBase layer, IDebuggable parent)
			: base(layer)
		{
			SetParent(parent);
		}
		
		#endregion
		
		#region Initialize, Cleanup
		
		protected override void Initialize()
		{
			InitializeCharacterCoordinateCache();
			InitializeText();
			InitializeParent();
			InitializeIDebugInfo();
		}
		
		protected override void Cleanup()
		{
			CleanupIDebugInfo();
			CleanupParent();
			CleanupText();
			CleanupCharacterCoordinateCache();
		}
		
		#endregion
		
		#region Render
		
		public override void RenderHelper()
		{
			//TODO: These need to be changed as little as possible
			DrawEngine2d.GraphicsContext.SetShaderProgram(Shader.ShaderProgram);
			DrawEngine2d.Textures.SetOpenGlTexture(DebugFont.TextureKey);
			
			//This seems like a terrible place to have this!!!
			if (RefreshNeeded)
			{
				ClearDebugInfo();
				Parent.RefreshDebugInfo();
				Text = DebugInfoBuilder.ToString();
			}
			
			if (RecalcRenderCacheRequired)
				RecalcRenderCache();
			
			TiledTexture tt = DrawEngine2d.TiledTextures.GetTiledTexture(DebugFont.TextureKey);
			
			foreach(RenderCacheData cacheData in RenderCache)
			{
				Single[] textureCoordinates = DrawEngine2d.DebugFont.GetCharTextureCoordinates(cacheData.Character);
				
				Shader.VertexBuffer.SetVertices(1, textureCoordinates);
				DrawEngine2d.GraphicsContext.SetVertexBuffer(0, Shader.VertexBuffer);
				
				Matrix4 sm = Matrix4.Scale(DebugFont.FontWidth, DebugFont.FontHeight, 0.0f);
				Matrix4 tm = Matrix4.Translation(cacheData.Position.X, cacheData.Position.Y, 0.0f);
				Matrix4 modelMatrix = tm * sm;
				
				Matrix4 worldViewProj = Layer.Camera.ProjectionMatrix * modelMatrix;
				
				Shader.ShaderProgram.SetUniformValue(0, ref worldViewProj);
				
				//TODO: this needs to be changed to be an array of VBOs, like ge2d.
				Layer.DrawEngine2d.IncrementDrawArrayCallsCounter();
				Layer.DrawEngine2d.GraphicsContext.DrawArrays(DrawMode.TriangleStrip, 0, SpriteShader.IndexCount);
			}
		}
		
		#endregion
		
		#region Position
		
		private void SetPositionFromParent()
		{
			if (IsDisposed || Parent == null)
			{
				SetPosition(DefaultPosition);
				return;
			}
			
			//TODO: Support coordinatesystemmode here.
			
			switch (RelativePosition)
			{
				case RelativePosition.TopLeft :
					SetPosition(Parent.Bounds.TopLeft);
					break;
				case RelativePosition.Top :
					SetPosition(Parent.Bounds.TopCenter);
					break;
				case RelativePosition.TopRight :
					SetPosition(Parent.Bounds.TopRight);
					break;
					
				case RelativePosition.Left :
					SetPosition(Parent.Bounds.CenterLeft);
					break;
				case RelativePosition.Center :
					SetPosition(Parent.Bounds.Center);
					break;
				case RelativePosition.Right :
					SetPosition(Parent.Bounds.CenterRight);
					break;
					
				case RelativePosition.BottomLeft :
					SetPosition(Parent.Bounds.BottomLeft);
					break;
				case RelativePosition.Bottom :
					SetPosition(Parent.Bounds.BottomCenter);
					break;
				case RelativePosition.BottomRight :
					SetPosition(Parent.Bounds.BottomRight);
					break;
			}
		}
		
		#endregion
		
		#region Recalc
		
		protected override void RecalcBounds()
		{
			//TODO: throw new NotImplementedException();
			
			if (RenderTextRequiresRecalc)
				GenerateRenderText();
			
			//TODO: get parents bounds.
			//TODO: calc bounds based on TextAlignment, RelativePosition, etc.
		}
		
		protected override void RecalcHelper()
		{
			SetPositionFromParent();
			
			if (RenderTextRequiresRecalc)
				GenerateRenderText();
			
			SetRenderRecacheRequired();
		}
		
		#endregion
		
		#region CharacterCoordinateCache
		
		private void InitializeCharacterCoordinateCache()
		{
			SetRenderRecacheRequired();
		}
		
		private void CleanupCharacterCoordinateCache()
		{
			ClearRenderRecacheRequired();
			RenderCache = null;
		}
		
		private void RecalcRenderCache()
		{
			ClearRenderRecacheRequired();
			
			if(String.IsNullOrWhiteSpace(Text))
			{
				RenderCache = new RenderCacheData[0];
				SetNaturalDimensions(DefaultNaturalWidth, DefaultNaturalHeight);
				return;
			}
			
			Int32 charCount = 0;
			
			foreach(Char c in Text)
			{
				if(c == '\n' || c == '\r')
					continue;
				else
					charCount++;
			}
			
			RenderCache = new RenderCacheData[charCount];
			
			Int32 cacheIndex = 0;
			Int32 lineNumber = 0;
			Int32 charOnThisLineNumber = 0;
			Int32 maxCharsOnLine = 0;
			
			foreach(Char c in Text)
			{
				if(c == '\n')
				{
					lineNumber++;
					charOnThisLineNumber = 0;
					continue;
				}
				else if(c == '\r')
					continue;
				
				//TODO: Add support for opposite Coordinate Mode here.
				
				if (DrawEngine2d.DebugFont.ContainsCharacterGlyph(c))
					RenderCache[cacheIndex].Character = c;
				else
					RenderCache[cacheIndex].Character = FallbackChar;
				
				Single x = Position.X + ((DebugFont.FontWidth + SpacingAdjustmentX) * charOnThisLineNumber);
				Single y = Position.Y + ((DebugFont.FontHeight + SpacingAdjustmentY) * lineNumber);
				RenderCache[cacheIndex].Position = new Coordinate2(x, y);
				
				//Final things to do.
				cacheIndex++;
				charOnThisLineNumber++;
				
				if (maxCharsOnLine < charOnThisLineNumber)
					maxCharsOnLine = charOnThisLineNumber;
			}
			
			SetNaturalDimensions(maxCharsOnLine * DebugFont.FontWidth, lineNumber * DebugFont.FontHeight);
		}
		
		private Boolean RecalcRenderCacheRequired;
		
		private RenderCacheData[] RenderCache;
		
		private struct RenderCacheData
		{
			public Coordinate2 Position;
			public Char Character;
			public override String ToString()
			{
				return Character.ToString() + " " + Position.ToString();
			}
		}
		
		private void SetRenderRecacheRequired()
		{
			RecalcRenderCacheRequired = true;
		}
		
		private void ClearRenderRecacheRequired()
		{
			RecalcRenderCacheRequired = false;
		}
		
		//Used to tweak the layout. Copies spacing seen in GameEngine2d.
		private const Int32 SpacingAdjustmentX = -1;
		private const Int32 SpacingAdjustmentY = 1;
		
		#endregion
		
		#region Text
		
		private void InitializeText()
		{
			Text = null;
		}
		
		private void CleanupText()
		{
			Text = null;
		}
		
		private String _Text;
		public String Text
		{
			get { return _Text; }
			private set
			{
				if (_Text == value)
					return;
				
				_Text = value;
				
				SetRenderTextRecalcRequired();
				SetRecalcRequired();
			}
		}
		
		private Char[] RenderText;
		
		private Int32 RenderTextCharCount;
		private Int32 RenderTextLineCount;
		private Int32 RenderTextMaxLineLength;
		
		private const Char FallbackChar = '?';
		
		private void GenerateRenderText()
		{
			ClearRenderTextRecalcRequired();
			
			if (IsDisposed)
				return;
			
			RenderTextCharCount = 0;
			RenderTextLineCount = 0;
			RenderTextMaxLineLength = 0;
			
			Int32 maxLength = String.IsNullOrWhiteSpace(Text) ? 0 : Text.Length;
			
			RenderText = new Char[maxLength];
			if (maxLength == 0)
				return;
			
			//At this point, we have at least one char so start with one line
			RenderTextLineCount++;
			
			Int32 charsOnThisLine = 0;
			
			foreach (Char c in Text)
			{
				//Windows only, ignore
				if (c == '\r')
					continue;
				
				//Linux & Windows
				if (c == '\n')
				{
					RenderTextLineCount++;
					charsOnThisLine = 0;
				}
				
				RenderTextCharCount++;
				
				charsOnThisLine++;
				if (RenderTextMaxLineLength < charsOnThisLine)
					RenderTextMaxLineLength = charsOnThisLine;
				
				RenderText[RenderTextCharCount - 1] = DrawEngine2d.DebugFont.ContainsCharacterGlyph(c) ? c : FallbackChar;
			}
		}
		
		private Boolean RenderTextRequiresRecalc;
		
		private void SetRenderTextRecalcRequired()
		{
			RenderTextRequiresRecalc = true;
		}
		
		private void ClearRenderTextRecalcRequired()
		{
			RenderTextRequiresRecalc = false;
		}
		
		#endregion
		
		#region Parent
		
		private void InitializeParent()
		{
			Parent = null;
		}
		
		private void CleanupParent()
		{
			Parent = null;
		}
		
		private IDebuggable Parent;
		
		private void SetParent(IDebuggable parent)
		{
			Parent = parent;
		}
		
		public void ParentUpdated()
		{
			SetRecalcRequired();
		}
		
		#endregion
		
		#region IDebugInfo
		
		private void InitializeIDebugInfo()
		{
			RefreshForcesRender = true;
			RefreshInterval = TimeSpan.Zero;
			
			RelativePosition = RelativePosition.Center;
			PlacementPosition = PlacementPosition.Inside;
			TextAlignment = TextAlignment.Center;
			
			DebugInfoBuilder = new StringBuilder();
		}
		
		private void CleanupIDebugInfo()
		{
			RefreshForcesRender = true;
			RefreshInterval = TimeSpan.Zero;
			
			RelativePosition = RelativePosition.Center;
			PlacementPosition = PlacementPosition.Inside;
			TextAlignment = TextAlignment.Center;
			
			DebugInfoBuilder.Clear();
			DebugInfoBuilder = null;
		}
		
		public Boolean RefreshForcesRender { get; set; }
		public TimeSpan RefreshInterval { get; set; }
		public DateTime LastRefresh { get; private set; }
		public Boolean RefreshNeeded { get; private set; }
		public void CalcIfRefreshNeeded(DateTime updateTime)
		{
			if (RefreshNeeded)
				return;
			
			if (IsDisposed)
				return;
			
			if (updateTime - LastRefresh > RefreshInterval)
				RefreshNeeded = true;
		}
		
		//TODO: Move these to region position?
		//TODO: force redraw when changed.
		private RelativePosition _RelativePosition;
		public RelativePosition RelativePosition
		{
			get { return _RelativePosition; }
			set
			{
				if (_RelativePosition == value)
					return;
				
				_RelativePosition = value;
				
				SetRecalcRequired();
			}
		}
		
		//TODO: Move these to region position?
		//TODO: force redraw when changed.
		private PlacementPosition _PlacementPosition;
		public PlacementPosition PlacementPosition
		{
			get { return _PlacementPosition; }
			set
			{
				if (_PlacementPosition == value)
					return;
				
				_PlacementPosition = value;
				
				SetRecalcRequired();
			}
		}
		
		private TextAlignment _TextAlignment;
		public TextAlignment TextAlignment
		{
			get { return _TextAlignment; }
			set
			{
				if (_TextAlignment == value)
					return;
				
				_TextAlignment = value;
				
				SetRecalcRequired();
			}
		}
		
		private StringBuilder DebugInfoBuilder;
		
		private const String DebugInfoSeparator = ": ";
		
		private void ClearDebugInfo()
		{
			if (DebugInfoBuilder != null)
				DebugInfoBuilder.Clear();
		}
		
		public void AddDebugInfoLine(String name, String data)
		{
			DebugInfoBuilder.Append(name);
			DebugInfoBuilder.Append(DebugInfoSeparator);
			DebugInfoBuilder.AppendLine(data);
		}
		
		public void AddDebugInfoLine(String name, Int32 data)
		{
			DebugInfoBuilder.Append(name);
			DebugInfoBuilder.Append(DebugInfoSeparator);
			DebugInfoBuilder.Append(data);
			DebugInfoBuilder.AppendLine();
		}
		
		public void AddDebugInfoLine(String name, Int64 data)
		{
			DebugInfoBuilder.Append(name);
			DebugInfoBuilder.Append(DebugInfoSeparator);
			DebugInfoBuilder.Append(data);
			DebugInfoBuilder.AppendLine();
		}
		
		public void AddDebugInfoLine(String name, Single data)
		{
			DebugInfoBuilder.Append(name);
			DebugInfoBuilder.Append(DebugInfoSeparator);
			DebugInfoBuilder.Append(data.ToString());
			DebugInfoBuilder.AppendLine();
		}
		
		public void AddDebugInfoLine(String name, Coordinate2 data)
		{
			DebugInfoBuilder.Append(name);
			DebugInfoBuilder.Append(DebugInfoSeparator);
			DebugInfoBuilder.Append(data.ToString());
			DebugInfoBuilder.AppendLine();
		}
		
		public void AddDebugInfoLine(String name, Coordinate2i data)
		{
			DebugInfoBuilder.Append(name);
			DebugInfoBuilder.Append(DebugInfoSeparator);
			DebugInfoBuilder.Append(data.ToString());
			DebugInfoBuilder.AppendLine();
		}
		
		public void AddDebugInfoLine(String name, RectangularArea2 data)
		{
			DebugInfoBuilder.Append(name);
			DebugInfoBuilder.Append(DebugInfoSeparator);
			DebugInfoBuilder.Append(data.ToString());
			DebugInfoBuilder.AppendLine();
		}
		
		public void AddDebugInfoLine(String name, RectangularArea2i data)
		{
			DebugInfoBuilder.Append(name);
			DebugInfoBuilder.Append(DebugInfoSeparator);
			DebugInfoBuilder.Append(data.ToString());
			DebugInfoBuilder.AppendLine();
		}
		
		public void AddDebugInfoLine(String name, Angle2 data)
		{
			DebugInfoBuilder.Append(name);
			DebugInfoBuilder.Append(DebugInfoSeparator);
			DebugInfoBuilder.Append(data.ToString());
			DebugInfoBuilder.AppendLine();
		}
		
		#endregion
	}
}

