// Name: Nicholas Kow
// AdminNo: 242682R

using Microsoft.EntityFrameworkCore;

namespace PokemonPocket
{
    public class PokemonContext : DbContext
    {
        public DbSet<Pokemon> Pokemons { get; set; }
        public DbSet<PokeDex> PokeDex { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=pokemonPocket.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<PokeDex>().HasData(
        new PokeDex
        {
            Id = 1,
            Name = "Pikachu",
            Skill = "Lightning Bolt",
            SkillDmg = 30
        },
        new PokeDex
        {
            Id = 2,
            Name = "Eevee",
            Skill = "Run Away",
            SkillDmg = 25
        },
        new PokeDex
        {
            Id = 3,
            Name = "Charmander",
            Skill = "Solar Power",
            SkillDmg = 10
        },
        new PokeDex
        {
            Id = 4,
            Name = "Raichu",
            Skill = "Lightning Bolt",
            SkillDmg = 30
        },
        new PokeDex
        {
            Id = 5,
            Name = "Flareon",
            Skill = "Run Away",
            SkillDmg = 25
        },
        new PokeDex
        {
            Id = 6,
            Name = "Charmeleon",
            Skill = "Solar Power",
            SkillDmg = 10
        }
    );
}

    }
}
