using Projet1BaseDuCsharpGrp5;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

public static class Dialogue
{
    public static void Show(Pnj speaker, string text)
    {
        var map = World.CurrentMap;
        int pw = Map.PixelWidth;
        int winW = Console.WindowWidth;
        int winH = Console.WindowHeight;

        // Box fixed size
        const int boxW = 35;
        const int boxH = 7;
        int startX = (winW - boxW) / 2;
        int startY = winH - boxH - 1;
        int endX = startX + boxW - 1;
        int endY = startY + boxH - 1;

        // Snapshot tiles under box ±1 cell
        int snapLeft = startX - pw;
        int snapRight = endX + pw;
        var backup = new List<(int gx, int gy, Tile tile)>();
        for (int gx = 0; gx < map.Width; gx++)
            for (int gy = 0; gy < map.Height; gy++)
                if (Map.GridToScreen(gx, gy, out int sx, out int sy)
                    && sx >= snapLeft && sx + pw - 1 <= snapRight
                    && sy >= startY && sy <= endY)
                    backup.Add((gx, gy, map.Tiles[gx, gy]));

        // Entities hidden by box
        var toRedraw = new List<Action>();
        if (Map.GridToScreen(World.player.X, World.player.Y, out int px, out int py)
            && px >= snapLeft && px <= snapRight && py >= startY && py <= endY)
            toRedraw.Add(() => World.player.Draw());
        foreach (var d in map.Dresseurs)
            if (Map.GridToScreen(d.X, d.Y, out int dx, out int dy)
                && dx >= snapLeft && dx <= snapRight && dy >= startY && dy <= endY)
                toRedraw.Add(() => d.Draw());

        // Prepare wrapped text
        int textW = boxW - 4;
        var words = text.Split(' ');
        var wrapped = new List<string>();
        var lineBuf = "";
        foreach (var w in words)
        {
            if (w.Length > textW)
            {
                if (lineBuf.Length > 0) { wrapped.Add(lineBuf); lineBuf = ""; }
                for (int i = 0; i < w.Length; i += textW)
                    wrapped.Add(w.Substring(i, Math.Min(textW, w.Length - i)));
            }
            else
            {
                if (lineBuf.Length + 1 + w.Length <= textW)
                    lineBuf = lineBuf.Length == 0 ? w : lineBuf + " " + w;
                else
                {
                    wrapped.Add(lineBuf);
                    lineBuf = w;
                }
            }
        }
        if (lineBuf.Length > 0) wrapped.Add(lineBuf);

        // Paginate lines into pages of 7 lines
        const int pageLines = boxH - 2;
        var pages = new List<List<string>>();
        for (int i = 0; i < wrapped.Count; i += pageLines)
            pages.Add(wrapped.Skip(i).Take(pageLines).ToList());

        // Draw static header and body background
        DrawHeader();
        FillBody();

        // Display each page
        for (int pi = 0; pi < pages.Count; pi++)
        {
            var page = pages[pi];
            bool skip = false;

            // Clear body if not first page
            if (pi > 0) FillBody();

            // Typewriter effect
            for (int r = 0; r < page.Count; r++)
            {
                var line = page[r];
                for (int c = 0; c < line.Length; c++)
                {
                    Console.SetCursorPosition(startX + 2 + c, startY + 1 + r);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(line[c]);
                    Console.ResetColor();

                    if (!skip)
                    {
                        Thread.Sleep(30);
                        if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.E)
                            skip = true;
                    }
                }
                if (skip) break;
            }

            // Reveal full page if skipped
            if (skip)
            {
                for (int r = 0; r < page.Count; r++)
                {
                    Console.SetCursorPosition(startX + 2, startY + 1 + r);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(page[r].PadRight(textW));
                    Console.ResetColor();
                }
            }

            if (pi < pages.Count - 1)
                BlinkArrow();
            else
                ShowPromptAndWait();
        }

        // Erase box and restore map area and entities
        EraseBox();
        RestoreBackup();
        foreach (var draw in toRedraw) draw();

        // Helpers

        void DrawHeader()
        {
            for (int dx = 0; dx < boxW; dx++)
            {
                Console.SetCursorPosition(startX + dx, startY);
                Ansi.WriteBg(speaker.Color);
                Console.Write(' ');
                Ansi.Reset();
            }
            var name = speaker.Name.ToUpper();
            Console.ForegroundColor = ConsoleColor.White;
            Ansi.WriteBg(speaker.Color);
            Console.SetCursorPosition(startX + (boxW - name.Length) / 2, startY);
            Console.Write(name);
            Console.ResetColor();
        }

        void FillBody()
        {
            for (int dy = 1; dy < boxH; dy++)
            {
                Console.SetCursorPosition(startX, startY + dy);
                Ansi.WriteBg(new Rgb(0, 0, 0));
                Console.Write(new string(' ', boxW));
                Ansi.Reset();
            }
        }

        void BlinkArrow()
        {
            int ax = startX + boxW - 2;
            int ay = startY + boxH - 1;
            bool on = true;
            while (true)
            {
                if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.E)
                {
                    FillBody();
                    break;
                }
                Console.SetCursorPosition(ax, ay);
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(on ? '↓' : ' ');
                Console.ResetColor();
                on = !on;
                Thread.Sleep(400);
            }
        }

        void ShowPromptAndWait()
        {
            var prompt = "[E] continuer";
            Console.SetCursorPosition(
                startX + boxW - prompt.Length - 2,
                startY + boxH - 1);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(prompt);
            Console.ResetColor();
            while (Console.ReadKey(true).Key != ConsoleKey.E) { }
        }

        void EraseBox()
        {
            for (int dy = 0; dy < boxH; dy++)
            {
                Console.SetCursorPosition(startX, startY + dy);
                Console.Write(new string(' ', boxW));
            }
        }

        void RestoreBackup()
        {
            foreach (var (gx, gy, tile) in backup)
            {
                Map.GridToScreen(gx, gy, out int sx, out int sy);
                Ansi.WriteBg(tile.Color);
                Console.SetCursorPosition(sx, sy);
                Console.Write(new string(' ', pw));
                Ansi.Reset();
            }
        }
    }
}