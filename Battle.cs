using System;
using System.Collections.Generic;
using System.Threading;

namespace PokemonPocket
{

    public class PokemonBattle
    {
        public void Explore()
        {
            for (int i = 0; i < 4; i++)
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
            double chance = 0.7;

            if (randomGen.NextDouble() < chance)
            {
                Battle();
            }
            else
            {
                Console.WriteLine("\nNo Pokemons found while exploring.");
            }
        }

        public void Battle()
        {
            Console.WriteLine("\nYou found a wild pokemonName!");

            // while (true)
            // {
                
            // }
        }

    }
}
