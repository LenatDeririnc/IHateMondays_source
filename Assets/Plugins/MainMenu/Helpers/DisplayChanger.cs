using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace MainMenu.Helpers
{
    public class DisplayChanger : MonoBehaviour
    {
        //cycles through the connected displays by calling the ChangeDisplayClicked() method
        //does not work with windowed mode
 
        List<DisplayInfo> Displays = new List<DisplayInfo>();
        List<Monitor> Monitors = new List<Monitor>();
        int monitorNumber = 0;
 
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, ExactSpelling = true, SetLastError = true)]
        internal static extern void MoveWindow(IntPtr hwnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
 
        [DllImport("user32.dll")]
        private static extern IntPtr GetActiveWindow();
 
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, ExactSpelling = true, SetLastError = true)]
        internal static extern bool GetWindowRect(IntPtr hWnd, ref RECT rect);
 
        [DllImport("user32.dll")]
        static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, MonitorEnumDelegate lpfnEnum, IntPtr dwData);
 
        delegate bool MonitorEnumDelegate(IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData);
 
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }
 
        public class DisplayInfo
        {
            public string Availability { get; set; }
            public string ScreenHeight { get; set; }
            public string ScreenWidth { get; set; }
            public RECT MonitorArea { get; set; }
            public RECT WorkArea { get; set; }
        }
 
        public class DisplayInfoCollection : List<DisplayInfo>
        {
        }
 
        [DllImport("User32.dll", CharSet = CharSet.Unicode)]
        public static extern bool GetMonitorInfo(IntPtr hmonitor, [In, Out] MONITORINFOEX info);
        [DllImport("User32.dll", ExactSpelling = true)]
        public static extern IntPtr MonitorFromPoint(POINTSTRUCT pt, int flags);
 
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 4)]
        public class MONITORINFOEX
        {
            public int cbSize = Marshal.SizeOf(typeof(MONITORINFOEX));
            public RECT rcMonitor = new RECT();
            public RECT rcWork = new RECT();
            public int dwFlags = 0;
            [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U2, SizeConst = 32)]
            public char[] szDevice = new char[32];
        }
 
        [StructLayout(LayoutKind.Sequential)]
        public struct POINTSTRUCT
        {
            public int x;
            public int y;
            public POINTSTRUCT(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }
 
        public DisplayInfoCollection GetDisplays()
        {
            DisplayInfoCollection col = new DisplayInfoCollection();
 
            EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero,
                delegate (IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData)
                {
                    MONITORINFOEX mi = new MONITORINFOEX();
                    mi.cbSize = (int)Marshal.SizeOf(mi);
                    bool success = GetMonitorInfo(hMonitor, mi);
                    if (success)
                    {
                        DisplayInfo di = new DisplayInfo();
                        di.ScreenWidth = (mi.rcMonitor.right - mi.rcMonitor.left).ToString();
                        di.ScreenHeight = (mi.rcMonitor.bottom - mi.rcMonitor.top).ToString();
                        di.MonitorArea = mi.rcMonitor;
                        di.WorkArea = mi.rcWork;
                        di.Availability = mi.dwFlags.ToString();
                        col.Add(di);
                    }
                    return true;
                }, IntPtr.Zero);
            return col;
        }
 
        public class Monitor
        {
            public int targetX;
            public int targetY;
            public int monitorNumber;
            public int height;
            public int width;
 
            public Monitor(int targetX, int targetY, int monitorNumber, int height, int width)
            {
                this.targetX = targetX;
                this.targetY = targetY;
                this.monitorNumber = monitorNumber;
                this.height = height;
                this.width = width;
            }
        }
 
        private void Start()
        {
            Displays = GetDisplays();
            for (int i = 0; i < Displays.Count; i++)
            {
                Monitors.Add(new Monitor(Displays[i].WorkArea.left, Displays[i].WorkArea.top ,i, Convert.ToInt32(Displays[i].ScreenHeight), Convert.ToInt32(Displays[i].ScreenWidth)));
            }
        }
 
        public void ChangeDisplayClicked()
        {
            if (monitorNumber < Displays.Count)
            {
                StartCoroutine(MoveWindowCoroutine(Monitors[monitorNumber].height, Monitors[monitorNumber].width, Monitors[monitorNumber].targetX, Monitors[monitorNumber].targetY));
                monitorNumber++;
            }
            else if (monitorNumber >= Displays.Count)
            {
                monitorNumber = 0;
                StartCoroutine(MoveWindowCoroutine(Monitors[monitorNumber].height, Monitors[monitorNumber].width, Monitors[monitorNumber].targetX, Monitors[monitorNumber].targetY));
            }
        }

        public void SetDisplay(int displayId)
        {
            if (displayId >= Displays.Count)
                return;
            
            StartCoroutine(MoveWindowCoroutine(Monitors[displayId].height, Monitors[displayId].width, Monitors[displayId].targetX, Monitors[displayId].targetY));
        }
 
        public IEnumerator MoveWindowCoroutine(int newHeight, int newWidth, int targetX, int targetY)
        {
            IntPtr hwnd;
            RECT Rect = new RECT();
            yield return new WaitForSeconds(0.5f);
            hwnd = GetActiveWindow();
            GetWindowRect(hwnd, ref Rect);
            MoveWindow(hwnd, targetX, targetY, newWidth, newHeight, true);
        }
    }
}