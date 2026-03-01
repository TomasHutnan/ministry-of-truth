using Expedition178.Interface;
using Expedition178.Team;
using Expedition178.Entity;

namespace Expedition178
{
    public class Battler : IBattle
    {
        public Team.Team Fight(Player player, Enemy enemy)
        {
            Adventurer? adventurer = player.GetNextAdventurer();
            Monster? creature = enemy.GetNextMonster();

            int round = 1;

            while (adventurer != null && creature != null)
            {
                Console.Write($"Round {round}: ");
                adventurer.WriteName();
                Console.Write(" vs. ");
                creature.WriteName();
                Console.WriteLine();

                Entity.Entity winner = Round(adventurer, creature);
                if (winner is Adventurer)
                {
                    creature.WriteName();
                    Console.WriteLine(" is defeated!");

                    creature = enemy.GetNextMonster();
                }
                else
                {
                    adventurer.WriteName();
                    Console.WriteLine(" is defeated!");

                    adventurer = player.GetNextAdventurer();
                }

                round++;
            }

            return adventurer != null ? player : enemy;
        }
        public Entity.Entity Round(Adventurer adventurer, Monster creature)
        {
            bool adventurersTurn = adventurer.Speed >= creature.Speed;

            while (adventurer.IsAlive && creature.IsAlive)
            {
                if (adventurersTurn)
                {
                    int damage = adventurer.Attack(creature);

                    adventurer.WriteName();
                    Console.Write($" dealt {damage} to ");
                    creature.WriteName();
                    Console.WriteLine(".");

                    creature.WriteName();
                    Console.WriteLine($" currently has {creature.Health} HP.");
                }
                else
                {
                    int damage = creature.Attack(adventurer);

                    creature.WriteName();
                    Console.Write($" dealt {damage} to ");
                    adventurer.WriteName();
                    Console.WriteLine(".");

                    adventurer.WriteName();
                    Console.WriteLine($" currently has {adventurer.Health} HP.");
                }
                adventurersTurn = !adventurersTurn;
            }

            return adventurer.IsAlive ? adventurer : creature;
        }
    }
}
