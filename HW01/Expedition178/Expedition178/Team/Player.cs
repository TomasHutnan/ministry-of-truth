using Expedition178.Entity;
using Expedition178.Utils;

namespace Expedition178.Team
{
    public class Player : Team
    {
        private Adventurer[] _adventurers = new Adventurer[3];
        private int[] _adventurerIndices = [0, 1, 2];

        public Player()
        {
            Console.WriteLine("Please, choose your three adventurers:");
            Adventurer[] choices = new Adventurer[6];
            for (int i = 0; i < choices.Length; i++)
            {
                choices[i] = new Adventurer();
                Console.Write($"{i + 1}. ");
                choices[i].WriteStats();
            }

            int[] chosenIndices = Input.GetAdventurerChoices();
            foreach (int i in chosenIndices)
            {
                _adventurers[i - 1] = choices[i - 1];
            }
        }

        public override void WriteStats()
        {
            for (int i = 0; i < _adventurers.Length; i++)
            {
                Console.Write($"{i + 1}. ");
                _adventurers[i].WriteStats();
            }
        }

        public Adventurer? GetNextAdventurer()
        {
            foreach (Adventurer adventurer in _adventurers)
            {
                if (adventurer.IsAlive)
                {
                    return adventurer;
                }
            }

            return null;
        }

        public void SortAdventurers()
        {
            Console.WriteLine("Choose the order:");


            _adventurerIndices = Input.GetSortChoices();
        }

        private void NextRound()
        {
            foreach (Adventurer adventurer in _adventurers)
            {
                adventurer.Heal();
            }

            _adventurerIndices = [0, 1, 2];
        }

        public void Victory()
        {
            int experienceGained = Random.Shared.Next(100, 200);
            foreach (Adventurer adventurer in _adventurers)
            {
                adventurer.GainExperience(experienceGained);
            }

            NextRound();
        }

        public void Defeat()
        {
            int experienceGained = Random.Shared.Next(50, 100);
            foreach (Adventurer adventurer in _adventurers)
            {
                adventurer.GainExperience(experienceGained);
            }

            NextRound();
        }
    }
}
