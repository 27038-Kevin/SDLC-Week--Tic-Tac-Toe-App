using TicTacToe.Models.Data;
using TicTacToe.Persistence.Serializers;

namespace TicTacToe.Persistence.Repositories
{
    /// <summary>
    /// Provides a base class for managing file-based storage and retrieval of objects of type T using serialization.
    /// </summary>
    /// <typeparam name="T">The type of objects managed by the repository. Must be a class with a parameterless constructor.</typeparam>
    internal abstract class FileRepository<T> : IRepository<T>
            where T : IHasId
    {
        /// <summary>
        /// The data file path to retrieve data from.
        /// </summary>
        protected readonly string _filePath;

        /// <summary>
        /// The serializer object used to serialize/deserialize data.
        /// </summary>
        protected readonly ISerializer<T> _serializer;

        /// <summary>
        /// A temporary in-memory buffer containing the data items.
        /// Smoothens repository operations instead of accessing the main file every time.
        /// </summary>
        private readonly List<T> _items;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileRepository{T}"/> class with the specified file directory, file name, and serializer.
        /// </summary>
        /// <param name="fileDirectory">The directory in which the file resides.</param>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="serializer">The serializer used to serialize/deserialize data to and from the file.</param>
        /// <exception cref="ArgumentException">Thrown if either the file name or file directory name is either blank or null.</exception>
        /// <exception cref="ArgumentNullException">Thrown when the serializer argument is empty.</exception>
        /// <remarks>
        /// Creates the specified file or directory if either does not exist.
        /// If the file/directory already exists, it will be used directly.
        /// </remarks>
        protected internal FileRepository(string fileDirectory, string fileName, ISerializer<T> serializer)
        {
            if (string.IsNullOrWhiteSpace(fileDirectory)
                || string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentException("Either file name or file directory name is null or empty.");
            }

            if (!Directory.Exists(fileDirectory))
            {
                Directory.CreateDirectory(fileDirectory);
            }

            this._filePath = Path.Combine(fileDirectory, fileName);
            this._serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));

            if (!File.Exists(this._filePath))
            {
                File.WriteAllText(this._filePath, string.Empty);
            }

            this._items = this.LoadAll().ToList();
        }

        /// <summary>
        /// Gets all the contents of the file repository.
        /// </summary>
        /// <returns>The contents of the file repository.</returns>
        public IEnumerable<T> GetAll()
        {
            return this._items;
        }

        /// <summary>
        /// Gets a singular item from the file repository using its ID for retrieval.
        /// The file is read, and each line is deserialized as an item. The item with the matching ID is returned.
        /// </summary>
        /// <param name="id">The ID of the item to be retrieved.</param>
        /// <returns>The item from the repository, if found.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the GUID of the item doesn't exist.</exception>
        /// <exception cref="KeyNotFoundException">Thrown when an item with such an ID cannot be found.</exception>
        public T GetById(Guid id)
        {
            return this._items.FirstOrDefault(item => item.Id == id)
                ?? throw new KeyNotFoundException($"{typeof(T).Name} with ID {id} not found.");
        }

        /// <summary>
        /// Saves an item to the file repository by serializing it and appending it to the file.
        /// </summary>
        /// <param name="item">The item to be serialized and added.</param>
        /// <exception cref="ArgumentNullException">Thrown when the item to be added is null.</exception>
        public void Save(T item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            this._items.Add(item);

            File.AppendAllLines(
                this._filePath,
                new[] { this._serializer.Serialize(item) });
        }

        /// <summary>
        /// Loads all items from the file into the repository.
        /// The file is read, and each line is deserialized as an item.
        /// </summary>
        /// <returns>List of all items in the file.</returns>
        private IEnumerable<T> LoadAll()
        {
            return File.ReadAllLines(this._filePath)
                        .Where(line => !string.IsNullOrWhiteSpace(line))
                        .Select(this._serializer.Deserialize)
                        .ToList();
        }
    }
}