using TicTacToe.Enums;
using TicTacToe.Models.Data;
using TicTacToe.Persistence.Repositories;
using TicTacToe.Persistence.Serializers;
using TicTacToe.Services;
using TicTacToe.View;
using TicTacToe.View.GameRenderers;

namespace TicTacToe.Controllers
{
    /// <summary>
    /// Contains controls and high-level state flow for the application.
    /// </summary>
    internal class ApplicationController
    {
        private readonly string _dataDirectory;
        private readonly string _gamesFileName;

        private readonly UserController _userController;
        private GameController? _gameController = default;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationController"/> class.
        /// </summary>
        /// <param name="dataDirectory">The directory in which all app data persists.</param>
        /// <param name="gamesFile">The file name used to generate the games file for each user.</param>
        /// <param name="userController">Contains user account-related and account management controls.</param>
        public ApplicationController(string dataDirectory, string gamesFile, UserController userController)
        {
            this._dataDirectory = dataDirectory;
            this._gamesFileName = gamesFile;
            this._userController = userController;
        }

        /// <summary>
        /// Runs the application.
        /// </summary>
        public void Run()
        {
            this.HomeMenu();
        }

        /// <summary>
        /// Loads the entire user's data into the application, initializing the game repository and firing up their session.
        /// Acts as a composition root for a user session.
        /// </summary>
        /// <param name="userId">The user ID, needed to load the content of the user's data files.</param>
        private void LoadUserApplication(Guid userId)
        {
            string userFilesDirectory = Path.Combine(this._dataDirectory, userId.ToString());

            var gameSerializer = new CsvSerializer<GameSummary>();
            var gameRepository = new GameRepository(userFilesDirectory, this._gamesFileName, gameSerializer);
            var gameDisplay = new GameDisplay();
            var gameService = new GameService(gameRepository, gameDisplay);

            this._gameController = new GameController(
                    gameService,
                    gameDisplay,
                    this._userController.CurrentUser?.Name ?? Constants.DefaultName);

            this._userController.OnUserNameChange += this._gameController.SetPlayerName;
            this._userController.RefreshUserSession += this.RefreshSession;
        }

        /// <summary>
        /// Displays the home menu at the start of the application, prompting the user to either login or sign up.
        /// </summary>
        /// <exception cref="ApplicationException">Thrown if the user's data hasn't been loaded into memory despite session initiation.</exception>
        private void HomeMenu()
        {
            var homeMenu = new Menu<HomeMenuOption>();

            string header = $"Welcome, {Constants.DefaultName}!";
            string footer = @"
New around here? Try signing up!
" + Menu.GetBasicMenuFooter(isMenuCancellable: true);

            foreach (HomeMenuOption selection in homeMenu.RunPersistently(
                                                    () => header,
                                                    () => footer,
                                                    isMenuCancellable: true))
            {
                switch (selection)
                {
                    case HomeMenuOption.Login:
                        this._userController.LoginUser();
                        break;

                    case HomeMenuOption.SignUp:
                        this._userController.RegisterUser();
                        break;
                }

                if (this._userController.IsSessionActive)
                {
                    this.LoadUserApplication(this._userController.CurrentUser?.Id ?? throw new ApplicationException("Cannot access user instance"));
                    this.MainMenu();

                    // Unload user application when user's session ends
                    this.UnloadUserApplication();
                }
            }
        }

        /// <summary>
        /// Renders the main menu and controls the flow of the application from the point of the user's selection.
        /// </summary>
        private void MainMenu()
        {
            var mainMenu = new Menu<MainMenuOption>();

            var getHeader = () => $"{Constants.Greeting}, {this._userController.CurrentUser?.Name ?? Constants.DefaultName}!{Environment.NewLine}{Environment.NewLine}MAIN MENU";
            string footer = Menu.GetBasicMenuFooter();

            foreach (MainMenuOption selection in mainMenu.RunPersistently(getHeader, () => footer, false))
            {
                switch (selection)
                {
                    case MainMenuOption.QuickMatch:
                        this._gameController?.QuickMatch();
                        break;

                    case MainMenuOption.ViewGameHistory:
                        this._gameController?.ViewGameHistory();
                        break;

                    case MainMenuOption.UserOptions:
                        this._userController.GetUserOptions();
                        break;

                    case MainMenuOption.Logout:
                        this._userController.CloseUserSession();
                        break;
                }

                if (!this._userController.IsSessionActive)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// An event handler triggered whenever a session refresh is required to reflect a state change.
        /// </summary>
        /// <param name="sender">The sender of the session refresh request.</param>
        /// <param name="args">Event arguments passed to the handler.</param>
        private void RefreshSession(object? sender, EventArgs args)
        {
            if (!this._userController.IsSessionActive)
            {
                return;
            }

            var currentUserId = this._userController.CurrentUser!.Id;
            this.UnloadUserApplication();
            this.LoadUserApplication(currentUserId);
        }

        /// <summary>
        /// Unloads the user application after the user's session ends.
        /// </summary>
        private void UnloadUserApplication()
        {
            if (this._gameController is null)
            {
                return;
            }

            this._userController.OnUserNameChange -= this._gameController.SetPlayerName;
            this._gameController = default;
        }
    }
}
