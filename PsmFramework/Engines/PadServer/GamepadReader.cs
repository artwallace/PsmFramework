using System;
using System.Net;
using System.Net.Sockets;
using Sce.PlayStation.Core.Input;

namespace PsmFramework.Engines.PadServer
{
	public class GamepadReader : IDisposable
	{
		#region Constructor, Dispose
		
		public GamepadReader(ControllerTypes controllerType)
		{
			Initialize(controllerType);
		}
		
		public void Dispose()
		{
			Cleanup();
		}
		
		#endregion
		
		#region Initialize, Cleanup
		
		private void Initialize(ControllerTypes controllerType)
		{
			InitializeControllerType(controllerType);
			InitializeTcpip();
			InitializeChannel();
		}
		
		private void Cleanup()
		{
			CleanupChannel();
			CleanupTcpip();
			CleanupControllerType();
		}
		
		#endregion
		
		#region Controller Type
		
		private void InitializeControllerType(ControllerTypes controllerType)
		{
			ControllerType = controllerType;
		}
		
		private void CleanupControllerType()
		{
		}
		
		public ControllerTypes ControllerType { get; set; }
		
		#endregion
		
		#region TcpIp
		
		private void InitializeTcpip()
		{
			TcpipAddress = IPAddress.Parse(TcpipAddressStr);
			TcpipEndPoint = new IPEndPoint(TcpipAddress, TcpipPort);
		}
		
		private void CleanupTcpip()
		{
			TcpipAddress = null;
			TcpipEndPoint = null;
		}
		
		public static String TcpipAddressStr = "127.0.0.1";
		private IPAddress TcpipAddress;
		public static Int32 TcpipPort = 1777;
		
		private IPEndPoint TcpipEndPoint;
		
		protected Socket TcpipSocket;
		
		#endregion
		
		#region Channel
		
		private void InitializeChannel()
		{
			ChannelState = ChannelStates.Closed;
			
			RequestMessage = new Byte[] { (Byte)'i' };
			CloseMessage = new Byte[1] { (Byte)'s' };
			
			IncomingDataBuffer = new Byte[IncomingMessageLength];
		}
		
		private void CleanupChannel()
		{
			if(ChannelState != ChannelStates.Closed)
				CloseChannel();
			
			RequestMessage = new Byte[0];
			CloseMessage = new Byte[0];
		}
		
		public void OpenChannel()
		{
			if(ChannelState != ChannelStates.Closed)
				throw new InvalidOperationException();
			
			TcpipSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			
			try
			{
				TcpipSocket.Connect(TcpipEndPoint);
				ChannelState = ChannelStates.Open;
			}
			catch(SocketException e)
			{
				TcpipSocket = null;
				throw new InvalidOperationException("Unable to access PadServer over TCP/IP.", e);
			}
		}
		
		public void CloseChannel()
		{
			if(ChannelState != ChannelStates.Open)
				throw new InvalidOperationException();
			
			if(TcpipSocket != null)
			{
				TcpipSocket.Send(CloseMessage);
				TcpipSocket.Shutdown(SocketShutdown.Both);
				TcpipSocket.Close();
				TcpipSocket.Dispose();
				TcpipSocket = null;
			}
			
			ChannelState = ChannelStates.Closed;
		}
		
		private void ReadDataFromChannel()
		{
			if(ChannelState != ChannelStates.Open)
				throw new InvalidOperationException();
			
			if(TcpipSocket == null)
				throw new InvalidOperationException();
			
			TcpipSocket.Send(RequestMessage);
			TcpipSocket.Receive(IncomingDataBuffer);
			
			Int32 incomingDataSize = BitConverter.ToInt32(IncomingDataBuffer, 0);
			if(incomingDataSize != IncomingMessageLength)
				throw new InvalidOperationException("Invalid data received from PadServer.");
		}
		
		public ChannelStates ChannelState { get; private set; }
		
		private Byte[] RequestMessage;
		
		private Byte[] IncomingDataBuffer;
		
		private Byte[] CloseMessage;
		
		private const Int32 IncomingMessageLength = 24;
		
		#endregion
		
		#region GetData
		
