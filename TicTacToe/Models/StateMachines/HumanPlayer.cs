using TicTacToe.Enums;

namespace TicTacToe.Models.StateMachines
{
    /// <summary>
    /// A human that plays tic-tac-toe.
    /// </summary>
    internal sealed class HumanPlayer : Player
    {
        /// <summary>
        /// Invokes a delegate used to get input from the user for the game.
        /// Returns the user's choice of move as an integer.
        /// </summary>
        /// <exception cref="Exceptions.QuitGameException">Thrown when the user attempts to quit the game.</exception>
        private readonly Func<int> _getGameInput;

        /// <summary>
        /// Initializes a new instance of the <see cref="HumanPlayer"/> class.
        /// </summary>
        /// <param name="letter">The letter assigned to the player (X/O).</param>
        /// <param name="name">The name of the human player.</param>
        /// <param name="getGameInput">The method invoked to get input from the player.</param>
        public HumanPlayer(
                Letter letter,
                string name,
                Func<int> getGameInput)
            : base(letter, name)
        {
            this._getGameInput = getGameInput;
        }

        /// <summary>
        /// Gets the next move from the human player.
        /// </summary>
        /// <param name="opponent">The opponent (either an AI or another human opponent).</param>
        /// <returns>The square marked by the player.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown if opponent's state overlaps with the player's state,
        /// or if opponent's letter is the same as the player's.
        /// </exception>
        /// <exception cref="InvalidOperationException">Thrown if the method is invoked after the game has ended.</exception>
        /// <exception cref="Exceptions.QuitGameException">
        /// Thrown if the input delegate throws the QuitGameException,
        /// given the player attempts to quit the game.
        /// </exception>
        public override int GetMove(Player opponent)
        {
            if (opponent.Letter == this.Letter)
            {
                throw new ArgumentException("Both players cannot play the same letter", nameof(opponent));
            }

            if ((this.State & opponent.State) != 0)
            {
                throw new ArgumentException("Opponent's state is inconsistent with current player's state", nameof(opponent));
            }

            if (this.HasWon || opponent.HasWon || (this.State | opponent.State) == FullBoardMask)
            {
                throw new InvalidOperationException("Cannot get a move after the game has ended");
            }

            int xState = this.Letter == Letter.X ? this.State : opponent.State;
            int oState = this.Letter == Letter.O ? this.State : opponent.State;

            while (true)
            {
                int moveChoice = this._getGameInput();  // Throws QuitGameException
                if (!Squares.Contains(moveChoice) || this.IsMarked(moveChoice, opponent.State))
                {
                    continue;   // Continue silently until user chooses a valid input
                }

                this.MarkSpot(moveChoice);
                return moveChoice;
            }
        }
    }
}
