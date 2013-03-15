using System;
using System.Collections.Generic;
using System.Diagnostics;
using PsmFramework.Engines.DrawEngine2d.Layers;
using PsmFramework.Engines.DrawEngine2d.Shaders;
using PsmFramework.Engines.DrawEngine2d.Support;
using PsmFramework.Engines.DrawEngine2d.Textures;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;

namespace PsmFramework.Engines.DrawEngine2d.Drawables
{
	public sealed class SpriteGroup : DrawableBase
	{
		//This class will evolve into a more advanced sprite class with more
		// complicated culling due to rotation and scaling.
		//TODO: A lot of the calcs need to be moved to the sprite class and only recalced when the props change.
		
		#region Constructor, Dispose
		
		public SpriteGroup(LayerBase layer, TiledTexture tiledTexture)
			: base(layer)
		{
			InitializeCustom(tiledTexture);
		}
		
		#endregion
		
		#region Initialize, Cleanup
		
		private void InitializeCustom(TiledTexture tiledTexture)
		{
			InitializeTiledTexture(tiledTexture);
			InitializeSprites();
			
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
			CleanupScalingMatrixCache();
			CleanupRotationMatrixCache();
			CleanupTransformationMatrixCache();
			
			CleanupShaderProgram();
			CleanupVertexBuffer();
			CleanupColor();
			CleanupTextureCoordinates();
			CleanupIndices();
			CleanupVertices();
			
			CleanupSprites();
			CleanupTiledTexture();
		}
		
		#endregion
		
		#region Render
		
		private Stopwatch TestTimer = new Stopwatch();
		
		public override void Render()
		{
			//Need to test how caching matrix calcs affects performance.
			//Could be a waste of time.
			//Could be better to recalc matrix in sprite when
			// position, angle and scale are changed and store it there.
			TestTimer.Start();
			
			//Set up the drawing
			
			//TODO: These need to be changed as little as possible, as seen in GOSLlib.
			Layer.DrawEngine2d.GraphicsContext.SetVertexBuffer(0, VertexBuffer);
			Layer.DrawEngine2d.GraphicsContext.SetShaderProgram(Shader.ShaderProgram);
			Layer.DrawEngine2d.GraphicsContext.SetTexture(0, TiledTexture.Texture);
			
			foreach(SpriteGroupItem sprite in Sprites)
			{
				Matrix4 scaleMatrix = GetScalingMatrix(sprite.Scale, sprite.TileWidth, sprite.TileHeight);
				Matrix4 rotMatrix = GetRotationMatrix(sprite.Rotation);
				Matrix4 transMatrix = GetTranslationMatrix(sprite.Position.X, sprite.Position.Y, sprite.Scale, sprite.Rotation);
				Matrix4 modelMatrix = transMatrix * rotMatrix * scaleMatrix;
				Matrix4 worldViewProj = Layer.DrawEngine2d.WorldCameraProjectionMatrix * modelMatrix;// * Layer.DrawEngine2d.ModelViewMatrix
				
				Shader.ShaderProgram.SetUniformValue(0, ref worldViewProj);
				
				//TODO: this needs to be changed to be an array of VBOs, like ge2d.
				Layer.DrawEngine2d.GraphicsContext.DrawArrays(DrawMode.TriangleStrip, 0, IndexCount);
			}
			
			//Clean up the drawing
			
			TestTimer.Stop();
		}
		
		#endregion
		
		#region TiledTexture
		
		private void InitializeTiledTexture(TiledTexture tiledTexture)
		{
			if(tiledTexture == null)
				throw new ArgumentNullException();
			
			TiledTexture = tiledTexture;
			
			RegisterAsUserOfTiledTexture();
		}
		
		private void CleanupTiledTexture()
		{
			UnregisterAsUserOfTiledTexture();
		}
		
		private TiledTexture TiledTexture;
		
		private void RegisterAsUserOfTiledTexture()
		{
			DrawEngine2d.AddTiledTextureUser(TiledTexture.Key, this);
		}
		
		private void UnregisterAsUserOfTiledTexture()
		{
			DrawEngine2d.RemoveTiledTextureUser(TiledTexture.Key, this);
		}
		
		public Single[] GetTiledTextureCoordinates(TiledTextureIndex index, out Int32 width, out Int32 height)
		{
			return TiledTexture.GetTextureCoordinates(index, out width, out height);
		}
		
//		public Int32 TileWidth
//		{
//			get { return TiledTexture.TileWidth; }
//		}
		
//		public Int32 TileHeight
//		{
//			get { return TiledTexture.TileHeight; }
//		}
		
		#endregion
		
		#region Sprites
		
		private void InitializeSprites()
		{
			Sprites = new List<SpriteGroupItem>();
		}
		
		private void CleanupSprites()
		{
			SpriteGroupItem[] sprites = Sprites.ToArray();
			
			foreach(SpriteGroupItem sprite in sprites)
				sprite.Dispose();
			Sprites.Clear();
			
			Sprites = null;
		}
		
