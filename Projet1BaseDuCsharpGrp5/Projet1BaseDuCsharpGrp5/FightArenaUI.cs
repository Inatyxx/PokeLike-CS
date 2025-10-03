using System.Collections.Generic;

namespace Projet1BaseDuCsharpGrp5
{
    public class FightArenaUI : IFightUI
    {
        private readonly FightArena arena;

        public FightArenaUI(FightArena arena)
        {
            this.arena = arena;
        }

        public void Update(Pokemon player, Pokemon enemy)
        {
            int playerPct = player.MaxLife == 0 ? 0 : (player.Life * 100 / player.MaxLife);
            int enemyPct = enemy.MaxLife == 0 ? 0 : (enemy.Life * 100 / enemy.MaxLife);
            int playerManaPct = player.MaxMana == 0 ? 0 : (player.Mana * 100 / player.MaxMana);
            int enemyManaPct = enemy.MaxMana == 0 ? 0 : (enemy.Mana * 100 / enemy.MaxMana);

            var playerSprite = PokemonSprites.GetByName(player?.Name ?? string.Empty);
            var enemySprite = PokemonSprites.GetByName(enemy?.Name ?? string.Empty);
            arena.UpdateSprites(playerSprite, enemySprite);

            // Propage vers FightArena
            arena.UpdateStatusFromOutside(
                playerPct, 
                enemyPct,
                playerManaPct,
                enemyManaPct,
                playerLabel: $"{player.Name} Lv{player.Level}",
                enemyLabel: $"{enemy.Name} Lv{enemy.Level}");
        }

        public void ShowMessage(string message)
        {
            if (arena != null)
            {
                // Méthode proposée dans FightArena (voir plus bas)
                arena.ShowMessage(message);
                return;
            }

            System.Console.WriteLine(message);
            System.Console.WriteLine("Appuyez sur une touche pour continuer...");
            System.Console.ReadKey(true);
        }

        public int ChooseFromList(string title, List<string> options)
        {
            int ox = Math.Max(0, (World.CurrentMap.Width - FightArena.ArenaWidthTiles) / 2);
            int oy = Math.Max(0, (World.CurrentMap.Height - FightArena.ArenaHeightTiles) / 2);

            arena.UpdateChoices(title, options, ox, oy);

            var choice = arena.ShowAndGetChoice(ox, oy);

            // Interprète le choix : si MainOption renvoie l'option index
            switch (choice.Type)
            {
                case FightArena.FightChoice.ChoiceType.MainOption:
                case FightArena.FightChoice.ChoiceType.AttackIndex:
                case FightArena.FightChoice.ChoiceType.ItemIndex:
                case FightArena.FightChoice.ChoiceType.PokeIndex:
                    return choice.Index;
                case FightArena.FightChoice.ChoiceType.Run:
                    return options.FindIndex(o => o.ToUpper().Contains("FUIR"));
                default:
                    return -1;
            }
        }
    }
}
