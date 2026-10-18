using TicTacToe.Enums;
using TicTacToe.Models.Data;
using TicTacToe.Models.Utils;
using TicTacToe.Services;
using TicTacToe.View;
using TicTacToe.View.GameRenderers;

namespace TicTacToe.Controllers
{
    internal class GameController
    {
        private readonly GameService _service;
        private readonly IGameDisplay _gameRenderer;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameController"/> class.
        /// </summary>
        /// <param name="service">The service that runs games and manages game history.</param>
        /// <param name="gameDisplay">The element that renders the game components on the screen.</param>
        public GameController(GameService service, IGameDisplay gameDisplay)
        {
            this._service = service;
            this._gameRenderer = gameDisplay;
        }

        /// <summary>
        /// Event triggered whenever there is a filter change.
        /// </summary>
        private event EventHandler? OnFilterChange;

        /// <summary>
        /// Gets or sets the summary filter to be used.
        /// </summary>
        /// <value>The filter applied to all the game summaries present in the repository.</value>
        private GameSummaryFilter Filter { get; set; } = GameSummaryFilter.Empty;

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
                gameModeMenu.RunWithStatelessCancellation(gameModeMenuHeader, menuFooter, out GameMode? gameModeSelection);
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
                        letterMenu.RunWithStatelessCancellation(letterMenuHeader, menuFooter, out Letter? letterSelection);
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

        /// <summary>
        /// Allows the user
        /// - to view game history
        /// - apply filters to the game history
        /// - see user's stats for each gamemode.
        /// - rewatch old games
        /// etc.
        /// </summary>
        internal void ViewGameHistory()
        {
            var getSummaries = () => this._service.GetGames(this.Filter);
            var getStats = () => Enum.GetValues<GameMode>()
                                    .Where(gameMode => gameMode != GameMode.Manual)
                                    .Select(this._service.GetUserStats)
                                    .ToArray();

            var gameHistory = new GameHistoryRenderer(getSummaries, getStats, this._gameRenderer.ReplayGame);

            this.OnFilterChange += gameHistory.ReloadGameSummaries;
            gameHistory.Render(this.SetFilter);

            // Reset filter after render
            this.Filter = GameSummaryFilter.Empty;
            this.OnFilterChange -= default;
        }

        /// <summary>
        /// Sets the filter to use on the game summaries.
        /// Activates when the user presses F on the <see cref="View.ListSelector.Selector"/> this handler is subscribed to.
        /// </summary>
        /// <param name="sender">The sender of the event. Required parameter.</param>
        /// <param name="keyPress">The user's keypress.</param>
        private void SetFilter(object? sender, ConsoleKeyInfo? keyPress)
        {
            if (keyPress is null || keyPress.Value.Key != ConsoleKey.F)
            {
                return;
            }

            var filterMenu = new Menu<FilterOption>();
            var getFilter = new FilterGetters();

            const string filterMenuHeader = "Filter Options";
            string filterMenuFooter = @"
Select a filter to either set or reset it, if it is already set.
" + Menu.GetBasicMenuFooter(isMenuCancellable: true);

            DateTimeRange? dateRangeFilter = this.Filter.DateRange;
            GameMode? gameModeFilter = this.Filter.Mode;
            PlayerOrientedGameOutcome? gameOutcomeFilter = this.Filter.Outcome;
            Letter? letterFilter = this.Filter.LetterPlayed;

            foreach (FilterOption selection in filterMenu.RunPersistently(
                                                filterMenuHeader,
                                                filterMenuFooter,
                                                isMenuCancellable: true))
            {
                switch (selection)
                {
                    case FilterOption.DateRange:
                        getFilter.GetDateRangeFilter(ref dateRangeFilter);
                        break;

                    case FilterOption.GameMode:
                        getFilter.GetGameModeFilter(ref gameModeFilter);
                        break;

                    case FilterOption.GameOutcome:
                        getFilter.GetPlayerOrientedGameOutcomeFilter(ref gameOutcomeFilter);
                        break;

                    case FilterOption.LetterPlayed:
                        getFilter.GetLetterFilter(ref letterFilter);
                        break;
                }
            }

            this.Filter = new GameSummaryFilter(
                                dateRangeFilter,
                                gameModeFilter,
                                gameOutcomeFilter,
                                letterFilter);

            this.OnFilterChange?.Invoke(this, EventArgs.Empty);
        }
    }
}
