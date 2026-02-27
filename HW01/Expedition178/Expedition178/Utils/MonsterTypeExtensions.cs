using Expedition178.Enumerate;

namespace Expedition178.Utils
{
    public static class MonsterTypeExtensions
    {
        public static ConsoleColor ToColor(this MonsterType monsterType) => monsterType switch
        {
            MonsterType.Nature => ConsoleColor.DarkGreen,
            MonsterType.Radiant => ConsoleColor.DarkYellow,
            MonsterType.Shadow => ConsoleColor.DarkBlue,
            _ => throw new ArgumentOutOfRangeException(nameof(monsterType), $"Not expected monster type value: {monsterType}")
        };
        
        public static decimal AttackMultiplier(this MonsterType monsterType, AttackType attackType) => (monsterType, attackType) switch
        {
            (MonsterType.Nature, AttackType.Fire) => 1.5m,
            (MonsterType.Nature, AttackType.Ice) => 1.5m,
            (MonsterType.Nature, AttackType.Light) => 0.5m,
            (MonsterType.Radiant, AttackType.Dark) => 1.5m,
            (MonsterType.Radiant, AttackType.Light) => 0m,
            (MonsterType.Shadow, AttackType.Light) => 1.5m,
            (MonsterType.Shadow, AttackType.Physical) => 0.5m,
            (MonsterType.Shadow, AttackType.Dark) => 0m,
            _ => 1m
        };
    }
}