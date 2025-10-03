using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Projet1BaseDuCsharpGrp5
{
    public class Pokemon
    {
        public string Name { get; set; }
        public PokemonType Type { get; set; }
        public int Level { get; set; }
        public int Experience { get; set; }
        public int MaxLife { get; set; }
        public int Life { get; set; }
        public int MaxMana { get; set; }
        public int Mana { get; set; }
        public int BaseDamageAttack { get; set; }
        public int DamageAttack { get; set; }
        public int BaseSpeedAttack { get; set; }
        public int SpeedAttack { get; set; }
        public int BaseDefense { get; set; }
        public int Defense { get; set; }
        public bool HasEvolution { get; set; }

        [JsonPropertyName("Competences")]
        public List<string> CompetenceNames { get; set; } = new List<string>();

        [JsonIgnore]
        public List<Competence> Competences { get; set; } = new List<Competence>();

        public Pokemon() { } // Obligatoire pour la désérialisation

        public void OnInitPokemon()
        {
            InitBasicCompetences();

            int baseLife = (MaxLife > 0) ? MaxLife : Life;
            MaxLife = LifeByLevel(baseLife, Level);
            Life = MaxLife;
            int baseMana = (MaxMana > 0) ? MaxMana : Mana;
            MaxMana = ManaByLevel(baseMana, Level);
            Mana = MaxMana;
            int baseDamage = (BaseDamageAttack > 0) ? BaseDamageAttack : DamageAttack;
            BaseDamageAttack = OtherStatByLevel(baseDamage, Level);
            DamageAttack = BaseDamageAttack;
            int baseSpeed = (BaseSpeedAttack > 0) ? BaseSpeedAttack : SpeedAttack;
            BaseSpeedAttack = OtherStatByLevel(baseSpeed, Level);
            SpeedAttack = BaseSpeedAttack;
            int baseDef = (BaseDefense > 0) ? BaseDefense : Defense;
            BaseDefense = OtherStatByLevel(baseDef, Level);
            Defense = BaseDefense;
        }

        private void InitBasicCompetences()
        {
            // Guard
            if (Competences == null) Competences = new List<Competence>();

            Competences.Clear();

            foreach (var name in CompetenceNames)
            {
                if (string.IsNullOrWhiteSpace(name)) continue;

                var comp = World.DataJson.CreateCompetence(name);
                if (comp == null) continue;
                Competences.Add(comp);
            }

        }
        public List<Competence> AddCompetences(string _name)
        {
            var comp = World.DataJson.CreateCompetence(_name);

            if (!Competences.Any(c => c.Name == comp.Name))
            {
                Competences.Add(comp);
            }
            return Competences;
        }

        public void TakeDamage(int value)
        {
            Life -= value;
            if (Life < 0)
                Life = 0;
        }
        public void RestoreLife(int value)
        {
            Life += value;
            if (Life > MaxLife)
                Life = MaxLife;
        }
        public void RestoreAllLife()
        {
            Life = MaxLife;
        }

        public void RestoreMidLife()
        {
            Life = MaxLife / 2;
        }

        public bool HasEnoughMana(Competence comp)
        {
            if (comp == null) return false;
            return Mana >= comp.ManaCost;   
        }

        public void ConsumeMana(int value)
        {
            Mana -= value;
            if (Mana < 0)
                Mana = 0;
        }
        public void RestoreMana(int value)
        {
            Mana += value;
            if (Mana > MaxMana)
                Mana = MaxMana;
        }
        public void RestoreAllMana()
        {
            Mana = MaxMana;
        }

        public void BoostDamageAttack(int value)
        {
            DamageAttack += value;
        }

        public void BoostSpeedAtack(int value)
        {
            SpeedAttack += value;
        }

        public void BoostDefense(int value)
        {
            Defense += value;
        }

        public void ResetCombatStats()
        {
            DamageAttack = BaseDamageAttack;
            SpeedAttack = BaseSpeedAttack;
            Defense = BaseDefense;
        }
        
        public int LifeByLevel(int hp, int level)
        {
            return (int)Math.Floor((2.0 * hp * level) / 100 + level + 10);
        }

        public int ManaByLevel(int mana, int level)
        {
            return (int)Math.Floor((2.0 * mana * level) / 100 + level + 40);
        }

        public int OtherStatByLevel(int stat, int level)
        {
            return (int)Math.Floor((2.0 * stat * level) / 100 + 5);
        }

        public int CalculateDamage(Pokemon attacker, Competence comp, Pokemon defender, Random rng)
        {
            // Guard
            if (attacker == null || comp == null || defender == null) return 0;
            if (rng == null) rng = new Random();

            // V1 
            //double mult = TypeMultiplier.GetMultiplier(comp.Type, defender.Type);
            //double damageTake = (attacker.DamageAttack + comp.DamageAttack) * mult - defender.Defense;
            //return Math.Max(0, (int)System.Math.Round(damageTake));

            // V2 officiel pokemon :

            // Valeurs de base
            int level = Math.Max(1, attacker.Level);
            int power = Math.Max(1, comp.DamageAttack);
            double atk = Math.Max(1, attacker.DamageAttack);
            double def = Math.Max(1, defender.Defense);

            // Étapes "floor" similaires au vrai pokemon
            double step1 = Math.Floor(2.0 * level / 5.0 + 2.0);
            double step2 = Math.Floor(step1 * power * atk / def);
            double baseDamage = Math.Floor(step2 / 50.0) + 2.0;

            // Modificateurs
            double modifier = 1.0;

            // Critique
            bool isCritical = (rng.Next(0, 16) == 0); // une chance sur 16 de CC
            if (isCritical) modifier *= 1.5;

            // Random
            int randPercent = rng.Next(85, 101);
            modifier *= randPercent / 100.0;

            // Bonus si type d'attaque et type de l'ataquant sont les mêmes
            if (comp.Type == attacker.Type) modifier *= 1.5;

            // gestion type
            modifier *= TypeMultiplier.GetMultiplier(comp.Type, defender.Type);

            double total = Math.Floor(baseDamage * modifier);

            int finalDamage = Math.Max(1, (int)total);

            return finalDamage;
        }

        public void LevelUp()
        {
            Experience -= Level * 5;
            Level++;
        }

        public void CheckExperience()
        {
            if(Experience > Level * 5)
            {
                LevelUp();
            }
        }

        public void GainExperience(int exp)
        {
            Experience += exp;
            CheckExperience();
        }
    }
}