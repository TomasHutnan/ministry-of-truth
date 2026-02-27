using Expedition178.Enumerate;
using Expedition178.Utils;

namespace Expedition178.Entity
{
    public class Monster : Entity
    {
        public MonsterType MonsterType;

        public Monster(string name, int attack, int health, int speed, MonsterType monsterType) : base(name, attack, health, speed)
        {
            MonsterType = monsterType;
        }

        public override void WriteName()
        {
            ConsoleColor originalColor = Console.ForegroundColor;

            Console.ForegroundColor = MonsterType.ToColor();
            Console.Write(Name);
            Console.ForegroundColor = originalColor;
        }

        public override void WriteStats()
        {
            ConsoleColor originalColor = Console.ForegroundColor;

            Console.ForegroundColor = MonsterType.ToColor();
            Console.Write($"{Name} ({MonsterType.ToString()})");
            Console.ForegroundColor = originalColor;
            Console.Write($": {AttackPower} Attack, {Health} HP, {Speed} Speed");
        }

        public void Attack(Adventurer adventurer)
        {
            if (!IsAlive)
                throw new InvalidOperationException($"{this} is dead and cannot attack.");
            if (!adventurer.IsAlive)
                throw new InvalidOperationException($"{adventurer} is dead and cannot be attacked.");

            adventurer.TakeDamage(AttackPower);
        }
    }
}
