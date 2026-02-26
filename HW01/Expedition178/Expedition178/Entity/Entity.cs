using System;
using System.Collections.Generic;
using System.Text;

namespace Expedition178.Entity
{
    abstract class Entity
    {
        protected int Attack { get; private set; }
        protected int Health { get; private set; }
        protected int Speed { get; private set; }

        public Entity(int attack, int health, int speed)
        {
            Attack = attack;
            Health = health;
            Speed = speed;
        }
    }
}
