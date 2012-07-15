using System;
using System.IO;
using System.Reflection;
using PsmFramework.Engines.DrawEngine2d;
using Sce.PlayStation.Core.Graphics;

namespace PsmFramework.Engines.DrawEngine2d.Shaders
{
	public abstract class ShaderBase : IDisposable
	{
		#region Constructor, Dispose
		
		public ShaderBase(DrawEngine2d drawEngine2d)
		{
			InitializeInternal(drawEngine2d);
			Initialize();
		}
		
		public void Dispose()
		{
			Cleanup();
			CleanupInternal();
		}
		
		#endregion
		
		#region Initialize, Cleanup
		
		private void InitializeInternal(DrawEngine2d drawEngine2d)
		{
			InitializeDrawEngine2d(drawEngine2d);
			InitializeShaderProgramInternal();
			InitializeShaderProgram();
		}
		
		private void CleanupInternal()
		{
			CleanupShaderProgram();
			CleanupShaderProgramInternal();
			CleanupDrawEngine2d();
		}
		
		protected virtual void Initialize()
		{
			//This method should remain empty in the base class.
		}
		
		protected virtual void Cleanup()
		{
			//This method should remain empty in the base class.
		}
		
		#endregion
		
		#region DrawEngine2d
		
		private void InitializeDrawEngine2d(DrawEngine2d drawEngine2d)
		{
			if(drawEngine2d == null)
				throw new ArgumentNullException();
			
			DrawEngine2d = drawEngine2d;
		}
		
		private void CleanupDrawEngine2d()
		{
			DrawEngine2d = null;
		}
		
		protected DrawEngine2d DrawEngine2d;
		
		#endregion
		
		#region Path
		
		public abstract String Path { get; }
		
		#endregion
		
		#region ShaderProgram
		
		private void InitializeShaderProgramInternal()
		{
			ShaderProgram = LoadEmbeddedShader(Path);
		}
		
		private void CleanupShaderProgramInternal()
		{
			ShaderProgram.Dispose();
			ShaderProgram = null;
		}
		
		protected abstract void InitializeShaderProgram();
		
		protected abstract void CleanupShaderProgram();
		
		public ShaderProgram ShaderProgram { get; private set; }
		
		#endregion
		
		#region Shader Resource Loader
		
		private static Assembly ResourceAssembly = Assembly.GetExecutingAssembly();
		
		private static ShaderProgram LoadEmbeddedShader(String resourcePath)
		{
			if (ResourceAssembly.GetManifestResourceInfo(resourcePath) == null)
			{
				String[] allResources = ResourceAssembly.GetManifestResourceNames();
				throw new ArgumentException("Unable to load shader from resource: " + resourcePath);
			}
			
			Byte[] data;
			using(Stream stream = ResourceAssembly.GetManifestResourceStream(resourcePath))
			{
				data = new Byte[stream.Length];
				stream.Read(data, 0, data.Length);
				stream.Close();
			}
			
			ShaderProgram shader = new ShaderProgram(data);
			
			return shader;
		}
		
		#endregion
	}
}

