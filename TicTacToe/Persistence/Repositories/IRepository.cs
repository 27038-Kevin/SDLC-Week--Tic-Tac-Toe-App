using TicTacToe.Models.Data;

namespace TicTacToe.Persistence.Repositories
{
    /// <summary>
    /// Abstracts file operations to aggregate data models and manage CRUD operations during runtime.
    /// Only supports Create and Read functions for now, due to the limited scope for Updating/Deletion in the application.
    /// </summary>
    /// <typeparam name="T">The type to which the repository corresponds.</typeparam>
    internal interface IRepository<T>
        where T : IHasId
    {
        /// <summary>
        /// Gets all entities present in the repository.
        /// </summary>
        /// <returns>A lazy list of all items in the repository.</returns>
        IEnumerable<T> GetAll();

        /// <summary>
        /// Gets a member from the repository using its GUID.
        /// </summary>
        /// <param name="id">The GUID to search by.</param>
        /// <returns>The member from the repository, if found.</returns>
        T GetById(Guid id);

        /// <summary>
        /// Saves a new member to the repository.
        /// </summary>
        /// <param name="entity">The new item to save in the repository.</param>
        void Save(T entity);
    }
}
