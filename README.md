# PokeLike-CS

Un Pokémon-like entièrement jouable dans une console Windows : carte explorable,
combats au tour par tour, capture, inventaire et sauvegardes.

Écrit en C# / .NET 8, sans moteur de jeu ni bibliothèque graphique — tout
l'affichage est produit à la main avec des séquences d'échappement ANSI.

---

## Ma contribution

Projet mené à trois. Je me suis occupé de :

**La sérialisation, de bout en bout.** Le chargement des données de jeu
(Pokémon, compétences, objets, biomes) depuis les fichiers JSON, et le système
de sauvegarde à trois emplacements — modèle `GameSave` / `PnjSave` qui capture
l'état du joueur, celui des PNJ et la carte courante.

**Le calcul de dégâts en combat.** La formule reprend celle des jeux Pokémon
officiels, avec ses arrondis par paliers :

```
base = ⌊ ⌊ ⌊2·niveau/5 + 2⌋ · puissance · attaque / défense ⌋ / 50 ⌋ + 2
```

puis les modificateurs appliqués dessus : coup critique (1 chance sur 16, ×1.5),
aléa de 85 à 100 %, bonus STAB quand le type de l'attaque est celui de
l'attaquant (×1.5), et enfin le multiplicateur de la table des types.

Une première version, plus naïve — `(attaque + puissance) × type − défense` —
est conservée en commentaire dans `Pokemon.CalculateDamage`, pour garder trace
du passage de l'une à l'autre.

Le reste du projet était réparti entre mes deux coéquipiers : la carte et
l'exploration pour l'un, la boucle de combat et la capture pour l'autre.

---

## L'intérêt du projet

La contrainte était de faire un jeu **en console**. Plutôt que de s'en tenir à
du texte, le rendu est traité comme un écran de pixels :

- le mode **VT100** est activé via `kernel32` pour débloquer la couleur vraie ;
- chaque couleur est écrite en **RGB 24 bits** (`\x1b[38;2;r;g;bm`) ;
- une case de la carte occupe **deux caractères de large** (`Map.PixelWidth`),
  ce qui donne des pixels à peu près carrés dans une console ;
- les sprites des Pokémon sont dessinés dans ce même système (`PokeSprite`).

C'est la partie la plus intéressante du code, et la moins habituelle.

## Ce que le jeu contient

- **Exploration** — plusieurs cartes reliées par des zones de transition,
  décors, maisons, PNJ avec états et dialogues.
- **Combats** — face aux Pokémon sauvages et aux dresseurs : attaques,
  changement de Pokémon, utilisation d'objets, table des types.
- **Capture** — tentative à probabilité calculée, selon la Pokéball et l'état
  du Pokémon visé.
- **Inventaire et objets**, définis en données.
- **Sauvegardes** — trois emplacements, sérialisés en JSON.

## Comment c'est construit

**Les données sont séparées du code.** Pokémon, compétences, objets et biomes
sont décrits dans quatre fichiers JSON désérialisés avec `System.Text.Json`.
Ajouter un Pokémon ou une attaque ne demande pas de recompiler.

**L'interface de combat passe par une abstraction.** `FightManager` ne connaît
que l'interface `IFightUI` (`Update`, `ShowMessage`, `ChooseFromList`), ce qui
permet deux rendus différents — `ConsoleFightUI` et `FightArenaUI` — sans
toucher à la logique de combat.

**Les menus suivent le même principe**, avec `IMenu` et `IOverlayMenu` pilotés
par `MenuManager` : menu principal, sélection de sauvegarde, inventaire, choix
du starter, saisie du nom, visionneuse de sprites.

**La table des types** est une matrice de multiplicateurs (`TypeMultiplier`),
indexée par les valeurs de l'énumération `PokemonType`.

```
Projet1BaseDuCsharpGrp5/
├── *.JSON                    Données : Pokémon, compétences, objets, biomes
└── Projet1BaseDuCsharpGrp5/
    ├── Ansi.cs, Rgb.cs       Couleur vraie et mode VT100
    ├── PokeSprite.cs, UI.cs  Rendu des sprites et de l'interface
    ├── Map.cs, World.cs      Cartes, transitions, boucle de jeu
    ├── FightManager.cs       Logique de combat
    ├── IFightUI.cs           Abstraction de l'affichage de combat
    ├── MenuManager.cs        Pilotage des menus et overlays
    └── json.cs, ClientSerialization.cs, GameSave.cs
```

## Lancer le jeu

Nécessite le **SDK .NET 8** et **Windows** (voir les limites ci-dessous).

```bash
cd Projet1BaseDuCsharpGrp5/Projet1BaseDuCsharpGrp5
dotnet run
```

La console est maximisée automatiquement au démarrage. Si l'affichage est
tronqué, agrandissez la fenêtre ou réduisez la taille de police.

## Tests

Quelques tests NUnit couvrent le chargement des données et la construction des
Pokémon et des compétences.

```bash
cd Projet1BaseDuCsharpGrp5/TestProject1
dotnet test
```

## Limites connues

Ces points sont assumés — le projet a été mené sur une semaine, et ils sont
documentés plutôt que masqués.

- **Windows uniquement.** Le mode VT100 et la maximisation de la fenêtre
  passent par `kernel32` et `user32`. Les chemins vers les JSON utilisent par
  ailleurs des séparateurs Windows et une casse d'extension qui ne
  fonctionneraient pas sur un système sensible à la casse.
- **Les JSON sont relus à chaque création de Pokémon.** Les champs de cache
  existent dans `Json` mais ne sont pas encore utilisés. Sans impact visible à
  cette échelle, mais c'est la première chose à corriger.
- **7 types sur 18.** Les autres sont présents en commentaire dans
  `PokemonType`, la matrice est dimensionnée en conséquence.
- **Les chemins des JSON remontent de cinq niveaux** depuis le dossier de
  build, ce qui lie l'exécution à la structure du dépôt.

## Contexte

Projet scolaire d'une semaine, mené en groupe à Gaming Campus (Bachelor G.Tech),
sur les bases du C# et de la programmation orientée objet.
