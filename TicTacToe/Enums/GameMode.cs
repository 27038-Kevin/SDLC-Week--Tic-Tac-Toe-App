namespace TicTacToe.Enums
{
    /// <summary>
    /// The tic-tac-toe game mode/difficulty level.
    /// </summary>
    internal enum GameMode
    {
        /// <summary>
        /// A game mode where the AI allows the player to win easily, providing a more relaxed and enjoyable experience.
        /// </summary>
        Easy,

        /// <summary>
        /// A game mode where the AI attempts to fight, and might or might not let the player win at the end.
        /// </summary>
        Medium,

        /// <summary>
        /// A game mode where the AI plays optimally, making it impossible for the player to win.
        /// </summary>
        Impossible,

        /// <summary>
        /// A game mode where the player can manually control both sides of the game, allowing two players to play on the same device/session.
        /// </summary>
        Manual,
    }
}
