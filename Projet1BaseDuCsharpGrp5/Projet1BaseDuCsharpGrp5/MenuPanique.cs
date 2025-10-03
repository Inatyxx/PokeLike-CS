using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projet1BaseDuCsharpGrp5;
public class NameEntryOverlay : IOverlayMenu
{
    private const int MaxLength = 8;
    private const string Title = "ENTREZ VOTRE PRÉNOM";

    private string input = "";

    // Taille de l'overlay en tuiles
    public int Width => 16;
    public int Height => 5;

    public void Run(int startX, int startY)
    {
        Console.CursorVisible = false;
        bool running = true;

        while (running)
        {
            // 1) Couleurs : fond blanc, bordure grise, texte noir
            var bg = new Rgb(255, 255, 255);
            var border = new Rgb(128, 128, 128);
            var fg = new Rgb(0, 0, 0);

            // 2) Fond et bordure
            UI.FillRect(startX, startY, Width, Height, bg);
            UI.DrawBorder(startX, startY, Width, Height, border);

            // 3) Titre centré sur la ligne 0
            UI.DrawText(
                startX,
                startY + 0,
                Width,
                Title,
                fg,
                bg
            );

            // 4) Zone de saisie avec underscore clignotant
            string display =
                input
              + (input.Length < MaxLength ? "_" : "")
              + new string(' ', MaxLength - input.Length);

            UI.DrawText(
                startX,
                startY + 2,
                Width,
                display,
                fg,
                bg
            );

            // 5) Gestion des touches
            var keyInfo = Console.ReadKey(true);
            switch (keyInfo.Key)
            {
                case ConsoleKey.Enter:
                    World.player.Name = input;
                    running = false;
                    break;

                case ConsoleKey.Escape:
                    World.player.Name = "";
                    running = false;
                    break;

                case ConsoleKey.Backspace:
                    if (input.Length > 0)
                        input = input.Substring(0, input.Length - 1);
                    break;

                default:
                    char c = keyInfo.KeyChar;
                    if (input.Length < MaxLength &&
                        (char.IsLetterOrDigit(c) || c == ' '))
                    {
                        input += c;
                    }
                    break;
            }
        }

        Console.CursorVisible = false;
    }
}

public class StarterSelectionMenu : IOverlayMenu
{
    private readonly List<PokeSprite> starters = new()
    {
        PokemonSprites.Bulbizarre,
        PokemonSprites.Salameche,
        PokemonSprites.Carapuce
    };

    public static readonly string[] StarterNames =
    {
        "Bulbizarre",
        "Salameche",
        "Carapuce"
    };

    private int selectedIndex = 0;

    private const int Spacing = 1;

    public int Width
        => starters.Count * PokeSprite.Width
         + (starters.Count - 1) * Spacing;
    public int Height => PokeSprite.Height + 1;

    public int Choice { get; private set; } = -1;

    public void Run(int startX, int startY)
    {
        bool running = true;
        while (running)
        {
            // Fond + border + titre
            UI.FillRect(startX - 1, startY - 1, Width + 2, Height + 2, new Rgb(20, 20, 60));
            UI.DrawBorder(startX - 1, startY - 1, Width + 2, Height + 2, new Rgb(204, 204, 204));
            UI.DrawText(startX - 1, startY - 1, Width + 2,
                        "CHOISISSEZ VOTRE STARTER",
                        new Rgb(0, 0, 0), new Rgb(204, 204, 204));

            // Affichage des sprites et surlignage
            for (int i = 0; i < starters.Count; i++)
            {
                int x = startX + i * (PokeSprite.Width + Spacing);
                int y = startY + 1;

                if (i == selectedIndex)
                {
                    UI.FillRect(x - 1, y - 1,
                                PokeSprite.Width + 2, PokeSprite.Height + 2,
                                new Rgb(60, 60, 100));
                }

                UI.FillRect(x, y,
                            PokeSprite.Width, PokeSprite.Height,
                            new Rgb(20, 20, 60));

                UI.DrawSprite(starters[i], x, y);
            }

            // Gestion des touches
            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.LeftArrow:
                case ConsoleKey.Q:
                    selectedIndex = (selectedIndex - 1 + starters.Count) % starters.Count;
                    break;

                case ConsoleKey.RightArrow:
                case ConsoleKey.D:
                    selectedIndex = (selectedIndex + 1) % starters.Count;
                    break;

                case ConsoleKey.Enter:
                case ConsoleKey.E:
                    var chosenName = StarterNames[selectedIndex];
                    World.player.AddPokemon(chosenName);

                    Choice = selectedIndex;
                    running = false;
                    break;

                case ConsoleKey.Escape:
                    running = false;
                    break;
            }
        }
    }
}