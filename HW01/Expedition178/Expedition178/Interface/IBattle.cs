using Expedition178.Entity;
using Expedition178.Team;

namespace Expedition178.Interface
{
    public interface IBattle
    {
        /// <summary>
        /// Performs a battle between the player's team of adventurers and a team of enemy creatures.
        /// </summary>
        /// <param name="player">The player's team</param>
        /// <param name="enemy">The enemy team</param>
        /// <returns>The winner</returns>
        Team.Team Fight(Player player, Enemy enemy);

        /// <summary>
        /// Performs one round of battle between two characters.
        /// </summary>
        /// <param name="adventurer">The player's adventurer</param>
        /// <param name="creature">The enemy creature</param>
        /// <returns>The character that wins the round</returns>
        Entity.Entity Round(Adventurer adventurer, Monster creature);
    }
}