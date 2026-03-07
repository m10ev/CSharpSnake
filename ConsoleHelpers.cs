using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace C_Snake
{
    public static class ConsoleHelpers
    {
        const int SW_MAXIMIZE = 3;

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        public static void MaximizeConsole()
        {
            var handle = GetConsoleWindow();
            ShowWindow(handle, SW_MAXIMIZE);
        }
    }
}
