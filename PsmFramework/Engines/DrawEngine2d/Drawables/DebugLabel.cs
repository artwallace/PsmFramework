using System;
using PsmFramework.Engines.DrawEngine2d.Layers;
using PsmFramework.Engines.DrawEngine2d.Shaders;
using PsmFramework.Engines.DrawEngine2d.Support;
using PsmFramework.Engines.DrawEngine2d.Textures;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;

namespace PsmFramework.Engines.DrawEngine2d.Drawables
{
	/// <summary>
	/// A simple text label class that draws strings using the hardcoded font data.
	/// DebugLabel is always drawn on top of every other Drawable in a special Render pass.
	/// </summary>
	public sealed class DebugLabel : SpriteDrawableBase
	{
		//TODO: Needs an auto Scale based on screen size.
		
		#region Factory
		
		public static DebugLabel CreateDebugLabel(DrawEngine2d drawEngine2d, LayerType type)
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
			
			return new DebugLabel(layer);
		}
		
		#endregion
		
		#region Constructor, Dispose
		
		private DebugLabel(LayerBase layer)
			: base(layer)
		{
		}
		
		#endregion
		
		#region Initialize, Cleanup
		
		protected override void Initialize()
		{
			InitializeCharacterCoordinateCache();
			InitializeText();
		}
		
		protected override void Cleanup()
		{
			CleanupText();
			CleanupCharacterCoordinateCache();
		}
		
		#endregion
		
		#region Render
		
		public override void RenderHelper()
		{
			//TODO: These need to be changed as little as possible
			DrawEngine2d.GraphicsContext.SetShaderProgram(Shader.ShaderProgram);
			DrawEngine2d.SetOpenGlTexture(DebugFont.TextureKey);
			
			if(RecalcRenderCacheRequired)
				RecalcRenderCache();
			
			TiledTexture tt = DrawEngine2d.GetTiledTexture(DebugFont.TextureKey);
			
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
		
		#region Recalc
		
		protected override void RecalcBounds()
		{
			//TODO: throw new NotImplementedException();
		}
		
		protected override void RecalcHelper()
		{
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
			set
			{
				if (_Text == value)
					return;
				
				_Text = value;
				
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
			if (IsDisposed)
				return;
			
			Int32 maxLength;
			
			RenderTextCharCount = 0;
			RenderTextLineCount = 0;
			RenderTextMaxLineLength = 0;
			
			maxLength = String.IsNullOrWhiteSpace(Text) ? 0 : Text.Length;
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
		
		#endregion
	}
}

