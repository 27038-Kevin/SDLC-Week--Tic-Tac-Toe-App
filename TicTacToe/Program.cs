using System.Text;
using TicTacToe.Controllers;
using TicTacToe.Models.Data;
using TicTacToe.Persistence.Repositories;
using TicTacToe.Persistence.Serializers;
using TicTacToe.Services;
using TicTacToe.View.GameRenderers;

namespace TicTacToe
{
    /// <summary>
    /// Contains the composition root for the main application.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Main entry point of the application.
        /// </summary>
        private static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.CursorVisible = false;

            string directoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Constants.DataDirectoryName);
            string gamesFileName = Constants.GamesFileName;

            var gameDisplay = new GameDisplay();
            var gameSerializer = new CsvSerializer<GameSummary>();
            var gameRepository = new GameRepository(directoryPath, gamesFileName, gameSerializer);
            var gameService = new GameService(gameRepository, gameDisplay);
            var gameController = new GameController(gameService, gameDisplay);

            var app = new ApplicationController(gameController);
            app.Run();
        }
    }
}