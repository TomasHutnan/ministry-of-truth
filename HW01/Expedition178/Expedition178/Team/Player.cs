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
                Console.WriteLine();
            }

            int[] chosenPositions = Input.GetAdventurerChoices();
            for (int i = 0; i < _adventurers.Length; i++)
            {
                _adventurers[i] = choices[chosenPositions[i] - 1];  // Subtract 1 to convert from 1-based to 0-based index
            }

            Console.Write("You have chosen ");
            _adventurers[0].WriteName();
            Console.Write(", ");
            _adventurers[1].WriteName();
            Console.Write(", ");
            _adventurers[2].WriteName();
            Console.WriteLine(".");
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
            for (int i = 0; i < _adventurers.Length; i++)
            {
                _adventurerIndices[i]--;  // Subtract 1 to convert from 1-based to 0-based index
            }

            Console.WriteLine("The order of your adventurers has been updated.");
        }

        public override void HealTeam()
        {
            foreach (Adventurer adventurer in _adventurers)
            {
                adventurer.Heal();
            }
        }

        public void ResetOrder()
        {
            _adventurerIndices = [0, 1, 2];
        }

        public int GainExperience(int experienceGained)
        {
            // int experienceGained = Random.Shared.Next(100, 200);
            int levelUpCount = 0;
            foreach (Adventurer adventurer in _adventurers)
            {
                levelUpCount = adventurer.GainExperience(experienceGained);
            }
            return levelUpCount;
        }
    }
}
