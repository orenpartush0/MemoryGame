using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Remoting.Lifetime;
using System.Text;
using Ex02.Logic;


namespace Ex02
{
    internal class CmdUserInterface
    {
        private const int k_2Sec = 2000;
        private const int k_HumanRival = 1;
        private readonly List<string> r_Instructions;
        private readonly MemoryGame r_Game;

        public CmdUserInterface()
        {
            Console.OutputEncoding = Encoding.UTF8;
            SupportedLanguage Language = getValidLanguage();
            string filePath = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "..", "..", "LanguageFiles", $"{(Language == SupportedLanguage.English ? "English" : "Japanese")}GameOrders.txt");
            r_Instructions = new List<string>(File.ReadAllLines(filePath));
            setGameConfiguration(out string firstPlayerName, out string secondPlayerName, out PlayerType rival, out Board board);
            List<PlayerType> playerTypes = new List<PlayerType> { PlayerType.Human, rival };
            List<string> playersName = new List<string> { firstPlayerName, secondPlayerName };
            r_Game = new MemoryGame(board, playersName, playerTypes);
        }

        public void StartGame()
        {
            Console.Clear();
            while (!r_Game.Board.IsBoardFinished())
            {
                printBoard();
                Console.WriteLine($"{r_Game.GetCurrentPlayerName()} {r_Instructions[(int)GameText.Turn]}");
                selectSquare(out bool validMove);
                bool isSucceededToGuess = r_Game.UpdateBoardAndData(validMove);
                if (validMove)
                {
                    Console.Clear();
                    printBoard();
                    if (!isSucceededToGuess && r_Game.IsSecondGuessOfPlayer() && validMove)
                    {
                        r_Game.HideSquaresAfter2Turns();
                        System.Threading.Thread.Sleep(k_2Sec);
                    }

                    r_Game.FinishMove();
                    Console.Clear();
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine(r_Instructions[(int)GameText.SquareUnavailable] + "\n");
                }
            }

            Console.Clear();
            Console.WriteLine($"{r_Game.Winner()} {r_Instructions[(int)GameText.Won]}");
        }

        SupportedLanguage getValidLanguage()
        {
            SupportedLanguage supportedLanguage = SupportedLanguage.English;
            bool validInput = false;

            while (!validInput)
            {
                Console.WriteLine("For English, please press 1. For Japanese, please press 2.");
                string inputLanguage = Console.ReadLine();

                if (int.TryParse(inputLanguage, out int intInputLanguage) && Enum.IsDefined(
                       typeof(SupportedLanguage),
                       intInputLanguage))
                {
                    supportedLanguage = (SupportedLanguage)intInputLanguage;
                    validInput = true;
                }
                else
                {
                    Console.WriteLine("invalid input");
                }
            }

            return supportedLanguage;
        }

        private void setGameConfiguration(
            out string o_FirstPlayerName,
            out string o_SecondPlayerName,
            out PlayerType o_Rival,
            out Board o_Board)
        {
            Console.WriteLine(r_Instructions[(int)GameText.ProvideName]);
            o_FirstPlayerName = Console.ReadLine();
            setRival(out o_Rival, out o_SecondPlayerName);
            getBoardSize(out int boardWidth, out int boardHeight);
            o_Board = new Board(boardWidth, boardHeight);
        }

        private void setRival(out PlayerType o_Rival, out string o_SecondPlayerName)
        {
            int choiceInput;
            Console.WriteLine(r_Instructions[(int)GameText.PlayMode]);
            while (true)
            {
                if (int.TryParse(Console.ReadLine(), out choiceInput))
                {
                    if (Enum.IsDefined(typeof(PlayerType), choiceInput))
                    {
                        break;
                    }
                }
                Console.WriteLine(r_Instructions[(int)GameText.InvalidInput]);
            }
            /
            o_Rival = choiceInput == k_HumanRival ? PlayerType.Human : PlayerType.Computer;
            if (o_Rival == PlayerType.Human)
            {
                Console.WriteLine(r_Instructions[(int)GameText.OtherPlayerName]);
                o_SecondPlayerName = Console.ReadLine();
            }
            else
            {
                o_SecondPlayerName = "Computer";
            }
        }

        private void getBoardSize(out int o_Width, out int o_Height)
        {
            while (true)
            {
                Console.WriteLine(r_Instructions[(int)GameText.EnterWidth]);
                while (!int.TryParse(Console.ReadLine(), out o_Width) || !Enum.IsDefined(typeof(BoardSize), o_Width))
                {
                    Console.WriteLine(r_Instructions[(int)GameText.InvalidInput]);
                }

                Console.WriteLine(r_Instructions[(int)GameText.EnterHeight]);
                while (!int.TryParse(Console.ReadLine(), out o_Height) || !Enum.IsDefined(typeof(BoardSize), o_Height))
                {
                    Console.WriteLine(r_Instructions[(int)GameText.EnterWidth]);
                }

                if ((o_Height * o_Width) % 2 == 0)
                {
                    break;
                }
                else
                {
                    Console.WriteLine(r_Instructions[(int)GameText.HeightWidthEven]);
                }
            }
        }

        private void selectSquare(out bool o_ValidMove)
        {
            string selectedSquare = "";

            if (r_Game.IsHumanPlay())
            {
                Console.WriteLine(r_Instructions[(int)GameText.ChooseSquare]);
                while (true)
                {
                    selectedSquare = Console.ReadLine();
                    if (isSelectedSquareIsValid(selectedSquare))
                    {
                        break;
                    }
                    else
                    {
                        Console.WriteLine(r_Instructions[(int)GameText.InvalidInput]);
                    }
                }
            }

            o_ValidMove = r_Game.ValidGameMove(selectedSquare);
        }



        private bool isSelectedSquareIsValid(string i_SelectedSquare)
        {
            bool selectedSquareSize = i_SelectedSquare.Length == 2;
            char lastValidLetter = (char)('A' + r_Game.Board.BoardWidth - 1);
            bool validLetter = i_SelectedSquare[0] <= lastValidLetter && i_SelectedSquare[0] >= 'A';
            int lastValidNumber = (int)r_Game.Board.BoardHeight;
            bool isNumber = int.TryParse(i_SelectedSquare[1].ToString(), out int digit);
            bool validNumber = digit <= lastValidNumber;

            return validLetter && validNumber && selectedSquareSize && isNumber;
        }

        private void printBoard()
        {
            StringBuilder sb = new StringBuilder();

            foreach (char currentChar in Enumerable.Range('A', (int)r_Game.Board.BoardWidth))
            {
                sb.AppendFormat("   {0}", currentChar);
            }

            sb.Append("\n");
            string repeatedEquals = new string('=', 4 * (int)r_Game.Board.BoardWidth + 1);
            foreach (int row in Enumerable.Range(0, (int)r_Game.Board.BoardHeight))
            {
                sb.AppendLine(repeatedEquals);
                sb.Append($"{row + 1}");
                foreach (int col in Enumerable.Range(0, (int)r_Game.Board.BoardWidth))
                {
                    sb.AppendFormat(
                        r_Game.Board.IsSquareRevealed(row, col)
                            ? $"| {r_Game.Board.GetSquareLetter(row, col)} "
                            : "|   ");
                }

                sb.Append("|\n");
            }

            sb.AppendLine(repeatedEquals);
            Console.WriteLine(sb.ToString());
        }
    }
}

