using System.Text;
using TicTacToe.Enums;
using TicTacToe.Exceptions;
using TicTacToe.Models.StateMachines;

namespace TicTacToe.View.GameRenderers
{
    /// <summary>
    /// Handles game display and renders.
    /// </summary>
    internal class GameDisplay : IGameDisplay
    {
        private const string EmptyBoard =
@"            |            |            
            |            |            
            |            |            
            |            |            
            |            |            
            |            |            
------------+------------+------------
            |            |            
            |            |            
            |            |            
            |            |            
            |            |            
            |            |            
------------+------------+------------
            |            |            
            |            |            
            |            |            
            |            |            
            |            |            
            |            |             ";

        /// <inheritdoc cref="IGameDisplay.RenderGameBoard(Game)"/>
        public void RenderGameBoard(Game game)
        {
            string board = this.DrawBoard(
                                game.XPlayer.State,
                                game.OPlayer.State,
                                showIndices: game.GameStatus == GameStatus.Ongoing);

            bool isXHuman = Player.IsHuman(game.XPlayer);
            bool isOHuman = Player.IsHuman(game.OPlayer);
            bool currentPlayerIsHuman = (game.IsXTurn && isXHuman) || (game.IsOTurn && isOHuman);

            string xNickname = Nickname(game.XPlayer.Name);
            string oNickname = Nickname(game.OPlayer.Name);

            ConsoleUtils.ClearScreen();
            switch (game.GameOutcome)
            {
                case GameOutcome.Undecided:
                    Console.WriteLine($"{(game.IsXTurn ? xNickname : oNickname)}'s move!");
                    break;

                case GameOutcome.XWon:
                    Console.WriteLine($"{xNickname} wins!");
                    break;

                case GameOutcome.OWon:
                    Console.WriteLine($"{oNickname} wins!");
                    break;

                case GameOutcome.Tie:
                    Console.WriteLine("The game is a tie!");
                    break;

                case GameOutcome.Quit:
                    Console.WriteLine("The game has been quit!");
                    break;
            }

            Console.WriteLine();
            Console.WriteLine(board);
            Console.WriteLine();

            if (game.GameStatus != GameStatus.Ongoing)
            {
                Console.WriteLine("Press any key to continue...");
                ConsoleUtils.GetKeyPress();
            }
            else if (currentPlayerIsHuman)
            {
                Console.WriteLine("Waiting for move...");
            }
            else
            {
                Console.WriteLine("Waiting for computer's move...");
            }

            static string Nickname(string name)
            {
                return name.Split()[0];
            }
        }

        /// <summary>
        /// Gets a move as input from the user.
        /// </summary>
        /// <returns>The inputted move.</returns>
        /// <exception cref="QuitGameException">Thrown if the user confirms that they want to quit the game by pressing Q.</exception>
        public int GetMoveInput()
        {
            var originalPosition = Console.GetCursorPosition();
            Console.WriteLine("Type any shown number to occupy that square, or press Q to quit the game.");

            while (true)
            {
                var keyPress = ConsoleUtils.GetKeyPress();
                switch (keyPress.Key)
                {
                    case ConsoleKey.D0:
                    case ConsoleKey.D1:
                    case ConsoleKey.D2:
                    case ConsoleKey.D3:
                    case ConsoleKey.D4:
                    case ConsoleKey.D5:
                    case ConsoleKey.D6:
                    case ConsoleKey.D7:
                    case ConsoleKey.D8:
                        CleanupConsole();
                        return keyPress.Key - ConsoleKey.D0;

                    case ConsoleKey.Q:
                        if (this.ConfirmQuit())
                        {
                            throw new QuitGameException();
                        }
                        break;

                    default:
                        break;
                }
            }

            void CleanupConsole()
            {
                Console.SetCursorPosition(originalPosition.Left, originalPosition.Top);
                Console.WriteLine();
                Console.SetCursorPosition(originalPosition.Left, originalPosition.Top);
            }
        }

