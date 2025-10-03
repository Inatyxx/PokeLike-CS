using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Projet1BaseDuCsharpGrp5;

public enum Direction
{
    Up,
    Down,
    Left,
    Right
}
public class Pnj : Dresseur
{
    public enum State
    {
        NeverSpoken,
        SpokenBefore,
        Defeated,
        SpokenAfter
    }

    public enum PersonalityState
    {
        Passive,
        Agressive,
        Pacifiste,
        ProfChen,
        JoySan,
        MasterArena
    }

    public State CurrentState { get; set; } = State.NeverSpoken;
    public PersonalityState Personality { get; set; } = PersonalityState.Passive;
    private readonly string firstDialogue;
    private readonly string defeatDialogue;
    private readonly string postDefeatDialogue;

    // Nouveaux champs pour l’agressivité
    private readonly bool _isAggressive;
    private readonly Direction watchDirection;
    private readonly int watchRange;

    public Item _reward {  get; set; }

    // Constructeur classique
    public Pnj(
        string name,
        int startX = 1,
        int startY = 1,
        Rgb? color = null,
        int pokeDollars = 0,
        string firstDialogue = "",
        string defeatDialogue = "",
        string postDefeatDialogue = "",
        PersonalityState personality = PersonalityState.Passive,
        Item reward = null
    ) : base(name, startX, startY, color ?? new Rgb(0, 0, 255), pokeDollars = 0)
    {
        Name = name;
        this.firstDialogue = firstDialogue;
        this.defeatDialogue = defeatDialogue;
        this.postDefeatDialogue = postDefeatDialogue;

        // Par défaut pas agressif
        _isAggressive = false;
        watchDirection = Direction.Down;
        watchRange = 0;

        Personality = personality;
        _reward = reward;
    }

    // Constructeur « agressif »
    public Pnj(
        string name,
        int startX,
        int startY,
        Rgb color,
        int pokeDollars,
        string firstDialogue,
        string defeatDialogue,
        string postDefeatDialogue,
        bool isAggressive,
        Direction watchDirection,
        PersonalityState personality = PersonalityState.Agressive,
        int watchRange = 10,
        Item reward = null
    ) : this(name, startX, startY, color, pokeDollars, firstDialogue, 
        defeatDialogue, postDefeatDialogue, personality, reward)
    {
        Name = name;
        PokeDollars = pokeDollars;
        _isAggressive = isAggressive;
        this.watchDirection = watchDirection;
        this.watchRange = watchRange;
        Personality = personality;
        _reward = reward;
    }

    public Pnj() : base() { }

