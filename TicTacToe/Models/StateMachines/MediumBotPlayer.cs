using TicTacToe.Enums;

namespace TicTacToe.Models.StateMachines
{
    /// <summary>
    /// A tic-tac-toe bot that plays at the medium level -- it does not easily lose to the user, and attempts to put up a fight most of the time.
    /// </summary>
    internal sealed class MediumBotPlayer : BotPlayer
    {
        private const string BotName = "Computer (Medium)";

        /// <summary>
        /// Initializes a new instance of the <see cref="MediumBotPlayer"/> class.
        /// </summary>
        /// <param name="letter">The letter assigned to the player (X/O).</param>
        public MediumBotPlayer(Letter letter)
            : base(letter, BotName)
        {
        }

        /// <summary>
        /// Gets the next move from the medium-mode AI bot.
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

            /**
             * Summary:
             * The medium-level bot should be able to hold its own against a human player.
             * While it should not lose unnecessarily or quickly, it can occasionally let a human win.

             * Pipeline:
             * There are two main strategies in play:
             * - A pre-computed state map
             * - A random-heuristic strategy

             * There is a 50-50 chance of the bot choosing between the state map and the random heuristic to get the next move.
             * 1. The random heuristic first checks whether a win is possible. If so, the bot makes the winning move.
             * 2. Next, it checks whether the opponent has a winning move within the next turn. If so, the bot marks that square to prevent the opponent from winning.
             * 3. As a final fall-back mechanism in the random heuristic, the bot will finally mark a square randomly if there are no opportunities for winning or prophylaxis.

             * Remarks:
             * - The bot can be beat if it randomly walks into a fork in the first few moves of the game.
             * - Likewise, the bot can win if the user makes a mistake into a fork within the first two moves of the game, and it randomly finds the fork afterwards.
             * - There is only a 50% probability that it will opt for the random heuristic strategy after the first two moves.
             * - This balances out the shortcomings of the random heuristic approach by drastically reducing its frequency.
             */
            int xState = this.Letter == Letter.X ? this.State : opponent.State;
            int oState = this.Letter == Letter.O ? this.State : opponent.State;
            var boardState = (xState, oState);

            this.SimulateWait();

            // Get a random condition to choose a strategy
            bool randomStrategy = this._random.Next() % 2 == 0;
            if (randomStrategy)
            {
                // If winning is possible, then mark the winning square
                int winningSquare = GetWinningSquare(this.State, opponent.State);
                if (winningSquare != -1)
                {
                    this.MarkSpot(winningSquare);
                    return winningSquare;
                }

                // If the opponent can win, block the win
                int opponentWinningSquare = GetWinningSquare(opponent.State, this.State);
                if (opponentWinningSquare != -1)
                {
                    this.MarkSpot(opponentWinningSquare);
                    return opponentWinningSquare;
                }

                // If neither winning nor prophlyaxis are feasible, then fallback to randomization
                int[] candidateMoves = Squares.Where(square => !this.IsMarked(square, opponent.State))
                                            .ToArray();

                int randomIndex = this._random.Next(candidateMoves.Length);
                int square = candidateMoves[randomIndex];

                this.MarkSpot(square);
                return square;
            }
            else
            {
                int moveChoice = this.LookupSearchMap(boardState);
                this.MarkSpot(moveChoice);
                return moveChoice;
            }
        }
    }
}
