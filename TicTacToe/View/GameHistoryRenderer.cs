using System.ComponentModel;
using TicTacToe.Enums;
using TicTacToe.Helpers;
using TicTacToe.Models.Data;
using TicTacToe.View.ListSelector;

namespace TicTacToe.View
{
    /// <summary>
    /// Renders the game history dynamically and contains follow-up renders.
    /// </summary>
    internal class GameHistoryRenderer
    {
        private const int PageSize = 10;
        private readonly Action<GameSummary> _replayGame;
        private readonly Paginator<GameSummary> _games;
        private readonly int _pageCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameHistoryRenderer"/> class.
        /// </summary>
        /// <param name="games">The list of games to render.</param>
        /// <param name="replayGame">The renderer used to replay an old game.</param>
        public GameHistoryRenderer(IEnumerable<GameSummary> games, Action<GameSummary> replayGame)
        {
            this._games = new Paginator<GameSummary>(games, PageSize);
            this._pageCount = (games.Count() + PageSize - 1) / PageSize;
            this._replayGame = replayGame;
        }

        /// <summary>
        /// Renders the game history on the console.
        /// </summary>
        /// <param name="externalKeyboardListeners">
        /// External listeners for specific actions (e.g.: creating a filter, etc).
        /// </param>
        public void Render(params EventHandler<ConsoleKeyInfo?>[] externalKeyboardListeners)
        {
            string[] summaryList = this._games.CurrentPage
                                        .Select(this.SerializeAsRow)
                                        .ToArray();

            var keyPressHandlerParams = new List<EventHandler<ConsoleKeyInfo?>> { this.KeyPressListener }
                                            .Concat(externalKeyboardListeners)
                                            .ToArray();

            using var selector = new Selector(
                summaryList,
                this.RenderSummaryList,
                ConsoleUtils.ClearScreen,
                isCancellable: true,
                keyPressHandlerParams);

            while (true)
            {
                while (selector.ViewState == SelectorViewState.Active)
                {
                    var keyPress = ConsoleUtils.GetKeyPress();
                    selector.Update(summaryList, keyPress);

                    summaryList = this._games.CurrentPage
                                        .Select(this.SerializeAsRow)
                                        .ToArray();

                    selector.Update(summaryList, null); // Dry refresh to correctly render state, if event handlers were invoked
                }

                switch (selector.ViewState)
                {
                    case SelectorViewState.Selected:
                        GameSummary selectedGame = this._games.CurrentPage
                                        .ElementAt(selector.Selection);
                        this.RenderGameSummary(selectedGame);
                        break;

                    case SelectorViewState.Cancelled:
                        return;
                }

                selector.Reset();
            }
        }

        /// <summary>
        /// An event handler for a keypress from the console; adjusts pages based on the event.
        /// </summary>
        /// <param name="sender">The sender of the event. Required parameter.</param>
        /// <param name="keyPress">The key pressed by the user.</param>
        private void KeyPressListener(object? sender, ConsoleKeyInfo? keyPress)
        {
            if (keyPress is null)
            {
                return;
            }

            try
            {
                if (keyPress.Value.Key == ConsoleKey.RightArrow)
                {
                    this._games.NextPage();
                }
                else if (keyPress.Value.Key == ConsoleKey.LeftArrow)
                {
                    this._games.PreviousPage();
                }
            }
            catch (InvalidOperationException)
            {
                // Silently ignore the exception as it does not affect user
            }
        }

        /// <summary>
        /// Renders the game summary on the screen.
        /// </summary>
        /// <param name="gameSummary">The game summary to render.</param>
        private void RenderGameSummary(GameSummary gameSummary)
        {
            while (true)
            {
                ConsoleUtils.ClearScreen();
                Console.WriteLine($"Game #{gameSummary.GameNo}");
                Console.WriteLine(gameSummary.DateAndTime.ToString());
                Console.WriteLine();
                Console.WriteLine($"X Player: {gameSummary.XPlayer}");
                Console.WriteLine($"O Player: {gameSummary.OPlayer}");
                Console.WriteLine($"Game Outcome: {CaseConverter.PascalToTitle(gameSummary.GameOutcome.ToString())}");
                Console.WriteLine();
                Console.WriteLine("Press R to watch a live replay of the game,");
                Console.WriteLine("or press any other key to return.");

                var keyPress = ConsoleUtils.GetKeyPress();
                if (keyPress.Key != ConsoleKey.R)
                {
                    break;
                }

                this._replayGame(gameSummary);
            }
        }

        /// <summary>
        /// Renders the list of game summaries on the screen.
        /// </summary>
        /// <param name="summaryList">The list of game summaries to render.</param>
        /// <param name="selectedRow">The currently selected row in the list.</param>
        private void RenderSummaryList(IList<string> summaryList, int selectedRow)
        {
            const string label = "Game No.                        Players";
            var outcomeThumbnailColorCode = new Dictionary<char, ConsoleColor>()
            {
                ['+'] = ConsoleColor.Green,
                ['-'] = ConsoleColor.Red,
                ['='] = ConsoleColor.Yellow,
                ['~'] = ConsoleColor.Gray,
            };

            Console.WriteLine("GAME HISTORY");
            Console.WriteLine();

            if (this._pageCount > 1)
            {
                Console.WriteLine($"Page: {this._games.CurrentPageNumber} / {this._pageCount}");
            }

            Console.WriteLine(label);
            Console.WriteLine();

            for (int i = 0; i < summaryList.Count; ++i)
            {
                char outcomeSymbol = summaryList[i][summaryList[i].Length - 1];

                if (i == selectedRow)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.BackgroundColor = outcomeThumbnailColorCode[outcomeSymbol];
                }
                else
                {
                    Console.ForegroundColor = outcomeThumbnailColorCode[outcomeSymbol];
                }

                Console.WriteLine(summaryList[i]);
                Console.ResetColor();
            }

            Console.WriteLine();
            Console.WriteLine(Menu.GetBasicMenuFooter(true));
            if (this._pageCount > 1)
            {
                Console.WriteLine("Use left and right arrow keys to navigate between different pages.");
            }
        }

        /// <summary>
        /// Serializes a game summary as a displayable row for the summary list render.
        /// </summary>
        /// <param name="gameSummary">The game summary to serialize.</param>
        /// <returns>A serialized line describing the game details briefly.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown if the game outcome is not known.</exception>
        private string SerializeAsRow(GameSummary gameSummary)
        {
            string gameDescription = $"{gameSummary.XPlayer} vs. {gameSummary.OPlayer}";
            if (gameDescription.Length > 50)
            {
                gameDescription = gameDescription.Substring(0, 47) + "...";
            }

            char outcome = gameSummary.GameOutcome switch
            {
                GameOutcome.XWon => gameSummary.IsXHuman ? '+' : '-',
                GameOutcome.OWon => gameSummary.IsOHuman ? '+' : '-',
                GameOutcome.Tie => '=',
                GameOutcome.Quit => '~',
                _ => throw new InvalidEnumArgumentException(nameof(gameSummary.GameOutcome)),
            };

            return $" {gameSummary.GameNo,-6}    {gameDescription,-50}    {outcome}";
        }
    }
}
