using System;

namespace Projet1BaseDuCsharpGrp5
{
    public static class Decors
    {
        // ─────────── Couleurs RGB de base ───────────
        public static readonly Rgb Black = new Rgb(0, 0, 0);
        public static readonly Rgb White = new Rgb(255, 255, 255);

        public static readonly Rgb Red = new Rgb(255, 0, 0);
        public static readonly Rgb Green = new Rgb(0, 255, 0);
        public static readonly Rgb Blue = new Rgb(0, 0, 255);
        public static readonly Rgb Yellow = new Rgb(255, 255, 0);
        public static readonly Rgb Magenta = new Rgb(255, 0, 255);
        public static readonly Rgb Cyan = new Rgb(0, 255, 255);

        public static readonly Rgb Orange = new Rgb(255, 165, 0);
        public static readonly Rgb Brown = new Rgb(139, 69, 19);
        public static readonly Rgb Gray = new Rgb(128, 128, 128);
        public static readonly Rgb DarkGray = new Rgb(64, 64, 64);

        // ─────────── Light / Dark Variantes ───────────

        public static readonly Rgb LightRed = new Rgb(255, 102, 102);
        public static readonly Rgb DarkRed = new Rgb(128, 0, 0);

        public static readonly Rgb LightGreen = new Rgb(102, 255, 102);
        public static readonly Rgb DarkGreen = new Rgb(0, 128, 0);

        public static readonly Rgb LightBlue = new Rgb(102, 102, 255);
        public static readonly Rgb DarkBlue = new Rgb(0, 0, 128);

        public static readonly Rgb LightYellow = new Rgb(255, 255, 153);
        public static readonly Rgb DarkYellow = new Rgb(128, 128, 0);

        public static readonly Rgb LightMagenta = new Rgb(255, 102, 255);
        public static readonly Rgb DarkMagenta = new Rgb(128, 0, 128);

        public static readonly Rgb LightCyan = new Rgb(102, 255, 255);
        public static readonly Rgb DarkCyan = new Rgb(0, 128, 128);

        public static readonly Rgb LightOrange = new Rgb(255, 200, 102);
        public static readonly Rgb DarkOrange = new Rgb(204, 82, 0);

        public static readonly Rgb LightBrown = new Rgb(205, 133, 63);
        public static readonly Rgb DarkBrown = new Rgb(92, 46, 0);

        public static readonly Rgb LightGray = new Rgb(192, 192, 192);
        public static readonly Rgb DarkGray2 = new Rgb(96, 96, 96);

        public static readonly Tile Transparent = new Tile(color: new Rgb(0, 0, 0),isTransparent: true);
        // Dans ta classe de sprites ou ton helper
        private static Tile T(Rgb color, bool isWall = false, bool isGrass = false)
        {
            return new Tile(color, isWall: isWall, isGrass: isGrass);
        }

        public static readonly Tile[,] LightTree = new Tile[,]
        {
            { T(LightGreen, isWall: true),   T(LightGreen, isWall: true)  , T(LightGreen, isWall: true) },
            { T(LightGreen, isWall: true),   T(LightGreen, isWall: true)  , T(LightGreen, isWall: true) },
            { T(LightGreen, isWall: true),   T(Brown,      isWall: true)  , T(LightGreen, isWall: true) },
            { T(LightGreen, isWall: true),   T(Brown,      isWall: true)  , T(LightGreen, isWall: true) },
            { Transparent                ,   T(Brown,      isWall: true)  , Transparent },
        };


        public static readonly Tile[,] Tree = new Tile[,]
        {
            { T(Green,    isWall: true), T(Green,    isWall: true), T(Green,    isWall: true) },
            { T(Green,    isWall: true), T(Green,    isWall: true), T(Green,    isWall: true) },
            { T(Green,    isWall: true), T(Brown,    isWall: true), T(Green,    isWall: true) },
            { T(Green,    isWall: true), T(Brown,    isWall: true), T(Green,    isWall: true) },
            { Transparent,               T(Brown,    isWall: true), Transparent                }
        };

        public static readonly Tile[,] DarkTree = new Tile[,]
        {
            { T(DarkGreen, isWall: true), T(DarkGreen, isWall: true), T(DarkGreen, isWall: true) },
            { T(DarkGreen, isWall: true), T(DarkGreen, isWall: true), T(DarkGreen, isWall: true) },
            { T(DarkGreen, isWall: true), T(Brown,     isWall: true), T(DarkGreen, isWall: true) },
            { T(DarkGreen, isWall: true), T(Brown, isWall: true), T(DarkGreen, isWall: true) },
            { Transparent,                T(Brown,     isWall: true), Transparent                   }
        };


