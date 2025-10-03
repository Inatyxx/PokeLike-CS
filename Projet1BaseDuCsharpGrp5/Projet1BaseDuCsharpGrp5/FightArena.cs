using System;
using System.Collections.Generic;
using System.Linq;
using Projet1BaseDuCsharpGrp5;

namespace Projet1BaseDuCsharpGrp5
{
    enum MenuState { Main, AttackList, ItemList, PokeList }

    public class FightArena : IMenu, IOverlayMenu
    {
        // dimensions fixes
        public const int ArenaWidthTiles = 46;
        public const int ArenaHeightTiles = 35;

        // boutons rectangulaires
        private const int BtnWidthTiles = 10;
        private const int BtnHeightTiles = 3;
        private const int BtnSpacingTiles = 2;
        private const int PanelHeight = BtnHeightTiles * 2 + BtnSpacingTiles;

        // espacement entre sprites
        private const int SpriteGapTiles = 4;

        // vie (0–100) exposée publiquement
        public int PnjPokeLife { get; set; } = 100;
        public int PlayerPokeLife { get; set; } = 100;
        public int PnjPokeMana { get; set; } = 100;
        public int PlayerPokeMana { get; set; } = 100;

        // couleurs globales
        static readonly Rgb BgColor = new Rgb(255, 255, 255);
        static readonly Rgb PanelColor = new Rgb(180, 180, 180);
        static readonly Rgb BtnFgSel = new Rgb(0, 0, 0);
        static readonly Rgb BtnFgUnsel = new Rgb(255, 255, 255);
        static readonly Rgb DialogueBg = new Rgb(20, 20, 20);

        // couleurs principaux / sélection
        static readonly Rgb[] BtnBgColors = {
            new Rgb(200,  50,  50),
            new Rgb( 50, 200,  50),
            new Rgb( 50,  50, 200),
            new Rgb(200, 200,  50)
        };
        static readonly Rgb[] BtnSelectedBg = {
            new Rgb(240,  80,  80),
            new Rgb( 80, 240,  80),
            new Rgb( 80,  80, 240),
            new Rgb(240, 240,  80)
        };

        public Rgb AttackHoverBg { get; set; } = new Rgb(240, 240, 240);

        private PokeSprite _playerSprite;
        private PokeSprite _enemySprite;

        private readonly List<string> mainOptions = new() { "ATTAQUE", "OBJET", "POKÉMON", "FUIR" };
        private List<string> dynamicOptions;
        private MenuState state;
        private int selectedIndex;
        private bool exitRequested;

        private bool overlayNeedsRedraw = false;
        private int lastDrawnSelectedIndex = -1;
        private int menuVersion = 0;
        private int lastDrawnMenuVersion = -1;

        private const int DialogueHeightTiles = 3; 
        private const int PanelPadding = 1; 


        public struct FightChoice
        {
            public enum ChoiceType { MainOption, AttackIndex, ItemIndex, PokeIndex, Run, None }
            public ChoiceType Type;
            public int Index; // pour Attack/Item/Poke -> index, sinon -1
            public string RawLabel;
        }

        private int overlayOx, overlayOy; // pour redessiner depuis l'extérieur
        private FightChoice lastChoice;

        private string playerLabelField = "";
        private string enemyLabelField = "";

        private string currentMenuTitle = "";
        private List<string> currentMenuOptions = new List<string>();
        private int currentSelectedIndex = 0;

        public FightArena(PokeSprite player, PokeSprite enemy)
        {
            _playerSprite = player;
            _enemySprite = enemy;
            dynamicOptions = mainOptions;
            state = MenuState.Main;
            selectedIndex = 0;
            exitRequested = false;
        }

        public void UpdateSprites(PokeSprite playerSprite, PokeSprite enemySprite, bool forceRedraw = false)
        {
            // fallback defensive
            playerSprite = playerSprite ?? PokemonSprites.Vierge;
            enemySprite = enemySprite ?? PokemonSprites.Vierge;

            // Si pas forcé et que rien n'a changé (même référence), ne redessine pas.
            if (!forceRedraw &&
                Object.ReferenceEquals(_playerSprite, playerSprite) &&
                Object.ReferenceEquals(_enemySprite, enemySprite))
            {
                return; // évite le redraw inutile
            }

            // Met à jour les champs
            _playerSprite = playerSprite;
            _enemySprite = enemySprite;

            DrawSprites(overlayOx, overlayOy);
        }

