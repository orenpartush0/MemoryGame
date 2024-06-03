using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ex02.Logic;

namespace Ex02
{
    internal class MemoryGame
    {
        private readonly List<int> r_CounterOfRevealedCards;
        private readonly List<string> r_Players;
        private readonly List<PlayerType> r_PlayerTypes;
        private readonly Turn r_Turn;

        public Board Board { get; }

        public MemoryGame(Board i_Board, List<string> i_PlayersName, List<PlayerType> i_PlayerTypes)
        {
            r_Players = i_PlayersName;
            r_PlayerTypes = i_PlayerTypes;
            r_Turn = new Turn(i_PlayersName);
            Board = i_Board;
            r_CounterOfRevealedCards = new List<int>();
            r_CounterOfRevealedCards = Enumerable.Repeat(0, r_Players.Count).ToList();
        }

        public bool ValidGameMove(string i_SelectedSquare)
        {
            bool validMove;

            if (r_PlayerTypes[r_Turn.CurrentPlayerIndex] == PlayerType.Human)
            {
                int boardCol = i_SelectedSquare[0] - 'A';
                int boardRow = i_SelectedSquare[1] - 1 - '0';
                validMove = !Board.IsSquareRevealed(boardRow, boardCol);

                if (validMove)
                {
                    Board.RevealSquare(boardRow, boardCol);
                }
            }
            else
            {
                validMove = true;
                Board.RandomRevealSquare();
            }

            return validMove;
        }

        public bool UpdateBoardAndData(bool i_ValidMove)
        {
            bool isSucceeded = false;

            if (i_ValidMove)
            {
                int size = Board.GuessHistory.Count;

                if (r_Turn.IsSecondGuess && Board.GuessHistory[size - 1].Letter == Board.GuessHistory[size - 2].Letter)
                {
                    r_CounterOfRevealedCards[r_Turn.CurrentPlayerIndex] += 2;
                    Board.CounterOfRevealedSquares += 2;
                    isSucceeded = true;
                }
                else if (r_Turn.IsSecondGuess && Board.GuessHistory[size - 1].Letter != Board.GuessHistory[size - 2].Letter)
                {
                    r_Turn.NextTurn();
                }

            }

            return isSucceeded;
        }

        public void HideSquaresAfter2Turns()
        {
            int size = Board.GuessHistory.Count;
            Board.HideSquare(Board.GuessHistory[size - 1].Row, Board.GuessHistory[size - 1].Col);
            Board.HideSquare(Board.GuessHistory[size - 2].Row, Board.GuessHistory[size - 2].Col);
        }


        public string GetCurrentPlayerName()
        {
            return r_Turn.GetCurrentPlayerName();
        }
        private int findMaxIndex(List<int> list)
        {
            int maxIndex = 0;
            int maxValue = list[0];

            for (int i = 1; i < list.Count; i++)
            {
                if (list[i] > maxValue)
                {
                    maxValue = list[i];
                    maxIndex = i;
                }
            }

            return maxIndex;
        }

        public string Winner()
        {
            int winnerPlayerIndex = findMaxIndex(r_CounterOfRevealedCards);

            return r_Players[winnerPlayerIndex];
        }

        public bool IsSecondGuessOfPlayer()
        {
            return r_Turn.IsSecondGuess;
        }

        public void FinishMove()
        {
            r_Turn.IsSecondGuess = !r_Turn.IsSecondGuess;
        }

        public bool IsHumanPlay(int i_PlayerIndex)
        {
            return r_PlayerTypes[i_PlayerIndex].Equals(PlayerType.Human);
        }

    }
}
