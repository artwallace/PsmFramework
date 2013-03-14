using System;
using System.Collections.Generic;
using PsmFramework.Engines.DrawEngine2d.Shaders;
using PsmFramework.Engines.DrawEngine2d.Support;
using PsmFramework.Engines.DrawEngine2d.Textures;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using System.Text;

namespace PsmFramework.Engines.DrawEngine2d.Drawables
{
	/// <summary>
	/// A simple text label class that draws strings using the hardcoded font data
	/// and does not support rotation.
	/// DebugLabel is always drawn on top of every other Drawable in a special Render pass.
	/// </summary>
	public class DebugLabel : DrawableBase
	{
		#region Constructor, Dispose
		
		public DebugLabel(LayerBase layer)
			: base(layer)
		{
		}
		
		#endregion
		
		#region Initialize, Cleanup
		
		protected override void Initialize()
		{
			InitializeRenderingCache();
			
			InitializeVertices();
			InitializeIndices();
			InitializeTextureCoordinates();
			InitializeColor();
			InitializeVertexBuffer();
			InitializeShaderProgram();
			
			InitializeScalingMatrixCache();
			InitializeRotationMatrixCache();
			InitializeTransformationMatrixCache();
		}
		
		protected override void Cleanup()
		{
			CleanupRenderingCache();
			
			CleanupScalingMatrixCache();
			CleanupRotationMatrixCache();
			CleanupTransformationMatrixCache();
			
			CleanupShaderProgram();
			CleanupVertexBuffer();
			CleanupColor();
			CleanupTextureCoordinates();
			CleanupIndices();
			CleanupVertices();
		}
		
		#endregion
		
		#region Render
		
		public override void Render()
		{
			//Set up the drawing
			
			//TODO: These need to be changed as little as possible
			DrawEngine2d.GraphicsContext.SetShaderProgram(Shader.ShaderProgram);
			DrawEngine2d.SetOpenGlTexture(DebugFont.TextureKey);
			
			if(RenderingRecacheRequired)
				GenerateCachedRendering();
			
			foreach(RenderingCacheData cacheData in CachedRendering)
			{
				TiledTexture tt = DrawEngine2d.GetTiledTexture(DebugFont.TextureKey);
				TiledTextureIndex index = DrawEngine2d.DebugFont.GetCharTileIndex(cacheData.CharCode);
				Single[] textureCoordinates = tt.GetTextureCoordinates(index);
				
				VertexBuffer.SetVertices(1, textureCoordinates);
				DrawEngine2d.GraphicsContext.SetVertexBuffer(0, VertexBuffer);
				
				Matrix4 scaleMatrix = GetScalingMatrix(1.0f);
				Matrix4 transMatrix = GetTranslationMatrix(cacheData.Position.X, cacheData.Position.Y, 1.0f, 0f);
				Matrix4 modelMatrix = transMatrix * scaleMatrix;
				Matrix4 worldViewProj = DrawEngine2d.WorldCameraProjectionMatrix * modelMatrix;
				
				Shader.ShaderProgram.SetUniformValue(0, ref worldViewProj);
				
				//TODO: this needs to be changed to be an array of VBOs, like ge2d.
				Layer.DrawEngine2d.GraphicsContext.DrawArrays(DrawMode.TriangleStrip, 0, IndexCount);
			}
			
			//Clean up the drawing
			//
		}
		
		#endregion
		
		#region Rendering Cache
		
		private void InitializeRenderingCache()
		{
			RenderingRecacheRequired = true;
		}
		
		private void CleanupRenderingCache()
		{
			CachedRendering = new RenderingCacheData[0];
		}
		
		private void GenerateCachedRendering()
		{
			RenderingRecacheRequired = false;
			
			if(String.IsNullOrWhiteSpace(Text))
			{
				CachedRendering = new RenderingCacheData[0];
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
			
			CachedRendering = new RenderingCacheData[charCount];
			
			Int32 cacheIndex = 0;
			Int32 lineNumber = 0;
			Int32 charOnThisLineNumber = 0;
			
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
				
				//TODO: Needs spacing added.
				//TODO: Add support for opposite Coordinate Mode here.
				
				if(DrawEngine2d.DebugFont.ContainsCharacterGlyph(c))
					CachedRendering[cacheIndex].CharCode = c;
				else
					CachedRendering[cacheIndex].CharCode = FallbackChar;
				CachedRendering[cacheIndex].Position.X = Position.X + (DebugFont.FontWidth * charOnThisLineNumber);
				CachedRendering[cacheIndex].Position.Y = Position.Y + (DebugFont.FontHeight * lineNumber);
				
				//Final things to do.
				cacheIndex++;
				charOnThisLineNumber++;
			}
		}
		
