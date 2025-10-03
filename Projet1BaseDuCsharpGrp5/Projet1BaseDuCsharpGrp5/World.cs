using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Projet1BaseDuCsharpGrp5
{
    public readonly struct Tile
    {
        public Rgb Color { get; }
        public bool IsWall { get; }
        public bool IsGrass { get; }
        public bool IsTransparent { get; }

        public Tile(Rgb color, bool isWall = false, bool isGrass = false, bool isTransparent = false)
        {
            Color = color;
            IsWall = isWall;
            IsGrass = isGrass;
            IsTransparent = isTransparent;
        }
    }
    public static class World
    {
        public static Json DataJson;
        public static Random Rng = new Random();
        public static int Width => CurrentMap.Width;
        public static int Height => CurrentMap.Height;
        public static int PixelWidth => Map.PixelWidth;
        public static bool InputBlocked { get; set; } = false;
        private static readonly Dictionary<string, Map> maps = new();
        private static readonly Dictionary<string, Biome> Biomes = new();
        public static Map CurrentMap { get; private set; }
        public static Player player;
        public static List<Pnj> Pnjs { get; private set; }
        public static Dictionary<string, Pnj> PnjsByName { get; private set; }

        public static void Initialize(Json json)
        {
            DataJson = json;

            // CHARGER LES BIOMES
            var biomesList = DataJson.ListBiomes() ?? new List<Biome>();
            Biomes.Clear();
            foreach (var biome in biomesList)
            {
                if (string.IsNullOrWhiteSpace(biome.Name)) continue;
                Biomes[biome.Name] = biome;
            }

            InitializeAllPnjAndPokemon();
            InitializeMap();
        }

        private static void InitializeAllPnjAndPokemon()
        {
            Pnjs = new List<Pnj>();
            PnjsByName = new Dictionary<string, Pnj>();

            player = new Player(null, new Rgb(255, 30, 30), 1, 1);
            for (int i=0; i < 50; i++)
            {
                player.AddItem("Pokeball");
            }
            player.AddItem("Hyperball");
            player.AddItem("Hyperball");
            player.AddItem("Hyperball");
            player.AddItem("Potion de soin");
            player.AddItem("Potion de soin");
            player.AddItem("RappelMax");
            player.AddItem("RappelMax");
            player.AddItem("RappelMax");

            var pnj1Jean = new Pnj(
                name: "Jean",
                personality: Pnj.PersonalityState.Pacifiste,
                startX: 44,
                startY: 31,
                color: new Rgb(50, 50, 255),
                firstDialogue: "Salut ! Aujourd'hui est un jour spécial… C’est le jour de choisir ton tout premier Pokémon. Va voir le Professeur Chen dans son laboratoir, il t’attend en bas au bout du chemin.",
                postDefeatDialogue: "Alors, t'as choisi ton pokemon ?!"
                );
            pnj1Jean.AddPokemon("Salameche");
            pnj1Jean.AddPokemon("Bulbizarre");
            AddPnjToLists(pnj1Jean);

            var pnjMartin = new Pnj(
                name: "Martin",
                personality: Pnj.PersonalityState.Pacifiste,
                startX: 32,
                startY: 19,
                color: new Rgb(200, 0, 200),   // vert amical
                firstDialogue: "Salut ! Tu vois ce bâtiment juste devant toi ? C’est le Centre Pokémon. Tu peux y faire soigner tous tes Pokémon gratuitement.",
                postDefeatDialogue: "N’hésite pas à y retourner si tes Pokémon ont besoin de repos avant de continuer ton aventure !"
            );
            AddPnjToLists(pnjMartin);

            var PROFCHEN = new Pnj(
                name: "Professeur Chen",
                personality: Pnj.PersonalityState.ProfChen,
                startX: 5,
                startY: 5,
                color: new Rgb(255, 255, 255),
                firstDialogue: "Ah ! Te voilà ! Bienvenue dans mon laboratoire. Aujourd’hui est un grand jour : tu vas recevoir ton tout premier Pokémon. Traite-le avec respect et il deviendra ton plus fidèle allié.",
                postDefeatDialogue: "Oh, tu es déjà de retour ? Dégage t'en a déja eu un ! Continue d’apprendre et tu deviendras un grand maître Pokémon !"
            );

            AddPnjToLists(PROFCHEN);

            var pnj2Bandit = new Pnj(
                name: "Chasseur D'Insecte",
                startX: 45,
                startY: 35,
                color: new Rgb(80, 130, 200),           // rouge sombre
                pokeDollars: 22,
                firstDialogue: "Hé ! Que fais-tu dans mon territoire ? Prépare-toi à un combat !",
                defeatDialogue: "Argh… Tu m’as eu cette fois…",
                postDefeatDialogue: "Tu crois que ça va s’arrêter là ? Je reviendrai plus fort !",
                isAggressive: true,                            // passe en mode aggro
                watchDirection: Direction.Down,               // regarde vers le bas
                watchRange: 10
            );

            pnj2Bandit.AddPokemon("Aspicot", 2);
            pnj2Bandit.AddPokemon("Chenipan", 3);
            AddPnjToLists(pnj2Bandit);

            var pnjForestRanger = new Pnj(
                name: "Ranger de la Forêt",
                startX: 63,
                startY: 18,
                color: new Rgb(30, 150, 70),
                pokeDollars: 18,
                firstDialogue: "C'est ma forêt ! Fais attention aux insectes sauvages. 😊",
                defeatDialogue: "Pas mal… Tu apprends vite !",
                postDefeatDialogue: "Rappelle-toi, la forêt est pleine de surprises. Reviens t’entraîner !",
                isAggressive: true,                    // n’attaque pas à vue
                watchDirection: Direction.Left,           // regarde vers le haut
                watchRange: 20
            );
            pnjForestRanger.AddPokemon("Chenipan", 2);
            pnjForestRanger.AddPokemon("Roucool", 4);
            AddPnjToLists(pnjForestRanger);

            // ─── PNJ 1 : l’éclaireur sylvestre ───────────────────────────────────────
            var pnjForestScout = new Pnj(
                name: "Éclaireur Sylvestre",
                startX: 35,
                startY: 30,
                color: new Rgb(80, 200, 120),
                pokeDollars: 12,
                firstDialogue: "J'explore ces bois depuis des années. Un duel te tente ?",
                defeatDialogue: "Bien joué ! Tu manies bien ton équipe.",
                postDefeatDialogue: "Reviens quand tu voudras tester d'autres sentiers.",
                isAggressive: false,
                watchDirection: Direction.Down,
                watchRange: 15
            );
            pnjForestScout.AddPokemon("Chenipan", 3);
            pnjForestScout.AddPokemon("Aspicot", 3);
            AddPnjToLists(pnjForestScout);


            // ─── PNJ 2 : le cueilleur de baies ───────────────────────────────────────
            var pnjBerryForager = new Pnj(
                name: "Cueilleur de Baies",
                startX: 45,
                startY: 16,
                color: new Rgb(200, 100, 50),
                pokeDollars: 10,
                firstDialogue: "Je ramasse des baies fraîches ici. Prêt pour un combat rapide ?",
                defeatDialogue: "Oh, tu m'as pris par surprise. Bravo !",
                postDefeatDialogue: "Reviens après ta pause, j'aurai d'autres baies à te montrer.",
                isAggressive: false,
                watchDirection: Direction.Right,
                watchRange: 10
            );
            pnjBerryForager.AddPokemon("Rattata", 4);
            pnjBerryForager.AddPokemon("Roucool", 2);
            AddPnjToLists(pnjBerryForager);


            // ─── PNJ 3 : le chasseur sylvestre ───────────────────────────────────────
            var pnjForestHunter = new Pnj(
                name: "Chasseur Sylvestre",
                startX: 70,
                startY: 15,
                color: new Rgb(120, 80, 40),
                pokeDollars: 20,
                firstDialogue: "Les proies sauvages se cachent dans l'ombre. Montre ton talent !",
                defeatDialogue: "Ils n'ont pas résisté longtemps. Impressionnant.",
                postDefeatDialogue: "Reviens chercher un vrai challenge quand tu seras prêt.",
                isAggressive: true,
                watchDirection: Direction.Left,
                watchRange: 10
            );
            pnjForestHunter.AddPokemon("Dardagnan", 7);
            AddPnjToLists(pnjForestHunter);


            // ─── PNJ 1 : Explorateur de Grotte ───────────────────────────────────────
            var pnjCaveExplorer = new Pnj(
                name: "Explorateur de Grotte",
                startX: 1,
                startY: 40,
                color: new Rgb(180, 180, 200),
                pokeDollars: 15,
                firstDialogue: "Les ténèbres ici sont fascinantes, jeune dresseur.",
                defeatDialogue: "Tu as survécu aux ombres, chapeau !",
                postDefeatDialogue: "Reviens explorer ces profondeurs quand tu voudras.",
                isAggressive: true,
                watchDirection: Direction.Right,
                watchRange: 20
            );
            pnjCaveExplorer.AddPokemon("Racaillou", 4);
            pnjCaveExplorer.AddPokemon("Taupiqueur", 5);
            AddPnjToLists(pnjCaveExplorer);


            // ─── PNJ 2 : Mineur souterrain ────────────────────────────────────────────
            var pnjCaveMiner = new Pnj(
                name: "Mineur Souterrain",
                startX: 18,
                startY: 25,
                color: new Rgb(150, 100, 50),
                pokeDollars: 12,
                firstDialogue: "J'extrais du minerai précieux depuis des lustres ! Prêt à t'affronter ?",
                defeatDialogue: "Ton talent m'a délogé de ma pioche.",
                postDefeatDialogue: "Reviens quand tu auras trouvé d'autres cristaux.",
                isAggressive: true,
                watchDirection: Direction.Left,
                watchRange: 20
            );
            pnjCaveMiner.AddPokemon("Triopiqueur", 9);
            AddPnjToLists(pnjCaveMiner);


            // ─── PNJ 3 : Garde des Cavernes ──────────────────────────────────────────
            var pnjCaveGuard = new Pnj(
                name: "Homme des Cavernes",
                startX: 0,
                startY: 10,
                color: new Rgb(100, 100, 180),
                pokeDollars: 20,
                firstDialogue: "J'adore les cailloux !!! Regarde mes cailloux !!!",
                defeatDialogue: ":( cailloux morts",
                postDefeatDialogue: ":( cailloux morts",
                isAggressive: true,
                watchDirection: Direction.Right,
                watchRange: 10
            );
            pnjCaveGuard.AddPokemon("Racaillou", 1);
            pnjCaveGuard.AddPokemon("Racaillou", 2);
            pnjCaveGuard.AddPokemon("Racaillou", 3);
            pnjCaveGuard.AddPokemon("Racaillou", 4);
            AddPnjToLists(pnjCaveGuard);

            // PNJ : Maître des Plantes
            var pnjarene1 = new Pnj(
                name: "Maître des Plantes",
                startX: 5,
                startY: 3,
                color: new Rgb(34, 139, 34),   // vert forêt
                pokeDollars: 20,
                firstDialogue: "Les plantes sont mes alliées. Prêt pour une bataille verte ? 🌿",
                defeatDialogue: "Aïe ! Mes pousses ont flétri sous ton talent…",
                postDefeatDialogue: "Reviens quand tes compétences auront germé.",
                isAggressive: false,
                watchDirection: Direction.Right,
                watchRange: 10
            );

            // (tu peux remplacer ces Bulbizarre/Mystherbe etc. par tes sprites de plante)
            pnjarene1.AddPokemon("Raflesia", 7);
            pnjarene1.AddPokemon("Dardagnan", 8);
            pnjarene1.AddPokemon("Florizarre", 15);

            AddPnjToLists(pnjarene1);


            var pnjHealer = new Pnj(
                name: "Infirmière Joel",
                startX: 9,
                startY: 3,
                color: new Rgb(255, 182, 193),   
                firstDialogue: "Bonjour ! Je vais soigner tes Pokémon...." +
                "Tes pokemons sont maintenant soignés !",
                defeatDialogue: "",
                postDefeatDialogue: "",
                personality: Pnj.PersonalityState.JoySan
                );
            AddPnjToLists(pnjHealer);
        }

        private static void InitializeMap()
        {
            //############################################################################# LA PLAINE #################################
            var plain = new Map("LaPlaine", 100, 50, "Plains");
            Map.SetCentering(plain, true);
            plain.Fill((x, y) => new Tile(new Rgb(139, 203, 35), isGrass: false));

            var HouseMainCharachter = new House(
                layout: Decors.SmallHouse,
                exteriorDoorX: 3, exteriorDoorY: 5,
                interiorMapName: "HouseInterior",
                interiorSpawnX: 9, interiorSpawnY: 10,
                interiorDoorX: 9, interiorDoorY: 11,
                exteriorMapName: "LaPlaine",
                exteriorSpawnX: 20, exteriorSpawnY: 15
            );
            var Labo = new House(
                layout: Decors.Laboratory,
                exteriorDoorX: 4, exteriorDoorY: 6,
                interiorMapName: "Laboratory",
                interiorSpawnX: 15, interiorSpawnY: 10,
                interiorDoorX: 15, interiorDoorY: 11,
                exteriorMapName: "LaPlaine",
                exteriorSpawnX: 35, exteriorSpawnY: 37
            );

            plain.AddDecor(HouseMainCharachter.Layout, 20, 15);
            plain.AddDecor(Labo.Layout, 35, 37);

            plain.AddDamier(new Tile(new Rgb(0, 128, 149), isWall: true), new Tile(new Rgb(1, 99, 223), isWall:true), 80, 0, 20, 60);
            plain.addPath(Decors.Sand, 77, 0, 3, 60);
            plain.addPath(Decors.Sand, 68, 40, 12, 60);
            plain.AddDamier(new Tile(new Rgb(0, 128, 149), isWall: true), new Tile(new Rgb(1, 99, 223), isWall: true), 71, 43, 9, 7);
            plain.addPath(Decors.TownPath, 22, 21, 3, 6);
            plain.addPath(Decors.TownPath, 22, 27, 40, 3);
            plain.addPath(Decors.TownPath, 46, 27, 3, 17);
            plain.addPath(Decors.TownPath, 38, 44, 11, 2); //labo path
            //foret de gauche (wow)
            plain.AddDecor(Decors.Tree, 3, 0);
            plain.AddDecor(Decors.LightTree, 0, 2);
            plain.AddDecor(Decors.DarkTree, 7, 4);
            plain.AddDecor(Decors.Tree, 1, 6);
            plain.AddDecor(Decors.LightTree, 9, 8);
            plain.AddDecor(Decors.DarkTree, 2, 10);
            plain.AddDecor(Decors.Tree, 6, 12);
            plain.AddDecor(Decors.LightTree, 4, 14);
            plain.AddDecor(Decors.DarkTree, 10, 16);
            plain.AddDecor(Decors.Tree, 0, 18);
            plain.AddDecor(Decors.LightTree, 5, 20);
            plain.AddDecor(Decors.DarkTree, 8, 22);
            plain.AddDecor(Decors.Tree, 3, 24);
            plain.AddDecor(Decors.LightTree, 7, 26);
            plain.AddDecor(Decors.DarkTree, 1, 28);
            plain.AddDecor(Decors.Tree, 9, 30);
            plain.AddDecor(Decors.LightTree, 2, 32);
            plain.AddDecor(Decors.DarkTree, 6, 34);
            plain.AddDecor(Decors.Tree, 0, 36);
            plain.AddDecor(Decors.LightTree, 4, 38);
            plain.AddDecor(Decors.DarkTree, 8, 40);
            plain.AddDecor(Decors.Tree, 5, 42);
            plain.AddDecor(Decors.LightTree, 10, 44);
            plain.AddDecor(Decors.DarkTree, 3, 46);
            plain.AddDecor(Decors.Tree, 7, 48);
            plain.AddDecor(Decors.LightTree, 1, 50);
            //foret de haut (alaide)
            plain.AddDecor(Decors.Tree, 9, 0);
            plain.AddDecor(Decors.LightTree, 12, 0);
            plain.AddDecor(Decors.DarkTree, 15, 1);
            plain.AddDecor(Decors.Tree, 18, 0);
            plain.AddDecor(Decors.LightTree, 21, 1);
            plain.AddDecor(Decors.DarkTree, 24, 0);
            plain.AddDecor(Decors.Tree, 27, 0);
            plain.AddDecor(Decors.LightTree, 30, 1);
            plain.AddDecor(Decors.DarkTree, 33, 0);
            plain.AddDecor(Decors.Tree, 36, 0);
            plain.AddDecor(Decors.LightTree, 39, 1);
            plain.AddDecor(Decors.DarkTree, 42, 0);
            plain.AddDecor(Decors.Tree, 45, 1);
            plain.AddDecor(Decors.LightTree, 48, 1);
            plain.AddDecor(Decors.DarkTree, 51, 0);
            plain.AddDecor(Decors.Tree, 54, 1);
            plain.AddDecor(Decors.LightTree, 57, 0);


            plain.addPath(Decors.TownPath, 60, 0, 3, 30); //labo path

            plain.AddTransitionZone(
                x: 60,
                y: 0,
                width: 10,
                height: 1,
                targetMap: "LaPlaine2",
                spawnX: 60,
                spawnY: 49
            );

            plain.AddTransition(
                x: HouseMainCharachter.ExteriorSpawnX,
                y: HouseMainCharachter.ExteriorSpawnY,
                targetMap: HouseMainCharachter.InteriorMapName,
                spawnX: HouseMainCharachter.InteriorSpawnX,
                spawnY: HouseMainCharachter.InteriorSpawnY
            );
            plain.AddTransition(
                x: Labo.ExteriorSpawnX,
                y: Labo.ExteriorSpawnY,
                targetMap: Labo.InteriorMapName,
                spawnX: Labo.InteriorSpawnX,
                spawnY: Labo.InteriorSpawnY
            );
            plain.AddDresseur(PnjsByName["Jean"]);

            maps[plain.Name] = plain;

            //############################################################################# MAISON PERSO #################################
            var interior = new Map("HouseInterior", 20, 12);
            Map.SetCentering(interior, true);

            interior.Fill((x, y) =>
            {
                bool wall = x == 0 || y == 0 || x == interior.Width - 1 || y == interior.Height - 1;
                return new Tile(
                    color: wall ? new Rgb(100, 100, 100) : new Rgb(200, 180, 150),
                    isWall: wall
                );
            });

            int doorX = HouseMainCharachter.InteriorDoorX;
            int doorY = HouseMainCharachter.InteriorDoorY;
            var original = interior.Tiles[doorX, doorY];
            interior.Tiles[doorX, doorY] = new Tile(
                color: new Rgb(139, 69, 19),
                isWall: false,
                isGrass: original.IsGrass
            );
            interior.AddTransition(
                x: doorX,
                y: doorY,
                targetMap: HouseMainCharachter.ExteriorMapName,
                spawnX: HouseMainCharachter.ExteriorSpawnX,
                spawnY: HouseMainCharachter.ExteriorSpawnY
            );

            maps[interior.Name] = interior;

            //############################################################################# MAISON CHEN #################################
            var Laboratory = new Map("Laboratory", 25, 12);
            Map.SetCentering(Laboratory, true);

            Laboratory.Fill((x, y) =>
            {
                bool wall = x == 0 || y == 0 || x == Laboratory.Width - 1 || y == Laboratory.Height - 1;
                return new Tile(
                    color: wall ? new Rgb(30, 30, 30) : new Rgb(60, 60, 60),
                    isWall: wall
                );
            });

            int labodoorX = Labo.InteriorDoorX;
            int labodoorY = Labo.InteriorDoorY;
            var ab = Laboratory.Tiles[labodoorX, labodoorY];
            Laboratory.Tiles[labodoorX, labodoorY] = new Tile(
                color: new Rgb(200,200,200),
                isWall: false
            );
            Laboratory.AddTransition(
                x: labodoorX,
                y: labodoorY,
                targetMap: Labo.ExteriorMapName,
                spawnX: Labo.ExteriorSpawnX,
                spawnY: Labo.ExteriorSpawnY
            );

            Laboratory.AddDresseur(PnjsByName["Professeur Chen"]);

            maps[Laboratory.Name] = Laboratory;

            //############################################################################# LA PLAINE 2 #################################
            var plain2 = new Map("LaPlaine2", 100, 50, "Plains");
            Map.SetCentering(plain2, true);
            plain2.Fill((x, y) => new Tile(new Rgb(139, 203, 35), isGrass: false));

            plain2.addPath(Decors.TownPath, 59, 35, 3, 15);
            plain2.AddDamier(new Tile(new Rgb(102, 177, 11), isGrass: true), new Tile(new Rgb(66, 139, 16), isGrass: true), 50, 26, 20, 10);
            plain2.addPath(Decors.Sand, 77, 20, 30, 30);
            plain2.AddDamier(new Tile(new Rgb(0, 128, 149), isWall: true), new Tile(new Rgb(1, 99, 223), isWall: true), 80, 23, 20, 60);
            plain2.addPath(Decors.TownPath, 59, 20, 3, 6);
            plain2.addPath(Decors.TownPath, 39, 17, 23, 3);
            plain2.AddDamier(new Tile(new Rgb(102, 177, 11), isGrass: true), new Tile(new Rgb(66, 139, 16), isGrass: true), 34, 14, 5, 9);
            plain2.AddDamier(new Tile(new Rgb(102, 177, 11), isGrass: true), new Tile(new Rgb(66, 139, 16), isGrass: true), 30, 30, 8, 8);
            plain2.AddDamier(new Tile(new Rgb(102, 177, 11), isGrass: true), new Tile(new Rgb(66, 139, 16), isGrass: true), 0, 0, 70, 12);
            plain2.addPath(Decors.TownPath, 0, 17, 34, 3);

            plain2.AddDecor(Decors.LightTree, 50, 45);
            plain2.AddDecor(Decors.Tree, 47, 44);
            plain2.AddDecor(Decors.LightTree, 44, 45);
            plain2.AddDecor(Decors.DarkTree, 41, 45);
            plain2.AddDecor(Decors.DarkTree, 38, 44);
            plain2.AddDecor(Decors.LightTree, 35, 45);
            plain2.AddDecor(Decors.DarkTree, 32, 44);
            plain2.AddDecor(Decors.LightTree, 29, 44);
            plain2.AddDecor(Decors.DarkTree, 26, 45);
            plain2.AddDecor(Decors.LightTree, 23, 45);
            plain2.AddDecor(Decors.DarkTree, 20, 45);
            plain2.AddDecor(Decors.Tree, 2, 21);
            plain2.AddDecor(Decors.LightTree, 6, 22);
            plain2.AddDecor(Decors.LightTree, 12, 23);
            plain2.AddDecor(Decors.DarkTree, 18, 24);
            plain2.AddDecor(Decors.LightTree, 4, 25);
            plain2.AddDecor(Decors.DarkTree, 10, 26);
            plain2.AddDecor(Decors.Tree, 15, 27);
            plain2.AddDecor(Decors.LightTree, 1, 28);
            plain2.AddDecor(Decors.DarkTree, 8, 29);
            plain2.AddDecor(Decors.LightTree, 14, 30);
            plain2.AddDecor(Decors.LightTree, 19, 31);
            plain2.AddDecor(Decors.LightTree, 5, 32);
            plain2.AddDecor(Decors.Tree, 11, 33);
            plain2.AddDecor(Decors.LightTree, 3, 34);
            plain2.AddDecor(Decors.DarkTree, 17, 35);
            plain2.AddDecor(Decors.Tree, 7, 36);
            plain2.AddDecor(Decors.DarkTree, 13, 37);
            plain2.AddDecor(Decors.DarkTree, 0, 38);
            plain2.AddDecor(Decors.Tree, 9, 39);
            plain2.AddDecor(Decors.DarkTree, 16, 40);
            plain2.AddDecor(Decors.DarkTree, 2, 41);
            plain2.AddDecor(Decors.LightTree, 12, 42);
            plain2.AddDecor(Decors.LightTree, 18, 43);
            plain2.AddDecor(Decors.DarkTree, 6, 44);
            plain2.AddDecor(Decors.Tree, 4, 46);
            plain2.AddDecor(Decors.LightTree, 10, 47);
            plain2.AddDecor(Decors.DarkTree, 15, 48);
            plain2.AddDecor(Decors.Tree, 1, 49);
            plain2.AddDecor(Decors.DarkTree, 8, 50);

            plain2.AddDecor(Decors.DarkTree, 74, 0);
            plain2.AddDecor(Decors.LightTree, 72, 4);
            plain2.AddDecor(Decors.DarkTree, 73, 9);
            plain2.AddDecor(Decors.Tree, 73, 12);
            plain2.AddDecor(Decors.LightTree, 74, 16);
            plain2.AddDecor(Decors.DarkTree, 72, 20);
            plain2.AddDecor(Decors.Tree, 73, 25);
            plain2.AddDecor(Decors.DarkTree, 72, 28);
            plain2.AddDecor(Decors.LightTree, 73, 31);
            plain2.AddDecor(Decors.DarkTree, 73, 36);
            plain2.AddDecor(Decors.LightTree, 74, 41);
            plain2.AddDecor(Decors.DarkTree, 73, 46);
            plain2.AddDecor(Decors.DarkTree, 80, 5);
            plain2.AddDecor(Decors.LightTree, 90, 12);
            plain2.AddDecor(Decors.DarkTree, 85, 13);
            plain2.AddDecor(Decors.LightTree, 86, 3);

            plain2.AddTransitionZone(
                x: 0,
                y: 14,
                width: 1,
                height: 5,
                targetMap: "Croisement",
                spawnX: 49,
                spawnY: 18
            );
            plain2.AddTransitionZone(
                x: 59,
                y: 49,
                width: 5,
                height: 1,
                targetMap: "LaPlaine",
                spawnX: 61,
                spawnY: 0
            );

            plain2.AddDresseur(PnjsByName["Chasseur D'Insecte"]);
            plain2.AddDresseur(PnjsByName["Ranger de la Forêt"]);
            maps[plain2.Name] = plain2;

            //############################################################################# Croisement #################################
            var Croisement = new Map("Croisement", 50, 30, "FlowerField");
            Map.SetCentering(Croisement, true);
            Croisement.Fill((x, y) => new Tile(new Rgb(139, 203, 35), isGrass: false));
            maps[Croisement.Name] = Croisement;

            Croisement.addPath(Decors.TownPath, 0, 17, 50, 3);
            Croisement.addPath(Decors.TownPath, 24, 14, 3, 4);
            Croisement.AddDecor(Decors.HealPost, 21, 6);

            for (int x = 1; x <= 48; x += 4)
            {
                Croisement.AddDecor(Decors.LightTree, x, 24);
            }

            var HealStation = new House(
                layout: Decors.HealPost,
                exteriorDoorX: 4, exteriorDoorY: 7,
                interiorMapName: "healmap",
                interiorSpawnX: 9, interiorSpawnY: 10,
                interiorDoorX: 9, interiorDoorY: 11,
                exteriorMapName: "Croisement",
                exteriorSpawnX: 21, exteriorSpawnY: 6
            );

            Croisement.AddTransitionZone(
                x: 49,
                y: 14,
                width: 1,
                height: 7,
                targetMap: "LaPlaine2",
                spawnX: 0,
                spawnY: 18
            );

            Croisement.AddTransitionZone(
                x: 0,
                y: 14,
                width: 1,
                height: 7,
                targetMap: "Foret",
                spawnX: 99,
                spawnY: 32
            );
            Croisement.AddTransition(
                x: HealStation.ExteriorSpawnX,
                y: HealStation.ExteriorSpawnY,
                targetMap: HealStation.InteriorMapName,
                spawnX: HealStation.InteriorSpawnX,
                spawnY: HealStation.InteriorSpawnY
            );

            Croisement.AddDresseur(PnjsByName["Martin"]);

            //############################################################################# MAISON HEAL #################################
            var HEALZONE = new Map("healmap", 19, 12);
            Map.SetCentering(HEALZONE, true);

            HEALZONE.Fill((x, y) =>
            {
                bool wall = x == 0 || y == 0 || x == HEALZONE.Width - 1 || y == HEALZONE.Height - 1;
                return new Tile(
                    color: wall ? new Rgb(128, 0, 0) : new Rgb(104, 41, 0),
                    isWall: wall
                );
            });

            int healdoorX = HealStation.InteriorDoorX;
            int healdoorY = HealStation.InteriorDoorY;
            var ac = HEALZONE.Tiles[healdoorX, healdoorY];
            HEALZONE.Tiles[healdoorX, healdoorY] = new Tile(
                color: new Rgb(200, 200, 200),
                isWall: false
            );
            HEALZONE.AddTransition(
                x: healdoorX,
                y: healdoorY,
                targetMap: HealStation.ExteriorMapName,
                spawnX: HealStation.ExteriorSpawnX,
                spawnY: HealStation.ExteriorSpawnY
            );

            HEALZONE.AddDresseur(PnjsByName["Infirmière Joel"]);

            maps[HEALZONE.Name] = HEALZONE;

            //############################################################################# Foret #################################
            var Forest = new Map("Foret", 100, 50, "Forest");
            Map.SetCentering(Forest, true);
            Forest.Fill((x, y) => new Tile(new Rgb(139, 203, 35), isGrass: false));

            Forest.addPath(Decors.ForestPath3, 80, 31, 20, 3);
            Forest.addPath(Decors.ForestPath3, 80, 10, 3, 21);
            Forest.addPath(Decors.ForestPath3, 50, 10, 30, 3);
            Forest.addPath(Decors.ForestPath3, 50, 10, 3, 25);
            Forest.addPath(Decors.ForestPath3, 20, 35, 33, 3);
            Forest.addPath(Decors.ForestPath3, 20, 5, 3, 30);
            Forest.AddDecor(Decors.Grotte, 17, 0);
            Forest.AddDamier(new Tile(new Rgb(102, 177, 11), isGrass: true), new Tile(new Rgb(66, 139, 16), isGrass: true), 23, 15, 27, 20);
            Forest.AddDamier(new Tile(new Rgb(102, 177, 11), isGrass: true), new Tile(new Rgb(66, 139, 16), isGrass: true), 53, 13, 27, 4);
            Forest.AddDamier(new Tile(new Rgb(102, 177, 11), isGrass: true), new Tile(new Rgb(66, 139, 16), isGrass: true), 83, 29, 20, 2);

            var baseGrass = new Rgb(139, 203, 35);
            var rng = new Random(12345);

            for (int x = 0; x < Forest.Width; x++)
            {
                for (int y = 0; y < Forest.Height; y++)
                {
                    if (!Forest.Tiles[x, y].Color.Equals(baseGrass))
                        continue;

                    bool zonePropre = true;
                    for (int dx = -1; dx <= 1 && zonePropre; dx++)
                        for (int dy = -0; dy <= 1; dy++)
                        {
                            int nx = x + dx, ny = y + dy;
                            if (nx < 0 || ny < 0 || nx >= Forest.Width || ny >= Forest.Height)
                                continue;
                            if (!Forest.Tiles[nx, ny].Color.Equals(baseGrass))
                            {
                                zonePropre = false;
                                break;
                            }
                        }

                    if (!zonePropre)
                        continue;

                    switch (rng.Next(1, 4))
                    {
                        case 1: Forest.AddDecor(Decors.LightTree, x, y); break;
                        case 2: Forest.AddDecor(Decors.DarkTree, x, y); break;
                        case 3: Forest.AddDecor(Decors.Tree, x, y); break;
                    }
                }
            }
            Forest.AddDresseur(PnjsByName["Éclaireur Sylvestre"]);
            Forest.AddDresseur(PnjsByName["Cueilleur de Baies"]);
            Forest.AddDresseur(PnjsByName["Chasseur Sylvestre"]);

            Forest.AddTransitionZone(
                x: 21,
                y: 4,
                width: 1,
                height: 1,
                targetMap: "grotte",
                spawnX: 10,
                spawnY: 49
            );
            Forest.AddTransitionZone(
                x: 99,
                y: 32,
                width: 1,
                height: 5,
                targetMap: "Croisement",
                spawnX: 0,
                spawnY: 18
            );
            maps[Forest.Name] = Forest;

            //############################################################################# CAVE #################################
            var Cave = new Map("grotte", 20, 50, "Cave");
            Map.SetCentering(Cave, true);
            Cave.Fill((x, y) => new Tile(new Rgb(100, 100, 100), isGrass: true));

            Cave.AddDecor(Decors.Rock, 2, 4);
            Cave.AddDecor(Decors.Rock, 2, 20);
            Cave.AddDecor(Decors.Rock, 13, 12);
            Cave.AddDecor(Decors.Rock, 9, 14);
            Cave.AddDecor(Decors.Rock, 3, 30);
            Cave.AddDecor(Decors.Rock, 15, 28);
            Cave.AddDecor(Decors.Rock, 6, 37);

            Cave.AddTransitionZone(
                x: 0,
                y: 49,
                width: 20,
                height: 1,
                targetMap: "Foret",
                spawnX: 21,
                spawnY: 4
            );
            Cave.AddTransitionZone(
                x: 0,
                y: 0,
                width: 20,
                height: 1,
                targetMap: "field",
                spawnX: 20,
                spawnY: 39
            );

            Cave.AddDresseur(PnjsByName["Explorateur de Grotte"]);
            Cave.AddDresseur(PnjsByName["Mineur Souterrain"]);
            Cave.AddDresseur(PnjsByName["Homme des Cavernes"]);

            maps[Cave.Name] = Cave;

            //############################################################################# Chsmpa #################################
            var Champs = new Map("field", 40, 40, "FlowerField");
            Map.SetCentering(Champs, true);
            Champs.Fill((x, y) => new Tile(new Rgb(139, 203, 35), isGrass: false));

            Champs.addPath(Decors.ForestPath3, 19, 20, 3, 20);

            Champs.AddDamier(new Tile(new Rgb(102, 177, 11), isGrass: true), new Tile(new Rgb(66, 139, 16), isGrass: true), 5, 25, 10, 10);
            Champs.AddDamier(new Tile(new Rgb(102, 177, 11), isGrass: true), new Tile(new Rgb(66, 139, 16), isGrass: true), 26, 25, 10, 10);

            Champs.AddTransitionZone(
                x: 19,
                y: 39,
                width: 3,
                height: 1,
                targetMap: "grotte",
                spawnX: 9,
                spawnY: 0
            );

            var ArenePLANTE = new House(
                layout: Decors.ARENE1,
                exteriorDoorX: 4, exteriorDoorY: 7,
                interiorMapName: "arenplante",
                interiorSpawnX: 5, interiorSpawnY: 14,
                interiorDoorX: 5, interiorDoorY: 14,
                exteriorMapName: "field",
                exteriorSpawnX: 16, exteriorSpawnY: 15
            );
            Champs.AddDecor(ArenePLANTE.Layout, 16, 15);

            Champs.AddTransition(
                x: ArenePLANTE.ExteriorSpawnX,
                y: ArenePLANTE.ExteriorSpawnY,
                targetMap: ArenePLANTE.InteriorMapName,
                spawnX: ArenePLANTE.InteriorSpawnX,
                spawnY: ArenePLANTE.InteriorSpawnY
            );

            maps[Champs.Name] = Champs;

            //############################################################################# MAISON ARENE #################################
            var FIGHTZONE = new Map("arenplante", 11, 15);
            Map.SetCentering(FIGHTZONE, true);

            FIGHTZONE.Fill((x, y) =>
            {
                bool wall = x == 0
                         || y == 0
                         || x == FIGHTZONE.Width - 1
                         || y == FIGHTZONE.Height - 1;

                return new Tile(
                    color: wall
                        ? new Rgb(0, 100, 0)   // vert foncé pour le contour
                        : new Rgb(144, 238, 144), // vert clair pour le sol
                    isWall: wall
                );
            });

            int arendoorX = ArenePLANTE.InteriorDoorX;
            int arendoorY = ArenePLANTE.InteriorDoorY;
            var ad = FIGHTZONE.Tiles[arendoorX, arendoorY];
            FIGHTZONE.Tiles[arendoorX, arendoorY] = new Tile(
                color: new Rgb(200, 200, 200),
                isWall: false
            );
            FIGHTZONE.AddTransition(
                x: arendoorX,
                y: arendoorY,
                targetMap: ArenePLANTE.ExteriorMapName,
                spawnX: ArenePLANTE.ExteriorSpawnX,
                spawnY: ArenePLANTE.ExteriorSpawnY
            );

            FIGHTZONE.AddDresseur(PnjsByName["Maître des Plantes"]);

            maps[FIGHTZONE.Name] = FIGHTZONE;


            LoadGame(DataJson.SlotNumber);

            ChangeMap("HouseInterior", 9, 5);
        }



































        public static void ChangeMap(string name)
        {
            if (!maps.TryGetValue(name, out var map))
                throw new ArgumentException($"Map introuvable : {name}");

            CurrentMap = map;

            Console.Clear();

            player.SetPosition(player.X, player.Y);

            Run();
        }
        public static void ChangeMap(string name, int spawnX = 1, int spawnY = 1)
        {
            if (!maps.TryGetValue(name, out var map))
                throw new ArgumentException($"Map introuvable : {name}");

            CurrentMap = map;
            Console.Clear();
            player.SetPosition(spawnX, spawnY);

            Run();
        }

        public static void SaveGame(int slot)
        {
            ClientSerialization.SaveGame(slot, player, Pnjs, CurrentMap.Name);
        }

        public static void LoadGame(int slot)
        {
            var save = ClientSerialization.LoadGame(slot);
            if (save == null)
            {
                Console.WriteLine("Aucune sauvegarde trouvée.");
                return;
            }

            
            player = save.Player;

            foreach (var pnjSave in save.Pnjs)
            {
                var pnj = Pnjs.Find(p => p.Name == pnjSave.Name);
                if (pnj != null)
                {
                    pnj.X = pnjSave.X;
                    pnj.Y = pnjSave.Y;
                    pnj.CurrentState = pnjSave.CurrentState;
                }
            }

            ChangeMap(save.CurrentMap, player.X, player.Y);
        }

        public static void Run()
        {
            Console.CursorVisible = false;
            CurrentMap.Render();
            player.Draw();
            if ( player.Name == null )
            {
                MenuManager.OpenOverlayCentered(new NameEntryOverlay());
            }


            while (true)
            {
                var key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.M)
                {
                    MenuManager.OpenFullScreen(new MainMenu());
                    break;
                }
                if (key == ConsoleKey.P)
                {
                    MenuManager.OpenOverlayCentered(
                        new SpriteViewerMenu(PokemonSprites.All.ToList())
                    );
                    continue;
                }
                if (key == ConsoleKey.I || key == ConsoleKey.Tab)
                {
                    var pokemons = World.player.AllPokemon
                        .Select(p => $"{p.Name} Lv{p.Level}")
                        .ToList();

                    var items = World.player.Items
                        .Select(i => $"{i.Name} x{i.Quantity}")
                        .ToList();

                    MenuManager.OpenOverlayCentered(new InventoryMenu(pokemons, items));
                    continue;
                }

                if (key == ConsoleKey.E)
                    player.Interact();    // nouvelle méthode
                else
                    if (!InputBlocked) { player.Move(key); }

                if (key == ConsoleKey.K) 
                {
                    SaveGame(DataJson.SlotNumber);
                    var pnj3SaveGuy = new Pnj(
                        name: "Système"
                        );
                    Dialogue.Show(pnj3SaveGuy, "Partie Sauvegardé !");
                    continue;
                }

                if (key == ConsoleKey.Y)
                {
                    var playerActive = World.player.TeamPokemons?.FirstOrDefault(p => p.Life > 0) ?? World.player.TeamPokemons?.FirstOrDefault();
                    var pnj = PnjsByName["Chasseur Sylvestre"].TeamPokemons?.FirstOrDefault(p => p.Life > 0) ?? World.player.TeamPokemons?.FirstOrDefault();

                    var playerSprite = PokemonSprites.GetByName(playerActive?.Name ?? "Vierge");
                    var enemySprite = PokemonSprites.GetByName(pnj?.Name ?? "Vierge");

                    var arena = new FightArena(playerSprite, enemySprite);
                    var ui = new FightArenaUI(arena); // Visuel
                    var uiConsole = new ConsoleFightUI(); // Console
                    var fm = new FightManager(World.player, ui);

                    var wild = World.GetRandomWildFromMap(World.CurrentMap);

                    //fm.StartWildBattle(wild);
                    fm.StartPnjBattle(PnjsByName["Chasseur Sylvestre"]);

                    //MenuManager.OpenFullScreen(new FightArena(PokemonSprites.Florizarre, PokemonSprites.Florizarre));
                    continue;
                }
            }
        }
        public static Biome GetBiomeForMap(Map map)
        {
            if (map == null || string.IsNullOrEmpty(map.BiomeName)) return null;
            var key = map.BiomeName;
            Biomes.TryGetValue(key, out var biome);
            return biome;
        }

        public static Pokemon GetRandomWildFromMap(Map map)
        {
            var biome = GetBiomeForMap(map);
            if (biome == null || biome.PokemonsNames == null || biome.PokemonsNames.Count == 0)
                return null;

            var rng = Rng;

            // Nom aleatoire :
            string name = biome.PokemonsNames[rng.Next(biome.PokemonsNames.Count)];

            var wild = DataJson.CreatePokemon(name);
            if (wild == null) return null;

            // Niveau aleatoire dans l'intervalle :
            int min = Math.Max(1, biome.LevelMin);
            int max = Math.Max(min, biome.LevelMax);
            wild.Level = rng.Next(min, max + 1);

            wild.OnInitPokemon();
            return wild;
        }

        private static void AddPnjToLists(Pnj pnj)
        {
            if (pnj == null || string.IsNullOrWhiteSpace(pnj.Name))
                throw new ArgumentException("PNJ invalide ou sans nom");

            Pnjs.Add(pnj);
            PnjsByName[pnj.Name] = pnj;
        }

        public static void RedrawCurrentMap()
        {
            Console.Clear();
            CurrentMap.Render();

            player.Draw();

            Ansi.Reset();   
            Console.ResetColor();
            Console.CursorVisible = false;

            InputBlocked = false;
        }
    }
}