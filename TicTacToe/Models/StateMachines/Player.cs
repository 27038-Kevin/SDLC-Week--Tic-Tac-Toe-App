namespace TicTacToe.Models.StateMachines
{
    /// <summary>
    /// Represents a player in a tic-tac-toe game, encapsulating the player's state, win conditions, and move logic.
    /// </summary>
    internal abstract class Player
    {
        /// <summary>
        /// Constant representing a fully marked board state; used for inverse checks during bitwise operations.
        /// </summary>
        protected const int FullBoardMask = 0b111_111_111;

        /// <summary>
        /// Represents the indices of the squares on the board.
        /// </summary>
        protected static readonly IEnumerable<int> Squares = Enumerable.Range(0, 9);

        /// <summary>
        /// Has an array of individual bits representing each square on the tic-tac-toe board.
        /// </summary>
        protected static readonly int[] IndividualBits = Squares.Select(i => 1 << i)
                                                            .ToArray();

        /// <summary>
        /// Defines the numerical board states representing a win condition.
        /// </summary>
        protected static readonly int[] WinConditions =
        {
            0b111_000_000,  // Top row
            0b000_111_000,  // Middle row
            0b000_000_111,  // Bottom row
            0b100_100_100,  // Left column
            0b010_010_010,  // Middle column
            0b001_001_001,  // Right column
            0b100_010_001,  // Diagonal from top-left to bottom-right
            0b001_010_100,  // Diagonal from top-right to bottom-left
        };

        /// <summary>
        /// Counts the number of spots marked on the board by the player.
        /// </summary>
        protected int _markedSpots = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="Player"/> class.
        /// </summary>
        /// <param name="letter">The letter assigned to the player for the game (X/O).</param>
        /// <param name="name">The name of the player.</param>
        protected Player(char letter, string name)
        {
            if (letter != 'x' && letter != 'X'
                && letter != 'o' && letter != 'O')
            {
                throw new ArgumentException("Invalid letter!", nameof(letter));
            }

            Letter = char.ToUpper(letter);
            Name = name;
        }

        /// <summary>
        /// Gets the letter the player has to play (X/O).
        /// </summary>
        /// <value>X if the player goes first; O otherwise.</value>
        public char Letter { get; init; }

        /// <summary>
        /// Gets the player's name.
        /// </summary>
        /// <value>The name of the player, provided at the time of initialization.</value>
        public string Name { get; init; } = string.Empty;

        /// <summary>
        /// Gets the current state of the player's marked spots on the board, represented as a bitwise integer.
        /// </summary>
        /// <value>9-bit integer, each integer mapping to an index on the board.</value>
        /// <remarks>
        /// Board indexing looks like
        ///  0 | 1 | 2
        /// ---+---+---
        ///  3 | 4 | 5
        /// ---+---+---
        ///  6 | 7 | 8
        /// </remarks>
        public int State { get; private set; } = 0b000_000_000;

        /// <summary>
        /// Gets a value indicating whether the player has won the game based on the current state of the board.
        /// </summary>
        /// <value>true if the board state from the player's point of view matches any win condition; false otherwise.</value>
        public bool HasWon => WinConditions.Any(
            condition => (State & condition) == condition);

        /// <summary>
        /// Makes a move for the player based on the opponent's current state of the board.
        /// </summary>
        /// <param name="opponent">The opponent player.</param>
        /// <returns>The square marked by the player on the current turn.</returns>
        public abstract int GetMove(Player opponent);

        /// <summary>
        /// Checks whether a square is marked on the board by either player.
        /// </summary>
        /// <param name="square">The square to check.</param>
        /// <param name="opponentState">The board state from the opponent's perspective.</param>
        /// <returns>true if the square is marked by either player, false otherwise.</returns>
        protected bool IsMarked(int square, int opponentState)
        {
            int bitFlag = IndividualBits[square];
            return (State & bitFlag) != 0
                || (opponentState & bitFlag) != 0;
        }

        /// <summary>
        /// Marks a spot on the board for the player, updating the player's state and the count of marked spots.
        /// </summary>
        /// <param name="spotIndex">The index representing the square that was marked by the player.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the spot index is out of the tic-tac-toe board's range.</exception>
        /// <exception cref="ArgumentException">Thrown if there is an attempt to mark a square that has already been marked.</exception>
        protected void MarkSpot(int spotIndex)
        {
            if (spotIndex < 0 || spotIndex > 8)
            {
                throw new ArgumentOutOfRangeException(nameof(spotIndex), "Spot index must be between 0 and 8.");
            }

            int flag = 1 << spotIndex;

            if ((State & flag) != 0)
            {
                throw new ArgumentException($"Spot {spotIndex} is already marked.", nameof(spotIndex));
            }

            // Mark the spot by setting the corresponding bit in the state integer.
            State |= flag;
            _markedSpots++;
        }
    }
}
