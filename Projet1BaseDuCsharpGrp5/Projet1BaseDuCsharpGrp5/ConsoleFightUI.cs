using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projet1BaseDuCsharpGrp5
{
    public class ConsoleFightUI : IFightUI
    {
        public void Update(Pokemon player, Pokemon enemy)
        {
            Console.WriteLine("----- STATUS -----");
            Console.WriteLine($"{player.Name} Lv{player.Level} PV:{player.Life}/{player.MaxLife} MANA:{player.Mana}/{player.MaxMana}");
            Console.WriteLine($"{enemy.Name}  Lv{enemy.Level} PV:{enemy.Life}/{enemy.MaxLife} MANA:{enemy.Mana}/{enemy.MaxMana}");
            Console.WriteLine("------------------");
        }

        public void ShowMessage(string message)
        {
            Console.WriteLine(message);
            Console.WriteLine("Appuyez sur une touche pour continuer...");
            Console.ReadKey(true);
        }

        public int ChooseFromList(string title, List<string> options)
        {
            Console.WriteLine(title);
            for (int i = 0; i < options.Count; i++)
                Console.WriteLine($"{i + 1}) {options[i]}");
            Console.WriteLine("0) retour");

            while (true)
            {
                var input = Console.ReadLine();
                if (input == "0") return -1;
                if (int.TryParse(input, out int choice) && choice >= 1 && choice <= options.Count)
                    return choice - 1;
                Console.WriteLine($"Entrez un numéro valide (0..{options.Count})");
            }
        }
    }
}
