using System;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;

// 29.03.2016
// For run. .\MultiDesktop " Display_Number Width Height Command"  ["Display_Number Width Height Command"] ..

namespace MultiDesktop
{
    class Program
    {
        static void StartProcess(string param)
        {
            Process p = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();

            int toDisplay = 0;
            int height = Screen.AllScreens[toDisplay].Bounds.Height;
            int width = Screen.AllScreens[toDisplay].Bounds.Width;

            Console.WriteLine("Param: " + param);
            param = param.Trim();
            Console.WriteLine("Param after trim: " + param);

            String[] args = param.Split(' ');

            foreach (String arg in args)
            {
                Console.WriteLine(args.Length + " - " + arg);
            }

            if (args.Length == 4)
            {
                if (!Int32.TryParse(args[0], out toDisplay))
                {
                    Console.WriteLine("Error parse dusplay: " + args[0]);
                }
                else
                    --toDisplay;
                if (!Int32.TryParse(args[1], out width))
                {
                    Console.WriteLine("Errror parse: " + args[1]);
                }
                if (width == -1)
                    width = Screen.AllScreens[toDisplay].Bounds.Width;
                if (!Int32.TryParse(args[2], out height))
                {
                    Console.WriteLine("Errror parse: " + args[2]);
                }
                if (height == -1)
                    height = Screen.AllScreens[toDisplay].Bounds.Height;
                startInfo.FileName = args[3];

            }
            else
            {
                Console.WriteLine("Arguments not correct: .\\MultiDesktop.exe \"Display_Number Width Height Command\"");
                return;
            }
            Console.WriteLine(toDisplay + " " + width + " " + height);

            p.StartInfo = startInfo;

            ThreadStart ths = new ThreadStart(() =>
            {
                p.Start();

                while (p.MainWindowHandle == (IntPtr)0) { }
                Console.WriteLine("Windows Handle: " + p.MainWindowHandle);
                    Program.SetWindowPos(p.MainWindowHandle, IntPtr.Zero, Screen.AllScreens[toDisplay].WorkingArea.Left, Screen.AllScreens[toDisplay].WorkingArea.Top,
                     width, height, SetWindowPosFlags.SWP_ZERO);
                });
            Thread th = new Thread(ths);
            th.Start();
        }

        static void Main(string[] args)
        {
            int maxScreen = 0;

            Console.WriteLine("Displays: " + Screen.AllScreens.Length);
            // List<string> handleList = new List<string>(); 
            foreach (var screen in Screen.AllScreens)
            {
                // For each screen, add the screen properties to a list box.
                Console.WriteLine("Device Name: " + screen.DeviceName);
                Console.WriteLine("Bounds: " + screen.Bounds.ToString());
                Console.WriteLine("Type: " + screen.GetType().ToString());
                //screen.
                Console.WriteLine("Working Area: " + screen.WorkingArea.ToString());
                Console.WriteLine("Primary Screen: " + screen.Primary.ToString());
                maxScreen++;
            }

            if (args.Length < 1 )
            {
                Console.WriteLine("Not arguments: " + args.Length + "(" + maxScreen + ")");
                return;
            }
            if (args.Length > maxScreen)
            {
                Console.WriteLine("Many arguments!");
                return;
            }

            foreach  (String arg in args)
            {
                StartProcess(arg);
            }
        }

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, SetWindowPosFlags uFlags);

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [Flags]
        public enum SetWindowPosFlags : uint
        {
            // ReSharper disable InconsistentNaming

            /// <summary>
            ///     If the calling thread and the thread that owns the window are attached to different input queues, the system posts the request to the thread that owns the window. This prevents the calling thread from blocking its execution while other threads process the request.
            /// </summary>
            SWP_ASYNCWINDOWPOS = 0x4000,

            /// <summary>
            ///     Prevents generation of the WM_SYNCPAINT message.
            /// </summary>
            SWP_DEFERERASE = 0x2000,

            /// <summary>
            ///     Draws a frame (defined in the window's class description) around the window.
            /// </summary>
            SWP_DRAWFRAME = 0x0020,

            /// <summary>
            ///     Applies new frame styles set using the SetWindowLong function. Sends a WM_NCCALCSIZE message to the window, even if the window's size is not being changed. If this flag is not specified, WM_NCCALCSIZE is sent only when the window's size is being changed.
            /// </summary>
            SWP_FRAMECHANGED = 0x0020,

            /// <summary>
            ///     Hides the window.
            /// </summary>
            SWP_HIDEWINDOW = 0x0080,

            /// <summary>
            ///     Does not activate the window. If this flag is not set, the window is activated and moved to the top of either the topmost or non-topmost group (depending on the setting of the hWndInsertAfter parameter).
            /// </summary>
            SWP_NOACTIVATE = 0x0010,

            /// <summary>
            ///     Discards the entire contents of the client area. If this flag is not specified, the valid contents of the client area are saved and copied back into the client area after the window is sized or repositioned.
            /// </summary>
            SWP_NOCOPYBITS = 0x0100,

            /// <summary>
            ///     Retains the current position (ignores X and Y parameters).
            /// </summary>
            SWP_NOMOVE = 0x0002,

            /// <summary>
            ///     Does not change the owner window's position in the Z order.
            /// </summary>
            SWP_NOOWNERZORDER = 0x0200,

            /// <summary>
            ///     Does not redraw changes. If this flag is set, no repainting of any kind occurs. This applies to the client area, the nonclient area (including the title bar and scroll bars), and any part of the parent window uncovered as a result of the window being moved. When this flag is set, the application must explicitly invalidate or redraw any parts of the window and parent window that need redrawing.
            /// </summary>
            SWP_NOREDRAW = 0x0008,

            /// <summary>
            ///     Same as the SWP_NOOWNERZORDER flag.
            /// </summary>
            SWP_NOREPOSITION = 0x0200,

            /// <summary>
            ///     Prevents the window from receiving the WM_WINDOWPOSCHANGING message.
            /// </summary>
            SWP_NOSENDCHANGING = 0x0400,

            /// <summary>
            ///     Retains the current size (ignores the cx and cy parameters).
            /// </summary>
            SWP_NOSIZE = 0x0001,

            /// <summary>
            ///     Retains the current Z order (ignores the hWndInsertAfter parameter).
            /// </summary>
            SWP_NOZORDER = 0x0004,

            /// <summary>
            ///     Displays the window.
            /// </summary>
            SWP_SHOWWINDOW = 0x0040,

            // ReSharper restore InconsistentNaming

            SWP_ZERO = 0x0
        }
    }
}
