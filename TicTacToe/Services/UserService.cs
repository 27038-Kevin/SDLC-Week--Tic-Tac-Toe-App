using TicTacToe.Models.Data;
using TicTacToe.Persistence.Repositories;

namespace TicTacToe.Services
{
    /// <summary>
    /// Services related to user management and repository methods.
    /// </summary>
    internal class UserService
    {
        private readonly UserRepository _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserService"/> class.
        /// </summary>
        /// <param name="repository">Repository to be injected into the service.</param>
        internal UserService(UserRepository repository)
        {
            this._repository = repository;
        }

        /// <summary>
        /// Deletes a user from the repository.
        /// </summary>
        /// <param name="userId">The GUID of the user.</param>
        /// <exception cref="KeyNotFoundException">Thrown if the user's email cannot be found.</exception>
        internal void DeleteUser(Guid userId)
        {
            this._repository.Delete(userId);    // Throws KeyNotFoundException
        }

        /// <summary>
        /// Whether or not the given username already exists in the userbase.
        /// </summary>
        /// <param name="username">The username to be validated.</param>
        /// <returns>true if the username exists; false otherwise.</returns>
        internal bool DoesUsernameExist(string username)
        {
            var usernames = this._repository.GetAllUsernames();
            return usernames.Contains(username);
        }

        /// <summary>
        /// Gets a user from the repository.
        /// </summary>
        /// <param name="username">Username of the user account.</param>
        /// <returns>The user data from the repository.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if the username cannot be found.</exception>
        internal User GetUser(string username)
        {
            return this._repository.GetByUsername(username);  // Throws KeyNotFoundException
        }

        /// <summary>
        /// Gets a user from the repository.
        /// </summary>
        /// <param name="userId">ID of the user.</param>
        /// <returns>The user data from the repository.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if the user's ID cannot be found.</exception>
        internal User GetUser(Guid userId)
        {
            return this._repository.GetById(userId);    // Throw KeyNotFoundException
        }

        /// <summary>
        /// Registers a new user and saves their details to the repository.
        /// </summary
        /// <param name="username">Username the user created.</param>
        /// <param name="password">Password of the user.</param>
        /// <param name="name">The user's name.</param>
        /// <exception cref="ArgumentException">
        /// Thrown if username already exists in the userbase.
        /// Can be avoided using <see cref="DoesUsernameExist(string)"/>.
        /// </exception>
        internal void RegisterUser(string username, Password password, string name)
        {
            if (this.DoesUsernameExist(username))
            {
                throw new ArgumentException("Username already exists", nameof(username));
            }

            this._repository.Add(
                new User(Guid.NewGuid(), name, username, password));
        }

        /// <summary>
        /// Updates a user's details.
        /// </summary>
        /// <param name="updatedUserInfo">User object containing updated data.</param>
        internal void UpdateUser(User updatedUserInfo)
        {
            // Throws KeyNotFoundException
            this._repository.Update(updatedUserInfo);
        }
    }
}