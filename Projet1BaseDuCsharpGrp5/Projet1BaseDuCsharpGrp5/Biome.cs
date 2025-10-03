using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projet1BaseDuCsharpGrp5
{
    public class Biome
    {
        public string Name { get; set; }
        public int LevelMin { get; set; }
        public int LevelMax { get; set; }
        public int ApparitionRate { get; set; } // 0..100
        public List<string> PokemonsNames { get; set; } = new();
    }
}
