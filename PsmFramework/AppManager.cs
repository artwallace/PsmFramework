using System;
using System.Collections.Generic;
using System.Diagnostics;
using PsmFramework.Modes;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Input;

namespace PsmFramework
{
	public sealed class AppManager : IDisposablePlus
	{
		#region Constructor, Dispose
		
		public AppManager(AppOptionsBase options, GraphicsContext gc, CreateModeDelegate defaultTitleScreen = null, CreateModeDelegate defaultOptionsScreen = null)
		{
			Initialize(options, gc, defaultTitleScreen, defaultOptionsScreen);
		}
		
		public void Dispose()
		{
			if(IsDisposed)
				return;
			
			Cleanup();
			IsDisposed = true;
		}
		
		public Boolean IsDisposed { get; private set; }
		
		#endregion
		
		#region Initialize, Cleanup
		
		private void Initialize(AppOptionsBase options, GraphicsContext gc, CreateModeDelegate defaultTitleScreen, CreateModeDelegate defaultOptionsScreen)
		{
			InitializeRunState();
			InitializeOptions(options);
			InitializeGraphics(gc);
			InitializeTimers();
			InitializeModes(defaultTitleScreen, defaultOptionsScreen);
			InitializeInput();
			InitializeRandomGenerator();
		}
		
		private void Cleanup()
		{
			CleanupRandomGenerator();
			CleanupInput();
			CleanupModes();
			CleanupTimers();
			CleanupGraphics();
			CleanupOptions();
			CleanupRunState();
		}
		
		#endregion
		
		#region AppLoop, Update, Render
		
		public void AppLoop()
		{
			SetRunStateToRunning();
			
			InitializeCurrentMode();
			
			while (RunState != RunState.Ending)
			{
				CalculateUpdateTime();
				CalculateTimeSinceLastFrame();
				CalculateFramesPerSecond();
				
				SystemEvents.CheckEvents();
				
				StartUpdateTimer();
				Update();
				CompleteUpdateTimer();
				
				StartRenderTimer();
				Render();
				CompleteRenderTimer();
				
				UpdateRunState();
				
				if (RunStateRecentlyChanged)
				{
					if(RunState == RunState.Paused)
						PauseInGameTimer();
					else
						ResumeInGameTimer();
				}
			}
		}
		
		private void Update()
		{
			RefreshInputData();
			
			CurrentMode.UpdateInternalPre();
			CurrentMode.Update();
			if (CurrentMode != null)
				CurrentMode.UpdateInternalPost();
			
			if (ModeChanged)
			{
				CleanupPreviousMode();
				InitializeCurrentMode();
			}
		}
		
		private void Render()
		{
			if (!ModeChanged)
				CurrentMode.RenderInternal();
		}
		
		#endregion
		
		#region RunState
		
		private void InitializeRunState()
		{
			SetRunStateToInitializing();
			UpdateRunState();
		}
		
		private void CleanupRunState()
		{
		}
		
		public RunState RunState { get; private set; }
		private RunState ChangeToRunState;
		private Boolean RunStateChanging;
		private Boolean RunStateRecentlyChanged;
		
		private void UpdateRunState()
		{
			if (RunStateChanging)
			{
				RunStateChanging = false;
				RunStateRecentlyChanged = true;
				RunState = ChangeToRunState;
			}
			else if (RunStateRecentlyChanged)
				RunStateRecentlyChanged = false;
		}
		
		public void SetRunStateToInitializing()
		{
			RunStateChanging = true;
			RunStateRecentlyChanged = false;
			ChangeToRunState = RunState.Initializing;
		}
		
		public void SetRunStateToRunning()
		{
			RunStateChanging = true;
			RunStateRecentlyChanged = false;
			ChangeToRunState = RunState.Running;
		}
		
		public void SetRunStateToPaused()
		{
			RunStateChanging = true;
			RunStateRecentlyChanged = false;
			ChangeToRunState = RunState.Paused;
		}
		
		public void SetRunStateToEnding()
		{
			RunStateChanging = true;
			RunStateRecentlyChanged = false;
			ChangeToRunState = RunState.Ending;
		}
		
		#endregion
		
		#region Modes
		
		private void InitializeModes(CreateModeDelegate defaultTitleScreen, CreateModeDelegate defaultOptionsScreen)
		{
			PreviousMode = null;
			CurrentMode = null;
			ReturnMode = null;
			
			DefaultTitleScreenFactory = defaultTitleScreen;
			DefaultOptionsScreenFactory = defaultOptionsScreen;
		}
		
