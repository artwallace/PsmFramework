using System;
using System.Collections.Generic;
using System.Text;
using Sce.PlayStation.Core;
using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

namespace PsmFramework.Modes
{
	public abstract class GameEngine2dModeBase : ModeBase
	{
		protected abstract UInt32 SpritesCapacity { get; }
		protected abstract UInt32 DrawHelpersCapacity { get; }
		protected abstract Vector4 ClearColor { get; }
		protected abstract Boolean DrawDebugGrid { get; }
		
		#region Constructor, Dispose
		
		protected GameEngine2dModeBase(AppManager mgr)
			: base(mgr)
		{
		}
		
		#endregion
		
		#region Initialize, Cleanup
		
		protected override void InitializeInternal()
		{
			Director.Initialize(SpritesCapacity, DrawHelpersCapacity, Mgr.GraphicsContext);
			Director.Instance.GL.Context.SetClearColor(ClearColor);
			
			if (DrawDebugGrid)
				Director.Instance.DebugFlags |= DebugFlags.DrawGrid;
			
			InitializeScene();
			InitializeTextureManager();
			InitializeSprites();
			
			Director.Instance.RunWithScene(GameScene, true);
		}
		
		protected override void CleanupInternal()
		{
			CleanupSprites();
			CleanupTextureManager();
			CleanupScene();
			
			Director.Terminate();
		}
		
		#endregion
		
		#region Update, Render
		
		internal override void UpdateInternal()
		{
			if (!Mgr.ModeChanged)
				Director.Instance.Update();
		}
		
		internal override void RenderInternal()
		{
			Director.Instance.Render();
			Director.Instance.GL.Context.SwapBuffers();
			Director.Instance.PostSwap();
		}
		
		#endregion
		
		#region Scene
		
		protected Scene GameScene;
		
		private void InitializeScene()
		{
			GameScene = new Scene();
			GameScene.Camera2D.SetViewFromWidthAndBottomLeft(Mgr.ScreenWidth, new Vector2(0f, 0f));
		}
		
		private void CleanupScene()
		{
			//TODO: I'm not sure how a scene is actually supposed to be stopped.
			GameScene.StopAllActions();
			GameScene.RemoveAllChildren(true);
			GameScene = null;
		}
		
		#endregion
		
		#region Camera
		
		private Vector2 LastCameraPosition;
		
		internal Vector2 CameraLowerLeftPosition { get; private set; }
		
		public void SetCamera(Vector2 position)
		{
			if (LastCameraPosition != position)
			{
				LastCameraPosition = position;
				GameScene.Camera2D.SetViewFromWidthAndCenter(Mgr.ScreenWidth, position);
				
				Bounds2 screen = GameScene.Camera2D.CalcBounds();
				CameraLowerLeftPosition = screen.Min;
			}
		}
		
		public Bounds2 GetCameraBounds()
		{
			return GameScene.Camera.CalcBounds();
		}
		
		public Single GetCameraWidth()
		{
			Bounds2 b = GameScene.Camera.CalcBounds();
			return b.Size.X;
		}
		
		public Single GetCameraHeight()
		{
			Bounds2 b = GameScene.Camera.CalcBounds();
			return b.Size.Y;
		}
		
		#endregion
		
		#region TextureManager
		
		public TextureManager TextureManager { get; private set; }
		
		private void InitializeTextureManager()
		{
			TextureManager = new TextureManager(Mgr);
		}
		
		private void CleanupTextureManager()
		{
			TextureManager.Dispose();
			TextureManager = null;
		}
		
		#endregion
		
		#region Sprites
		
		private List<SpriteList> SpriteListsInScene;
		private List<RawSpriteTileArray> RawSpriteTileListsInScene;
		private List<SpriteUV> SpritesInScene;
		
		private void InitializeSprites()
		{
			SpriteListsInScene = new List<SpriteList>();
			RawSpriteTileListsInScene = new List<RawSpriteTileArray>();
			SpritesInScene = new List<SpriteUV>();
		}
		
		private void CleanupSprites()
		{
			RemoveAllFromScene();
			
			SpriteListsInScene.Clear();
			SpriteListsInScene = null;
			
			RawSpriteTileListsInScene.Clear();
			RawSpriteTileListsInScene = null;
			
			SpritesInScene.Clear();
			SpritesInScene = null;
		}
		
