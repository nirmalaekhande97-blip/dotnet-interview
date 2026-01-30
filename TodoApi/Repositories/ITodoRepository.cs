using TodoApi.Models;

namespace TodoApi.Repositories
{
    /// <summary>
    /// Repository interface for TODO data access operations
    /// Provides abstraction over data storage implementation
    /// </summary>
    public interface ITodoRepository
    {
        /// <summary>
        /// Creates a new TODO item in the data store
        /// </summary>
        Task<Todo> CreateAsync(Todo todo);

        /// <summary>
        /// Retrieves all TODO items from the data store
        /// </summary>
        Task<IEnumerable<Todo>> GetAllAsync();

        /// <summary>
        /// Retrieves a specific TODO item by its ID
        /// </summary>
        Task<Todo?> GetByIdAsync(int id);

        /// <summary>
        /// Updates an existing TODO item
        /// </summary>
        Task<Todo?> UpdateAsync(int id, Todo todo);

        /// <summary>
        /// Deletes a TODO item by its ID
        /// </summary>
        Task<bool> DeleteAsync(int id);
    }
}
