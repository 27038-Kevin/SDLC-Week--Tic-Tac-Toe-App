using System.ComponentModel;
using TicTacToe.Enums;
using TicTacToe.Models.Data;
using TicTacToe.Models.StateMachines;
using TicTacToe.Models.Utils;
using TicTacToe.Persistence.Repositories;
using TicTacToe.View.GameRenderers;

namespace TicTacToe.Services
{
    /// <summary>
    /// Oversees repository operations on <see cref="GameSummary"/> objects,
    /// as well as game-related services such as playing, rewatching, etc.
    /// </summary>
    internal class GameService
    {
        private readonly GameRepository _repository;
        private readonly IGameDisplay _gameDisplay;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameService"/> class.
        /// </summary>
        /// <param name="repository">The repository that aggregates <see cref="GameSummary"/> instances.</param>
        /// <param name="gameDisplayViewModel">The view model used to render the game live.</param>
        public GameService(GameRepository repository, IGameDisplay gameDisplayViewModel)
        {
            this._repository = repository;
            this._gameDisplay = gameDisplayViewModel;
        }

        /// <summary>
        /// Gets a game based on its GUID.
        /// </summary>
        /// <param name="id">The GUID to fetch a game by.</param>
        /// <returns>The game, if found.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if the game summary could not be found for the provided GUID.</exception>
        public GameSummary GetGame(Guid id)
        {
            return this._repository.GetById(id);    // Throws KeyNotFoundException
        }

        /// <summary>
        /// Gets a game based on its game number.
        /// </summary>
        /// <param name="gameNumber">The game number to fetch a game by.</param>
        /// <returns>The game, if found.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if the game summary could not be found for the provided game number.</exception>
        public GameSummary GetGame(uint gameNumber)
        {
            return this._repository.GetByGameNumber(gameNumber);
        }

        /// <summary>
        /// Gets a list of game summaries based on custom filters.
        /// </summary>
        /// <param name="filter">Struct containing the filters for the repository query.</param>
        /// <returns>The list of game summaries from the repository.</returns>
        public IEnumerable<GameSummary> GetGames(GameSummaryFilter filter)
        {
            return this._repository.Filter(filter);
        }

        /// <summary>
        /// Generates a stats model for the selected game mode.
        /// </summary>
        /// <param name="mode">The game mode to select stats for.</param>
        /// <returns>The stats model for games played against the computer difficulty level.</returns>
        /// <exception cref="NotSupportedException">Thrown if there is an attempt to get stats for manual mode.</exception>
        public GameStats GetStats(GameMode mode)
        {
            if (mode == GameMode.Manual)
            {
                throw new NotSupportedException("Statistics not calculated for manual mode");
            }

            var modeFilter = new GameSummaryFilter(mode: mode);

            var xFilter = new GameSummaryFilter(
                                mode: mode,
                                letterPlayed: Letter.X);

            var oFilter = new GameSummaryFilter(
                                mode: mode,
                                letterPlayed: Letter.O);

            var winFilter = new GameSummaryFilter(
                                mode: mode,
                                outcome: PlayerOrientedGameOutcome.Win);

            var lossFilter = new GameSummaryFilter(
                                mode: mode,
                                outcome: PlayerOrientedGameOutcome.Loss);

            var tieFilter = new GameSummaryFilter(
                                mode: mode,
                                outcome: PlayerOrientedGameOutcome.Tie);

            return new GameStats(
                    mode,
                    this._repository.Filter(modeFilter).Count(),
                    this._repository.Filter(xFilter).Count(),
                    this._repository.Filter(oFilter).Count(),
                    this._repository.Filter(winFilter).Count(),
                    this._repository.Filter(lossFilter).Count(),
                    this._repository.Filter(tieFilter).Count());
        }

        /// <summary>
        /// Starts a new game.
        /// </summary>
        /// <param name="mode">The game mode to start the game in. For values, see <see cref="GameMode"/>.</param>
        /// <param name="humanGoesFirst">
        /// Whether or not the human player goes first.
        /// Irrelevant parameter for manual mode.
        /// </param>
        /// <param name="player1Name">
        /// The name of the first (default) player passed as a parameter for game metadata information.
        /// </param>
        /// <param name="player2Name">
        /// The name of a second player passed as a parameter for game metadata information.
        /// Unused parameter for easy/medium/impossible mode.
        /// </param>
        /// <exception cref="InvalidEnumArgumentException">Thrown if the game mode is not not a known value.</exception>
        public void StartNewGame(GameMode mode, bool humanGoesFirst, string? player1Name = default, string? player2Name = default)
        {
            Player xPlayer, oPlayer;

            if (mode == GameMode.Manual)
            {
                xPlayer = new HumanPlayer(Letter.X, player1Name ?? "Player", this._gameDisplay.GetMoveInput);
                oPlayer = new HumanPlayer(Letter.O, player2Name ?? "Player", this._gameDisplay.GetMoveInput);
            }
            else
            {
                Letter humanLetter = humanGoesFirst ? Letter.X : Letter.O;
                Letter botLetter = humanGoesFirst ? Letter.O : Letter.X;

                Player human = new HumanPlayer(humanLetter, player1Name ?? "Player", this._gameDisplay.GetMoveInput);
                Player bot = mode switch
                {
                    GameMode.Easy => new EasyBotPlayer(botLetter),
                    GameMode.Medium => new MediumBotPlayer(botLetter),
                    GameMode.Impossible => new ImpossibleBotPlayer(botLetter),
                    _ => throw new InvalidEnumArgumentException("Unknown game mode"),
                };

                xPlayer = humanGoesFirst ? human : bot;
                oPlayer = humanGoesFirst ? bot : human;
            }

            uint gameNo = (uint)this._repository.GetAll().Count() + 1;
            var game = new Game(gameNo, xPlayer, oPlayer, mode, this._gameDisplay.RenderGameBoard);
            game.Start();   // Wait for game to end

            // Do not save games that were not started in the first place
            if (game.Moves.Any())
            {
                this._repository.Save(game.ExportSummary());
            }
        }
    }
}
