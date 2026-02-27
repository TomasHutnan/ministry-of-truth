using Expedition178.Enumerate;
using Expedition178.Utils;

namespace Expedition178.Entity
{
    public class Adventurer : Entity
    {
        const int ExperienceToLevelUp = 100;

        private AttackType _attackType;

        private int _experience = 0;
        private int _level = 1;

        public Adventurer(string name, int attack, int health, int speed, AttackType attackType) : base(name, attack, health, speed)
        {
            _attackType = attackType;
        }

        public Adventurer() : base()
        {
            var attackTypes = Enum.GetValues<AttackType>();
            _attackType = attackTypes[Random.Shared.Next(attackTypes.Length)];
        }

        public override void WriteName()
        {
            ConsoleColor originalColor = Console.ForegroundColor;

            Console.ForegroundColor = _attackType.ToColor();
            Console.Write(Name);
            Console.ForegroundColor = originalColor;
        }

        public override void WriteStats()
        {
            ConsoleColor originalColor = Console.ForegroundColor;

            Console.ForegroundColor = _attackType.ToColor();
            Console.Write($"{Name} ({_attackType.ToString()})");
            Console.ForegroundColor = originalColor;
            Console.Write($": {AttackPower} Attack, {Health} HP, {Speed} Speed, Level {_level}, {_experience}/{ExperienceToLevelUp} XP");
        }

        public void Attack(Monster monster)
        {
            if (!IsAlive)
                throw new InvalidOperationException($"{this} is dead and cannot attack.");
            if (!monster.IsAlive)
                throw new InvalidOperationException($"{monster} is dead and cannot be attacked.");

            decimal multiplier = monster.MonsterType.AttackMultiplier(_attackType);
            int damage = (int)Math.Ceiling(AttackPower * multiplier);

            monster.TakeDamage(damage);
        }
    }
}
