using Expedition178.Entity;
using Expedition178.Enumerate;

namespace Expedition178.Test
{
    public static class TestHelpers
    {
        public static Adventurer CreateAdventurer(
            string name = "TestAdventurer",
            int attack = 10,
            int health = 10,
            int speed = 10,
            AttackType type = AttackType.Physical)
            => new(name, attack, health, speed, type);

        public static Monster CreateMonster(
            string name = "TestMonster",
            int attack = 10,
            int health = 10,
            int speed = 10,
            MonsterType type = MonsterType.Nature)
            => new(name, attack, health, speed, type);
    }
}