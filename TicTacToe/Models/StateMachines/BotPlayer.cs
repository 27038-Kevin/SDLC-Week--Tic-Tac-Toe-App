using TicTacToe.Enums;

namespace TicTacToe.Models.StateMachines
{
    /// <summary>
    /// Represents a tic-tac-toe bot player, which is the abstract class serving as the base for all tic-tac-toe bot implementations.
    /// </summary>
    internal abstract class BotPlayer : Player
    {
        /// <summary>
        /// Common field to all tic-tac-toe bots;
        /// can be used to potentially increase the temperature of the AI,
        /// or to pick deliberately losing moves.
        /// </summary>
        protected readonly Random _random = new ();

        /// <summary>
        /// Enumerates and stores the most optimal plays for every position,
        /// along with a position score that provides a heuristic for evaluation.
        /// </summary>
        private static readonly Dictionary<
                    (int XState, int OState),
                    (int[] BestMoves, int Score)>
            StateMap = new ();

        /// <summary>
        /// Initializes static members of the <see cref="BotPlayer"/> class.
        /// Generates the state map upon initialization.
        /// </summary>
        static BotPlayer()
        {
            EnumeratePosition(
                xState: 0,
                oState: 0,
                xToMove: true,
                depth: 0,
                maxDepth: 9);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BotPlayer"/> class.
        /// </summary>
        /// <param name="letter">The letter assigned to the player.</param>
        /// <param name="name">The name designated to the bot.</param>
        public BotPlayer(Letter letter, string name)
            : base(letter, name)
        {
        }

        /// <summary>
        /// Gets the index of the square that would allow the player to win in their next move, if such a square exists.
        /// </summary>
        /// <param name="playerState">The player's board state.</param>
        /// <param name="opponentState">The opponent's board state.</param>
        /// <returns>
        /// The index of the winning square, if such a square exists.
        /// -1 if there is no winning square.
        /// </returns>
        protected static int GetWinningSquare(int playerState, int opponentState)
        {
            foreach (int condition in WinConditions)
            {
                // Get the bits that need to be added to win for the current condition
                int stateAddendToWin = condition & ~playerState;

                // Check whether the required addend is a single square (i.e., a power of two)
                int bitIndex = Array.BinarySearch(IndividualBits, stateAddendToWin);
                bool isAddendSingleSquare = bitIndex >= 0;

                // If that square is unoccupied, the player is one move away from winning
                if (isAddendSingleSquare && (opponentState & stateAddendToWin) == 0)
                {
                    return bitIndex;
                }
            }

            return -1; // No winning square found
        }

        /// <summary>
        /// Simulates waiting time for a bot.
        /// </summary>
        protected void SimulateWait()
        {
            int maximumWaitTime = 2500; // ms
            float randomCoefficient = this._random.NextSingle();
            float waitTime = randomCoefficient * maximumWaitTime;
            Thread.Sleep((int)waitTime);
        }

        /// <summary>
        /// Looks up the pre-computed search map for the most optimal plays in a position.
        /// </summary>
        /// <param name="boardState">The total state of the board, consisting of both the X-player's state and the O-player's state.</param>
        /// <returns>A random choice from the list of most optimal choices for the specified board state.</returns>
        protected int LookupSearchMap((int XState, int OState) boardState)
        {
            int[] viableChoices = StateMap[boardState].BestMoves;
            int index = this._random.Next(viableChoices.Length);
            return viableChoices[index];
        }

        /// <summary>
        /// Enumerates all follow-up positions, scoring it based on the position's most likely outcome with optimal play.
        /// Additional memoizes the position along with its board state so that it can be referenced by bots in the future.
        /// </summary>
        /// <param name="xState">The board state from the X-player's perspective.</param>
        /// <param name="oState">The board state from the O-player's perspective.</param>
        /// <param name="xToMove">Whether or not the current move is X's turn.</param>
        /// <param name="depth">The current depth at which enumeration is taking place.</param>
        /// <param name="maxDepth">The maximum allowed depth for enumeration.</param>
        /// <returns>The score of the position.</returns>
        private static int EnumeratePosition(
            int xState,
            int oState,
            bool xToMove,
            int depth,
            int maxDepth)
        {
            // Return score if x has already won
            if (HasWon(xState))
            {
                return 10 - depth;  // Deduct depth for faster wins
            }

            // Return negative score if o has already won
            if (HasWon(oState))
            {
                return depth - 10;  // Add depth for slower losses
            }

            // Return zero if a tie has occurred
            int occupied = xState | oState;
            if (occupied == FullBoardMask)
            {
                return 0;
            }

            // Return zero if reached max depth
            if (depth >= maxDepth)
            {
                return 0;
            }

            // Check memo for state
            var key = (xState, oState);
            if (StateMap.TryGetValue(key, out var cached))
            {
                return cached.Score;
            }

            int bestScore = xToMove ? int.MinValue : int.MaxValue;
            var bestMoves = new List<int>();

            for (int move = 0; move < 9; ++move)
            {
                int mask = 1 << move;
                if ((occupied & mask) != 0)
                {
                    continue;
                }

                int nextX = xState;
                int nextO = oState;

                if (xToMove)
                {
                    nextX |= mask;
                }
                else
                {
                    nextO |= mask;
                }

                int score = EnumeratePosition(nextX, nextO, !xToMove, depth + 1, maxDepth);

                if ((xToMove && score > bestScore) ||
                    (!xToMove && score < bestScore))
                {
                    bestScore = score;
                    bestMoves.Clear();
                    bestMoves.Add(move);
                }
                else if (score == bestScore)
                {
                    bestMoves.Add(move);
                }
            }

            StateMap.Add(key, (bestMoves.ToArray(), bestScore));
            return bestScore;

            static bool HasWon(int state)
            {
                return WinConditions.Any(condition => (state & condition) == condition);
            }
        }
    }
}
