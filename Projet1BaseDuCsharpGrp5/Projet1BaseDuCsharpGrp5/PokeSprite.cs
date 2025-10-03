using Projet1BaseDuCsharpGrp5;
using System.Drawing;
using System.Reflection;

public class PokeSprite
{
    public const int Width = 8;
    public const int Height = 8;

    // Chaque pixel est une Tile (color + isTransparent)
    public Tile[,] Pixels { get; }

    public PokeSprite(Tile[,] source)
    {
        if (source.GetLength(0) != Width ||
            source.GetLength(1) != Height)
        {
            throw new ArgumentException($"Le sprite doit faire {Width}×{Height}", nameof(source));
        }

        Pixels = (Tile[,])source.Clone();
    }

    // Indexeur pratique
    public Tile this[int x, int y]
    {
        get => Pixels[x, y];
        set => Pixels[x, y] = value;
    }
}

/*
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent }
*/

public static class PokemonSprites
{
    private static readonly IReadOnlyDictionary<string, PokeSprite> nameToSprite;

    static PokemonSprites()
    {
        var dict = new Dictionary<string, PokeSprite>(StringComparer.OrdinalIgnoreCase);
        var fields = typeof(PokemonSprites).GetFields(BindingFlags.Public | BindingFlags.Static);
        foreach (var f in fields)
        {
            if (f.FieldType == typeof(PokeSprite))
            {
                var value = f.GetValue(null) as PokeSprite;
                if (value != null)
                {
                    dict[f.Name] = value;
                }
            }
        }
        nameToSprite = dict;
    }