        public static readonly Tile[,] TallGrass = new Tile[,]
        {
            {new Tile(new Rgb(102, 177, 11), isGrass:true), new Tile(new Rgb(66, 139, 16), isGrass:true), new Tile(new Rgb(102, 177, 11), isGrass:true)},
            {new Tile(new Rgb(66, 139, 16), isGrass:true), new Tile(new Rgb(102, 177, 11), isGrass:true), new Tile(new Rgb(66, 139, 16), isGrass:true)},
            {new Tile(new Rgb(102, 177, 11), isGrass:true), new Tile(new Rgb(66, 139, 16), isGrass:true), new Tile(new Rgb(102, 177, 11), isGrass:true)},
        };

        public static readonly Tile[,] ForestPath3 = new Tile[,]
        {
            {new Tile(new Rgb(217, 246, 102))}
        };

        public static readonly Tile[,] TownPath = new Tile[,]
        {
            {new Tile(new Rgb(169, 134, 109))}
        };
        public static readonly Tile[,] Sand = new Tile[,]
        {
            {new Tile(new Rgb(247, 201, 130))}
        };

        public static readonly Tile[,] SmallHouse = new Tile[,]
        {
            { Transparent, T(DarkRed,isWall: true), T(DarkRed,isWall: true), T(DarkRed,isWall: true), T(DarkRed,isWall: true), T(DarkRed,isWall: true), Transparent },
            { T(DarkRed,isWall: true), T(DarkRed,isWall: true), T(DarkRed,isWall: true), T(DarkRed,isWall: true), T(DarkRed,isWall: true), T(DarkRed,isWall: true), T(DarkRed,isWall: true) },
            { T(DarkRed,isWall: true), T(DarkRed,isWall: true), T(DarkRed,isWall: true), T(DarkRed,isWall: true), T(DarkRed,isWall: true), T(DarkRed,isWall: true), T(DarkRed,isWall: true) },
            { T(Gray, isWall: true), T(Gray, isWall: true), T(Gray, isWall: true), T(Gray, isWall: true), T(Gray, isWall: true), T(Gray, isWall: true), T(Gray, isWall: true) },
            { T(Gray, isWall: true), T(Gray, isWall: true), T(Gray, isWall: true), T(Brown, isWall: true), T(Gray, isWall: true), T(Cyan, isWall: true), T(Gray, isWall: true) },
            { T(Gray, isWall: true), T(Gray, isWall: true), T(Gray, isWall: true), T(Brown, isWall: false), T(Gray, isWall: true), T(Gray, isWall: true), T(Gray, isWall: true) }
        };

        public static readonly Tile[,] Laboratory = new Tile[,]
        {
            { Transparent, Transparent, T(LightGray,isWall: true), T(LightGray,isWall: true), Transparent, Transparent, Transparent, Transparent },
            { Transparent, Transparent, T(Gray,isWall: true), T(Gray,isWall: true), Transparent, Transparent, Transparent, Transparent },
            { T(LightGray,isWall: true), T(LightGray,isWall: true), T(LightGray,isWall: true), T(LightGray,isWall: true), T(LightGray,isWall: true), T(LightGray,isWall: true), T(LightGray,isWall: true), T(LightGray,isWall: true) },
            { T(LightGray,isWall: true), T(LightGray,isWall: true), T(LightGray,isWall: true), T(LightGray,isWall: true), T(LightGray,isWall: true), T(LightGray,isWall: true), T(LightGray,isWall: true), T(LightGray,isWall: true) },
            { T(Gray, isWall: true), T(Gray, isWall: true), T(Gray, isWall: true), T(Gray, isWall: true), T(Gray, isWall: true), T(Gray, isWall: true), T(Gray, isWall: true), T(Gray, isWall: true) },
            { T(Gray, isWall: true), T(LightGray, isWall: true), T(LightGray, isWall: true), T(Gray, isWall: true), T(White, isWall: true), T(Gray, isWall: true), T(LightGray, isWall: true), T(Gray, isWall: true) },
            { T(Gray, isWall: true), T(Gray, isWall: true), T(Gray, isWall: true), T(Gray, isWall: true), T(White, isWall: false), T(Gray, isWall: true), T(Gray, isWall: true), T(Gray, isWall: true) }
        };

