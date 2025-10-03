using System;
using System.Collections.Generic;
using System.Drawing;
using Projet1BaseDuCsharpGrp5;
using System.IO;
using System.Text.Json;

namespace Projet1BaseDuCsharpGrp5;
using System;
using System.Reflection.Emit;
using Projet1BaseDuCsharpGrp5;

public class MainMenu : IMenu
{
    private const int ButtonWidth = 30;
    private const int ButtonHeight = 3;
    private const int ButtonSpacing = 2;
    private readonly string[] options = { "JOUER", "QUITTER" };
    private int selectedIndex = 0;
    private Rgb[,] art;
    private const int ArtW = 100;
    private const int ArtH = 20;

    // Lettres P O K E M O N en matrice 7×5
    private static readonly int[,] LetterP = new int[,]
    {
        {1,1,1,1,0},
        {1,0,0,0,1},
        {1,0,0,0,1},
        {1,1,1,1,0},
        {1,0,0,0,0},
        {1,0,0,0,0},
        {1,0,0,0,0},
    };
    private static readonly int[,] LetterO = new int[,]
    {
        {0,1,1,1,0},
        {1,0,0,0,1},
        {1,0,0,0,1},
        {1,0,0,0,1},
        {1,0,0,0,1},
        {1,0,0,0,1},
        {0,1,1,1,0},
    };
    private static readonly int[,] LetterK = new int[,]
    {
        {1,0,0,0,1},
        {1,0,0,1,0},
        {1,0,1,0,0},
        {1,1,0,0,0},
        {1,0,1,0,0},
        {1,0,0,1,0},
        {1,0,0,0,1},
    };
    private static readonly int[,] LetterE = new int[,]
    {
        {1,1,1,1,1},
        {1,0,0,0,0},
        {1,0,0,0,0},
        {1,1,1,1,0},
        {1,0,0,0,0},
        {1,0,0,0,0},
        {1,1,1,1,1},
    };
    private static readonly int[,] LetterM = new int[,]
    {
        {1,0,0,0,1},
        {1,1,0,1,1},
        {1,0,1,0,1},
        {1,0,0,0,1},
        {1,0,0,0,1},
        {1,0,0,0,1},
        {1,0,0,0,1},
    };
    private static readonly int[,] LetterN = new int[,]
    {
        {1,0,0,0,1},
        {1,1,0,0,1},
        {1,0,1,0,1},
        {1,0,0,1,1},
        {1,0,0,0,1},
        {1,0,0,0,1},
        {1,0,0,0,1},
    };

    private static readonly int[][,] Letters = new[]
    {
        LetterP, LetterO, LetterK, LetterE,
        LetterM, LetterO, LetterN
    };

    public MainMenu()
    {
        art = new Rgb[ArtH, ArtW];

        // 1) Remplir le contour en bleu, intérieur en noir
        for (int y = 0; y < ArtH; y++)
            for (int x = 0; x < ArtW; x++)
                art[y, x] = (x == 0 || y == 0 || x == ArtW - 1 || y == ArtH - 1)
                    ? new Rgb(0, 0, 255)
                    : new Rgb(0, 0, 0);

        // 2) Paramètres de mise à l’échelle ×2
        const int scale = 2;
        int letterW = Letters[0].GetLength(1);  // 5 colonnes
        int letterH = Letters[0].GetLength(0);  // 7 lignes
        int spacing = 2;                       // espacement “logique”
        int sxLetter = letterW * scale;         // 5×2 = 10 cols effectives
        int syLetter = letterH * scale;         // 7×2 = 14 lignes effectives
        int sxSpacing = spacing * scale;         // 2×2 = 4 cols entre lettres
        int totalTextWidth = Letters.Length * sxLetter
                           + (Letters.Length - 1) * sxSpacing;
        int startX = (ArtW - totalTextWidth) / 2;
        int startY = (ArtH - syLetter) / 2;

        // 3) Peindre chaque “pixel” de lettre en bloc 2×2
        for (int i = 0; i < Letters.Length; i++)
        {
            int[,] matrix = Letters[i];
            int baseX = startX + i * (sxLetter + sxSpacing);
            for (int my = 0; my < letterH; my++)
            {
                for (int mx = 0; mx < letterW; mx++)
                {
                    if (matrix[my, mx] != 1)
                        continue;

                    // dessiner un carré scale×scale
                    for (int dy = 0; dy < scale; dy++)
                        for (int dx = 0; dx < scale; dx++)
                        {
                            int py = startY + my * scale + dy;
                            int px = baseX + mx * scale + dx;
                            art[py, px] = new Rgb(255, 255, 0);
                        }
                }
            }
        }
    }