    public static PokeSprite GetByName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return Vierge;
        if (nameToSprite.TryGetValue(name, out var s)) return s;
        return Vierge;
    }

    // ─────────── Couleurs RGB de BULBI ───────────
    public static readonly Rgb BleuBulbi = new Rgb(58, 149, 152);
    public static readonly Rgb BleuclairBulbi = new Rgb(102, 210, 178);
    public static readonly Rgb VertBulbi = new Rgb(114, 171, 58);
    public static readonly Rgb RoseBulbi = new Rgb(249, 130, 136);
    public static readonly Rgb RougeBulbi = new Rgb(203, 70, 75);
    public static readonly Rgb JauneBulbi = new Rgb(248, 212, 14);

    public static readonly Tile Transparent = new Tile(color: new Rgb(0, 0, 0), isTransparent: true);
    private static Tile T(Rgb color) => new Tile(color);

    // ─────────── Couleurs RGB de SALA ───────────
    public static readonly Rgb OrangeSala = new Rgb(250, 103, 59);
    public static readonly Rgb BeigeSala = new Rgb(250, 216, 131);
    public static readonly Rgb JauneSala = new Rgb(231, 218, 44);
    public static readonly Rgb RougeSala = new Rgb(217, 41, 79);
    public static readonly Rgb VertSala = new Rgb(58, 151, 140);

    // ─────────── Couleurs RGB de CARA ───────────
    public static readonly Rgb ClairCara = new Rgb(189, 249, 239);
    public static readonly Rgb JauneCara = new Rgb(254, 216, 119);
    public static readonly Rgb TurqCara = new Rgb(89, 172, 152);
    public static readonly Rgb MarronCara = new Rgb(179, 107, 23);
    public static readonly Rgb VioCara = new Rgb(146, 138, 237);
    public static readonly Rgb GrisCara = new Rgb(203, 215, 231);
    public static readonly Rgb FonceCara = new Rgb(54, 112, 219);

    // ─────────── Couleurs RGB de Chenipan ───────────
    public static readonly Rgb Noir = new Rgb(0, 0, 0);
    public static readonly Rgb Shrek = new Rgb(166, 210, 73);
    public static readonly Rgb vioch = new Rgb(91, 74, 116);

    // ─────────── Couleurs RGB de Aspicot ───────────
    public static readonly Rgb beigeAsp = new Rgb(230, 172, 90);


    // ─────────── Couleurs RGB de Roucool ───────────
    public static readonly Rgb MarronF = new Rgb(146, 96, 96);

    // ─────────── Couleurs RGB de Ratata ───────────
    public static readonly Rgb vioRa = new Rgb(179, 124, 189);

    //-------MAELYS
    public static readonly Rgb rougetaupe = new Rgb(238, 39, 98);
    public static readonly Rgb marronctaupe = new Rgb(232, 165, 86);
    public static readonly Rgb marronftaupe = new Rgb(178, 109, 6);
    public static readonly Rgb taupe = new Rgb(158, 91, 100);


    //-------Racaillou
    public static readonly Rgb griscaillou = new Rgb(188, 179, 182);
    public static readonly Rgb grisFcaillou = new Rgb(132, 124, 122);
    public static readonly Rgb MarGrolem = new Rgb(144, 94, 103);

    // Sprite Famille Bulbizarre

    public static readonly PokeSprite Bulbizarre = new PokeSprite(new Tile[,]
    {
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, Transparent, T(VertBulbi), Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, T(VertBulbi), T(VertBulbi), T(VertBulbi), Transparent, Transparent },
        { Transparent, Transparent, T(BleuclairBulbi), T(BleuBulbi), T(VertBulbi), T(VertBulbi), Transparent, Transparent },
        { Transparent, Transparent, T(BleuBulbi), T(BleuBulbi), T(BleuBulbi), T(BleuBulbi), Transparent, Transparent },
        { Transparent, Transparent, Transparent, T(BleuBulbi), Transparent, T(BleuBulbi), Transparent, Transparent }
    });

    public static readonly PokeSprite Herbizarre = new PokeSprite(new Tile[,]
    {
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, Transparent, T(RoseBulbi), Transparent, Transparent, Transparent },
        { Transparent, Transparent, T(VertBulbi), T(VertBulbi), T(RougeBulbi), T(RoseBulbi), Transparent, Transparent },
        { Transparent, Transparent, T(BleuclairBulbi), T(BleuBulbi), T(VertBulbi), T(VertBulbi), Transparent, Transparent },
        { Transparent, Transparent, T(BleuBulbi), T(BleuBulbi), T(BleuBulbi), T(BleuBulbi), Transparent, Transparent },
        { Transparent, Transparent, Transparent, T(BleuBulbi), Transparent, T(BleuBulbi), Transparent, Transparent }
    });

    public static readonly PokeSprite Florizarre = new PokeSprite(new Tile[,]
    {
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, T(RougeBulbi), T(JauneBulbi), T(JauneBulbi), T(RougeBulbi), Transparent, Transparent },
        { T(VertBulbi), T(VertBulbi), T(VertBulbi), T(RoseBulbi), T(RoseBulbi), T(VertBulbi), Transparent, Transparent },
        { T(BleuclairBulbi), T(VertBulbi), T(BleuclairBulbi), T(VertBulbi), T(VertBulbi), T(VertBulbi), Transparent, Transparent },
        { T(BleuclairBulbi), T(BleuclairBulbi), T(BleuclairBulbi), T(BleuBulbi), T(BleuBulbi), T(BleuBulbi),  T(VertBulbi), Transparent },
        { Transparent, T(BleuBulbi), T(BleuBulbi), T(BleuBulbi), T(BleuBulbi), T(BleuBulbi), Transparent, Transparent },
        { Transparent, T(BleuBulbi), Transparent, T(BleuBulbi), Transparent, T(BleuBulbi), Transparent, Transparent }
    });

    // Famille Salamèche 

    public static readonly PokeSprite Salameche = new PokeSprite(new Tile[,]
    {
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, T(OrangeSala), T(OrangeSala), Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, T(OrangeSala), T(OrangeSala), Transparent, Transparent, Transparent },
        { Transparent, Transparent,T(OrangeSala), T(BeigeSala), T(OrangeSala), Transparent, T(JauneSala), Transparent },
        { Transparent, Transparent, Transparent, T(BeigeSala), T(OrangeSala), Transparent, T(OrangeSala), Transparent },
        { Transparent, Transparent, Transparent, T(OrangeSala), T(OrangeSala), T(OrangeSala), T(OrangeSala), Transparent }
    });

    public static readonly PokeSprite Reptincel = new PokeSprite(new Tile[,]
   {
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, Transparent, T(RougeSala), Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, T(RougeSala), T(RougeSala), Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, T(RougeSala), T(RougeSala), Transparent, Transparent, Transparent },
        { Transparent, Transparent,T(RougeSala) ,T(BeigeSala), T(RougeSala), Transparent, T(JauneSala), Transparent },
        { Transparent, Transparent, Transparent, T(BeigeSala), T(RougeSala), Transparent, T(RougeSala), Transparent },
        { Transparent, Transparent, Transparent, T(RougeSala), T(RougeSala), T(RougeSala), T(RougeSala), Transparent }
   });

    public static readonly PokeSprite Dracaufeu = new PokeSprite(new Tile[,]
   {
        { Transparent, Transparent, Transparent, Transparent, T(OrangeSala), Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, T(OrangeSala), Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, T(OrangeSala), T(OrangeSala), T(OrangeSala), Transparent, Transparent, Transparent },
        { Transparent, Transparent, T(OrangeSala), T(OrangeSala), T(OrangeSala), Transparent, Transparent, Transparent },
        { Transparent, T(OrangeSala), T(OrangeSala), T(BeigeSala), T(OrangeSala), T(OrangeSala), Transparent, Transparent },
        { Transparent, T(VertSala), T(OrangeSala), T(BeigeSala), T(OrangeSala), T(VertSala), T(JauneSala), Transparent },
        { Transparent, T(VertSala), T(OrangeSala), T(BeigeSala), T(OrangeSala), T(VertSala), T(OrangeSala), Transparent },
        { Transparent, Transparent, T(OrangeSala), T(OrangeSala), T(OrangeSala), T(OrangeSala), T(OrangeSala), Transparent }

   });

    // Famille Carapuce

    public static readonly PokeSprite Carapuce = new PokeSprite(new Tile[,]
    {
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, T(ClairCara), T(ClairCara), Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, T(ClairCara), T(ClairCara), Transparent, Transparent, Transparent },
        { Transparent, Transparent, T(ClairCara), T(JauneCara), T(ClairCara), T(MarronCara), Transparent, Transparent },
        { Transparent, Transparent, Transparent, T(JauneCara), T(JauneCara), T(MarronCara), Transparent, Transparent },
        { Transparent, Transparent, Transparent, T(TurqCara), T(TurqCara), T(ClairCara), Transparent, Transparent }

    });

    public static readonly PokeSprite Carabaffe = new PokeSprite(new Tile[,]
    {
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, T(GrisCara), Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, T(VioCara), T(VioCara), T(GrisCara), Transparent, Transparent },
        { Transparent, Transparent, Transparent, T(VioCara), T(VioCara), Transparent, Transparent, Transparent },
        { Transparent, Transparent, T(VioCara), T(JauneCara), T(VioCara), T(MarronCara), Transparent, Transparent },
        { Transparent, Transparent, Transparent, T(JauneCara), T(JauneCara), T(MarronCara), T(GrisCara), Transparent },
        { Transparent, Transparent, Transparent, T(VioCara), T(VioCara), T(GrisCara), T(GrisCara), Transparent }

    });

    public static readonly PokeSprite Tortank = new PokeSprite(new Tile[,]
    {
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, T(FonceCara), T(ClairCara), T(FonceCara), T(MarronCara), Transparent, Transparent },
        { Transparent, T(GrisCara), T(FonceCara), T(FonceCara), T(FonceCara), T(GrisCara), T(MarronCara), Transparent },
        { Transparent, T(FonceCara), T(JauneCara), T(JauneCara), T(JauneCara), T(FonceCara), T(MarronCara), Transparent },
        { Transparent, Transparent, T(JauneCara), T(JauneCara), T(JauneCara), T(JauneCara), T(MarronCara), Transparent },
        { Transparent, Transparent, T(FonceCara), T(JauneCara), T(JauneCara), T(FonceCara), Transparent, Transparent },
        { Transparent, Transparent, T(FonceCara), Transparent, Transparent, T(FonceCara), Transparent, Transparent }

   });

    // Famille Chenipan 

    public static readonly PokeSprite Chenipan = new PokeSprite(new Tile[,]
    {
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, T(RougeSala), Transparent, T(RougeSala), Transparent, Transparent },
        { Transparent, Transparent, Transparent, Transparent, T(RougeSala), T(VertBulbi), Transparent, Transparent },
        { Transparent, Transparent, Transparent, Transparent, T(VertBulbi), T(Noir), Transparent, Transparent },
        { Transparent, T(JauneCara), Transparent, T(VertBulbi), T(JauneCara), T(VertBulbi), Transparent, Transparent },
        { Transparent, Transparent, T(VertBulbi), T(JauneCara), T(VertBulbi), Transparent, Transparent, Transparent }
    });

    public static readonly PokeSprite Chrysacier = new PokeSprite(new Tile[,]
    {
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, T(Shrek), Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, T(Noir), T(Shrek), Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, T(VertBulbi), T(Shrek), T(Shrek), Transparent, Transparent, Transparent },
        { Transparent, Transparent, T(VertBulbi), T(VertBulbi), T(VertBulbi), T(Shrek), Transparent, Transparent },
        { Transparent, Transparent, Transparent, T(VertBulbi), T(VertBulbi), T(VertBulbi), T(Shrek), Transparent }
   });

    public static readonly PokeSprite Papillusion = new PokeSprite(new Tile[,]
    {
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, T(GrisCara), Transparent, T(GrisCara), Transparent, Transparent },
        { Transparent, Transparent, T(VioCara), T(RougeBulbi), T(GrisCara), T(GrisCara), Transparent, Transparent },
        { Transparent, Transparent, T(FonceCara), T(VioCara), T(vioch), T(GrisCara), Transparent, Transparent },
        { Transparent, Transparent, Transparent, T(vioch), T(vioch), T(GrisCara), Transparent, Transparent },
        { Transparent, Transparent, Transparent, T(FonceCara), T(FonceCara), Transparent, Transparent, Transparent }
   });

    // Famille Aspicot

    public static readonly PokeSprite Aspicot = new PokeSprite(new Tile[,]
    {
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, T(GrisCara), Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, T(beigeAsp), T(beigeAsp), Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, T(RoseBulbi), T(beigeAsp), Transparent, Transparent, T(GrisCara), Transparent, Transparent },
        { Transparent, Transparent, T(MarronCara), T(beigeAsp), Transparent, T(MarronCara), Transparent, Transparent },
        { Transparent, Transparent, Transparent, T(MarronCara), T(MarronCara), Transparent, Transparent, Transparent }
   });

    public static readonly PokeSprite Coconfort = new PokeSprite(new Tile[,]
    {
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, T(BeigeSala), T(BeigeSala), Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, T(Noir), T(BeigeSala), Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, T(beigeAsp), T(beigeAsp), T(beigeAsp), Transparent, Transparent },
        { Transparent, Transparent, Transparent, Transparent, T(beigeAsp), T(beigeAsp), Transparent, Transparent },
        { Transparent, Transparent, Transparent, Transparent, Transparent, T(beigeAsp), Transparent, Transparent }
   });

    public static readonly PokeSprite Dardagnan = new PokeSprite(new Tile[,]
    {
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, T(GrisCara), Transparent, T(GrisCara), Transparent, Transparent },
        { Transparent, Transparent, T(JauneSala), T(RougeSala), Transparent, T(GrisCara), Transparent, Transparent },
        { Transparent, Transparent, T(JauneSala), T(JauneSala), T(beigeAsp), T(GrisCara), T(GrisCara), Transparent },
        { Transparent, Transparent, T(Noir), T(beigeAsp), T(beigeAsp), T(Noir), Transparent, Transparent },
        { Transparent, T(GrisCara), T(GrisCara), T(Noir), T(Noir), T(GrisCara), Transparent, Transparent },
        { Transparent, Transparent, Transparent, T(JauneSala), T(JauneSala), Transparent, Transparent, Transparent }
   });

    // Famille Roucool

    public static readonly PokeSprite Roucool = new PokeSprite(new Tile[,]
    {
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, T(MarronCara), T(BeigeSala), Transparent, Transparent, Transparent },
        { Transparent, Transparent, T(MarronF), T(BeigeSala), T(MarronF), Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, T(BeigeSala), T(MarronCara), T(MarronCara), T(MarronF), Transparent },
        { Transparent, Transparent, Transparent, T(beigeAsp), T(beigeAsp), T(MarronF), Transparent, Transparent }
   });

    public static readonly PokeSprite Roucoups = new PokeSprite(new Tile[,]
    {
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, T(MarronCara), T(OrangeSala), Transparent, Transparent, Transparent, Transparent },
        { Transparent, T(MarronF), T(BeigeSala), T(MarronF), T(OrangeSala), Transparent, Transparent, Transparent },
        { Transparent, Transparent, T(BeigeSala), T(MarronCara), T(MarronCara), T(MarronF), Transparent, Transparent },
        { Transparent, Transparent, T(beigeAsp), T(beigeAsp), T(BeigeSala), T(BeigeSala), Transparent, Transparent },
        { Transparent, Transparent, T(MarronF), T(MarronF), T(OrangeSala), Transparent, Transparent, Transparent }
   });

    public static readonly PokeSprite Roucarnage = new PokeSprite(new Tile[,]
    {
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, T(beigeAsp), T(JauneSala), T(OrangeSala), Transparent, Transparent, Transparent },
        { Transparent, T(MarronF), T(BeigeSala), T(MarronF), Transparent, T(OrangeSala), Transparent, Transparent },
        { Transparent, Transparent, T(BeigeSala), T(MarronCara), T(MarronF), T(MarronF), Transparent, Transparent },
        { Transparent, Transparent, T(beigeAsp), T(beigeAsp), T(BeigeSala), T(BeigeSala), Transparent, Transparent },
        { Transparent, Transparent, T(MarronF), T(MarronF), T(OrangeSala), T(OrangeSala), Transparent, Transparent }
   });

    // Famille Ratata

    public static readonly PokeSprite Rattata = new PokeSprite(new Tile[,]
    {
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, T(vioRa), Transparent, T(vioRa), Transparent, Transparent, Transparent },
        { Transparent, Transparent, T(vioRa), T(vioRa), Transparent, Transparent, T(vioRa), Transparent },
        { Transparent, Transparent, T(BeigeSala), T(BeigeSala), T(vioRa), T(vioRa), Transparent, Transparent },
        { Transparent, Transparent, Transparent, T(vioRa), Transparent, T(vioRa), Transparent, Transparent }
   });

    public static readonly PokeSprite Ratatac = new PokeSprite(new Tile[,]
    {
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, Transparent, Transparent, T(MarronCara), Transparent, Transparent },
        { Transparent, Transparent, T(beigeAsp), T(beigeAsp), Transparent, Transparent, T(MarronCara), Transparent },
        { Transparent, T(BeigeSala), T(beigeAsp), T(BeigeSala), T(beigeAsp), Transparent, T(MarronCara), Transparent },
        { Transparent, T(GrisCara), T(BeigeSala), T(BeigeSala), T(beigeAsp), Transparent, T(MarronCara), Transparent },
        { Transparent, T(BeigeSala), T(BeigeSala), T(BeigeSala), T(beigeAsp), T(MarronCara), Transparent, Transparent },
        { Transparent, Transparent, T(BeigeSala), T(beigeAsp), T(beigeAsp), Transparent, Transparent, Transparent }
   });

    // Famille Piafabec 

    public static readonly PokeSprite Piafabec = new PokeSprite(new Tile[,]
    {
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, T(MarronF), T(MarronF), Transparent, Transparent, Transparent },
        { Transparent, Transparent, T(OrangeSala), T(MarronF), T(MarronF), Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, T(BeigeSala), T(RougeSala), T(RougeSala), T(MarronCara), Transparent },
        { Transparent, Transparent, Transparent, T(beigeAsp), T(beigeAsp), T(MarronF), Transparent, Transparent }
   });

    public static readonly PokeSprite Rapasdepik = new PokeSprite(new Tile[,]
    {
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, T(RougeSala), Transparent, Transparent, T(MarronCara), T(MarronCara), T(beigeAsp) },
        { Transparent, T(RougeSala), T(MarronCara), Transparent, Transparent, T(MarronCara), T(beigeAsp), Transparent },
        { T(OrangeSala), T(OrangeSala), T(MarronCara), T(MarronCara), T(MarronCara), T(beigeAsp), Transparent, Transparent },
        { Transparent, Transparent, Transparent, T(beigeAsp), T(MarronCara), Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, Transparent, T(MarronCara), T(MarronCara), Transparent, Transparent },
        { Transparent, Transparent, Transparent, T(RougeSala), Transparent, Transparent, T(MarronCara), Transparent }
   });
    public static readonly PokeSprite Taupiqueur = new PokeSprite(new Tile[,]
    {
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, T(taupe), T(taupe), Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, T(rougetaupe), T(taupe), Transparent, Transparent, Transparent },
        { Transparent, Transparent, T(marronctaupe), T(taupe), T(taupe), T(marronctaupe), Transparent, Transparent },
        { Transparent, Transparent, T(marronftaupe), T(marronftaupe), T(marronctaupe), T(marronftaupe), Transparent, Transparent }
    });

    public static readonly PokeSprite Triopiqueur = new PokeSprite(new Tile[,]
    {
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, T(taupe), T(taupe), Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, T(rougetaupe), T(taupe), Transparent, Transparent, Transparent },
        { T(taupe), T(taupe), Transparent, T(taupe), T(taupe), Transparent, T(taupe), T(taupe) },
        { T(rougetaupe), T(taupe), T(marronctaupe), T(taupe), T(taupe), Transparent, T(taupe), T(rougetaupe) },
        { T(taupe), T(taupe), T(marronftaupe), T(marronftaupe), T(marronctaupe), T(marronctaupe), T(taupe), T(taupe) },
        { T(marronftaupe), T(marronctaupe), T(marronftaupe), T(marronftaupe), T(marronctaupe), T(marronftaupe), T(marronftaupe), T(marronftaupe) }
    });

    public static readonly PokeSprite Racaillou = new PokeSprite(new Tile[,]
    {
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, T(griscaillou), Transparent, Transparent, Transparent, Transparent, T(griscaillou), Transparent },
        { Transparent, T(grisFcaillou), T(grisFcaillou), T(griscaillou), T(griscaillou), T(grisFcaillou), T(grisFcaillou), Transparent },
        { Transparent, Transparent, Transparent, T(griscaillou), T(griscaillou), Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent }
    });

    public static readonly PokeSprite Gravalanche = new PokeSprite(new Tile[,]
    {
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, T(griscaillou), Transparent, T(griscaillou), T(griscaillou), T(griscaillou), Transparent, T(griscaillou) },
        { Transparent, T(grisFcaillou), T(grisFcaillou), T(grisFcaillou), T(grisFcaillou), T(grisFcaillou), T(grisFcaillou), T(grisFcaillou) },
        { Transparent, Transparent, T(griscaillou), T(grisFcaillou), T(grisFcaillou), T(grisFcaillou), T(griscaillou), Transparent },
        { Transparent, Transparent, Transparent, T(grisFcaillou), Transparent, T(grisFcaillou), Transparent, Transparent }
    });

    public static readonly PokeSprite Grolem = new PokeSprite(new Tile[,]
    {
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, T(MarGrolem), T(MarGrolem), T(MarGrolem), T(MarGrolem), Transparent, Transparent },
        { Transparent, T(BeigeSala), T(MarGrolem), T(BeigeSala), T(BeigeSala), T(MarGrolem), T(BeigeSala), Transparent },
        { Transparent, T(MarGrolem), T(MarGrolem),  T(JauneCara),  T(JauneCara), T(MarGrolem), T(MarGrolem), Transparent },
        { Transparent, Transparent, T(MarGrolem), T(MarGrolem), T(MarGrolem), T(MarGrolem), Transparent, Transparent },
        { Transparent, Transparent, T(BeigeSala), Transparent, Transparent, T(BeigeSala), Transparent, Transparent }
    });

    public static readonly PokeSprite Mystherbe = new PokeSprite(new Tile[,]
    {
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, T(VertBulbi), Transparent, Transparent, T(VertBulbi), Transparent, Transparent },
        { Transparent, Transparent, T(VertBulbi), T(VertBulbi), T(VertBulbi), T(VertBulbi), Transparent, Transparent },
        { Transparent, Transparent, Transparent, T(VertBulbi), T(VertBulbi), Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, T(FonceCara), T(FonceCara), Transparent, Transparent, Transparent },
        { Transparent, Transparent, T(FonceCara), Transparent, T(FonceCara), Transparent, Transparent, Transparent }
   });

    public static readonly PokeSprite Ortide = new PokeSprite(new Tile[,]
    {
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, T(MarronCara), Transparent, Transparent, Transparent, Transparent },
        { T(OrangeSala), Transparent, T(MarronCara), T(MarronF), T(MarronCara), Transparent, T(OrangeSala), Transparent },
        { Transparent, T(OrangeSala), T(MarronF), T(MarronCara), T(MarronF), T(OrangeSala), Transparent, Transparent },
        { Transparent, T(OrangeSala), T(vioRa), T(vioRa), T(vioRa), T(OrangeSala), Transparent, Transparent },
        { T(OrangeSala), Transparent, T(VioCara), T(VioCara), T(vioRa), Transparent, T(OrangeSala), Transparent },
        { Transparent, Transparent, T(VioCara), Transparent, T(vioRa), Transparent, Transparent, Transparent }
   });

    public static readonly PokeSprite Raflesia = new PokeSprite(new Tile[,]
    {
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, T(OrangeSala), T(OrangeSala), T(RougeBulbi), T(RougeBulbi), T(OrangeSala), T(RougeBulbi), Transparent },
        { T(OrangeSala), T(OrangeSala), T(RougeBulbi), T(JauneSala), T(JauneSala), T(OrangeSala), T(OrangeSala), T(RougeBulbi) },
        { T(OrangeSala), T(OrangeSala), T(OrangeSala), T(RougeBulbi), T(OrangeSala), T(OrangeSala), T(OrangeSala), T(RougeBulbi) },
        { Transparent, T(RougeBulbi), T(RougeBulbi), T(RougeBulbi), T(RougeBulbi), T(RougeBulbi), T(RougeBulbi), Transparent },
        { Transparent, Transparent, T(FonceCara), T(FonceCara), T(FonceCara), T(FonceCara), Transparent, Transparent },
        { Transparent, Transparent, Transparent, T(FonceCara), T(FonceCara), Transparent, Transparent, Transparent },
        { Transparent, Transparent, T(FonceCara), Transparent, T(FonceCara), Transparent, Transparent, Transparent }
   });




    public static readonly PokeSprite Vierge = new PokeSprite(new Tile[,]
    {
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent },
        { Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent, Transparent }
    });
    public static readonly IReadOnlyList<PokeSprite> All = new[]
    {
        Bulbizarre,
        Herbizarre,
        Florizarre,
        Salameche,
        Reptincel,
        Dracaufeu,
        Carapuce,
        Carabaffe,
        Tortank,
        Chenipan,
        Chrysacier,
        Papillusion,
        Aspicot,
        Coconfort,
        Dardagnan,
        Roucool,
        Roucoups,
        Roucarnage,
        Rattata,
        Ratatac,
        Piafabec,
        Rapasdepik,
        Taupiqueur,
        Triopiqueur,
        Racaillou,
        Gravalanche,
        Grolem,
        Mystherbe,
        Ortide,
        Raflesia
    };
}
