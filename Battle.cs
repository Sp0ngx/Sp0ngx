using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PokemonPocket
{
    public class PokemonBattle
    {
        private readonly PokemonContext _db; // Sync DB with Program.cs

        private PokeDex enemyPokemon;
        private int enemyMaxHP;
        private int enemyCurrHP;
        private int enemyLevel;

        private Pokemon selectedPokemon;
        private int selectedPokemonCurrHp;

        public PokemonBattle(PokemonContext db)
        {
            _db = db;
        }

        public List<PokeDex> LoadAllPokemon()
        {
            return _db.PokeDex.ToList();
        }

        public void Explore()
        {
            LoadingAnimation("Exploring", 3, "Blue");

            if (new Random().NextDouble() < 0.7)
                Battle();
            else
                Console.WriteLine("No Pokemons found while exploring.");
        }

        private void LoadingAnimation(string message, int repeat, string colour, int delay = 300)
        {
            if (!Enum.TryParse(colour, true, out ConsoleColor parsedColor))
                parsedColor = ConsoleColor.Gray;

            for (int i = 0; i < repeat; i++)
            {
                for (int dot = 0; dot <= 3; dot++)
                {
                    Console.ForegroundColor = parsedColor;
                    Console.Write("\r" + new string(' ', Console.WindowWidth));
                    Console.Write($"\r{message}" + new string('.', dot));
                    Thread.Sleep(delay);
                    Console.ResetColor();
                }
            }
            Console.WriteLine("");
        }

        public void Display()
        {
            Console.WriteLine("========================================");
            Console.WriteLine("                 Battle                 ");
            Console.WriteLine("========================================");
            Console.WriteLine($"                     {enemyPokemon.Name} (Lv{enemyLevel})");
            Console.WriteLine($"                     {enemyCurrHP}/{enemyMaxHP}");
            Console.WriteLine($"{selectedPokemon.Name} (Lv{selectedPokemon.Level})");
            Console.WriteLine($"{selectedPokemonCurrHp}/{selectedPokemon.MaxHP}");
            Console.WriteLine("========================================");
        }

        public void RunAway()
        {
            LoadingAnimation("Returning Home", 2, "Red");
            Console.WriteLine();
        }

        public void Fight()
        {
            Console.WriteLine($"{selectedPokemon.Name} used {selectedPokemon.Skill} dealing {selectedPokemon.SkillDmg} Dmg.");
            enemyCurrHP -= selectedPokemon.SkillDmg;
            Thread.Sleep(1500);

            if (enemyCurrHP <= 0)
            {
                int expGained = new Random().Next(20, 101) * enemyLevel / 2;
                int previousLevel = selectedPokemon.Level;

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"You defeated the wild {enemyPokemon.Name}!");
                Console.WriteLine($"You gained {expGained} Exp!");
                Console.ResetColor();
                Thread.Sleep(1500);

                var pokemonInDb = _db.Pokemons.SingleOrDefault(p => p.Id == selectedPokemon.Id);
                if (pokemonInDb != null)
                {
                    pokemonInDb.Exp += expGained;
                    if (pokemonInDb.Exp >= 100)
                    {
                        int levelGained = pokemonInDb.Exp / 100;
                        pokemonInDb.Level += levelGained;
                        pokemonInDb.Exp %= 100;

                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"Your {pokemonInDb.Name}'s level increased from Lvl {previousLevel} to Lvl {pokemonInDb.Level}!");
                        Console.ResetColor();
                    }
                    _db.SaveChanges();
                }

                RunAway();
            }
            else
            {
                EnemyFight();
            }
        }

        public void EnemyFight()
        {
            Console.WriteLine($"Wild {enemyPokemon.Name} used {enemyPokemon.Skill} dealing {enemyPokemon.SkillDmg} HP.");
            selectedPokemonCurrHp -= enemyPokemon.SkillDmg;
            Thread.Sleep(1500);

            var pokemonInDb = _db.Pokemons.SingleOrDefault(p => p.Id == selectedPokemon.Id);
            if (pokemonInDb != null)
            {
                pokemonInDb.HP = Math.Max(selectedPokemonCurrHp, 0);
                _db.SaveChanges();
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
            var pocket = _db.Pokemons.OrderByDescending(p => p.HP).ToList();

            if (!pocket.Any(p => p.HP > 0))
            {
                Console.WriteLine("All pokemons have fainted.");
                RunAway();
                return;
            }

            foreach (var p in pocket)
            {
                string Status = p.HP > 0 ? "Alive" : "Fainted";
                Console.ForegroundColor = p.HP > 0 ? ConsoleColor.Green : ConsoleColor.Red;
                Console.WriteLine($"Id: {p.Id}\nName: {p.Name}\nLevel: {p.Level}\nHP: {p.HP}\nStatus: {Status}");
                Console.ResetColor();
                Console.WriteLine("------------------------------");
            }

            Pokemon newSelectedId = null;
            while (newSelectedId == null)
            {
                Console.Write("Enter Pokemon Id to select: ");
                if (int.TryParse(Console.ReadLine().Trim(), out int id))
                {
                    newSelectedId = pocket.FirstOrDefault(p => p.Id == id && p.HP > 0);
                    if (newSelectedId == null)
                        Console.WriteLine("Invalid Id or Pokemon has fainted.");
                }
                else
                    Console.WriteLine("Invalid input. Please enter a number.");
            }

            selectedPokemon = newSelectedId;
            selectedPokemonCurrHp = selectedPokemon.HP;
            Console.WriteLine($"Selected {selectedPokemon.Name} (Lv{selectedPokemon.Level}).");
            Thread.Sleep(1000);
        }

        public void CapturePokemon()
        {
            double chance = (double)(enemyMaxHP - enemyCurrHP) / enemyMaxHP;
            Console.WriteLine($"You threw a pokeball at a wild {enemyPokemon.Name}.");
            Thread.Sleep(1500);
            LoadingAnimation("Capturing", 3, "Blue");
            Console.WriteLine();

            if (new Random().NextDouble() < chance)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"You successfully captured {enemyPokemon.Name} (Lv{enemyLevel})!");
                Console.ResetColor();

                var type = Type.GetType($"PokemonPocket.{enemyPokemon.Name}");
                if (type != null)
                {
                    var captured = (Pokemon)Activator.CreateInstance(type, enemyPokemon.Name, enemyMaxHP, 0, enemyLevel);
                    _db.Pokemons.Add(captured);
                    _db.SaveChanges();
                }
                else
                    Console.WriteLine("An error has occurred.");

                enemyCurrHP = 0;
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
                string input = Console.ReadLine().Trim().ToLower();

                if (input == "1")
                {
                    var pokemonInDb = _db.Pokemons.SingleOrDefault(p => p.Id == selectedPokemon.Id);
                    if (pokemonInDb != null)
                    {
                        int oldHP = pokemonInDb.HP;
                        pokemonInDb.HP = Math.Min(pokemonInDb.HP + 25, pokemonInDb.MaxHP);
                        _db.SaveChanges();

                        selectedPokemonCurrHp = pokemonInDb.HP;
                        selectedPokemon.HP = pokemonInDb.HP;
                        Console.WriteLine($"Your {pokemonInDb.Name} healed from {oldHP} HP to {pokemonInDb.HP} HP.");
                    }
                    Thread.Sleep(1500);
                    EnemyFight();
                    return;
                }
                else if (input == "2")
                {
                    CapturePokemon();
                    return;
                }
                else if (input == "q")
                {
                    return;
                }
                else
                {
                    Console.WriteLine("Invalid Input. Try again.");
                }
            }
        }

        public void Battle()
        {
            var allPokemon = LoadAllPokemon();
            var random = new Random();

            enemyPokemon = allPokemon[random.Next(allPokemon.Count)];
            enemyMaxHP = random.Next(50, 151); // Range 50 - 150
            enemyLevel = random.Next(1, 11); // Range 1 - 10
            enemyCurrHP = enemyMaxHP;

            Console.WriteLine($"You found a wild {enemyPokemon.Name} (Lv{enemyLevel})!");
            Thread.Sleep(2000);

            ChangePokemon();

            selectedPokemonCurrHp = selectedPokemon.HP;
            LoadingAnimation("Commencing Battle", 2, "Blue");
            Console.WriteLine();

            while (enemyCurrHP > 0 && _db.Pokemons.Any(p => p.HP > 0))
            {
                Display();
                Console.WriteLine("(1). Fight\n(2). Inventory\n(3). Change Pokemon\n(4). Run Away");
                Console.Write("Please only enter [1,2,3,4]: ");

                switch (Console.ReadLine().Trim())
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
                        RunAway(); return;

                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }
            }
        }
    }
}
