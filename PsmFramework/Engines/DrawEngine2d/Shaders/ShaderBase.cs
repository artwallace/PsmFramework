using System;
using System.IO;
using System.Reflection;
using PsmFramework.Engines.DrawEngine2d;
using Sce.PlayStation.Core.Graphics;

namespace PsmFramework.Engines.DrawEngine2d.Shaders
{
	public abstract class ShaderBase : IDisposablePlus
	{
		#region Constructor, Dispose
		
		public ShaderBase(DrawEngine2d drawEngine2d)
		{
			InitializeInternal(drawEngine2d);
			Initialize();
		}
		
		public void Dispose()
		{
			if(IsDisposed)
				return;
			
			Cleanup();
			CleanupInternal();
			IsDisposed = true;
		}
		
		public Boolean IsDisposed { get; private set; }
		
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
		}
		
		protected virtual void Cleanup()
		{
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
		
		protected const String BasePath = "PsmFramework.Engines.DrawEngine2d.Shaders.";
		
		public abstract String Path { get; }
		
		#endregion
		
		#region ShaderProgram
		
		private void InitializeShaderProgramInternal()
		{
			LoadEmbeddedShader();
		}
		
		private void CleanupShaderProgramInternal()
		{
			ShaderProgram.Dispose();
			ShaderProgram = null;
		}
		
		protected abstract void InitializeShaderProgram();
		
		protected abstract void CleanupShaderProgram();
		
		public ShaderProgram ShaderProgram { get; private set; }
		
		private void LoadEmbeddedShader()
		{
			Assembly resourceAssembly = Assembly.GetExecutingAssembly();
			
			if (resourceAssembly.GetManifestResourceInfo(Path) == null)
			{
				//String[] debugText = resourceAssembly.GetManifestResourceNames();
				throw new ArgumentException("Unable to load shader from resource: " + Path);
			}
			
			Byte[] data;
			using(Stream stream = resourceAssembly.GetManifestResourceStream(Path))
			{
				data = new Byte[stream.Length];
				stream.Read(data, 0, data.Length);
				stream.Close();
			}
			
			ShaderProgram = new ShaderProgram(data);
			
			data = null;
		}
		
		#endregion
	}
}

