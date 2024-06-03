using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ex02.Logic
{
    internal class Turn
    {
        public int CurrentPlayerIndex { set; get; }
        public bool IsSecondGuess { get; set; }

        private readonly List<string> r_Players;


        public Turn(List<string> i_Players)
        {
            CurrentPlayerIndex = 0;
            r_Players = i_Players;
        }

        public string GetCurrentPlayerName()
        {
            return r_Players[CurrentPlayerIndex];
        }

        public void NextTurn()
        {
            CurrentPlayerIndex = (CurrentPlayerIndex + 1) % r_Players.Count;
        }

    }
}
