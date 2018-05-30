using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

// ReSharper disable InconsistentNaming

namespace Faithlife.DockerShim.Services
{
	// See https://github.com/moby/moby/issues/25982#issuecomment-375105522 / http://www.webcitation.org/6z4QjWT0i
	internal sealed class WindowsSignalService : ISignalService
	{
		/// <summary>
		/// Creates a Windows implementation of <see cref="ISignalService"/>.
		/// </summary>
		public WindowsSignalService()
		{
			// Since SetConsoleCtrlHandler invokes its handlers on a last-registered, first-called basis, we force the .NET console to register its handler before us
			//  by subscribing an empty handler to Console.CancelKeyPress.
			//  (see https://docs.microsoft.com/en-us/windows/console/setconsolectrlhandler / http://www.webcitation.org/6z49Wueb1 )
			Console.CancelKeyPress += (_, args) => { };

			m_handlerRoutine = ConsoleCtrlHandler;
			if (!SetConsoleCtrlHandler(m_handlerRoutine, add: true))
				throw Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());
		}

		public void AddHandler(Action<string> handler) => Handler += handler;

		private bool ConsoleCtrlHandler(ConsoleControlEvent controlType)
		{
			if (controlType == ConsoleControlEvent.CTRL_C_EVENT || controlType == ConsoleControlEvent.CTRL_CLOSE_EVENT ||
			    controlType == ConsoleControlEvent.CTRL_SHUTDOWN_EVENT)
			{
				Handler?.Invoke(controlType.ToString());
				return true;
			}

			return false;
		}

		private enum ConsoleControlEvent : uint
		{
			CTRL_C_EVENT = 0,
			CTRL_CLOSE_EVENT = 2,
			CTRL_SHUTDOWN_EVENT = 6,
		}

		[UnmanagedFunctionPointer(CallingConvention.Winapi)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private delegate bool SetConsoleCtrlHandler_HandlerRoutine(ConsoleControlEvent controlType);

		[DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool SetConsoleCtrlHandler(SetConsoleCtrlHandler_HandlerRoutine handler,
			[MarshalAs(UnmanagedType.Bool)] bool add);

		private event Action<string> Handler;

		// ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
		private readonly SetConsoleCtrlHandler_HandlerRoutine m_handlerRoutine;
	}
}
