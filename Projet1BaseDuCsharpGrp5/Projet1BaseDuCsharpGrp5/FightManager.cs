using Projet1BaseDuCsharpGrp5;
using System;
using System.Collections.Generic;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace Projet1BaseDuCsharpGrp5
{
    public class FightManager
    {
        private readonly Player _player;
        private readonly Random _rng = new Random();
        private readonly IFightUI _ui;

        public FightManager(Player player, IFightUI ui)
        {
            _player = player ?? throw new ArgumentNullException(nameof(player));
            _ui = ui ?? new ConsoleFightUI();
        }
        private bool _capturedPokemon = false;
        // -------------- Choix entre Pokemon Sauvage ou Dresseur ----------
        public void StartWildBattle(Pokemon wild)
        {
            _capturedPokemon = false;
            if (wild == null) throw new ArgumentNullException(nameof(wild));

            var pokemonOfPlayerActive = GetFirstAlivePokemon(_player);
            if (pokemonOfPlayerActive == null)
            {
                _ui.ShowMessage("Vous n'avez aucun Pokémon vivant pour combattre.");
                return;
            }

            pokemonOfPlayerActive.ResetCombatStats();

            RunBattleLoop(pokemonOfPlayerActive, wild, isWild: true);
        }

        public void StartPnjBattle(Pnj pnj)
        {
            if (pnj == null) throw new ArgumentNullException(nameof(pnj));

            //_ui.ShowMessage($"Vous affrontez {pnj.Name} !");

            var pokemonOfPlayerActive = GetFirstAlivePokemon(_player);
            var pokemonOfPnjActive = GetFirstAlivePokemon(pnj);

            if (pokemonOfPlayerActive == null)
            {
                _ui.ShowMessage("Vous n'avez aucun Pokémon vivant.");
                return;
            }
            if (pokemonOfPnjActive == null)
            {
                _ui.ShowMessage($"Le dresseur {pnj.Name} n'a pas de Pokémon. Victoire sans effort !");
                return;
            }

            pokemonOfPlayerActive.ResetCombatStats();

            RunBattleLoop(pokemonOfPlayerActive, pokemonOfPnjActive, isWild: false, pnj: pnj);
        }

        private void RunBattleLoop(Pokemon playerPoke, Pokemon enemyPoke, bool isWild, Pnj pnj = null)
        {
            bool battleOver = false;

            while (!battleOver)
            {
                // 1) Mise à jour de l'affichage
                _ui.Update(playerPoke, enemyPoke);

                // 2) Choix du joueur
                var mainOptions = new List<string> { "ATTAQUE", "OBJET", "POKÉMON", "FUIR" };
                int choice = _ui.ChooseFromList("Que veux-tu faire ?", mainOptions);

                if (choice == -1)
                {
                    continue;
                }

                string cho = mainOptions[choice];

                if (cho == "ATTAQUE")
                {
                    // Choix de competence
                    var comps = playerPoke.Competences.Select(c => c.Name + $" (Mana:{c.ManaCost})").ToList();
                    if (comps.Count == 0)
                    {
                        _ui.ShowMessage("Pas de compétences !");

                        Competence enemyCompFallback = ChooseEnemyCompetence(enemyPoke, playerPoke);
                        ResolveAttacks(playerPoke, null, enemyPoke, enemyCompFallback);
                    }
                    else
                    {
                        int compIndex = _ui.ChooseFromList("Choisis une attaque :", comps);
                        if (compIndex == -1) continue;

                        var chosenComp = playerPoke.Competences[compIndex];

                        // Choix de l'ennemi
                        var enemyChosenComp = ChooseEnemyCompetence(enemyPoke, playerPoke);

                        ResolveAttacks(playerPoke, chosenComp, enemyPoke, enemyChosenComp);

                        if (enemyPoke.Life <= 0)
                        {
                            _ui.Update(playerPoke, enemyPoke);
                            _ui.ShowMessage($"{enemyPoke.Name} est K.O. !");

                            if (!isWild && pnj != null)
                            {
                                var nextEnemy = GetFirstAlivePokemon(pnj);
                                if (nextEnemy != null)
                                {
                                    _ui.ShowMessage($"{pnj.Name} envoie {nextEnemy.Name} !");
                                    enemyPoke = nextEnemy;
                                    _ui.Update(playerPoke, enemyPoke);
                                    continue;
                                }
                            }

                            OnEnemyDefeated(playerPoke, enemyPoke, isWild, pnj);
                            battleOver = true;
                            break;
                        }

                        if (playerPoke.Life <= 0)
                        {
                            _ui.Update(playerPoke, enemyPoke);
                            _ui.ShowMessage($"{playerPoke.Name} est K.O. !");
                            // gestion changement d'un autre pokémon ou défaite
                            var next = ChooseAlivePokemonForReplacement(_player);
                            if (next == null)
                            {
                                _ui.ShowMessage("Tous vos Pokémon sont K.O. Vous perdez le combat.");
                                battleOver = true;
                                World.RedrawCurrentMap();
                                break;
                            }
                            else
                            {
                                playerPoke = next;
                                _ui.ShowMessage($"Vous envoyez {playerPoke.Name} !");
                            }
                        }
                    }
                }
                else if (cho == "OBJET")
                {
                    bool used = ShowAllItemsOfPlayerAndChooseOne(enemyPoke, isWild);

                    if (_capturedPokemon)
                    {
                        _ui.ShowMessage("Vous avez capturé le pokémon ! Le combat est terminé.");
                        battleOver = true;
                        World.RedrawCurrentMap();
                        break;
                    }

                    if (used && !_capturedPokemon)
                    {
                        var enemyComp = ChooseEnemyCompetence(enemyPoke, playerPoke);
                        if (enemyComp != null)
                        {
                            DoAttack(enemyPoke, enemyComp, playerPoke);
                            _ui.Update(playerPoke, enemyPoke);
                        }

                        // Si le joueur est K.O. suite à l'attaque adverse
                        if (playerPoke.Life <= 0)
                        {
                            _ui.ShowMessage($"{playerPoke.Name} est K.O. !");
                            var next = ChooseAlivePokemonForReplacement(_player);
                            if (next == null)
                            {
                                _ui.ShowMessage("Tous vos Pokémon sont K.O. Vous perdez le combat.");
                                battleOver = true;
                                World.RedrawCurrentMap();
                                break;
                            }
                            else
                            {
                                playerPoke = next;
                                _ui.ShowMessage($"Vous envoyez {playerPoke.Name} !");
                                _ui.Update(playerPoke, enemyPoke);
                            }
                        }

                        // Si l'ennemi est K.O. (rare après un item, mais on vérifie)
                        if (enemyPoke.Life <= 0)
                        {
                            _ui.Update(playerPoke, enemyPoke);
                            _ui.ShowMessage($"{enemyPoke.Name} est K.O. !");
                            OnEnemyDefeated(playerPoke, enemyPoke, isWild, pnj);
                            battleOver = true;
                            break;
                        }
                    }
                }
                else if (cho == "POKÉMON")
                {
                    var chosen = ChooseAlivePokemonForSwitch(_player, playerPoke);
                    if (chosen == null) continue;

                    _ui.ShowMessage($"{_player.Name} envoie {chosen.Name} !");
                    playerPoke = chosen;

                    _ui.Update(playerPoke, enemyPoke);

                    var enemyComp = ChooseEnemyCompetence(enemyPoke, playerPoke);
                    if (enemyComp != null)
                    {
                        DoAttack(enemyPoke, enemyComp, playerPoke);
                        _ui.Update(playerPoke, enemyPoke);
                    }

                    // gestion KO du joueur après l'attaque adverse (suite au switch)
                    if (playerPoke.Life <= 0)
                    {
                        _ui.ShowMessage($"{playerPoke.Name} est K.O. !");
                        var next = ChooseAlivePokemonForReplacement(_player);
                        if (next == null)
                        {
                            _ui.ShowMessage("Tous vos Pokémon sont K.O. Vous perdez le combat.");
                            battleOver = true;
                            World.RedrawCurrentMap();
                            break;
                        }
                        else
                        {
                            playerPoke = next;
                            _ui.ShowMessage($"Vous envoyez {playerPoke.Name} !");
                            _ui.Update(playerPoke, enemyPoke);
                        }
                    }
                }
                else if (cho == "FUIR")
                {
                    if (isWild)
                    {
                        bool escaped = _rng.NextDouble() < 0.6; // 60% de chance de fuir
                        if (escaped)
                        {
                            _ui.ShowMessage("Tu as réussi à fuir !");
                            battleOver = true;
                            World.RedrawCurrentMap();
                            break;
                        }
                        else
                        {
                            _ui.ShowMessage("La fuite a échoué !");
                        }
                    }
                    else
                    {
                        _ui.ShowMessage("Impossible de fuir d'un combat contre un dresseur !");
                    }
                }
            }
        }

        // ------------------------------------- Utilitaires simples -----------------------------

        // Compare les vitesses et applique degats dans l'ordre
        private void ResolveAttacks(Pokemon a, Competence compA, Pokemon b, Competence compB)
        {
            int speedA = a.SpeedAttack + (compA?.SpeedAttack ?? 0);
            int speedB = b.SpeedAttack + (compB?.SpeedAttack ?? 0);

            if (speedA >= speedB)
            {
                DoAttack(a, compA, b);
                if (b.Life > 0)
                    DoAttack(b, compB, a);
            }
            else
            {
                DoAttack(b, compB, a);
                if (a.Life > 0)
                    DoAttack(a, compA, b);
            }
        }
        private void DoAttack(Pokemon attacker, Competence comp, Pokemon target)
        {
            // Guard 
            if (attacker == null || target == null) return;
            if (target.Life <= 0) return;

            if (comp == null)
            {
                int dmgBase = Math.Max(1, attacker.DamageAttack);
                target.TakeDamage(dmgBase);
                _ui.ShowMessage($"{attacker.Name} frappe et inflige {dmgBase} dégâts à {target.Name}.");
                return;
            }

            if (!attacker.HasEnoughMana(comp))
            {
                int fallback = Math.Max(1, attacker.DamageAttack);
                target.TakeDamage(fallback);
                _ui.ShowMessage($"{attacker.Name} tente d'utiliser {comp.Name} mais manque de mana — frappe de base et inflige {fallback} dégâts.");
                return;
            }

            int dmg = attacker.CalculateDamage(attacker, comp, target, _rng);
            attacker.ConsumeMana(comp.ManaCost);
            target.TakeDamage(dmg);

            _ui.ShowMessage($"{attacker.Name} utilise {comp.Name} et inflige {dmg} dégâts à {target.Name}.");
        }

        private Pokemon ChooseAlivePokemonForSwitch(Dresseur d, Pokemon currentlyActive)
        {
            var candidates = d.TeamPokemons?.Where(p => p.Life > 0 && p != currentlyActive).ToList();
            if (candidates == null || candidates.Count == 0)
            {
                _ui.ShowMessage("Aucun  Pokémon vivant disponible");
                return null;
            }

            var labels = candidates.Select(p => $"{p.Name} Lv{p.Level}").ToList();
            int sel = _ui.ChooseFromList("Choisissez votre prochain pokémon :", labels);
            if (sel == -1) return null;
            return candidates[sel];
        }
        private Pokemon ChooseAlivePokemonForReplacement(Dresseur d)
        {
            var candidates = d.TeamPokemons?.Where(p => p.Life > 0).ToList();
            if (candidates == null || candidates.Count == 0)
            {
                _ui.ShowMessage("Aucun Pokémon vivant disponible.");
                return null;
            }

            var labels = candidates.Select(p => $"{p.Name} Lv{p.Level}").ToList();
            int sel = _ui.ChooseFromList("Choisissez votre prochain pokémon :", labels);
            if (sel == -1) return null;
            return candidates[sel];
        }

        private bool ShowAllItemsOfPlayerAndChooseOne(Pokemon enemy, bool isWildBattle)
        {
            // Guard
            if (_player == null)
            {
                _ui.ShowMessage("Erreur: joueur introuvable.");
                return false;
            }
            if (_player.Items == null || _player.Items.Count == 0)
            {
                _ui.ShowMessage("Vous n'avez aucun objet.");
                return false;
            }

            var itemsLabels = _player.Items.Select(i => $"{i.Name} [{i.Quantity}]").ToList();

            int itemIndex = _ui.ChooseFromList("Choisissez un objet :", itemsLabels);
            if (itemIndex == -1) // 0 / annulation
                return false;

            var selectedItem = _player.Items[itemIndex];

            // Si c'est une Pokeball : seulement sur sauvage
            if (selectedItem.ItemType == ItemType.Pokeball)
            {
                if (!isWildBattle)
                {
                    _ui.ShowMessage("Le pokemon n'est pas capturable.");
                    return false;
                }

                // Lance la tentative de capture
                bool captured = TryCapturePokemon(selectedItem, enemy);
                // On retourne true car on a utilisé l'objet (même si échec)
                return true;
            }

            // Détermination si l'objet est utilisable sur pokémon morts uniquement
            bool canBeUsedOnPokemonDead = false;
            if (selectedItem.ItemType == ItemType.PotionRappel
                || selectedItem.ItemType == ItemType.PotionRappelMax)
            {
                canBeUsedOnPokemonDead = true;
            }

            // CHOIX POKEMON (selon vivant vs mort)
            List<Pokemon> selectablePokemons;
            if (canBeUsedOnPokemonDead)
                selectablePokemons = _player.TeamPokemons?.Where(p => p.Life <= 0).ToList();
            else
                selectablePokemons = _player.TeamPokemons?.Where(p => p.Life > 0).ToList();

            if (selectablePokemons == null || selectablePokemons.Count == 0)
            {
                _ui.ShowMessage(canBeUsedOnPokemonDead
                    ? "Aucun Pokémon K.O. sur lequel utiliser cet objet."
                    : "Aucun Pokémon vivant sur lequel utiliser cet objet.");
                return false;
            }

            var pokeLabels = selectablePokemons
                .Select(p => $"{p.Name} PV:{p.Life}/{p.MaxLife}").ToList();

            int pokemonIndex = _ui.ChooseFromList("Choisissez le pokémon :", pokeLabels);
            if (pokemonIndex == -1)
                return false;

            var selectedPokemon = selectablePokemons[pokemonIndex];

            bool itemUsed = selectedItem.DoEffectOfItemOn(selectedPokemon);

            if (itemUsed)
            {
                _player.UseItem(selectedItem);
            }

            _ui.ShowMessage($"{selectedItem.Name} utilisé sur {selectedPokemon.Name}.");
            return true;
        }
        private bool TryCapturePokemon(Item pokeball, Pokemon wild)
        {
            // Guard 
            if (pokeball == null) throw new ArgumentNullException(nameof(pokeball));
            if (wild == null) throw new ArgumentNullException(nameof(wild));

            int baseRate = Math.Clamp(pokeball.Value, 5, 95);

            double hpLostRatio = (double)(wild.MaxLife - wild.Life) / wild.MaxLife;
            double chancePercent = baseRate + hpLostRatio * 50.0;

            chancePercent = Math.Min(chancePercent, 97.0);

            int roll = _rng.Next(1, 101);
            bool captured = roll <= chancePercent;

            _player.UseItem(pokeball);

            _ui.ShowMessage($"Lancement de {pokeball.Name} — {Math.Round(chancePercent, 1)}% de chance de capture !");
            if (captured)
            {
                _ui.ShowMessage($"{wild.Name} capturé !");
                _capturedPokemon = true;
                _player.AddPokemon(wild);
                return true;
            }
            else
            {
                _ui.ShowMessage($"{wild.Name} a résisté.");
                return false;
            }
        }

        private Competence ChooseEnemyCompetence(Pokemon enemy, Pokemon target)
        {
            if (enemy == null || enemy.Competences == null || enemy.Competences.Count == 0)
                return null;

            // Uniquement les compétences où il a suffisament de mana
            var usable = enemy.Competences.Where(c => enemy.HasEnoughMana(c)).ToList();
            if (usable.Count == 0)
                return null;

            double hpPercent = (double)enemy.Life / Math.Max(1, enemy.MaxLife) * 100.0;

            // Soin si pas beaucoup de PV :
            if (hpPercent <= 30.0)
            {
                //var heal = usable.FirstOrDefault(c => c.Name.IndexOf("soin", StringComparison.OrdinalIgnoreCase) >= 0
                //                                      || c.Name.IndexOf("heal", StringComparison.OrdinalIgnoreCase) >= 0
                //                                      || c.Name.IndexOf("restore", StringComparison.OrdinalIgnoreCase) >= 0);
                //if (heal != null) return heal;
            }

            // Meilleur compétence de dégats
            Competence best = null;
            int bestDmg = -1;
            foreach (var comp in usable)
            {
                int temp = enemy.CalculateDamage(enemy, comp, target, _rng);
                if (temp > bestDmg)
                {
                    best = comp;
                    bestDmg = temp;
                }
            }

            if (best != null) return best;

            return usable[_rng.Next(usable.Count)];
        }
        private Pokemon GetFirstAlivePokemon(Dresseur d)
        {
            return d.TeamPokemons?.FirstOrDefault(p => p.Life > 0);
        }
        private void OnEnemyDefeated(Pokemon playerPoke, Pokemon enemyPoke, bool isWild, Pnj pnj)
        {
            int xpGain = Math.Max(1, enemyPoke.Level * 5);
            playerPoke.GainExperience(xpGain);
            _player.CheckLevelPokemon();

            _ui.ShowMessage($"{playerPoke.Name} gagne {xpGain} XP !");
            if (!isWild && pnj != null && pnj._reward != null)
            {
                _player.Items.Add(pnj._reward);
                _ui.ShowMessage($"Tu reçois {_player.Items.Last().Name} en récompense !");
            }
            World.RedrawCurrentMap();
        }

    }
}

