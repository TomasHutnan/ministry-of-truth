namespace Expedition178.Entity
{
    public abstract class Entity
    {
        private int _attackPower;
        private int _health;
        private int _speed;
        private int _maxHealth;

        public string Name { get; }
        public int AttackPower { 
            get => _attackPower;
            protected set
            {
                ArgumentOutOfRangeException.ThrowIfNegative(value);
                _attackPower = value;
            }
        }
        public int Health
        {
            get => _health;
            protected set
            {
                ArgumentOutOfRangeException.ThrowIfNegative(value);
                _health = value;
            }
        }
        public int Speed
        {
            get => _speed;
            protected set
            {
                ArgumentOutOfRangeException.ThrowIfNegative(value);
                _speed = value;
            }
        }
        protected int MaxHealth
        {
            get => _maxHealth;
            set
            {
                ArgumentOutOfRangeException.ThrowIfNegative(value);
                _maxHealth = value;
            }
        }

        public bool IsAlive => Health > 0;

        public Entity(string name, int attackPower, int health, int speed)
        {
            ArgumentException.ThrowIfNullOrEmpty(name, nameof(name));
            Name = name;

            AttackPower = attackPower;
            Health = health;
            Speed = speed;
            MaxHealth = Health;
        }

        public Entity()
        {
            Name = Utils.NameGenerator.GenerateName();
            AttackPower = Random.Shared.Next(1, 11);
            Health = Random.Shared.Next(1, 11);
            Speed = Random.Shared.Next(1, 11);
            MaxHealth = Health;
        }

        public virtual void TakeDamage(int damage)
        {
            Health -= damage;
            if (Health < 0)
                Health = 0;
        }

        public void Heal()
        {
            Health = _maxHealth;
        }

        public abstract void WriteName();
        public abstract void WriteStats();
    }
}
