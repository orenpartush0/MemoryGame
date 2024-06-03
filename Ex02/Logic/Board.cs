using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ex02.Logic;

namespace Ex02
{
    internal class Board
    {
        private readonly Square[,] r_Squares;
        public List<Guess> GuessHistory { get; set; }
        public int CounterOfRevealedSquares { get; set; }
        public BoardSize BoardWidth { get; }
        public BoardSize BoardHeight { get; }

        public Board(BoardSize i_BoardWidth, BoardSize i_BoardHeight)
        {
            CounterOfRevealedSquares = 0;
            GuessHistory = new List<Guess>();
            BoardWidth = i_BoardWidth;
            BoardHeight = i_BoardHeight;
            r_Squares = new Square[(int)BoardHeight, (int)BoardWidth];
            int numOfSquares = (int)BoardWidth * (int)BoardHeight;
            List<int> availableSquares = Enumerable.Range(0, numOfSquares).ToList();
            Random rand = new Random();

            foreach (char letter in Enumerable.Range('A', numOfSquares / 2))
            {
                for (int i = 0; i < 2; i++)
                {
                    int index = rand.Next(availableSquares.Count);
                    int squareIndex = availableSquares[index];
                    availableSquares.Remove(squareIndex);
                    r_Squares[squareIndex / (int)BoardWidth, squareIndex % (int)BoardWidth] = new Square(letter);
                }
            }
        }

        public bool IsSquareRevealed(int i_Row, int i_Col)
        {
            return r_Squares[i_Row, i_Col].IsReveal;
        }

        public void RevealSquare(int i_Row, int i_Col)
        {
            r_Squares[i_Row, i_Col].IsReveal = true;
            char lastGuessLetter = r_Squares[i_Row, i_Col].Content;
            GuessHistory.Add(new Guess(i_Row, i_Col, lastGuessLetter));
        }

        public void HideSquare(int i_Row, int i_Col)
        {
            r_Squares[i_Row, i_Col].IsReveal = false;
        }

        public bool IsBoardFinished()
        {
            return (int)BoardHeight * (int)BoardWidth == CounterOfRevealedSquares;
        }

        public void RandomRevealSquare()
        {
            Random random = new Random();
            int numRows = r_Squares.GetLength(0);
            int numCols = r_Squares.GetLength(1);

            while (true)
            {
                int row = random.Next(numRows);
                int col = random.Next(numCols);

                if (!r_Squares[row, col].IsReveal)
                {
                    char lastGuessLetter = r_Squares[row, col].Content;
                    GuessHistory.Add(new Guess(row, col, lastGuessLetter));
                    break;
                }
            }
        }

        public char GetSquareLetter(int i_Row, int i_Col)
        {
            return r_Squares[i_Row, i_Col].Content;
        }

    }
}
