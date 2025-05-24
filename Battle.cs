using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace PokemonPocket
{

    public class PokemonBattle
    {
        private PokeDex enemyPokemon;
        private int EnemyMaxHP;
        private int EnemyCurrHP;
        private int EnemyLevel;

        private Pokemon selectedPokemon;
        private int selectedPokemonCurrHp;

        public List<PokeDex> LoadAllPokemon()
        {
            using (var db = new PokemonContext())
            {
                return db.PokeDex.ToList();
            }
        }

        public void Explore()
        {
            for (int i = 0; i < 3; i++)
            {
                int dotCount = 0;

                for (dotCount = 0; dotCount <= 3; dotCount++)
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write("\r" + new string(' ', Console.WindowWidth)); // Clear line
                    Console.Write("\rExploring" + new string('.', dotCount));
                    Thread.Sleep(300);
                    Console.ResetColor();
                }
            }


            Random randomGen = new Random();
            double chance = 1; // Change back to 0.7

            if (randomGen.NextDouble() < chance)
            {
                Battle();
            }
            else
            {
                Console.WriteLine("\nNo Pokemons found while exploring.");
            }
        }

        public void Display()
        {
            Console.WriteLine("========================================");
            Console.WriteLine("                 Battle                 ");
            Console.WriteLine("========================================");
            Console.WriteLine($"                     {enemyPokemon.Name} (Lv{EnemyLevel})");
            Console.WriteLine($"                     {EnemyCurrHP}/{EnemyMaxHP}");
            Console.WriteLine($"{selectedPokemon.Name} (Lv{selectedPokemon.Level})");
            Console.WriteLine($"{selectedPokemonCurrHp}/{selectedPokemon.MaxHP}");
            Console.WriteLine("========================================");
        }

        public void RunAway()
        {
            for (int i = 0; i < 2; i++)
            {
                int dotCount = 0;

                for (dotCount = 0; dotCount <= 3; dotCount++)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("\r" + new string(' ', Console.WindowWidth)); // Clear line
                    Console.Write("\rReturning Home" + new string('.', dotCount));
                    Thread.Sleep(300);
                    Console.ResetColor();
                }
            }
            Console.WriteLine("");
        }

        public void Fight()
        {
            Console.WriteLine($"Wild {selectedPokemon.Name} used {selectedPokemon.Skill} dealing {selectedPokemon.SkillDmg}.");
            EnemyCurrHP -= selectedPokemon.SkillDmg;

            Thread.Sleep(1500);


            if (EnemyCurrHP <= 0)
            {
                // Exp gained is based off enemy pokemon level
                var random = new Random();
                int randomExp = random.Next(20, 101);
                int ExpGained = (int)(randomExp * EnemyLevel * 0.5);
                int holderLevel = selectedPokemon.Level;

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"You defeated the wild {enemyPokemon.Name}!");
                Console.WriteLine($"You gained {ExpGained} Exp!");
                Console.ResetColor();
                Thread.Sleep(1500);


                using (var db = new PokemonContext())
                {
                    var pokemonInDb = db.Pokemons.SingleOrDefault(p => p.Id == selectedPokemon.Id);

                    pokemonInDb.Exp += ExpGained;

                    if (pokemonInDb.Exp >= 100)
                    {
                        int LvlGained = ExpGained / 100;
                        pokemonInDb.Level += LvlGained;
                        pokemonInDb.Exp = pokemonInDb.Exp - (LvlGained * 100);

                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"Your {pokemonInDb.Name}'s level increased from {holderLevel} to {pokemonInDb.Level}!");
                        Console.ResetColor();
                    }
                    db.SaveChanges();
                }


                RunAway();
                return;
            }
            else
            {
                EnemyFight();
            }
        }

        public void EnemyFight()
        {
            Console.WriteLine($"{enemyPokemon.Name} used {enemyPokemon.Skill} dealing {enemyPokemon.SkillDmg}.");
            selectedPokemonCurrHp -= enemyPokemon.SkillDmg;

            Thread.Sleep(1500);

            using (var db = new PokemonContext())
            {
                var pokemonInDb = db.Pokemons.SingleOrDefault(p => p.Id == selectedPokemon.Id);
                if (pokemonInDb != null)
                {
                    if (selectedPokemonCurrHp > 0)
                    {
                        pokemonInDb.HP = selectedPokemonCurrHp;
                    }
                    else
                    {
                        pokemonInDb.HP = 0;
                    }
                    db.SaveChanges();
                }
            }

            if (selectedPokemonCurrHp <= 0)
            {
                Console.WriteLine($"Your {selectedPokemon.Name} has fainted!");
                Thread.Sleep(1500);
                ChangePokemon();
            }
        }

        public void ChangePokemon()
        {
            using (var db = new PokemonContext())
            {
                List<Pokemon> pocket = db.Pokemons.OrderByDescending(p => p.HP).ToList();

                if (pocket.Any(p => p.HP > 0))
                {

                    foreach (var pokemon in pocket)
                    {
                        string status;

                        if (pokemon.HP > 0)
                        {
                            status = "Alive";
                            Console.ForegroundColor = ConsoleColor.Green;
                        }
                        else
                        {
                            status = "Fainted";
                            Console.ForegroundColor = ConsoleColor.Red;
                        }

                        Console.WriteLine($"Id: {pokemon.Id}\nName: {pokemon.Name}\nLevel: {pokemon.Level}\nHP: {pokemon.HP}\nStatus: {status}");

                        Console.ResetColor();
                        Console.WriteLine("------------------------------");
                    }


                    Pokemon newSelectedPokemon = null;

                    while (newSelectedPokemon == null)
                    {
                        Console.Write("Enter Pokemon Id to select: ");
                        string input = Console.ReadLine().Trim();

                        if (!int.TryParse(input, out int id))
                        {
                            Console.WriteLine("Invalid input. Please enter a number.");
                            continue;
                        }

                        newSelectedPokemon = pocket.SingleOrDefault(p => p.Id == id);

                        if (newSelectedPokemon == null)
                        {
                            Console.WriteLine("No Pokemon found with that ID. Please try again.");
                            continue;
                        }

                        if (newSelectedPokemon.HP <= 0)
                        {
                            Console.WriteLine($"Cannot switch to {newSelectedPokemon.Name} because it has fainted.");
                            newSelectedPokemon = null;  // reset to ask again
                        }
                    }

                    selectedPokemon = newSelectedPokemon;
                    selectedPokemonCurrHp = selectedPokemon.HP;

                    Console.WriteLine($"Selected {selectedPokemon.Name} (Lv{selectedPokemon.Level}).");
                    Thread.Sleep(1000);
                }
                else
                {
                    Console.WriteLine("All pokemons have fainted.");
                    RunAway();
                }
            }
        }

        public void CapturePokemon()
        {
            Random randomGen = new Random();
            double chance = (double)(EnemyMaxHP - EnemyCurrHP) / EnemyMaxHP;
            Console.WriteLine($"You threw a pokeball at a wild {enemyPokemon.Name}.");
            // Console.WriteLine(chance);

            Thread.Sleep(1500);

            for (int i = 0; i < 3; i++)
            {
                int dotCount = 0;

                for (dotCount = 0; dotCount <= 3; dotCount++)
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write("\r" + new string(' ', Console.WindowWidth)); // Clear line
                    Console.Write("\rCapturing" + new string('.', dotCount));
                    Thread.Sleep(300);
                    Console.ResetColor();
                }
            }

            Console.WriteLine("");

            if (randomGen.NextDouble() < chance)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"You successfully captured {enemyPokemon.Name} (Lv{EnemyLevel})!");
                Console.ResetColor();

                Type type = Type.GetType($"PokemonPocket.{enemyPokemon.Name}");

                using (var db = new PokemonContext())
                {
                    Pokemon pokemon = (Pokemon)Activator.CreateInstance(type, enemyPokemon.Name, EnemyMaxHP, 0, EnemyLevel);
                    if (pokemon != null)
                    {
                        db.Pokemons.Add(pokemon);
                        db.SaveChanges();
                    }
                    else
                    {
                        Console.WriteLine("An error has occurred.");
                    }
                }

                EnemyCurrHP = 0;

                Task.Delay(1500);

                RunAway();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Wild {enemyPokemon.Name} broke out of pokeball.");
                Console.ResetColor();

                Thread.Sleep(1500);

                EnemyFight();
            }


        }

        public void Inventory()
        {
            while (true)
            {
                Console.WriteLine("========================================");
                Console.WriteLine("                Inventory                ");
                Console.WriteLine("========================================");
                Console.WriteLine("(1). Heal Potion (25HP)");
                Console.WriteLine("(2). Pokeball (Capture)");
                Console.Write("Enter [1,2] or Q to return: ");
                string input = Console.ReadLine().Trim();


                switch (input)
                {
                    case "1":
                        using (var db = new PokemonContext())
                        {
                            var pokemonInDb = db.Pokemons.SingleOrDefault(p => p.Id == selectedPokemon.Id);
                            if (pokemonInDb != null)
                            {
                                int holderHP = pokemonInDb.HP;
                                pokemonInDb.HP += 25;
                                if (pokemonInDb.HP > pokemonInDb.MaxHP)
                                {
                                    pokemonInDb.HP = pokemonInDb.MaxHP;
                                }
                                db.SaveChanges();

                                selectedPokemonCurrHp = pokemonInDb.HP;
                                selectedPokemon.HP = pokemonInDb.HP;

                                Console.WriteLine($"Your {pokemonInDb.Name} healed from {holderHP} to {pokemonInDb.HP}");
                            }
                            else
                            {
                                Console.WriteLine("Error: Pokemon not found in database.");
                            }
                        }

                        // int holderHP = selectedPokemonCurrHp;

                        // selectedPokemonCurrHp += 25;

                        // if (selectedPokemonCurrHp >= selectedPokemon.HP)
                        // {
                        //     selectedPokemonCurrHp = selectedPokemon.HP;
                        // }

                        // Console.WriteLine($"Your {selectedPokemon.Name} healed from {holderHP} to {selectedPokemonCurrHp}");

                        Thread.Sleep(1500);
                        EnemyFight();
                        return;

                    case "2":
                        CapturePokemon();
                        return;

                    case "q":
                    case "Q":
                        return;

                    default:
                        Console.WriteLine("Invalid Input. Try again.");
                        break;
                }
            }
        }

        public void Battle()
        {
            var allPokemon = LoadAllPokemon();
            var random = new Random();


            // Enemy Information
            enemyPokemon = allPokemon[random.Next(allPokemon.Count)];
            EnemyMaxHP = random.Next(50, 101);
            EnemyLevel = random.Next(1, 11);
            EnemyCurrHP = EnemyMaxHP;

            Console.WriteLine($"\nYou found a wild {enemyPokemon.Name} (Lv{EnemyLevel})!");

            Thread.Sleep(2000);

            using (var db = new PokemonContext())
            {
                ChangePokemon();

                selectedPokemonCurrHp = selectedPokemon.HP;

                for (int i = 0; i < 2; i++)
                {
                    int dotCount = 0;

                    for (dotCount = 0; dotCount <= 3; dotCount++)
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.Write("\r" + new string(' ', Console.WindowWidth)); // Clear line
                        Console.Write("\rCommencing Battle" + new string('.', dotCount));
                        Thread.Sleep(300);
                        Console.ResetColor();
                    }
                }

                Console.WriteLine("");


                while (EnemyCurrHP > 0 && db.Pokemons.Any(p => p.HP > 0))
                {
                    Display();

                    Console.WriteLine("(1). Fight");
                    Console.WriteLine("(2). Inventory");
                    Console.WriteLine("(3). Change Pokemon");
                    Console.WriteLine("(4). Run Away");
                    Console.Write("Please only enter [1,2,3,4]: ");
                    string input = Console.ReadLine().Trim();

                    switch (input)
                    {
                        case "1":
                            Fight();
                            break;
                        case "2":
                            Inventory();
                            break;
                        case "3":
                            ChangePokemon();
                            break;
                        case "4":
                            Console.WriteLine("You ran away!");
                            RunAway();
                            return;
                        default:
                            Console.WriteLine("Invalid option.");
                            break;
                    }
                }


            }
        }
    }
}