		private void CleanupModes()
		{
			if (PreviousMode != null)
			{
				PreviousMode.Dispose();
				PreviousMode = null;
			}
			
			if (CurrentMode != null)
			{
				CurrentMode.Dispose();
				CurrentMode = null;
			}
			
			if (ReturnMode != null)
			{
				ReturnMode.Dispose();
				ReturnMode = null;
			}
			
			DefaultTitleScreenFactory = null;
			DefaultOptionsScreenFactory = null;
		}
		
		public delegate ModeBase CreateModeDelegate(AppManager mgr);
		
		public CreateModeDelegate DefaultTitleScreenFactory { get; private set; }
		public CreateModeDelegate DefaultOptionsScreenFactory { get; private set; }
		
		private CreateModeDelegate NextModeFactory;
		
		private readonly TimeSpan cMinTicksBetweenModeChanges = TimeSpan.FromTicks(100);
		private DateTime LastModeChange;
		
		public ModeBase PreviousMode { get; private set; }
		public ModeBase CurrentMode { get; private set; }
		public ModeBase ReturnMode { get; private set; }
		
		public Boolean ModeChangeAllowed
		{
			get
			{
				return (UpdateTime - LastModeChange) > cMinTicksBetweenModeChanges;
			}
		}
		
		public Boolean ModeChanged
		{
			get { return PreviousMode != null; }
		}
		
		public void GoToMode(CreateModeDelegate factory)
		{
			LastModeChange = UpdateTime;
			PreviousMode = CurrentMode;
			NextModeFactory = factory;
			CurrentMode = null;
			ReturnMode = null;
		}
		
		//TODO: GoToThenReturn should not dispose of the original mode. perhaps as an option or another method.
		public void GoToModeThenReturn(CreateModeDelegate factory, ModeBase returnMode)
		{
			LastModeChange = UpdateTime;
			PreviousMode = CurrentMode;
			NextModeFactory = factory;
			CurrentMode = null;
			ReturnMode = returnMode;
		}
		
		public void ReturnToMode()
		{
			LastModeChange = UpdateTime;
			PreviousMode = CurrentMode;
			CurrentMode = ReturnMode;
			ReturnMode = null;
		}
		
		public void InitializeCurrentMode()
		{
			CurrentMode = NextModeFactory(this);
		}
		
		public void CleanupPreviousMode()
		{
			PreviousMode.Dispose();
			PreviousMode = null;
			
			FreeMemory();
		}
		
		public void GoToTitleScreenMode()
		{
			if(DefaultTitleScreenFactory == null)
				throw new ArgumentNullException();
			
			GoToMode(DefaultTitleScreenFactory);
		}
		
		public void GoToOptionsScreenMode()
		{
			if(DefaultOptionsScreenFactory == null)
				throw new ArgumentNullException();
			
			GoToMode(DefaultOptionsScreenFactory);
		}
		
		#endregion
		
		#region Graphics
		
		//TODO: This doesn't take screen rotation into account because PSM doesn't either.
		
		private void InitializeGraphics(GraphicsContext gc)
		{
			GraphicsContext = gc;
			
			ScreenWidth = GraphicsContext.Screen.Width;
			ScreenHeight = GraphicsContext.Screen.Height;
			ScreenRectangle = GraphicsContext.Screen.Rectangle;
			
			DevicePpiX = SystemParameters.DisplayDpiX;
			DevicePpiY = SystemParameters.DisplayDpiY;
			DeviceSizeWidth = (Single)Math.Round(ScreenWidth / DevicePpiX, 1);
			DeviceSizeHeight = (Single)Math.Round(ScreenHeight / DevicePpiY, 1);
			
			DevicePpi = CalculateDevicePpi();
			DeviceSize = CalculateDeviceSize();
		}
		
		private void CleanupGraphics()
		{
			GraphicsContext.Dispose();
			GraphicsContext = null;
		}
		
		internal GraphicsContext GraphicsContext { get; private set; }
		public Single ScreenWidth { get; private set; }
		public Single ScreenHeight { get; private set; }
		public ImageRect ScreenRectangle { get; private set; }
		public Single DevicePpi { get; private set; }
		public Single DevicePpiX { get; private set; }
		public Single DevicePpiY { get; private set; }
		public Single DeviceSize { get; private set; }
		public Single DeviceSizeWidth { get; private set; }
		public Single DeviceSizeHeight { get; private set; }
		