		public GamePadData GetData()
		{
			ReadDataFromChannel();
			
			GamePadData gpd = new GamePadData();
			
			gpd.AnalogLeftX = BitConverter.ToSingle(IncomingDataBuffer, 4);
			gpd.AnalogLeftY = BitConverter.ToSingle(IncomingDataBuffer, 8);
			gpd.AnalogRightX = BitConverter.ToSingle(IncomingDataBuffer, 12);
			gpd.AnalogRightY = BitConverter.ToSingle(IncomingDataBuffer, 16);
			
			RawButtonData = BitConverter.ToUInt32(IncomingDataBuffer, 20);
			
			//The order of button decoding is important here.
			switch(ControllerType)
			{
			case ControllerTypes.SonySixaxis:
				if(ReadNextRawButtonValue())
					gpd.Buttons = gpd.Buttons | GamePadButtons.Triangle;
				if(ReadNextRawButtonValue())
					gpd.Buttons = gpd.Buttons | GamePadButtons.Circle;
				if(ReadNextRawButtonValue())
					gpd.Buttons = gpd.Buttons | GamePadButtons.Cross;
				if(ReadNextRawButtonValue())
					gpd.Buttons = gpd.Buttons | GamePadButtons.Square;
				if(ReadNextRawButtonValue())
				{
					//L2, do nothing.
				}
				if(ReadNextRawButtonValue())
				{
					//R2, do nothing.
				}
				if(ReadNextRawButtonValue())
					gpd.Buttons = gpd.Buttons | GamePadButtons.L;
				if(ReadNextRawButtonValue())
					gpd.Buttons = gpd.Buttons | GamePadButtons.R;
				if(ReadNextRawButtonValue())
					gpd.Buttons = gpd.Buttons | GamePadButtons.Start;
				if(ReadNextRawButtonValue())
					gpd.Buttons = gpd.Buttons | GamePadButtons.Select;
				if(ReadNextRawButtonValue())
				{
					//L3, do nothing.
				}
				if(ReadNextRawButtonValue())
				{
					//R3, do nothing.
				}
				if(ReadNextRawButtonValue())
					gpd.Buttons = gpd.Buttons | GamePadButtons.Left;
				if(ReadNextRawButtonValue())
					gpd.Buttons = gpd.Buttons | GamePadButtons.Right;
				if(ReadNextRawButtonValue())
					gpd.Buttons = gpd.Buttons | GamePadButtons.Up;
				if(ReadNextRawButtonValue())
					gpd.Buttons = gpd.Buttons | GamePadButtons.Down;
				break;
				
			case ControllerTypes.MotionInJoy:
				if(ReadNextRawButtonValue())
					gpd.Buttons = gpd.Buttons | GamePadButtons.Triangle;
				if(ReadNextRawButtonValue())
					gpd.Buttons = gpd.Buttons | GamePadButtons.Circle;
				if(ReadNextRawButtonValue())
					gpd.Buttons = gpd.Buttons | GamePadButtons.Cross;
				if(ReadNextRawButtonValue())
					gpd.Buttons = gpd.Buttons | GamePadButtons.Square;
				if(ReadNextRawButtonValue())
				{
					//L2, do nothing.
				}
				if(ReadNextRawButtonValue())
				{
					//R2, do nothing.
				}
				if(ReadNextRawButtonValue())
					gpd.Buttons = gpd.Buttons | GamePadButtons.L;
				if(ReadNextRawButtonValue())
					gpd.Buttons = gpd.Buttons | GamePadButtons.R;
				if(ReadNextRawButtonValue())
					gpd.Buttons = gpd.Buttons | GamePadButtons.Select;
				if(ReadNextRawButtonValue())
					gpd.Buttons = gpd.Buttons | GamePadButtons.Start;
				if(ReadNextRawButtonValue())
				{
					//L3, do nothing.
				}
				if(ReadNextRawButtonValue())
				{
					//R3, do nothing.
				}
				if(ReadNextRawButtonValue())
					gpd.Buttons = gpd.Buttons | GamePadButtons.Left;
				if(ReadNextRawButtonValue())
					gpd.Buttons = gpd.Buttons | GamePadButtons.Right;
				if(ReadNextRawButtonValue())
					gpd.Buttons = gpd.Buttons | GamePadButtons.Up;
				if(ReadNextRawButtonValue())
					gpd.Buttons = gpd.Buttons | GamePadButtons.Down;
				break;
			
			default:
				throw new InvalidProgramException("Unknown controller type.");
			}
			
			return gpd;
		}
		
		private Boolean ReadNextRawButtonValue()
		{
			Boolean pressed = (RawButtonData & 1) != 0;
			RawButtonData >>= 1;
			return pressed;
		}
		
		private UInt32 RawButtonData;
		//private UInt32 LastRawButtonData;
		
		//private GamePadData TranslateIncomingData()
		//{
//			uint curb = BitConverter.ToUInt32(ibuf, 20);
//			uint newb = curb & (~buttons);
//			uint repb = curb & buttons;
//			
//			buttons = curb;
			
			//Current.decodeNet(curb);
			//NewlyPressed.decodeNet(newb);
			//Repeat.decodeNet(repb);
		//}
		
		#endregion
	}
}
