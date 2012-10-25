using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Calendo
{
    /// <summary>
    /// Fixes for maximizing window
    /// Courtesy of LesterLobo
    /// http://blogs.msdn.com/b/llobo/archive/2006/08/01/maximizing-window-_2800_with-windowstyle_3d00_none_2900_-considering-taskbar.aspx
    /// </summary>
    public class FormMaximize
    {
        /// <summary>
        /// Directly override WinProc messages
        /// </summary>
        public static System.IntPtr WindowProc(
              System.IntPtr hwnd,
              int msg,
              System.IntPtr wParam,
              System.IntPtr lParam,
              ref bool handled)
        {
            switch (msg)
            {
                case 0x0024: // Directly handle WM_GETMINMAXINFO message
                    WmGetMinMaxInfo(hwnd, lParam);
                    handled = true;
                    break;
            }

            return (System.IntPtr)0;
        }

        /// <summary>
        /// Get Min-Max screen information from WinAPI
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="param"></param>
        private static void WmGetMinMaxInfo(System.IntPtr handle, System.IntPtr param)
        {
            MINMAXINFO mmInfo = (MINMAXINFO)Marshal.PtrToStructure(param, typeof(MINMAXINFO));

            // Get current monitor information
            int MONITOR_DEFAULT_TO_NEAREST = 0x00000002;
            System.IntPtr monitor = MonitorFromWindow(handle, MONITOR_DEFAULT_TO_NEAREST);

            if (monitor != System.IntPtr.Zero) // Not null pointer
            {
                MONITORINFO monitorInfo = new MONITORINFO();
                GetMonitorInfo(monitor, monitorInfo);
                RECT rcWorkArea = monitorInfo.rcWork;
                RECT rcMonitorArea = monitorInfo.rcMonitor;
                mmInfo.ptMaxPosition.x = Math.Abs(rcWorkArea.left - rcMonitorArea.left);
                mmInfo.ptMaxPosition.y = Math.Abs(rcWorkArea.top - rcMonitorArea.top);
                mmInfo.ptMaxSize.x = Math.Abs(rcWorkArea.right - rcWorkArea.left);
                mmInfo.ptMaxSize.y = Math.Abs(rcWorkArea.bottom - rcWorkArea.top);
            }
            Marshal.StructureToPtr(mmInfo, param, true);
        }

        // Structures below are used by WinAPI

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;

            public POINT(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MINMAXINFO
        {
            public POINT ptReserved;
            public POINT ptMaxSize;
            public POINT ptMaxPosition;
            public POINT ptMinTrackSize;
            public POINT ptMaxTrackSize;
        };

        [StructLayout(LayoutKind.Sequential)]
        public class MONITORINFO
        {
            public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));
            public RECT rcMonitor = new RECT();
            public RECT rcWork = new RECT();
            public int dwFlags = 0;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        // Win API external reference
        [DllImport("User32")]
        internal static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);
        [DllImport("User32")]
        internal static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);
    }
}