		//public Single DeviceStyle { get; private set; }
		
		private Single CalculateDevicePpi()
		{
			if(DevicePpiX < 1 || DevicePpiY < 1)
				throw new InvalidOperationException();
			
			if(DevicePpiX == DevicePpiY)
				return DevicePpiX;
			
			//Is it best to just average them???
			return (Single)Math.Round((DevicePpiX + DevicePpiY) / 2, 1);
		}
		
		private Single CalculateDeviceSize()
		{
			Single diagonalInPixels = (Single)Math.Sqrt(Math.Pow(ScreenWidth, 2) + Math.Pow(ScreenHeight, 2));
			Single diagonalInInches = (Single)Math.Round(diagonalInPixels / DevicePpi, 1);
			
			return diagonalInInches;
		}
		
		#endregion
		
		#region Timers, Performace tracking
		
		private void InitializeTimers()
		{
			FrameTimer = new Stopwatch();
			UpdateTimer = new Stopwatch();
			RenderTimer = new Stopwatch();
			
			CalculateUpdateTime();
		}
		
		private void CleanupTimers()
		{
			FrameTimer.Stop();
			FrameTimer = null;
			
			UpdateTimer.Stop();
			UpdateTimer = null;
			
			RenderTimer.Stop();
			RenderTimer = null;
		}
		
		private Stopwatch FrameTimer;
		private Stopwatch UpdateTimer;
		private Stopwatch RenderTimer;
		
		public DateTime UpdateTime { get; private set; }
		public TimeSpan TimeSinceLastFrame { get; private set; }
		public TimeSpan UpdateLength { get; private set; }
		public TimeSpan RenderLength { get; private set; }
		
		private void CalculateUpdateTime()
		{
			UpdateTime = DateTime.Now;
		}
		
		private void CalculateTimeSinceLastFrame()
		{
			TimeSinceLastFrame = FrameTimer.Elapsed;
			FrameTimer.Reset();
			FrameTimer.Start();
		}
		
		private void StartUpdateTimer()
		{
			UpdateTimer.Reset();
			UpdateTimer.Start();
		}
		
		private void CompleteUpdateTimer()
		{
			UpdateTimer.Stop();
			UpdateLength = UpdateTimer.Elapsed;
		}
		
		private void StartRenderTimer()
		{
			RenderTimer.Reset();
			RenderTimer.Start();
		}
		
		private void CompleteRenderTimer()
		{
			RenderTimer.Stop();
			RenderLength = RenderTimer.Elapsed;
		}
		
		private void PauseInGameTimer()
		{
			//throw new NotImplementedException();
		}
		
		private void ResumeInGameTimer()
		{
			//throw new NotImplementedException();
		}
		
		public Int32 FramesPerSecond { get; private set; }
		private Int32 LastSecondTracked;
		private Int32 CurrentFramesPerSecond;
		
		private void CalculateFramesPerSecond()
		{
			Int32 CurrentSec = UpdateTime.Second;
			
			if(LastSecondTracked == CurrentSec)
				CurrentFramesPerSecond++;
			else
			{
				LastSecondTracked = CurrentSec;
				FramesPerSecond = CurrentFramesPerSecond;
				CurrentFramesPerSecond = 1;
			}
		}
		
		public void FreeMemory()
		{
			GC.Collect();
		}
		
		#endregion
		
		#region Options
		
		private void InitializeOptions(AppOptionsBase options)
		{
			Options = options;
		}
		
		private void CleanupOptions()
		{
			Options.Dispose();
			Options = null;
		}
		
		public AppOptionsBase Options { get; private set; }
		
		#endregion
		
		#region Random Numbers
		
		private void InitializeRandomGenerator()
		{
			RandomGenerator = new RandomGenerator(System.Environment.TickCount);
		}
		
		private void CleanupRandomGenerator()
		{
			RandomGenerator.Dispose();
			RandomGenerator = null;
		}
		
		public RandomGenerator RandomGenerator { get; private set; }
		
		#endregion
		
		#region Input
		
		//TODO: Add timers to record how long buttons have been held down for.
		
		private void InitializeInput()
		{
			CollectGamePadData = true;
			CollectTouchData = false;
		}
		
		private void CleanupInput()
		{
			CollectGamePadData = false;
			CollectTouchData = false;
		}
		
