using Expedition178.Entity;
using Expedition178.Enumerate;
using Expedition178.Test;
using Expedition178.Team;

namespace Expedition178.Test.Battler
{
    public class BattlerTests
    {
        // Round tests
        [Fact]
        public void Round_AdventurerWins_WhenMonsterHasLessHealth()
        {
            Adventurer adventurer = TestHelpers.CreateAdventurer(attack: 10, health: 50);
            Monster monster = TestHelpers.CreateMonster(attack: 10, health: 5);

            Expedition178.Battler battler = new();
            Entity.Entity winner = battler.Round(adventurer, monster);
            
            Assert.Equal(adventurer, winner);
            Assert.True(adventurer.IsAlive);
            Assert.False(monster.IsAlive);
            Assert.Equal(0, monster.Health);
            Assert.True(adventurer.Health > 0);
        }

        [Fact]
        public void Round_MonsterWins_WhenAdventurerHasLessHealth()
        {
            Adventurer adventurer = TestHelpers.CreateAdventurer(attack: 10, health: 5);
            Monster monster = TestHelpers.CreateMonster(attack: 10, health: 50);

            Expedition178.Battler battler = new();
            Entity.Entity winner = battler.Round(adventurer, monster);

            Assert.Equal(monster, winner);
            Assert.True(monster.IsAlive);
            Assert.False(adventurer.IsAlive);
            Assert.Equal(0, adventurer.Health);
            Assert.True(monster.Health > 0);
        }

        [Fact]
        public void Round_CorrectTurnOrder_WhenAdventurerIsFaster()
        {
            Adventurer adventurer = TestHelpers.CreateAdventurer(attack: 10, health: 1, speed: 2);
            Monster monster = TestHelpers.CreateMonster(attack: 10, health: 1, speed: 1);

            Expedition178.Battler battler = new();
            Entity.Entity winner = battler.Round(adventurer, monster);

            Assert.Equal(adventurer, winner);
            Assert.Equal(0, monster.Health);
            Assert.True(adventurer.IsAlive);
            Assert.False(monster.IsAlive);
        }

        [Fact]
        public void Round_CorrectTurnOrder_WhenSameSpeed()
        {
            Adventurer adventurer = TestHelpers.CreateAdventurer(attack: 10, health: 1, speed: 1);
            Monster monster = TestHelpers.CreateMonster(attack: 10, health: 1, speed: 1);

            Expedition178.Battler battler = new();
            Entity.Entity winner = battler.Round(adventurer, monster);

            Assert.Equal(adventurer, winner);
            Assert.Equal(0, monster.Health);
            Assert.True(adventurer.IsAlive);
            Assert.False(monster.IsAlive);
        }

        [Fact]
        public void Round_CorrectTurnOrder_WhenMonsterIsFaster()
        {
            Adventurer adventurer = TestHelpers.CreateAdventurer(attack: 10, health: 1, speed: 1);
            Monster monster = TestHelpers.CreateMonster(attack: 10, health: 1, speed: 2);

            Expedition178.Battler battler = new();
            Entity.Entity winner = battler.Round(adventurer, monster);

            Assert.Equal(monster, winner);
            Assert.Equal(0, adventurer.Health);
            Assert.True(monster.IsAlive);
            Assert.False(adventurer.IsAlive);
        }

        [Theory]
        [InlineData(AttackType.Physical, MonsterType.Nature)]
        [InlineData(AttackType.Physical, MonsterType.Radiant)]
        [InlineData(AttackType.Dark, MonsterType.Nature)]
        [InlineData(AttackType.Fire, MonsterType.Shadow)]
        [InlineData(AttackType.Fire, MonsterType.Radiant)]
        [InlineData(AttackType.Ice, MonsterType.Shadow)]
        [InlineData(AttackType.Ice, MonsterType.Radiant)]
        public void Round_AppliesAttackMultiplier_OnNoSynergy(AttackType attackType, MonsterType monsterType)
        {
            Adventurer adventurer = TestHelpers.CreateAdventurer(attack: 100, health: 1, speed: 2, type: attackType);
            Monster monsterWithLowHp = TestHelpers.CreateMonster(attack: 10, health: 100, speed: 1, type: monsterType);
            Monster monsterWithTooMuchHp = TestHelpers.CreateMonster(attack: 10, health: 101, speed: 1, type: monsterType);

            Expedition178.Battler battler = new();

            Entity.Entity winner = battler.Round(adventurer, monsterWithLowHp);
            Assert.Equal(adventurer, winner);
            Assert.Equal(0, monsterWithLowHp.Health);

            adventurer.Heal();

            winner = battler.Round(adventurer, monsterWithTooMuchHp);
            Assert.Equal(monsterWithTooMuchHp, winner);
            Assert.Equal(1, monsterWithTooMuchHp.Health);
            Assert.Equal(0, adventurer.Health);
        }

