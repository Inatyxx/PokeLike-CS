using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Projet1BaseDuCsharpGrp5;

public class Player : Dresseur
{
    public List<Pokemon> AllPokemon { get; set; }
    public Player(string name, Rgb color, int startX = 1, int startY = 1)
    : base(name, startX, startY, color)
    {
        AllPokemon = new List<Pokemon>();
    }

    public Player() : base("", 1, 1, new Rgb(255, 0, 0)) { } // Constructeur par défault pour la déserialisation du json de sauvegarde

    public void AddPokeDollars(int value) => PokeDollars += value;

    public void LosePokeDollars(int value)
    {
        PokeDollars -= value;
        if (PokeDollars < 0) PokeDollars = 0;
    }

    public override List<Pokemon> AddPokemon(string _name, int _level = 1)
    {
        // Guard
        if (_name == null) throw new ArgumentNullException(nameof(_name));

        var pokemon = World.DataJson.CreatePokemon(_name);

        pokemon.Level = _level;
        pokemon.OnInitPokemon();

        pokemon.Level = _level;

        if (TeamPokemons.Count < 6)
        {
            TeamPokemons.Add(pokemon);
            //Console.WriteLine($"{pokemon.Name} ajouté à l'équipe de {Name}");
        }
        else
        {
            AllPokemon.Add(pokemon);
            //Console.WriteLine($"{pokemon.Name} envoyé au PC de {Name}.");
        }

        return TeamPokemons;
    }

    public  List<Pokemon> AddPokemon(Pokemon pokemon)
    {
        // Guard
        if (pokemon == null) throw new ArgumentNullException(nameof(pokemon));

        if (TeamPokemons.Count < 6)
        {
            TeamPokemons.Add(pokemon);
        }
        else
        {
            AllPokemon.Add(pokemon);
        }

        return TeamPokemons;
    }

    public void Interact()
    {
        foreach (var d in World.CurrentMap.Dresseurs)
        {
            int dx = Math.Abs(d.X - X);
            int dy = Math.Abs(d.Y - Y);
            if (dx + dy == 1 && d is Pnj pnj)
            {
                pnj.Converse(this);
                return;
            }
        }
    }
    public void Move(ConsoleKey key)
    {
        // 1) Calcul du delta
        int dx = 0, dy = 0;
        switch (key)
        {
            case ConsoleKey.LeftArrow:
            case ConsoleKey.Q:
                dx = -1;
                PokeDollars++;
                break;
            case ConsoleKey.RightArrow:
            case ConsoleKey.D:
                dx = +1;
                break;
            case ConsoleKey.UpArrow:
            case ConsoleKey.Z:
                dy = -1;
                break;
            case ConsoleKey.DownArrow:
            case ConsoleKey.S:
                dy = +1;
                break;
            case ConsoleKey.X:
                return;
            default:
                return; // Touche non gérée
        }

        int nx = X + dx;
        int ny = Y + dy;

        // 3) Test de collision sur la map courante
        var map = World.CurrentMap;
        if (nx < 0 || ny < 0 || nx >= map.Width || ny >= map.Height)
            return;
        if (map.Dresseurs.Any(d => d.X == nx && d.Y == ny))
            return;

        if (map.Tiles[nx, ny].IsWall)
            return;


        if (map.Tiles[nx, ny].IsGrass)
        {
            var biome = World.GetBiomeForMap(map);

            if (biome != null)
            {
                double rate = biome.ApparitionRate;

                if (World.Rng.NextDouble() * 100.0 < rate)
                {
                    World.InputBlocked = true;

                    var wild = World.GetRandomWildFromMap(map);
                    if (wild != null)
                    {
                        var playerActive = World.player.TeamPokemons?.FirstOrDefault(p => p.Life > 0) ?? World.player.TeamPokemons?.FirstOrDefault();

                        var playerSprite = PokemonSprites.GetByName(playerActive?.Name ?? "Vierge");
                        var enemySprite = PokemonSprites.GetByName(wild?.Name ?? "Vierge");

                        var arena = new FightArena(playerSprite, enemySprite);
                        var ui = new FightArenaUI(arena); // Visuel
                        var uiConsole = new ConsoleFightUI(); // Console
                        var fm = new FightManager(World.player, ui);

                        fm.StartWildBattle(wild);
                    }

                    World.InputBlocked = false;
                }
            }
        }

        // Empêche la transition en haut de la Plaine si aucun Pokémon
        if (map.TryGetTransition(nx, ny, out var t))
        {
            if (
                // Bloque la transition si le joueur n'a pas de Pokémon
                (World.CurrentMap.Name == "LaPlaine" && t.TargetMap == "LaPlaine2")
                && TeamPokemons.Count == 0
            )
            {
                Console.SetCursorPosition(0, Console.WindowHeight - 2);
                var bodyBlock = new Pnj(
                        name: "Système"
                        );
                Dialogue.Show(bodyBlock, "Il te faut un pokémon pour aller la !");
                return;
            }
            Erase();
            World.ChangeMap(t.TargetMap, t.SpawnX, t.SpawnY);
            return;
        }

        Erase();
        SetPosition(nx, ny);
        Draw();

        foreach (var p in map.Dresseurs.OfType<Pnj>())
            p.TryTriggerAggro(this);
    }

    public void PrintPokemonTeam()
    {
        Console.Clear();
        Console.WriteLine("📦 Ton équipe Pokémon :\n");

        if (TeamPokemons.Count == 0)
        {
            Console.WriteLine("— Aucun Pokémon dans l’équipe —");
        }
        else
        {
            for (int i = 0; i < TeamPokemons.Count; i++)
            {
                var p = TeamPokemons[i];
                Console.WriteLine($"{i + 1}. {p.Name} — Niveau {p.Level}");
            }
        }

        Console.WriteLine("\nAppuie sur une touche pour continuer...");
        Console.ReadKey(true);
        Console.Clear();
        World.CurrentMap.Render();
        foreach (var d in World.CurrentMap.Dresseurs)
            d.Draw();
        Draw(); // Redessine le joueur
    }

    public void HealAllTeamPokemons()
    {
        foreach (var pokemon in TeamPokemons)
        {
            pokemon.RestoreAllLife();
            pokemon.RestoreAllMana();
            pokemon.ResetCombatStats();
        }
    }

    public void CheckLevelPokemon()
    {
        List<Pokemon> listPokemons = World.DataJson.ListPokemon();

        for (int i = 0; i < TeamPokemons.Count; i++)
        {
            var current = TeamPokemons[i];
            if (current.HasEvolution)
            {
                int idx = listPokemons.FindIndex(p => p.Name == current.Name);
                if (idx >= 0 && idx < listPokemons.Count - 1)
                {
                    var next = listPokemons[idx + 1];
                    if (current.Level >= next.Level)
                    {
                        var evolved = World.DataJson.CreatePokemon(next.Name);
                        evolved.Level = current.Level;
                        evolved.Experience = current.Experience;
                        evolved.Competences = current.Competences;

                        TeamPokemons[i] = evolved;
                        Console.WriteLine($"{current.Name} évolue en {evolved.Name} !");
                    }
                }
            }
        }
    }
}