		public Boolean CollectGamePadData { get; private set; }
		public Boolean CollectTouchData { get; private set; }
		
		private GamePadData GamePadData;// { get; private set; }
		public List<TouchData> TouchData { get; private set; }
		
		private void RefreshInputData()
		{
			if (CollectGamePadData)
				GamePadData = GamePad.GetData(0);
			if (CollectTouchData)
				TouchData = Touch.GetData(0);
		}
		
		#region GamePad Button Shortcuts
		
		#region Up
		
		public Boolean GamePad0_Up_Pressed
		{
			get { return ((GamePadData.ButtonsDown & GamePadButtons.Up) != 0); }
		}
		
		public Boolean GamePad0_Up
		{
			get { return ((GamePadData.Buttons & GamePadButtons.Up) != 0); }
		}
		
		public Boolean GamePad0_Up_Released
		{
			get { return ((GamePadData.ButtonsUp & GamePadButtons.Up) != 0); }
		}
		
		#endregion
		
		#region Down
		
		public Boolean GamePad0_Down_Pressed
		{
			get { return ((GamePadData.ButtonsDown & GamePadButtons.Down) != 0); }
		}
		
		public Boolean GamePad0_Down
		{
			get { return ((GamePadData.Buttons & GamePadButtons.Down) != 0); }
		}
		
		public Boolean GamePad0_Down_Released
		{
			get { return ((GamePadData.ButtonsUp & GamePadButtons.Down) != 0); }
		}
		
		#endregion
		
		#region Left
		
		public Boolean GamePad0_Left_Pressed
		{
			get { return ((GamePadData.ButtonsDown & GamePadButtons.Left) != 0); }
		}
		
		public Boolean GamePad0_Left
		{
			get { return ((GamePadData.Buttons & GamePadButtons.Left) != 0); }
		}
		
		public Boolean GamePad0_Left_Released
		{
			get { return ((GamePadData.ButtonsUp & GamePadButtons.Left) != 0); }
		}
		
		#endregion
		
		#region Right
		
		public Boolean GamePad0_Right_Pressed
		{
			get { return ((GamePadData.ButtonsDown & GamePadButtons.Right) != 0); }
		}
		
		public Boolean GamePad0_Right
		{
			get { return ((GamePadData.Buttons & GamePadButtons.Right) != 0); }
		}
		
		public Boolean GamePad0_Right_Released
		{
			get { return ((GamePadData.ButtonsUp & GamePadButtons.Right) != 0); }
		}
		
		#endregion
		
		#region Cross
		
		public Boolean GamePad0_Cross_Pressed
		{
			get { return ((GamePadData.ButtonsDown & GamePadButtons.Cross) != 0); }
		}
		
		public Boolean GamePad0_Cross
		{
			get { return ((GamePadData.Buttons & GamePadButtons.Cross) != 0); }
		}
		
		public Boolean GamePad0_Cross_Released
		{
			get { return ((GamePadData.ButtonsUp & GamePadButtons.Cross) != 0); }
		}
		
		#endregion
		
		#region Square
		
		public Boolean GamePad0_Square_Pressed
		{
			get { return ((GamePadData.ButtonsDown & GamePadButtons.Square) != 0); }
		}
		
		public Boolean GamePad0_Square
		{
			get { return ((GamePadData.Buttons & GamePadButtons.Square) != 0); }
		}
		
		public Boolean GamePad0_Square_Released
		{
			get { return ((GamePadData.ButtonsUp & GamePadButtons.Square) != 0); }
		}
		
		#endregion
		
		#region Triangle
		
		public Boolean GamePad0_Triangle_Pressed
		{
			get { return ((GamePadData.ButtonsDown & GamePadButtons.Triangle) != 0); }
		}
		
		public Boolean GamePad0_Triangle
		{
			get { return ((GamePadData.Buttons & GamePadButtons.Triangle) != 0); }
		}
		
		public Boolean GamePad0_Triangle_Released
		{
			get { return ((GamePadData.ButtonsUp & GamePadButtons.Triangle) != 0); }
		}
		
		#endregion
		
		#region Circle
		
		public Boolean GamePad0_Circle_Pressed
		{
			get { return ((GamePadData.ButtonsDown & GamePadButtons.Circle) != 0); }
		}
		
		public Boolean GamePad0_Circle
		{
			get { return ((GamePadData.Buttons & GamePadButtons.Circle) != 0); }
		}
		
