using System.ComponentModel;
using TicTacToe.Enums;
using TicTacToe.Models.Data;
using TicTacToe.View.ListSelector;

namespace TicTacToe.View
{
    internal class GameHistory
    {
        private const int PageSize = 10;
        private readonly Paginator<GameSummary> _games;
        private readonly int _pageCount;

        public GameHistory(IEnumerable<GameSummary> games)
        {
            this._games = new Paginator<GameSummary>(games, PageSize);
            this._pageCount = (games.Count() + PageSize - 1) / PageSize;
        }

        public void Render()
        {
            string[] summaryList = this._games.CurrentPage
                                        .Select(this.SerializeAsRow)
                                        .ToArray();

            using var selector = new Selector(
                    summaryList,
                    this.RenderSummaryList,
                    ConsoleUtils.ClearScreen,
                    isCancellable: true,
                    this.KeyPressListener);

            try
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
                _ => throw new InvalidEnumArgumentException(nameof(gameSummary.GameOutcome));
            };

            return $" {gameSummary.GameNo,-6}    {gameDescription,-50}    {outcome}";
        }
    }
}
