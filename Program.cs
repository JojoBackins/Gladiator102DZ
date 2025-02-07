using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

class Program
{
    static void Main(string[] args)
    {
        ReadInt();
    }

    public static void ReadInt()
    {
        const string CommandWatchFight = "1";
        const string CommandExit = "2";

        bool isWork = true;

        Console.WriteLine("Приветствуем на нашей арене!");

        while (isWork)
        {
            Console.WriteLine($"Меню:\n" +
                $"\n{CommandWatchFight} - просмотреть бой." +
                $"\n{CommandExit} - выйти из программы.\n");

            string ?userInput = Console.ReadLine();

            switch (userInput)
            {
                case CommandWatchFight:
                    StartFight();
                    break;

                case CommandExit:
                    isWork = false;
                    break;

                default:
                    Console.WriteLine("Не верная команда!");
                    break;
            }

            Console.ReadKey();
            Console.Clear();
        }
    }
    private static void StartFight()
    {
        List<Fighter> fighters = new List<Fighter>
        {
            new DoubleDamageFighter("Варвар", 6, 5, 100),
            new TripleAttackFighter("Берсерк", 8, 3, 100),
            new FuryFighter("Паладин", 6,2, 100),
            new FireballFighter("Маг", 12, 4, 100),
            new DodgeFighter("Каратель", 7, 7, 100)
        };

        Console.WriteLine("Доступные бойцы:");

        for (int i = 0; i < fighters.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {fighters[i].GetInfo()}");
        }

        Fighter fighter1 = SelectFighter(fighters);
        Fighter fighter2 = SelectFighter(fighters);

        Fight(fighter1, fighter2);
    }

    private static Fighter SelectFighter(List<Fighter> fighters)
    {
        int fighterIndex = -1;

        while (fighterIndex < 0 || fighterIndex >= fighters.Count)
        {
            Console.WriteLine("Выберите бойца. Введите номер: ");

            if(int.TryParse(Console.ReadLine(), out fighterIndex))
            {
                fighterIndex--; ;

                if(fighterIndex < 0 || fighterIndex > fighters.Count)
                {
                    Console.WriteLine("Неверный номер бойца. Пожалуйста попробуйте снова.");
                }
            }
            else
            {
                Console.WriteLine("Введите корректное число.");
            }
        }
        return fighters[fighterIndex].Clone();
    }

    private static void Fight(Fighter fighter1, Fighter fighter2)
    {
        Console.WriteLine($"{fighter1.Name} против {fighter2.Name}!");

        while (fighter1.Health > 0 && fighter2.Health > 0)
        {
            fighter1.Attack(fighter2);

            if (fighter2.Health > 0)
            {
                fighter2.Attack(fighter1);
            }

            Console.WriteLine($"{fighter1.Name}: {fighter1.Health} HP, {fighter2.Name}: {fighter2.Health} HP");
            Console.WriteLine();
        }

        if(fighter1.Health <= 0 && fighter2.Health <= 0)
        {
            Console.WriteLine("Ничья!");
        }
        else if (fighter1.Health <= 0)
        {
            Console.WriteLine($"{fighter2.Name} победил!");
        }
        else
        {
            Console.WriteLine($"{fighter1.Name} победил!");
        }

    }
}

public abstract class Fighter 
{
    protected Random random = new Random();

    public Fighter(string name, int damage, int protection, int health)
    {
        Name = name;
        Damage = damage;
        Protection = protection;
        Health = health;
    }

    public string Name { get; private set; }
    public int Damage { get; private set; }
    public int Protection { get; private set; }
    public int Health { get; protected set; }

    public virtual void Attack(Fighter opponent)
    {
        int damageDealt = Damage - opponent.Protection;

        if (damageDealt < 0) 
            damageDealt = 0;

        opponent.TakeDamage(damageDealt);

        Console.WriteLine($"{Name} атакует {opponent.Name} и наносит {damageDealt} урона.");
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;

        if (Health < 0)
            Health = 0;
    }

    public virtual string GetInfo()
    {
        return $"{Name}. Урон: {Damage}, Защита: {Protection}, Здоровья: {Health}";
    }

    public abstract Fighter Clone();
}

