using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projet1BaseDuCsharpGrp5
{
    public class Competence
    {
        public string Name { get; }
        public PokemonType Type { get; }
        public int ManaCost { get; set; }
        public int DamageAttack { get; set; }
        public int SpeedAttack { get; set; }

        public Competence(string name, PokemonType type, int manaCost, int damageAttack, int speedAttack)
        {
            Name = name;
            Type = type;
            ManaCost = manaCost;
            DamageAttack = damageAttack;
            SpeedAttack = speedAttack;
        }
    }
}
