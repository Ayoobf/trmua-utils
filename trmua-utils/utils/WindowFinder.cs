using System;
using System.Runtime.InteropServices;
using System.Text;

namespace trmua_utils.utils
{
    public class WindowFinder
    {
        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        public static IntPtr FindWindowByTitle(string title)
        {
            IntPtr foundWindow = IntPtr.Zero;

            EnumWindows((hWnd, lParam) =>
            {
                if (!IsWindowVisible(hWnd))
                    return true;

                StringBuilder sb = new StringBuilder(256);
                GetWindowText(hWnd, sb, 256);

                if (sb.ToString().Contains(title, StringComparison.OrdinalIgnoreCase))
                {
                    foundWindow = hWnd;
                    return false;
                }
                return true;
            }, IntPtr.Zero);

            return foundWindow;
        }
    }
}