        public static readonly Tile[,] HealPost = new Tile[,]
        {
            { Transparent, Transparent, T(Red, isWall:true), T(Red, isWall:true), T(Red, isWall:true), T(Red, isWall:true), T(Red, isWall:true), Transparent, Transparent },
            { T(Red, isWall:true), T(Red, isWall:true), T(Red, isWall:true), T(White, isWall:true), T(White, isWall:true), T(White, isWall:true), T(Red, isWall:true), T(Red, isWall:true), T(Red, isWall:true) },
            { T(White, isWall:true), T(White, isWall:true), T(White, isWall:true), T(White, isWall:true), T(Red, isWall:true), T(White, isWall:true), T(White, isWall:true), T(White, isWall:true), T(White, isWall:true) },
            { T(Red, isWall:true), T(Red, isWall:true), T(Red, isWall:true), T(White, isWall:true), T(White, isWall:true), T(White, isWall:true), T(Red, isWall:true), T(Red, isWall:true), T(Red, isWall:true) },
            { T(DarkRed, isWall:true), T(DarkRed, isWall:true), T(DarkRed, isWall:true), T(DarkRed, isWall:true), T(DarkRed, isWall:true), T(DarkRed, isWall:true), T(DarkRed, isWall:true), T(DarkRed, isWall:true), T(DarkRed, isWall:true) },
            { T(DarkRed, isWall:true), T(Cyan, isWall:true), T(DarkRed, isWall:true), T(DarkRed, isWall:true), T(DarkRed, isWall:true), T(DarkRed, isWall:true), T(DarkRed, isWall:true), T(Cyan, isWall:true), T(DarkRed, isWall:true) },
            { T(DarkRed, isWall:true), T(Cyan, isWall:true), T(DarkRed, isWall:true), T(DarkRed, isWall:true), T(White, isWall:true), T(DarkRed, isWall:true), T(DarkRed, isWall:true), T(Cyan, isWall:true), T(DarkRed, isWall:true) },
            { T(DarkRed, isWall:true), T(DarkRed, isWall:true), T(DarkRed, isWall:true), T(DarkRed, isWall:true), T(White, isWall:false), T(DarkRed, isWall:true), T(DarkRed, isWall:true), T(DarkRed, isWall:true), T(DarkRed, isWall:true) },
        };

        public static readonly Tile[,] ARENE1 = new Tile[,]
        {
            { Transparent, Transparent, T(DarkGreen, isWall:true), T(DarkGreen, isWall:true), T(DarkGreen, isWall:true), T(DarkGreen, isWall:true), T(DarkGreen, isWall:true), Transparent, Transparent },
            { T(DarkGreen, isWall:true), T(DarkGreen, isWall:true), T(DarkGreen, isWall:true), T(White, isWall:true), T(White, isWall:true), T(White, isWall:true), T(DarkGreen, isWall:true), T(DarkGreen, isWall:true), T(DarkGreen, isWall:true) },
            { T(White, isWall:true), T(White, isWall:true), T(White, isWall:true), T(White, isWall:true), T(DarkGreen, isWall:true), T(White, isWall:true), T(White, isWall:true), T(White, isWall:true), T(White, isWall:true) },
            { T(DarkGreen, isWall:true), T(DarkGreen, isWall:true), T(DarkGreen, isWall:true), T(White, isWall:true), T(White, isWall:true), T(White, isWall:true), T(DarkGreen, isWall:true), T(DarkGreen, isWall:true), T(DarkGreen, isWall:true) },
            { T(Gray, isWall:true), T(Gray, isWall:true), T(Gray, isWall:true), T(Gray, isWall:true), T(Gray, isWall:true), T(Gray, isWall:true), T(Gray, isWall:true), T(Gray, isWall:true), T(Gray, isWall:true) },
            { T(Gray, isWall:true), T(Cyan, isWall:true), T(Gray, isWall:true), T(Gray, isWall:true), T(Gray, isWall:true), T(Gray, isWall:true), T(Gray, isWall:true), T(Cyan, isWall:true), T(Gray, isWall:true) },
            { T(Gray, isWall:true), T(Cyan, isWall:true), T(Gray, isWall:true), T(Gray, isWall:true), T(White, isWall:true), T(Gray, isWall:true), T(Gray, isWall:true), T(Cyan, isWall:true), T(Gray, isWall:true) },
            { T(Gray, isWall:true), T(Gray, isWall:true), T(Gray, isWall:true), T(Gray, isWall:true), T(White, isWall:false), T(Gray, isWall:true), T(Gray, isWall:true), T(Gray, isWall:true), T(Gray, isWall:true) },
        };
        public static readonly Tile[,] Grotte = new Tile[,]
        {
            { Transparent, Transparent, Transparent, T(Gray, isWall:true), Transparent, Transparent, Transparent, Transparent, Transparent },
            { Transparent, Transparent, Transparent, T(Gray, isWall:true), T(Gray, isWall:true), T(Gray, isWall:true), Transparent, Transparent, Transparent },
            { Transparent, Transparent, Transparent, T(DarkGray2, isWall:true), T(Gray, isWall:true), T(Gray, isWall:true), Transparent, Transparent, Transparent },
            { Transparent, Transparent, T(DarkGray, isWall:true), T(DarkGray, isWall:true), T(DarkGray, isWall:true), T(DarkGray2, isWall:true), T(DarkGray2, isWall:true), Transparent, Transparent },
            { Transparent, T(DarkGray, isWall:true), T(DarkGray, isWall:true), T(DarkGray, isWall:false), T(Black, isWall:false), T(DarkGray, isWall:true), T(DarkGray, isWall:true), T(DarkGray2, isWall:true), Transparent },
        };

        public static readonly Tile[,] Rock = new Tile[,]
        {
            { Transparent, T(Gray, isWall:true), Transparent},
            { T(DarkGray, isWall:true), T(Gray, isWall:true), T(Gray, isWall:true)},
        };
        public static readonly Tile[,] Door = new Tile[,]
        {
            { new Tile(new Rgb(0, 255,  0)) }
        };
    }
}