public class DoubleDamageFighter : Fighter
{
    public DoubleDamageFighter(string name, int damage, int protection, int health)
        : base(name, damage, protection, health) { }

    public override void Attack(Fighter opponent)
    {
        const int MinRandomNumber = 0;
        const int MaxRandomNumber = 100;
        const int HitChance = 20;

        int damageDealt = Damage - opponent.Protection;

        if (damageDealt < 0) 
            damageDealt = 0;

        if (random.Next(MinRandomNumber, MaxRandomNumber) < HitChance)
        {
            damageDealt *= 2;
            Console.WriteLine($"{Name} наносит удвоенный урон!");
        }

        opponent.TakeDamage(damageDealt);
        Console.WriteLine($"{Name} атакует {opponent.Name} и наносит {damageDealt} урона.");
    }

    public override string GetInfo()
    {
        return $"{Name}. Урон Варвара : {Damage}, Защита: {Protection}, Здоровья {Health}";
    }
    public override Fighter Clone()
    {
        return new DoubleDamageFighter(Name, Damage, Protection, Health);
    }
}

public class TripleAttackFighter : Fighter
{
    private int attackCount = 0;

    public TripleAttackFighter(string name, int damage, int protection, int health)
        : base(name, damage, protection, health) { }

    public override void Attack(Fighter opponent)
    {
        attackCount++;
        int damageDealt = Damage - opponent.Protection;

        if (damageDealt < 0)
            damageDealt = 0;

        if (attackCount % 3 == 0)
        {
            damageDealt *= 2;
            Console.WriteLine($"{Name} наносит двойной урон на третьей атаке!");
        }

        opponent.TakeDamage(damageDealt);
        Console.WriteLine($"{Name} атакует {opponent.Name} и наносит {damageDealt} урона.");
    }

    public override Fighter Clone()
    {
        return new TripleAttackFighter(Name, Damage, Protection, Health);
    }
}

public class FuryFighter : Fighter
{
    private int _fury = 0;
    private const int MaxFury = 10;

    public FuryFighter(string name, int damage, int protection, int health)
        : base(name, damage, protection, health) { }

    public override void Attack(Fighter opponent)
    {
        int damageDealth = Damage - opponent.Protection;

        if (damageDealth < 0) 
            damageDealth = 0;

        opponent.TakeDamage(damageDealth);
        _fury += damageDealth;
        Console.WriteLine($"{Name} атакует {opponent.Name} и наносит {damageDealth} урона. Ярость: {_fury}");

        if (_fury >= MaxFury)
        {
            Health += 10;
            _fury = 0;
            Console.WriteLine($"{Name} использует лечение!");
        }
    }
    public override Fighter Clone()
    {
        return new FuryFighter(Name, Damage, Protection, Health);
    }
}

public class FireballFighter : Fighter
{
    private int _mana = 20;

    public FireballFighter(string name, int damage, int protection, int health)
        : base(name, damage, protection, health) { }

    public override void Attack(Fighter opponent)
    {
        int damageDealt = Damage - opponent.Protection;

        if (damageDealt < 0)
            damageDealt = 0;

        if (_mana >= 5)
        {
            damageDealt += 5;
            _mana -= 5;
            Console.WriteLine($"{Name} использует Оненый шар!");
        }

        opponent.TakeDamage(damageDealt);
        Console.WriteLine($"{Name} атакует {opponent.Name} и наносит {damageDealt} урона. Мана: {_mana}");
    }

    public override Fighter Clone()
    {
        return new FireballFighter(Name, Damage, Protection, Health);
    }
}

public class DodgeFighter : Fighter
{
    public DodgeFighter(string name, int damage, int protection, int health)
        : base(name, damage, protection, health) { }

    public override void Attack(Fighter opponent)
    {
        int damageDealt = Damage - opponent.Protection;

        if (damageDealt < 0) 
            damageDealt = 0;

        if (random.Next(0, 100) < 30)
        {
            Console.WriteLine($"{Name} уклоняется от удара!");
            return;
        }

        opponent.TakeDamage(damageDealt);
        Console.WriteLine($"{Name} атакует {opponent.Name} и наносит {damageDealt} урона.");
    }

    public override Fighter Clone()
    {
        return new DodgeFighter(Name, Damage, Protection, Health);
    }
}