    public void Converse(Player player)
    {
        switch (CurrentState)
        {
            case State.NeverSpoken:
                Dialogue.Show(this, firstDialogue);
                switch (Personality)
                {
                    case PersonalityState.Agressive:
                        CurrentState = State.SpokenAfter;
                        var playerActive = World.player.TeamPokemons?.FirstOrDefault(p => p.Life > 0) ?? World.player.TeamPokemons?.FirstOrDefault();
                        var pnj = this.TeamPokemons?.FirstOrDefault(p => p.Life > 0) ?? World.player.TeamPokemons?.FirstOrDefault();

                        var playerSprite = PokemonSprites.GetByName(playerActive?.Name ?? "Vierge");
                        var enemySprite = PokemonSprites.GetByName(pnj?.Name ?? "Vierge");

                        var arena = new FightArena(playerSprite, enemySprite);
                        var ui = new FightArenaUI(arena); // Visuel
                        var uiConsole = new ConsoleFightUI(); // Console
                        var fm = new FightManager(World.player, ui);

                        fm.StartPnjBattle(this);
                        break;

                    case PersonalityState.Passive:
                        CurrentState = State.SpokenAfter;
                        playerActive = World.player.TeamPokemons?.FirstOrDefault(p => p.Life > 0) ?? World.player.TeamPokemons?.FirstOrDefault();
                        pnj = this.TeamPokemons?.FirstOrDefault(p => p.Life > 0) ?? World.player.TeamPokemons?.FirstOrDefault();

                        playerSprite = PokemonSprites.GetByName(playerActive?.Name ?? "Vierge");
                        enemySprite = PokemonSprites.GetByName(pnj?.Name ?? "Vierge");

                        arena = new FightArena(playerSprite, enemySprite);
                        ui = new FightArenaUI(arena); // Visuel
                        uiConsole = new ConsoleFightUI(); // Console
                        fm = new FightManager(World.player, ui);

                        fm.StartPnjBattle(this);
                        break;

                    case PersonalityState.Pacifiste:
                        CurrentState = State.SpokenAfter;
                        break;

                    case PersonalityState.ProfChen:
                        CurrentState = State.SpokenAfter;
                        var menu = new StarterSelectionMenu();
                        int sx = (Console.WindowWidth - menu.Width) / 2;
                        int sy = (Console.WindowHeight - menu.Height) / 2;

                        MenuManager.OpenOverlayCentered(menu);

                        if (menu.Choice >= 0)
                        {
                            Dialogue.Show(
                                new Pnj(name: "Système"),
                                $"Vous avez choisi {StarterSelectionMenu.StarterNames[menu.Choice]} !"
                            );
                        }

                        break;

                    case PersonalityState.JoySan:
                        player.HealAllTeamPokemons();
                        break;

                    case PersonalityState.MasterArena:
                        playerActive = World.player.TeamPokemons?.FirstOrDefault(p => p.Life > 0) ?? World.player.TeamPokemons?.FirstOrDefault();
                        pnj = this.TeamPokemons?.FirstOrDefault(p => p.Life > 0) ?? World.player.TeamPokemons?.FirstOrDefault();

                        playerSprite = PokemonSprites.GetByName(playerActive?.Name ?? "Vierge");
                        enemySprite = PokemonSprites.GetByName(pnj?.Name ?? "Vierge");

                        arena = new FightArena(playerSprite, enemySprite);
                        ui = new FightArenaUI(arena); // Visuel
                        uiConsole = new ConsoleFightUI(); // Console
                        fm = new FightManager(World.player, ui);

                        fm.StartPnjBattle(this);
                        break;
                }
                    break;

            case State.Defeated:
                Dialogue.Show(this, postDefeatDialogue);
                CurrentState = State.SpokenAfter;
                break;

            case State.SpokenAfter:
                Dialogue.Show(this, postDefeatDialogue);
                if (Personality == PersonalityState.JoySan)
                {
                    player.HealAllTeamPokemons();
                }
                break;
        }
    }

    public void OnDefeat()
    {
        if (CurrentState == State.SpokenBefore)
        {
            Dialogue.Show(this, defeatDialogue);
            CurrentState = State.Defeated;
        }
    }

    /// <summary>
    /// À appeler après chaque déplacement du joueur.
    /// Déclenche la chasse si l’aggro est activée et le joueur passe dans le champ de vision.
    /// </summary>
    public void TryTriggerAggro(Player player)
    {
        if (!_isAggressive
            || World.InputBlocked
            || CurrentState != State.NeverSpoken)
            return;

        bool inSight = false;
        switch (watchDirection)
        {
            case Direction.Left:
                inSight = player.Y == Y
                       && (X - player.X) >= 1
                       && (X - player.X) <= watchRange;
                break;

            case Direction.Right:
                inSight = player.Y == Y
                       && (player.X - X) >= 1
                       && (player.X - X) <= watchRange;
                break;

            case Direction.Up:
                inSight = player.X == X
                       && (Y - player.Y) >= 1
                       && (Y - player.Y) <= watchRange;
                break;

            case Direction.Down:
                inSight = player.X == X
                       && (player.Y - Y) >= 1
                       && (player.Y - Y) <= watchRange;
                break;
        }

        if (!inSight)
            return;

        World.InputBlocked = true;
        ChasePlayer(player);
        Converse(player);
        World.InputBlocked = false;
    }

    private void ChasePlayer(Player player)
    {
        while (Math.Abs(player.X - X) + Math.Abs(player.Y - Y) > 1)
        {
            Erase();
            if (player.X > X) X++;
            else if (player.X < X) X--;
            else if (player.Y > Y) Y++;
            else Y--;

            Draw();
            Thread.Sleep(40);
        }
    }
}