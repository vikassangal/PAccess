using System;
using System.Runtime.InteropServices;

namespace PatientAccess.UI.UserInactivityMonitoring
{
	internal delegate int Win32HookProcHandler(int nCode, IntPtr wParam, IntPtr lParam);

	internal class User32
	{
		[DllImport("user32.dll", CharSet = CharSet.Auto,
				    CallingConvention = CallingConvention.StdCall)]
		internal static extern int SetWindowsHookEx(int idHook, Win32HookProcHandler lpfn,
												   IntPtr hInstance, int threadId);

		[DllImport("user32.dll", CharSet = CharSet.Auto,
				   CallingConvention = CallingConvention.StdCall)]
		internal static extern bool UnhookWindowsHookEx(int idHook);

		[DllImport("user32.dll", CharSet = CharSet.Auto,
				   CallingConvention = CallingConvention.StdCall)]
		internal static extern int CallNextHookEx(int idHook, int nCode,
												 IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll", CharSet = CharSet.Auto,
				   CallingConvention = CallingConvention.StdCall)]
		internal static extern bool GetLastInputInfo(out Win32LastInputInfo plii);

		private User32()
		{
		}
	}
}
