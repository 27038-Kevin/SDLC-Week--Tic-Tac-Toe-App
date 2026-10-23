using System.Text;
using TicTacToe.Controllers;
using TicTacToe.Models.Data;
using TicTacToe.Persistence.Repositories;
using TicTacToe.Persistence.Serializers;
using TicTacToe.Services;

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

            const string usersFileName = Constants.UsersFileName;
            const string gamesFileName = Constants.GamesFileName;

            string directoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Constants.DataDirectoryName);
            var userSerializer = new CsvSerializer<User>();
            var userRepository = new UserRepository(directoryPath, usersFileName, userSerializer);
            var userService = new UserService(userRepository);
            var userController = new UserController(userService, directoryPath);

            var app = new ApplicationController(directoryPath, gamesFileName, userController);
            app.Run();
        }
    }
}