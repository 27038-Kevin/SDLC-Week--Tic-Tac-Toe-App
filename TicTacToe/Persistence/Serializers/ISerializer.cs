namespace TicTacToe.Persistence.Serializers
{
    /// <summary>
    /// Defines a contract for serializing and deserializing data between objects and string representations.
    /// </summary>
    /// <typeparam name="T">The type of object the serializer works for.</typeparam>
    internal interface ISerializer<T>
    {
        /// <summary>
        /// Serializes an object to its respective serialized string representation.
        /// </summary>
        /// <param name="entity">The object to be serialized.</param>
        /// <returns>The line representing the serialized object.</returns>
        string Serialize(T entity);

        /// <summary>
        /// Deserializes a line to the object it characterizes.
        /// </summary>
        /// <param name="line">The line to be parsed and deserialized.</param>
        /// <returns>The respective object representation of the line.</returns>
        T Deserialize(string line);
    }
}