		private List<SpriteGroupItem> Sprites;
		
		internal void AddSprite(SpriteGroupItem sprite)
		{
			if(sprite == null)
				throw new ArgumentNullException();
			
			if(Sprites.Contains(sprite))
				throw new ArgumentException();
			
			Sprites.Add(sprite);
			Layer.DrawEngine2d.SetRenderRequired();
		}
		
		internal void RemoveSprite(SpriteGroupItem sprite)
		{
			if(sprite == null)
				throw new ArgumentNullException();
			
			if(!Sprites.Contains(sprite))
				throw new ArgumentException();
			
			Sprites.Remove(sprite);
			Layer.DrawEngine2d.SetRenderRequired();
		}
		
		#endregion
		
		#region Bounds
		
		protected override void UpdateBounds()
		{
			throw new NotImplementedException();
		}
		
		#endregion
		
		
		//Old crap, need to fix.
		
		#region Blend Mode NOT IMPLEMENTED
		
		#endregion
		
		#region Vertices
		
		private void InitializeVertices()
		{
			Vertices = new Single[VertexCount * 3];
			
			VertexCoordinates_0_TopLeft = new Coordinate2(0.0f, 0.0f);
			VertexCoordinates_1_BottomLeft = new Coordinate2(0.0f, 1.0f);
			VertexCoordinates_2_TopRight = new Coordinate2(1.0f, 0.0f);
			VertexCoordinates_3_BottomRight = new Coordinate2(1.0f, 1.0f);
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
		
		private Coordinate2 VertexCoordinates_2_TopRight
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
		
		private Coordinate2 VertexCoordinates_3_BottomRight
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
			Indices = default(UInt16[]);;
		}
		
		private const Int32 IndexCount = 4;
		private UInt16[] Indices;
		
		#endregion
		
		#region Texture Coordinates
		
		private void InitializeTextureCoordinates()
		{
			TextureCoordinates = new Single[4 * 2];
			
			//TODO: these are temporary, for testing.
			TextureCoordinates_0_TopLeft = new Coordinate2(0.0f, 0.0f);
			TextureCoordinates_1_BottomLeft = new Coordinate2(0.0f, 1.0f);
			TextureCoordinates_2_TopRight = new Coordinate2(1.0f, 0.0f);
			TextureCoordinates_3_BottomRight = new Coordinate2(1.0f, 1.0f);
		}
		
		private void CleanupTextureCoordinates()
		{
			TextureCoordinates = default(Single[]);
		}
		
		private Single[] TextureCoordinates;
		
		private Coordinate2 TextureCoordinates_0_TopLeft
		{
			get
			{
				return new Coordinate2(TextureCoordinates[0], TextureCoordinates[1]);
			}
			set
			{
				TextureCoordinates[0] = value.X;
				TextureCoordinates[1] = value.Y;
			}
		}
		private Coordinate2 TextureCoordinates_1_BottomLeft
		{
			get
			{
				return new Coordinate2(TextureCoordinates[2], TextureCoordinates[3]);
			}
			set
			{
				TextureCoordinates[2] = value.X;
				TextureCoordinates[3] = value.Y;
			}
		}
		private Coordinate2 TextureCoordinates_2_TopRight
		{
			get
			{
				return new Coordinate2(TextureCoordinates[4], TextureCoordinates[5]);
			}
			set
			{
				TextureCoordinates[4] = value.X;
				TextureCoordinates[5] = value.Y;
			}
		}
		private Coordinate2 TextureCoordinates_3_BottomRight
		{
			get
			{
				return new Coordinate2(TextureCoordinates[6], TextureCoordinates[7]);
			}
			set
			{
				TextureCoordinates[6] = value.X;
				TextureCoordinates[7] = value.Y;
			}
		}
		
		#endregion
		
		#region Color
		
		private void InitializeColor()
		{
			VertexColors = new Single[VertexCount * 4];
			Color = Colors.White;
		}
		
		private void CleanupColor()
		{
			VertexColors = new Single[0];
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
			Shader = new SpriteShader(Layer.DrawEngine2d);
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
		
		public Matrix4 GetScalingMatrix(Single scale, Int32 tileWidth, Int32 tileHeight)
		{
			if(!ScalingMatrixCache.ContainsKey(scale))
				GenerateScalingMatrix(scale, tileWidth, tileHeight);
			return ScalingMatrixCache[scale];
		}
		
		private void GenerateScalingMatrix(Single scale, Int32 tileWidth, Int32 tileHeight)
		{
			Single scaleX = tileWidth * scale;
			Single scaleY = tileHeight * scale;
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
			return Sprites.Count * ScalingMatrixCacheLimitFactor;
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
			return Sprites.Count * RotationMatrixCacheLimitFactor;
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
			return Sprites.Count * TranslationMatrixCacheLimitFactor;
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

