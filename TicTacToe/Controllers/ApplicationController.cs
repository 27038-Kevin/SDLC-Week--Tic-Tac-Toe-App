using TicTacToe.Enums;
using TicTacToe.View;

namespace TicTacToe.Controllers
{
    internal class ApplicationController
    {
        private readonly GameController _gameController;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationController"/> class.
        /// </summary>
        /// <param name="gameController">Contains game and game history-related controls.</param>
        public ApplicationController(GameController gameController)
        {
            this._gameController = gameController;
        }

        public void Run()
        {
            this.MainMenu();
        }

        private void MainMenu()
        {
            var mainMenu = new Menu<MainMenuOption>();

            // TODO: Change hardcoded name to user's name.
            string header = $"{Constants.Greeting}, {Constants.TempName}!{Environment.NewLine}{Environment.NewLine}MAIN MENU";
            string footer = Menu.GetBasicMenuFooter();

            foreach (MainMenuOption selection in mainMenu.RunPersistently(header, footer, false))
            {
                switch (selection)
                {
                    case MainMenuOption.QuickMatch:
                        this._gameController.QuickMatch();
                        break;

                    case MainMenuOption.ViewGameHistory:
                        this._gameController.ViewGameHistory();
                        break;

                    case MainMenuOption.UserOptions:
                        throw new NotImplementedException();

                    case MainMenuOption.Logout:
                        return;
                }
            }
        }
    }
}
