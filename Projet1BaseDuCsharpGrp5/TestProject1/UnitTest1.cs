using NUnit.Framework;
using Projet1BaseDuCsharpGrp5;
using System.Linq;
using System.Collections.Generic;

namespace TestProject1
{
    public class Tests
    {
        private List<Pokemon> pokemons;
        private List<Competence> competences;

        [SetUp]
        public void Setup()
        {
            var json = new Json();
            pokemons = json.ListPokemon();
            competences = json.ListCompetence();
        }

        [Test]
        public void AddCompetences_ShouldAddCompetenceToList()
        {
            var competence = competences.First(c => c.Name == "Charge");
            var pokemon = pokemons.First(p => p.Name == "Tortank");
            pokemon.AddCompetences("Charge");
            var result = pokemon.AddCompetences("Charge");
            

            Assert.That(result.Any(c => c.Name == competence.Name), Is.True);
            Assert.That(result.Count, Is.EqualTo(1));
        }

       

        [Test]
        public void Competence_Constructor_ShouldSetProperties()
        {
            var competence = competences.First(c => c.Name == "Tonnerre");

            Assert.That(competence.Name, Is.EqualTo("Tonnerre"));
            Assert.That(competence.Type, Is.EqualTo(PokemonType.Electric));
            Assert.That(competence.ManaCost, Is.EqualTo(15));
            Assert.That(competence.DamageAttack, Is.EqualTo(90));
        }

        [Test]
        public void Pokemon_TakeDamage()
        {
            var fire = pokemons.First(p => p.Name == "Salameche");
            fire.OnInitPokemon();
            var water = pokemons.First(p => p.Name == "Carapuce");
            water.OnInitPokemon();

            fire.TakeDamage(4);
            water.TakeDamage(30);

            Assert.That(fire.Life, Is.EqualTo(7));
            Assert.That(water.Life, Is.EqualTo(0));
        }

        [Test]
        public void Pokemon_RestoreLife()
        {
            var fire = pokemons.First(p => p.Name == "Salameche");
            fire.OnInitPokemon();

            fire.TakeDamage(4);
            Assert.That(fire.Life, Is.EqualTo(7));

            fire.RestoreLife(2);
            Assert.That(fire.Life, Is.EqualTo(9));
        }

        [Test]
        public void Pokemon_RestoreAllLife()
        {
            var fire = pokemons.First(p => p.Name == "Salameche");
            fire.TakeDamage(50);
            fire.RestoreAllLife();

            Assert.That(fire.Life, Is.EqualTo(fire.MaxLife));
        }

        [Test]
        public void Pokemon_ConsumeMana()
        {
            var fire = pokemons.First(p => p.Name == "Salameche");
            fire.ConsumeMana(10);

            Assert.That(fire.Mana, Is.EqualTo(29));

            fire.ConsumeMana(100); // Exceeds

            Assert.That(fire.Mana, Is.EqualTo(0));
        }

        [Test]
        public void Pokemon_RestoreMana()
        {
            var fire = pokemons.First(p => p.Name == "Salameche");
            fire.OnInitPokemon();

            fire.ConsumeMana(2);
            fire.RestoreMana(1);
            Assert.That(fire.Mana, Is.EqualTo(10));

            fire.ConsumeMana(100);
            fire.RestoreMana(4);
            Assert.That(fire.Mana, Is.EqualTo(4));
        }

        [Test]
        public void Pokemon_RestoreAllMana()
        {
            var fire = pokemons.First(p => p.Name == "Salameche");
            fire.ConsumeMana(50);
            fire.RestoreAllMana();

            Assert.That(fire.Mana, Is.EqualTo(fire.MaxMana));
        }

        [Test]
        public void Player_AddPokeDollars()
        {
            var player = new Player("Bruno", new Rgb(0, 0, 255));
            player.AddPokeDollars(200);

            Assert.That(player.PokeDollars, Is.EqualTo(200));
        }

        [Test]
        public void Player_LosePokeDollars()
        {
            var bruno = new Player("Bruno", new Rgb(0, 0, 255));
            var maxime = new Player("Maxime", new Rgb(0, 0, 255));

            bruno.AddPokeDollars(200);
            maxime.AddPokeDollars(200);

            bruno.LosePokeDollars(100);
            maxime.LosePokeDollars(250);

            Assert.That(bruno.PokeDollars, Is.EqualTo(100));
            Assert.That(maxime.PokeDollars, Is.EqualTo(0));
        }

        [Test]
        public void Pnj_Reward_ShouldBeSetAndGet()
        {
            var json = new Json();
            var pnj = new Pnj("Bob");
            var potion = json.CreateItem("Potion de soin");

            pnj._reward = potion;

            Assert.That(pnj._reward.Name, Is.EqualTo("Potion de soin"));
        }

        [Test]
        public void AddCompetences_ShouldNotAddDuplicate()
        {
            var pikachu = pokemons.First(p => p.Name == "Pikachu");

            pikachu.AddCompetences("Tonnerre");
            int compNumberFirst = pikachu.Competences.Count;
            pikachu.AddCompetences("Tonnerre");
            int compNumberSecond = pikachu.Competences.Count;

            Assert.That(compNumberFirst, Is.EqualTo(compNumberSecond));
        }

        [Test]
        public void TakeDamage_ShouldNotGoBelowZero()
        {
            var pikachu = pokemons.First(p => p.Name == "Pikachu");
            pikachu.TakeDamage(999);

            Assert.That(pikachu.Life, Is.EqualTo(0));
        }

        [Test]
        public void RestoreLife_ShouldNotExceedMaxLife()
        {
            var pikachu = pokemons.First(p => p.Name == "Pikachu");
            pikachu.RestoreLife(999);

            Assert.That(pikachu.Life, Is.EqualTo(pikachu.MaxLife));
        }

        [Test]
        public void ConsumeMana_ShouldNotGoBelowZero()
        {
            var pikachu = pokemons.First(p => p.Name == "Pikachu");
            pikachu.ConsumeMana(999);

            Assert.That(pikachu.Mana, Is.EqualTo(0));
        }

        [Test]
        public void RestoreMana_ShouldNotExceedMaxMana()
        {
            var pikachu = pokemons.First(p => p.Name == "Pikachu");
            pikachu.RestoreMana(999);

            Assert.That(pikachu.Mana, Is.EqualTo(pikachu.MaxMana));
        }
    }
}
