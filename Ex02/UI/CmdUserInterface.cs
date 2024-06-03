using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Ex02.ConsoleUtils;

namespace Ex02
{
    internal class CmdUserInterface
    {
        private const int k_2Sec = 2000;
        private const int k_HumanRival = 1;
        private readonly List<string> r_Instructions;
        private readonly MemoryGame r_Game;
        private bool IsPvP { set; get; }

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
            Screen.Clear();

            while (!r_Game.Board.IsBoardFinished())
            {
                clearAndPrintBoard();
                Console.WriteLine($"{r_Game.GetCurrentPlayerName()} {r_Instructions[(int)GameText.Turn]}");
                bool validMove = selectSquare();

                if (validMove)
                {
                    r_Game.UpdateData();
                    if (r_Game.IsSecondGuessOfPlayer() && !isSucceededToGuess())
                    {
                        clearAndPrintBoard();
                        delay(k_2Sec);
                        r_Game.HideSquaresAfter2Turns();
                    }

                    handleValidMove();
                }
                else
                {
                    notifyInvalidSquare();
                    delay(k_2Sec);
                }
            }

            Screen.Clear();
            bool tie = r_Game.Winner() == null;
            Console.WriteLine($"{(tie ? r_Instructions[(int)GameText.Tie] : r_Game.Winner())} {(tie ? "" : r_Instructions[(int)GameText.Won])}");
        }

        private void clearAndPrintBoard()
        {
            Screen.Clear();
            printBoard();
        }

        private bool isSucceededToGuess()
        {
            return r_Game.Board.IsTwoInRow();
        }

        private bool selectSquare()
        {
            selectSquare(out bool validMove);

            return validMove;
        }

        private void handleValidMove()
        {
            Screen.Clear();
            printBoard();
            r_Game.NextOne();
            r_Game.FinishMove();
        }

        private void notifyInvalidSquare()
        {
            Screen.Clear();
            Console.WriteLine(r_Instructions[(int)GameText.SquareUnavailable] + "\n");
        }

        private void delay(int i_Milliseconds)
        {
            System.Threading.Thread.Sleep(i_Milliseconds);
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
            setMode();
            setRival(out o_Rival, out o_SecondPlayerName);
            getBoardSize(out int boardWidth, out int boardHeight);
            o_Board = new Board(boardWidth, boardHeight);
        }

        private void setMode()
        {
            Console.WriteLine(r_Instructions[(int)GameText.PlayMode]);
            int choiceInput;
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

            IsPvP = choiceInput == k_HumanRival ? true : false;
        }

        private void setRival(out PlayerType o_Rival, out string o_SecondPlayerName)
        {

            o_Rival = IsPvP ? PlayerType.Human : PlayerType.Computer;
            if (o_Rival == PlayerType.Human)
            {
                Console.WriteLine(r_Instructions[(int)GameText.OtherPlayerName]);
                o_SecondPlayerName = Console.ReadLine();
            }
            else
            {
                o_SecondPlayerName = r_Instructions[(int)GameText.Computer];
            }
        }

        private void setBoardLen(out int o_Len, int i_TextToPrint)
        {
            Console.WriteLine(r_Instructions[i_TextToPrint]);
            while (!int.TryParse(Console.ReadLine(), out o_Len) || !Enum.IsDefined(typeof(BoardSize), o_Len))
            {
                Console.WriteLine(r_Instructions[(int)GameText.InvalidInput]);
            }
        }
        private void getBoardSize(out int o_Width, out int o_Height)
        {
            while (true)
            {
                setBoardLen(out o_Width, (int)GameText.EnterWidth);
                setBoardLen(out o_Height, (int)GameText.EnterHeight);
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
            bool isNumber = false; bool validNumber = false;
            bool selectedSquareSize = i_SelectedSquare.Length == 2;
            char lastValidLetter = (char)('A' + r_Game.Board.BoardWidth - 1);
            bool validLetter = i_SelectedSquare[0] <= lastValidLetter && i_SelectedSquare[0] >= 'A';
            int lastValidNumber = (int)r_Game.Board.BoardHeight;

            if (selectedSquareSize)
            {
                isNumber = int.TryParse(i_SelectedSquare[1].ToString(), out int digit);
                validNumber = digit <= lastValidNumber;
            }

            return validLetter && validNumber && selectedSquareSize && isNumber;
        }

        private void printBoard()
        {
            StringBuilder sb = new StringBuilder();

            for (char currentChar = 'A'; currentChar < 'A' + r_Game.Board.BoardWidth; currentChar++)
            {
                sb.AppendFormat("   {0}", currentChar);
            }
            sb.AppendLine();

            string repeatedEquals = new string('=', 4 * r_Game.Board.BoardWidth + 1);

            for (int row = 0; row < r_Game.Board.BoardHeight; row++)
            {
                sb.AppendLine(repeatedEquals);
                sb.Append($"{row + 1}");

                for (int col = 0; col < r_Game.Board.BoardWidth; col++)
                {
                    char squareLetter = r_Game.Board.GetSquareLetter(row, col);
                    string squareDisplay = r_Game.Board.IsSquareRevealed(row, col) ? $"| {squareLetter} " : "|   ";
                    sb.Append(squareDisplay);
                }

                sb.AppendLine("|");
            }

            sb.AppendLine(repeatedEquals);
            Console.WriteLine(sb.ToString());
        }
    }
}

