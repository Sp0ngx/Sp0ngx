using Microsoft.EntityFrameworkCore;

namespace PokemonPocket
{
    public class PokemonContext : DbContext
    {
        public DbSet<Pokemon> Pokemons { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=pokemonPocket.db");
        }
    }
}