        public void UpdateChoices(string title, List<string> options, int ox, int oy)
        {
            currentMenuTitle = title ?? "";
            currentMenuOptions = options != null ? new List<string>(options) : new List<string>();
            currentMenuOptions = currentMenuOptions ?? new List<string>();
            currentSelectedIndex = 0;

            overlayOx = ox;
            overlayOy = oy;

            // Synchronize the dynamicOptions used by DrawRectButtons and input handling
            dynamicOptions = new List<string>(currentMenuOptions);

            if (dynamicOptions.Count == 0)
                dynamicOptions = new List<string>(); // keep empty -> buttons disabled

            // Try to deduce which state the menu represents (existing logic)
            var upperTitle = (title ?? "").ToUpperInvariant();
            if (dynamicOptions.SequenceEqual(mainOptions))
                state = MenuState.Main;
            else if (upperTitle.Contains("ATTAQUE") || upperTitle.Contains("COMPETENCE") || (dynamicOptions.Count == 4 && dynamicOptions.All(d => !d.ToUpper().Contains(" X"))))
                state = MenuState.AttackList;
            else if (upperTitle.Contains("OBJET") || dynamicOptions.Any(o => o.ToUpper().Contains(" X")))
                state = MenuState.ItemList;
            else if (upperTitle.Contains("POK") || dynamicOptions.Any(o => o.ToUpper().Contains("LV")))
                state = MenuState.PokeList;
            else
                state = MenuState.AttackList;

            // clamp selectedIndex to a selectable index if needed
            if (!IsSelectable(selectedIndex))
                selectedIndex = FindNextSelectableFrom(0);

            // Mark for redraw instead of immediate draw
            menuVersion++;
            overlayNeedsRedraw = true;
        }

        private void RenderMenu(int ox, int oy)
        {
            if (string.IsNullOrEmpty(currentMenuTitle)) return;

            int panelWidth = BtnWidthTiles * 2 + BtnSpacingTiles;
            int panelHeight = PanelHeight;
            int panelX = ox + Width - panelWidth - PanelPadding;
            int panelY = oy + Height - panelHeight - PanelPadding;

            // afficher le titre dans la zone de dialogue (au dessus du panel)
            int titleX = panelX + 1;
            int titleY = panelY - DialogueHeightTiles;
            UI.DrawText(titleX, titleY, panelWidth - 2, currentMenuTitle, BtnFgUnsel, DialogueBg);
        }

        public FightChoice ShowAndGetChoice(int ox, int oy)
        {
            // stocke la position de l'overlay pour updates
            overlayOx = ox; overlayOy = oy;
            lastChoice = new FightChoice { Type = FightChoice.ChoiceType.None, Index = -1, RawLabel = null };
            exitRequested = false;

            // draw initiale
            Console.CursorVisible = false;
            DrawAll(ox, oy);
            DrawRectButtons(ox, oy);
            lastDrawnSelectedIndex = selectedIndex;
            lastDrawnMenuVersion = menuVersion;
            overlayNeedsRedraw = false;

            while (!exitRequested)
            {
                var key = Console.ReadKey(true).Key;

                HandleInput(key);

                if (overlayNeedsRedraw || selectedIndex != lastDrawnSelectedIndex || menuVersion != lastDrawnMenuVersion)
                {
                    if (menuVersion != lastDrawnMenuVersion)
                    {
                        DrawAll(ox, oy);
                    }
                    DrawRectButtons(ox, oy);

                    lastDrawnSelectedIndex = selectedIndex;
                    lastDrawnMenuVersion = menuVersion;
                    overlayNeedsRedraw = false;
                }
            }

            Console.CursorVisible = true;
            return lastChoice;
        }

        // IOverlayMenu
        public int Width => ArenaWidthTiles;
        public int Height => ArenaHeightTiles;

        // IMenu : ouvre centré
        public void Run() =>
            MenuManager.OpenOverlayCentered(this);