        /// <summary>
        /// Gets confirmation from the user to quit the game.
        /// </summary>
        /// <returns>true if the user confirms that they want to quit; false otherwise.</returns>
        private bool ConfirmQuit()
        {
            var originalPosition = Console.GetCursorPosition();

            Console.WriteLine();
            Console.WriteLine("Are you sure you want to quit the game?");
            Console.WriteLine("Press Y to confirm.");

            var keyPress = ConsoleUtils.GetKeyPress();

            Console.SetCursorPosition(originalPosition.Left, originalPosition.Top);
            for (int i = 0; i < 3; ++i)
            {
                Console.WriteLine(new string(' ', Console.WindowWidth));
            }

            Console.SetCursorPosition(originalPosition.Left, originalPosition.Top);
            return keyPress.Key == ConsoleKey.Y;
        }

        /// <summary>
        /// Pre-renders the board before printing it to the console, based on the current state.
        /// </summary>
        /// <param name="xState">The board state from the X player's perspective.</param>
        /// <param name="oState">The board state from the O player's perspective.</param>
        /// <param name="showIndices">Whether or not to show the indices on the board.</param>
        /// <returns>The drawn board.</returns>
        private string DrawBoard(int xState, int oState, bool showIndices)
        {
            char[][] board = EmptyBoard.Split('\n')
                                .Select(line => line.ToCharArray())
                                .ToArray();

            int length = board[0].Length;
            int height = board.Length;

            int squareLength = (length + 1) / 3;
            int squareHeight = (height + 1) / 3;

            int[][] centers =
            {
                new[] { squareHeight / 2, squareLength / 2 },
                new[] { squareHeight / 2, squareLength + (squareLength / 2) },
                new[] { squareHeight / 2, (squareLength * 2) + (squareLength / 2) },
                new[] { squareHeight + (squareHeight / 2), squareLength / 2 },
                new[] { squareHeight + (squareHeight / 2), squareLength + (squareLength / 2) },
                new[] { squareHeight + (squareHeight / 2), (squareLength * 2) + (squareLength / 2) },
                new[] { (squareHeight * 2) + (squareHeight / 2), squareLength / 2 },
                new[] { (squareHeight * 2) + (squareHeight / 2), squareLength + (squareLength / 2) },
                new[] { (squareHeight * 2) + (squareHeight / 2), (squareLength * 2) + (squareLength / 2) },
            };

            for (int box = 0; box < centers.Length; ++box)
            {
                if ((xState & (1 << box)) != 0)
                {
                    this.DrawX(board, centers[box]);
                }
                else if ((oState & (1 << box)) != 0)
                {
                    this.DrawO(board, centers[box]);
                }
                else if (showIndices)
                {
                    board[centers[box][0]][centers[box][1]] = (char)('0' + box);
                }
            }

            var consolidatedBoard = new StringBuilder();
            for (int i = 0; i < height; ++i)
            {
                for (int j = 0; j < length; ++j)
                {
                    consolidatedBoard.Append(board[i][j]);
                }

                consolidatedBoard.AppendLine();
            }

            return consolidatedBoard.ToString();
        }

        /// <summary>
        /// Draws an X in a square of the board.
        /// </summary>
        /// <param name="board">The board to draw on.</param>
        /// <param name="center">The coordinates to the center of the square.</param>
        private void DrawX(char[][] board, int[] center)
        {
            int y = center[0];
            int x = center[1];

            board[y - 2][x - 2] = '\\';
            board[y - 1][x - 1] = '\\';
            board[y][x] = '\\';
            board[y + 1][x + 1] = '\\';

            board[y - 2][x + 1] = '/';
            board[y - 1][x] = '/';
            board[y][x - 1] = '/';
            board[y + 1][x - 2] = '/';
        }

        /// <summary>
        /// Draws an O in a square of the board.
        /// </summary>
        /// <param name="board">The board to draw on.</param>
        /// <param name="center">The coordinates to the center of the square.</param>
        private void DrawO(char[][] board, int[] center)
        {
            int y = center[0];
            int x = center[1];

            board[y - 3][x - 2] = '_';
            board[y - 3][x - 1] = '_';
            board[y - 3][x] = '_';
            board[y - 3][x + 1] = '_';

            board[y - 2][x - 3] = '|';
            board[y - 2][x + 2] = '|';
            board[y - 1][x - 3] = '|';
            board[y - 1][x + 2] = '|';
            board[y][x - 3] = '|';
            board[y][x + 2] = '|';
            board[y + 1][x - 3] = '|';
            board[y + 1][x + 2] = '|';

            board[y + 1][x - 2] = '_';
            board[y + 1][x - 1] = '_';
            board[y + 1][x] = '_';
            board[y + 1][x + 1] = '_';
        }
    }
}
