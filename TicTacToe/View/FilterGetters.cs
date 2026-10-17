using System.Globalization;
using TicTacToe.Enums;
using TicTacToe.Helpers;
using TicTacToe.Models.Utils;

namespace TicTacToe.View
{
    internal class FilterGetters
    {
        private const string DateFormat = Constants.DateFormat;

        /// <summary>
        /// Gets a date range filter from the user.
        /// </summary>
        /// <param name="dateTimeRange">
        /// The currently applied <see cref="DateTimeRange"/> filter.
        /// Can be changed, deselected, or remain unmodified.
        /// </param>
        internal void GetDateRangeFilter(ref DateTimeRange? dateTimeRange)
        {
            this.GetFilter(
                ref dateTimeRange,
                this.DisplayDateRangeFilter,
                this.GetDateRange);
        }

        /// <summary>
        /// Gets a game mode filter from the user.
        /// </summary>
        /// <param name="gameMode">
        /// The currently applied <see cref="GameMode"/> filter.
        /// Can be changed, deselected, or remain unmodified.
        /// </param>
        internal void GetGameModeFilter(ref GameMode? gameMode)
        {
            this.GetFilter(
                ref gameMode,
                this.DisplayGameModeFilter,
                this.GetGameMode);
        }

        /// <summary>
        /// Gets a letter filter from the user.
        /// </summary>
        /// <param name="letter">
        /// The currently applied <see cref="Letter"/> filter.
        /// Can be changed, deselected, or remain unmodified.</param>
        internal void GetLetterFilter(ref Letter? letter)
        {
            this.GetFilter(
                ref letter,
                this.DisplayLetter,
                this.GetLetter);
        }

        /// <summary>
        /// Gets a game outcome filter from the user.
        /// </summary>
        /// <param name="gameOutcome">
        /// The currently applied <see cref="PlayerOrientedGameOutcome"/> filter.
        /// Can be changed, deselected, or remain unmodified.
        /// </param>
        internal void GetPlayerOrientedGameOutcomeFilter(ref PlayerOrientedGameOutcome? gameOutcome)
        {
            this.GetFilter(
                ref gameOutcome,
                this.DisplayPlayerOrientedGameOutcomeFilter,
                this.GetPlayerOrientedGameOutcome);
        }