        // IOverlayMenu : boucle principale
        public void Run(int ox, int oy)
        {
            Console.CursorVisible = false;
            overlayOx = ox; 
            overlayOy = oy;

            DrawAll(ox, oy);
            DrawRectButtons(ox, oy);

            while (!exitRequested)
            {
                var key = Console.ReadKey(true).Key;
                HandleInput(key);
                DrawRectButtons(ox, oy);
            }

            Console.CursorVisible = true;
        }

        private void DrawAll(int ox, int oy)
        {
            // fond 
            UI.FillRect(ox, oy, Width, Height, BgColor);

            // calcule les dimensions du panel des boutons (2 colonnes × 2 lignes)
            int panelWidth = BtnWidthTiles * 2 + BtnSpacingTiles;
            int panelHeight = PanelHeight;

            // position du panel des boutons (ancré à droite)
            int panelX = ox + Width - panelWidth - PanelPadding;
            int panelY = oy + Height - panelHeight - PanelPadding;

            // Panel dialogue
            int dialogX = panelX;
            int dialogY = panelY - DialogueHeightTiles;
            UI.FillRect(dialogX, dialogY, panelWidth, DialogueHeightTiles, DialogueBg);

            // Panel btn gris
            UI.FillRect(panelX, panelY, panelWidth, panelHeight, PanelColor);

            DrawSprites(ox, oy);
            DrawEnemyInfo(ox, oy);
            DrawPlayerInfo(ox, oy);

            RenderMenu(ox, oy);
        }

        private void DrawSprites(int ox, int oy)
        {
            const int scaleP = 3, scaleE = 2;

            int swP = PokeSprite.Width * scaleP;
            int shP = PokeSprite.Height * scaleP;
            int swE = PokeSprite.Width * scaleE;
            int shE = PokeSprite.Height * scaleE;

            int areaW = swP + swE + SpriteGapTiles;
            int startX = ox + (Width - areaW) / 2;
            int startY = oy + (Height - PanelHeight - Math.Max(shP, shE)) / 2;

            UI.DrawReverseSpriteScaled(_playerSprite, startX, startY, scaleP);
            UI.DrawSpriteScaled(_enemySprite, startX + swP + SpriteGapTiles, startY, scaleE);
        }

        private void DrawEnemyInfo(int ox, int oy)
        {
            const int scaleE = 2;
            int swP = PokeSprite.Width * 3;
            int swE = PokeSprite.Width * scaleE;
            int areaW = swP + swE + SpriteGapTiles;
            int startX = ox + (Width - areaW) / 2;
            int startY = oy + (Height - PanelHeight - PokeSprite.Height * scaleE) / 2;

            int ex = startX + swP + SpriteGapTiles;
            int ey = startY - 2;

            // niveau+nom en noir sur blanc
            var label = string.IsNullOrEmpty(enemyLabelField) ? "???" : enemyLabelField;
            UI.DrawText(ex, ey - 1, swE, label, BtnFgSel, BgColor);

            // barre de vie : un cran plus haut (ey+1 au lieu de ey+2)
            DrawBar(ex, ey, swE, PnjPokeLife, new Rgb(50, 200, 50), new Rgb(200, 50, 50), height: 2);
            DrawBar(ex, ey + 2, swE, PnjPokeMana, new Rgb(50, 50, 200), new Rgb(30, 30, 90), height: 1);

        }

        private void DrawPlayerInfo(int ox, int oy)
        {
            // on récupère la position des boutons
            int totalBtnW = BtnWidthTiles * 2 + BtnSpacingTiles;
            int totalBtnH = BtnHeightTiles * 2 + BtnSpacingTiles;
            int baseX = ox + Width - totalBtnW;
            int baseY = oy + Height - totalBtnH;

            const int scaleP = 2;
            int swP = PokeSprite.Width * scaleP;

            int px = baseX - swP - 2;
            int py = baseY;

            var label = string.IsNullOrEmpty(playerLabelField) ? "???" : playerLabelField;
            UI.DrawText(px, py + 1, swP, label, BtnFgSel, BgColor);

            DrawBar(px, py + 2, swP - 2, PlayerPokeLife, new Rgb(50, 200, 50), new Rgb(200, 50, 50), height: 2);
            DrawBar(px, py + 4, swP - 2, PlayerPokeMana, new Rgb(50, 50, 200), new Rgb(30, 30, 90), height: 1);
        }

