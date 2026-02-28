using Expedition178.Enumerate;

namespace Expedition178.Utils
{
    public static class Input
    {
        public static string ReadLine()
        {
            string? input = Console.ReadLine();
            while (string.IsNullOrEmpty(input))
            {
                Console.WriteLine("Invalid input. Input cannot be empty. Please try again.");
                input = Console.ReadLine();
            }
            return input;
        }

        public static string AskForInput()
        {
            Console.Write("[Player]: ");
            return ReadLine().Trim();
        }

        public static bool TryParseStringToIntArray(string input, out int[] ints, bool unique = true, int expectedLength = 3, int min = 1, int max = 3)
        {
            ints = new int[expectedLength];

            string[] parts = input.Split();
            if (parts.Length != expectedLength)
            {
                return false;
            }

            for (int i = 0; i < parts.Length; i++)
            {
                int parsedIdx;
                if (!int.TryParse(parts[i], out parsedIdx) || min > parsedIdx || max < parsedIdx || (unique && ints.Contains(parsedIdx)))
                {
                    return false;
                }

                ints[i] = parsedIdx;
            }

            return true;
        }

        public static int[] GetAdventurerChoices()
        {
            string[] splitInput = AskForInput().Split(' ', count: 2);
            int[] choices;

            while (splitInput.Length != 2 || splitInput[0] != "start" || !TryParseStringToIntArray(splitInput[1], out choices, max: 6))
            {
                Console.WriteLine("Invalid input. Please enter start and three numbers to choose your adventureres. E.g. 'start 2 5 1'");
                splitInput = AskForInput().Split(' ', count: 2);
            }

            return choices;
        }

        public static int[] GetSortChoices()
        {
            string input = AskForInput();
            int[] choices;

            while (!TryParseStringToIntArray(input, out choices))
            {
                Console.WriteLine("Invalid input. Please enter the desired order of your adventureres. E.g. '1 3 2'");
                input = AskForInput();
            }

            return choices;
        }

        public static CommandType GetCommand()
        {
            string input = AskForInput();
            CommandType commandType;

            while (!Enum.TryParse(input, true, out commandType))
            {
                Console.WriteLine("Invalid input. Please enter a valid command (check, fight, info, sort, quit).");
                input = AskForInput();
            }

            return commandType;
        }
    }
}
