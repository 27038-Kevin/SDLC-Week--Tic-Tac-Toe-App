using TicTacToe.Enums;
using TicTacToe.Exceptions;
using TicTacToe.Models.Data;

namespace TicTacToe.Models.StateMachines
{
    /// <summary>
    /// State machine representing a tic-tac-toe game's internal state transitions.
    /// </summary>
    internal class Game
    {
        private readonly uint _gameNumber;
        private readonly Action<Game> _renderGameBoard;

        /// <summary>
        /// The current turn number.
        /// </summary>
        private int _turnNumber;

        /// <summary>
        /// Initializes a new instance of the <see cref="Game"/> class.
        /// </summary>
        /// <param name="gameNumber">The game number used for identification and enumeration.</param>
        /// <param name="xPlayer">The player who plays X.</param>
        /// <param name="oPlayer">The player who plays O.</param>
        /// <param name="gameMode">The game mode/difficulty level.</param>
        /// <param name="renderGameBoard">Renders the game board on the view.</param>
        public Game(
                uint gameNumber,
                Player xPlayer,
                Player oPlayer,
                GameMode gameMode,
                Action<Game> renderGameBoard)
        {
            this._gameNumber = gameNumber;

            this.XPlayer = xPlayer;
            this.OPlayer = oPlayer;

            this._renderGameBoard = renderGameBoard;

            this.GameMode = gameMode;
            this.GameStatus = GameStatus.Ready;
            this.GameOutcome = GameOutcome.Undecided;
        }

        /// <summary>
        /// Gets the player who plays X in the game.
        /// </summary>
        /// <value>Player (computer or human) who goes first and plays X.</value>
        public Player XPlayer { get; init; }

        /// <summary>
        /// Gets the player who plays O in the game.
        /// </summary>
        /// <value>Player (computer or human) who goes next and plays O.</value>
        public Player OPlayer { get; init; }

        /// <summary>
        /// Gets the current status of the game.
        /// </summary>
        /// <value>
        /// Ready when the game is ready to be played.
        /// Ongoing when the game is currently being played.
        /// Completed when the game was successfully finished with a winner or a tie.
        /// Unfinished when the game remains incomplete due to a player quitting.
        /// </value>
        public GameStatus GameStatus { get; private set; }

        /// <summary>
        /// Gets the outcome of the game.
        /// </summary>
        /// <value>
        /// Undecided when the game is currently ongoing or still yet to be played.
        /// XWon when the player playing X won the game.
        /// OWon when the player playing O won the game.
        /// Tie when the game ends in a tie.
        /// Quit when either player quits the game.
        /// </value>
        public GameOutcome GameOutcome { get; private set; }

        /// <summary>
        /// Gets the game mode/difficulty level.
        /// </summary>
        /// <value>Easy, medium, impossible, or manual.</value>
        public GameMode GameMode { get; init; }

        /// <summary>
        /// Gets a value indicating whether it is currently X's turn or not.
        /// </summary>
        /// <value>true if it is X's turn to play; false otherwise.</value>
        public bool IsXTurn => this.GameStatus == GameStatus.Ongoing && this._turnNumber % 2 == 1;

        /// <summary>
        /// Gets a value indicating whether it is currently O's turn or not.
        /// </summary>
        /// <value>true if it is O's turn to play; false otherwise.</value>
        public bool IsOTurn => this.GameStatus == GameStatus.Ongoing && this._turnNumber % 2 == 0;

        /// <summary>
        /// Gets the list of moves leading up to the current position on the board.
        /// </summary>
        /// <value>The list of moves, each integer representing the marked square in the tic-tac-toe board.</value>
        public List<int> Moves { get; } = new List<int>();

        /// <summary>
        /// Starts a game, observing its state until the game either ends or is disrupted.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the game has already happened at least once.</exception>
        public void Start()
        {
            if (this.GameStatus != GameStatus.Ready)
            {
                throw new InvalidOperationException("Cannot restart game");
            }

            this.GameStatus = GameStatus.Ongoing;
            this.GameOutcome = GameOutcome.Undecided;

            while (this.GameStatus == GameStatus.Ongoing)
            {
                ++this._turnNumber;
                try
                {
                    this._renderGameBoard(this);

                    var move = this.IsXTurn
                        ? this.XPlayer.GetMove(this.OPlayer)
                        : this.OPlayer.GetMove(this.XPlayer);
                    this.Moves.Add(move);

                    if (this.XPlayer.HasWon)
                    {
                        this.GameStatus = GameStatus.Completed;
                        this.GameOutcome = GameOutcome.XWon;
                    }
                    else if (this.OPlayer.HasWon)
                    {
                        this.GameStatus = GameStatus.Completed;
                        this.GameOutcome = GameOutcome.OWon;
                    }
                    else if (this._turnNumber == 9)
                    {
                        // 9 moves with no winner == tie
                        this.GameStatus = GameStatus.Completed;
                        this.GameOutcome = GameOutcome.Tie;
                    }
                }
                catch (QuitGameException)
                {
                    this.GameStatus = GameStatus.Unfinished;
                    this.GameOutcome = GameOutcome.Quit;
                }
            }

            this._renderGameBoard(this);
        }

        /// <summary>
        /// Gets a summary for the game, containing crucial details for storing in the game history.
        /// </summary>
        /// <returns>A summary representing the details of the game.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the game has not yet been completed.</exception>
        public GameSummary ExportSummary()
        {
            if (this.GameStatus != GameStatus.Completed && this.GameStatus != GameStatus.Unfinished)
            {
                throw new InvalidOperationException("Cannot export summary of a game that has not completed");
            }

            return new GameSummary
            {
                Id = Guid.NewGuid(),
                GameNo = this._gameNumber,
                XPlayer = this.XPlayer.Name,
                OPlayer = this.OPlayer.Name,
                IsXHuman = Player.IsHuman(this.XPlayer),
                IsOHuman = Player.IsHuman(this.OPlayer),
                DateAndTime = DateTime.Now,
                Moves = this.Moves.ToArray(),
                GameOutcome = this.GameOutcome,
                GameMode = this.GameMode,
            };
        }
    }
}