		public Boolean GamePad0_Circle_Released
		{
			get { return ((GamePadData.ButtonsUp & GamePadButtons.Circle) != 0); }
		}
		
		#endregion
		
		#region L1
		
		public Boolean GamePad0_L1_Pressed
		{
			get { return ((GamePadData.ButtonsDown & GamePadButtons.L) != 0); }
		}
		
		public Boolean GamePad0_L1
		{
			get { return ((GamePadData.Buttons & GamePadButtons.L) != 0); }
		}
		
		public Boolean GamePad0_L1_Released
		{
			get { return ((GamePadData.ButtonsUp & GamePadButtons.L) != 0); }
		}
		
		#endregion
		
		#region R1
		
		public Boolean GamePad0_R1_Pressed
		{
			get { return ((GamePadData.ButtonsDown & GamePadButtons.R) != 0); }
		}
		
		public Boolean GamePad0_R1
		{
			get { return ((GamePadData.Buttons & GamePadButtons.R) != 0); }
		}
		
		public Boolean GamePad0_R1_Released
		{
			get { return ((GamePadData.ButtonsUp & GamePadButtons.R) != 0); }
		}
		
		#endregion
		
		#region Start
		
		public Boolean GamePad0_Start_Pressed
		{
			get { return ((GamePadData.ButtonsDown & GamePadButtons.Start) != 0); }
		}
		
		public Boolean GamePad0_Start
		{
			get { return ((GamePadData.Buttons & GamePadButtons.Start) != 0); }
		}
		
		public Boolean GamePad0_Start_Released
		{
			get { return ((GamePadData.ButtonsUp & GamePadButtons.Start) != 0); }
		}
		
		#endregion
		
		#region Select
		
		public Boolean GamePad0_Select_Pressed
		{
			get { return ((GamePadData.ButtonsDown & GamePadButtons.Select) != 0); }
		}
		
		public Boolean GamePad0_Select
		{
			get { return ((GamePadData.Buttons & GamePadButtons.Select) != 0); }
		}
		
		public Boolean GamePad0_Select_Released
		{
			get { return ((GamePadData.ButtonsUp & GamePadButtons.Select) != 0); }
		}
		
		#endregion
		
		#region Back
		
		public Boolean GamePad0_Back_Pressed
		{
			get { return ((GamePadData.ButtonsDown & GamePadButtons.Back) != 0); }
		}
		
		public Boolean GamePad0_Back
		{
			get { return ((GamePadData.Buttons & GamePadButtons.Back) != 0); }
		}
		
		public Boolean GamePad0_Back_Released
		{
			get { return ((GamePadData.ButtonsUp & GamePadButtons.Back) != 0); }
		}
		
		#endregion
		
		#region Enter
		
		public Boolean GamePad0_Enter_Pressed
		{
			get { return ((GamePadData.ButtonsDown & GamePadButtons.Enter) != 0); }
		}
		
		public Boolean GamePad0_Enter
		{
			get { return ((GamePadData.Buttons & GamePadButtons.Enter) != 0); }
		}
		
		public Boolean GamePad0_Enter_Released
		{
			get { return ((GamePadData.ButtonsUp & GamePadButtons.Enter) != 0); }
		}
		
		#endregion
		
		#endregion
		
		#region Analog Stick Shortcuts
		
		public Single AnalogStickDeadZone = 0.01f;
		
		#region Left Stick
		
		public Boolean GamePad0_LeftStick_Active
		{
			get { return FMath.Abs(GamePad0_LeftStick_X) > AnalogStickDeadZone || FMath.Abs(GamePad0_LeftStick_Y) > AnalogStickDeadZone; }
		}
		
		public Single GamePad0_LeftStick_X
		{
			get { return GamePadData.AnalogLeftX; }
		}
		
		public Single GamePad0_LeftStick_Y
		{
			get { return GamePadData.AnalogLeftY; }
		}
		
		#endregion
		
		#region Right Stick
		
		public Boolean GamePad0_RightStick_Active
		{
			get { return FMath.Abs(GamePad0_RightStick_X) > AnalogStickDeadZone || FMath.Abs(GamePad0_RightStick_Y) > AnalogStickDeadZone; }
		}
		
		public Single GamePad0_RightStick_X
		{
			get { return GamePadData.AnalogRightX; }
		}
		
		public Single GamePad0_RightStick_Y
		{
			get { return GamePadData.AnalogRightY; }
		}
		
		#endregion
		
		#endregion
		
		#endregion
	}
}

