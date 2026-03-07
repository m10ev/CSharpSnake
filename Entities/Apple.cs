using C_Snake.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C_Snake.Entities
{
    class Apple : IRenderable
    {
        public Apple(Position position)
        {
            Position = position;
        }

        public Position Position { get; }

        public void Render()
        {
            Console.SetCursorPosition(Position.Left, Position.Top);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("@");
        }
    }
}