        [Theory]
        [InlineData(AttackType.Light, MonsterType.Shadow)]
        [InlineData(AttackType.Dark, MonsterType.Radiant)]
        [InlineData(AttackType.Fire, MonsterType.Nature)]
        [InlineData(AttackType.Ice, MonsterType.Nature)]
        public void Round_AppliesAttackMultiplier_OnWeakness(AttackType attackType, MonsterType monsterType)
        {
            Adventurer adventurer = TestHelpers.CreateAdventurer(attack: 100, health: 1, speed: 2, type: attackType);
            Monster monsterWithLowHp = TestHelpers.CreateMonster(attack: 10, health: 150, speed: 1, type: monsterType);
            Monster monsterWithTooMuchHp = TestHelpers.CreateMonster(attack: 10, health: 151, speed: 1, type: monsterType);

            Expedition178.Battler battler = new();

            Entity.Entity winner = battler.Round(adventurer, monsterWithLowHp);
            Assert.Equal(adventurer, winner);
            Assert.Equal(0, monsterWithLowHp.Health);

            adventurer.Heal();

            winner = battler.Round(adventurer, monsterWithTooMuchHp);
            Assert.Equal(monsterWithTooMuchHp, winner);
            Assert.Equal(1, monsterWithTooMuchHp.Health);
            Assert.Equal(0, adventurer.Health);
        }

        [Theory]
        [InlineData(AttackType.Physical, MonsterType.Shadow)]
        [InlineData(AttackType.Light, MonsterType.Nature)]
        public void Round_AppliesAttackMultiplier_OnResistance(AttackType attackType, MonsterType monsterType)
        {
            Adventurer adventurer = TestHelpers.CreateAdventurer(attack: 100, health: 1, speed: 2, type: attackType);
            Monster monsterWithLowHp = TestHelpers.CreateMonster(attack: 10, health: 50, speed: 1, type: monsterType);
            Monster monsterWithTooMuchHp = TestHelpers.CreateMonster(attack: 10, health: 51, speed: 1, type: monsterType);

            Expedition178.Battler battler = new();

            Entity.Entity winner = battler.Round(adventurer, monsterWithLowHp);
            Assert.Equal(adventurer, winner);
            Assert.Equal(0, monsterWithLowHp.Health);

            adventurer.Heal();

            winner = battler.Round(adventurer, monsterWithTooMuchHp);
            Assert.Equal(monsterWithTooMuchHp, winner);
            Assert.Equal(1, monsterWithTooMuchHp.Health);
            Assert.Equal(0, adventurer.Health);
        }

        [Theory]
        [InlineData(AttackType.Light, MonsterType.Radiant)]
        [InlineData(AttackType.Dark, MonsterType.Shadow)]
        public void Round_AppliesAttackMultiplier_OnImmunity(AttackType attackType, MonsterType monsterType)
        {
            Adventurer adventurer = TestHelpers.CreateAdventurer(attack: int.MaxValue, health: 1, speed: 2, type: attackType);
            Monster monster = TestHelpers.CreateMonster(attack: 10, health: 1, speed: 1, type: monsterType);

            Expedition178.Battler battler = new();

            Entity.Entity winner = battler.Round(adventurer, monster);
            Assert.Equal(monster, winner);
            Assert.Equal(0, adventurer.Health);
        }