		private Boolean RenderingRecacheRequired;
		
		private RenderingCacheData[] CachedRendering;
		
		private struct RenderingCacheData
		{
			public Coordinate2 Position;
			public Char CharCode;
		}
		
		private const Char FallbackChar = '?';
		
		#endregion
		
		#region Text
		
		private String _Text;
		public String Text
		{
			get { return _Text; }
			set
			{
				if (_Text == value)
					return;
				
				MarkAsChanged();
				
				_Text = value;
			}
		}
		
		#endregion
		
		#region Position
		
		private Coordinate2 _Position;
		public Coordinate2 Position
		{
			get { return _Position; }
			set
			{
				if (_Position == value)
					return;
				
				MarkAsChanged();
				
				_Position = value;
			}
		}
		
		#endregion
		
		#region Bounds
		
		protected override void UpdateBounds()
		{
			throw new NotImplementedException();
		}
		
		#endregion
		
		
		
		//Old crap, need to fix.
		
		#region Vertices
		
		private void InitializeVertices()
		{
			Vertices = new Single[VertexCount * 3];
			
			VertexCoordinates_0_TopLeft = new Coordinate2(0.0f, 0.0f);
			VertexCoordinates_1_BottomLeft = new Coordinate2(0.0f, 1.0f);
			VertexCoordinates_2_BottomRight = new Coordinate2(1.0f, 1.0f);
			VertexCoordinates_3_TopRight = new Coordinate2(1.0f, 0.0f);
		}
		
		private void CleanupVertices()
		{
			Vertices = new Single[0];
		}
		
		private Single[] Vertices;
		
		private const Int32 VertexCount = 4;
		
		private const Single VertexZ = 0.0f;
		
		private Coordinate2 VertexCoordinates_0_TopLeft
		{
			get
			{
				return new Coordinate2(Vertices[0], Vertices[1]);
			}
			set
			{
				Vertices[0] = value.X;
				Vertices[1] = value.Y;
				Vertices[2] = VertexZ;
			}
		}
		
		private Coordinate2 VertexCoordinates_1_BottomLeft
		{
			get
			{
				return new Coordinate2(Vertices[3], Vertices[4]);
			}
			set
			{
				Vertices[3] = value.X;
				Vertices[4] = value.Y;
				Vertices[5] = VertexZ;
			}
		}
		
		private Coordinate2 VertexCoordinates_2_BottomRight
		{
			get
			{
				return new Coordinate2(Vertices[6], Vertices[7]);
			}
			set
			{
				Vertices[6] = value.X;
				Vertices[7] = value.Y;
				Vertices[8] = VertexZ;
			}
		}
		
		private Coordinate2 VertexCoordinates_3_TopRight
		{
			get
			{
				return new Coordinate2(Vertices[9], Vertices[10]);
			}
			set
			{
				Vertices[9] = value.X;
				Vertices[10] = value.Y;
				Vertices[11] = VertexZ;
			}
		}
		
		#endregion
		
		#region Indices
		
		//Vertex Rendering Order Indices
		
		private void InitializeIndices()
		{
			Indices = new UInt16[IndexCount];
			
			Indices[0] = 0;
			Indices[1] = 1;
			Indices[2] = 2;
			Indices[3] = 3;
		}
		
		private void CleanupIndices()
		{
			Indices = default(UInt16[]);
		}
		
		private const Int32 IndexCount = 4;
		
		private UInt16[] Indices;
		
		#endregion
		
		#region Texture Coordinates
		
		private void InitializeTextureCoordinates()
		{
			TextureCoordinates = new Single[4 * 2];
		}
		
		private void CleanupTextureCoordinates()
		{
			TextureCoordinates = default(Single[]);
		}
		
		private Single[] TextureCoordinates;
		
		#endregion
		
		#region Color
		
		private void InitializeColor()
		{
			VertexColors = new Single[VertexCount * 4];
			Color = Colors.White;
		}
		
		private void CleanupColor()
		{
			VertexColors = default(Single[]);
		}
		
		private Single[] VertexColors;
		