        private void DrawBar(int x, int y, int width, int percent, Rgb fillColor, Rgb emptyColor, int height = 1)
        {
            // clamp percent -> portion remplie
            int filled = Math.Clamp(percent * width / 100, 0, width);

            for (int row = 0; row < height; row++)
            {
                UI.FillRect(x, y + row, filled, 1, fillColor);
                UI.FillRect(x + filled, y + row, width - filled, 1, emptyColor);
            }
        }
            
        private bool IsSelectable(int i)
        {
            return dynamicOptions != null && i >= 0 && i < dynamicOptions.Count && !string.IsNullOrWhiteSpace(dynamicOptions[i]);
        }

        private int FindNextSelectableFrom(int start)
        {
            if (dynamicOptions == null || dynamicOptions.Count == 0)
                return 0;

            int max = Math.Max(1, dynamicOptions.Count);
            int idx = start % max;
            for (int n = 0; n < max; n++)
            {
                int cand = (idx + n) % max;
                if (IsSelectable(cand)) return cand;
            }
            // none selectable -> fallback to 0
            return 0;
        }

        private void DrawRectButtons(int ox, int oy)
        {
            int baseX = ox + Width - (BtnWidthTiles * 2 + BtnSpacingTiles) - PanelPadding;
            int baseY = oy + Height - (BtnHeightTiles * 2 + BtnSpacingTiles) - PanelPadding;

            for (int i = 0; i < 4; i++)
            {
                bool sel = (i == selectedIndex);
                int col = i % 2;
                int row = i / 2;
                int gx = baseX + col * (BtnWidthTiles + BtnSpacingTiles);
                int gy = baseY + row * (BtnHeightTiles + BtnSpacingTiles);

                bool selectable = IsSelectable(i);

                Rgb bg;
                if (!selectable)
                {
                    bg = new Rgb(120, 120, 120); // disabled bg
                }
                else if (state == MenuState.AttackList)
                {
                    bg = sel ? AttackHoverBg : new Rgb(50, 200, 50);
                }
                else
                {
                    bg = sel ? BtnSelectedBg[i] : BtnBgColors[i];
                }

                UI.FillRect(gx, gy, BtnWidthTiles, BtnHeightTiles, bg);
                UI.DrawBorder(gx, gy, BtnWidthTiles, BtnHeightTiles, bg);

                string label = i < dynamicOptions.Count ? dynamicOptions[i] : "";

                var fg = (!selectable) ? new Rgb(200, 200, 200) : (sel ? BtnFgSel : BtnFgUnsel);

                // truncate label to avoid overflow (simple)
                int maxChars = BtnWidthTiles * 2;
                if (label.Length > maxChars) label = label.Substring(0, maxChars);

                UI.DrawText(
                    gx,
                    gy + BtnHeightTiles / 2,
                    BtnWidthTiles,
                    label,
                    sel ? BtnFgSel : BtnFgUnsel,
                    bg
                );
            }
        }

        private int FindNextSelectableInDirection(int step)
        {
            // On parcourt au plus 4 positions (2x2)
            int cand = selectedIndex;
            for (int i = 0; i < 4; i++)
            {
                cand = (cand + step + 4) % 4; // wrap correct pour négatifs
                if (IsSelectable(cand)) return cand;
            }
            // Aucun bouton sélectionnable trouvé : retourne l'index courant (ou 0)
            return Math.Max(0, Math.Min(3, selectedIndex));
        }

