using System;
using System.Collections.Generic;

namespace Projet1BaseDuCsharpGrp5;

public interface IMenu
{
    void Run();
}

public interface IOverlayMenu
{
    int Width { get; }
    int Height { get; }

    void Run(int startX, int startY);
}

public static class MenuManager
{
    public static void OpenFullScreen(IMenu menu)
    {
        Console.Clear();
        menu.Run();
        Console.Clear();
        RedrawMap();
    }

    public static void OpenOverlay(IOverlayMenu overlay, int startX, int startY)
    {
        var map = World.CurrentMap;
        int pw = Map.PixelWidth;

        // zone en tuiles (bordure incluse)
        int gx0 = startX - 1;
        int gy0 = startY - 1;
        int gx1 = startX + overlay.Width;
        int gy1 = startY + overlay.Height;

        // 1) backup des tuiles existantes + cases hors-map
        var backup = new List<(int gx, int gy, Tile tile)>();
        var outOfGrid = new List<(int gx, int gy)>();
        for (int gx = gx0; gx <= gx1; gx++)
        {
            for (int gy = gy0; gy <= gy1; gy++)
            {
                if (gx >= 0 && gy >= 0 &&
                    gx < map.Width && gy < map.Height)
                {
                    backup.Add((gx, gy, map.Tiles[gx, gy]));
                }
                else
                {
                    outOfGrid.Add((gx, gy));
                }
            }
        }

        overlay.Run(startX, startY);

        // 3) clear des blocs hors-map
        foreach (var (gx, gy) in outOfGrid)
        {
            if (Map.GridToScreen(gx, gy, out int sx, out int sy))
            {
                Console.SetCursorPosition(sx, sy);
                Console.Write(new string(' ', pw));
            }
        }

        // 4) restore du fond des tuiles sauvegardées
        foreach (var (gx, gy, tile) in backup)
        {
            Map.GridToScreen(gx, gy, out int sx, out int sy);
            Ansi.WriteBg(tile.Color);
            Console.SetCursorPosition(sx, sy);
            Console.Write(new string(' ', pw));
            Ansi.Reset();
        }

        foreach (var d in map.Dresseurs)
        {
            if (d.X >= gx0 && d.X <= gx1 &&
                d.Y >= gy0 && d.Y <= gy1)
            {
                d.Draw();
            }
        }

        var p = World.player;
        if (p.X >= gx0 && p.X <= gx1 &&
            p.Y >= gy0 && p.Y <= gy1)
        {
            p.Draw();
        }
    }


    public static void OpenOverlayCentered(IOverlayMenu overlay)
    {
        int gx = (World.Width - overlay.Width) / 2;
        int gy = (World.Height - overlay.Height) / 2;
        gx = Math.Max(0, gx);
        gy = Math.Max(0, gy);

        OpenOverlay(overlay, gx, gy);
    }

    private static void RedrawMap()
    {
        World.CurrentMap.Render();
        foreach (var d in World.CurrentMap.Dresseurs)
            d.Draw();
        World.player.Draw();
    }
}