		private Color _Color;
		private Color Color
		{
			get { return _Color; }
			set
			{
				_Color = value;
				
				VertexColors[0] = _Color.R;
				VertexColors[1] = _Color.G;
				VertexColors[2] = _Color.B;
				VertexColors[3] = _Color.A;
				
				VertexColors[4] = _Color.R;
				VertexColors[5] = _Color.G;
				VertexColors[6] = _Color.B;
				VertexColors[7] = _Color.A;
				
				VertexColors[8] = _Color.R;
				VertexColors[9] = _Color.G;
				VertexColors[10] = _Color.B;
				VertexColors[11] = _Color.A;
				
				VertexColors[12] = _Color.R;
				VertexColors[13] = _Color.G;
				VertexColors[14] = _Color.B;
				VertexColors[15] = _Color.A;
			}
		}
		
		#endregion
		
		#region Vertex Buffer
		
		private void InitializeVertexBuffer()
		{
			VertexBuffer = new VertexBuffer(VertexCount, IndexCount, VertexFormat.Float3, VertexFormat.Float2, VertexFormat.Float4);
			
			VertexBuffer.SetVertices(0, Vertices);
			VertexBuffer.SetVertices(1, TextureCoordinates);
			VertexBuffer.SetVertices(2, VertexColors);
			VertexBuffer.SetIndices(Indices);
		}
		
		private void CleanupVertexBuffer()
		{
			VertexBuffer.Dispose();
			VertexBuffer = null;
		}
		
		private VertexBuffer VertexBuffer;
		
		#endregion
		
		#region Shader Program
		
		private void InitializeShaderProgram()
		{
			Shader = new SpriteShader(DrawEngine2d);
		}
		
		private void CleanupShaderProgram()
		{
			Shader.Dispose();
			Shader = null;
		}
		
		private SpriteShader Shader;
		
		#endregion
		
		#region Scaling Matrix Cache
		
		private void InitializeScalingMatrixCache()
		{
			ScalingMatrixCacheIndex = new Queue<Single>();
			
			ScalingMatrixCache = new Dictionary<Single, Matrix4>();
			
			ScalingMatrixCacheLimitFactor = 1;
		}
		
		private void CleanupScalingMatrixCache()
		{
			ScalingMatrixCacheIndex.Clear();
			ScalingMatrixCacheIndex = null;
			
			ScalingMatrixCache.Clear();
			ScalingMatrixCache = null;
		}
		
		private Queue<Single> ScalingMatrixCacheIndex;
		private Dictionary<Single, Matrix4> ScalingMatrixCache;
		
		private Int32 _ScalingMatrixCacheLimitFactor;
		public Int32 ScalingMatrixCacheLimitFactor
		{
			get { return _ScalingMatrixCacheLimitFactor; }
			set
			{
				if(value < 1)
					throw new ArgumentOutOfRangeException();
				_ScalingMatrixCacheLimitFactor = value;
			}
		}
		
		public Matrix4 GetScalingMatrix(Single scale)
		{
			if(!ScalingMatrixCache.ContainsKey(scale))
				GenerateScalingMatrix(scale);
			return ScalingMatrixCache[scale];
		}
		
		private void GenerateScalingMatrix(Single scale)
		{
			Single scaleX = DebugFont.FontWidth * scale;
			Single scaleY = DebugFont.FontHeight * scale;
			Single scaleZ = 1.0f;
			
			Vector3 scaleV = new Vector3(scaleX, scaleY, scaleZ);
			
			Matrix4 m = Matrix4.Scale(scaleV);
			
			//Add to cache
			ScalingMatrixCacheIndex.Enqueue(scale);
			ScalingMatrixCache.Add(scale, m);
			
			//Remove extra from cache
			Int32 limit = GetScalingMatrixLimit();
			while(ScalingMatrixCache.Count > limit)
			{
				Single old = ScalingMatrixCacheIndex.Dequeue();
				ScalingMatrixCache.Remove(old);
			}
		}
		
		private Int32 GetScalingMatrixLimit()
		{
			return Text.Length * ScalingMatrixCacheLimitFactor;
		}
		
		#endregion
		
		#region Rotation Matrix Cache
		
		private void InitializeRotationMatrixCache()
		{
			RotationMatrixCacheIndex = new Queue<Single>();
			
			RotationMatrixCache = new Dictionary<Single, Matrix4>();
			
			RotationMatrixCacheLimitFactor = 1;
		}
		
		private void CleanupRotationMatrixCache()
		{
			RotationMatrixCacheIndex.Clear();
			RotationMatrixCacheIndex = null;
			
			RotationMatrixCache.Clear();
			RotationMatrixCache = null;
		}
		
		private Queue<Single> RotationMatrixCacheIndex;
		private Dictionary<Single, Matrix4> RotationMatrixCache;
		