        private void HandleInput(ConsoleKey key)
        {
            if (dynamicOptions == null) dynamicOptions = new List<string>();

            switch (key)
            {
                case ConsoleKey.LeftArrow:
                    selectedIndex = FindNextSelectableInDirection(-1);
                    break;
                case ConsoleKey.RightArrow:
                    selectedIndex = FindNextSelectableInDirection(+1);
                    break;
                case ConsoleKey.UpArrow:
                    selectedIndex = FindNextSelectableInDirection(-2);
                    break;
                case ConsoleKey.DownArrow:
                    selectedIndex = FindNextSelectableInDirection(+2);
                    break;
                case ConsoleKey.Enter:
                    if (!IsSelectable(selectedIndex))
                        return;
                    // Si on est dans Main -> mappe en choice MainOption/Run
                    if (state == MenuState.Main)
                    {
                        var opt = dynamicOptions[selectedIndex];
                        if (opt == "FUIR")
                        {
                            lastChoice = new FightChoice { Type = FightChoice.ChoiceType.Run, Index = -1, RawLabel = opt };
                        }
                        else
                        {
                            lastChoice = new FightChoice { Type = FightChoice.ChoiceType.MainOption, Index = selectedIndex, RawLabel = opt };
                        }
                        exitRequested = true;
                        return;
                    }
                    else if (state == MenuState.AttackList)
                    {
                        lastChoice = new FightChoice { Type = FightChoice.ChoiceType.AttackIndex, Index = selectedIndex, RawLabel = dynamicOptions[selectedIndex] };
                        exitRequested = true;
                        return;
                    }
                    else // ItemList / PokeList
                    {
                        // dynamicOptions contient les labels
                        var label = dynamicOptions[selectedIndex];
                        var t = state == MenuState.ItemList ? FightChoice.ChoiceType.ItemIndex : FightChoice.ChoiceType.PokeIndex;
                        lastChoice = new FightChoice { Type = t, Index = selectedIndex, RawLabel = label };
                        exitRequested = true;
                        return;
                    }
            }
        }

        public void UpdateStatusFromOutside(int playerLifePercent, int enemyLifePercent, int playerManaPercent, int enemyManaPercent, string playerLabel = null, string enemyLabel = null)
        {
            PlayerPokeLife = playerLifePercent;
            PnjPokeLife = enemyLifePercent;
            PlayerPokeMana = playerManaPercent;
            PnjPokeMana = enemyManaPercent;

            if (!string.IsNullOrEmpty(playerLabel)) playerLabelField = playerLabel;
            if (!string.IsNullOrEmpty(enemyLabel)) enemyLabelField = enemyLabel;

            menuVersion++;
            overlayNeedsRedraw = true;
        }

        public void ShowMessage(string message)
        {
            if (overlayOx < 0 || overlayOy < 0)
            {
                // fallback console
                Console.Clear();
                Console.WriteLine(message);
                Console.WriteLine("Appuyez sur une touche pour continuer...");
                Console.ReadKey(true);
                return;
            }

            int ox = overlayOx;
            int oy = overlayOy;

            int panelWidth = BtnWidthTiles * 2 + BtnSpacingTiles;
            int panelHeight = PanelHeight;
            int panelX = ox + Width - panelWidth - PanelPadding;
            int panelY = oy + Height - panelHeight - PanelPadding;

            // zone dialogue
            int gx = panelX + 1;
            int gy = panelY - DialogueHeightTiles;
            int w = panelWidth - 2;
            int h = DialogueHeightTiles;

            // background
            UI.FillRect(panelX, gy, panelWidth, h, DialogueBg);

            //UI.DrawText(gx, gy, w, message, new Rgb(230, 230, 230), new Rgb(20, 20, 20));
            // wrapping simple : split en lignes que l'on dessine
            var lines = WrapText(message, w);
            int startLine = Math.Max(0, lines.Count - h); // si overflow, affiche dernières lignes
            for (int i = 0; i < Math.Min(h, lines.Count); i++)
            {
                UI.DrawText(gx, gy + i, w, lines[startLine + i], new Rgb(230, 230, 230), DialogueBg);
            }

            Console.SetCursorPosition(0, 0);
            Console.ReadKey(true);

            DrawAll(ox, oy);
            DrawRectButtons(ox, oy);
        }

        private static List<string> WrapText(string text, int maxWidth)
        {
            var lines = new List<string>();
            if (string.IsNullOrEmpty(text))
            {
                lines.Add("");
                return lines;
            }

            // preserve existing line breaks
            var paragraphs = text.Replace("\r\n", "\n").Split('\n');
            foreach (var para in paragraphs)
            {
                var words = para.Split(' ');
                var current = "";
                foreach (var w in words)
                {
                    if (current.Length == 0)
                    {
                        current = w;
                    }
                    else if (current.Length + 1 + w.Length <= maxWidth)
                    {
                        current += " " + w;
                    }
                    else
                    {
                        lines.Add(current);
                        current = w;
                    }
                }
                if (current.Length > 0) lines.Add(current);
            }
            return lines;
        }
    }
}