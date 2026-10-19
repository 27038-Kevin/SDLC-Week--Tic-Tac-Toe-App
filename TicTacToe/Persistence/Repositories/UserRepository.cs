using TicTacToe.Models.Data;
using TicTacToe.Persistence.Serializers;

namespace TicTacToe.Persistence.Repositories
{
    /// <summary>
    /// Manages file-based storage and retrieval of <see cref="User"/> objects using serialization.
    /// Abstracts the underlying file operations and retrieval queries.
    /// </summary>
    internal class UserRepository : FileRepository<User>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserRepository"/> class with the specified file directory, file name, and serializer.
        /// </summary>
        /// <param name="fileDirectory">The directory the file resides in.</param>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="serializer">The serializer used to serialize/deserialize data to and from the repository.</param>
        internal UserRepository(string fileDirectory, string fileName, ISerializer<User> serializer)
                : base(fileDirectory, fileName, serializer)
        {
        }

        /// <summary>
        /// Gets all usernames in the repository.
        /// </summary>
        /// <returns>All usernames from the file.</returns>
        internal IEnumerable<string> GetAllUsernames()
        {
            return this.GetAll()
                    .Select(user => user.Username);
        }

        /// <summary>
        /// Gets a <see cref="User"/> by their username.
        /// </summary>
        /// <param name="username">The user's username (must be unique).</param>
        /// <returns>The user, if their username is found.</returns>
        /// <exception cref="KeyNotFoundException">
        /// Thrown if the user's username cannot be found in the repository.
        /// </exception>
        internal User GetByUsername(string username)
        {
            return this.GetAll()
                    .FirstOrDefault(user => user.Username == username)
                ?? throw new KeyNotFoundException($"User with username '{username}' not found.");
        }
    }
}