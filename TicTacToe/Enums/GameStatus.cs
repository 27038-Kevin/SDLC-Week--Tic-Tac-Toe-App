namespace TicTacToe.Enums
{
    /// <summary>
    /// Represents the stage the game is in.
    /// </summary>
    internal enum GameStatus
    {
        /// <summary>
        /// Ready stage: The game is ready to be started.
        /// </summary>
        Ready,

        /// <summary>
        /// Ongoing stage: The game is currently being played.
        /// </summary>
        Ongoing,

        /// <summary>
        /// Completed stage: The game has been finished.
        /// </summary>
        Completed,

        /// <summary>
        /// Unfinished stage: The game has been terminated; a player quit.
        /// </summary>
        Unfinished,
    }
}
