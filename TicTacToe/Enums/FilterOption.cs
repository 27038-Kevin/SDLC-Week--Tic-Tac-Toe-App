namespace TicTacToe.Enums
{
    /// <summary>
    /// Contains options to filter game summaries by.
    /// </summary>
    internal enum FilterOption
    {
        /// <summary>
        /// Filter games based on a range of dates between which games were played.
        /// </summary>
        DateRange,

        /// <summary>
        /// Filter games based on the game mode.
        /// </summary>
        GameMode,

        /// <summary>
        /// Filter games based on the game's outcome.
        /// </summary>
        GameOutcome,

        /// <summary>
        /// Filter games based on the letter played by the user (X/O).
        /// </summary>
        LetterPlayed,
    }
}
