using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace PokemonPocket
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var db = new PokemonContext())
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                db.Database.EnsureCreated();

                // // For testing
                // if (!db.Pokemons.Any())
                // {
                //     db.Pokemons.AddRange(
                //         new Charmander("Charmander", 67, 56),
                //         new Pikachu("Pikachu", 23, 45),
                //         new Pikachu("Eevee", 45, 89),
                //         new Eevee("Eevee", 23, 87),
                //         new Eevee("Pikachu", 12, 34),
                //         new Charmander("Eevee", 12, 21)
                //     );
                //     db.SaveChanges();
                // }

                // List<Pokemon> pocket = new List<Pokemon>();
                // pocket.Add(new Charmander("Charmander", 67, 56)); // Testing
                // pocket.Add(new Pikachu("Pikachu", 23, 45)); // Testing
                // pocket.Add(new Pikachu("Eevee", 45, 89)); // Testing
                // pocket.Add(new Eevee("Eevee", 23, 87)); // Testing
                // pocket.Add(new Eevee("Pikachu", 12, 34)); // Testing
                // pocket.Add(new Charmander("Eevee", 12, 21)); // Testing
                // PokemonMaster list for checking pokemon evolution availability.
                // new PokemonMaster(<Type of Pokemon>, <Required no. of the same type to evolve>, <Name of Pokemon it will evolve to>)

                List<PokemonMaster> pokemonMasters = new List<PokemonMaster>()
                {
                    new PokemonMaster("Pikachu", 2, "Raichu", 5),
                    new PokemonMaster("Eevee", 3, "Flareon", 3),
                    new PokemonMaster("Charmander", 1, "Charmeleon", 8)
                };

                //Use "Environment.Exit(0);" if you want to implement an exit of the console program
                //Start your assignment 1 requirements below.

                while (true)
                {
                    Console.WriteLine("*****************************");
                    Console.WriteLine("Welcome to Pokemon Pocket App");
                    Console.WriteLine("*****************************");
                    Console.WriteLine("(1). Add pokemon to my pocket");
                    Console.WriteLine("(2). List pokemon(s) in my Pocket");
                    Console.WriteLine("(3). Check if I can evolve pokemon");
                    Console.WriteLine("(4). Evolve pokemon");
                    Console.WriteLine("(5). Heal all pokemon");
                    Console.WriteLine("(6). Delete pokemon");
                    Console.WriteLine("(7). Explore to find pokemons to battle");
                    Console.Write("Please only enter [1,2,3,4,5,6,7] or Q to quit: ");

                    string input = Console.ReadLine().Trim();

                    switch (input)
                    {
                        case "1": // Add Pokemon
                            Console.Write("Enter Pokemon's Name: ");
                            string name = Console.ReadLine().Trim();
                            string titleName = char.ToUpper(name[0]) + name.Substring(1).ToLower();

                            Type type = Type.GetType($"PokemonPocket.{titleName}");

                            if (type != null && typeof(Pokemon).IsAssignableFrom(type))
                            {
                                int hp, exp, level;

                                Console.Write("Enter Pokemon's HP: ");
                                if (!int.TryParse(Console.ReadLine(), out hp) || hp < 0)
                                {
                                    Console.WriteLine("Invalid HP. Try again.");
                                    break;
                                }

                                Console.Write("Enter Pokemon's Exp: ");
                                if (!int.TryParse(Console.ReadLine(), out exp) || exp < 0)
                                {
                                    Console.WriteLine("Invalid Exp. Try again.");
                                    break;
                                }

                                Console.Write("Enter Pokemon's Level: ");
                                if (!int.TryParse(Console.ReadLine(), out level) || level < 0)
                                {
                                    Console.WriteLine("Invalid Level. Try again.");
                                    break;
                                }

                                Pokemon pokemon = (Pokemon)Activator.CreateInstance(type, titleName, hp, exp, level);
                                db.Pokemons.Add(pokemon);
                                db.SaveChanges();
                            }
                            else
                            {
                                Console.WriteLine("Invalid Pokemon. Try again.");
                            }

                            break;

                        case "2": // List All Pokemons
                            List<Pokemon> pocket = db.Pokemons.OrderByDescending(p => p.Exp).ToList();
                            foreach (var pokemon in pocket)
                            {
                                Console.WriteLine($"Id: {pokemon.Id}\nName: {pokemon.Name}\nLevel: {pokemon.Level}\nHP: {pokemon.HP}\nExp: {pokemon.Exp}\nSkill: {pokemon.Skill}");
                                Console.WriteLine("------------------------------");
                            }
                            break;

                        case "3": // Check if any Pokemon meets evolution requirement
                            var pokemons = db.Pokemons.ToList();

                            List<string> evolvableTypes = pokemonMasters.Where(m => pokemons.Count(p => p.Name == m.Name) >= m.NoToEvolve).Select(m => m.Name).Distinct().ToList();

                            if (evolvableTypes.Count == 0)
                            {
                                Console.WriteLine("No available Pokemons can be evolved.");
                            }
                            else
                            {
                                foreach (var master in pokemonMasters)
                                {
                                    int pokemonCount = pokemons.Count(p => p.Name == master.Name);
                                    if (pokemonCount >= master.NoToEvolve)
                                    {
                                        if (evolvableTypes.Count == 1)
                                        {
                                            Console.WriteLine($"{master.Name} --> {master.EvolveTo}");
                                        }
                                        else
                                        {
                                            Console.WriteLine($"{master.NoToEvolve} {master.Name} --> 1 {master.EvolveTo}");
                                        }
                                    }
                                }
                            }
                            break;

                        case "4": // Evolve All Pokemons
                            foreach (var master in pokemonMasters)
                            {
                                int evoAmount = db.Pokemons.Count(p => p.Name == master.Name) / master.NoToEvolve;

                                if (evoAmount == 0) continue;

                                for (int i = 0; i < evoAmount; i++)
                                {
                                    List<Pokemon> pokemonsToRemove = db.Pokemons.Where(p => p.Name == master.Name).Take(master.NoToEvolve).ToList();

                                    int level = pokemonsToRemove.Max(p => p.Level);

                                    foreach (var p in pokemonsToRemove)
                                    {
                                        db.Pokemons.Remove(p);
                                    }

                                    Type typeEvo = Type.GetType($"PokemonPocket.{master.EvolveTo}");

                                    try
                                    {
                                        if (typeEvo != null && typeof(Pokemon).IsAssignableFrom(typeEvo))
                                        {
                                            Pokemon pokemon = (Pokemon)Activator.CreateInstance(typeEvo, master.EvolveTo, 100, 0, level);
                                            db.Pokemons.Add(pokemon);
                                            Console.WriteLine($"{master.NoToEvolve} {master.Name} evolved into 1 {master.EvolveTo}!");
                                        }
                                    }
                                    catch
                                    {
                                        Console.WriteLine("An error has occurred.");
                                    }
                                    db.SaveChanges();

                                }
                            }
                            break;

                        case "5": // Heal All Pokemons
                            var pokemonsToHeal = db.Pokemons.ToList(); // re-fetch
                            foreach (var pokemon in pokemonsToHeal)
                            {
                                pokemon.HP = pokemon.MaxHP;
                            }
                            db.SaveChanges();

                            Console.WriteLine("All Pokemons have been healed!");
                            break;

                        case "6": // Delete Pokemon
                            if (db.Pokemons.Count() == 0)
                            {
                                Console.WriteLine("Your pocket is empty.");
                            }
                            else
                            {
                                foreach (var pokemon in db.Pokemons.OrderBy(p => p.Id))
                                {
                                    Console.WriteLine($"Id: {pokemon.Id}\nName: {pokemon.Name}\nHP: {pokemon.HP}\nExp: {pokemon.Exp}\nSkill: {pokemon.Skill}");
                                    Console.WriteLine("------------------------------");
                                }

                                Console.Write("Enter Pokemon Id to delete (0 to exit): ");
                                string deleteId = Console.ReadLine();

                                if (deleteId == "0")
                                {
                                    break;
                                }
                                else
                                {
                                    var pokemonToDelete = db.Pokemons.SingleOrDefault(p => p.Id.ToString() == deleteId);
                                    if (pokemonToDelete != null)
                                    {
                                        db.Pokemons.Remove(pokemonToDelete);
                                        db.SaveChanges();
                                        Console.WriteLine($"Deleted {pokemonToDelete.Name} (ID: {pokemonToDelete.Id}) from your pocket.");
                                    }
                                    else
                                    {
                                        Console.WriteLine($"No Pokemon found with ID: {deleteId}");
                                    }
                                }

                            }

                            break;

                        case "7": // Explore and Battle Pokemons
                            PokemonBattle pokemonBattle = new PokemonBattle(db); // To sync DB with Battle.cs
                            if (db.Pokemons.Count() >= 1 && db.Pokemons.Any(p => p.HP > 0)) // Check if player has at least 1 pokemon & if at least 1 is "alive"
                            {
                                pokemonBattle.Explore();    
                            }
                            else
                            {
                                Console.WriteLine("You need at least 1 pokemon to battle and it needs to have at least 1 hp.");
                            }
                            break;



                        case "q":
                        case "Q":
                            db.SaveChanges();
                            Environment.Exit(0);
                            break;

                        default:
                            Console.WriteLine("Invalid Input. Try again.");
                            break;
                    }
                }
            }
        }
    }
}
