using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ex02.Logic
{
    internal struct Guess
    {
        public int Col { set; get; }

        public int Row { set; get; }

        public char Letter { set; get; }

        public Guess(int i_Row, int i_Col, char i_Letter)
        {
            Col = i_Col;
            Row = i_Row;
            Letter = i_Letter;
        }
    }
}
