using TicTacToe.Enums;

namespace TicTacToe.Models.StateMachines
{
    /// <summary>
    /// A tic-tac-toe bot that plays at the impossible level -- it does not lose.
    /// </summary>
    internal sealed class ImpossibleBotPlayer : BotPlayer
    {
        private const string BotName = "Computer (Impossible)";

        /// <summary>
        /// Initializes a new instance of the <see cref="ImpossibleBotPlayer"/> class.
        /// </summary>
        /// <param name="letter">The letter assigned to the player (X/O).</param>
        public ImpossibleBotPlayer(Letter letter)
            : base(letter, BotName)
        {
        }

        /// <summary>
        /// Gets the next move from the impossible-mode AI bot.
        /// </summary>
        /// <param name="opponent">The opponent (a human player).</param>
        /// <returns>The square marked on the computer's turn, after it has made its move.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown if opponent's state overlaps with the player's state,
        /// or if opponent's letter is the same as the player's.
        /// </exception>
        /// <exception cref="InvalidOperationException">Thrown if the method is invoked after the game has ended.</exception>
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

            this.SimulateWait();

            /**
             * Summary:
             * The impossible-level bot should be impossible to win against.
             * It plays with perfect accuracy, finding the optimal play in every position by searching up the state map.

             * Remarks:
             * - The user can lose to this bot quickly if they make a mistake.
             * - If there are multiple optimal plays in a given position, the bot chooses any random play.
             * - This adds temperature to make the game interesting and dynamic, rather than following a pre-determined state flow.
             */
            int moveChoice = this.LookupSearchMap((xState, oState));
            this.MarkSpot(moveChoice);
            return moveChoice;
        }
    }
}
