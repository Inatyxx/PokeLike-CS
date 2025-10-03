using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projet1BaseDuCsharpGrp5
{
    public enum PokemonType
    {
        Normal = 0,
        Fire = 1,
        Water = 2,
        Grass = 3,
        Electric = 4,
        Ground = 5,
        Fighting = 6

        //Ice,,
        //Poison,
        //Flying,
        //Psychic,
        //Bug,
        //Rock,
        //Ghost,
        //Dragon,
        //Dark,
        //Steel,
        //Fairy,
    }

    public static class TypeMultiplier
    {
        private static readonly double[,] multipliers = new double[,]
        {
            // Defender:          Normal Fire Water Grass  Elec Ground Fighting
            /*Attacker Normal  */ { 1.0, 1.0,  1.0,  1.0,  1.0,   1.0,   1.0 },
            /*Attacker Fire    */ { 1.0, 0.5,  0.5,  2.0,  1.0,   1.0,   1.0 },
            /*Attacker Water   */ { 1.0, 2.0,  0.5,  0.5,  1.0,   2.0,   1.0 },
            /*Attacker Grass   */ { 1.0, 0.5,  2.0,  0.5,  1.0,   0.5,   1.0 },
            /*Attacker Electric*/ { 1.0, 1.0,  2.0,  0.5,  0.5,   0.0,   1.0 },
            /*Attacker Ground  */ { 1.0, 2.0,  1.0,  2.0,  2.0,   1.0,   1.0 },
            /*Attacker Fighting*/ { 2.0, 1.0,  1.0,  1.0,  1.0,   1.0,   1.0 },
        };

        public static double GetMultiplier(PokemonType attacker, PokemonType defender)
        {
            int a = (int)attacker;
            int d = (int)defender;

            // Si la matrice n'a pas la bonne taille, on retourne 1 :
            if (a < 0 || d < 0
                || a >= multipliers.GetLength(0)
                || d >= multipliers.GetLength(1))
            {
                Console.WriteLine("Matrice mal dimensionnée !");
                return 1.0;
            }

            return multipliers[a, d];
        }
    }
}