    public void Run()
    {
        Console.Clear();
        Console.CursorVisible = false;

        int screenW = Console.WindowWidth;
        int screenH = Console.WindowHeight;

        int artW = art.GetLength(1);
        int artH = art.GetLength(0);

        // Hauteur totale : art + 3 noms + spacing + boutons
        int totalHeight = artH
                        + 3                // trois noms
                        + ButtonSpacing    // espace avant les boutons
                        + options.Length * ButtonHeight
                        + (options.Length - 1) * ButtonSpacing;

        int startY = (screenH - totalHeight) / 2;
        int artX = (screenW - artW) / 2;
        int artY = startY;

        // 1) Afficher le pixel-art
        for (int row = 0; row < artH; row++)
            for (int col = 0; col < artW; col++)
                DrawPixel(artX + col, artY + row, art[row, col]);

        // 2) Afficher les 3 noms
        string[] names = {
            "Simon - BILLET",
            "Elouan - BOUCHE",
            "Antoine - BOUTARIN"
        };
        int namesY = artY + artH;
        for (int i = 0; i < names.Length; i++)
        {
            string s = names[i];
            int tx = (screenW - s.Length) / 2;
            Ansi.WriteFg(new Rgb(200, 200, 200));
            Console.SetCursorPosition(tx, namesY + i);
            Console.Write(s);
            Ansi.Reset();
        }

        // 3) Position des boutons
        int btnX = (screenW - ButtonWidth) / 2;
        int btnY = namesY + names.Length + ButtonSpacing;

        bool running = true;
        while (running)
        {
            // 4) Afficher chaque bouton avec effet hover
            for (int i = 0; i < options.Length; i++)
            {
                int y = btnY + i * (ButtonHeight + ButtonSpacing);
                DrawHoverButton(
                    x: btnX,
                    y: y,
                    w: ButtonWidth,
                    h: ButtonHeight,
                    label: options[i],
                    selected: i == selectedIndex
                );
            }

            // 5) Navigation
            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.UpArrow:
                case ConsoleKey.Z:
                    selectedIndex =
                        (selectedIndex - 1 + options.Length) % options.Length;
                    break;

                case ConsoleKey.DownArrow:
                case ConsoleKey.S:
                    selectedIndex =
                        (selectedIndex + 1) % options.Length;
                    break;

                case ConsoleKey.Enter:
                    if (selectedIndex == 0)
                    {
                        new SaveFileMenu().Run();
                        Console.Clear();
                        running = false;
                    }
                    else
                        Environment.Exit(0);
                    break;
            }
        }

        Console.CursorVisible = true;
    }

    private void DrawPixel(int x, int y, Rgb color)
    {
        Ansi.WriteBg(color);
        Console.SetCursorPosition(x, y);
        Console.Write(' ');
        Ansi.Reset();
    }

    private void DrawHoverButton(int x, int y, int w, int h, string label, bool selected)
    {
        // Fond & bordure jaunes au hover, gris sinon
        var bg = selected ? new Rgb(255, 255, 0) : new Rgb(80, 80, 80);
        var border = selected ? new Rgb(255, 255, 0) : new Rgb(80, 80, 80);
        var textColor = selected ? new Rgb(0, 0, 0) : new Rgb(200, 200, 200);

        // Fond du bouton
        for (int dy = 0; dy < h; dy++)
            for (int dx = 0; dx < w; dx++)
                DrawPixel(x + dx, y + dy, bg);

        // Bordure tout autour
        for (int dx = 0; dx < w; dx++)
        {
            DrawPixel(x + dx, y, border);
            DrawPixel(x + dx, y + h - 1, border);
        }
        for (int dy = 0; dy < h; dy++)
        {
            DrawPixel(x, y + dy, border);
            DrawPixel(x + w - 1, y + dy, border);
        }

        // Texte centré
        int tx = x + (w - label.Length) / 2;
        int ty = y + h / 2;
        Ansi.WriteBg(bg);
        Ansi.WriteFg(textColor);
        Console.SetCursorPosition(tx, ty);
        Console.Write(label);
        Ansi.Reset();
    }
}