        [Fact]
        public void Round_DamageDecreasesHealthCorrectly()
        {
            Adventurer adventurer = TestHelpers.CreateAdventurer(attack: 20, health: 100, speed: 10);
            Monster monster = TestHelpers.CreateMonster(attack: 15, health: 100, speed: 1);

            int initialAdventurerHealth = adventurer.Health;
            int initialMonsterHealth = monster.Health;

            Expedition178.Battler battler = new();
            Entity.Entity winner = battler.Round(adventurer, monster);

            Assert.Equal(40, adventurer.Health);
            Assert.True(adventurer.IsAlive);
        }

        // Fight tests
        [Fact]
        public void Fight_ReturnsPlayer_WhenAllMonstersDefeated()
        {
            Adventurer adventurer = TestHelpers.CreateAdventurer(name: "Strong Adventurer", attack: 100, health: 100, speed: 10);
            Monster monster1 = TestHelpers.CreateMonster(name: "Weak Monster 1", attack: 1, health: 10, speed: 1);
            Monster monster2 = TestHelpers.CreateMonster(name: "Weak Monster 2", attack: 1, health: 10, speed: 1);
            Monster monster3 = TestHelpers.CreateMonster(name: "Weak Monster 3", attack: 1, health: 10, speed: 1);

            Player player = new([adventurer, new Adventurer(), new Adventurer()]);
            Enemy enemy = new([monster1, monster2, monster3]);

            Expedition178.Battler battler = new();
            Team.Team winner = battler.Fight(player, enemy);

            Assert.Equal(player, winner);
            Assert.Null(enemy.GetNextMonster());
            Assert.True(player.GetNextAdventurer() != null);
            Assert.False(monster1.IsAlive);
            Assert.False(monster2.IsAlive);
            Assert.False(monster3.IsAlive);
        }

        [Fact]
        public void Fight_ReturnsEnemy_WhenAllAdventurersDefeated()
        {
            Adventurer adventurer1 = TestHelpers.CreateAdventurer(name: "Weak Adventurer 1", attack: 1, health: 10, speed: 1);
            Adventurer adventurer2 = TestHelpers.CreateAdventurer(name: "Weak Adventurer 2", attack: 1, health: 10, speed: 1);
            Adventurer adventurer3 = TestHelpers.CreateAdventurer(name: "Weak Adventurer 3", attack: 1, health: 10, speed: 1);
            Monster monster = TestHelpers.CreateMonster(name: "Strong Monster", attack: 100, health: 100, speed: 10);

            Player player = new([adventurer1, adventurer2, adventurer3]);
            Enemy enemy = new([monster, new Monster(), new Monster()]);

            Expedition178.Battler battler = new();
            Team.Team winner = battler.Fight(player, enemy);

            Assert.Equal(enemy, winner);
            Assert.Null(player.GetNextAdventurer());
            Assert.True(enemy.GetNextMonster() != null);
            Assert.False(adventurer1.IsAlive);
            Assert.False(adventurer2.IsAlive);
            Assert.False(adventurer3.IsAlive);
        }

        [Fact]
        public void Fight_CyclesTeamMembers_WhenOneCharacterDies()
        {
            Adventurer adventurer1 = TestHelpers.CreateAdventurer(name: "Adventurer 1", attack: 50, health: 20, speed: 10);
            Adventurer adventurer2 = TestHelpers.CreateAdventurer(name: "Adventurer 2", attack: 50, health: 20, speed: 1);
            Adventurer adventurer3 = TestHelpers.CreateAdventurer(name: "Adventurer 3", attack: 50, health: 100, speed: 10);
            Monster monster1 = TestHelpers.CreateMonster(name: "Monster 1", attack: 30, health: 25, speed: 5);
            Monster monster2 = TestHelpers.CreateMonster(name: "Monster 2", attack: 30, health: 25, speed: 15);
            Monster monster3 = TestHelpers.CreateMonster(name: "Monster 3", attack: 30, health: 25, speed: 5);

            Player player = new([adventurer1, adventurer2, adventurer3]);
            Enemy enemy = new([monster1, monster2, monster3]);

            Expedition178.Battler battler = new();
            Team.Team winner = battler.Fight(player, enemy);

            Assert.Equal(player, winner);
            Assert.False(adventurer1.IsAlive);
            Assert.False(adventurer2.IsAlive);
            Assert.False(monster1.IsAlive);
            Assert.False(monster2.IsAlive);
            Assert.False(monster3.IsAlive);
            Assert.True(adventurer3.IsAlive);
            Assert.Null(enemy.GetNextMonster());
        }

