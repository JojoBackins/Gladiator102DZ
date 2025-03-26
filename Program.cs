using System.Text;

namespace Homeswork
{
    public enum WarriorType
    {
        Swordsman,
        Mage,
        Knight,
        Pirate,
        Dodger
    }
    internal static class Program
    {
        private static void Main()
        {
            var warriorsConfigs =
                new List<(WarriorType type, float health, float defaultDamage)>
                {
                    (WarriorType.Swordsman, 100, 25),
                    (WarriorType.Knight, 110, 20),
                    (WarriorType.Pirate, 100, 25),
                    (WarriorType.Mage, 100, 20),
                    (WarriorType.Dodger, 90, 25),
                };

            var warriorsManager = new WarriorsManager();

            List<DefaultWarrior> availableWarriors = warriorsManager.CreateWarriors(warriorsConfigs);

            var battle = new WarriorsBattle(availableWarriors);

            battle.StartBattle();
        }
    }
    public class WarriorsBattle
    {
        private readonly List<DefaultWarrior> _availableWarriors;
        private readonly DefaultWarrior _firstBattleWarrior;
        private readonly DefaultWarrior _secondBattleWarrior;

        public WarriorsBattle(List<DefaultWarrior> availableWarriors)
        {
            _availableWarriors = availableWarriors;

            Console.WriteLine("          Приветствуем!\n" +
                              "   Пожалуйста, выберите двух бойцов:\n\n" +
                              $"{GetAvailableWarriorsString()}");

            (_firstBattleWarrior, _secondBattleWarrior) = SelectWarriors();
        }

        public void StartBattle()
        {
            var battleStage = 1;

            Console.WriteLine("       Да начнётся же бой!");

            while (_firstBattleWarrior.IsAlive && _secondBattleWarrior.IsAlive)
            {
                Console.WriteLine($"\n    Стадия боя номер {battleStage}:");
                battleStage++;
                _firstBattleWarrior.Attack(_secondBattleWarrior);
                _secondBattleWarrior.Attack(_firstBattleWarrior);
            }

            ShowBattleResults();
        }
        private void ShowBattleResults()
        {
            if (_firstBattleWarrior.IsAlive == false)
            {
                if (_secondBattleWarrior.IsAlive == false)
                {
                    Console.WriteLine("Ничья!");
                    return;
                }

                Console.WriteLine($"Победил {_secondBattleWarrior} - у него осталось {_secondBattleWarrior.Health} HP");
                return;
            }

            if (_secondBattleWarrior.IsAlive == false)
            {
                Console.WriteLine($"Победил {_firstBattleWarrior} - у него осталось {_firstBattleWarrior.Health} HP");
            }
        }
        private string GetAvailableWarriorsString()
        {
            var availableWarriors = new StringBuilder();
            var index = 0;

            foreach (DefaultWarrior warrior in _availableWarriors)
            {
                availableWarriors.Append($"          {warrior} - [{++index}]\n");
            }
            return availableWarriors.ToString();
        }
        private DefaultWarrior GetWarriorFromId(string rawId)
        {
            if (uint.TryParse(rawId, out uint id) == false)
            {
                throw new ArgumentException("Не удалось получить ID!");
            }

            id--;

            if (_availableWarriors.Count <= id)
            {
                throw new ArgumentException("Такого ID не существует!");
            }

            return _availableWarriors[(int)id];
        }
        private (DefaultWarrior, DefaultWarrior) SelectWarriors()
        {
            Console.Write("   Введите ID первого война: ");

            DefaultWarrior firstWarrior = GetWarriorFromId(Console.ReadLine()!);

            Console.Write("   Введите ID второго война: ");

            DefaultWarrior secondWarrior = GetWarriorFromId(Console.ReadLine()!);

            if (firstWarrior == secondWarrior)
            {
                secondWarrior = (Activator.CreateInstance(firstWarrior.GetType()) as DefaultWarrior)!;
            }

            Console.WriteLine("\n        Выбранные классы:\n" +
                              $"            1 - {firstWarrior}\n" +
                              $"            2 - {secondWarrior}");
            return (firstWarrior, secondWarrior);
        }
    }
    public class WarriorsManager
    {
        public List<DefaultWarrior> CreateWarriors(List<(WarriorType type, float health, float defaultDamage)> warriorsConfigs)
        {
            var createdWarriors = new List<DefaultWarrior>();

            foreach ((WarriorType type, float health, float defaultDamage) in warriorsConfigs)
            {
                switch (type)
                {
                    case WarriorType.Swordsman:
                        createdWarriors.Add(new Swordsman(health, defaultDamage));
                        break;
                    case WarriorType.Mage:
                        createdWarriors.Add(new Mage(health, defaultDamage));
                        break;
                    case WarriorType.Knight:
                        createdWarriors.Add(new Knight(health, defaultDamage));
                        break;
                    case WarriorType.Pirate:
                        createdWarriors.Add(new Pirate(health, defaultDamage));
                        break;
                    case WarriorType.Dodger:
                        createdWarriors.Add(new Dodger(health, defaultDamage));
                        break;
                    default:
                        throw new ArgumentException("Неверный тип бойца: " + type);
                }
            }

            return createdWarriors;
        }
    }