		public void AddToScene(SpriteList spriteList, Int32 order = 0)
		{
			SpriteListsInScene.Add(spriteList);
			GameScene.AddChild(spriteList, order);
		}
		
		public void AddToScene(RawSpriteTileArray spriteList, Int32 order = 0)
		{
			RawSpriteTileListsInScene.Add(spriteList);
			GameScene.AddChild(spriteList, order);
		}
		
		public void AddToScene(SpriteUV sprite, Int32 order = 0)
		{
			SpritesInScene.Add(sprite);
			GameScene.AddChild(sprite, order);
		}
		
		public void RemoveFromScene(SpriteList spriteList)
		{
			SpriteListsInScene.Remove(spriteList);
			GameScene.RemoveChild(spriteList, false);
		}
		
		public void RemoveFromScene(RawSpriteTileArray spriteList)
		{
			RawSpriteTileListsInScene.Remove(spriteList);
			GameScene.RemoveChild(spriteList, false);
		}
		
		public void RemoveFromScene(SpriteUV sprite)
		{
			SpritesInScene.Remove(sprite);
			GameScene.RemoveChild(sprite, false);
		}
		
		private void RemoveAllFromScene()
		{
			SpriteListsInScene.ForEach(s => { GameScene.RemoveChild(s, false); });
			SpriteListsInScene.Clear();
			SpritesInScene.ForEach(s => { GameScene.RemoveChild(s, false); });
			SpritesInScene.Clear();
		}
		
		#endregion
		
		#region Debug
		
		protected StringBuilder DebugInfo = new StringBuilder();
		
		private Single DebugPstnOffsetX;
		private Single DebugPstnOffsetY;
		private Single DebugHeight;
		
		private String GetDebugInfo()
		{
			DebugInfo.Clear();
			
			DebugInfo.Append("RAM Used: ");
			DebugInfo.AppendLine((System.Math.Round(GC.GetTotalMemory(false) / 1048576d, 2)).ToString() + " MiB");
			
			DebugInfo.Append("TimeSinceLastFrame: ");
			DebugInfo.AppendLine(Mgr.TimeSinceLastFrame.Ticks.ToString());
			
			DebugInfo.Append("Update Ticks: ");
			DebugInfo.AppendLine(Mgr.UpdateLength.Ticks.ToString());
			
			DebugInfo.Append("Render Ticks: ");
			DebugInfo.AppendLine(Mgr.RenderLength.Ticks.ToString());
			
			DebugInfo.Append("FPS: ");
			DebugInfo.AppendLine(Mgr.FramesPerSecond.ToString());
			
			GetAdditionalDebugInfo();
			
			return DebugInfo.ToString();
		}
		
		protected virtual void GetAdditionalDebugInfo()
		{
		}
		
		protected void EnableDebugInfo()
		{
			DebugPstnOffsetX = (Mgr.ScreenWidth / -2) + 10;
			DebugPstnOffsetY = (Mgr.ScreenHeight / 2) - 19;
			DebugHeight = GameScene.Camera2D.GetPixelSize() * EmbeddedDebugFontData.CharSizei.Y;
			
			GameScene.AdHocDraw += DrawDebugInfo;
		}
		
		protected void DisableDebugInfo()
		{
			GameScene.AdHocDraw -= DrawDebugInfo;
		}
		
		private void DrawDebugInfo()
		{
			Director.Instance.GL.ModelMatrix.Push();
			Director.Instance.GL.ModelMatrix.SetIdentity();//go in world space
			
			Director.Instance.GL.SetBlendMode(BlendMode.Normal);
			Director.Instance.SpriteRenderer.DefaultFontShader.SetColor(ref Colors.Yellow);
			
			Vector2 pstn = new Vector2(GameScene.Camera2D.Center.X + DebugPstnOffsetX, GameScene.Camera2D.Center.Y + DebugPstnOffsetY);
			
			Director.Instance.SpriteRenderer.DrawTextDebug(
				GetDebugInfo(),
				pstn, 
				DebugHeight
				);
			
			Director.Instance.GL.ModelMatrix.Pop();
		}
		
		#endregion
	}
}

