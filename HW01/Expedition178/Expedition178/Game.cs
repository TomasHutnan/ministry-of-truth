using System;
using System.Collections.Generic;
using System.Text;
using Expedition178.Interface;
using Expedition178.Team;
using Expedition178.Utils;
using Expedition178.Enumerate;

namespace Expedition178
{
    public class Game : IGame
    {
        private Player? _player;
        private Enemy? _enemy;
        private static readonly Battle _battler = new Battle();

        private int _wavesBeaten = 0;

        public void Start()
        {
            Console.WriteLine("Welcome to Expedition 178!");

            _player = new Player();
            _enemy = new Enemy();

            Console.WriteLine("Your commands are: check, fight, info, sort, quit.");

            Run();
        }

        private void Run()
        {
            bool isRunning = true;
            while (isRunning)
            {
                switch (Input.GetCommand())
                {
                    case CommandType.Check:
                        Console.WriteLine("You see the next wave of monsters:");
                        _enemy!.WriteStats();
                        break;
                    case CommandType.Fight:
                        Fight();
                        if (_wavesBeaten >= 3)
                        {
                            isRunning = false;
                        }
                        break;
                    case CommandType.Info:
                        Console.WriteLine($"Waves defeated: {_wavesBeaten}, the adventurers:");
                        _player!.WriteStats();
                        break;
                    case CommandType.Sort:
                        _player!.SortAdventurers();
                        break;
                    case CommandType.Quit:
                        isRunning = false;
                        break;
                }
            }

            if (_wavesBeaten >= 3)
            {
                Console.WriteLine($"Congratulations! You have completed Expedition 178 by defeating {_wavesBeaten} waves of monsters!");
            }
            else
            {
                Console.WriteLine("You have lost the game :(");
            }

            Console.WriteLine("Thanks for playing Expedition 178!");
        }

        private void Fight()
        {
            Team.Team winner = _battler.Fight(_player!, _enemy!);

            if (winner == _player)
            {
                _wavesBeaten++;
                int experienceGained = Random.Shared.Next(100, 200);
                int levelUpCount = _player.GainExperience(experienceGained);

                Console.Write($"You won the battle! ");
                WriteExperienceGained(experienceGained, levelUpCount);
                Console.WriteLine();

                _enemy = new Enemy();
            }
            else
            {
                int experienceGained = Random.Shared.Next(25, 75);
                int levelUpCount = _player!.GainExperience(experienceGained);

                Console.Write($"The adventurers lost the battle. ");
                WriteExperienceGained(experienceGained, levelUpCount);
                Console.WriteLine();
            }

            _player.HealTeam();
            _player.ResetOrder();
            _enemy!.HealTeam();
        }

        private void WriteExperienceGained(int experienceGained, int levelUpCount)
        {
            Console.Write($"The adventurers gained {experienceGained} XP!");
            if (levelUpCount > 0)
            {
                Console.Write($" The adventurers leveled up {levelUpCount} time{(levelUpCount > 1 ? "s" : "")}!");
            }
        }
    }
}
