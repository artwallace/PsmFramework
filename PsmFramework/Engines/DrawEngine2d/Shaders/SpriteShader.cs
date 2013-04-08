using System;
using PsmFramework.Engines.DrawEngine2d.Support;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;

namespace PsmFramework.Engines.DrawEngine2d.Shaders
{
	public sealed class SpriteShader : ShaderBase
	{
		#region Constructor
		
		public SpriteShader(DrawEngine2d drawEngine2d)
			: base(drawEngine2d)
		{
		}
		
		#endregion
		
		#region Initialize, Cleanup
		
		protected override void Initialize()
		{
			InitializeVertices();
			InitializeIndices();
			InitializeTextureCoordinates();
			InitializeColor();
			
			InitializeVertexBuffer();
		}
		
		protected override void Cleanup()
		{
			CleanupVertexBuffer();
			
			CleanupColor();
			CleanupTextureCoordinates();
			CleanupIndices();
			CleanupVertices();
		}
		
		#endregion
		
		#region Path
		
		public override String Path
		{
			get { return BasePath + "Sprite.cgx"; }
		}
		
		#endregion
		
		#region ShaderProgram
		
		protected override void InitializeShaderProgram()
		{
			ShaderProgram.SetUniformBinding(0, "u_WorldMatrix");
		}
		
		protected override void CleanupShaderProgram()
		{
		}
		
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
			Vertices = default(Single[]);
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
			Indices = default(UInt16[]);
		}
		
		public const Int32 IndexCount = 4;
		
		private UInt16[] Indices;
		
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
		
		public VertexBuffer VertexBuffer { get; private set; }
		
		#endregion
		
		public static Matrix4 GetScalingMatrix(Single width, Single height, Single scale)
		{
			return Matrix4.Scale(width * scale, height * scale, VertexZ);
		}
		
		public static Matrix4 GetTranslationMatrix(Single x, Single y, Single scale, Single radianAngle)
		{
			//I'm not sure what this actually does.
			Single nx = x + (scale * (FMath.Sin(radianAngle) - FMath.Cos(radianAngle)));
			Single ny = y + (scale * (FMath.Cos(radianAngle) + FMath.Sin(radianAngle)));
			return Matrix4.Translation(nx, ny, VertexZ);
		}
		
		public static Matrix4 GetTranslationMatrix(Single x, Single y)
		{
			return Matrix4.Translation(x, y, VertexZ);
		}
	}
}

