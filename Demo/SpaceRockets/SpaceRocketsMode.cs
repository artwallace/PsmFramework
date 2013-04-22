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
		#region Mode Factory Delegate
		
		public static ModeBase DrawEngineTestModeFactory(AppManager mgr)
		{
			return new SpaceRocketsMode(mgr);
		}
		
		#endregion
		
		#region Constructor
		
		public SpaceRocketsMode(AppManager mgr)
			: base(mgr)
		{
		}
		
		#endregion
		
		#region Initialize, Cleanup
		
		protected override void Initialize()
		{
			DebugInfoEnabled = true;
			DebugInfoForcesRender = false;
			DrawEngine2d.ClearColor = Colors.Blue;
			
			TiledTexture tt = DrawEngine2d.CreateTiledTexture("/Application/TwinStickShooter/Images/Ship64.png");
			ColumnKey key = tt.CreateColumnIndex(1).GetKey(0);
			
			LayerBase l2 = DrawEngine2d.Layers.GetOrCreateWorldLayer(1);
			
			Ship1 = new Sprite(l2, key);
			Ship1.SetPosition(100f, 100f);
			
			Sprite testSprite = new Sprite(l2,key);
			testSprite.SetDimensionsProportionallyByWidth(200f);
			testSprite.SetPosition(l2.Camera.Center);
			
			DebugLabel debugTextLabel = DebugLabel.CreateDebugLabel(DrawEngine2d, LayerType.Screen);
			debugTextLabel.Text = "This text is in a screen layer.";
			debugTextLabel.SetPosition(400.0f, 100.0f);
		}
		
		protected override void Cleanup()
		{
			Ship1.Dispose();
			Ship1 = null;
		}
		
		private Sprite Ship1;
		
		#endregion
		
		#region Update
		
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
				
				return;
			}
			
			Single xMove = 0.0f;
			Single yMove = 0.0f;
			if(FMath.Abs(Mgr.GamePad0_LeftStick_X) > 0.1f)
				xMove = Mgr.GamePad0_LeftStick_X * 2;
			if(FMath.Abs(Mgr.GamePad0_LeftStick_Y) > 0.1f)
				yMove = Mgr.GamePad0_LeftStick_Y * 2;
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
			
			if(Mgr.GamePad0_LeftStick_Active)
				Ship1.SetRotation(Angle2.GetAngleFromOrigin(Mgr.GamePad0_LeftStick_X, -Mgr.GamePad0_LeftStick_Y));
			
			if (Mgr.GamePad0_L1)
				Ship1.AdjustRotation(turnLeft);
			else if (Mgr.GamePad0_R1)
				Ship1.AdjustRotation(turnRight);
			
			if (Mgr.GamePad0_Square)
				DrawEngine2d.WorldCamera.Rotation = new Angle2(DrawEngine2d.WorldCamera.Rotation.Degree - 1.0f);
			else if (Mgr.GamePad0_Circle)
				DrawEngine2d.WorldCamera.Rotation = new Angle2(DrawEngine2d.WorldCamera.Rotation.Degree + 1.0f);
			
			if (Mgr.GamePad0_Triangle)
				DrawEngine2d.WorldCamera.Zoom += 0.1f;
			else if (Mgr.GamePad0_Cross)
				DrawEngine2d.WorldCamera.Zoom -= 0.1f;
		}
		
		private readonly Angle2 turnLeft = new Angle2(-1.0f);
		private readonly Angle2 turnRight = new Angle2(1.0f);
		
		#endregion
		
		#region DebugInfo
		
		protected override void GetAdditionalDebugInfo()
		{
			AddDebugInfoLine("Camera Center", DrawEngine2d.WorldCamera.Center);
			AddDebugInfoLine("Camera Bounds", DrawEngine2d.WorldCamera.Bounds);
			AddDebugInfoLine("Camera Width", DrawEngine2d.WorldCamera.Bounds.Width);
			AddDebugInfoLine("Camera Rotation", DrawEngine2d.WorldCamera.Rotation);
			AddDebugInfoLine("Camera Zoom", DrawEngine2d.WorldCamera.Zoom);
			AddDebugInfoLine("Left Analog", Mgr.GamePad0_LeftStick_X + "x" + Mgr.GamePad0_LeftStick_Y);
		}
		
		#endregion
	}
}

