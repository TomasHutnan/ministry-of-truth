namespace Expedition178.Entity
{
    public abstract class Entity
    {
        private int _attackPower;
        private int _health;
        private int _speed;

        protected string Name { get; }
        protected int AttackPower { 
            get => _attackPower;
            set
            {
                ArgumentOutOfRangeException.ThrowIfNegative(value);
                _attackPower = value;
            }
        }
        protected int Health
        {
            get => _health;
            set
            {
                ArgumentOutOfRangeException.ThrowIfNegative(value);
                _health = value;
            }
        }
        protected int Speed
        {
            get => _speed;
            set
            {
                ArgumentOutOfRangeException.ThrowIfNegative(value);
                _speed = value;
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
        }

        public virtual void TakeDamage(int damage)
        {
            Health -= damage;
            if (Health < 0)
                Health = 0;
        }

        public abstract void WriteName();
        public abstract void WriteStats();
    }
}