    public abstract class DefaultWarrior
    {
        protected DefaultWarrior(float health, float defaultDamage)
        {
            Health = health;
            DefaultDamage = defaultDamage;
        }

        public abstract string Name { get; }
        public float Health { get; private set; }
        public float DefaultDamage { get; private init; }
        public bool IsAlive => Health > 0;
        public virtual void Attack(DefaultWarrior enemy) => Attack(enemy, DefaultDamage);
        public override string ToString() => Name;

        protected virtual void TakeDamage(float damageAmount)
        {
            if (damageAmount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(damageAmount), "Значение должно быть больше 0");
            }
            Health -= damageAmount;
        }

        protected virtual void Attack(DefaultWarrior enemy, float damage)
        {
            enemy.TakeDamage(damage);

            if (enemy.IsAlive == false)
            {
                Console.WriteLine($"{enemy.Name} повержен ударом {Name} силой в {damage} урона!");
                return;
            }
            Console.WriteLine($"{Name} попытался нанести удар по {enemy.Name} - {damage} урона.\n" +
                              $"У противника осталось {enemy.Health} HP");
        }
    }

    public sealed class Swordsman : DefaultWarrior
    {
        public Swordsman(float health, float defaultDamage) : base(health, defaultDamage) { }

        public override string Name => "Мечник";
    }

    public sealed class Mage : DefaultWarrior
    {
        private const int _DefaultStrikesToStrongAttack = 3;
        private const float _StrongAttackDamage = 40f;
        private int _strikesToStrongAttack = _DefaultStrikesToStrongAttack;

        public Mage(float health, float defaultDamage) : base(health, defaultDamage) { }

        public override string Name => "Маг";

        public override void Attack(DefaultWarrior enemy)
        {
            if (_strikesToStrongAttack == 0)
            {
                _strikesToStrongAttack = _DefaultStrikesToStrongAttack;
                Attack(enemy, _StrongAttackDamage);
                return;
            }

            _strikesToStrongAttack--;
            base.Attack(enemy);
        }
    }

    public sealed class Knight : DefaultWarrior
    {
        private const float _DamageMultiply = 0.8f;

        public Knight(float health, float defaultDamage) : base(health, defaultDamage) { }

        public override string Name => "Рыцарь";
        protected override void TakeDamage(float damageAmount) => base.TakeDamage(damageAmount * _DamageMultiply);
    }

    public sealed class Pirate : DefaultWarrior
    {
        private const float _AmountOfAddedMultiplierPerDamage = 0.1f;

        private float _damageMultiplier = 1f;

        public Pirate(float health, float defaultDamage) : base(health, defaultDamage) { }

        public override string Name => "Пират";
        public override void Attack(DefaultWarrior enemy) => Attack(enemy, DefaultDamage * _damageMultiplier);

        protected override void TakeDamage(float damage)
        {
            _damageMultiplier += _AmountOfAddedMultiplierPerDamage;
            base.TakeDamage(damage);
        }
    }

    public sealed class Dodger : DefaultWarrior
    {
        private const int _MaxRandomNumber = 100;
        private const int _MinRandomNumberForDodge = 80;

        private static readonly Random _random = new();

        public Dodger(float health, float defaultDamage) : base(health, defaultDamage) { }

        public override string Name => "Ловкач";
        protected override void TakeDamage(float damageAmount)
        {
            if (_random.Next(_MaxRandomNumber) > _MinRandomNumberForDodge)
            {
                return;
            }

            base.TakeDamage(damageAmount);
        }
    }
}