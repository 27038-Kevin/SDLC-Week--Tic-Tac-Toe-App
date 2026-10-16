using System.ComponentModel;
using TicTacToe.Enums;
using TicTacToe.Exceptions;
using TicTacToe.Services;
using TicTacToe.View;

namespace TicTacToe.Controllers
{
    internal class GameController
    {
        private readonly GameService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameController"/> class.
        /// </summary>
        /// <param name="service">The service that runs games and manages game history.</param>
        public GameController(GameService service)
        {
            this._service = service;
        }

        /// <summary>
        /// Starts a new game of tic-tac-toe.
        /// </summary>
        internal void QuickMatch()
        {
            var gameModeMenu = new Menu<GameMode>();
            var letterMenu = new Menu<Letter>();

            string gameModeMenuHeader = "Select the game mode:";
            string letterMenuHeader = "Which letter? (X goes first)";
            string menuFooter = Menu.GetBasicMenuFooter(isMenuCancellable: true);

            while (true)
            {
                gameModeMenu.RenderWithStatelessCancellation(gameModeMenuHeader, menuFooter, out GameMode? gameModeSelection);
                if (gameModeSelection is null)
                {
                    break;  // Control exits loop and returns to main menu
                }

                string? player1Name = default;
                string? player2Name = default;
                bool playerGoesFirst = true;

                switch (gameModeSelection.Value)
                {
                    case GameMode.Manual:
                        player1Name = ConsoleUtils.GetLine("Enter Player 1's Name:");
                        player2Name = ConsoleUtils.GetLine("Enter Player 2's Name:");
                        break;

                    case GameMode.Easy:
                    case GameMode.Medium:
                    case GameMode.Impossible:
                        player1Name = Constants.TempName;
                        letterMenu.RenderWithStatelessCancellation(letterMenuHeader, menuFooter, out Letter? letterSelection);
                        if (letterSelection is null)
                        {
                            continue;   // Goes back to game mode menu
                        }
                        playerGoesFirst = letterSelection.Value == Letter.X;
                        break;
                }

                this._service.StartNewGame(gameModeSelection.Value, playerGoesFirst, player1Name, player2Name);
                break;  // Break loop after game ends
            }
        }
    }
}
