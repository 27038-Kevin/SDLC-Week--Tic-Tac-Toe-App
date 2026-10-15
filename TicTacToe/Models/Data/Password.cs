using System.Collections.Immutable;

namespace TicTacToe.Models.Data
{
    /// <summary>
    /// Stores a user's password securely.
    /// </summary>
    internal readonly struct Password
    {
        private const char Delimiter = ',';

        /// <summary>
        /// Initializes a new instance of the <see cref="Password"/> struct.
        /// </summary>
        /// <param name="hashcode">The hashcode of the salted password.</param>
        /// <param name="salt">The hex string of the salt.</param>
        public Password(string hashcode, string salt)
        {
            this.Hashcode = hashcode;
            this.Salt = Convert.FromHexString(salt)
                            .ToImmutableArray();
        }

        /// <summary>
        /// Gets the password's hashcode, encoded as a hex string.
        /// </summary>
        /// <value>The hashcode of the password, encoded as a hex string.</value>
        public readonly string Hashcode { get; init; }

        /// <summary>
        /// Gets the salt used to hash the password.
        /// </summary>
        /// <value>The salt used to hash the password, stored immutably to maintain integrity.</value>
        public readonly ImmutableArray<byte> Salt { get; init; }

        /// <summary>
        /// Method to deserialize an encoded/serialized password string.
        /// </summary>
        /// <param name="encodedPassword">The serialized string representation of the password's hashcode and salt.</param>
        /// <returns>The password struct after parsing the serialized string representation.</returns>
        /// <exception cref="FormatException">
        /// Thrown if the password's salt is not encoded properly,
        /// or if the password encoding itself is unrecognizable.
        /// </exception>
        public static Password FromString(string encodedPassword)
        {
            var parts = encodedPassword.Split(Delimiter);
            if (parts.Length != 2)
            {
                throw new FormatException("Unrecognized password encoding format");
            }

            return new Password(parts[0], parts[1]);
        }

        /// <summary>
        /// Serializes the password struct.
        /// </summary>
        /// <returns>An interpolated string containing the hashcode and salt encoded as hex strings, separated by a pipe (|) character.</returns>
        public override string ToString()
        {
            byte[] saltBytes = this.Salt.ToArray();
            return string.Concat(this.Hashcode, Delimiter, Convert.ToHexString(saltBytes));
        }
    }
}