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

        public Adventurer(string name, int attack, int health, int speed, AttackType attackType) : base(name, attack, health, speed, attackType.ToColor())
        {
            _attackType = attackType;
        }

        public Adventurer() : base()
        {
            var attackTypes = Enum.GetValues<AttackType>();
            _attackType = attackTypes[Random.Shared.Next(attackTypes.Length)];

            _color = _attackType.ToColor();
        }

        public override void WriteStats()
        {
            WriteColored($"{Name} ({_attackType.ToString()})");
            Console.Write($": {AttackPower} Attack, {Health} HP, {Speed} Speed, Level {_level}, {_experience}/{ExperienceToLevelUp} XP");
        }

        public int Attack(Monster monster)
        {
            if (!IsAlive)
                throw new InvalidOperationException($"{this} is dead and cannot attack.");
            if (!monster.IsAlive)
                throw new InvalidOperationException($"{monster} is dead and cannot be attacked.");

            decimal multiplier = monster.MonsterType.AttackMultiplier(_attackType);
            decimal rawDamage = AttackPower * multiplier;
            int damage = multiplier switch
            {
                1.5m => (int)Math.Ceiling(rawDamage),
                0.5m => (int)Math.Floor(rawDamage),
                _ => (int)rawDamage
            };

            monster.TakeDamage(damage);
            return damage;
        }

        private void LevelUp()
        {
            _level++;
            AttackPower += Random.Shared.Next(1, 4);
            MaxHealth += Random.Shared.Next(1, 4);
            Speed += Random.Shared.Next(1, 4);
        }

        public int GainExperience(int experience)
        {
            int levelUpCount = 0;

            _experience += experience;
            while (_experience >= ExperienceToLevelUp)
            {
                levelUpCount++;
                _experience -= ExperienceToLevelUp;
                LevelUp();
            }

            return levelUpCount;
        }
    }
}
