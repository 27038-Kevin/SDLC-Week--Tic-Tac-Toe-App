using TicTacToe.Models.Data;
using TicTacToe.Models.StateMachines;

namespace TicTacToe.View.GameRenderers
{
    /// <summary>
    /// Defines a contract for a view model to render the game in real-time on the console.
    /// </summary>
    internal interface IGameDisplay
    {
        /// <summary>
        /// Renders the board on the console screen.
        /// </summary>
        /// <param name="game">The game to render.</param>
        void RenderGameBoard(Game game);

        /// <summary>
        /// Gets a move from the user as input.
        /// </summary>
        /// <returns>The inputted move.</returns>
        int GetMoveInput();

        /// <summary>
        /// Replays an old game in real time.
        /// </summary>
        /// <param name="gameSummary">The game information used for re-rendering the game.</param>
        void ReplayGame(GameSummary gameSummary);
    }
}
