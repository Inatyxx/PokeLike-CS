using System;
using System.Collections.Generic;

namespace Projet1BaseDuCsharpGrp5
{
    public class Map
    {
        public class Transition
        {
            public int X { get; }
            public int Y { get; }
            public int Width { get; }
            public int Height { get; }
            public string TargetMap { get; }
            public int SpawnX { get; }
            public int SpawnY { get; }

            public Transition(
                int x, int y,
                int width, int height,
                string targetMap,
                int spawnX, int spawnY)
            {
                X = x;
                Y = y;
                Width = width;
                Height = height;
                TargetMap = targetMap;
                SpawnX = spawnX;
                SpawnY = spawnY;
            }

            // Vérifie si la position (gx, gy) est dans la zone
            public bool Contains(int gx, int gy) =>
                gx >= X && gx < X + Width
             && gy >= Y && gy < Y + Height;
        }

        private readonly List<Transition> transitions = new();

        // Pour une transition 1×1 (ancienne méthode)
        public void AddTransition(
            int x, int y,
            string targetMap,
            int spawnX, int spawnY) =>
            AddTransitionZone(x, y, 1, 1, targetMap, spawnX, spawnY);

        // Nouvelle surcharge pour une zone rectangulaire
        public void AddTransitionZone(
            int x, int y,
            int width, int height,
            string targetMap,
            int spawnX, int spawnY)
        {
            transitions.Add(
                new Transition(x, y, width, height, targetMap, spawnX, spawnY)
            );
        }

        // Renvoie la première transition dont la zone contient (x,y)
        public bool TryGetTransition(int x, int y, out Transition transition)
        {
            transition = transitions.FirstOrDefault(t => t.Contains(x, y));
            return transition != null;
        }


        // largeur en caractères d'un "pixel" de tuile (2 charactères)
        public const int PixelWidth = 2;

        // offset global pour la map courante
        private static int offsetX;
        private static int offsetY;

        public string Name { get; }
        public int Width { get; }
        public int Height { get; }
        public Tile[,] Tiles { get; }
        public List<Dresseur> Dresseurs { get; } = new(); 
        public bool Centered { get; private set; }
        public string BiomeName { get; set; } = string.Empty;


        public Map(string name, int width, int height, string biomeName = "Campaign")
        {
            Name = name;
            Width = width;
            Height = height;
            Tiles = new Tile[width, height];
            BiomeName = biomeName;
        }

        public void Fill(Func<int, int, Tile> factory)
        {
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    Tiles[x, y] = factory(x, y);
        }

        public void AddDresseur(Dresseur d)
        {
            Dresseurs.Add(d);
        }
        public static void SetCentering(Map map, bool centered)
        {
            map.Centered = centered;
        }
        public static void UpdateOffsets()
        {
            var map = World.CurrentMap;
            if (map.Centered)
            {
                offsetX = (Console.WindowWidth - map.Width * PixelWidth) / 2;
                offsetY = (Console.WindowHeight - map.Height) / 2;
            }
            else
            {
                offsetX = 0;
                offsetY = 0;
            }
        }


        public static bool GridToScreen(int gx, int gy, out int sx, out int sy)
        {
            sx = offsetX + gx * PixelWidth;
            sy = offsetY + gy;
            return sx >= 0
                && sy >= 0
                && sx + PixelWidth <= Console.WindowWidth
                && sy < Console.WindowHeight;
        }

        public void Render()
        {
            UpdateOffsets();

            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                {
                    if (!GridToScreen(x, y, out int sx, out int sy))
                        continue;

                    var bg = Tiles[x, y].Color;
                    Console.SetCursorPosition(sx, sy);
                    Ansi.WriteBg(bg);
                    Console.Write(new string(' ', PixelWidth));
                    Ansi.Reset();
                }

            foreach (var d in Dresseurs)
                d.Draw();
        }
        public void AddDecor(Tile[,] decor, int startX, int startY)
        {
            int w = decor.GetLength(0), h = decor.GetLength(1);

            for (int dy = 0; dy < w; dy++)
                for (int dx = 0; dx < h; dx++)
                {
                    var t = decor[dy, dx];
                    if (t.IsTransparent)
                        continue;             // on ne touche pas la tile d’origine

                    int mx = startX + dx, my = startY + dy;
                    if (mx < 0 || my < 0 || mx >= Width || my >= Height)
                        continue;

                    Tiles[mx, my] = t;
                }
        }
        public void addPath(Tile[,] decor, int startX, int startY, int width, int height)
        {
            for (int dx = 0; dx < width; dx++)
                for (int dy = 0; dy < height; dy++)
                {
                    var t = decor[0, 0];

                    int mx = startX + dx, my = startY + dy;
                    if (mx < 0 || my < 0 || mx >= Width || my >= Height)
                        continue;

                    Tiles[mx, my] = t;
                }
        }
        public void AddDamier(Tile tileA, Tile tileB, int startX, int startY, int width, int height)
        {
            for (int dy = 0; dy < height; dy++)
            {
                for (int dx = 0; dx < width; dx++)
                {
                    // Choix de la tuile selon la parité
                    var tile = ((dx + dy) % 2 == 0) ? tileA : tileB;

                    // Optionnel : ignorer les tuiles transparentes
                    if (tile.IsTransparent)
                        continue;

                    int mx = startX + dx;
                    int my = startY + dy;

                    // Vérification des bornes de la map
                    if (mx < 0 || my < 0 || mx >= Width || my >= Height)
                        continue;

                    Tiles[mx, my] = tile;
                }
            }
        }

        public static Rgb GetColorCell(int gx, int gy)
        {
            // On récupère la map courante depuis World
            var map = World.CurrentMap;

            // Protection hors bornes
            if (gx < 0 || gy < 0 || gx >= map.Width || gy >= map.Height)
                return new Rgb(0, 0, 0);

            return map.Tiles[gx, gy].Color;
        }

    }
}