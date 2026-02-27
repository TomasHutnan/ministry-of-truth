using Expedition178.Enumerate;

namespace Expedition178.Utils
{
    public static class AttackTypeExtensions
    {
        public static ConsoleColor ToColor(this AttackType attackType) => attackType switch
        {
            AttackType.Physical => ConsoleColor.Gray,
            AttackType.Light => ConsoleColor.Yellow,
            AttackType.Dark => ConsoleColor.Magenta,
            AttackType.Ice => ConsoleColor.Cyan,
            AttackType.Fire => ConsoleColor.Red
        };
    }
}