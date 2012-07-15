using System;
using PsmFramework.Engines.DrawEngine2d.Support;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;

namespace PsmFramework.Engines.DrawEngine2d.Shaders
{
	public class FontShader : ShaderBase
	{
		#region Constructor
		
		public FontShader(DrawEngine2d drawEngine2d)
			: base(drawEngine2d)
		{
		}
		
		#endregion
		
		#region Initialize, Cleanup
		
		protected override void Initialize()
		{
			InitializeShaderProgram();
		}
		
		protected override void Cleanup()
		{
			CleanupShaderProgram();
		}
		
		#endregion
		
		#region Path
		
		public override String Path
		{
			get
			{
				return "PsmFramework.Engines.DrawEngine2d.Shaders.Font.cgx";
			}
		}
		
		#endregion
		
		#region ShaderProgram
		
		protected override void InitializeShaderProgram()
		{
			ShaderProgram.SetUniformBinding(0, "MVP");
			ShaderProgram.SetUniformBinding(1, "Color");
			ShaderProgram.SetUniformBinding(2, "UVTransform");
			
			ShaderProgram.SetAttributeBinding(0, "vin_data");
		}
		
		protected override void CleanupShaderProgram()
		{
		}
		
		public void SetShaderProgram()
		{
			if(DrawEngine2d.GraphicsContext.GetShaderProgram() != ShaderProgram)
				DrawEngine2d.GraphicsContext.SetShaderProgram(ShaderProgram);
		}
		
		public void DrawArrays()
		{
			DrawEngine2d.GraphicsContext.DrawArrays(DrawMode.TriangleStrip, 0, IndexCount);
		}
		
		#endregion
		
		#region WorldViewProjection
		
		public void SetWorldViewProjection(ref Matrix4 worldViewProj)
		{
			ShaderProgram.SetUniformValue(0, ref worldViewProj);
		}
		
		#endregion
		
		#region Vertex Rendering Order Indices
		
		private void InitializeIndices()
		{
			Indices = new UInt16[4];
			Indices[0] = 0;
			Indices[1] = 1;
			Indices[2] = 2;
			Indices[3] = 3;
		}
		
		private void CleanupIndices()
		{
			Indices = new UInt16[0];
		}
		
		private const Int32 IndexCount = 4;
		private UInt16[] Indices;
		
		#endregion
		
		#region Vertex Coordinates
		
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
		
		#region Texture Coordinates
		
		private void InitializeTextureCoordinates()
		{
			TextureCoordinates = new Single[4 * 2];
			
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
				
				Vector4 c = _Color.AsVector4;
				
				ShaderProgram.SetUniformValue(1, ref c);
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
		
		public void SetVertexBuffer()
		{
			if(DrawEngine2d.GraphicsContext.GetVertexBuffer(0) != VertexBuffer)
				DrawEngine2d.GraphicsContext.SetVertexBuffer(0, VertexBuffer);
		}
		
		#endregion
	}
}
