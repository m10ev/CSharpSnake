using C_Snake.Game;
using C_Snake.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace C_Snake
{
    public static class Program
    {
        static int highScore;
        public static async Task Main(string[] args)
        {
            ConsoleHelpers.MaximizeConsole();
            Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
            Console.SetBufferSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
            Console.CursorVisible = false;
            Console.Clear();

            var tickRate = TimeSpan.FromMilliseconds(100);

            highScore = HighScoreService.LoadHighScore();
            bool playAgain;

            do
            {
                Console.Clear();
                var snakeGame = new SnakeGame();
                playAgain = false;

                using (var cts = new CancellationTokenSource())
                {
                    async Task MonitorKeyPresses()
                    {
                        while (!cts.Token.IsCancellationRequested)
                        {
                            if (Console.KeyAvailable)
                            {
                                var key = Console.ReadKey(intercept: true).Key;
                                snakeGame.OnKeyPress(key);
                            }

                            await Task.Delay(10);
                        }
                    }

                    var monitorKeyPresses = MonitorKeyPresses();

                    try
                    {
                        while (!snakeGame.GameOver)
                        {
                            snakeGame.OnGameTick();
                            snakeGame.Render();
                            await Task.Delay(tickRate);
                        }
                    }
                    catch { }

                    // Allow time for user to weep.
                    for (var i = 0; i < 3; i++)
                    {
                        Console.Clear();
                        await Task.Delay(500);
                        snakeGame.Render();
                        await Task.Delay(500);
                    }

                    Console.Clear();
                    Console.SetCursorPosition(0, 1);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(@"░▒▓██████▓▒░ ░▒▓██████▓▒░░▒▓██████████████▓▒░░▒▓████████▓▒░       ░▒▓██████▓▒░░▒▓█▓▒░░▒▓█▓▒░▒▓████████▓▒░▒▓███████▓▒░  
░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░             ░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░      ░▒▓█▓▒░░▒▓█▓▒░ 
░▒▓█▓▒░      ░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░             ░▒▓█▓▒░░▒▓█▓▒░░▒▓█▓▒▒▓█▓▒░░▒▓█▓▒░      ░▒▓█▓▒░░▒▓█▓▒░ 
░▒▓█▓▒▒▓███▓▒░▒▓████████▓▒░▒▓█▓▒░░▒▓█▓▒░░▒▓█▓▒░▒▓██████▓▒░        ░▒▓█▓▒░░▒▓█▓▒░░▒▓█▓▒▒▓█▓▒░░▒▓██████▓▒░ ░▒▓███████▓▒░  
░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░             ░▒▓█▓▒░░▒▓█▓▒░ ░▒▓█▓▓█▓▒░ ░▒▓█▓▒░      ░▒▓█▓▒░░▒▓█▓▒░ 
░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░             ░▒▓█▓▒░░▒▓█▓▒░ ░▒▓█▓▓█▓▒░ ░▒▓█▓▒░      ░▒▓█▓▒░░▒▓█▓▒░ 
 ░▒▓██████▓▒░░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░░▒▓█▓▒░▒▓████████▓▒░       ░▒▓██████▓▒░   ░▒▓██▓▒░  ░▒▓████████▓▒░▒▓█▓▒░░▒▓█▓▒░ 
");

                    Console.SetCursorPosition(0, 10);
                    Console.WriteLine($"Your Score: {snakeGame.Score}");

                    if (snakeGame.Score > highScore)
                    {
                        highScore = snakeGame.Score;
                        HighScoreService.SaveHighScore(highScore);
                        Console.WriteLine("New High Score!");
                    }
                    else
                    {
                        Console.WriteLine($"High Score: {highScore}");
                    }

                    Console.WriteLine("\nPress [R] to Restart or [Q] to Quit.");

                    ConsoleKey responseKey;
                    do
                    {
                        responseKey = Console.ReadKey(true).Key;
                    } while (responseKey != ConsoleKey.R && responseKey != ConsoleKey.Q);

                    if (responseKey == ConsoleKey.R)
                    {
                        playAgain = true;
                    }


                    cts.Cancel();
                    await monitorKeyPresses;
                }

            } while (playAgain);
        }

       
    }
}