		private Int32 _RotationMatrixCacheLimitFactor;
		public Int32 RotationMatrixCacheLimitFactor
		{
			get { return _RotationMatrixCacheLimitFactor; }
			set
			{
				if(value < 1)
					throw new ArgumentOutOfRangeException();
				_RotationMatrixCacheLimitFactor = value;
			}
		}
		
		public Matrix4 GetRotationMatrix(Single angle)
		{
			if(!RotationMatrixCache.ContainsKey(angle))
				GenerateRotationMatrix(angle);
			return RotationMatrixCache[angle];
		}
		
		private void GenerateRotationMatrix(Single angle)
		{
			Single RadianAngle = DegreeToRadian(angle);
			Matrix4 m = Matrix4.RotationZ(RadianAngle);
			
			//Add to cache
			RotationMatrixCacheIndex.Enqueue(angle);
			RotationMatrixCache.Add(angle, m);
			
			//Remove extra from cache
			Int32 limit = GetRotationMatrixCacheLimit();
			while(RotationMatrixCache.Count > limit)
			{
				Single old = RotationMatrixCacheIndex.Dequeue();
				RotationMatrixCache.Remove(old);
			}
		}
		
		private Int32 GetRotationMatrixCacheLimit()
		{
			return Text.Length * RotationMatrixCacheLimitFactor;
		}
		
		#endregion
		
		#region Transformation Matrix Cache
		
		//TODO: This especially seems like it should be moved to the sprite.
		// There shouldn't be much if any benefit to a shared cache.
		// Should just cache it in each sprite.
		
		private void InitializeTransformationMatrixCache()
		{
			TranslationMatrixCacheIndex = new Queue<SpriteTranslationKey>();
			
			TranslationMatrixCache = new Dictionary<SpriteTranslationKey, Matrix4>();
			
			TranslationMatrixCacheLimitFactor = 1;
		}
		
		private void CleanupTransformationMatrixCache()
		{
			TranslationMatrixCacheIndex.Clear();
			TranslationMatrixCacheIndex = null;
			
			TranslationMatrixCache.Clear();
			TranslationMatrixCache = null;
		}
		
		private Queue<SpriteTranslationKey> TranslationMatrixCacheIndex;
		private Dictionary<SpriteTranslationKey, Matrix4> TranslationMatrixCache;
		
		private Int32 _TranslationMatrixCacheLimitFactor;
		public Int32 TranslationMatrixCacheLimitFactor
		{
			get { return _TranslationMatrixCacheLimitFactor; }
			set
			{
				if(value < 1)
					throw new ArgumentOutOfRangeException();
				_TranslationMatrixCacheLimitFactor = value;
			}
		}
		
		public Matrix4 GetTranslationMatrix(Single x, Single y, Single scale, Single angle)
		{
			SpriteTranslationKey key = new SpriteTranslationKey(x, y, scale, angle);
			
			if(!TranslationMatrixCache.ContainsKey(key))
				GenerateTranslationMatrix(key);
			return TranslationMatrixCache[key];
		}
		
		private void GenerateTranslationMatrix(SpriteTranslationKey key)
		{
			Single RadianAngle = DegreeToRadian(key.Angle);
			
			//TODO: Verify that these formulas are correct.
			Single x = key.X + (key.Scale * (FMath.Sin(RadianAngle) - FMath.Cos(RadianAngle)));
			Single y = key.Y + (key.Scale * (FMath.Cos(RadianAngle) + FMath.Sin(RadianAngle)));
			Single z = 0.0f;
			
			Vector3 transV = new Vector3(x, y, z);
			
			Matrix4 m = Matrix4.Translation(transV);
			
			//Add to cache
			TranslationMatrixCacheIndex.Enqueue(key);
			TranslationMatrixCache.Add(key, m);
			
			//Remove extra from cache
			Int32 limit = GetTranslationMatrixCacheLimit();
			while(TranslationMatrixCache.Count > limit)
			{
				SpriteTranslationKey old = TranslationMatrixCacheIndex.Dequeue();
				TranslationMatrixCache.Remove(old);
			}
		}
		
		private Int32 GetTranslationMatrixCacheLimit()
		{
			return Text.Length * TranslationMatrixCacheLimitFactor;
		}
		
		#endregion
		
		#region Angle utilities
		
		//TODO: Move to a generic math class.
		
		//TODO: Verify that this formula is correct.
		private static Single DegreeToRadianValue = (Single)(Math.PI / 180D);
		private static Single DegreeToRadian(Single angle)
		{
			return angle * DegreeToRadianValue;
		}
		
		#endregion
	}
}

