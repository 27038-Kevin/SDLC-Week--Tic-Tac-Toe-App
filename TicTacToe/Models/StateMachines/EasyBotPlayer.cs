using TicTacToe.Enums;

namespace TicTacToe.Models.StateMachines
{
    /// <summary>
    /// A tic-tac-toe bot that plays at the easy level -- it does not prevent the user from winning the game and moves without any direction or motive.
    /// </summary>
    internal sealed class EasyBotPlayer : BotPlayer
    {
        private const string BotName = "Computer (Easy)";

        /// <summary>
        /// Initializes a new instance of the <see cref="EasyBotPlayer"/> class.
        /// </summary>
        /// <param name="letter">The letter assigned to the player (X/O).</param>
        public EasyBotPlayer(Letter letter)
            : base(letter, BotName)
        {
        }

        /// <summary>
        /// Gets the next move from the easy-mode AI bot.
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

            this.SimulateWait();

            /**
             * Summary:
             * The easy-level bot player should select randomly as long as its move does not prevent the opponent from winning in their next move.

             * Pipeline:
             * 1. If a win is possible anywhere ont the board, play the winning move.
             * 2. Otherwise, find all unmarked squares on the board. Select these as a list of potential moves.
             * 3. Check if the opponent has a winning square. Remove this square from the list of potential moves, if it exists.
             * 4. Randomly select a square from the remaining potential moves.

             * Remarks:
             * - The bot does not attempt to block the opponent from winning.
             * - The bot does not have any strategy. It moves semi-randomly.
             * - In the event of a fork, the opponent may have multiple ways to win.
             * - The bot only finds one winning square, and removes that square from its list of candidate moves.
             * - This mechanism will allow the opponent to win in at least one way.
             */
            int winningSquare = GetWinningSquare(this.State, opponent.State);
            if (winningSquare != -1)
            {
                this.MarkSpot(winningSquare);
                return winningSquare;
            }

            int winningSquareForOpponent = GetWinningSquare(opponent.State, this.State);
            int[] candidateMoves = Squares.Where(square => !this.IsMarked(square, opponent.State)
                                                && square != winningSquareForOpponent)
                                          .ToArray();

            int randomIndex = this._random.Next(candidateMoves.Length);
            int square = candidateMoves[randomIndex];

            this.MarkSpot(square);
            return square;
        }
    }
}
