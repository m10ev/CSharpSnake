using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C_Snake.Core
{
    readonly struct Position
    {
        public Position(int top, int left)
        {
            Top = top;
            Left = left;
        }
        public int Top { get; }
        public int Left { get; }

        public Position RightBy(int n) => new Position(Top, Left + n);
        public Position DownBy(int n) => new Position(Top + n, Left);
    }
}
