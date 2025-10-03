using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Projet1BaseDuCsharpGrp5
{
    public class Json
    {
        // L'ideal serait d'avoir ca pour stocker les listes et n'avoir a les appelé qu'une seul fois. 
        // Parce que actuellement on deserialize le JSON a chaque fois qu'on créer un pokemon
        private List<Pokemon> _cachedPokemons;
        private List<Competence> _cachedCompetences;
        private List<Item> _cachedItems;
        private List<Biome> _cachedBiomes;

        public int SlotNumber { get; set; }
        public List<Pokemon> ListPokemon()
        {
            var basePath = AppContext.BaseDirectory;
            var projectRoot = Path.GetFullPath(Path.Combine(basePath, @"..\..\..\..\.."));
            var pokJsonPath = Path.Combine(projectRoot, "Projet1BaseDuCsharpGrp5", "Pokemon.json");

            var jsonPok = File.ReadAllText(pokJsonPath);
            var listPokemons = JsonSerializer.Deserialize<List<Pokemon>>(jsonPok);

            return listPokemons;
        }

        public List<Competence> ListCompetence()
        {
            var basePath = AppContext.BaseDirectory;
            var projectRoot = Path.GetFullPath(Path.Combine(basePath, @"..\..\..\..\.."));
            var competencesJsonPath = Path.Combine(projectRoot, "Projet1BaseDuCsharpGrp5", "Competences.json");

            var jsonComp = File.ReadAllText(competencesJsonPath);
            var listCompetences = JsonSerializer.Deserialize<List<Competence>>(jsonComp);

            return listCompetences;
        }

        public List<Item> ListItems()
        {
            var basePath = AppContext.BaseDirectory;
            var projectRoot = Path.GetFullPath(Path.Combine(basePath, @"..\..\..\..\.."));
            var itemsJsonPath = Path.Combine(projectRoot, "Projet1BaseDuCsharpGrp5", "Item.json");

            var jsonItem = File.ReadAllText(itemsJsonPath);
            var listItems = JsonSerializer.Deserialize<List<Item>>(jsonItem);

            return listItems;
        }

        public List<Biome> ListBiomes()
        {
            var basePath = AppContext.BaseDirectory;
            var projectRoot = Path.GetFullPath(Path.Combine(basePath, @"..\..\..\..\.."));
            var biomesJsonPath = Path.Combine(projectRoot, "Projet1BaseDuCsharpGrp5", "Biomes.json");

            var jsonBiomes = File.ReadAllText(biomesJsonPath);
            var listBiomes = JsonSerializer.Deserialize<List<Biome>>(jsonBiomes);

            return listBiomes;
        }

        public Pokemon CreatePokemon(string name)
        {
            // Guard 
            if (string.IsNullOrWhiteSpace(name)) return null;

            var pokemons = ListPokemon();
            var pokemon = pokemons.First(p => p.Name == name);
            pokemon.OnInitPokemon();
            return pokemon;
        }

        public Competence CreateCompetence(string name)
        {
            // Guard 
            if (string.IsNullOrWhiteSpace(name)) return null;

            var competences = ListCompetence();
            var competence = competences.First(c => c.Name == name);
            return competence;
        }

        public Item CreateItem(string name)
        {
            // Guard 
            if (string.IsNullOrWhiteSpace(name)) return null;

            var items = ListItems();
            var item = items.First(p => p.Name == name);
            //item.OnInitItem();
            return item;
        }
    }
}