        /// <summary>
        /// Generic method to get a new filter.
        /// </summary>
        /// <typeparam name="TFilter">The type of filter to get.</typeparam>
        /// <param name="currentFilter">The currently selected filter.</param>
        /// <param name="showFilter">Displays the currently selected filter on the screen.</param>
        /// <param name="getNewFilter">Gets a new filter.</param>
        private void GetFilter<TFilter>(
                    ref TFilter? currentFilter,
                    Action<TFilter> showFilter,
                    Func<TFilter> getNewFilter)
            where TFilter : struct
        {
            while (true)
            {
                ConsoleUtils.ClearScreen();

                if (currentFilter is not null)
                {
                    Console.WriteLine("Filter is currently applied!");
                    showFilter(currentFilter.Value);
                    Console.WriteLine();
                    Console.WriteLine("Press R to remove the filter, press C to change it,");
                    Console.WriteLine("or press any other key to return.");

                    ConsoleKeyInfo keyPress = ConsoleUtils.GetKeyPress();
                    if (keyPress.Key == ConsoleKey.R)
                    {
                        currentFilter = default;
                    }
                    else if (keyPress.Key == ConsoleKey.C)
                    {
                        currentFilter = getNewFilter();
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    Console.WriteLine("Filter currently unselected.");
                    Console.WriteLine("Press A to apply a new filter,");
                    Console.WriteLine("or press any other key to return.");

                    ConsoleKeyInfo keyPress = ConsoleUtils.GetKeyPress();
                    if (keyPress.Key == ConsoleKey.A)
                    {
                        currentFilter = getNewFilter();
                    }
                    else
                    {
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Gets date from the user as input.
        /// </summary>
        /// <param name="dateContext">The context from which the date is being taken.</param>
        /// <returns>Date on which the amount was incurred as expense/received as income.</returns>
        private DateTime GetDate(string? dateContext)
        {
            char space = ' ';
            string dateContextFormat = (dateContext is not null) ? string.Concat(space, dateContext) : string.Empty;
            string message = $"Enter the{dateContextFormat} date ({Constants.DateFormat}):";

            Console.WriteLine(message);
            DateTime recordDate;

            do
            {
                var input = Console.ReadLine()?.Trim() ?? string.Empty;

                if (!DateTime.TryParseExact(
                        input,
                        Constants.DateFormat,
                        Constants.ProjectCulture,
                        DateTimeStyles.None,
                        out recordDate))
                {
                    Console.WriteLine("Please enter the date in the provided format.");
                    continue;
                }

                break;
            }
            while (true);

            return recordDate;
        }

        /// <summary>
        /// Gets a new date range as input.
        /// </summary>
        /// <returns>The date range.</returns>
        private DateTimeRange GetDateRange()
        {
            DateTimeRange dateRange = default;
            do
            {
                DateTime startDate = this.GetDate("start");
                DateTime endDate = this.GetDate("end");

                if (startDate <= endDate)
                {
                    dateRange = new DateTimeRange(startDate, endDate);
                    break;
                }
                else
                {
                    Console.WriteLine("Starting date cannot come after ending date!");
                }
            }
            while (true);

            return dateRange;
        }

        /// <summary>
        /// Gets a new game mode from the user as input.
        /// </summary>
        /// <returns>The user-selected game mode.</returns>
        private GameMode GetGameMode()
        {
            var gameModeMenu = new Menu<GameMode>();

            const string menuHeader = "Select the game mode:";
            string menuFooter = Menu.GetBasicMenuFooter();

            // Will not throw because menu is not cancellable
            return gameModeMenu.Run(menuHeader, menuFooter, false);
        }

        /// <summary>
        /// Gets a new tic-tac-toe letter from the user as input.
        /// </summary>
        /// <returns>The user-selected tic-tac-toe letter.</returns>
        private Letter GetLetter()
        {
            var letterMenu = new Menu<Letter>();

            const string menuHeader = "Select the letter you play:";
            string menuFooter = Menu.GetBasicMenuFooter();

            // Will not throw because menu is not cancellable
            return letterMenu.Run(menuHeader, menuFooter, false);
        }

        /// <summary>
        /// Gets a new game outcome from the user as input.
        /// </summary>
        /// <returns>The user-selected game outcome.</returns>
        private PlayerOrientedGameOutcome GetPlayerOrientedGameOutcome()
        {
            var gameOutcomeMenu = new Menu<PlayerOrientedGameOutcome>();

            const string menuHeader = "Select the game outcome:";
            string menuFooter = Menu.GetBasicMenuFooter();

            // Will not throw because menu is not cancellable
            return gameOutcomeMenu.Run(menuHeader, menuFooter, false);
        }

        /// <summary>
        /// Displays the date time range filter.
        /// </summary>
        /// <param name="dateTimeRange">The date time range filter to display.</param>
        private void DisplayDateRangeFilter(DateTimeRange dateTimeRange)
        {
            Console.WriteLine($"Start Date: {dateTimeRange.Start.ToString(DateFormat)}");
            Console.WriteLine($"End Date: {dateTimeRange.End.ToString(DateFormat)}");
        }

        /// <summary>
        /// Displays the currently selected game mode filter.
        /// </summary>
        /// <param name="gameMode">The game mode filter to display.</param>
        private void DisplayGameModeFilter(GameMode gameMode)
        {
            Console.WriteLine($"Game Mode: {CaseConverter.PascalToTitle(gameMode.ToString())}");
        }

        /// <summary>
        /// Displays the currently selected letter filter.
        /// </summary>
        /// <param name="letter">The letter filter to display.</param>
        private void DisplayLetter(Letter letter)
        {
            Console.WriteLine($"You play: {letter}");
        }

        /// <summary>
        /// Displays the currently selected game outcome filter.
        /// </summary>
        /// <param name="gameOutcome">The game outcome filter to display.</param>
        private void DisplayPlayerOrientedGameOutcomeFilter(PlayerOrientedGameOutcome gameOutcome)
        {
            Console.WriteLine($"Game Outcome: {CaseConverter.PascalToTitle(gameOutcome.ToString())}");
        }
    }
}
