namespace TicTacToe.Models.Data
{
    /// <summary>
    /// Represents a single user of the application.
    /// Used for login, account management, and session management.
    /// </summary>
    internal class User : IHasId
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class.
        /// </summary>
        public User()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class.
        /// </summary>
        /// <param name="id">The user's GUID.</param>
        /// <param name="name">The user's name.</param>
        /// <param name="username">The user's username.</param>
        /// <param name="password">The user's password.</param>
        public User(Guid id, string name, string username, Password password)
        {
            this.Id = id;
            this.Name = name;
            this.Username = username;
            this.Password = password;
        }

        /// <summary>
        /// Gets the global unique identifier of the user.
        /// </summary>
        /// <value>The GUID generated for the user.</value>
        public Guid Id { get; init; }

        /// <summary>
        /// Gets or sets the user's name.
        /// </summary>
        /// <value>The user's name (full/nickname is based on user's choice).</value>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets the user's account username.
        /// </summary>
        /// <value>The username selected by the user during registration.</value>
        public string Username { get; init; } = string.Empty;

        /// <summary>
        /// Gets or sets the user's password.
        /// </summary>
        /// <value>
        /// The hashcode of the user's password,
        /// along with the salt used to produce the hashcode.
        /// </value>
        public Password Password { get; set; }
    }
}
