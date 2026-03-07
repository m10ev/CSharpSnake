using C_Snake.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C_Snake.Entities
{
    class Snake : IRenderable
    {
        private List<Position> _body;
        private int _growthSpurtsRemaining;

        public Snake(Position spawnLocation, int initialSize = 1)
        {
            _body = new List<Position> { spawnLocation };
            _growthSpurtsRemaining = Math.Max(0, initialSize - 1);
            Dead = false;
        }

        public bool Dead { get; private set; }
        public Position Head => _body.First();
        public IEnumerable<Position> Body => _body.Skip(1);

        public void Move(Direction direction)
        {
            if (Dead) throw new InvalidOperationException();

            Position newHead;

            switch (direction)
            {
                case Direction.Up:
                    newHead = Head.DownBy(-1);
                    break;

                case Direction.Left:
                    newHead = Head.RightBy(-1);
                    break;

                case Direction.Down:
                    newHead = Head.DownBy(1);
                    break;

                case Direction.Right:
                    newHead = Head.RightBy(1);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (_body.Contains(newHead) || !PositionIsValid(newHead))
            {
                Dead = true;
                return;
            }

            _body.Insert(0, newHead);

            if (_growthSpurtsRemaining > 0)
            {
                _growthSpurtsRemaining--;
            }
            else
            {
                _body.RemoveAt(_body.Count - 1);
            }
        }

        public void Grow()
        {
            if (Dead) throw new InvalidOperationException();

            _growthSpurtsRemaining++;
        }

        public void Render()
        {
            Console.SetCursorPosition(Head.Left, Head.Top);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("o");

            foreach (var position in Body)
            {
                Console.SetCursorPosition(position.Left, position.Top);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("■");
            }
        }

        public bool OnBody(Position position)
        {
            return _body.Contains(position);
        }

        public void Kill()
        {
            Dead = true;
        }

        private static bool PositionIsValid(Position position) =>
            position.Top >= 0 &&
            position.Left >= 0 &&
            position.Top < 28 &&
            position.Left < 40;
    }

}
