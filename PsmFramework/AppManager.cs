using System;
using System.Collections.Generic;
using System.Diagnostics;
using PsmFramework.Modes;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Input;

namespace PsmFramework
{
	public sealed class AppManager
	{
		#region Constructor, Dispose
		
		public AppManager(AppOptionsBase options, GraphicsContext gc)
		{
			Initialize(options, gc);
		}
		
		public void Dispose()
		{
			Cleanup();
		}
		
		#endregion
		
		#region Initialize, Cleanup
		
		private void Initialize(AppOptionsBase options, GraphicsContext gc)
		{
			SetRunStateToInitializing();
			UpdateRunState();
			
			InitializeOptions(options);
			InitializeGraphics(gc);
			InitializeTimers();
			InitializeModes();
			InitializeInput();
		}
		
		private void Cleanup()
		{
			CleanupInput();
			CleanupModes();
			CleanupTimers();
			CleanupGraphics();
			CleanupOptions();
		}
		
		#endregion
		
		#region AppLoop, Update, Render
		
		public void AppLoop()
		{
			SetRunStateToRunning();
			
			InitializeCurrentMode();
			
			while (RunState != RunState.Ending)
			{
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
			
			CurrentMode.UpdateInternal();
			CurrentMode.Update();
			
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
		
		#region Graphics
		
		private void InitializeGraphics(GraphicsContext gc)
		{
			GraphicsContext = gc;
			
			ScreenWidth = GraphicsContext.Screen.Width;
			ScreenHeight = GraphicsContext.Screen.Height;
			ScreenRectangle = GraphicsContext.Screen.Rectangle;
		}
		
		private void CleanupGraphics()
		{
			GraphicsContext.Dispose();
			GraphicsContext = null;
		}
		
		public GraphicsContext GraphicsContext { get; private set; }
		public Single ScreenWidth { get; private set; }
		public Single ScreenHeight { get; private set; }
		public ImageRect ScreenRectangle { get; private set; }
		
		#endregion
		
		#region Timers, Performace tracking
		
		private void InitializeTimers()
		{
			FrameTimer = new Stopwatch();
			UpdateTimer = new Stopwatch();
			RenderTimer = new Stopwatch();
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
		
		public TimeSpan TimeSinceLastFrame { get; private set; }
		public TimeSpan UpdateLength { get; private set; }
		public TimeSpan RenderLength { get; private set; }
		
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
		}
		
		private void ResumeInGameTimer()
		{
		}
		
		public Int32 FramesPerSecond { get; private set; }
		private Int32 LastSecondTracked;
		private Int32 CurrentFramesPerSecond;
		
		private void CalculateFramesPerSecond()
		{
			Int32 CurrentSec = DateTime.Now.Second;
			
			if(LastSecondTracked == CurrentSec)
				CurrentFramesPerSecond++;
			else
			{
				LastSecondTracked = CurrentSec;
				FramesPerSecond = CurrentFramesPerSecond;
				CurrentFramesPerSecond = 1;
			}
		}
		
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
		
		public GamePadData GamePadData { get; private set; }
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
		
		#endregion
		
		#region Modes
		
		private void InitializeModes()
		{
			PreviousMode = null;
			CurrentMode = null;
			ReturnMode = null;
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
		}
		
		public delegate ModeBase CreateModeDelegate(AppManager mgr);
		
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
				return (DateTime.UtcNow - LastModeChange) > cMinTicksBetweenModeChanges;
			}
		}
		
		public Boolean ModeChanged
		{
			get { return PreviousMode != null; }
		}
		
		public void GoToMode(CreateModeDelegate factory)
		{
			LastModeChange = DateTime.UtcNow;
			PreviousMode = CurrentMode;
			NextModeFactory = factory;
			CurrentMode = null;
			ReturnMode = null;
		}
		
		//TODO: GoToThenReturn should not dispose of the original mode. perhaps as an option or another method.
		public void GoToModeThenReturn(CreateModeDelegate factory, ModeBase returnMode)
		{
			LastModeChange = DateTime.UtcNow;
			PreviousMode = CurrentMode;
			NextModeFactory = factory;
			CurrentMode = null;
			ReturnMode = returnMode;
		}
		
		public void ReturnToMode()
		{
			LastModeChange = DateTime.UtcNow;
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
			//PreviousMode.CleanupInternal();
			PreviousMode.Dispose();
			PreviousMode = null;
			
			//TODO: Re-enable this after Node finalizer is fixed!!!
			//if (!Debugger.IsAttached)
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
		
		private RandomGenerator _RandomGenerator = new RandomGenerator(System.Environment.TickCount);
		public RandomGenerator RandomGenerator { get { return _RandomGenerator; } }
		
		#endregion
	}
}

