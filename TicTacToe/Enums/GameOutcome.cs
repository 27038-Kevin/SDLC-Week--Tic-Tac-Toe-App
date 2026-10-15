namespace TicTacToe.Enums
{
    /// <summary>
    /// The outcome of a tic-tac-toe game, indicating which player won, or if the game ended in a tie; or if the game was quit.
    /// </summary>
    internal enum GameOutcome
    {
        /// <summary>
        /// The game outcome is not yet decided because the game hasn't finished yet.
        /// </summary>
        Undecided,

        /// <summary>
        /// The player playing X won the game.
        /// </summary>
        XWon,

        /// <summary>
        /// The player playing O won the game.
        /// </summary>
        OWon,

        /// <summary>
        /// Neither player won the game; the game is a tie.
        /// </summary>
        Tie,

        /// <summary>
        /// Either player quit the game, resulting in an incomplete game state.
        /// </summary>
        Quit,
    }
}
