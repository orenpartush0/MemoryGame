using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ex02
{
    internal class Square
    {
        public bool IsReveal { set; get; }
        public char Content { get; }

        public Square(char i_Content)
        {
            IsReveal = false;
            Content = i_Content;
        }
    }
}
