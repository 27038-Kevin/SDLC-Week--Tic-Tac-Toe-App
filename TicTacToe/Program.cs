using TicTacToe.Models.StateMachines;

namespace TicTacToe
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Player xPlayer = new MediumBotPlayer('X');
            Player oPlayer = new HumanPlayer('O', "Kevin Premkumar", GetInput);

            Game game = new Game(0, xPlayer, oPlayer, DrawBoard);
            game.Start();
            var summary = game.ExportSummary();

            Console.ReadKey();
        }

        private static void DrawBoard(int xState, int oState)
        {
            Console.Clear();
            char[] board = new char[9];
            for (int i = 0; i < 9; ++i)
            {
                if ((xState & (1 << i)) != 0)
                {
                    board[i] = 'X';
                }
                else if ((oState & (1 << i)) != 0)
                {
                    board[i] = 'O';
                }
                else
                {
                    board[i] = ' ';
                }
            }

            Console.WriteLine(StringifyBoard(board));
        }

        private static int GetInput()
        {
            Console.WriteLine("Enter input");
            string input = Console.ReadLine()?.Trim() ?? string.Empty;
            return int.Parse(input);
        }

        private static string StringifyBoard(char[] board)
        {
            return $@"
   |   |
 {board[0]} | {board[1]} | {board[2]}
   |   |
---+---+---
   |   |
 {board[3]} | {board[4]} | {board[5]}
   |   |
---+---+---
   |   |
 {board[6]} | {board[7]} | {board[8]}
   |   |";
        }
    }
}