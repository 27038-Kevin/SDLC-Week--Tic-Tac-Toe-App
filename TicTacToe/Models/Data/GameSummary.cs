using TicTacToe.Enums;

namespace TicTacToe.Models.Data
{
    /// <summary>
    /// Represents a summary of a game that took place.
    /// </summary>
    internal record GameSummary : IHasId
    {
        /// <summary>
        /// Gets the global unique identifier for the game summary.
        /// </summary>
        /// <value>The GUID used as a unique identifier.</value>
        public Guid Id { get; init; }

        /// <summary>
        /// Gets the unique game number that is used to enumerate the number of games played till date.
        /// </summary>
        /// <value>A unique number assigned to each game.</value>
        public uint GameNo { get; init; }

        /// <summary>
        /// Gets the name of the player who played X during the game.
        /// </summary>
        /// <value>The name of the X player.</value>
        public string XPlayer { get; init; } = default!;

        /// <summary>
        /// Gets the name of the player who played O during the game.
        /// </summary>
        /// <value>The name of the O player.</value>
        public string OPlayer { get; init; } = default!;

        /// <summary>
        /// Gets a value indicating whether the X player was a human or not.
        /// </summary>
        /// <value>true if the X player was human; false if the X player was a bot.</value>
        public bool IsXHuman { get; init; }

        /// <summary>
        /// Gets a value indicating whether the O player was a human or not.
        /// </summary>
        /// <value>true if the O player was human; false if the O player was a bot.</value>
        public bool IsOHuman { get; init; }

        /// <summary>
        /// Gets the date and timestamp the game ended at.
        /// </summary>
        /// <value>The date and time the game was saved.</value>
        public DateTime DateAndTime { get; init; }

        /// <summary>
        /// Gets the list of moves in the game.
        /// </summary>
        /// <value>Integers representing the box indices for each square that was marked during each turn.</value>
        public int[] Moves { get; init; } = default!;

        /// <summary>
        /// Gets the game outcome.
        /// </summary>
        /// <value>The outcome of the game; whether X won, O won, the game was tied, or the game was quit.</value>
        public GameOutcome GameOutcome { get; init; }

        /// <summary>
        /// Gets the game mode.
        /// </summary>
        /// <value>The game mode/difficulty level (easy/medium/impossible/manual).</value>
        public GameMode GameMode { get; init; }
    }
}
