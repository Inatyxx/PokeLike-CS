using Projet1BaseDuCsharpGrp5;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

public class Dresseur
{
    public int X { get; set; }
    public int Y { get; set; }
    public int PokeDollars { get; set; }

    [JsonIgnore]
    public Rgb Color { get; set; }
    public string Name { get; set; }

    public List<Item> Items { get; set; }
    public List<Pokemon> TeamPokemons { get; set; }
    

    public Dresseur(string name, int startX, int startY, Rgb color, int pokeDollars = 0)
    {

        Name = name;
        X = startX;
        Y = startY;
        Color = color;
        PokeDollars = pokeDollars;

        Items = new List<Item>();
        TeamPokemons = new List<Pokemon>();
    }

public Dresseur() { }
    public void SetPosition(int x, int y)
    {
        X = x;
        Y = y;
    }

    public void ShowCoordinates()
    {
        Console.SetCursorPosition(0, Console.WindowHeight - 1);
        Ansi.Reset();
        Trace.WriteLine($"{Name} Pos : [{X}|{Y}]");
    }

    public virtual void Draw()
    {
        Map.UpdateOffsets();
        if (!Map.GridToScreen(X, Y, out int sx, out int sy)) return;

        string pixel = new string('█', Map.PixelWidth);
        try
        {
            Console.SetCursorPosition(sx, sy);
            Ansi.WriteFg(Color);
            Console.Write(pixel);
            Ansi.Reset();
        }
        catch { }
    }

    public virtual void Erase()
    {
        Map.UpdateOffsets();
        if (!Map.GridToScreen(X, Y, out int sx, out int sy)) return;

        Rgb bg = Map.GetColorCell(X, Y);
        string pixel = new string(' ', Map.PixelWidth);
        try
        {
            Console.SetCursorPosition(sx, sy);
            Ansi.WriteBg(bg);
            Console.Write(pixel);
            Ansi.Reset();
        }
        catch { }
    }

    public virtual List<Pokemon> AddPokemon(string _name, int _level = 1) 
    {
        // Guard
        if (_name == null) throw new ArgumentNullException(nameof(_name));

        var pokemon = World.DataJson.CreatePokemon(_name);

        pokemon.Level = _level;
        pokemon.OnInitPokemon();

        if (TeamPokemons.Count < 6)
        {
            TeamPokemons.Add(pokemon);
        }
        else
        {
            Console.WriteLine($"{Name} ne peut pas recevoir {pokemon.Name} : équipe pleine.");
        }
        return TeamPokemons;
    }

    public List<Item> AddItem(string _name)
    {
        // Guard
        if (_name == null) throw new ArgumentNullException(nameof(_name));

        var item = World.DataJson.CreateItem(_name);

        for (int i = 0; i < Items.Count; i++)
        {
            if (Items[i].Name == item.Name)
            {
                Items[i].Quantity += item.Quantity;
                return Items;
            }
        }
            
        Items.Add(item);
        return Items;
    }

    public void UseItem(Item item)
    {
        if (item.Quantity > 1)
        {
            item.Quantity -= 1;
            return;
        }
        Items.Remove(item);
    }
}   

