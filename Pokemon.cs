// Name: Nicholas Kow
// AdminNo: 242682R

using System;
using System.Collections.Generic;

namespace PokemonPocket
{

    public class PokeDex
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Skill { get; set; }
        public int SkillDmg { get; set; }
    }

    public class Inventory
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class PokemonMaster
    {
        public string Name { get; set; }
        public int NoToEvolve { get; set; }
        public string EvolveTo { get; set; }
        public int LevelToEvolve { get; set; }

        public PokemonMaster(string name, int noToEvolve, string evolveTo, int levelToEvolve)
        {
            this.Name = name;
            this.NoToEvolve = noToEvolve;
            this.EvolveTo = evolveTo;
            this.LevelToEvolve = levelToEvolve;
        }
    }

    public class Pokemon
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int HP { get; set; }
        public int Exp { get; set; }
        public string Skill { get; set; }
        public int SkillDmg { get; set; }
        public int Level { get; set; }
        public int MaxHP { get; set; }

        public Pokemon() { }
        public Pokemon(string name, int hp, int exp, int level)
        {
            this.Name = name;
            this.HP = hp;
            this.Exp = exp;
            this.Level = level;
            this.MaxHP = hp;
        }

        public virtual void CalculateDamage(int opponentSkillDmg) { }

    }
    public class Pikachu : Pokemon
    {
        public Pikachu(string name, int hp, int exp, int level) : base(name, hp, exp, level)
        {
            name = "Pikachu";
            this.Skill = "Lightning Bolt";
            this.SkillDmg = 30;
        }

        public override void CalculateDamage(int opponentSkillDmg)
        {
            int dmgTaken = 3 * opponentSkillDmg;
            this.HP -= dmgTaken;
        }
    }

    public class Eevee : Pokemon
    {
        public Eevee(string name, int hp, int exp, int level) : base(name, hp, exp, level)
        {
            name = "Eevee";
            this.Skill = "Run Away";
            this.SkillDmg = 25;
        }

        public override void CalculateDamage(int opponentSkillDmg)
        {
            int dmgTaken = 2 * opponentSkillDmg;
            this.HP -= dmgTaken;
        }
    }
    public class Charmander : Pokemon
    {
        public Charmander(string name, int hp, int exp, int level) : base(name, hp, exp, level)
        {
            name = "Charmander";
            this.Skill = "Solar Power";
            this.SkillDmg = 10;
        }

        public override void CalculateDamage(int opponentSkillDmg)
        {
            int dmgTaken = 1 * opponentSkillDmg;
            this.HP -= dmgTaken;
        }
    }

    public class Raichu : Pokemon
    {
        public Raichu(string name, int hp, int exp, int level) : base(name, hp, exp, level)
        {
            name = "Raichu";
            this.Skill = "Lightning Bolt";
            this.SkillDmg = 30;
        }

        public override void CalculateDamage(int opponentSkillDmg)
        {
            int dmgTaken = 3 * opponentSkillDmg;
            this.HP -= dmgTaken;
        }
    }

    public class Flareon : Pokemon
    {
        public Flareon(string name, int hp, int exp, int level) : base(name, hp, exp, level)
        {
            name = "Flareon";
            this.Skill = "Run Away";
            this.SkillDmg = 25;
        }

        public override void CalculateDamage(int opponentSkillDmg)
        {
            int dmgTaken = 2 * opponentSkillDmg;
            this.HP -= dmgTaken;
        }
    }

    public class Charmeleon : Pokemon
    {
        public Charmeleon(string name, int hp, int exp, int level) : base(name, hp, exp, level)
        {
            name = "Charmeleon";
            this.Skill = "Solar Power";
            this.SkillDmg = 10;
        }

        public override void CalculateDamage(int opponentSkillDmg)
        {
            int dmgTaken = 1 * opponentSkillDmg;
            this.HP -= dmgTaken;
        }
    }

}