public class SaveFileMenu : IMenu
{
    private const int ButtonWidth = 35;  // largeur en caractères
    private const int ButtonHeight = 20;  // hauteur en lignes
    private const int ButtonSpacing = 4;  // espace entre les rectangles

    private readonly string[] slots = { "SAVE SLOT 1", "SAVE SLOT 2", "SAVE SLOT 3" };
    private int selectedIndex = 0;

    public string GetSlotDisplayName(int slotIndex)
    {
        string savePath = $"Save{slotIndex+1}.json";
        if (File.Exists(savePath))
        {
            try
            {
                var json = File.ReadAllText(savePath);
                var save = JsonSerializer.Deserialize<GameSave>(json);
                if (save?.Player?.Name != null)
                    return save.Player.Name;
            }
            catch { /* ignore erreur de lecture ou de parsing */ }
        }
        return "NEW GAME";
    }

    public void Run()
    {
        Console.Clear();
        Console.CursorVisible = false;

        bool running = true;
        while (running)
        {
            int screenW = Console.WindowWidth;
            int screenH = Console.WindowHeight;

            int totalWidth = slots.Length * ButtonWidth
                           + (slots.Length - 1) * ButtonSpacing;
            int startX = (screenW - totalWidth) / 2;
            int startY = (screenH - ButtonHeight) / 2;

            Console.Clear();

            for (int i = 0; i < slots.Length; i++)
            {
                int x = startX + i * (ButtonWidth + ButtonSpacing);
                int y = startY;
                bool isSel = (i == selectedIndex);

                // Choix des couleurs
                var bg = isSel ? new Rgb(40, 40, 40) : new Rgb(10, 10, 10);
                var fgTitle = isSel ? new Rgb(255, 255, 255) : new Rgb(200, 200, 200);

                // 4.a) Fond du rectangle
                for (int dy = 0; dy < ButtonHeight; dy++)
                {
                    Console.SetCursorPosition(x, y + dy);
                    Ansi.WriteBg(bg);
                    Console.Write(new string(' ', ButtonWidth));
                    Ansi.Reset();
                }

                // 4.b) Titre en haut, centré
                int titleX = x + (ButtonWidth - slots[i].Length) / 2;
                Console.SetCursorPosition(titleX, y);
                Ansi.WriteBg(bg);
                Ansi.WriteFg(fgTitle);
                Console.Write(slots[i]);
                Ansi.Reset();

                // Affichage du nom du joueur (plus haut)
                string playerName = GetSlotDisplayName(i);
                // Calcul du centre pour le bloc nom+sprite
                int blocHeight = 1 + PokeSprite.Height; // 1 ligne pour le nom + sprite
                int blocY = y + (ButtonHeight - blocHeight) / 2;
                int nameX = x + (ButtonWidth - playerName.Length) / 2;
                int nameY = blocY;
                Console.SetCursorPosition(nameX, nameY);
                Ansi.WriteBg(bg);
                Ansi.WriteFg(fgTitle);
                Console.Write(playerName);
                Ansi.Reset();

                // Affichage du sprite du premier Pokémon si sauvegarde existante
                string savePath = $"Save{i+1}.json";
                if (File.Exists(savePath))
                {
                    try
                    {
                        var json = File.ReadAllText(savePath);
                        var save = JsonSerializer.Deserialize<GameSave>(json);
                        var team = save?.Player?.TeamPokemons;
                        if (team != null && team.Count > 0)
                        {
                            var pokeName = team[0].Name;
                            var sprite = PokemonSprites.GetByName(pokeName);
                            if (sprite != null)
                            {
                                int spriteX = x + (ButtonWidth - PokeSprite.Width) / 2;
                                int spriteY = nameY + 1;
                                for (int sy = 0; sy < PokeSprite.Height; sy++)
                                {
                                    Console.SetCursorPosition(spriteX, spriteY + sy);
                                    for (int sx = 0; sx < PokeSprite.Width; sx++)
                                    {
                                        var tile = sprite.Pixels[sy, PokeSprite.Width - 1 - sx];
                                        Ansi.WriteBg(tile.Color);
                                        Console.Write(' ');
                                    }
                                    Ansi.Reset();
                                }
                            }
                        }
                    }
                    catch { /* ignore erreur de lecture ou de parsing */ }
                }

                // Bordure si sélectionné (inchangé)
                if (isSel)
                {
                    var border = new Rgb(255, 255, 255);
                    // ligne du haut et du bas
                    Console.SetCursorPosition(x, y);
                    Ansi.WriteBg(border); Console.Write(new string(' ', ButtonWidth)); Ansi.Reset();

                    Console.SetCursorPosition(x, y + ButtonHeight - 1);
                    Ansi.WriteBg(border); Console.Write(new string(' ', ButtonWidth)); Ansi.Reset();

                    // côtés
                    for (int dy = 0; dy < ButtonHeight; dy++)
                    {
                        Console.SetCursorPosition(x, y + dy);
                        Ansi.WriteBg(border); Console.Write(' '); Ansi.Reset();

                        Console.SetCursorPosition(x + ButtonWidth - 1, y + dy);
                        Ansi.WriteBg(border); Console.Write(' '); Ansi.Reset();
                    }
                }
            }

            // 5) Gestion des touches
            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.LeftArrow:
                case ConsoleKey.Q:
                    selectedIndex = (selectedIndex - 1 + slots.Length) % slots.Length;
                    break;

                case ConsoleKey.RightArrow:
                case ConsoleKey.D:
                    selectedIndex = (selectedIndex + 1) % slots.Length;
                    break;

                case ConsoleKey.Enter:
                    // Lancer la logique selon selectedIndex
                    switch (selectedIndex)
                    {
                        case 0: LoadGame(0); break;
                        case 1: LoadGame(1); break;
                        case 2: LoadGame(2); break;
                    }
                    running = false;
                    break;

                case ConsoleKey.Escape:
                    MenuManager.OpenFullScreen(new MainMenu());
                    break;
            }
        }