        [Fact]
        public void Fight_CyclesMonsterMembers_WhenOneMonsterDies()
        {
            Adventurer adventurer1 = TestHelpers.CreateAdventurer(name: "Adventurer 1", attack: 50, health: 100, speed: 10);
            Adventurer adventurer2 = TestHelpers.CreateAdventurer(name: "Adventurer 2", attack: 50, health: 100, speed: 10);
            Monster monster1 = TestHelpers.CreateMonster(name: "Monster 1", attack: 10, health: 30, speed: 5);
            Monster monster2 = TestHelpers.CreateMonster(name: "Monster 2", attack: 10, health: 30, speed: 5);
            Monster monster3 = TestHelpers.CreateMonster(name: "Monster 3", attack: 10, health: 30, speed: 5);

            Player player = new([adventurer1, adventurer2, new Adventurer()]);
            Enemy enemy = new([monster1, monster2, monster3]);

            Expedition178.Battler battler = new();
            Team.Team winner = battler.Fight(player, enemy);

            Assert.Equal(player, winner);
            Assert.False(monster1.IsAlive);
            Assert.False(monster2.IsAlive);
            Assert.False(monster3.IsAlive);
            Assert.True(adventurer1.IsAlive);
            Assert.True(adventurer2.IsAlive);
            Assert.Null(enemy.GetNextMonster());
        }

        [Fact]
        public void Fight_CorrectlyProgressesThroughMultipleBattles()
        {
            Adventurer adventurer = TestHelpers.CreateAdventurer(name: "Adventurer", attack: 60, health: 150, speed: 10);
            Monster monster1 = TestHelpers.CreateMonster(name: "Monster 1", attack: 25, health: 40, speed: 15);
            Monster monster2 = TestHelpers.CreateMonster(name: "Monster 2", attack: 30, health: 80, speed: 5);
            Monster monster3 = TestHelpers.CreateMonster(name: "Monster 3", attack: 35, health: 40, speed: 15);

            Player player = new([adventurer, new Adventurer(), new Adventurer()]);
            Enemy enemy = new([monster1, monster2, monster3]);

            Expedition178.Battler battler = new();
            Team.Team winner = battler.Fight(player, enemy);

            Assert.Equal(player, winner);
            Assert.True(adventurer.IsAlive);
            Assert.False(monster1.IsAlive);
            Assert.False(monster2.IsAlive);
            Assert.False(monster3.IsAlive);
            Assert.Equal(60, adventurer.Health);
            Assert.Null(enemy.GetNextMonster());
        }

        [Fact]
        public void Fight_RespectSpeedStat_InDeterminingTurnOrder()
        {
            Adventurer fastAdventurer = TestHelpers.CreateAdventurer(name: "Fast Adventurer", attack: 50, health: 1, speed: 20);
            Monster slowMonster1 = TestHelpers.CreateMonster(name: "Slow Monster 1", attack: 50, health: 10, speed: 1);
            Monster slowMonster2 = TestHelpers.CreateMonster(name: "Slow Monster 2", attack: 50, health: 10, speed: 1);
            Monster slowMonster3 = TestHelpers.CreateMonster(name: "Slow Monster 3", attack: 50, health: 10, speed: 1);

            Player player = new([fastAdventurer, new Adventurer(), new Adventurer()]);
            Enemy enemy = new([slowMonster1, slowMonster2, slowMonster3]);

            Expedition178.Battler battler = new();
            Team.Team winner = battler.Fight(player, enemy);

            Assert.Equal(player, winner);
            Assert.True(fastAdventurer.IsAlive);
        }
    }
}
