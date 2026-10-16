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
    }
}
