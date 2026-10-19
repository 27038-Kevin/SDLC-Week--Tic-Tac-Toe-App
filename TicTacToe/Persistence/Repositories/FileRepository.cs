using System.Reflection;
using System.Runtime.CompilerServices;
using TicTacToe.Models.Data;
using TicTacToe.Persistence.Serializers;

namespace TicTacToe.Persistence.Repositories
{
    /// <summary>
    /// Provides a base class for managing file-based storage and retrieval of objects of type T using serialization.
    /// </summary>
    /// <typeparam name="T">The type of objects managed by the repository. Must be a class with a parameterless constructor.</typeparam>
    internal abstract class FileRepository<T>
            where T : IHasId, new()
    {
        private protected readonly string _filePath;
        private protected readonly ISerializer<T> _serializer;

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
        /// Adds an item to the file repository by serializing it and appending it to the file.
        /// </summary>
        /// <param name="item">The item to be serialized and added.</param>
        /// <exception cref="ArgumentNullException">Thrown when the item to be added is null.</exception>
        protected internal void Add(T item)
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
        /// Adds a range of items to the file repository by serializing them and appending them to the file.
        /// </summary>
        /// <param name="items">The items to be serialized and added.</param>
        /// <exception cref="ArgumentNullException">Thrown when the range of items is null.</exception>
        protected internal void Add(IEnumerable<T> items)
        {
            if (items is null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            this._items.AddRange(items);

            File.AppendAllLines(
                this._filePath,
                items.Select(this._serializer.Serialize));
        }

        /// <summary>
        /// Updates an item from the file repository using its ID for retrieval, and a list of properties to update.
        /// The properties are updated in the existing item, and the entire collection is saved back to the file.
        /// </summary>
        /// <param name="updatedItem">Placeholder containing updated item details.</param>
        /// <exception cref="ArgumentException">Thrown when the property list does not contain the necessary argument to update.</exception>
        /// <exception cref="ArgumentNullException">Thrown when the parameter passed to update the repository is null.</exception>
        /// <exception cref="KeyNotFoundException">Thrown if the item's ID cannot be found.</exception>
        protected internal void Update(T updatedItem)
        {
            ArgumentNullException.ThrowIfNull(updatedItem);
            T itemToUpdate = this.GetById(updatedItem.Id);    // Throws KeyNotFoundException

            var propertiesToUpdate = updatedItem
                    .GetType()
                    .GetProperties()
                    .Where(property => property.SetMethod is not null
                                    && property.CanWrite
                                    && !property.SetMethod.ReturnParameter
                                            .GetRequiredCustomModifiers()
                                            .Contains(typeof(IsExternalInit)));

            foreach (PropertyInfo property in propertiesToUpdate)
            {
                property.SetValue(
                    itemToUpdate,
                    property.GetValue(updatedItem));
            }

            this.SaveAll();
        }

        /// <summary>
        /// Deletes an item from the file repository using its ID for retrieval.
        /// The item is removed from the collection, and the entire collection is saved back to the file.
        /// </summary>
        /// <param name="id">The ID of the item to be deleted.</param>
        /// <exception cref="KeyNotFoundException">Thrown if the item's ID cannot be found.</exception>
        protected internal void Delete(Guid id)
        {
            T itemToDelete = this.GetById(id);    // Throws KeyNotFoundException
            this._items.Remove(itemToDelete);
            this.SaveAll();
        }

        /// <summary>
        /// Gets a singular item from the file repository using its ID for retrieval.
        /// The file is read, and each line is deserialized as an item. The item with the matching ID is returned.
        /// </summary>
        /// <param name="id">The ID of the item to be retrieved.</param>
        /// <returns>The item from the repository, if found.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the GUID of the item doesn't exist.</exception>
        /// <exception cref="KeyNotFoundException">Thrown when an item with such an ID cannot be found.</exception>
        protected internal T GetById(Guid id)
        {
            return this._items.FirstOrDefault(item => item.Id == id)
                ?? throw new KeyNotFoundException($"{typeof(T).Name} with ID {id} not found.");
        }

        /// <summary>
        /// Gets all the contents of the file repository.
        /// </summary>
        /// <returns>The contents of the file repository.</returns>
        private protected IEnumerable<T> GetAll()
        {
            return this._items;
        }

        /// <summary>
        /// Saves all items to the file repository by serializing each item and writing them to the file.
        /// </summary>
        /// <param name="items">The list of items to be saved.</param>
        /// <exception cref="ArgumentNullException">Thrown when the list of items is null.</exception>
        private protected void SaveAll()
        {
            if (this._items is null)
            {
                throw new ArgumentNullException(nameof(this._items));
            }

            File.WriteAllLines(
                this._filePath,
                this._items.Select(this._serializer.Serialize));
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