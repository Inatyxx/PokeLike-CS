using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Projet1BaseDuCsharpGrp5;

namespace Projet1BaseDuCsharpGrp5
{
    // Active le mode VT100 et fournit des utilitaires d’écriture ANSI RGB
    internal static class Ansi
    {
        private const int STD_OUTPUT_HANDLE = -11;
        private const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll")]
        private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        static Ansi()
        {
            var handle = GetStdHandle(STD_OUTPUT_HANDLE);
            GetConsoleMode(handle, out uint mode);
            SetConsoleMode(handle, mode | ENABLE_VIRTUAL_TERMINAL_PROCESSING);
        }

        public static void WriteFg(Rgb c) => Console.Write($"\x1b[38;2;{c.R};{c.G};{c.B}m");
        public static void WriteBg(Rgb c) => Console.Write($"\x1b[48;2;{c.R};{c.G};{c.B}m");
        public static void Reset() => Console.Write("\x1b[0m");
    }
}