using Expedition178.Entity;

namespace Expedition178.Team
{
    public class Enemy : Team
    {
        private Monster[] _monsters = new Monster[3];

        public Enemy()
        {
            for (int i = 0; i < _monsters.Length; i++)
            {
                _monsters[i] = new Monster();
            }
        }

        public override void WriteStats()
        {
            for (int i = 0; i < _monsters.Length; i++)
            {
                Console.Write($"{i + 1}. ");
                _monsters[i].WriteStats();
            }
        }

        public Monster? GetNextMonster()
        {
            foreach (Monster monster in _monsters)
            {
                if (monster.IsAlive)
                {
                    return monster;
                }
            }

            return null;
        }

        public override void HealTeam()
        {
            foreach (Monster monster in _monsters)
            {
                monster.Heal();
            }
        }
    }
}
