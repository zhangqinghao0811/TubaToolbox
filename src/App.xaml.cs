using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;

namespace TubaToolbox
{
    public partial class App : Application
    {
        private static Mutex? _mutex;

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string? lpClassName, string lpWindowName);

        private const int SW_RESTORE = 9;

        protected override void OnStartup(StartupEventArgs e)
        {
            _mutex = new Mutex(true, "TubaToolbox_SingleInstance", out bool createdNew);

            if (!createdNew)
            {
                ActivateExistingWindow();
                Shutdown();
                return;
            }

            base.OnStartup(e);

            try
            {
                string iconPath = Path.Combine(AppContext.BaseDirectory, "app.ico");
                IconGenerator.GenerateIcon(iconPath);
            }
            catch { }
        }

        private static void ActivateExistingWindow()
        {
            try
            {
                IntPtr hWnd = FindWindow(null, "Tuba Toolbox 硬件工具箱");
                if (hWnd != IntPtr.Zero)
                {
                    ShowWindow(hWnd, SW_RESTORE);
                    SetForegroundWindow(hWnd);
                }
            }
            catch { }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _mutex?.ReleaseMutex();
            _mutex?.Dispose();
            base.OnExit(e);
        }
    }
}
