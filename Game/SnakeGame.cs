using C_Snake.Core;
using C_Snake.Entities;
using C_Snake.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C_Snake.Game
{
    class SnakeGame : IRenderable
    {
        private const int InnerRows = 30;
        private const int InnerCols = 40;

        private const int Rows = InnerRows + 2;
        private const int Cols = InnerCols + 2;

        private char[,] prevFrame = new char[Rows, Cols];
        private char[,] currFrame = new char[Rows, Cols];

        private ConsoleColor[,] prevColor = new ConsoleColor[Rows, Cols];
        private ConsoleColor[,] currColor = new ConsoleColor[Rows, Cols];

        private static readonly Position Origin = new Position(0, 0);

        private Direction _currentDirection;
        private Direction _nextDirection;
        private Snake _snake;
        private Apple _apple;
        public int Score { get; private set; } = 0;

        public SnakeGame()
        {
            _snake = new Snake(Origin, initialSize: 5);
            _apple = CreateApple();
            _currentDirection = Direction.Right;
            _nextDirection = Direction.Right;
        }

        private void BuildFrame()
        {
            // Fill frame with spaces and default color
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    currFrame[r, c] = ' ';
                    currColor[r, c] = ConsoleColor.White;
                }
            }

            // Draw horizontal borders
            for (int c = 0; c < Cols; c++)
            {
                currFrame[0, c] = '#';
                currColor[0, c] = ConsoleColor.White;

                currFrame[Rows - 1, c] = '#';
                currColor[Rows - 1, c] = ConsoleColor.White;
            }

            // Draw vertical borders
            for (int r = 0; r < Rows; r++)
            {
                currFrame[r, 0] = '#';
                currColor[r, 0] = ConsoleColor.White;

                currFrame[r, Cols - 1] = '#';
                currColor[r, Cols - 1] = ConsoleColor.White;
            }

            // Draw apple
            var appleRow = _apple.Position.Top + 1;
            var appleCol = _apple.Position.Left + 1;
            currFrame[appleRow, appleCol] = '@';
            currColor[appleRow, appleCol] = ConsoleColor.Red;

            // Draw snake head
            var headRow = _snake.Head.Top + 1;
            var headCol = _snake.Head.Left + 1;
            currFrame[headRow, headCol] = 'O';
            currColor[headRow, headCol] = ConsoleColor.Green;

            // Draw snake body
            foreach (var part in _snake.Body)
            {
                var row = part.Top + 1;
                var col = part.Left + 1;
                currFrame[row, col] = '■';
                currColor[row, col] = ConsoleColor.Green;
            }
        }


        private void RenderDiff()
        {
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    if (prevFrame[r, c] != currFrame[r, c] || prevColor[r, c] != currColor[r, c])
                    {
                        Console.SetCursorPosition(c, r);
                        Console.ForegroundColor = currColor[r, c];
                        Console.Write(currFrame[r, c]);
                    }
                }
            }

            // Swap buffers
            var tempFrame = prevFrame;
            prevFrame = currFrame;
            currFrame = tempFrame;

            var tempColor = prevColor;
            prevColor = currColor;
            currColor = tempColor;
        }

        private void RenderScoreBox()
        {
            string scoreText = $"Score: {Score}";
            string highScoreText = $"High: {HighScoreService.LoadHighScore()}";

            // Determine box width dynamically
            int maxTextLength = Math.Max(scoreText.Length, highScoreText.Length);
            int padding = 2; // space around text inside the box
            int boxWidth = maxTextLength + padding * 2;

            int boxLeft = Cols;
            int boxTop = 0;

            // Build box lines
            string topBorder = "+" + new string('-', boxWidth) + "+";
            string scoreLine = "|" + new string(' ', padding) + scoreText
                             + new string(' ', boxWidth - padding - scoreText.Length) + "|";
            string highScoreLine = "|" + new string(' ', padding) + highScoreText
                                 + new string(' ', boxWidth - padding - highScoreText.Length) + "|";
            string bottomBorder = "+" + new string('-', boxWidth) + "+";

            string[] lines = { topBorder, scoreLine, highScoreLine, bottomBorder };

            for (int i = 0; i < lines.Length; i++)
            {
                Console.SetCursorPosition(boxLeft, boxTop + i);
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(lines[i]);
            }
        }

        public bool GameOver => _snake.Dead;

        public void OnKeyPress(ConsoleKey key)
        {
            Direction newDirection;

            switch (key)
            {
                case ConsoleKey.W:
                    newDirection = Direction.Up;
                    break;

                case ConsoleKey.A:
                    newDirection = Direction.Left;
                    break;

                case ConsoleKey.S:
                    newDirection = Direction.Down;
                    break;

                case ConsoleKey.D:
                    newDirection = Direction.Right;
                    break;

                default:
                    return;
            }

            // Snake cannot turn 180 degrees.
            if (newDirection == OppositeDirectionTo(_currentDirection))
            {
                return;
            }

            _nextDirection = newDirection;
        }

        public void OnGameTick()
        {
            if (GameOver) throw new InvalidOperationException();

            _currentDirection = _nextDirection;
            _snake.Move(_currentDirection);

            // If the snake's head moves to the same position as an apple, the snake
            // eats it.
            if (_snake.Head.Equals(_apple.Position))
            {
                _snake.Grow();
                _apple = CreateApple();
                Score++;
            }
        }

        public void Render()
        {
            BuildFrame();
            RenderDiff();
            RenderScoreBox();

            Console.SetCursorPosition(0, 21);
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static Direction OppositeDirectionTo(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up: return Direction.Down;
                case Direction.Left: return Direction.Right;
                case Direction.Right: return Direction.Left;
                case Direction.Down: return Direction.Up;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        private Apple CreateApple()
        {
            const int numberOfRows = 30;
            const int numberOfColumns = 40;

            var random = new Random();
            Position position;

            do
            {
                var top = random.Next(0, numberOfRows);
                var left = random.Next(0, numberOfColumns);
                position = new Position(top, left);
            } while (!_snake.OnBody(position)); // Avoid overlap with head

            return new Apple(position);
        }
    }
}
