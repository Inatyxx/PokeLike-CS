using System;

namespace Projet1BaseDuCsharpGrp5
{
    public static class UI
    {
        public static void DrawTile(int gx, int gy, Rgb color)
        {
            // ne dessine que si la grille convertit en écran
            if (!Map.GridToScreen(gx, gy, out int sx, out int sy))
                return;
            Ansi.WriteBg(color);
            Console.SetCursorPosition(sx, sy);
            Console.Write(new string(' ', Map.PixelWidth));
            Ansi.Reset();
        }
        public static void FillRect(int gx, int gy, int wTiles, int hTiles, Rgb color)
        {
            for (int y = gy; y < gy + hTiles; y++)
                for (int x = gx; x < gx + wTiles; x++)
                    DrawTile(x, y, color);
        }
        public static void DrawBorder(int gx, int gy, int wTiles, int hTiles, Rgb color)
        {
            // Haut et bas
            for (int x = gx; x < gx + wTiles; x++)
            {
                DrawTile(x, gy, color);
                DrawTile(x, gy + hTiles - 1, color);
            }
            // Gauche et droite
            for (int y = gy; y < gy + hTiles; y++)
            {
                DrawTile(gx, y, color);
                DrawTile(gx + wTiles - 1, y, color);
            }
        }
        public static void DrawText(
        int gx,
        int gy,
        int wTiles,
        string text,
        Rgb fg,
        Rgb bg)
        {
            if (!Map.GridToScreen(gx, gy, out int sx, out int sy))
                return;

            int totalPx = wTiles * Map.PixelWidth;

            Ansi.WriteBg(bg);
            Console.SetCursorPosition(sx, sy);
            Console.Write(new string(' ', totalPx));

            int tx = sx + (totalPx - text.Length) / 2;
            Console.SetCursorPosition(tx, sy);

            Ansi.WriteFg(fg);
            Console.Write(text);

            Console.ResetColor();
        }

        public static void DrawButton(int gx, int gy, int wTiles, string label, bool selected)
        {
            // Fond
            var bg = selected
                ? new Rgb(40, 40, 40)
                : new Rgb(10, 10, 10);
            FillRect(gx, gy, wTiles, 1, bg);

            // Bordure si sélectionné
            if (selected)
                DrawBorder(gx, gy, wTiles, 1, new Rgb(5, 5, 5));

            // Label centré
            DrawText(gx, gy, wTiles, label, selected ? new Rgb(200, 200, 200) : new Rgb(200, 200, 200), new Rgb(200, 000, 200));
        }

        public static void DrawSprite(PokeSprite sprite, int gx, int gy)
        {
            for (int row = 0; row < PokeSprite.Height; row++)
            {
                for (int col = 0; col < PokeSprite.Width; col++)
                {
                    var tile = sprite.Pixels[row, col];          
                    if (tile.IsTransparent) continue;

                    if (!Map.GridToScreen(gx + col, gy + row, out int sx, out int sy))
                        continue;

                    Console.SetCursorPosition(sx, sy);
                    Ansi.WriteBg(tile.Color);
                    Console.Write(new string(' ', Map.PixelWidth));
                    Ansi.Reset();
                }
            }
        }
        public static void DrawReverseSprite(PokeSprite sprite, int gx, int gy)
        {
            for (int row = 0; row < PokeSprite.Height; row++)
            {
                for (int col = 0; col < PokeSprite.Width; col++)
                {
                    // on inverse seulement la colonne : col' = Width - 1 - col
                    var tile = sprite.Pixels[row, PokeSprite.Width - 1 - col];
                    if (tile.IsTransparent)
                        continue;

                    if (!Map.GridToScreen(gx + col, gy + row, out int sx, out int sy))
                        continue;

                    Console.SetCursorPosition(sx, sy);
                    Ansi.WriteBg(tile.Color);
                    Console.Write(new string(' ', Map.PixelWidth));
                    Ansi.Reset();
                }
            }
        }

        // Dessine un PokeSprite échelle uniforme scale×scale
        public static void DrawSpriteScaled(PokeSprite sprite, int gx, int gy, int scale)
        {
            for (int row = 0; row < PokeSprite.Height; row++)
            {
                for (int col = 0; col < PokeSprite.Width; col++)
                {
                    var tile = sprite.Pixels[row, col];
                    if (tile.IsTransparent) continue;

                    // Pour chaque Pixel -> un bloc scale×scale
                    FillRect(
                        gx + col * scale,
                        gy + row * scale,
                        scale,
                        scale,
                        tile.Color
                    );
                }
            }
        }

        // Même principe, en miroir horizontal
        public static void DrawReverseSpriteScaled(PokeSprite sprite, int gx, int gy, int scale)
        {
            for (int row = 0; row < PokeSprite.Height; row++)
            {
                for (int col = 0; col < PokeSprite.Width; col++)
                {
                    var tile = sprite.Pixels[row, PokeSprite.Width - 1 - col];
                    if (tile.IsTransparent) continue;

                    FillRect(
                        gx + col * scale,
                        gy + row * scale,
                        scale,
                        scale,
                        tile.Color
                    );
                }
            }
        }
    }
}