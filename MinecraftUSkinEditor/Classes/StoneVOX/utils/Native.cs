using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;

namespace stonevox
{
    public static class Native
    {
        [DllImport("gdi32.dll", EntryPoint = "AddFontResourceW", SetLastError = true)]
        static extern int AddFontResource([In][MarshalAs(UnmanagedType.LPWStr)]
                                         string lpFileName);

        public static void AddFont(string filePath)
        {
            var result = AddFontResource(filePath);
            var error = Marshal.GetLastWin32Error();
            if (error != 0)
            {
                Console.WriteLine(new Win32Exception(error).Message);
            }
            else
            {
                Console.WriteLine((result == 0) ? "Font is already installed." :
                                                  "Font installed successfully.");
            }
        }

        [DllImport("gdi32.dll")]
        static extern int GetDeviceCaps(IntPtr hdc, int nIndex);
        public enum DeviceCap
        {
            VERTRES = 10,
            DESKTOPVERTRES = 117,

            // http://pinvoke.net/default.aspx/gdi32/GetDeviceCaps.html
        }


        public static float GetScalingFactor()
        {
            Graphics g = Graphics.FromHwnd(IntPtr.Zero);
            IntPtr desktop = g.GetHdc();
            int LogicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.VERTRES);
            int PhysicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.DESKTOPVERTRES);

            float ScreenScalingFactor = (float)PhysicalScreenHeight / (float)LogicalScreenHeight;

            g.ReleaseHdc();

            return ScreenScalingFactor; // 1.25 = 125%
        }
    }
}