        Console.CursorVisible = false;
    }
    private void LoadGame(int slotNumber)
    {
        var json = new Json();
        json.SlotNumber = slotNumber;
        World.Initialize(json);
    }
}
public class SpriteViewerMenu : IOverlayMenu
{
    private readonly List<PokeSprite> sprites;
    private int currentIndex;
    private bool reverse;

    public int Width => PokeSprite.Width;
    public int Height => PokeSprite.Height;

    public SpriteViewerMenu(List<PokeSprite> sprites)
    {
        this.sprites = sprites;
        this.currentIndex = 0;
        this.reverse = false;
    }

    public void Run(int startX, int startY)
    {
        UI.FillRect(startX - 1, startY - 1, Width + 2, Height + 2, new Rgb(20, 20, 20));
        UI.DrawBorder(startX - 1, startY - 1, Width + 2, Height + 2, new Rgb(200, 200, 200));
        UI.DrawText(startX - 1, startY - 1, Width + 2, "SPRITE", new Rgb(0, 0, 0), new Rgb(100, 100, 100));

        bool running = true;
        while (running)
        {
            UI.FillRect(startX, startY, Width, Height, new Rgb(20, 20, 20));

            var sprite = sprites[currentIndex];
            if (reverse)
                UI.DrawReverseSprite(sprite, startX, startY);
            else
                UI.DrawSprite(sprite, startX, startY);

            var key = Console.ReadKey(true).Key;
            switch (key)
            {
                case ConsoleKey.LeftArrow:
                    reverse = false;
                    break;

                case ConsoleKey.RightArrow:
                    reverse = true;
                    break;

                case ConsoleKey.DownArrow:
                    currentIndex = (currentIndex + 1) % sprites.Count;
                    break;

                case ConsoleKey.UpArrow:
                    currentIndex = (currentIndex - 1 + sprites.Count) % sprites.Count;
                    break;

                case ConsoleKey.P:
                case ConsoleKey.Escape:
                    running = false;
                    break;
            }
        }
    }
}
public class InventoryMenu : IOverlayMenu
{
    private readonly List<string> pokemons;
    private readonly List<string> items;
    private int currentTab = 0;
    private readonly int[] cursors = { 0, 0 };
    private readonly int[] scrollOff = { 0, 0 };
    public int Width => 20;
    public int Height => 17;
    public InventoryMenu(IEnumerable<string> pokemons, IEnumerable<string> items)
    {
        this.pokemons = pokemons?.ToList() ?? new List<string>();
        this.items = items?.ToList() ?? new List<string>();
    }

