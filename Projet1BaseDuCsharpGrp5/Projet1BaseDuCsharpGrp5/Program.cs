using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Reflection;
using Projet1BaseDuCsharpGrp5;

class Program
{
    // 1) Maximiser la fenêtre console
    static class NativeMethods
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetConsoleWindow();
        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        public const int SW_MAXIMIZE = 3;
    }
    private static void ConfigureBaseConsole()
    {
        var handle = NativeMethods.GetConsoleWindow();
        NativeMethods.ShowWindow(handle, NativeMethods.SW_MAXIMIZE);

        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.CursorVisible = false;
        Console.Title = "POKEMON 2";
    }
    static void Main()
    {
        ConfigureDebugLog();
        ConfigureBaseConsole();

        RunGame();
    }

    static void RunGame()
    {
        var menu = new MainMenu();
        menu.Run();

        int neededWidth = World.Width * Map.PixelWidth;
        int neededHeight = World.Height;
        if (Console.BufferWidth < neededWidth || Console.BufferHeight < neededHeight)
        {
            Console.SetBufferSize(
                Math.Max(Console.BufferWidth, neededWidth),
                Math.Max(Console.BufferHeight, neededHeight)
            );
        }
        Console.Clear();
    }


    private static void ConfigureDebugLog()
    {
        var logPath = Path.Combine(Directory.GetCurrentDirectory(), "debug.log");
        File.WriteAllText(logPath, "");

        Trace.Listeners.Clear();
        Trace.Listeners.Add(new TextWriterTraceListener(logPath));
        Trace.AutoFlush = true;
        /*
        Process.Start(new ProcessStartInfo
        {
            FileName = "powershell.exe",
            Arguments = $"-NoExit -Command \"Get-Content -Path '{logPath}' -Wait\"",
            UseShellExecute = true
        });*/
    }
}
