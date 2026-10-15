using TicTacToe.Enums;

namespace TicTacToe.Models.Utils
{
    /// <summary>
    /// Struct containing various filters to be applied on a game summary query.
    /// </summary>
    internal readonly struct GameSummaryFilter
    {
        /// <summary>
        /// Represents an empty game summary filter.
        /// </summary>
        public static readonly GameSummaryFilter Empty = new () { };

        /// <summary>
        /// Initializes a new instance of the <see cref="GameSummaryFilter"/> struct.
        /// </summary>
        /// <param name="range">The <see cref="DateTimeRange"/> to filter by; null if not included.</param>
        /// <param name="mode">The <see cref="GameMode"/> to filter by; null if not included.</param>
        /// <param name="outcome">The <see cref="PlayerOrientedGameOutcome"/> to filter by; null if not included.</param>
        /// <param name="letterPlayed">The letter played by the user in each game; null if not included.</param>
        public GameSummaryFilter(
                DateTimeRange? range = default,
                GameMode? mode = default,
                PlayerOrientedGameOutcome? outcome = default,
                char? letterPlayed = default)
        {
            if (letterPlayed is not null
                && letterPlayed != 'X'
                && letterPlayed != 'O')
            {
                throw new ArgumentException(
                    "The letter played should either be 'X' or 'O'",
                    nameof(letterPlayed));
            }

            this.DateRange = range;
            this.Mode = mode;
            this.Outcome = outcome;
            this.LetterPlayed = letterPlayed;
        }

        /// <summary>
        /// Gets the date range to filter by.
        /// </summary>
        /// <value>The date range struct to filter by; null if not included.</value>
        public readonly DateTimeRange? DateRange { get; init; }

        /// <summary>
        /// Gets the game mode to filter by.
        /// </summary>
        /// <value>The game mode to filter by; null if not included.</value>
        public readonly GameMode? Mode { get; init; }

        /// <summary>
        /// Gets the game outcome (from the player's perspective) to filter by.
        /// </summary>
        /// <value>The game outcome to filter by; null if not included.</value>
        public readonly PlayerOrientedGameOutcome? Outcome { get; init; }

        /// <summary>
        /// Gets the letter played by the user to filter by.
        /// </summary>
        /// <value>The letter played to filter by; null if not included.</value>
        public readonly char? LetterPlayed { get; init; }
    }
}
