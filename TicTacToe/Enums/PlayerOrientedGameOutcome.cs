namespace TicTacToe.Enums
{
    /// <summary>
    /// Represents the game outcome from the perspective of an individual player.
    /// </summary>
    internal enum PlayerOrientedGameOutcome
    {
        /// <summary>
        /// The player wins the game.
        /// </summary>
        Win,

        /// <summary>
        /// The player loses the game.
        /// </summary>
        Loss,

        /// <summary>
        /// The player ties the game.
        /// </summary>
        Tie,

        /// <summary>
        /// The player quits the game.
        /// </summary>
        Quit,
    }
}