    public void Run(int startX, int startY)
    {
        UI.FillRect(startX - 1, startY - 1, Width + 2, Height + 2, new Rgb(20, 20, 60));
        UI.DrawBorder(startX - 1, startY - 1, Width + 2, Height + 2, new Rgb(204, 204, 204));
        Ansi.WriteBg(new Rgb(20, 20, 60));
        UI.DrawText(startX - 1, startY - 1, Width + 2, "INVENTAIRE", new Rgb(0, 0, 0), new Rgb(204, 204, 204));

        DrawTabs(startX, startY);

        bool running = true;
        while (running)
        {
            List<string> list = currentTab == 0
                ? World.player.TeamPokemons
                     .Select(p => $"{p.Name} Lv{p.Level}")
                     .ToList()
                : World.player.Items
                     .Select(i => $"{i.Name} x{i.Quantity}")
                     .ToList();

            int cursor = cursors[currentTab];
            int scroll = scrollOff[currentTab];
            int dispH = Height - 1;
            int maxCursor = Math.Max(0, list.Count - 1);
            int maxScroll = Math.Max(0, list.Count - dispH);

            cursor = Math.Clamp(cursor, 0, maxCursor);
            scroll = Math.Clamp(scroll, 0, maxScroll);
            cursors[currentTab] = cursor;
            scrollOff[currentTab] = scroll;

            for (int line = 0; line < dispH; line++)
            {
                int idx = scroll + line;
                string lbl = idx < list.Count ? list[idx] : "";

                bool isSel = idx == cursor;
                var bg = isSel
                    ? new Rgb(204, 204, 204)
                    : new Rgb(241, 247, 251);

                UI.FillRect(startX, startY + 1 + line, Width, 1, bg);
                UI.DrawText(startX,
                            startY + 1 + line,
                            Width,
                            lbl,
                            isSel ? new Rgb(154, 125, 127) : new Rgb(213, 205, 172),
                            isSel ? new Rgb(213, 205, 172) : new Rgb(235, 227, 201));
            }

            var key = Console.ReadKey(true).Key;
            switch (key)
            {
                case ConsoleKey.LeftArrow:
                case ConsoleKey.RightArrow:
                    currentTab = 1 - currentTab;
                    DrawTabs(startX, startY);
                    break;

                case ConsoleKey.UpArrow:
                    if (cursors[currentTab] > 0)
                    {
                        cursors[currentTab]--;
                        if (cursors[currentTab] < scrollOff[currentTab])
                            scrollOff[currentTab]--;
                    }
                    break;

                case ConsoleKey.DownArrow:
                    if (cursors[currentTab] < list.Count - 1)
                    {
                        cursors[currentTab]++;
                        if (cursors[currentTab] >= scrollOff[currentTab] + dispH)
                            scrollOff[currentTab]++;
                    }
                    break;

                case ConsoleKey.Escape:
                case ConsoleKey.Tab:
                case ConsoleKey.I:
                    running = false;
                    break;
            }
        }
    }

    private void DrawTabs(int startX, int startY)
    {
        string[] titles = { "POKÉMON", "OBJETS" };
        int tabW = Width / 2;

        for (int i = 0; i < 2; i++)
        {
            bool selected = i == currentTab;
            var bg = selected
                ? new Rgb(60, 60, 100)
                : new Rgb(20, 20, 60);

            UI.FillRect(startX + i * tabW, startY, tabW, 1, bg);
            UI.DrawText(startX + i * tabW,
                        startY,
                        tabW,
                        titles[i], 
                        selected ? new Rgb(0, 0, 0) : new Rgb(255, 255, 255),
                        selected ? new Rgb(255, 255, 255) : new Rgb(0, 00, 0));
        }
    }
}
