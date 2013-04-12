using System;
using PsmFramework;
using PsmFramework.Engines.DrawEngine2d;
using PsmFramework.Engines.DrawEngine2d.Drawables;
using PsmFramework.Engines.DrawEngine2d.Layers;
using PsmFramework.Engines.DrawEngine2d.Support;
using PsmFramework.Engines.DrawEngine2d.Textures;
using PsmFramework.Modes;
using PsmFramework.Modes.TopDown2dAlt;
using Sce.PlayStation.Core;

namespace Demo.SpaceRockets
{
	public class SpaceRocketsMode : TopDown2dAltModeBase
	{
		#region Constructor
		
		public SpaceRocketsMode(AppManager mgr)
			: base(mgr)
		{
		}
		
		#endregion
		
		#region Mode Factory Delegate
		
		public static ModeBase DrawEngineTestModeFactory(AppManager mgr)
		{
			return new SpaceRocketsMode(mgr);
		}
		
		#endregion
		
		#region Mode Logic
		
		private DebugLabel _DebugTextLabel;
		
		private Sprite Ship1;
		
		protected override void Initialize()
		{
			//TODO: Remove this after testing!
			DrawEngine2d.ClearColor = Colors.Blue;
			
			DebugInfoEnabled = true;
			DebugInfoForcesRender = false;
			
			String shipSprite = "/Application/TwinStickShooter/Images/Ship64.png";
			
			TiledTexture tt = DrawEngine2d.CreateTiledTexture(shipSprite);
			ColumnKey key = tt.CreateColumnIndex(1).GetKey(0);
			
			LayerBase l2 = DrawEngine2d.GetOrCreateWorldLayer(1);
			
			Ship1 = new Sprite(l2, key);
			Ship1.SetPosition(100f, 100f);
			
			Sprite testSprite = new Sprite(l2,key);
			testSprite.SetDimensionsProportionallyFromWidth(200f);
			testSprite.SetPosition(l2.Camera.Center);
			
			LayerBase debugOverlay = DrawEngine2d.GetOrCreateScreenLayer(2);
			_DebugTextLabel = new DebugLabel(debugOverlay);
			_DebugTextLabel.Text = "This text is in a screen layer.";
			_DebugTextLabel.SetPosition(100.0f, 100.0f);
		}
		
		protected override void Cleanup()
		{
			_DebugTextLabel.Dispose();
			_DebugTextLabel = null;
		}
		
		public override void Update()
		{
			if (Mgr.GamePad0_Start_Pressed && Mgr.ModeChangeAllowed)
			{
				Mgr.GoToTitleScreenMode();
				return;
			}
			
			if (Mgr.GamePad0_Select_Pressed)
			{
				if (Mgr.RunState == RunState.Running)
					Mgr.SetRunStateToPaused();
				else
					Mgr.SetRunStateToRunning();
			}
			
			Single xMove = 0.0f;
			Single yMove = 0.0f;
			if(FMath.Abs(Mgr.GamePadData.AnalogLeftX) > 0.1f)
				xMove = Mgr.GamePadData.AnalogLeftX * 2;
			if(FMath.Abs(Mgr.GamePadData.AnalogLeftY) > 0.1f)
				yMove = Mgr.GamePadData.AnalogLeftY * 2;
			Ship1.AdjustPosition(xMove, yMove);
			
			
			Single xCam = 0.0f;
			Single yCam = 0.0f;
			if (Mgr.GamePad0_Left)
				xCam = -1f;
			else if (Mgr.GamePad0_Right)
				xCam = 1f;
			if (Mgr.GamePad0_Up)
				yCam = -1f;
			else if (Mgr.GamePad0_Down)
				yCam = 1f;
			DrawEngine2d.WorldCamera.AdjustCenter(xCam, yCam);
			
			if (Mgr.GamePad0_L1)
				Ship1.AdjustRotation(-1.0f);
			else if (Mgr.GamePad0_R1)
				Ship1.AdjustRotation(1.0f);
			
			if (Mgr.GamePad0_Square)
				DrawEngine2d.WorldCamera.Rotation = new Angle2(DrawEngine2d.WorldCamera.Rotation.Degree - 1.0f);
			else if (Mgr.GamePad0_Circle)
				DrawEngine2d.WorldCamera.Rotation = new Angle2(DrawEngine2d.WorldCamera.Rotation.Degree + 1.0f);
			
			if (Mgr.GamePad0_Triangle)
				DrawEngine2d.WorldCamera.Zoom += 0.1f;
			else if (Mgr.GamePad0_Cross)
				DrawEngine2d.WorldCamera.Zoom -= 0.1f;
		}
		
		#endregion
		
		#region DebugInfo
		
		protected override void GetAdditionalDebugInfo()
		{
			AddDebugInfoLine("Camera Center", DrawEngine2d.WorldCamera.Center);
			AddDebugInfoLine("Camera Bounds", DrawEngine2d.WorldCamera.Bounds);
			AddDebugInfoLine("Camera Width", DrawEngine2d.WorldCamera.Bounds.Width);
			AddDebugInfoLine("Camera Rotation", DrawEngine2d.WorldCamera.Rotation);
			AddDebugInfoLine("Camera Zoom", DrawEngine2d.WorldCamera.Zoom);
			
//			Single x = Logo.Position.X + Logo.TileWidth / 2;
//			Single y = Logo.Position.Y + Logo.TileHeight / 2;
//			
//			AddDebugInfoLine("Logo Center", x + "x" + y);
//			
//			AddDebugInfoLine("Logo Bounds", Logo.TileWidth);
		}
		
		#endregion
	}
}

